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

            // ── Top bar ───────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44,
                BackColor = Color.FromArgb(40, 40, 80) };
            var btnN = new Button
            {
                Text = "+ Novo Fornecedor", Left = 8, Top = 8, Width = 150, Height = 28,
                BackColor = Color.FromArgb(0, 150, 136), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnN.FlatAppearance.BorderSize = 0; btnN.Click += (_, __) => ModoNovo();
            var btnR = new Button
            {
                Text = "Atualizar", Left = 168, Top = 8, Width = 100, Height = 28,
                BackColor = Color.FromArgb(63, 81, 181), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnR.FlatAppearance.BorderSize = 0; btnR.Click += (_, __) => CarregarGrid();
            topBar.Controls.AddRange(new Control[] { btnN, btnR });

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
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 240;
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = false;

            // Linha 1: Razão Social | Nome Fantasia | CNPJ | Situação
            var lblRaz = new Label { Text = "Razão Social:", Left = 10, Top = 11, AutoSize = true };
            txtRazao.Left = 90; txtRazao.Top = 8; txtRazao.Width = 230;

            var lblFan = new Label { Text = "Nome Fantasia:", Left = 332, Top = 11, AutoSize = true };
            txtFantasia.Left = 430; txtFantasia.Top = 8; txtFantasia.Width = 160;

            var lblCnpj = new Label { Text = "CNPJ/CPF:", Left = 600, Top = 11, AutoSize = true };
            txtCnpj.Left = 660; txtCnpj.Top = 8; txtCnpj.Width = 130;

            var lblSit = new Label { Text = "Situação:", Left = 800, Top = 11, AutoSize = true };
            cmbSituacao.Left = 854; cmbSituacao.Top = 8; cmbSituacao.Width = 65;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.Items.AddRange(new object[] { "A", "I" });
            cmbSituacao.SelectedIndex = 0;

            // Linha 2: IE | Telefone | Email | Contato
            var lblIe   = new Label { Text = "Insc. Estadual:", Left = 10, Top = 43, AutoSize = true };
            txtIe.Left = 100; txtIe.Top = 40; txtIe.Width = 120;

            var lblTel  = new Label { Text = "Telefone:", Left = 232, Top = 43, AutoSize = true };
            txtTelefone.Left = 290; txtTelefone.Top = 40; txtTelefone.Width = 120;

            var lblEmail = new Label { Text = "E-mail:", Left = 422, Top = 43, AutoSize = true };
            txtEmail.Left = 467; txtEmail.Top = 40; txtEmail.Width = 200;

            var lblCont = new Label { Text = "Contato:", Left = 678, Top = 43, AutoSize = true };
            txtContato.Left = 730; txtContato.Top = 40; txtContato.Width = 150;

            // Linha 3: CEP | Endereço | Nº | Bairro
            var lblCep  = new Label { Text = "CEP:",      Left = 10,  Top = 75, AutoSize = true };
            txtCep.Left = 42; txtCep.Top = 72; txtCep.Width = 85;

            var lblEnd  = new Label { Text = "Endereço:", Left = 138, Top = 75, AutoSize = true };
            txtEndereco.Left = 200; txtEndereco.Top = 72; txtEndereco.Width = 220;

            var lblNum  = new Label { Text = "Nº:",       Left = 430, Top = 75, AutoSize = true };
            txtNumero.Left = 448; txtNumero.Top = 72; txtNumero.Width = 60;

            var lblBai  = new Label { Text = "Bairro:",   Left = 520, Top = 75, AutoSize = true };
            txtBairro.Left = 565; txtBairro.Top = 72; txtBairro.Width = 150;

            // Linha 4: Cidade | UF
            var lblCid  = new Label { Text = "Cidade:", Left = 10,  Top = 107, AutoSize = true };
            txtCidade.Left = 58; txtCidade.Top = 104; txtCidade.Width = 200;

            var lblUF   = new Label { Text = "UF:",     Left = 268, Top = 107, AutoSize = true };
            txtEstado.Left = 285; txtEstado.Top = 104; txtEstado.Width = 42;

            // Linha 5: Observações
            var lblObs  = new Label { Text = "Observações:", Left = 10, Top = 139, AutoSize = true };
            txtObs.Left = 92; txtObs.Top = 136; txtObs.Width = 450;

            // Botões
            var btnS = new Button { Text = "Salvar",   Left = 10,  Top = 175, Width = 100, Height = 28,
                BackColor = Color.FromArgb(33, 150, 243),  ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += BtnSalvar_Click;

            var btnC = new Button { Text = "Cancelar", Left = 120, Top = 175, Width = 100, Height = 28,
                BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };

            pnlForm.Controls.AddRange(new Control[]
            {
                lblRaz, txtRazao, lblFan, txtFantasia, lblCnpj, txtCnpj, lblSit, cmbSituacao,
                lblIe, txtIe, lblTel, txtTelefone, lblEmail, txtEmail, lblCont, txtContato,
                lblCep, txtCep, lblEnd, txtEndereco, lblNum, txtNumero, lblBai, txtBairro,
                lblCid, txtCidade, lblUF, txtEstado,
                lblObs, txtObs,
                btnS, btnC
            });

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(934, 581);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(860, 540);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cadastro de Fornecedores";
        }

        private DataGridView grid;
        private Panel        pnlForm;
        private TextBox      txtRazao;
        private TextBox      txtFantasia;
        private TextBox      txtCnpj;
        private TextBox      txtIe;
        private TextBox      txtTelefone;
        private TextBox      txtEmail;
        private TextBox      txtContato;
        private TextBox      txtCep;
        private TextBox      txtEndereco;
        private TextBox      txtNumero;
        private TextBox      txtBairro;
        private TextBox      txtCidade;
        private TextBox      txtEstado;
        private TextBox      txtObs;
        private ComboBox     cmbSituacao;
    }
}
