using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Pedeai.BLL;

namespace Pedeai.Forms
{
    public class frmRelatorioFinanceiro : Form
    {
        private readonly PedidoBLL           _pedidoBLL;
        private readonly GastoMaterialBLL    _gastosBLL;
        private readonly EntradaMercadoriaBLL _entradaBLL;

        private ComboBox        _cmbTipo;
        private DateTimePicker  _dtpDe;
        private DateTimePicker  _dtpAte;
        private DataGridView    _grid;
        private Label           _lblTotal;

        public frmRelatorioFinanceiro(PedidoBLL pedidoBLL, GastoMaterialBLL gastosBLL, EntradaMercadoriaBLL entradaBLL)
        {
            _pedidoBLL  = pedidoBLL;
            _gastosBLL  = gastosBLL;
            _entradaBLL = entradaBLL;
            ConstruirUI();
        }

        private void ConstruirUI()
        {
            Text             = "Relatório Financeiro";
            StartPosition    = FormStartPosition.CenterParent;
            Size             = new Size(860, 600);
            MinimumSize      = new Size(700, 450);
            BackColor        = Color.FromArgb(248, 245, 240);
            Font             = new Font("Segoe UI", 9F);

            // ── Top bar ──────────────────────────────────────────────────────
            var topBar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 44,
                BackColor = Color.FromArgb(176, 110, 42)
            };
            var lblTit = new Label
            {
                Text      = "Relatório Financeiro",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(12, 0, 0, 0)
            };
            topBar.Controls.Add(lblTit);

            // ── Filter bar ───────────────────────────────────────────────────
            var pnlFil = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 46,
                BackColor = Color.FromArgb(235, 228, 214),
                Padding   = new Padding(8, 8, 8, 4)
            };

            int lx = 8;
            pnlFil.Controls.Add(MkLbl("Tipo:", lx, 14)); lx += 36;

            _cmbTipo = new ComboBox
            {
                Left = lx, Top = 10, Width = 100, DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(14, 21, 46), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _cmbTipo.Items.AddRange(new object[] { "Ambos", "Entrada", "Saída" });
            _cmbTipo.SelectedIndex = 0;
            pnlFil.Controls.Add(_cmbTipo); lx += 110;

            pnlFil.Controls.Add(MkLbl("De:", lx, 14)); lx += 28;
            _dtpDe = new DateTimePicker { Left = lx, Top = 10, Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };
            pnlFil.Controls.Add(_dtpDe); lx += 130;

            pnlFil.Controls.Add(MkLbl("Até:", lx, 14)); lx += 32;
            _dtpAte = new DateTimePicker { Left = lx, Top = 10, Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            pnlFil.Controls.Add(_dtpAte); lx += 130;

            var btnFil = MkBtn("Filtrar", lx, Color.FromArgb(224, 113, 42)); lx += 90;
            btnFil.Click += (_, __) => Carregar();
            pnlFil.Controls.Add(btnFil);

            var btnExp = MkBtn("Exportar CSV", lx, Color.FromArgb(87, 120, 38)); lx += 120;
            btnExp.Width = 114;
            btnExp.Click += (_, __) => ExportarCsv();
            pnlFil.Controls.Add(btnExp);

            // ── Bottom bar ───────────────────────────────────────────────────
            var pnlBot = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 32,
                BackColor = Color.FromArgb(235, 228, 214)
            };
            _lblTotal = new Label
            {
                Dock      = DockStyle.Fill,
                ForeColor = Color.FromArgb(130, 80, 20),
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(8, 0, 0, 0),
                Text      = ""
            };
            pnlBot.Controls.Add(_lblTotal);

            // ── Grid ─────────────────────────────────────────────────────────
            _grid = new DataGridView
            {
                Dock                    = DockStyle.Fill,
                ReadOnly                = true,
                AllowUserToAddRows      = false,
                RowHeadersVisible       = false,
                SelectionMode           = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor         = Color.FromArgb(248, 245, 240),
                GridColor               = Color.FromArgb(210, 200, 180),
                EnableHeadersVisualStyles = false,
                BorderStyle             = BorderStyle.None,
                Font                    = new Font("Segoe UI", 9F)
            };
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(176, 110, 42);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _grid.DefaultCellStyle.BackColor              = Color.FromArgb(250, 246, 238);
            _grid.DefaultCellStyle.ForeColor              = Color.FromArgb(50, 40, 25);
            _grid.DefaultCellStyle.SelectionBackColor     = Color.FromArgb(224, 113, 42);
            _grid.DefaultCellStyle.SelectionForeColor     = Color.White;
            _grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 234, 218);
            _grid.DataError += (_, ev) => ev.ThrowException = false;

            Controls.Add(_grid);
            Controls.Add(pnlBot);
            Controls.Add(pnlFil);
            Controls.Add(topBar);

