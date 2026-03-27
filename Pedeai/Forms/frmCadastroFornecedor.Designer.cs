using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroFornecedor
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
            txtRazao    = new TextBox();
            txtFantasia = new TextBox();
            txtCnpj     = new TextBox();
            txtIe       = new TextBox();
            txtTelefone = new TextBox();
            txtEmail    = new TextBox();
            txtContato  = new TextBox();
            txtCep      = new TextBox();
            txtEndereco = new TextBox();
            txtNumero   = new TextBox();
            txtBairro   = new TextBox();
            txtCidade   = new TextBox();
            txtEstado   = new TextBox();
            txtObs      = new TextBox();
            cmbSituacao = new ComboBox();

            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 581);
            this.MinimumSize = new System.Drawing.Size(860, 540);
            this.BackColor = System.Drawing.Color.FromArgb(15, 22, 45);
            this.ForeColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Fornecedores";

            // ── Top bar ───────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Color.FromArgb(36, 48, 82) };
            btnN = new Button
            {
                Text = "+ Novo Fornecedor", Left = 8, Top = 8, Width = 150, Height = 28,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnN.FlatAppearance.BorderSize = 0; btnN.Click += (_, __) => ModoNovo();
            btnR = new Button
            {
                Text = "Atualizar", Left = 168, Top = 8, Width = 100, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnR.FlatAppearance.BorderSize = 0; btnR.Click += (_, __) => CarregarGrid();
            topBar.Controls.AddRange(new Control[] { btnN, btnR });

            // ── Grid ──────────────────────────────────────────────────────
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true; grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.BackColor = Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.ForeColor = Color.White;
            grid.GridColor = Color.FromArgb(40, 55, 90);
            grid.Font = new Font("Segoe UI", 9F); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36, 48, 82);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();
            grid.DataError += (_, e) => e.ThrowException = false;

            // ── Painel formulário ──────────────────────────────────────────
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 240;
            pnlForm.BackColor = Color.FromArgb(28, 37, 65);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = DesignMode;

            var lblRaz = new Label { Text = "Raz\u00e3o Social:", Left = 10, Top = 11, AutoSize = true };
            txtRazao.Left = 90; txtRazao.Top = 8; txtRazao.Width = 230;

            var lblFan = new Label { Text = "Nome Fantasia:", Left = 332, Top = 11, AutoSize = true };
            txtFantasia.Left = 430; txtFantasia.Top = 8; txtFantasia.Width = 160;

            var lblCnpj = new Label { Text = "CNPJ/CPF:", Left = 600, Top = 11, AutoSize = true };
            txtCnpj.Left = 660; txtCnpj.Top = 8; txtCnpj.Width = 130;

            var lblSit = new Label { Text = "Situa\u00e7\u00e3o:", Left = 800, Top = 11, AutoSize = true };
            cmbSituacao.Left = 854; cmbSituacao.Top = 8; cmbSituacao.Width = 65;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.Items.AddRange(new object[] { "A", "I" });
            cmbSituacao.SelectedIndex = 0;

            var lblIe   = new Label { Text = "Insc. Estadual:", Left = 10, Top = 43, AutoSize = true };
            txtIe.Left = 100; txtIe.Top = 40; txtIe.Width = 120;

            var lblTel  = new Label { Text = "Telefone:", Left = 232, Top = 43, AutoSize = true };
            txtTelefone.Left = 290; txtTelefone.Top = 40; txtTelefone.Width = 120;

            var lblEmail = new Label { Text = "E-mail:", Left = 422, Top = 43, AutoSize = true };
            txtEmail.Left = 467; txtEmail.Top = 40; txtEmail.Width = 200;

            var lblCont = new Label { Text = "Contato:", Left = 678, Top = 43, AutoSize = true };
            txtContato.Left = 730; txtContato.Top = 40; txtContato.Width = 150;

            var lblCep  = new Label { Text = "CEP:",      Left = 10,  Top = 75, AutoSize = true };
            txtCep.Left = 42; txtCep.Top = 72; txtCep.Width = 85;

            var lblEnd  = new Label { Text = "Endere\u00e7o:", Left = 138, Top = 75, AutoSize = true };
            txtEndereco.Left = 200; txtEndereco.Top = 72; txtEndereco.Width = 220;

            var lblNum  = new Label { Text = "N\u00ba:",       Left = 430, Top = 75, AutoSize = true };
            txtNumero.Left = 448; txtNumero.Top = 72; txtNumero.Width = 60;

            var lblBai  = new Label { Text = "Bairro:",   Left = 520, Top = 75, AutoSize = true };
            txtBairro.Left = 565; txtBairro.Top = 72; txtBairro.Width = 150;

            var lblCid  = new Label { Text = "Cidade:", Left = 10,  Top = 107, AutoSize = true };
            txtCidade.Left = 58; txtCidade.Top = 104; txtCidade.Width = 200;

            var lblUF   = new Label { Text = "UF:",     Left = 268, Top = 107, AutoSize = true };
            txtEstado.Left = 285; txtEstado.Top = 104; txtEstado.Width = 42;

            var lblObs  = new Label { Text = "Observa\u00e7\u00f5es:", Left = 10, Top = 139, AutoSize = true };
            txtObs.Left = 92; txtObs.Top = 136; txtObs.Width = 450;

            var pnlBtns = new Panel { Dock = DockStyle.Bottom, Height = 48, BackColor = Color.FromArgb(28, 37, 65) };
            btnS = new Button { Text = "Salvar",   Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += BtnSalvar_Click;

            btnC = new Button { Text = "Cancelar", Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(80, 95, 130), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };

            void CentrarBotoes()
            {
                int x = (pnlBtns.Width - 110 * 2 - 10) / 2;
                if (x < 10) x = 10;
                btnS.Left = x; btnC.Left = x + 120;
            }
            pnlBtns.SizeChanged += (_, __) => CentrarBotoes();
            pnlBtns.Controls.AddRange(new Control[] { btnS, btnC });

            pnlForm.Controls.AddRange(new Control[]
            {
                lblRaz, txtRazao, lblFan, txtFantasia, lblCnpj, txtCnpj, lblSit, cmbSituacao,
                lblIe, txtIe, lblTel, txtTelefone, lblEmail, txtEmail, lblCont, txtContato,
                lblCep, txtCep, lblEnd, txtEndereco, lblNum, txtNumero, lblBai, txtBairro,
                lblCid, txtCidade, lblUF, txtEstado, lblObs, txtObs
            });
            pnlForm.Controls.Add(pnlBtns);

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);
            this.ResumeLayout(false);
        }

        internal DataGridView grid;
        internal Panel        pnlForm;
        internal TextBox      txtRazao;
        internal TextBox      txtFantasia;
        internal TextBox      txtCnpj;
        internal TextBox      txtIe;
        internal TextBox      txtTelefone;
        internal TextBox      txtEmail;
        internal TextBox      txtContato;
        internal TextBox      txtCep;
        internal TextBox      txtEndereco;
        internal TextBox      txtNumero;
        internal TextBox      txtBairro;
        internal TextBox      txtCidade;
        internal TextBox      txtEstado;
        internal TextBox      txtObs;
        internal ComboBox     cmbSituacao;
        internal Button       btnN;
        internal Button       btnR;
        internal Button       btnS;
        internal Button       btnC;
    }
}
