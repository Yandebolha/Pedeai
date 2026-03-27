using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroCupom
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
            txtCodigo   = new TextBox();
            txtDescricao = new TextBox();
            cmbTipo     = new ComboBox();
            cmbSituacao = new ComboBox();
            numValor    = new NumericUpDown();
            numMinimo   = new NumericUpDown();
            numLimite   = new NumericUpDown();
            dtpValido   = new DateTimePicker();

            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 481);
            this.MinimumSize = new System.Drawing.Size(750, 450);
            this.BackColor = System.Drawing.Color.FromArgb(15, 22, 45);
            this.ForeColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Cupons";

            // ── Top bar ──────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Color.FromArgb(36, 48, 82) };
            btnN = new Button
            {
                Text = "+ Novo Cupom", Left = 8, Top = 8, Width = 110, Height = 28,
                BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnN.FlatAppearance.BorderSize = 0; btnN.Click += (_, __) => ModoNovo();
            btnR = new Button
            {
                Text = "Atualizar", Left = 128, Top = 8, Width = 100, Height = 28,
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
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 150;
            pnlForm.BackColor = Color.FromArgb(28, 37, 65);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = DesignMode;

            var lblCod  = new Label { Text = "C\u00f3digo:",    AutoSize = true, Left = 10,  Top = 11 };
            txtCodigo.Left = 65; txtCodigo.Top = 8; txtCodigo.Width = 100;

            var lblDesc = new Label { Text = "Descri\u00e7\u00e3o:", AutoSize = true, Left = 175, Top = 11 };
            txtDescricao.Left = 245; txtDescricao.Top = 8; txtDescricao.Width = 200;

            var lblTipo = new Label { Text = "Tipo:",      AutoSize = true, Left = 455, Top = 11 };
            cmbTipo.Left = 490; cmbTipo.Top = 8; cmbTipo.Width = 100;
            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipo.Items.AddRange(new object[] { "PERCENTUAL", "VALOR" });
            cmbTipo.SelectedIndex = 0;

            var lblSit  = new Label { Text = "Situa\u00e7\u00e3o:",  AutoSize = true, Left = 601, Top = 11 };
            cmbSituacao.Left = 665; cmbSituacao.Top = 8; cmbSituacao.Width = 90;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbSituacao.SelectedIndex = 0;

            var lblVal  = new Label { Text = "Valor:",         AutoSize = true, Left = 10,  Top = 45 };
            numValor.Left = 55;  numValor.Top = 42;  numValor.Width = 80;
            numValor.DecimalPlaces = 2; numValor.Maximum = 9999;

            var lblMin  = new Label { Text = "Ped. M\u00ednimo:",   AutoSize = true, Left = 145, Top = 45 };
            numMinimo.Left = 230; numMinimo.Top = 42; numMinimo.Width = 80;
            numMinimo.DecimalPlaces = 2; numMinimo.Maximum = 9999;

            var lblLim  = new Label { Text = "Limite Usos:",   AutoSize = true, Left = 320, Top = 45 };
            numLimite.Left = 404; numLimite.Top = 42; numLimite.Width = 70;
            numLimite.Minimum = 0; numLimite.Maximum = 99999;

            var lblVal2 = new Label { Text = "V\u00e1lido at\u00e9:",    AutoSize = true, Left = 484, Top = 45 };
            dtpValido.Left = 555; dtpValido.Top = 42; dtpValido.Width = 120;
            dtpValido.Format = DateTimePickerFormat.Short;

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

            pnlForm.Controls.AddRange(new Control[]
            {
                lblCod, txtCodigo, lblDesc, txtDescricao, lblTipo, cmbTipo, lblSit, cmbSituacao,
                lblVal, numValor, lblMin, numMinimo, lblLim, numLimite, lblVal2, dtpValido
            });
            pnlForm.Controls.Add(pnlBtns);

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);
            this.ResumeLayout(false);
        }

        internal DataGridView  grid;
        internal Panel         pnlForm;
        internal TextBox       txtCodigo;
        internal TextBox       txtDescricao;
        internal ComboBox      cmbTipo;
        internal ComboBox      cmbSituacao;
        internal NumericUpDown numValor;
        internal NumericUpDown numMinimo;
        internal NumericUpDown numLimite;
        internal DateTimePicker dtpValido;
        internal Button        btnN;
        internal Button        btnR;
        internal Button        btnS;
        internal Button        btnC;
        internal Button        btnD;
    }
}
