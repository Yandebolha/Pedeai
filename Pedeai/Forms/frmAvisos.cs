using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;

namespace Pedeai.Forms
{
    public class frmAvisos : Form
    {
        private readonly EntradaMercadoriaBLL _bll = new EntradaMercadoriaBLL();
        private DataGridView _grid;
        private ComboBox     _cmbFiltro;
        private Label        _lblResumo;

        public frmAvisos()
        {
            var corFundo  = Color.FromArgb(15, 22, 45);
            var corCard   = Color.FromArgb(28, 37, 65);
            var corTopBar = Color.FromArgb(36, 48, 82);
            var corGrid   = Color.FromArgb(20, 28, 55);

            Text         = "Avisos — Parcelas a Pagar";
            BackColor    = corFundo;
            ForeColor    = Color.White;
            Font         = new Font("Segoe UI", 9F);
            ClientSize   = new Size(950, 600);
            MinimumSize  = new Size(820, 480);
            StartPosition= FormStartPosition.CenterParent;

            // ── Top bar ────────────────────────────────────────────────
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = corTopBar };

            var lblTit = new Label
            {
                Text = "🔔  Avisos de Pagamento", AutoSize = true, Top = 15,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.White
            };
            pnlTop.SizeChanged += (_, __) => lblTit.Left = (pnlTop.Width - lblTit.Width) / 2;

            _cmbFiltro = new ComboBox
            {
                Left = 12, Top = 14, Width = 160, DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbFiltro.Items.AddRange(new object[] { "Todas em Aberto", "Vencidas", "Vencem Hoje", "Próximos 7 dias", "Próximos 30 dias", "Pagas" });
            _cmbFiltro.SelectedIndex = 0;

            var btnAtualizar = new Button
            {
                Text = "⟳ Atualizar", Left = 182, Top = 13, Width = 95, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnAtualizar.FlatAppearance.BorderSize = 0;
            btnAtualizar.Click += (_, __) => CarregarGrid();
            _cmbFiltro.SelectedIndexChanged += (_, __) => CarregarGrid();
            pnlTop.Controls.AddRange(new Control[] { _cmbFiltro, btnAtualizar, lblTit });

            // ── Resumo ─────────────────────────────────────────────────
            var pnlResumo = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = corCard };
            _lblResumo = new Label
            {
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(200, 220, 255),
                Padding = new Padding(12, 0, 0, 0)
            };
            pnlResumo.Controls.Add(_lblResumo);

            // ── Grid ────────────────────────────────────────────────────
            _grid = new DataGridView
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
            _grid.DataError += (_, e) => e.ThrowException = false;
            _grid.RowPrePaint += Grid_RowPrePaint;

            // ── Footer ─────────────────────────────────────────────────
            var pnlFoot = new Panel { Dock = DockStyle.Bottom, Height = 46, BackColor = corCard };
            var btnPago = new Button
            {
                Text = "✔ Marcar como Pago", Left = 12, Top = 9, Width = 162, Height = 28,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnPago.FlatAppearance.BorderSize = 0;
            btnPago.Click += BtnPago_Click;
            pnlFoot.Controls.Add(btnPago);

            Controls.Add(_grid);
            Controls.Add(pnlResumo);
            Controls.Add(pnlFoot);
            Controls.Add(pnlTop);

            Load += (_, __) => CarregarGrid();
        }

        private void CarregarGrid()
        {
            try
            {
                var filtro = _cmbFiltro.SelectedItem?.ToString() ?? "";
                var dt = _bll.ListarAvisosFiltrados(filtro);
                _grid.DataSource = dt;
                ConfigurarColunas();
                AtualizarResumo(dt);
                ColorirLinhas();
            }
            catch (Exception ex) { _lblResumo.Text = "Erro: " + ex.Message; }
        }

        private void ConfigurarColunas()
        {
            if (_grid.Columns.Count == 0) return;
            foreach (DataGridViewColumn col in _grid.Columns)
                col.Visible = false;

            var show = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Codigo"]       = "Cód.",
                ["CodEntrada"]   = "Entrada #",
                ["Fornecedor"]   = "Fornecedor",
                ["Documento"]    = "Nº Doc",
                ["DataEntrada"]  = "Data Nota",
                ["Parcela"]      = "Parcela",
                ["TotalParcelas"]= "Total",
                ["Vencimento"]   = "Vencimento",
                ["Valor"]        = "Valor R$",
                ["Observacao"]   = "Observação",
                ["Situacao"]     = "Situação",
                ["DataPagamento"]= "Dt. Pagamento",
            };
            foreach (var kv in show)
                if (_grid.Columns.Contains(kv.Key))
                {
                    _grid.Columns[kv.Key].Visible = true;
                    _grid.Columns[kv.Key].HeaderText = kv.Value;
                }
        }

        private void AtualizarResumo(System.Data.DataTable dt)
        {
            if (dt == null) { _lblResumo.Text = ""; return; }
            decimal totalAberto = 0;
            int vencidas = 0;
            foreach (System.Data.DataRow r in dt.Rows)
            {
                if (r["Situacao"]?.ToString() == "A")
                {
                    if (decimal.TryParse(r["Valor"]?.ToString(), out decimal v)) totalAberto += v;
                    if (r["Vencimento"] is DateTime vc && vc.Date < DateTime.Today) vencidas++;
                }
            }
            _lblResumo.Text = $"  {dt.Rows.Count} parcelas  |  Total em aberto: R$ {totalAberto:N2}  |  Vencidas: {vencidas}";
        }

        private void ColorirLinhas()
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.DataBoundItem == null) continue;
                var dr = ((System.Data.DataRowView)row.DataBoundItem).Row;
                var sit = dr["Situacao"]?.ToString();
                if (sit == "P") { row.DefaultCellStyle.ForeColor = Color.FromArgb(100, 170, 100); continue; }
                if (!(dr["Vencimento"] is DateTime vcto)) continue;
                var dias = (vcto.Date - DateTime.Today).Days;
                if (dias < 0)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(80, 30, 30);
                else if (dias == 0)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(90, 70, 10);
                else if (dias <= 7)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(70, 55, 10);
            }
        }

        private void Grid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) { }

        private void BtnPago_Click(object sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma parcela para marcar como paga."); return; }
            var row = _grid.SelectedRows[0];
            if (row.DataBoundItem == null) return;
            var dr  = ((System.Data.DataRowView)row.DataBoundItem).Row;
            var cod = Convert.ToInt32(dr["Codigo"]);
            var sit = dr["Situacao"]?.ToString();
            if (sit == "P") { MessageBox.Show("Esta parcela já está paga."); return; }
            if (MessageBox.Show("Confirmar pagamento desta parcela?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var erro = _bll.MarcarParcelaPaga(cod);
            if (!string.IsNullOrEmpty(erro)) MessageBox.Show("Erro: " + erro);
            else CarregarGrid();
        }
    }
}
