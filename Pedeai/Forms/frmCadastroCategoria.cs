using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.DB;

namespace Pedeai.Forms
{
    public class frmCadastroCategoria : Form
    {
        private DataGridView grid = new DataGridView();
        private TextBox txtNome = new TextBox();
        private NumericUpDown numOrdem = new NumericUpDown();
        private ComboBox cmbSituacao = new ComboBox();
        private Button btnNovo = new Button();
        private Button btnSalvar = new Button();
        private Button btnCancelar = new Button();
        private Panel pnlForm = new Panel();
        private int _codigoEditando = 0;

        public frmCadastroCategoria()
        {
            Text = "Categorias de Produtos";
            Size = new Size(700, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(600, 450);
            Font = new Font("Segoe UI", 9);

            BuildUI();
            Carregar();
        }

        private void BuildUI()
        {
            // ── Barra superior ────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8, 6, 8, 0) };
            topBar.BackColor = Color.FromArgb(40, 40, 80);

            btnNovo = Botao("+ Nova Categoria", Color.FromArgb(0, 150, 136));
            btnNovo.Click += (_, __) => ModoNovo();
            topBar.Controls.Add(btnNovo);

            // ── Grid ──────────────────────────────────────────────────────────
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.None
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            // ── Formulário de edição ──────────────────────────────────────────
            pnlForm = new Panel { Dock = DockStyle.Bottom, Height = 130, Padding = new Padding(10), Visible = false };
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;

            var lbl1 = new Label { Text = "Nome:", AutoSize = true, Left = 10, Top = 14 };
            txtNome = new TextBox { Left = 70, Top = 10, Width = 280 };

            var lbl2 = new Label { Text = "Ordem:", AutoSize = true, Left = 370, Top = 14 };
            numOrdem = new NumericUpDown { Left = 430, Top = 10, Width = 60, Minimum = 0, Maximum = 999 };

            var lbl3 = new Label { Text = "Situação:", AutoSize = true, Left = 510, Top = 14 };
            cmbSituacao = new ComboBox { Left = 575, Top = 10, Width = 80, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSituacao.Items.AddRange(new[] { "A", "I" });
            cmbSituacao.SelectedIndex = 0;

            btnSalvar = Botao("Salvar", Color.FromArgb(33, 150, 243));
            btnSalvar.Left = 10; btnSalvar.Top = 50;
            btnSalvar.Click += BtnSalvar_Click;

            btnCancelar = Botao("Cancelar", Color.FromArgb(158, 158, 158));
            btnCancelar.Left = 120; btnCancelar.Top = 50;
            btnCancelar.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };

            var btnExcluir = Botao("Desativar", Color.FromArgb(244, 67, 54));
            btnExcluir.Left = 230; btnExcluir.Top = 50;
            btnExcluir.Click += BtnDesativar_Click;

            pnlForm.Controls.AddRange(new Control[] { lbl1, txtNome, lbl2, numOrdem, lbl3, cmbSituacao,
                                                       btnSalvar, btnCancelar, btnExcluir });

            Controls.Add(grid);
            Controls.Add(topBar);
            Controls.Add(pnlForm);
        }

        private Button Botao(string texto, Color cor)
        {
            return new Button
            {
                Text = texto, BackColor = cor, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Width = 100, Height = 28,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
        }

        private void Carregar()
        {
            try { grid.DataSource = DbHelper.ListarCategorias(); }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            txtNome.Clear();
            numOrdem.Value = 0;
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Visible = true;
            txtNome.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var row = DbHelper.GetCategoria(cod);
            if (row == null) return;
            _codigoEditando = cod;
            txtNome.Text = row["grmeDescricao_"]?.ToString() ?? "";
            numOrdem.Value = Convert.ToDecimal(row["grmeOrdem"] ?? 0);
            var sit = row["Situacao"]?.ToString() ?? "A";
            cmbSituacao.SelectedItem = sit;
            pnlForm.Visible = true;
            txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text)) { MessageBox.Show("Informe o nome."); return; }
            var erro = DbHelper.SalvarCategoria(_codigoEditando, txtNome.Text.Trim(),
                (int)numOrdem.Value, cmbSituacao.SelectedItem?.ToString() ?? "A");
            if (erro != "") { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false;
            _codigoEditando = 0;
            Carregar();
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Desativar categoria?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            DbHelper.SalvarCategoria(cod, grid.SelectedRows[0].Cells["Nome"].Value?.ToString() ?? "",
                0, "I");
            pnlForm.Visible = false;
            Carregar();
        }
    }
}
