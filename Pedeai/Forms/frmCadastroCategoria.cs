using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public class frmCadastroCategoria : Form
    {
        private readonly GrupoMercadoriaBLL _bll = new GrupoMercadoriaBLL();

        private DataGridView grid = new DataGridView();
        private TextBox txtNome = new TextBox();
        private NumericUpDown numOrdem = new NumericUpDown();
        private ComboBox cmbSituacao = new ComboBox();
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
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8, 6, 8, 0) };
            topBar.BackColor = Color.FromArgb(40, 40, 80);
            var btnNovo = Botao("+ Nova Categoria", Color.FromArgb(0, 150, 136));
            btnNovo.Click += (_, __) => ModoNovo();
            topBar.Controls.Add(btnNovo);

            grid = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, BackgroundColor = Color.White, Font = new Font("Segoe UI", 9), BorderStyle = BorderStyle.None };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            pnlForm = new Panel { Dock = DockStyle.Bottom, Height = 130, Padding = new Padding(10), Visible = false };
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;

            pnlForm.Controls.Add(new Label { Text = "Nome:", AutoSize = true, Left = 10, Top = 14 });
            txtNome = new TextBox { Left = 70, Top = 10, Width = 280 };
            pnlForm.Controls.Add(txtNome);

            pnlForm.Controls.Add(new Label { Text = "Ordem:", AutoSize = true, Left = 370, Top = 14 });
            numOrdem = new NumericUpDown { Left = 430, Top = 10, Width = 60, Minimum = 0, Maximum = 999 };
            pnlForm.Controls.Add(numOrdem);

            pnlForm.Controls.Add(new Label { Text = "Situacao:", AutoSize = true, Left = 510, Top = 14 });
            cmbSituacao = new ComboBox { Left = 575, Top = 10, Width = 80, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSituacao.Items.AddRange(new[] { "A", "I" });
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Controls.Add(cmbSituacao);

            var btnS = Botao("Salvar", Color.FromArgb(33, 150, 243)); btnS.Left = 10; btnS.Top = 50; btnS.Click += BtnSalvar_Click;
            var btnC = Botao("Cancelar", Color.FromArgb(158, 158, 158)); btnC.Left = 120; btnC.Top = 50;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };
            var btnD = Botao("Desativar", Color.FromArgb(244, 67, 54)); btnD.Left = 230; btnD.Top = 50; btnD.Click += BtnDesativar_Click;
            pnlForm.Controls.AddRange(new System.Windows.Forms.Control[] { btnS, btnC, btnD });

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);
        }

        private Button Botao(string t, Color c) => new Button { Text = t, BackColor = c, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Width = 100, Height = 28, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

        private void Carregar()
        {
            try { grid.DataSource = _bll.Listar(); }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0; txtNome.Clear(); numOrdem.Value = 0; cmbSituacao.SelectedIndex = 0;
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando = cod;
            txtNome.Text = obj.grmeDescricao_ ?? "";
            numOrdem.Value = obj.grmeOrdem;
            cmbSituacao.SelectedItem = obj.Situacao;
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            var obj = new GrupoMercadoria
            {
                Codigo       = _codigoEditando,
                grmeDescricao_ = txtNome.Text.Trim(),
                grmeOrdem    = (int)numOrdem.Value,
                Situacao     = cmbSituacao.SelectedItem?.ToString() ?? "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; Carregar();
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Desativar categoria?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            obj.Situacao = "I";
            _bll.Salvar(obj);
            pnlForm.Visible = false; Carregar();
        }
    }
}
