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

            doc.PrintPage += (_, pe) =>
            {
                var g    = pe.Graphics;
                var fnt  = new Font("Segoe UI", 8F);
                var bold = new Font("Segoe UI", 8F, FontStyle.Bold);
                var hdr  = new Font("Segoe UI", 11F, FontStyle.Bold);
                var brushDark  = new SolidBrush(Color.FromArgb(50, 40, 25));
                var brushAmber = new SolidBrush(Color.FromArgb(176, 110, 42));
                var brushHdrBg = new SolidBrush(Color.FromArgb(176, 110, 42));
                var brushHdrFg = Brushes.White;
                var brushAlt   = new SolidBrush(Color.FromArgb(240, 234, 218));

                float x  = pe.MarginBounds.Left;
                float y  = pe.MarginBounds.Top;
                float pw = pe.MarginBounds.Width;

                if (printRow == 0)
                {
                    // cabeçalho do relatório
                    string sit = _turno.turSituacao == 'A' ? "ABERTO" : "FECHADO";
                    g.DrawString($"Relatório de Turno #{_turno.Codigo}  —  {sit}", hdr, brushAmber, x, y); y += 22;
                    g.DrawString($"Abertura: {_turno.turAbertura:dd/MM/yyyy HH:mm}   |   Usuário: {_turno.turUsuario}", fnt, brushDark, x, y); y += 15;
                    g.DrawString($"Caixa Inicial: R$ {_turno.turCaixa_Inicial:N2}   |   {lblFechamento.Text}   |   {lblCaixaFinal.Text}", fnt, brushDark, x, y); y += 15;
                    g.DrawLine(new Pen(Color.FromArgb(176, 110, 42), 1.5f), x, y, x + pw, y); y += 5;
                    g.DrawString(lblResumo.Text, bold, brushAmber, x, y); y += 5;
                    g.DrawLine(new Pen(Color.FromArgb(210, 190, 160)), x, y, x + pw, y); y += 10;

                    // cabeçalho das colunas
                    float colW = pw / cols.Count;
                    float cx   = x;
                    g.FillRectangle(brushHdrBg, cx, y, pw, 18);
                    foreach (var col in cols)
                    {
                        g.DrawString(col.HeaderText, bold, brushHdrFg, cx + 2, y + 2);
                        cx += colW;
                    }
                    y += 20;
                }

                float rowH = 17f;
                float colWr = pw / cols.Count;
                while (printRow < dt.Rows.Count && y + rowH <= pe.MarginBounds.Bottom)
                {
                    var row = dt.Rows[printRow];
                    float cx = x;
                    if (printRow % 2 == 1)
                        g.FillRectangle(brushAlt, cx, y, pw, rowH);
                    foreach (var col in cols)
                    {
                        var val = row.Table.Columns.Contains(col.Name) && row[col.Name] != DBNull.Value
                            ? (row[col.Name]?.ToString() ?? "") : "";
                        g.DrawString(val, fnt, brushDark, cx + 2, y + 2);
                        cx += colWr;
                    }
                    y += rowH;
                    printRow++;
                }

                pe.HasMorePages = printRow < dt.Rows.Count;
            };

            using var pv = new PrintPreviewDialog
            {
                Document    = doc,
                WindowState = FormWindowState.Maximized,
                UseAntiAlias = true,
                Text        = "Pré-visualização — Relatório de Turno"
            };
            pv.ShowDialog(this);
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
