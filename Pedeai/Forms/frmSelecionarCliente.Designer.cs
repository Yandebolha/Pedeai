using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmSelecionarCliente
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            grid         = new DataGridView();
            pnlForm      = new Panel();
            txtBusca     = new TextBox();
            txtNomeCad   = new TextBox();
            txtTelefoneCad = new MaskedTextBox();
            txtCelularCad  = new MaskedTextBox();
            txtEmailCad  = new TextBox();
            txtCpfCad    = new TextBox();
            cmbSituacao  = new ComboBox();

            // ── Top bar ──────────────────────────────────────────────────
            var topBar = new Panel
            {
                Dock = DockStyle.Top, Height = 44,
                BackColor = Color.FromArgb(40, 40, 80)
            };

            var lblB = new Label { Text = "Buscar:", ForeColor = Color.White, Left = 8, Top = 12, AutoSize = true };

            txtBusca.Left = 65; txtBusca.Top = 8; txtBusca.Width = 220;
            txtBusca.KeyDown += TxtBusca_KeyDown;

            btnB = new Button
            {
                Text = "Buscar", Left = 295, Top = 8, Width = 80, Height = 28,
                BackColor = Color.FromArgb(63, 81, 181), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnB.FlatAppearance.BorderSize = 0;
            btnB.Click += BtnBuscar_Click;

            btnN = new Button
            {
                Text = "+ Novo", Left = 385, Top = 8, Width = 80, Height = 28,
                BackColor = Color.FromArgb(0, 150, 136), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnN.FlatAppearance.BorderSize = 0;
            btnN.Click += BtnNovo_Click;

            btnSel = new Button
            {
                Text = "✔ Selecionar", Left = 475, Top = 8, Width = 110, Height = 28,
                BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnSel.FlatAppearance.BorderSize = 0;
            btnSel.Click += BtnSelecionar_Click;

            topBar.Controls.AddRange(new Control[] { lblB, txtBusca, btnB, btnN, btnSel });

            // ── Grid ──────────────────────────────────────────────────────
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true; grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.White;
            grid.Font = new Font("Segoe UI", 9F); grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.DoubleClick += Grid_DoubleClick;

            // ── Painel formulário novo cliente ────────────────────────────
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 110;
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = false;

            // Linha 1: Nome | CPF/CNPJ | Situação
            var lblNome = new Label { Text = "Nome / Razão Social:", Left = 10, Top = 11, AutoSize = true };
            txtNomeCad.Left = 150; txtNomeCad.Top = 8; txtNomeCad.Width = 240;

            var lblCpf  = new Label { Text = "CPF / CNPJ:", Left = 402, Top = 11, AutoSize = true };
            txtCpfCad.Left = 476; txtCpfCad.Top = 8; txtCpfCad.Width = 140;

            var lblSit  = new Label { Text = "Situação:", Left = 628, Top = 11, AutoSize = true };
            cmbSituacao.Left = 682; cmbSituacao.Top = 8; cmbSituacao.Width = 90;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbSituacao.SelectedIndex = 0;

            // Linha 2: Telefone | Celular | E-mail
            var lblTel  = new Label { Text = "Telefone:", Left = 10, Top = 43, AutoSize = true };
            txtTelefoneCad.Mask = "(00) 0000-0000"; txtTelefoneCad.Left = 72; txtTelefoneCad.Top = 40; txtTelefoneCad.Width = 125;

            var lblCel  = new Label { Text = "Celular:", Left = 208, Top = 43, AutoSize = true };
            txtCelularCad.Mask = "(00) 00000-0000"; txtCelularCad.Left = 255; txtCelularCad.Top = 40; txtCelularCad.Width = 135;

            var lblEmail = new Label { Text = "E-mail:", Left = 402, Top = 43, AutoSize = true };
            txtEmailCad.Left = 443; txtEmailCad.Top = 40; txtEmailCad.Width = 230;

            // Botões
            btnS = new Button
            {
                Text = "Salvar", Left = 10, Top = 72, Width = 100, Height = 28,
                BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnS.FlatAppearance.BorderSize = 0;
            btnS.Click += BtnSalvarCad_Click;

            btnC = new Button
            {
                Text = "Cancelar", Left = 120, Top = 72, Width = 100, Height = 28,
                BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.Click += BtnCancelarCad_Click;

            pnlForm.Controls.AddRange(new Control[]
            {
                lblNome, txtNomeCad, lblCpf, txtCpfCad, lblSit, cmbSituacao,
                lblTel, txtTelefoneCad, lblCel, txtCelularCad, lblEmail, txtEmailCad,
                btnS, btnC
            });

            Controls.Add(grid);
            Controls.Add(topBar);
            Controls.Add(pnlForm);

            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(820, 500);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new System.Drawing.Size(700, 400);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Selecionar Cliente";
        }

        private DataGridView  grid;
        private Panel         pnlForm;
        private TextBox       txtBusca;
        private TextBox       txtNomeCad;
        private MaskedTextBox txtTelefoneCad;
        private MaskedTextBox txtCelularCad;
        private TextBox       txtEmailCad;
        private TextBox       txtCpfCad;
        private ComboBox      cmbSituacao;
        internal Button       btnB;
        internal Button       btnN;
        internal Button       btnSel;
        internal Button       btnS;
        internal Button       btnC;
    }
}
