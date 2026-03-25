using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroNecessidade
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dtpData      = new DateTimePicker();
            cmbCategoria = new ComboBox();
            txtDescricao = new TextBox();
            numValor     = new NumericUpDown();

            BackColor       = Color.FromArgb(36, 48, 82);
            ForeColor       = Color.White;
            Font            = new Font("Segoe UI", 9F);
            ClientSize      = new Size(420, 208);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = MinimizeBox = false;
            Text            = "Lancar Necessidade da Empresa";

            var lblData = new Label { Text = "Data:", Left = 12, Top = 16, AutoSize = true };
            dtpData.Left = 90; dtpData.Top = 12; dtpData.Width = 120;
            dtpData.Format = DateTimePickerFormat.Short;
            dtpData.Value  = DateTime.Today;

            var lblCat = new Label { Text = "Categoria:", Left = 12, Top = 52, AutoSize = true };
            cmbCategoria.Left = 90; cmbCategoria.Top = 48; cmbCategoria.Width = 200;
            cmbCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoria.Items.AddRange(new object[]
                { "Aluguel", "Agua", "Energia", "Internet", "Telefone", "Salario", "Manutencao", "Outro" });
            cmbCategoria.SelectedIndex = 0;

            var lblDesc = new Label { Text = "Descricao:", Left = 12, Top = 88, AutoSize = true };
            txtDescricao.Left = 90; txtDescricao.Top = 84; txtDescricao.Width = 306;
            txtDescricao.BackColor = Color.FromArgb(50, 65, 100);
            txtDescricao.ForeColor = Color.White;

            var lblVal = new Label { Text = "Valor R$:", Left = 12, Top = 124, AutoSize = true };
            numValor.Left = 90; numValor.Top = 120; numValor.Width = 130;
            numValor.DecimalPlaces = 2; numValor.Maximum = 9999999;
            numValor.BackColor = Color.FromArgb(50, 65, 100);
            numValor.ForeColor = Color.White;

            var btnSal = new Button
            {
                Text = "Salvar", Left = 110, Top = 162, Width = 110, Height = 30,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnSal.FlatAppearance.BorderSize = 0;
            btnSal.Click += BtnSalvar_Click;

            var btnCanc = new Button
            {
                Text = "Cancelar", Left = 232, Top = 162, Width = 110, Height = 30,
                BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnCanc.FlatAppearance.BorderSize = 0;
            btnCanc.Click += (_, __) => Close();

            Controls.AddRange(new Control[]
                { lblData, dtpData, lblCat, cmbCategoria, lblDesc, txtDescricao,
                  lblVal, numValor, btnSal, btnCanc });
        }

        private DateTimePicker dtpData;
        private ComboBox       cmbCategoria;
        private TextBox        txtDescricao;
        private NumericUpDown  numValor;
    }
}
