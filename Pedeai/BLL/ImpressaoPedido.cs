using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    /// <summary>
    /// Servico de impressao do cupom do pedido.
    /// Gera duas vias (producao + entrega/cliente) na mesma impressao.
    /// </summary>
    public static class ImpressaoPedido
    {
        private static List<string> _linhas;
        private static int _linhaAtual;
        private static Font _fontNormal;
        private static Font _fontBold;
        private static Font _fontSmall;

        /// <summary>
        /// Imprime duas vias do pedido informado.
        /// Retorna string vazia em caso de sucesso ou mensagem de erro.
        /// </summary>
        public static string Imprimir(PedidoWeb pedido,
                                      List<ItemPedidoWeb> itens,
                                      ConfiguracaoImpressao cfg,
                                      Empresa empresa,
                                      string nomeAtendente = "")
        {
            try
            {
                int larg = cfg.larguraCaracteres > 0 ? cfg.larguraCaracteres : 42;

                // Monta as linhas para as duas vias — página separada para cada
                _linhas = new List<string>();
                _linhas.AddRange(GerarVia(pedido, itens, cfg, empresa, nomeAtendente, larg, "PRODUCAO"));
                _linhas.Add("§P§"); // quebra de página
                _linhas.AddRange(GerarVia(pedido, itens, cfg, empresa, nomeAtendente, larg, "ENTREGA"));
                _linhas.Add("");
                _linhas.Add("");
                _linhas.Add("");

                _linhaAtual = 0;

                using var doc = new PrintDocument();
                if (!string.IsNullOrWhiteSpace(cfg.impressoraNome))
                    doc.PrinterSettings.PrinterName = cfg.impressoraNome;

                _fontNormal = new Font("Courier New", 8.5f, FontStyle.Regular);
                _fontBold   = new Font("Courier New", 9f,   FontStyle.Bold);
                _fontSmall  = new Font("Courier New", 7.5f, FontStyle.Regular);

                doc.PrintPage += OnPrintPage;
                doc.Print();

                _fontNormal.Dispose();
                _fontBold.Dispose();
                _fontSmall.Dispose();

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static void OnPrintPage(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            float y       = e.MarginBounds.Top;
            float x       = e.MarginBounds.Left;
            float lineH   = _fontNormal.GetHeight(g) + 1.5f;

            while (_linhaAtual < _linhas.Count)
            {
                string linha = _linhas[_linhaAtual];

                // Quebra de página explícita
                if (linha == "§P§")
                {
                    _linhaAtual++;
                    e.HasMorePages = _linhaAtual < _linhas.Count;
                    return;
                }

                // Detectar linhas em negrito (marcadas com prefixo §B§)
                Font f;
                if (linha.StartsWith("§B§"))
                {
                    linha = linha.Substring(3);
                    f = _fontBold;
                }
                else if (linha.StartsWith("§S§"))
                {
                    linha = linha.Substring(3);
                    f = _fontSmall;
                }
                else
                {
                    f = _fontNormal;
                }

                g.DrawString(linha, f, Brushes.Black, x, y);
                y += f.GetHeight(g) + 1.5f;
                _linhaAtual++;

                if (y + lineH > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = _linhaAtual < _linhas.Count;
                    return;
                }
            }
            e.HasMorePages = false;
        }

        // ── Geracao do texto do cupom ────────────────────────────────────────

        private static List<string> GerarVia(PedidoWeb pedido,
                                             List<ItemPedidoWeb> itens,
                                             ConfiguracaoImpressao cfg,
                                             Empresa empresa,
                                             string atendente,
                                             int larg,
                                             string via)
        {
            char sep = cfg.separador.Length > 0 ? cfg.separador[0] : '-';
            string linha  = new string(sep, larg);
            var linhas = new List<string>();

            // ── Cabecalho ────────────────────────────────────────────────
            string nomeCab = string.IsNullOrWhiteSpace(cfg.cabNomeEmpresa)
                             ? empresa.empNome_Fantasia.Or(empresa.empNome)
                             : cfg.cabNomeEmpresa;
            string endCab  = string.IsNullOrWhiteSpace(cfg.cabEndereco)  ? empresa.empEndereco : cfg.cabEndereco;
            string telCab  = string.IsNullOrWhiteSpace(cfg.cabTelefone)  ? empresa.empTelefone : cfg.cabTelefone;
            string cnpjCab = string.IsNullOrWhiteSpace(cfg.cabCNPJ)      ? empresa.empCNPJ     : cfg.cabCNPJ;

            linhas.Add("§B§" + Centralizar(nomeCab, larg));
            if (!string.IsNullOrWhiteSpace(endCab))  linhas.Add(Centralizar(endCab, larg));
            if (!string.IsNullOrWhiteSpace(telCab))  linhas.Add(Centralizar(telCab, larg));
            if (!string.IsNullOrWhiteSpace(cnpjCab)) linhas.Add("§S§CNPJ: " + cnpjCab);
            linhas.Add(linha);

            // ── Data/hora impressao ──────────────────────────────────────
            linhas.Add("IMPRESSO EM " + DateTime.Now.ToString("dd/MM/yy HH:mm"));
            linhas.Add("");

            // ── Aviso fiscal ─────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(cfg.rodapeAvisoFiscal))
                linhas.Add(Centralizar(cfg.rodapeAvisoFiscal, larg));
            linhas.Add(linha);

            // ── Dados do cliente ─────────────────────────────────────────
            linhas.Add("§B§" + (pedido.pediNome_Cliente ?? "").ToUpper());
            if (!string.IsNullOrWhiteSpace(pedido.pediTelefone_Cliente))
                linhas.Add(pedido.pediTelefone_Cliente);

            bool entrega = pedido.pediTipo_Entrega == 1;
            if (entrega)
            {
                linhas.Add("(Entregar no endereco)");
                if (!string.IsNullOrWhiteSpace(pedido.pediEndereco_Entrega))
                    linhas.Add(pedido.pediEndereco_Entrega);
            }
            else
            {
                linhas.Add("(Retirada no balcao)");
            }

            linhas.Add("");
            linhas.Add("ABERTO EM " + pedido.pediData_Lancamento.ToString("dd/MM/yyyy HH:mm"));
            linhas.Add("");

            // ── Numero do pedido ─────────────────────────────────────────
            linhas.Add("   (" + cfg.lblNumeroPedido + " " + pedido.pediNumero + ")");
            linhas.Add("");

            // ── Itens ────────────────────────────────────────────────────
            string hdrEsq = cfg.lblColunaItem;
            string hdrDir = cfg.lblColunaTotal;
            linhas.Add("§B§" + hdrEsq + PreencharDir(hdrDir, larg - hdrEsq.Length));

            foreach (var item in itens)
            {
                string desc     = $"{item.itpwQtde:0.##} {item.itpwNome_Mercadoria} ({item.itpwPreco_Unitario:0.00})";
                string totalStr = item.itpwSubtotal.ToString("0.00");
                if (desc.Length + totalStr.Length + 1 <= larg)
                    linhas.Add(desc + PreencharDir(totalStr, larg - desc.Length));
                else
                    linhas.Add(desc);

                if (!string.IsNullOrWhiteSpace(item.itpwObservacoes))
                    linhas.Add("§S§  Obs: " + item.itpwObservacoes);
            }

            linhas.Add(linha);

            // ── Totais ───────────────────────────────────────────────────
            string sub  = pedido.pediSubtotal.ToString("0.00");
            string taxa = pedido.pediTaxa_Entrega.ToString("0.00");
            string tot  = pedido.pediValor_Total.ToString("0.00");

            linhas.Add(cfg.lblSubtotal + PreencharDir(sub, larg - cfg.lblSubtotal.Length));
            if (pedido.pediDesconto > 0)
            {
                string valDesc = pedido.pediDesconto.ToString("0.00");
                bool ehCupom = !string.IsNullOrWhiteSpace(pedido.pediCodigo_Cupom);
                string lblDesc;
                if (ehCupom)
                {
                    string lblBase = string.IsNullOrWhiteSpace(cfg.lblCupom) ? "- CUPOM:" : cfg.lblCupom;
                    lblDesc = lblBase + " " + pedido.pediCodigo_Cupom;
                }
                else
                {
                    lblDesc = string.IsNullOrWhiteSpace(cfg.lblDesconto) ? "- DESCONTO:" : cfg.lblDesconto;
                }
                if (lblDesc.Length + valDesc.Length + 1 > larg)
                    lblDesc = (ehCupom ? "- CUPOM:" : "- DESCONTO:");
                linhas.Add(lblDesc + PreencharDir(valDesc, larg - lblDesc.Length));
            }
            if (entrega)
                linhas.Add(cfg.lblTaxaEntrega + PreencharDir(taxa, larg - cfg.lblTaxaEntrega.Length));
            linhas.Add("§B§" + cfg.lblTotalPagar + PreencharDir(tot, larg - cfg.lblTotalPagar.Length));
            linhas.Add("");

            // ── Atendente ────────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(atendente))
                linhas.Add(cfg.lblAtendente + " " + atendente);

            // ── Observacao do pedido ─────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(pedido.pediObservacoes))
            {
                linhas.Add("Obs: " + pedido.pediObservacoes);
            }

            linhas.Add("");
            linhas.Add(Centralizar("***", larg));
            linhas.Add("");

            // ── Rodape livre ─────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(cfg.rodapeTextoLivre))
            {
                foreach (var l in cfg.rodapeTextoLivre.Split('\n'))
                    linhas.Add(Centralizar(l.Trim(), larg));
            }

            // ── Via ──────────────────────────────────────────────────────
            linhas.Add("");
            linhas.Add("§S§" + Centralizar("VIA: " + via, larg));

            return linhas;
        }

        // ── Helpers de formatacao ────────────────────────────────────────────

        private static string Centralizar(string texto, int larg)
        {
            if (texto.Length >= larg) return texto;
            int pad = (larg - texto.Length) / 2;
            return new string(' ', pad) + texto;
        }

        private static string PreencharDir(string texto, int espacos)
        {
            if (espacos <= 0) return " " + texto;
            return new string(' ', espacos) + texto;
        }
    }

    // Extensao auxiliar local
    internal static class StringExtensions
    {
        public static string Or(this string s, string fallback)
            => string.IsNullOrWhiteSpace(s) ? fallback : s;
    }
}
