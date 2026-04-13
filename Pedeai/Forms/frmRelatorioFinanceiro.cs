using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Pedeai.BLL;

namespace Pedeai.Forms
{
    public partial class frmRelatorioFinanceiro : Form
    {
        private PedidoBLL            _pedidoBLL;
        private GastoMaterialBLL     _gastosBLL;
        private EntradaMercadoriaBLL _entradaBLL;
        private DateTime _initDe;
        private DateTime _initAte;

        // Parameterless constructor required by the WinForms Designer
        public frmRelatorioFinanceiro()
        {
            InitializeComponent();
        }

        public frmRelatorioFinanceiro(PedidoBLL pedidoBLL, GastoMaterialBLL gastosBLL,
                                      EntradaMercadoriaBLL entradaBLL,
                                      DateTime? de = null, DateTime? ate = null)
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode ==
                System.ComponentModel.LicenseUsageMode.Designtime) return;
            _pedidoBLL  = pedidoBLL;
            _gastosBLL  = gastosBLL;
            _entradaBLL = entradaBLL;
            _initDe     = de  ?? DateTime.Today.AddMonths(-1);
            _initAte    = ate ?? DateTime.Today;
            Load += Form_Load;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            dtpDe.Value  = _initDe;
            dtpAte.Value = _initAte;
            Carregar();
        }

        private void BtnFiltrar_Click(object sender, EventArgs e) => Carregar();
        private void BtnImprimir_Click(object sender, EventArgs e) => Imprimir();
        private void BtnExportCsv_Click(object sender, EventArgs e) => ExportarCsv();

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
            => e.ThrowException = false;

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _pedidoBLL == null) return;
            var row = grid.Rows[e.RowIndex];
            if (row.DataBoundItem == null) return;
            var dr = ((DataRowView)row.DataBoundItem).Row;
            if (!dr.Table.Columns.Contains("CodigoPedido")) return;
            if (dr["Tipo"]?.ToString() != "Venda") return;
            int cod = dr["CodigoPedido"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CodigoPedido"]);
            if (cod <= 0) return;
            try
            {
                var dtItens = _pedidoBLL.ListarItens(cod);
                MostrarItensDialog(dr["Referencia"]?.ToString() ?? "", dr["Descricao"]?.ToString() ?? "", dtItens);
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar itens: " + ex.Message); }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Carregar()
        {
            if (_pedidoBLL == null) return;
            try
            {
                var de  = dtpDe.Value.Date;
                var ate = dtpAte.Value.Date;
                string tipo = cmbTipo.SelectedItem?.ToString() ?? "Ambos";

                var dt = _pedidoBLL.GetMovimentacoesPeriodo(de, ate);

                // Filter by type if needed
                if (tipo == "Sa\u00edda (Vendas)")
                {
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                        if (dt.Rows[i]["Tipo"]?.ToString() != "Venda") dt.Rows.RemoveAt(i);
                }
                else if (tipo == "Entrada (Compras/Gastos)")
                {
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                        if (dt.Rows[i]["Tipo"]?.ToString() != "Compra") dt.Rows.RemoveAt(i);
                }

                // Always append gasto_material as Compra rows when showing Entrada or Ambos
                if (tipo == "Entrada (Compras/Gastos)" || tipo == "Ambos")
                {
                    var dtGastos = _gastosBLL.Listar(de, ate);
                    foreach (DataRow r in dtGastos.Rows)
                    {
                        var nr = dt.NewRow();
                        nr["Horario"]      = r["Data"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDateTime(r["Data"]);
                        nr["Tipo"]         = "Compra";
                        nr["Referencia"]   = "Gasto";
                        nr["Descricao"]    = r["Descricao"]?.ToString() ?? "";
                        nr["Valor"]        = -(r["Valor"] == DBNull.Value ? 0m : Convert.ToDecimal(r["Valor"]));
                        nr["CodigoPedido"] = 0;
                        nr["Status"]       = "Lan\u00e7ado";
                        dt.Rows.Add(nr);
                    }
                }

                decimal totalSaida = 0m, totalEntrada = 0m, totalDescontos = 0m;
                foreach (DataRow r in dt.Rows)
                {
                    decimal v = r["Valor"] == DBNull.Value ? 0m : Convert.ToDecimal(r["Valor"]);
                    if (v >= 0) totalSaida   += v;
                    else        totalEntrada += Math.Abs(v);
                    if (dt.Columns.Contains("Desconto") && r["Desconto"] != DBNull.Value)
                        totalDescontos += Convert.ToDecimal(r["Desconto"]);
                }

                grid.DataSource = dt;
                ConfigurarColunas(dt);
                ColorirLinhas(dt);
                grid.Cursor = Cursors.Hand;

                lblTotal.Text = $"  Vendas: R$ {totalSaida:N2}   |   Compras: R$ {totalEntrada:N2}   |   " +
                                $"Saldo: R$ {(totalSaida - totalEntrada):N2}" +
                                (totalDescontos > 0 ? $"   |   \u2193 Descontos: R$ {totalDescontos:N2}" : "") +
                                "   \u2502 Duplo clique em venda para ver os itens";
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar: " + ex.Message); }
        }

        private void ConfigurarColunas(DataTable dt)
        {
            if (grid.Columns.Count == 0) return;
            bool temDesconto = false;
            foreach (DataRow r in dt.Rows)
                if (dt.Columns.Contains("Desconto") && r["Desconto"] != DBNull.Value
                    && Convert.ToDecimal(r["Desconto"]) > 0) { temDesconto = true; break; }

            var show = new System.Collections.Generic.Dictionary<string, (string header, int fill)>
            {
                ["Horario"]       = ("Hor\u00e1rio",      11),
                ["Tipo"]          = ("Tipo",               7),
                ["Referencia"]    = ("Refer\u00eancia",    8),
                ["Descricao"]     = ("Descri\u00e7\u00e3o", 28),
                ["Valor"]         = ("Valor R$",           10),
                ["ValorOriginal"] = ("Total Original",     11),
                ["ValorRecebido"] = ("Valor Recebido",     11),
                ["Desconto"]      = ("Desconto R$",         9),
                ["Autorizador"]   = ("Autorizado por",     10),
                ["Pagamento"]     = ("Pagamento",           9),
                ["Status"]        = ("Status",              8),
            };
            if (grid.Columns.Contains("CodigoPedido"))
                grid.Columns["CodigoPedido"].Visible = false;

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (!show.ContainsKey(col.Name)) { col.Visible = false; continue; }
                if ((col.Name == "ValorOriginal" || col.Name == "ValorRecebido" ||
                     col.Name == "Desconto"      || col.Name == "Autorizador") && !temDesconto)
                { col.Visible = false; continue; }
                col.Visible    = true;
                col.HeaderText = show[col.Name].header;
                col.FillWeight = show[col.Name].fill;
            }
            if (grid.Columns.Contains("Horario"))
                grid.Columns["Horario"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
        }

        private void ColorirLinhas(DataTable dt)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.DataBoundItem == null) continue;
                var dr   = ((DataRowView)row.DataBoundItem).Row;
                var tipo = dr["Tipo"]?.ToString();
                bool temDesconto = dt.Columns.Contains("Desconto")
                                   && dr["Desconto"] != DBNull.Value
                                   && Convert.ToDecimal(dr["Desconto"]) > 0;
                if (tipo == "Compra")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 215);
                else if (temDesconto)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 200);
                else if (tipo == "Venda")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(215, 245, 220);
            }
        }

        private static void MostrarItensDialog(string numPedido, string cliente, DataTable dtItens)
        {
            using var frm = new Form();
            frm.Text             = $"Itens \u2014 Pedido {numPedido}";
            frm.BackColor        = Color.FromArgb(248, 245, 240);
            frm.Font             = new Font("Segoe UI", 9F);
            frm.ClientSize       = new System.Drawing.Size(720, 420);
            frm.StartPosition    = FormStartPosition.CenterParent;
            frm.FormBorderStyle  = FormBorderStyle.FixedDialog;
            frm.MaximizeBox      = frm.MinimizeBox = false;

            var pTop = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Color.FromArgb(176, 110, 42) };
            pTop.Controls.Add(new Label
            {
                Text = $"Pedido {numPedido}  |  Cliente: {cliente}",
                AutoSize = true, Top = 12, Left = 12,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.White
            });

            var g2 = new DataGridView
            {
                Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false,
                BackgroundColor = Color.FromArgb(250, 246, 238), BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9F),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DefaultCellStyle          = { BackColor = Color.FromArgb(250, 246, 238), ForeColor = Color.FromArgb(50, 40, 25),
                                              SelectionBackColor = Color.FromArgb(224, 113, 42), SelectionForeColor = Color.White },
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(240, 234, 218) },
                ColumnHeadersDefaultCellStyle   = { BackColor = Color.FromArgb(176, 110, 42), ForeColor = Color.White,
                                                    Font = new Font("Segoe UI", 9F, FontStyle.Bold) },
            };
            g2.DataError += (_, ev) => ev.ThrowException = false;

            var pFoot = new Panel { Dock = DockStyle.Bottom, Height = 42, BackColor = Color.FromArgb(235, 228, 214) };
            var btnF  = new Button { Text = "Fechar", Width = 100, Height = 28, Top = 7,
                BackColor = Color.FromArgb(224, 113, 42), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, DialogResult = DialogResult.OK };
            btnF.FlatAppearance.BorderSize = 0;
            pFoot.SizeChanged += (_, __) => btnF.Left = (pFoot.Width - btnF.Width) / 2;
            pFoot.Controls.Add(btnF);

            frm.Controls.Add(g2); frm.Controls.Add(pFoot); frm.Controls.Add(pTop);
            frm.AcceptButton = btnF;

            if (dtItens != null)
            {
                g2.DataSource = dtItens;
                var hide = new System.Collections.Generic.HashSet<string>
                    { "Codigo", "Codigo_Pedido", "Codigo_Mercadoria", "auxCodigo",
                      "Situacao", "Status_Transmissao", "Info", "itpwDesconto_Pct" };
                var captions = new System.Collections.Generic.Dictionary<string, string>
                {
                    ["itpwNome_Mercadoria"] = "Produto",
                    ["itpwQtde"]           = "Qtde",
                    ["itpwPreco_Unitario"] = "Unit\u00e1rio (R$)",
                    ["itpwSubtotal"]       = "Subtotal (R$)",
                    ["itpwObservacoes"]    = "Observa\u00e7\u00f5es",
                    ["Produto"]  = "Produto",
                    ["Qtde"]     = "Qtde",
                    ["Unitario"] = "Unit\u00e1rio (R$)",
                    ["Subtotal"] = "Subtotal (R$)",
                    ["Obs"]      = "Observa\u00e7\u00f5es",
                };
                foreach (DataGridViewColumn col in g2.Columns)
                {
                    if (hide.Contains(col.Name)) { col.Visible = false; continue; }
                    if (captions.TryGetValue(col.Name, out string h)) col.HeaderText = h;
                }
            }
            frm.ShowDialog();
        }

        private void Imprimir()
        {
            var dt = grid.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            { MessageBox.Show("Sem dados para imprimir.", "Imprimir"); return; }

            var de  = dtpDe.Value.Date;
            var ate = dtpAte.Value.Date;
            int printRow = 0;

            var doc = new System.Drawing.Printing.PrintDocument();
            doc.DocumentName = $"Vendas {de:dd-MM-yyyy} a {ate:dd-MM-yyyy}";
            doc.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(75, 75, 75, 75);

            doc.PrintPage += (_, pe) =>
            {
                try
                {
                    var g    = pe.Graphics;
                    using var fnt  = new Font("Arial", 9F);
                    using var bold = new Font("Arial", 9F, FontStyle.Bold);
                    using var hdr  = new Font("Arial", 12F, FontStyle.Bold);
                    using var darkBrush  = new SolidBrush(Color.FromArgb(50, 40, 25));
                    using var amberBrush = new SolidBrush(Color.FromArgb(176, 110, 42));
                    using var greenBrush = new SolidBrush(Color.FromArgb(60, 110, 30));
                    using var redBrush   = new SolidBrush(Color.FromArgb(180, 50, 30));
                    using var altBrush   = new SolidBrush(Color.FromArgb(240, 234, 218));

                    float x  = pe.MarginBounds.Left;
                    float y  = pe.MarginBounds.Top;
                    float pw = pe.MarginBounds.Width;
                    if (pw <= 0) { pw = 650; x = 75; y = 75; }

                    var visCols = new System.Collections.Generic.List<DataGridViewColumn>();
                    foreach (DataGridViewColumn col in grid.Columns)
                        if (col.Visible) visCols.Add(col);
                    float colW = visCols.Count > 0 ? pw / visCols.Count : pw;

                    if (printRow == 0)
                    {
                        g.DrawString("Vendas por Per\u00edodo", hdr, amberBrush, x, y); y += 24;
                        g.DrawString($"Per\u00edodo: {de:dd/MM/yyyy} a {ate:dd/MM/yyyy}", fnt, darkBrush, x, y); y += 16;
                        string resumo = lblTotal.Text;
                        int sep = resumo.IndexOf('\u2502');
                        if (sep > 0) resumo = resumo[..sep].Trim();
                        g.DrawString(resumo, bold, amberBrush, x, y); y += 20;

                        using var hdrBg = new SolidBrush(Color.FromArgb(176, 110, 42));
                        g.FillRectangle(hdrBg, x, y, pw, 20);
                        float cx = x;
                        foreach (var col in visCols)
                        {
                            g.DrawString(col.HeaderText, bold, System.Drawing.Brushes.White, cx + 3, y + 3);
                            cx += colW;
                        }
                        y += 22;
                    }

                    float rowH = 18f;
                    while (printRow < dt.Rows.Count && y + rowH <= pe.MarginBounds.Bottom)
                    {
                        var row = dt.Rows[printRow];
                        if (printRow % 2 == 1) g.FillRectangle(altBrush, x, y, pw, rowH);
                        string tipo = row.Table.Columns.Contains("Tipo") ? row["Tipo"]?.ToString() ?? "" : "";
                        var txtBrush = tipo == "Venda" ? greenBrush : tipo == "Compra" ? redBrush : darkBrush;
                        float cx = x;
                        foreach (var col in visCols)
                        {
                            string val = "";
                            if (dt.Columns.Contains(col.Name) && row[col.Name] != DBNull.Value)
                            {
                                if (row[col.Name] is DateTime dtv) val = dtv.ToString("dd/MM HH:mm");
                                else if (row[col.Name] is decimal dv) val = Math.Abs(dv).ToString("N2");
                                else val = row[col.Name]?.ToString() ?? "";
                            }
                            using var sf = new System.Drawing.StringFormat { Trimming = System.Drawing.StringTrimming.EllipsisCharacter };
                            g.DrawString(val, fnt, txtBrush,
                                new System.Drawing.RectangleF(cx + 3, y + 2, colW - 6, rowH), sf);
                            cx += colW;
                        }
                        y += rowH;
                        printRow++;
                    }
                    pe.HasMorePages = printRow < dt.Rows.Count;
                }
                catch (Exception ex)
                {
                    pe.HasMorePages = false;
                    MessageBox.Show(this, "Erro ao gerar impress\u00e3o:\n" + ex.Message, "Erro de Impress\u00e3o",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            using var pd = new PrintDialog { Document = doc, UseEXDialog = true };
            if (pd.ShowDialog(this) != DialogResult.OK) return;
            try   { doc.Print(); }
            catch (Exception ex) { MessageBox.Show("Erro ao imprimir: " + ex.Message); }
        }

        private void ExportarCsv()
        {
            var dt = grid.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0) { MessageBox.Show("Sem dados para exportar."); return; }

            using var sfd = new SaveFileDialog
            {
                Title = "Exportar Relatório", Filter = "CSV (*.csv)|*.csv",
                FileName = $"vendas_{DateTime.Today:yyyyMMdd}.csv", DefaultExt = "csv"
            };
            if (sfd.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var sb = new StringBuilder();
                var headers = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                    headers[i] = $"\"{dt.Columns[i].ColumnName}\"";
                sb.AppendLine(string.Join(";", headers));

                foreach (DataRow row in dt.Rows)
                {
                    var cells = new string[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var val = row[i] == DBNull.Value ? "" : row[i].ToString();
                        if (row[i] is decimal d) val = d.ToString("N2");
                        else if (row[i] is DateTime dt2) val = dt2.ToString("dd/MM/yyyy HH:mm");
                        cells[i] = $"\"{val}\"";
                    }
                    sb.AppendLine(string.Join(";", cells));
                }
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"Exportado!\n{sfd.FileName}", "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Erro ao exportar: " + ex.Message); }
        }
    }
}
