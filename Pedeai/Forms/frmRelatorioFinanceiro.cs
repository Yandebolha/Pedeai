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

            // Colunas desejadas na ordem de exibição: Horário, Tipo, Referência,
            // Total Original, Desconto, Valor Recebido, Autorizado por, Pagamento
            var show = new System.Collections.Generic.Dictionary<string, (string header, int fill, int minW, int order)>
            {
                ["Horario"]       = ("Hor\u00e1rio",       11,  94, 0),
                ["Tipo"]          = ("Tipo",                 6,  46, 1),
                ["Referencia"]    = ("Refer\u00eancia",     10,  94, 2),
                ["ValorOriginal"] = ("Total Original",      13,  98, 3),
                ["Desconto"]      = ("Desconto R$",         11,  84, 4),
                ["ValorRecebido"] = ("Valor Recebido",      13,  98, 5),
                ["Autorizador"]   = ("Autorizado por",      13,  98, 6),
                ["Pagamento"]     = ("Pagamento",           11,  84, 7),
            };

            // oculta tudo que não está na lista (Descrição, Valor, Status, CodigoPedido…)
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (!show.ContainsKey(col.Name)) { col.Visible = false; continue; }
                col.Visible         = true;
                col.HeaderText      = show[col.Name].header;
                col.FillWeight      = show[col.Name].fill;
                col.MinimumWidth    = show[col.Name].minW;
                col.DisplayIndex    = show[col.Name].order;
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
            int pageNum  = 0;

            var doc = new System.Drawing.Printing.PrintDocument();
            doc.DocumentName = $"Vendas {de:dd-MM-yyyy} a {ate:dd-MM-yyyy}";
            doc.DefaultPageSettings.Landscape = true;
            doc.DefaultPageSettings.Margins   = new System.Drawing.Printing.Margins(50, 50, 50, 50);

            // Numeric column names — right-aligned
            var numericCols = new System.Collections.Generic.HashSet<string>
                { "Valor", "ValorOriginal", "ValorRecebido", "Desconto" };

            doc.PrintPage += (_, pe) =>
            {
                try
                {
                    pageNum++;
                    var g = pe.Graphics;

                    using var fnt      = new Font("Arial", 7F);
                    using var bold     = new Font("Arial", 7.5F, FontStyle.Bold);
                    using var hdr      = new Font("Arial", 10F,  FontStyle.Bold);
                    using var small    = new Font("Arial", 7F);
                    using var darkBrush  = new SolidBrush(Color.FromArgb(50,  40,  25));
                    using var amberBrush = new SolidBrush(Color.FromArgb(176, 110, 42));
                    using var greenBrush = new SolidBrush(Color.FromArgb(50,  110, 30));
                    using var redBrush   = new SolidBrush(Color.FromArgb(180, 50,  30));
                    using var altBrush   = new SolidBrush(Color.FromArgb(240, 234, 218));
                    using var hdrBg      = new SolidBrush(Color.FromArgb(176, 110, 42));
                    using var rowPen     = new Pen(Color.FromArgb(210, 200, 180), 0.5f);
                    using var colPen     = new Pen(Color.FromArgb(200, 190, 170), 0.5f);
                    using var divPen     = new Pen(Color.FromArgb(220, 255, 255, 255), 0.5f);

                    using var sfL = new StringFormat
                        { Trimming = StringTrimming.EllipsisCharacter, Alignment = StringAlignment.Near,
                          FormatFlags = StringFormatFlags.NoWrap };
                    using var sfR = new StringFormat
                        { Trimming = StringTrimming.EllipsisCharacter, Alignment = StringAlignment.Far,
                          FormatFlags = StringFormatFlags.NoWrap };
                    using var sfC = new StringFormat
                        { Trimming = StringTrimming.EllipsisCharacter, Alignment = StringAlignment.Center,
                          FormatFlags = StringFormatFlags.NoWrap };

                    float x           = pe.MarginBounds.Left;
                    float y           = pe.MarginBounds.Top;
                    float pw          = pe.MarginBounds.Width;
                    float pageBottom  = pe.MarginBounds.Bottom;

                    // Build visible column list (ordered, with weights)
                    var visCols = new System.Collections.Generic.List<DataGridViewColumn>();
                    foreach (DataGridViewColumn col in grid.Columns)
                        if (col.Visible) visCols.Add(col);
                    float totalWeight = 0f;
                    foreach (var c2 in visCols) totalWeight += c2.FillWeight;
                    if (totalWeight <= 0) totalWeight = visCols.Count;
                    float GetW(DataGridViewColumn c2) => pw * c2.FillWeight / totalWeight;

                    // ── Page header ──────────────────────────────────────────
                    if (pageNum == 1)
                    {
                        g.DrawString("Vendas por Per\u00edodo", hdr, amberBrush, x, y); y += 18;
                        g.DrawString($"Per\u00edodo: {de:dd/MM/yyyy} a {ate:dd/MM/yyyy}", small, darkBrush, x, y); y += 13;
                        string resumo = lblTotal.Text;
                        int pipeIdx = resumo.IndexOf('\u2502');
                        if (pipeIdx > 0) resumo = resumo[..pipeIdx].Trim();
                        g.DrawString(resumo, bold, amberBrush, x, y); y += 14;
                    }
                    else
                    {
                        string cont = $"Vendas por Per\u00edodo  [{de:dd/MM/yyyy} a {ate:dd/MM/yyyy}]  — continua\u00e7\u00e3o";
                        g.DrawString(cont, small, amberBrush, x, y); y += 11;
                    }
                    y += 3;

                    // ── Column header row ────────────────────────────────────
                    const float hdrH = 16f;
                    g.FillRectangle(hdrBg, x, y, pw, hdrH);
                    float cx = x;
                    foreach (var col in visCols)
                    {
                        float cw = GetW(col);
                        bool isNum = numericCols.Contains(col.Name);
                        var sf2 = isNum ? sfR : sfL;
                        g.DrawString(col.HeaderText, bold, Brushes.White,
                            new RectangleF(cx + 3, y + 2, cw - 5, hdrH - 2), sf2);
                        if (cx > x) g.DrawLine(divPen, cx, y + 2, cx, y + hdrH - 1);
                        cx += cw;
                    }
                    y += hdrH;

                    // ── Data rows ────────────────────────────────────────────
                    const float rowH    = 13f;
                    const float footerH = 14f;

                    while (printRow < dt.Rows.Count && y + rowH <= pageBottom - footerH)
                    {
                        var row  = dt.Rows[printRow];
                        string tipo = row.Table.Columns.Contains("Tipo") ? row["Tipo"]?.ToString() ?? "" : "";
                        var txtBrush = tipo == "Venda" ? greenBrush : tipo == "Compra" ? redBrush : darkBrush;

                        if (printRow % 2 == 1) g.FillRectangle(altBrush, x, y, pw, rowH);

                        cx = x;
                        foreach (var col in visCols)
                        {
                            float cw    = GetW(col);
                            bool isNum  = numericCols.Contains(col.Name);
                            var sf2     = isNum ? sfR : sfL;
                            string val  = "";
                            if (dt.Columns.Contains(col.Name) && row[col.Name] != DBNull.Value)
                            {
                                if   (row[col.Name] is DateTime dtv) val = dtv.ToString("dd/MM HH:mm");
                                else if (row[col.Name] is decimal dv)
                                    val = dv == 0m ? "" : Math.Abs(dv).ToString("N2");
                                else val = row[col.Name]?.ToString() ?? "";
                            }
                            g.DrawString(val, fnt, txtBrush, new RectangleF(cx + 3, y + 1, cw - 5, rowH - 1), sf2);
                            if (cx > x) g.DrawLine(colPen, cx, y, cx, y + rowH);
                            cx += cw;
                        }
                        g.DrawLine(rowPen, x, y + rowH, x + pw, y + rowH);
                        y += rowH;
                        printRow++;
                    }

                    // ── Footer: page number ──────────────────────────────────
                    string footer = $"P\u00e1gina {pageNum}  \u2014  Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}";
                    g.DrawString(footer, small, darkBrush,
                        new RectangleF(x, pageBottom - footerH, pw, footerH), sfR);

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
