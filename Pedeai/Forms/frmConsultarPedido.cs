using System;
using System.Windows.Forms;
using Pedeai.BLL;

namespace Pedeai.Forms
{
    public partial class frmConsultarPedido : Form
    {
        private readonly PedidoBLL _bll;

        public frmConsultarPedido()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new PedidoBLL();
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            BuscarPedido();
        }

        private void TxtNumPedido_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) BuscarPedido();
        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void BuscarPedido()
        {
            string numero = txtNumPedido.Text.Trim();
            if (string.IsNullOrEmpty(numero))
            {
                MessageBox.Show("Informe o n\u00famero do pedido.", "Consultar Pedido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNumPedido.Focus();
                return;
            }

            try
            {
                var pedido = _bll.PesquisaPorNumero(numero);
                if (pedido == null)
                {
                    LimparInfo();
                    MessageBox.Show($"Nenhum pedido encontrado com o n\u00famero '{numero}'.",
                        "Consultar Pedido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Cliente
                lblCliente.Text = pedido.pediNome_Cliente;

                // Taxa de entrega
                lblTaxaEnt.Text = pedido.pediTaxa_Entrega.ToString("C");

                // Condição de pagamento
                string condPgto;
                switch (pedido.pediForma_Pagamento)
                {
                    case 0:  condPgto = "Dinheiro"; break;
                    case 1:  condPgto = "Cart\u00e3o"; break;
                    case 2:  condPgto = "Pix"; break;
                    default: condPgto = "Outro"; break;
                }
                condPgto += pedido.pediTipo_Entrega == 0 ? " \u00b7 Retirada" : " \u00b7 Entrega";
                lblCondPgto.Text = condPgto;

                // Valor do pedido (total original)
                lblValorPedido.Text = pedido.pediValor_Total.ToString("C");

                // Valor pago (se NULL, assume o total)
                decimal valorPago = pedido.pediValor_Pago.HasValue
                    ? pedido.pediValor_Pago.Value
                    : pedido.pediValor_Total;
                lblValorPago.Text = valorPago.ToString("C");

                // Desconto: cupom + diferença de pagamento
                decimal descontoCupom = pedido.pediDesconto;
                decimal descontoPgto  = (pedido.pediValor_Pago.HasValue && pedido.pediValor_Pago.Value < pedido.pediValor_Total)
                    ? pedido.pediValor_Total - pedido.pediValor_Pago.Value
                    : 0m;
                decimal totalDesconto = descontoCupom + descontoPgto;

                if (totalDesconto > 0)
                {
                    string dText = totalDesconto.ToString("C");
                    if (!string.IsNullOrWhiteSpace(pedido.pediCodigo_Cupom))
                        dText += " (Cupom: " + pedido.pediCodigo_Cupom + ")";
                    lblDesconto.Text      = dText;
                    lblDesconto.ForeColor = System.Drawing.Color.FromArgb(192, 57, 43);
                }
                else
                {
                    lblDesconto.Text      = "\u2014";
                    lblDesconto.ForeColor = System.Drawing.Color.FromArgb(40, 30, 20);
                }

                // Usuário que autorizou desconto
                string aut = pedido.pediAutorizador ?? "";
                lblAutorizador.Text = string.IsNullOrWhiteSpace(aut) ? "\u2014" : aut;

                // Itens
                gridItens.DataSource = _bll.ListarItens(pedido.Codigo);
                ConfigurarGridItens();

                pnlInfo.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao buscar pedido: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimparInfo()
        {
            lblCliente.Text     = "\u2014";
            lblTaxaEnt.Text     = "\u2014";
            lblCondPgto.Text    = "\u2014";
            lblValorPedido.Text = "\u2014";
            lblValorPago.Text   = "\u2014";
            lblDesconto.Text    = "\u2014";
            lblAutorizador.Text = "\u2014";
            gridItens.DataSource = null;
            pnlInfo.Visible = false;
        }

        private void ConfigurarGridItens()
        {
            if (gridItens.Columns.Count == 0) return;
            if (gridItens.Columns["Produto"]  != null) { gridItens.Columns["Produto"].HeaderText  = "Produto";   gridItens.Columns["Produto"].FillWeight  = 40; }
            if (gridItens.Columns["Qtde"]     != null) { gridItens.Columns["Qtde"].HeaderText     = "Qtde";      gridItens.Columns["Qtde"].FillWeight     = 8; }
            if (gridItens.Columns["Unitario"] != null) { gridItens.Columns["Unitario"].HeaderText = "Unit. R$";  gridItens.Columns["Unitario"].FillWeight = 12; }
            if (gridItens.Columns["Subtotal"] != null) { gridItens.Columns["Subtotal"].HeaderText = "Subtotal";  gridItens.Columns["Subtotal"].FillWeight = 12; }
            if (gridItens.Columns["Obs"]      != null) { gridItens.Columns["Obs"].HeaderText      = "Obs.";      gridItens.Columns["Obs"].FillWeight      = 28; }
        }
    }
}
