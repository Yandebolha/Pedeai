using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmRelatorioTurno : Form
    {
        private TurnoBLL _bll;
        private readonly Turno _turno;

        // Construtor sem parÃ¢metros â€” obrigatÃ³rio para o Designer
        public frmRelatorioTurno()
        {
            InitializeComponent();
        }

        public frmRelatorioTurno(Turno turno)
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _turno = turno;
            _bll   = new TurnoBLL();
            Load  += frmRelatorioTurno_Load;
        }

        private void frmRelatorioTurno_Load(object sender, EventArgs e)
        {
            MontarCabecalho();
            CarregarPedidos();
        }

        private void MontarCabecalho()
        {
            string sit = _turno.turSituacao == 'A' ? "ABERTO" : "FECHADO";
            lblCabecalho.Text =
                $"Turno #{_turno.Codigo}   |   {sit}   |   " +
                $"Abertura: {_turno.turAbertura:dd/MM/yyyy HH:mm}   |   " +
                $"Usu\u00e1rio: {_turno.turUsuario}";

            lblCaixaInicial.Text  = $"Caixa Inicial: R$ {_turno.turCaixa_Inicial:N2}";
            lblFechamento.Text    = _turno.turFechamento.HasValue
                ? $"Fechamento: {_turno.turFechamento.Value:dd/MM/yyyy HH:mm}"
                : "Fechamento: (em aberto)";
            lblCaixaFinal.Text    = _turno.turCaixa_Final.HasValue
                ? $"Caixa Final: R$ {_turno.turCaixa_Final.Value:N2}"
                : "Caixa Final: â€”";
        }

        private void CarregarPedidos()
        {
            try
            {
                var ate = _turno.turFechamento ?? DateTime.Now;
                var dt  = _bll.GetPedidosTurno(_turno.turAbertura, ate);
                gridPedidos.DataSource = dt;
                ConfigurarColunas(dt);
                MontarResumo(dt);
            }
            catch (Exception ex)
            {
                lblResumo.Text = "Erro: " + ex.Message;
            }
        }

        private void ConfigurarColunas(DataTable dt)
        {
            if (gridPedidos.Columns.Count == 0) return;

            var show = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Codigo"]              = "N\u00ba",
                ["pediNome_Cliente"]    = "Cliente",
                ["pediSituacao"]        = "Sit.",
                ["pediValor_Total"]     = "Total R$",
                ["pediPago_Dinheiro"]   = "Dinheiro",
                ["pediPago_Cartao"]     = "Cart\u00e3o",
                ["pediPago_Pix"]        = "Pix",
                ["pediData_Lancamento"] = "Lan\u00e7amento",
            };

            foreach (DataGridViewColumn col in gridPedidos.Columns)
                col.Visible = false;

            foreach (var kv in show)
                if (gridPedidos.Columns.Contains(kv.Key))
                {
                    gridPedidos.Columns[kv.Key].Visible    = true;
                    gridPedidos.Columns[kv.Key].HeaderText = kv.Value;
                }
        }

        private void MontarResumo(DataTable dt)
        {
            decimal totalVendas = 0, totalDin = 0, totalCar = 0, totalPix = 0;
            int qtdPedidos = dt.Rows.Count;

            decimal C(DataRow r, string c) => r[c] == DBNull.Value ? 0m : Convert.ToDecimal(r[c]);

            foreach (DataRow r in dt.Rows)
            {
                totalVendas += C(r, "pediValor_Total");
                totalDin    += C(r, "pediPago_Dinheiro");
                totalCar    += C(r, "pediPago_Cartao");
                totalPix    += C(r, "pediPago_Pix");
            }

            lblResumo.Text =
                $"Pedidos: {qtdPedidos}   |   " +
                $"Total Vendas: R$ {totalVendas:N2}   |   " +
                $"Dinheiro: R$ {totalDin:N2}   |   " +
                $"Cart\u00e3o: R$ {totalCar:N2}   |   " +
                $"Pix: R$ {totalPix:N2}";

            decimal diferenca = totalVendas - (totalDin + totalCar + totalPix);
            if (Math.Abs(diferenca) > 0.01m)
                lblResumo.Text += $"   |   Dif. pagamento: R$ {diferenca:N2}";
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            var dt = gridPedidos.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            { MessageBox.Show("Sem dados para imprimir.", "Imprimir"); return; }

            // Colunas visíveis na ordem de exibição
            var cols = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn c in gridPedidos.Columns)
                if (c.Visible) cols.Add(c);
            cols.Sort((a, b) => a.DisplayIndex.CompareTo(b.DisplayIndex));

            int printRow = 0;

            var doc = new PrintDocument();
            doc.DocumentName = $"Relatório Turno #{_turno.Codigo}";

            // Forçar tamanho de página para evitar MarginBounds zerado (sem impressora padrão)
            doc.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(75, 75, 75, 75);

            doc.PrintPage += (_, pe) =>
            {
                try
                {
                    var g    = pe.Graphics;
                    using var fnt  = new Font("Arial", 9F);
                    using var bold = new Font("Arial", 9F, FontStyle.Bold);
                    using var hdr  = new Font("Arial", 12F, FontStyle.Bold);
                    using var brushDark  = new SolidBrush(Color.FromArgb(50, 40, 25));
                    using var brushAmber = new SolidBrush(Color.FromArgb(176, 110, 42));
                    using var brushHdrBg = new SolidBrush(Color.FromArgb(176, 110, 42));
                    using var brushAlt   = new SolidBrush(Color.FromArgb(240, 234, 218));

                    float x  = pe.MarginBounds.Left;
                    float y  = pe.MarginBounds.Top;
                    float pw = pe.MarginBounds.Width;
                    if (pw <= 0) { pw = 650; x = 75; y = 75; }  // fallback se não houver impressora

                    // Larguras proporcionais por coluna para evitar sobreposição
                    var colWeightMap = new System.Collections.Generic.Dictionary<string, float>
                    {
                        ["Codigo"]              = 0.4f,
                        ["pediNome_Cliente"]    = 2.2f,
                        ["pediSituacao"]        = 0.4f,
                        ["pediValor_Total"]     = 0.9f,
                        ["pediPago_Dinheiro"]   = 0.9f,
                        ["pediPago_Cartao"]     = 0.9f,
                        ["pediPago_Pix"]        = 0.7f,
                        ["pediData_Lancamento"] = 1.4f,
                    };
                    float totalWeight = 0f;
                    foreach (var col in cols)
                        totalWeight += colWeightMap.TryGetValue(col.Name, out var ww) ? ww : 1.0f;
                    float[] colWidths = new float[cols.Count];
                    for (int ci = 0; ci < cols.Count; ci++)
                        colWidths[ci] = pw * (colWeightMap.TryGetValue(cols[ci].Name, out var cw) ? cw : 1.0f) / totalWeight;

                    if (printRow == 0)
                    {
                        string sit = _turno.turSituacao == 'A' ? "ABERTO" : "FECHADO";
                        g.DrawString($"Relatório de Turno #{_turno.Codigo}  —  {sit}", hdr, brushAmber, x, y); y += 24;
                        g.DrawString($"Abertura: {_turno.turAbertura:dd/MM/yyyy HH:mm}   |   Usuário: {_turno.turUsuario}", fnt, brushDark, x, y); y += 16;
                        g.DrawString($"Caixa Inicial: R$ {_turno.turCaixa_Inicial:N2}   |   {lblFechamento.Text}   |   {lblCaixaFinal.Text}", fnt, brushDark, x, y); y += 16;
                        g.DrawString(lblResumo.Text, bold, brushAmber, x, y); y += 20;
                        using var sepPen = new Pen(Color.FromArgb(210, 190, 160));
                        g.DrawLine(sepPen, x, y, x + pw, y); y += 8;

                        float cx = x;
                        g.FillRectangle(brushHdrBg, cx, y, pw, 20);
                        for (int ci = 0; ci < cols.Count; ci++)
                        {
                            g.DrawString(cols[ci].HeaderText, bold, Brushes.White, cx + 3, y + 3);
                            cx += colWidths[ci];
                        }
                        y += 22;
                    }

                    float rowH = 18f;
                    while (printRow < dt.Rows.Count && y + rowH <= pe.MarginBounds.Bottom)
                    {
                        var row = dt.Rows[printRow];
                        float cx = x;
                        if (printRow % 2 == 1)
                            g.FillRectangle(brushAlt, cx, y, pw, rowH);
                        for (int ci = 0; ci < cols.Count; ci++)
                        {
                            var col = cols[ci];
                            var val = row.Table.Columns.Contains(col.Name) && row[col.Name] != DBNull.Value
                                ? (row[col.Name]?.ToString() ?? "") : "";
                            g.DrawString(val, fnt, brushDark, cx + 3, y + 3);
                            cx += colWidths[ci];
                        }
                        y += rowH;
                        printRow++;
                    }

                    pe.HasMorePages = printRow < dt.Rows.Count;
                }
                catch (Exception ex)
                {
                    pe.HasMorePages = false;
                    MessageBox.Show(this, "Erro ao gerar impressão:\n" + ex.Message, "Erro de Impressão",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            using var pd = new PrintDialog { Document = doc, UseEXDialog = true };
            if (pd.ShowDialog(this) != DialogResult.OK) return;
            try   { doc.Print(); }
            catch (Exception ex) { MessageBox.Show("Erro ao imprimir: " + ex.Message); }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void GridPedidos_DataError(object sender, System.Windows.Forms.DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
    }
}
