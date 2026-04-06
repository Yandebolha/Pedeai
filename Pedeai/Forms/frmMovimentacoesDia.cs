using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    public partial class frmMovimentacoesDia : Form
    {
        // Parameterless constructor required by the WinForms Designer
        public frmMovimentacoesDia()
        {
            InitializeComponent();
        }

        public frmMovimentacoesDia(DateTime dia, DataTable dt, Func<int, DataTable> getItens = null)
        {
            InitializeComponent();

            Text      = $"Movimenta\u00e7\u00f5es \u2014 {dia:dd/MM/yyyy}";
            lblTit.Text = $"\U0001F4CB  Movimenta\u00e7\u00f5es do dia {dia:dd/MM/yyyy}";

            // Resumo
            decimal totalVendas = 0, totalCompras = 0;
            if (dt != null)
                foreach (DataRow r in dt.Rows)
                {
                    if (r["Valor"] != DBNull.Value)
                    {
                        var v = Convert.ToDecimal(r["Valor"]);
                        if (v >= 0) totalVendas  += v;
                        else        totalCompras += Math.Abs(v);
                    }
                }
            decimal saldo = totalVendas - totalCompras;
            lblRes.Text = $"  Vendas: R$ {totalVendas:N2}   |   Compras: R$ {totalCompras:N2}   |   " +
                          $"Saldo do dia: R$ {saldo:N2}";

            // Bind data
            if (dt != null)
            {
                grid.DataSource = dt;
                ConfigurarColunas(grid);
                ColorirLinhas(grid);
            }

            if (getItens != null)
            {
                grid.Cursor = Cursors.Hand;
                grid.CellDoubleClick += (_, e) =>
                {
                    if (e.RowIndex < 0) return;
                    var dr = ((DataRowView)grid.Rows[e.RowIndex].DataBoundItem)?.Row;
                    if (dr == null || dr["Tipo"]?.ToString() != "Venda") return;
                    int cod = dr["CodigoPedido"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CodigoPedido"]);
                    if (cod <= 0) return;
                    try
                    {
                        var dtItens = getItens(cod);
                        MostrarItensDialog(
                            dr["Referencia"]?.ToString() ?? "",
                            dr["Descricao"]?.ToString() ?? "",
                            dtItens);
                    }
                    catch (Exception ex) { MessageBox.Show("Erro ao carregar itens: " + ex.Message); }
                };
            }
        }

        private void PnlTop_SizeChanged(object sender, EventArgs e)
        {
            lblTit.Left = (pnlTop.Width - lblTit.Width) / 2;
        }

        private void PnlFoot_SizeChanged(object sender, EventArgs e)
        {
            btnFech.Left = (pnlFoot.Width - btnFech.Width) / 2;
        }

        private void BtnFech_Click(object sender, EventArgs e) => Close();

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void Grid_RowColor(object sender, DataGridViewRowPrePaintEventArgs e) { }

        private void ConfigurarColunas(DataGridView g)
        {
            if (g.Columns.Count == 0) return;
            var show = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Horario"]    = "Hor\u00e1rio",
                ["Tipo"]       = "Tipo",
                ["Referencia"] = "Refer\u00eancia",
                ["Descricao"]  = "Descri\u00e7\u00e3o",
                ["Valor"]      = "Valor R$",
                ["Pagamento"]  = "Pagamento",
                ["Status"]     = "Status",
            };
            foreach (DataGridViewColumn col in g.Columns)
                col.Visible = show.ContainsKey(col.Name);
            foreach (var kv in show)
                if (g.Columns.Contains(kv.Key))
                    g.Columns[kv.Key].HeaderText = kv.Value;
        }

        private static void MostrarItensDialog(string numPedido, string cliente, DataTable dtItens)
        {
            var corFundo  = Color.FromArgb(15, 22, 45);
            var corCard   = Color.FromArgb(28, 37, 65);
            var corTopBar = Color.FromArgb(36, 48, 82);

            using var frm = new Form();
            frm.Text             = $"Itens \u2014 Pedido {numPedido}";
            frm.BackColor        = corFundo;
            frm.ForeColor        = Color.White;
            frm.Font             = new Font("Segoe UI", 9F);
            frm.ClientSize       = new Size(720, 420);
            frm.StartPosition    = FormStartPosition.CenterParent;
            frm.FormBorderStyle  = FormBorderStyle.FixedDialog;
            frm.MaximizeBox      = frm.MinimizeBox = false;

            var pnlTop2 = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = corTopBar };
            var lblTit2 = new Label
            {
                Text      = $"Pedido {numPedido}  |  Cliente: {cliente}",
                AutoSize  = true, Top = 12, Left = 12,
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White
            };
            pnlTop2.Controls.Add(lblTit2);

            var grid2 = new DataGridView
            {
                Dock                      = DockStyle.Fill,
                ReadOnly                  = true,
                AllowUserToAddRows        = false,
                SelectionMode             = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible         = false,
                BackgroundColor           = corCard,
                DefaultCellStyle          = { BackColor = corCard, ForeColor = Color.White,
                                              SelectionBackColor = Color.FromArgb(52, 152, 219), SelectionForeColor = Color.White },
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(36, 48, 82) },
                ColumnHeadersDefaultCellStyle   = { BackColor = corTopBar, ForeColor = Color.White,
                                                    Font = new Font("Segoe UI", 9F, FontStyle.Bold) },
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight        = 36,
                RowTemplate                = { Height = 28 },
                GridColor                  = Color.FromArgb(50, 60, 100),
                BorderStyle                = BorderStyle.None,
                Font                       = new Font("Segoe UI", 9F),
                AutoSizeColumnsMode        = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid2.DataError += (_, e) => e.ThrowException = false;

            var pnlFoot2 = new Panel { Dock = DockStyle.Bottom, Height = 42, BackColor = corCard };
            var btnFech2 = new Button
            {
                Text      = "Fechar", Width = 100, Height = 28, Top = 7,
                BackColor = Color.FromArgb(80, 95, 130), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnFech2.FlatAppearance.BorderSize = 0;
            btnFech2.Click += (_, __) => frm.Close();
            pnlFoot2.SizeChanged += (_, __) => btnFech2.Left = (pnlFoot2.Width - btnFech2.Width) / 2;
            pnlFoot2.Controls.Add(btnFech2);

            frm.Controls.Add(grid2);
            frm.Controls.Add(pnlFoot2);
            frm.Controls.Add(pnlTop2);

            if (dtItens != null)
            {
                grid2.DataSource = dtItens;
                var captions = new System.Collections.Generic.Dictionary<string, string>
                {
                    ["Produto"]  = "Produto",
                    ["Qtde"]     = "Qtde",
                    ["Unitario"] = "Unit\u00e1rio (R$)",
                    ["Subtotal"] = "Subtotal (R$)",
                    ["Obs"]      = "Observa\u00e7\u00f5es",
                };
                foreach (var kv in captions)
                    if (grid2.Columns.Contains(kv.Key))
                        grid2.Columns[kv.Key].HeaderText = kv.Value;
            }

            frm.ShowDialog();
        }

        private void ColorirLinhas(DataGridView g)
        {
            foreach (DataGridViewRow row in g.Rows)
            {
                if (row.DataBoundItem == null) continue;
                var dr   = ((DataRowView)row.DataBoundItem).Row;
                var tipo = dr["Tipo"]?.ToString();
                if (tipo == "Compra")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 215);
                else
                    row.DefaultCellStyle.BackColor = Color.FromArgb(215, 245, 220);
            }
        }
    }
}
