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

        private void BuildUI()
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
                BackColor = Color.FromArgb(36, 48, 82) };
            var lblB = new Label { Text = "Buscar:", ForeColor = Color.White,
                Left = 8, Top = 12, AutoSize = true };
            txtBusca.Left = 65; txtBusca.Top = 8; txtBusca.Width = 220;
            txtBusca.BackColor = Color.FromArgb(28, 37, 65); txtBusca.ForeColor = Color.White;
            txtBusca.KeyDown += (_, k) => { if (k.KeyCode == Keys.Enter) CarregarGrid(); };
            var btnB = new Button
            {
                Text = "Buscar", Left = 295, Top = 8, Width = 100, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnB.FlatAppearance.BorderSize = 0; btnB.Click += (_, __) => CarregarGrid();
            var btnN = new Button
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
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.BackColor = Color.FromArgb(20, 28, 55);
            grid.DefaultCellStyle.ForeColor = Color.White;
            grid.GridColor = Color.FromArgb(40, 55, 90);
            grid.Font = new Font("Segoe UI", 9F); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(36, 48, 82);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += (_, __) => CarregarParaEditar();

            // ── Painel formulário ──────────────────────────────────────────
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 195;
            pnlForm.BackColor = Color.FromArgb(28, 37, 65);
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
            var pnlBtns = new Panel { Dock = DockStyle.Bottom, Height = 48,
                BackColor = Color.FromArgb(28, 37, 65) };
            var btnS = new Button { Text = "Salvar",    Top = 10, Width = 110, Height = 28,
                BackColor = Color.FromArgb(52, 152, 219),  ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += BtnSalvar_Click;

            var btnC = new Button { Text = "Cancelar",  Top = 10, Width = 110, Height = 28,
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

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(934, 581);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(860, 540);
            BackColor = Color.FromArgb(15, 22, 45);
            ForeColor = Color.White;
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