            Carregar();
        }

        private static Label MkLbl(string t, int l, int top) => new Label
        {
            Text      = t,
            Left      = l, Top = top, AutoSize = true,
            ForeColor = Color.FromArgb(50, 40, 25),
            Font      = new Font("Segoe UI", 8.5F)
        };

        private static Button MkBtn(string t, int l, Color bg)
        {
            var b = new Button
            {
                Text      = t,
                Left      = l, Top = 7, Width = 80, Height = 28,
                BackColor = bg, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void Carregar()
        {
            try
            {
                var de   = _dtpDe.Value.Date;
                var ate  = _dtpAte.Value.Date;
                string tipo = _cmbTipo.SelectedItem?.ToString() ?? "Ambos";

                var result = new DataTable();
                result.Columns.Add("Data",      typeof(DateTime));
                result.Columns.Add("Tipo",      typeof(string));
                result.Columns.Add("Descrição", typeof(string));
                result.Columns.Add("Valor",     typeof(decimal));

                decimal totalEntrada = 0m, totalSaida = 0m;

                if (tipo == "Entrada" || tipo == "Ambos")
                {
                    var dtFin = _pedidoBLL.GetFinanceiro(de, ate);
                    foreach (DataRow r in dtFin.Rows)
                    {
                        if (r["Dia"] == DBNull.Value) continue;
                        decimal v = r["TotalBruto"] == DBNull.Value ? 0m : Convert.ToDecimal(r["TotalBruto"]);
                        int p     = r["Pedidos"]    == DBNull.Value ? 0  : Convert.ToInt32(r["Pedidos"]);
                        if (v == 0m && p == 0) continue;
                        var nr = result.NewRow();
                        nr["Data"]      = Convert.ToDateTime(r["Dia"]);
                        nr["Tipo"]      = "Entrada";
                        nr["Descrição"] = $"Vendas — {p} pedido(s)";
                        nr["Valor"]     = v;
                        result.Rows.Add(nr);
                        totalEntrada += v;
                    }
                }

                if (tipo == "Saída" || tipo == "Ambos")
                {
                    // Gastos de material
                    var dtGastos = _gastosBLL.Listar(de, ate);
                    foreach (DataRow r in dtGastos.Rows)
                    {
                        decimal v = r["Valor"] == DBNull.Value ? 0m : Convert.ToDecimal(r["Valor"]);
                        var nr = result.NewRow();
                        nr["Data"]      = r["Data"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDateTime(r["Data"]);
                        nr["Tipo"]      = "Saída";
                        nr["Descrição"] = $"Gasto — {r["Descricao"]}";
                        nr["Valor"]     = v;
                        result.Rows.Add(nr);
                        totalSaida += v;
                    }

                    // Compras (entradas de mercadoria)
                    var dtEntradas = _entradaBLL.Listar(de, ate);
                    foreach (DataRow r in dtEntradas.Rows)
                    {
                        decimal v = r["Total"] == DBNull.Value ? 0m : Convert.ToDecimal(r["Total"]);
                        var nr = result.NewRow();
                        nr["Data"]      = r["entData"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDateTime(r["entData"]);
                        nr["Tipo"]      = "Saída";
                        nr["Descrição"] = $"Compra — {r["Fornecedor"]}";
                        nr["Valor"]     = v;
                        result.Rows.Add(nr);
                        totalSaida += v;
                    }
                }

                // Ordenar por data desc
                var sorted = result.DefaultView;
                sorted.Sort = "Data DESC";
                _grid.DataSource = sorted.ToTable();

                FormatarGrid();

                // Resumo
                string resumo = tipo == "Ambos"
                    ? $"Entradas: R$ {totalEntrada:N2}   |   Saídas: R$ {totalSaida:N2}   |   Saldo: R$ {(totalEntrada - totalSaida):N2}"
                    : tipo == "Entrada"
                        ? $"Total Entradas: R$ {totalEntrada:N2}"
                        : $"Total Saídas: R$ {totalSaida:N2}";
                _lblTotal.Text = resumo;
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar: " + ex.Message); }
        }

        private void FormatarGrid()
        {
            if (_grid.Columns.Count == 0) return;
            if (_grid.Columns.Contains("Data"))
            {
                _grid.Columns["Data"].DefaultCellStyle.Format = "dd/MM/yyyy";
                _grid.Columns["Data"].FillWeight = 15;
            }
            if (_grid.Columns.Contains("Tipo"))
            {
                _grid.Columns["Tipo"].FillWeight = 12;
                _grid.Columns["Tipo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (_grid.Columns.Contains("Descrição"))
                _grid.Columns["Descrição"].FillWeight = 55;
            if (_grid.Columns.Contains("Valor"))
            {
                _grid.Columns["Valor"].DefaultCellStyle.Format = "N2";
                _grid.Columns["Valor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                _grid.Columns["Valor"].FillWeight = 18;
            }

            // Colorir linhas por tipo
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.IsNewRow) continue;
                var tipo = row.Cells["Tipo"]?.Value?.ToString();
                if (tipo == "Entrada")
                {
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(60, 110, 30);
                }
                else if (tipo == "Saída")
                {
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(180, 50, 30);
                }
            }
        }

        private void ExportarCsv()
        {
            var dt = _grid.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0) { MessageBox.Show("Sem dados para exportar."); return; }

            using var sfd = new SaveFileDialog
            {
                Title      = "Exportar Relatório",
                Filter     = "CSV (*.csv)|*.csv",
                FileName   = $"relatorio_financeiro_{DateTime.Today:yyyyMMdd}.csv",
                DefaultExt = "csv"
            };
            if (sfd.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var sb = new StringBuilder();
                // Header
                var headers = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                    headers[i] = $"\"{dt.Columns[i].ColumnName}\"";
                sb.AppendLine(string.Join(";", headers));

                // Rows
                foreach (DataRow row in dt.Rows)
                {
                    var cells = new string[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var val = row[i] == DBNull.Value ? "" : row[i].ToString()!;
                        if (row[i] is decimal d) val = d.ToString("N2");
                        else if (row[i] is DateTime dt2) val = dt2.ToString("dd/MM/yyyy");
                        cells[i] = $"\"{val}\"";
                    }
                    sb.AppendLine(string.Join(";", cells));
                }

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"Relatório exportado com sucesso!\n{sfd.FileName}", "Exportar CSV",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Erro ao exportar: " + ex.Message); }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
