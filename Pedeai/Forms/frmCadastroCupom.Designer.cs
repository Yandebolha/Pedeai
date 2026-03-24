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

            // ── Top bar ──────────────────────────────────────────────────
            var topBar = new Panel { Dock = DockStyle.Top, Height = 44,
                BackColor = Color.FromArgb(40, 40, 80) };
            var btnN = new Button
            {
                Text = "+ Novo Cupom", Left = 8, Top = 8, Width = 110, Height = 28,
                BackColor = Color.FromArgb(0, 150, 136), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnN.FlatAppearance.BorderSize = 0; btnN.Click += (_, __) => ModoNovo();
            var btnR = new Button
            {
                Text = "Atualizar", Left = 128, Top = 8, Width = 100, Height = 28,
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
            pnlForm.Dock = DockStyle.Bottom; pnlForm.Height = 150;
            pnlForm.BackColor = Color.FromArgb(245, 245, 250);
            pnlForm.BorderStyle = BorderStyle.FixedSingle;
            pnlForm.Visible = false;

            // Linha 1
            var lblCod  = new Label { Text = "Código:",    AutoSize = true, Left = 10,  Top = 11 };
            txtCodigo.Left = 65; txtCodigo.Top = 8; txtCodigo.Width = 100;

            var lblDesc = new Label { Text = "Descrição:", AutoSize = true, Left = 175, Top = 11 };
            txtDescricao.Left = 245; txtDescricao.Top = 8; txtDescricao.Width = 200;

            var lblTipo = new Label { Text = "Tipo:",      AutoSize = true, Left = 455, Top = 11 };
            cmbTipo.Left = 490; cmbTipo.Top = 8; cmbTipo.Width = 100;
            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipo.Items.AddRange(new object[] { "PERCENTUAL", "VALOR" });
            cmbTipo.SelectedIndex = 0;

            var lblSit  = new Label { Text = "Situação:",  AutoSize = true, Left = 601, Top = 11 };
            cmbSituacao.Left = 665; cmbSituacao.Top = 8; cmbSituacao.Width = 90;
            cmbSituacao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSituacao.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbSituacao.SelectedIndex = 0;

            // Linha 2
            var lblVal  = new Label { Text = "Valor:",         AutoSize = true, Left = 10,  Top = 45 };
            numValor.Left = 55;  numValor.Top = 42;  numValor.Width = 80;
            numValor.DecimalPlaces = 2; numValor.Maximum = 9999;

            var lblMin  = new Label { Text = "Ped. Mínimo:",   AutoSize = true, Left = 145, Top = 45 };
            numMinimo.Left = 230; numMinimo.Top = 42; numMinimo.Width = 80;
            numMinimo.DecimalPlaces = 2; numMinimo.Maximum = 9999;

            var lblLim  = new Label { Text = "Limite Usos:",   AutoSize = true, Left = 320, Top = 45 };
            numLimite.Left = 404; numLimite.Top = 42; numLimite.Width = 70;
            numLimite.Minimum = 0; numLimite.Maximum = 99999;

            var lblVal2 = new Label { Text = "Válido até:",    AutoSize = true, Left = 484, Top = 45 };
            dtpValido.Left = 555; dtpValido.Top = 42; dtpValido.Width = 120;
            dtpValido.Format = DateTimePickerFormat.Short;

            // Botões
            var btnS = new Button { Text = "Salvar",    Left = 10,  Top = 84, Width = 100, Height = 28,
                BackColor = Color.FromArgb(33, 150, 243),  ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += BtnSalvar_Click;

            var btnC = new Button { Text = "Cancelar",  Left = 120, Top = 84, Width = 100, Height = 28,
                BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.Click += (_, __) => { pnlForm.Visible = false; _codigoEditando = 0; };

            var btnD = new Button { Text = "Desativar", Left = 230, Top = 84, Width = 100, Height = 28,
                BackColor = Color.FromArgb(244, 67, 54),   ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            btnD.FlatAppearance.BorderSize = 0; btnD.Click += BtnDesativar_Click;

            pnlForm.Controls.AddRange(new Control[]
            {
                lblCod, txtCodigo, lblDesc, txtDescricao, lblTipo, cmbTipo, lblSit, cmbSituacao,
                lblVal, numValor, lblMin, numMinimo, lblLim, numLimite, lblVal2, dtpValido,
                btnS, btnC, btnD
            });

            Controls.Add(grid); Controls.Add(topBar); Controls.Add(pnlForm);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(834, 481);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(750, 450);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cadastro de Cupons";
        }

        private DataGridView  grid;
        private Panel         pnlForm;
        private TextBox       txtCodigo;
        private TextBox       txtDescricao;
        private ComboBox      cmbTipo;
        private ComboBox      cmbSituacao;
        private NumericUpDown numValor;
        private NumericUpDown numMinimo;
        private NumericUpDown numLimite;
        private DateTimePicker dtpValido;
    }
}
