using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    public class frmMovimentacoesDia : Form
    {
        public frmMovimentacoesDia(DateTime dia, DataTable dt)
        {
            var corFundo  = Color.FromArgb(15, 22, 45);
            var corCard   = Color.FromArgb(28, 37, 65);
            var corTopBar = Color.FromArgb(36, 48, 82);
            var corGrid   = Color.FromArgb(20, 28, 55);

            Text         = $"Movimentações — {dia:dd/MM/yyyy}";
            BackColor    = corFundo;
            ForeColor    = Color.White;
            Font         = new Font("Segoe UI", 9F);
            ClientSize   = new Size(900, 520);
            MinimumSize  = new Size(700, 400);
            StartPosition= FormStartPosition.CenterParent;

            // ── Top bar ─────────────────────────────────────────────────
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = corTopBar };
            var lblTit = new Label
            {
                Text = $"📋  Movimentações do dia {dia:dd/MM/yyyy}", AutoSize = true, Top = 14,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.White
            };
            pnlTop.SizeChanged += (_, __) => lblTit.Left = (pnlTop.Width - lblTit.Width) / 2;
            pnlTop.Controls.Add(lblTit);

            // ── Resumo ───────────────────────────────────────────────────
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

            var pnlRes = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = corCard };
            var lblRes = new Label
            {
                Dock    = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Font    = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 225, 255),
                Text    = $"  Vendas: R$ {totalVendas:N2}   |   Compras: R$ {totalCompras:N2}   |   " +
                          $"Saldo do dia: R$ {saldo:N2}"
            };
            pnlRes.Controls.Add(lblRes);

            // ── Grid ────────────────────────────────────────────────────
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, BackgroundColor = corGrid,
                DefaultCellStyle  = { BackColor = corGrid, ForeColor = Color.White },
                GridColor = Color.FromArgb(40, 55, 90), BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = { BackColor = corTopBar, ForeColor = Color.White },
                Font = new Font("Segoe UI", 9F),
            };
            grid.DataError += (_, e) => e.ThrowException = false;
            grid.RowPrePaint += Grid_RowColor;

            // ── Footer ──────────────────────────────────────────────────
            var pnlFoot = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = corCard };
            var btnFech = new Button
            {
                Text = "Fechar", Left = 0, Top = 8, Width = 100, Height = 26,
                BackColor = Color.FromArgb(80, 95, 130), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnFech.FlatAppearance.BorderSize = 0;
            btnFech.Click += (_, __) => Close();
            pnlFoot.SizeChanged += (_, __) => btnFech.Left = (pnlFoot.Width - btnFech.Width) / 2;
            pnlFoot.Controls.Add(btnFech);

            Controls.Add(grid);
            Controls.Add(pnlRes);
            Controls.Add(pnlFoot);
            Controls.Add(pnlTop);

            // Bind data
            if (dt != null)
            {
                grid.DataSource = dt;
                ConfigurarColunas(grid);
                ColorirLinhas(grid);
            }
        }

        private void ConfigurarColunas(DataGridView grid)
        {
            if (grid.Columns.Count == 0) return;
            var show = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Horario"]    = "Horário",
                ["Tipo"]       = "Tipo",
                ["Referencia"] = "Referência",
                ["Descricao"]  = "Descrição",
                ["Valor"]      = "Valor R$",
                ["Pagamento"]  = "Pagamento",
                ["Status"]     = "Status",
            };
            foreach (DataGridViewColumn col in grid.Columns)
                col.Visible = show.ContainsKey(col.Name);
            foreach (var kv in show)
                if (grid.Columns.Contains(kv.Key))
                    grid.Columns[kv.Key].HeaderText = kv.Value;
        }

        private void ColorirLinhas(DataGridView grid)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.DataBoundItem == null) continue;
                var dr   = ((DataRowView)row.DataBoundItem).Row;
                var tipo = dr["Tipo"]?.ToString();
                if (tipo == "Compra")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(60, 20, 20);
                else
                    row.DefaultCellStyle.BackColor = Color.FromArgb(15, 40, 25);
            }
        }

        private void Grid_RowColor(object sender, DataGridViewRowPrePaintEventArgs e) { }
    }
}
