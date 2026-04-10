using System;
using System.Data;
using System.Diagnostics;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class FidelizacaoBLL
    {
        private readonly FidelizacaoDAL _dal = new FidelizacaoDAL();

        public ConfigFidelizacao Carregar(int codigo = 1) => _dal.Carregar(codigo);

        public DataTable Listar() => _dal.Listar();

        public string Incluir(ConfigFidelizacao cfg)
        {
            if (cfg == null) return "Configuração inválida.";
            if (string.IsNullOrWhiteSpace(cfg.fidNome)) return "Informe o nome da regra.";
            if (cfg.fidMeta_Gasto <= 0) return "Meta de gasto deve ser maior que zero.";
            if (cfg.fidPremio_Tipo == "CUPOM" && cfg.fidCupom_Valor <= 0)
                return "Valor do cupom deve ser maior que zero.";
            if (cfg.fidPremio_Tipo == "PRODUTO" && string.IsNullOrWhiteSpace(cfg.fidProduto_Nome))
                return "Informe o produto prêmio.";
            return _dal.Incluir(cfg);
        }

        public string Alterar(ConfigFidelizacao cfg)
        {
            if (cfg == null) return "Configuração inválida.";
            if (cfg.Codigo <= 0) return "Código inválido.";
            if (string.IsNullOrWhiteSpace(cfg.fidNome)) return "Informe o nome da regra.";
            if (cfg.fidMeta_Gasto <= 0) return "Meta de gasto deve ser maior que zero.";
            if (cfg.fidPremio_Tipo == "CUPOM" && cfg.fidCupom_Valor <= 0)
                return "Valor do cupom deve ser maior que zero.";
            if (cfg.fidPremio_Tipo == "PRODUTO" && string.IsNullOrWhiteSpace(cfg.fidProduto_Nome))
                return "Informe o produto prêmio.";
            return _dal.Alterar(cfg);
        }

        public string Excluir(int codigo) => _dal.Excluir(codigo);

        [Obsolete("Use Incluir or Alterar instead")]
        public string Salvar(ConfigFidelizacao cfg) => cfg.Codigo == 0 ? Incluir(cfg) : Alterar(cfg);

        public DataTable ListarHistorico(DateTime de, DateTime ate)
            => _dal.ListarHistorico(de, ate);

        /// <summary>Retorna o primeiro cupom de fidelização disponível para o cliente, ou null.</summary>
        public Modelo.Cupom BuscarCupomDisponivel(int codigoCliente)
            => _dal.BuscarCupomDisponivel(codigoCliente);

        /// <summary>Remove a chamada ao WhatsApp — mantida apenas para não quebrar chamadas externas.</summary>
        [Obsolete("WhatsApp automático desativado.")]
        public void AbrirWhatsAppManual(int codigoCliente, int codigoConfig) { }

        /// <summary>
        /// Verifica se o cliente atingiu um novo patamar em qualquer regra ativa e, se sim, emite o prêmio.
        /// Retorna mensagem para exibir ao operador, ou string vazia se nada foi feito.
        /// </summary>
        public string VerificarEDispararPremio(int codigoCliente, int codigoPedido)
        {
            try
            {
                var clienteDal = new ClienteDAL();
                var cliente = clienteDal.PesquisaCodigo(codigoCliente);
                if (cliente == null) return "";

                var configs = _dal.Listar();
                var mensagens = new System.Text.StringBuilder();

                foreach (DataRow row in configs.Rows)
                {
                    bool ativo = row["Ativo"] != DBNull.Value && Convert.ToBoolean(row["Ativo"]);
                    if (!ativo) continue;

                    int codigoConfig = Convert.ToInt32(row["Codigo"]);
                    var cfg = _dal.Carregar(codigoConfig);
                    if (cfg.fidMeta_Gasto <= 0) continue;

                    // Usa gasto mensal para fidelização — 1 cupom por mês quando bate a meta
                    decimal gastoMes = cliente.clieGasto_Mensal;
                    if (gastoMes < cfg.fidMeta_Gasto) continue;

                    int jaDeuMes = _dal.ContarPremiosEnviadosMes(
                        codigoCliente, codigoConfig, DateTime.Today.Year, DateTime.Today.Month);
                    if (jaDeuMes > 0) continue; // já recebeu prêmio este mês

                    string descricao;
                    string cupomGerado = "";

                    if (cfg.fidPremio_Tipo == "PRODUTO")
                    {
                        cupomGerado = $"PROD:{cfg.fidProduto_Codigo}";
                        int qtde    = cfg.fidProduto_Qtde > 0 ? cfg.fidProduto_Qtde : 1;
                        descricao   = $"Prêmio produto: {qtde}x {cfg.fidProduto_Nome}";
                    }
                    else
                    {
                        cupomGerado = GerarCodigoCupom(cliente.clieNome_RazaoSocial);
                        _dal.CriarCupomFidelizacao(cupomGerado, cfg);
                        descricao = $"Cupom {cupomGerado} ({cfg.fidCupom_Tipo} {cfg.fidCupom_Valor:N2})";
                    }

                    string telefone = !string.IsNullOrWhiteSpace(cliente.clieCelular)
                        ? cliente.clieCelular : cliente.clieTelefone;

                    _dal.RegistrarHistorico(codigoCliente, codigoPedido, codigoConfig,
                                            cupomGerado, descricao, telefone);

                    // Notificar via WhatsApp se cupom foi gerado
                    if (!string.IsNullOrEmpty(cupomGerado) && !string.IsNullOrEmpty(telefone))
                    {
                        string validade = DateTime.Today
                            .AddDays(cfg.fidCupom_Validade > 0 ? cfg.fidCupom_Validade : 30)
                            .ToString("dd/MM/yyyy");
                        WhatsAppService.NotificarCupom(telefone, cliente.clieNome_RazaoSocial, cupomGerado, validade);
                    }

                    mensagens.AppendLine($"[{cfg.fidNome}] {descricao}");
                }

                string resultado = mensagens.ToString().Trim();
                return string.IsNullOrEmpty(resultado)
                    ? ""
                    : $"Fidelização: {cliente.clieNome_RazaoSocial} atingiu R$ {cliente.clieGasto_Mensal:N2} este mês. Prêmios emitidos:\n{resultado}";
            }
            catch (Exception ex)
            {
                return $"Fidelização (erro não crítico): {ex.Message}";
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────

        private static string GerarCodigoCupom(string nomeCliente)
        {
            string sufixo = "";
            if (!string.IsNullOrWhiteSpace(nomeCliente))
            {
                var partes = nomeCliente.Trim().ToUpperInvariant().Split(' ');
                sufixo = partes[0].Length > 4 ? partes[0].Substring(0, 4) : partes[0];
            }
            // Inclui segundos + sufixo aleatório para evitar duplicatas
            return $"FID{DateTime.Now:yyyyMMddHHmmss}{sufixo}";
        }

        /// <summary>Retorna o prêmio PRODUTO pendente mais recente do cliente, ou (0,0,"",0).</summary>
        public (int historicoCod, int codigoProduto, string nomeProduto, int qtde) BuscarPremioProdutoPendente(int codigoCliente)
            => _dal.BuscarPremioProdutoPendente(codigoCliente);

        /// <summary>Marca o prêmio PRODUTO como utilizado (fidCupomCodigo → 'PROD:OK').</summary>
        public void MarcarPremioProdutoUsado(int codigoHistorico)
            => _dal.MarcarPremioProdutoUsado(codigoHistorico);

        private static void AbrirWhatsApp(string telefone, ConfigFidelizacao cfg,
                                           string nome, string cupomCodigo,
                                           decimal totalGasto, decimal meta)
        {
            // Limpar telefone (manter só dígitos)
            string fone = System.Text.RegularExpressions.Regex.Replace(telefone ?? "", @"\D", "");
            if (string.IsNullOrEmpty(fone)) return;

            // Montar mensagem do template
            string msg = (cfg.fidMensagem ?? "")
                .Replace("{Nome}",        nome ?? "")
                .Replace("{Meta}",        meta.ToString("N2"))
                .Replace("{CupomCodigo}", cupomCodigo)
                .Replace("{Validade}",    DateTime.Today.AddDays(cfg.fidCupom_Validade > 0 ? cfg.fidCupom_Validade : 30).ToString("dd/MM/yyyy"))
                .Replace("{Produto}",     cfg.fidProduto_Nome ?? "")
                .Replace("{TotalGasto}",  totalGasto.ToString("N2"));

            string url = $"https://wa.me/55{fone}?text={Uri.EscapeDataString(msg)}";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
