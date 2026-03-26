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
            grid        = new DataGridView();
            pnlForm     = new Panel();
            txtNome     = new TextBox();
            cmbSituacao = new ComboBox();

            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 481);
            this.MinimumSize = new System.Drawing.Size(600, 450);
            this.BackColor = System.Drawing.Color.FromArgb(15, 22, 45);
            this.ForeColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Categorias de Produtos";

            // ── Top bar ─────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44,
                BackColor = Color.FromArgb(36, 48, 82) };
            btnNovo = new Button
            {
                Text = "+ Nova Categoria", Left = 8, Top = 8, Width = 140, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White,
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
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.BackColor = Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.ForeColor = Color.White;
            grid.Font = new Font("Segoe UI", 9F); grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Color.FromArgb(40, 55, 90);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36, 48, 82);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            // ── Painel formulário ─────────────────────────────────────────
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 130;
            pnlForm.BackColor = Color.FromArgb(28, 37, 65);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = false;

            var lblNome  = new Label { Text = "Nome:", ForeColor = Color.FromArgb(180, 190, 220), AutoSize = true, Left = 20, Top = 18 };
            txtNome.Left = 70; txtNome.Top = 14; txtNome.Width = 320;
            txtNome.BackColor = Color.FromArgb(20, 28, 55); txtNome.ForeColor = Color.White; txtNome.BorderStyle = BorderStyle.FixedSingle;

            var lblSit = new Label { Text = "Situa\u00e7\u00e3o:", ForeColor = Color.FromArgb(180, 190, 220), AutoSize = true, Left = 402, Top = 18 };
            cmbSituacao.Left = 465; cmbSituacao.Top = 14; cmbSituacao.Width = 80;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.Items.AddRange(new object[] { "A", "I" });
            cmbSituacao.SelectedIndex = 0;

            var pnlBtns = new Panel { Dock = DockStyle.Bottom, Height = 48, BackColor = Color.FromArgb(28, 37, 65) };
            btnS = new Button { Text = "Salvar",    Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += BtnSalvar_Click;

            btnC = new Button { Text = "Cancelar",  Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(80, 95, 130), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };

            btnD = new Button { Text = "Desativar", Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnD.FlatAppearance.BorderSize = 0; btnD.Click += BtnDesativar_Click;

            void CentrarBotoes()
            {
                int x = (pnlBtns.Width - 110 * 3 - 10 * 2) / 2;
                if (x < 10) x = 10;
                btnS.Left = x; btnC.Left = x + 120; btnD.Left = x + 240;
            }
            pnlBtns.SizeChanged += (_, __) => CentrarBotoes();
            pnlBtns.Controls.AddRange(new Control[] { btnS, btnC, btnD });

            pnlForm.Controls.AddRange(new Control[] { lblNome, txtNome, lblSit, cmbSituacao });
            pnlForm.Controls.Add(pnlBtns);

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);
            this.ResumeLayout(false);
        }

        internal DataGridView grid;
        internal Panel        pnlForm;
        internal TextBox      txtNome;
        internal ComboBox     cmbSituacao;
        internal Button       btnNovo;
        internal Button       btnS;
        internal Button       btnC;
        internal Button       btnD;
    }
}
