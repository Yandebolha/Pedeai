using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroCliente
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            grid           = new DataGridView();
            pnlForm        = new Panel();
            txtBusca       = new TextBox();
            txtNome        = new TextBox();
            txtEmail       = new TextBox();
            txtCpf         = new TextBox();
            txtEndereco    = new TextBox();
            txtNumero      = new TextBox();
            txtComplemento = new TextBox();
            txtBairro      = new TextBox();
            txtCidade      = new TextBox();
            txtEstado      = new TextBox();
            txtTelefone    = new MaskedTextBox();
            txtCelular     = new MaskedTextBox();
            txtCep         = new MaskedTextBox();
            cmbSituacao    = new ComboBox();

            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 581);
            this.MinimumSize = new System.Drawing.Size(860, 540);
            this.BackColor = System.Drawing.Color.FromArgb(15, 22, 45);
            this.ForeColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Clientes";

            // ── Top bar ──────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Color.FromArgb(36, 48, 82) };
            var lblB = new Label { Text = "Buscar:", ForeColor = Color.White, Left = 8, Top = 12, AutoSize = true };
            txtBusca.Left = 65; txtBusca.Top = 8; txtBusca.Width = 220;
            txtBusca.BackColor = Color.FromArgb(28, 37, 65); txtBusca.ForeColor = Color.White;
            txtBusca.KeyDown += (_, k) => { if (k.KeyCode == Keys.Enter) CarregarGrid(); };
            btnB = new Button
            {
                Text = "Buscar", Left = 295, Top = 8, Width = 100, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnB.FlatAppearance.BorderSize = 0; btnB.Click += (_, __) => CarregarGrid();
            btnN = new Button
            {
                Text = "+ Novo Cliente", Left = 405, Top = 8, Width = 120, Height = 28,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnN.FlatAppearance.BorderSize = 0; btnN.Click += (_, __) => ModoNovo();
            topBar.Controls.AddRange(new Control[] { lblB, txtBusca, btnB, btnN });

            // ── Grid ──────────────────────────────────────────────────────
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true; grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            grid.BackgroundColor = Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.BackColor = Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.ForeColor = Color.White;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(28, 37, 65);
            grid.GridColor = Color.FromArgb(40, 55, 90);
            grid.Font = new Font("Segoe UI", 9F); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36, 48, 82);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 34;
            grid.RowTemplate.Height = 28;
            grid.DoubleClick += (_, __) => CarregarParaEditar();
            grid.DataError += (_, e) => e.ThrowException = false;

            // ── Painel formulário ──────────────────────────────────────────
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 210;
            pnlForm.BackColor = Color.FromArgb(28, 37, 65);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = DesignMode;

            var cLbl = Color.FromArgb(160, 175, 210);
            var cIn  = Color.FromArgb(20, 28, 55);

            var lblNome = new Label { Text = "Nome / Razão Social:", Left = 10, Top = 11, AutoSize = true, ForeColor = cLbl };
            txtNome.Left = 155; txtNome.Top = 8; txtNome.Width = 255;
            txtNome.BackColor = cIn; txtNome.ForeColor = Color.White;

            var lblCpf  = new Label { Text = "CPF / CNPJ:", Left = 422, Top = 11, AutoSize = true, ForeColor = cLbl };
            txtCpf.Left = 500; txtCpf.Top = 8; txtCpf.Width = 160;
            txtCpf.BackColor = cIn; txtCpf.ForeColor = Color.White;
            txtCpf.KeyPress += (s, e) => { if (!System.Char.IsDigit(e.KeyChar) && !System.Char.IsControl(e.KeyChar)) e.Handled = true; };
            txtCpf.TextChanged += TxtCpf_TextChanged;

            var lblSit  = new Label { Text = "Situação:", Left = 672, Top = 11, AutoSize = true, ForeColor = cLbl };
            cmbSituacao.Left = 730; cmbSituacao.Top = 8; cmbSituacao.Width = 95;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.BackColor = Color.FromArgb(20, 28, 55);
            cmbSituacao.ForeColor = Color.White;
            cmbSituacao.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbSituacao.SelectedIndex = 0;

            var lblTel  = new Label { Text = "Telefone:", Left = 10, Top = 47, AutoSize = true, ForeColor = cLbl };
            txtTelefone.Mask = "(00) 0000-0000"; txtTelefone.Left = 72; txtTelefone.Top = 44; txtTelefone.Width = 130;
            txtTelefone.BackColor = cIn; txtTelefone.ForeColor = Color.White;

            var lblCel  = new Label { Text = "Celular:",  Left = 216, Top = 47, AutoSize = true, ForeColor = cLbl };
            txtCelular.Mask = "(00) 00000-0000"; txtCelular.Left = 268; txtCelular.Top = 44; txtCelular.Width = 140;
            txtCelular.BackColor = cIn; txtCelular.ForeColor = Color.White;

            var lblEmail = new Label { Text = "E-mail:", Left = 420, Top = 47, AutoSize = true, ForeColor = cLbl };
            txtEmail.Left = 462; txtEmail.Top = 44; txtEmail.Width = 265;
            txtEmail.BackColor = cIn; txtEmail.ForeColor = Color.White;

            var lblCep  = new Label { Text = "CEP:",     Left = 10,  Top = 83, AutoSize = true, ForeColor = cLbl };
            txtCep.Mask = "00000-000"; txtCep.Left = 44; txtCep.Top = 80; txtCep.Width = 95;
            txtCep.BackColor = cIn; txtCep.ForeColor = Color.White;
            txtCep.Leave += TxtCep_Leave;

            var lblEnd  = new Label { Text = "Endereço:", Left = 152, Top = 83, AutoSize = true, ForeColor = cLbl };
            txtEndereco.Left = 218; txtEndereco.Top = 80; txtEndereco.Width = 225;
            txtEndereco.BackColor = cIn; txtEndereco.ForeColor = Color.White;

            var lblNum  = new Label { Text = "Nº:",      Left = 456, Top = 83, AutoSize = true, ForeColor = cLbl };
            txtNumero.Left = 476; txtNumero.Top = 80; txtNumero.Width = 58;
            txtNumero.BackColor = cIn; txtNumero.ForeColor = Color.White;

            var lblComp = new Label { Text = "Compl.:",  Left = 547, Top = 83, AutoSize = true, ForeColor = cLbl };
            txtComplemento.Left = 594; txtComplemento.Top = 80; txtComplemento.Width = 130;
            txtComplemento.BackColor = cIn; txtComplemento.ForeColor = Color.White;

            var lblBai  = new Label { Text = "Bairro:",  Left = 10,  Top = 119, AutoSize = true, ForeColor = cLbl };
            txtBairro.Left = 58; txtBairro.Top = 116; txtBairro.Width = 188;
            txtBairro.BackColor = cIn; txtBairro.ForeColor = Color.White;

            var lblCid  = new Label { Text = "Cidade:",  Left = 260, Top = 119, AutoSize = true, ForeColor = cLbl };
            txtCidade.Left = 308; txtCidade.Top = 116; txtCidade.Width = 188;
            txtCidade.BackColor = cIn; txtCidade.ForeColor = Color.White;

            var lblUF   = new Label { Text = "UF:",      Left = 508, Top = 119, AutoSize = true, ForeColor = cLbl };
            txtEstado.Left = 528; txtEstado.Top = 116; txtEstado.Width = 50;
            txtEstado.BackColor = cIn; txtEstado.ForeColor = Color.White;

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
                lblNome, txtNome, lblCpf, txtCpf, lblSit, cmbSituacao,
                lblTel, txtTelefone, lblCel, txtCelular, lblEmail, txtEmail,
                lblCep, txtCep, lblEnd, txtEndereco, lblNum, txtNumero, lblComp, txtComplemento,
                lblBai, txtBairro, lblCid, txtCidade, lblUF, txtEstado
            });
            pnlForm.Controls.Add(pnlBtns);

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);
            this.ResumeLayout(false);
        }

        internal DataGridView   grid;
        internal Panel          pnlForm;
        internal TextBox        txtBusca;
        internal TextBox        txtNome;
        internal TextBox        txtEmail;
        internal TextBox        txtCpf;
        internal TextBox        txtEndereco;
        internal TextBox        txtNumero;
        internal TextBox        txtComplemento;
        internal TextBox        txtBairro;
        internal TextBox        txtCidade;
        internal TextBox        txtEstado;
        internal MaskedTextBox  txtTelefone;
        internal MaskedTextBox  txtCelular;
        internal MaskedTextBox  txtCep;
        internal ComboBox       cmbSituacao;
        internal Button         btnB;
        internal Button         btnN;
        internal Button         btnS;
        internal Button         btnC;
    }
}
