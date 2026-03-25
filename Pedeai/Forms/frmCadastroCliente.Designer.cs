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
            grid        = new DataGridView();
            pnlForm     = new Panel();
            txtBusca    = new TextBox();
            txtNome     = new TextBox();
            txtEmail    = new TextBox();
            txtCpf      = new TextBox();
            txtEndereco = new TextBox();
            txtNumero   = new TextBox();
            txtComplemento = new TextBox();
            txtBairro   = new TextBox();
            txtCidade   = new TextBox();
            txtEstado   = new TextBox();
            txtTelefone = new MaskedTextBox();
            txtCelular  = new MaskedTextBox();
            txtCep      = new MaskedTextBox();
            cmbSituacao = new ComboBox();

            // ── Top bar ──────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44,
                BackColor = Color.FromArgb(40, 40, 80) };
            var lblB = new Label { Text = "Buscar:", ForeColor = Color.White,
                Left = 8, Top = 12, AutoSize = true };
            txtBusca.Left = 65; txtBusca.Top = 8; txtBusca.Width = 220;
            txtBusca.KeyDown += (_, k) => { if (k.KeyCode == Keys.Enter) CarregarGrid(); };
            var btnB = new Button
            {
                Text = "Buscar", Left = 295, Top = 8, Width = 100, Height = 28,
                BackColor = Color.FromArgb(63, 81, 181), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnB.FlatAppearance.BorderSize = 0; btnB.Click += (_, __) => CarregarGrid();
            var btnN = new Button
            {
                Text = "+ Novo Cliente", Left = 405, Top = 8, Width = 120, Height = 28,
                BackColor = Color.FromArgb(0, 150, 136), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnN.FlatAppearance.BorderSize = 0; btnN.Click += (_, __) => ModoNovo();
            topBar.Controls.AddRange(new Control[] { lblB, txtBusca, btnB, btnN });

            // ── Grid ──────────────────────────────────────────────────────
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true; grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.White;
            grid.Font = new Font("Segoe UI", 9F); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            // ── Painel formulário ──────────────────────────────────────────
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 195;
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = false;

            // Linha 1: Nome | CPF | Situação
            var lblNome = new Label { Text = "Nome / Razão Social:", Left = 10, Top = 11, AutoSize = true };
            txtNome.Left = 150; txtNome.Top = 8; txtNome.Width = 260;

            var lblCpf  = new Label { Text = "CPF / CNPJ:", Left = 422, Top = 11, AutoSize = true };
            txtCpf.Left = 500; txtCpf.Top = 8; txtCpf.Width = 160;
            txtCpf.KeyPress += (s, e) => { if (!System.Char.IsDigit(e.KeyChar) && !System.Char.IsControl(e.KeyChar)) e.Handled = true; };
            txtCpf.TextChanged += TxtCpf_TextChanged;

            var lblSit  = new Label { Text = "Situação:", Left = 670, Top = 11, AutoSize = true };
            cmbSituacao.Left = 724; cmbSituacao.Top = 8; cmbSituacao.Width = 100;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbSituacao.SelectedIndex = 0;

            // Linha 2: Telefone | Celular | E-mail
            var lblTel  = new Label { Text = "Telefone:", Left = 10, Top = 43, AutoSize = true };
            txtTelefone.Mask = "(00) 0000-0000"; txtTelefone.Left = 72; txtTelefone.Top = 40; txtTelefone.Width = 130;

            var lblCel  = new Label { Text = "Celular:",  Left = 214, Top = 43, AutoSize = true };
            txtCelular.Mask = "(00) 00000-0000"; txtCelular.Left = 265; txtCelular.Top = 40; txtCelular.Width = 140;

            var lblEmail = new Label { Text = "E-mail:", Left = 416, Top = 43, AutoSize = true };
            txtEmail.Left = 455; txtEmail.Top = 40; txtEmail.Width = 264;

            // Linha 3: CEP | Endereço | Nº | Compl.
            var lblCep  = new Label { Text = "CEP:",     Left = 10,  Top = 75, AutoSize = true };
            txtCep.Mask = "00000-000"; txtCep.Left = 42; txtCep.Top = 72; txtCep.Width = 95;
            txtCep.Leave += TxtCep_Leave;

            var lblEnd  = new Label { Text = "Endereço:", Left = 148, Top = 75, AutoSize = true };
            txtEndereco.Left = 210; txtEndereco.Top = 72; txtEndereco.Width = 230;

            var lblNum  = new Label { Text = "Nº:",      Left = 452, Top = 75, AutoSize = true };
            txtNumero.Left = 468; txtNumero.Top = 72; txtNumero.Width = 60;

            var lblComp = new Label { Text = "Compl.:",  Left = 540, Top = 75, AutoSize = true };
            txtComplemento.Left = 582; txtComplemento.Top = 72; txtComplemento.Width = 134;

            // Linha 4: Bairro | Cidade | UF
            var lblBai  = new Label { Text = "Bairro:",  Left = 10,  Top = 107, AutoSize = true };
            txtBairro.Left = 55; txtBairro.Top = 104; txtBairro.Width = 190;

            var lblCid  = new Label { Text = "Cidade:",  Left = 257, Top = 107, AutoSize = true };
            txtCidade.Left = 305; txtCidade.Top = 104; txtCidade.Width = 190;

            var lblUF   = new Label { Text = "UF:",      Left = 507, Top = 107, AutoSize = true };
            txtEstado.Left = 522; txtEstado.Top = 104; txtEstado.Width = 50;

            // Botões
            var btnS = new Button { Text = "Salvar",    Left = 10,  Top = 140, Width = 100, Height = 28,
                BackColor = Color.FromArgb(33, 150, 243),  ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += BtnSalvar_Click;

            var btnC = new Button { Text = "Cancelar",  Left = 120, Top = 140, Width = 100, Height = 28,
                BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };

            pnlForm.Controls.AddRange(new Control[]
            {
                lblNome, txtNome, lblCpf, txtCpf, lblSit, cmbSituacao,
                lblTel, txtTelefone, lblCel, txtCelular, lblEmail, txtEmail,
                lblCep, txtCep, lblEnd, txtEndereco, lblNum, txtNumero, lblComp, txtComplemento,
                lblBai, txtBairro, lblCid, txtCidade, lblUF, txtEstado,
                btnS, btnC
            });

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(934, 581);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(860, 540);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cadastro de Clientes";
        }

        private DataGridView   grid;
        private Panel          pnlForm;
        private TextBox        txtBusca;
        private TextBox        txtNome;
        private TextBox        txtEmail;
        private TextBox        txtCpf;
        private TextBox        txtEndereco;
        private TextBox        txtNumero;
        private TextBox        txtComplemento;
        private TextBox        txtBairro;
        private TextBox        txtCidade;
        private TextBox        txtEstado;
        private MaskedTextBox  txtTelefone;
        private MaskedTextBox  txtCelular;
        private MaskedTextBox  txtCep;
        private ComboBox       cmbSituacao;
    }
}
