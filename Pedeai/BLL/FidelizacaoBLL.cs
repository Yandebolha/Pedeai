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

        public ConfigFidelizacao Carregar() => _dal.Carregar();

        public string Salvar(ConfigFidelizacao cfg)
        {
            if (cfg == null) return "Configuração inválida.";
            if (cfg.fidMeta_Gasto <= 0) return "Meta de gasto deve ser maior que zero.";
            if (cfg.fidPremio_Tipo == "CUPOM" && cfg.fidCupom_Valor <= 0)
                return "Valor do cupom deve ser maior que zero.";
            if (cfg.fidPremio_Tipo == "PRODUTO" && string.IsNullOrWhiteSpace(cfg.fidProduto_Nome))
                return "Informe o produto prêmio.";
            return _dal.Salvar(cfg);
        }

        public DataTable ListarHistorico(DateTime de, DateTime ate)
            => _dal.ListarHistorico(de, ate);

        /// <summary>
        /// Verifica se o cliente atingiu um novo patamar de prêmio e, se sim, emite o prêmio.
        /// Retorna mensagem para exibir ao operador, ou string vazia se nada foi feito.
        /// </summary>
        public string VerificarEDispararPremio(int codigoCliente, int codigoPedido)
        {
            try
            {
                var cfg = _dal.Carregar();
                if (!cfg.fidAtivo || cfg.fidMeta_Gasto <= 0) return "";

                var clienteDal = new ClienteDAL();
                var cliente = clienteDal.PesquisaCodigo(codigoCliente);
                if (cliente == null) return "";

                int deveDar  = (int)(cliente.clieTotalGasto / cfg.fidMeta_Gasto);
                int jaDeu    = _dal.ContarPremiosEnviados(codigoCliente);
                if (deveDar <= jaDeu) return "";

                string descricao;
                string cupomGerado = "";

                if (cfg.fidPremio_Tipo == "PRODUTO")
                {
                    descricao = $"Prêmio produto: {cfg.fidProduto_Nome}";
                }
                else
                {
                    // Gerar código de cupom único
                    cupomGerado = GerarCodigoCupom(cliente.clieNome_RazaoSocial);
                    _dal.CriarCupomFidelizacao(cupomGerado, cfg);
                    descricao = $"Cupom {cupomGerado} ({cfg.fidCupom_Tipo} {cfg.fidCupom_Valor:N2})";
                }

                // Obter telefone preferencial
                string telefone = !string.IsNullOrWhiteSpace(cliente.clieCelular)
                    ? cliente.clieCelular
                    : cliente.clieTelefone;
                AbrirWhatsApp(telefone, cfg, cliente.clieNome_RazaoSocial, cupomGerado,
                              cliente.clieTotalGasto, cfg.fidMeta_Gasto);

                _dal.RegistrarHistorico(codigoCliente, codigoPedido, cupomGerado, descricao, telefone);

                return $"Fidelização: {cliente.clieNome_RazaoSocial} atingiu R$ {cliente.clieTotalGasto:N2} em compras. {descricao} enviado!";
            }
            catch (Exception ex)
            {
                return $"Fidelização (erro não crítico): {ex.Message}";
            }
        }

        // ─────────────────────────────────────────────────────────────────────

        private static string GerarCodigoCupom(string nomeCliente)
        {
            string sufixo = "";
            if (!string.IsNullOrWhiteSpace(nomeCliente))
            {
                var partes = nomeCliente.Trim().ToUpperInvariant().Split(' ');
                sufixo = partes[0].Length > 4 ? partes[0].Substring(0, 4) : partes[0];
            }
            return $"FID{DateTime.Now:yyyyMMddHHmm}{sufixo}";
        }

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
