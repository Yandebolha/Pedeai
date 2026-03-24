using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroCategoria
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            grid       = new DataGridView();
            pnlForm    = new Panel();
            txtNome    = new TextBox();
            numOrdem   = new NumericUpDown();
            cmbSituacao = new ComboBox();

            // ── Top bar ─────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44,
                BackColor = Color.FromArgb(40, 40, 80) };
            var btnNovo = new Button
            {
                Text = "+ Nova Categoria", Left = 8, Top = 8, Width = 140, Height = 28,
                BackColor = Color.FromArgb(0, 150, 136), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnNovo.FlatAppearance.BorderSize = 0;
            btnNovo.Click += (_, __) => ModoNovo();
            topBar.Controls.Add(btnNovo);

            // ── Grid ─────────────────────────────────────────────────────
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true; grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.White;
            grid.Font = new Font("Segoe UI", 9F); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            // ── Painel formulário ─────────────────────────────────────────
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 130;
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = false;

            var lblNome  = new Label { Text = "Nome:",     AutoSize = true, Left = 10,  Top = 14 };
            txtNome.Left = 70;  txtNome.Top = 10; txtNome.Width = 280;

            var lblOrdem = new Label { Text = "Ordem:",    AutoSize = true, Left = 370, Top = 14 };
            numOrdem.Left = 430; numOrdem.Top = 10; numOrdem.Width = 60;
            numOrdem.Minimum = 0; numOrdem.Maximum = 999;

            var lblSit   = new Label { Text = "Situação:", AutoSize = true, Left = 510, Top = 14 };
            cmbSituacao.Left = 575; cmbSituacao.Top = 10; cmbSituacao.Width = 80;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.Items.AddRange(new object[] { "A", "I" });
            cmbSituacao.SelectedIndex = 0;

            var btnS = new Button { Text = "Salvar",    Left = 10,  Top = 50, Width = 100, Height = 28,
                BackColor = Color.FromArgb(33, 150, 243),  ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += BtnSalvar_Click;

            var btnC = new Button { Text = "Cancelar",  Left = 120, Top = 50, Width = 100, Height = 28,
                BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };

            var btnD = new Button { Text = "Desativar", Left = 230, Top = 50, Width = 100, Height = 28,
                BackColor = Color.FromArgb(244, 67, 54),   ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnD.FlatAppearance.BorderSize = 0; btnD.Click += BtnDesativar_Click;

            pnlForm.Controls.AddRange(new Control[]
                { lblNome, txtNome, lblOrdem, numOrdem, lblSit, cmbSituacao, btnS, btnC, btnD });

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);

            // ── Propriedades do Form ──────────────────────────────────────
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(684, 481);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(600, 450);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Categorias de Produtos";
        }

        private DataGridView grid;
        private Panel        pnlForm;
        private TextBox      txtNome;
        private NumericUpDown numOrdem;
        private ComboBox     cmbSituacao;
    }
}
