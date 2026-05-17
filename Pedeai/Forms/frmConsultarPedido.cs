using System;
using System.Data;
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
            dtpDe.Value  = DateTime.Today.AddDays(-30);
            dtpAte.Value = DateTime.Today;
            this.Load += new EventHandler(Form_Load);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            CarregarLista();
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            CarregarLista();
        }

        private void BtnTodos_Click(object sender, EventArgs e)
        {
            dtpDe.Value        = DateTime.Today.AddDays(-30);
            dtpAte.Value       = DateTime.Today;
            txtCliente.Text    = "";
            txtNumPedido.Text  = "";
            CarregarLista();
        }

        private void TxtNumPedido_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) CarregarLista();
        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void GridPedidos_SelectionChanged(object sender, EventArgs e)
        {
            if (gridPedidos.SelectedRows.Count == 0) { LimparInfo(); return; }
            var row = gridPedidos.SelectedRows[0];
            if (row.Cells["Codigo"]?.Value == null || row.Cells["Codigo"].Value == DBNull.Value) { LimparInfo(); return; }
            int cod = Convert.ToInt32(row.Cells["Codigo"].Value);
            BuscarDetalhes(cod);
        }

        private void CarregarLista()
        {
            LimparInfo();
            try
            {
                var dt = _bll.ListarConsulta(dtpDe.Value.Date, dtpAte.Value.Date,
                    txtCliente.Text.Trim(), txtNumPedido.Text.Trim());
                gridPedidos.DataSource = dt;
                ConfigurarGridPedidos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar pedidos: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarGridPedidos()
        {
            if (gridPedidos.Columns.Count == 0) return;
            if (gridPedidos.Columns["Codigo"]   != null) { gridPedidos.Columns["Codigo"].Visible    = false; }
            if (gridPedidos.Columns["Telefone"] != null) { gridPedidos.Columns["Telefone"].Visible  = false; }
            if (gridPedidos.Columns["Numero"]   != null) { gridPedidos.Columns["Numero"].HeaderText   = "N\u00ba";       gridPedidos.Columns["Numero"].FillWeight   = 8; }
            if (gridPedidos.Columns["Cliente"]  != null) { gridPedidos.Columns["Cliente"].HeaderText  = "Cliente";    gridPedidos.Columns["Cliente"].FillWeight  = 32; }
            if (gridPedidos.Columns["Status"]   != null) { gridPedidos.Columns["Status"].HeaderText   = "Status";     gridPedidos.Columns["Status"].FillWeight   = 16; }
            if (gridPedidos.Columns["Total"]    != null) { gridPedidos.Columns["Total"].HeaderText    = "Total R$";   gridPedidos.Columns["Total"].FillWeight    = 12;
                                                            gridPedidos.Columns["Total"].DefaultCellStyle.Format = "C2"; }
            if (gridPedidos.Columns["DataHora"] != null) { gridPedidos.Columns["DataHora"].HeaderText = "Data/Hora";  gridPedidos.Columns["DataHora"].FillWeight = 18;
                                                            gridPedidos.Columns["DataHora"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"; }
        }

        private void BuscarDetalhes(int codigoPedido)
        {
            try
            {
                var pedido = _bll.PesquisaCodigo(codigoPedido);
                if (pedido == null) { LimparInfo(); return; }

                // Cliente
                lblCliente.Text = pedido.pediNome_Cliente;

                // Taxa de entrega
                lblTaxaEnt.Text = pedido.pediTaxa_Entrega.ToString("C");

                // Condição de pagamento
                var formasPgto = new System.Collections.Generic.List<string>();
                if (pedido.pediPago_Dinheiro > 0) formasPgto.Add($"Dinheiro: {pedido.pediPago_Dinheiro:C}");
                if (pedido.pediPago_Cartao   > 0) formasPgto.Add($"Cr\u00e9dito: {pedido.pediPago_Cartao:C}");
                if (pedido.pediPago_CartaoDebito > 0) formasPgto.Add($"D\u00e9bito: {pedido.pediPago_CartaoDebito:C}");
                if (pedido.pediPago_Pix      > 0) formasPgto.Add($"Pix: {pedido.pediPago_Pix:C}");
                string condPgto = formasPgto.Count > 0
                    ? string.Join(" + ", formasPgto)
                    : pedido.pediForma_Pagamento switch
                      {
                          0 => "Dinheiro",
                          1 => "Cart\u00e3o Cr\u00e9dito",
                          2 => "Pix",
                          3 => "Cart\u00e3o D\u00e9bito",
                          _ => "Outro"
                      };
                condPgto += pedido.pediTipo_Entrega == 0 ? " \u00b7 Retirada" : " \u00b7 Entrega";
                lblCondPgto.Text = condPgto;

                // Valor do pedido (total original)
                lblValorPedido.Text = pedido.pediValor_Total.ToString("C");

                // Valor pago (se NULL, assume o total)
                decimal valorPago = pedido.pediValor_Pago.HasValue
                    ? pedido.pediValor_Pago.Value
                    : pedido.pediValor_Total;
                lblValorPago.Text = valorPago.ToString("C");

                // Desconto: pediDesconto já contém tanto o desconto de cupom quanto o
                // desconto de pagamento autorizado (gravado por FinalizarPedido).
                // Não somar novamente pediValor_Total - pediValor_Pago para evitar dupla contagem.
                decimal descontoCupom = pedido.pediDesconto;
                decimal descontoPgto  = 0m;   // já está incluido em pediDesconto

                // Somar descontos por item (itpwPreco_Unitario * qtde - subtotal)
                decimal descontoItens = 0m;
                try
                {
                    var itensDT = _bll.ListarItens(pedido.Codigo);
                    foreach (System.Data.DataRow ri in itensDT.Rows)
                    {
                        decimal dpct = ri["Desconto"] == System.DBNull.Value ? 0m : Convert.ToDecimal(ri["Desconto"]);
                        if (dpct > 0)
                        {
                            decimal unit = Convert.ToDecimal(ri["Unitario"]);
                            decimal qtde = Convert.ToDecimal(ri["Qtde"]);
                            decimal sub  = Convert.ToDecimal(ri["Subtotal"]);
                            descontoItens += unit * qtde - sub;
                        }
                    }
                }
                catch { }

                decimal totalDesconto = descontoCupom + descontoPgto + descontoItens;

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

                // Itens — carregar e acrescentar taxa de entrega como linha
                var dtItensConsulta = _bll.ListarItens(pedido.Codigo);
                if (pedido.pediTaxa_Entrega > 0 && dtItensConsulta != null)
                {
                    var rowEnt = dtItensConsulta.NewRow();
                    rowEnt["Produto"]  = "\U0001F69A Taxa de Entrega";
                    rowEnt["Qtde"]     = 1m;
                    rowEnt["Unitario"] = pedido.pediTaxa_Entrega;
                    if (dtItensConsulta.Columns.Contains("Desconto")) rowEnt["Desconto"] = 0m;
                    rowEnt["Subtotal"] = pedido.pediTaxa_Entrega;
                    if (dtItensConsulta.Columns.Contains("Obs"))      rowEnt["Obs"]      = "";
                    dtItensConsulta.Rows.Add(rowEnt);
                }
                gridItens.DataSource = dtItensConsulta;
                ConfigurarGridItens();

                // Título dos itens
                string sitTxt = pedido.pediSituacao switch
                {
                    0 => "Pendente", 1 => "Confirmado", 2 => "Em Preparo",
                    3 => "Pronto", 4 => "Saiu p/ Entrega", 5 => "Entregue",
                    6 => "Cancelado", _ => pedido.pediSituacao.ToString()
                };
                lblItensTitle.Text = $"Itens - Pedido #{pedido.pediNumero}  |  \u2713 {sitTxt}  |  Total: R$ {pedido.pediValor_Total:N2}";

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
            if (gridItens.Columns["Desconto"] != null) { gridItens.Columns["Desconto"].HeaderText = "Desc.%";    gridItens.Columns["Desconto"].FillWeight = 8; }
            if (gridItens.Columns["Subtotal"] != null) { gridItens.Columns["Subtotal"].HeaderText = "Subtotal";  gridItens.Columns["Subtotal"].FillWeight = 12; }
            if (gridItens.Columns["Obs"]      != null) { gridItens.Columns["Obs"].HeaderText      = "Obs.";      gridItens.Columns["Obs"].FillWeight      = 20; }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
