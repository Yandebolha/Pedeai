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
            this.dtpData      = new System.Windows.Forms.DateTimePicker();
            this.cmbCategoria = new System.Windows.Forms.ComboBox();
            this.txtDescricao = new System.Windows.Forms.TextBox();
            this.numValor     = new System.Windows.Forms.NumericUpDown();
            this.btnSal       = new System.Windows.Forms.Button();
            this.btnCanc      = new System.Windows.Forms.Button();
            this.lblData      = new System.Windows.Forms.Label();
            this.lblCat       = new System.Windows.Forms.Label();
            this.lblDesc      = new System.Windows.Forms.Label();
            this.lblVal       = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // lblData
            this.lblData.Text = "Data:";
            this.lblData.Left = 12;
            this.lblData.Top  = 16;
            this.lblData.AutoSize = true;
            // dtpData
            this.dtpData.Left = 90;
            this.dtpData.Top  = 12;
            this.dtpData.Width = 120;
            this.dtpData.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            // lblCat
            this.lblCat.Text = "Categoria:";
            this.lblCat.Left = 12;
            this.lblCat.Top  = 52;
            this.lblCat.AutoSize = true;
            // cmbCategoria
            this.cmbCategoria.Left = 90;
            this.cmbCategoria.Top  = 48;
            this.cmbCategoria.Width = 200;
            this.cmbCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoria.Items.AddRange(new object[]
                { "Aluguel", "Agua", "Energia", "Internet", "Telefone", "Salario", "Manutencao", "Outro" });
            this.cmbCategoria.SelectedIndex = 0;
            // lblDesc
            this.lblDesc.Text = "Descricao:";
            this.lblDesc.Left = 12;
            this.lblDesc.Top  = 88;
            this.lblDesc.AutoSize = true;
            // txtDescricao
            this.txtDescricao.Left = 90;
            this.txtDescricao.Top  = 84;
            this.txtDescricao.Width = 306;
            this.txtDescricao.BackColor = System.Drawing.Color.FromArgb(248, 242, 235);
            this.txtDescricao.ForeColor = System.Drawing.Color.FromArgb(42, 20, 8);
            // lblVal
            this.lblVal.Text = "Valor R$:";
            this.lblVal.Left = 12;
            this.lblVal.Top  = 124;
            this.lblVal.AutoSize = true;
            // numValor
            this.numValor.Left = 90;
            this.numValor.Top  = 120;
            this.numValor.Width = 130;
            this.numValor.DecimalPlaces = 2;
            this.numValor.Maximum = 9999999;
            this.numValor.BackColor = System.Drawing.Color.FromArgb(248, 242, 235);
            this.numValor.ForeColor = System.Drawing.Color.FromArgb(42, 20, 8);
            // btnSal
            this.btnSal.Text = "Salvar";
            this.btnSal.Left = 110;
            this.btnSal.Top  = 162;
            this.btnSal.Width  = 110;
            this.btnSal.Height = 30;
            this.btnSal.BackColor = System.Drawing.Color.FromArgb(200, 70, 20);
            this.btnSal.ForeColor = System.Drawing.Color.White;
            this.btnSal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSal.FlatAppearance.BorderSize = 0;
            this.btnSal.Click += new System.EventHandler(this.BtnSalvar_Click);
            // btnCanc
            this.btnCanc.Text = "Cancelar";
            this.btnCanc.Left = 232;
            this.btnCanc.Top  = 162;
            this.btnCanc.Width  = 110;
            this.btnCanc.Height = 30;
            this.btnCanc.BackColor = System.Drawing.Color.FromArgb(160, 135, 110);
            this.btnCanc.ForeColor = System.Drawing.Color.White;
            this.btnCanc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCanc.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCanc.FlatAppearance.BorderSize = 0;
            this.btnCanc.Click += new System.EventHandler(this.BtnCanc_Click);
            // Form
            this.BackColor       = System.Drawing.Color.FromArgb(45, 22, 10);
            this.ForeColor       = System.Drawing.Color.FromArgb(42, 20, 8);
            this.Font            = new System.Drawing.Font("Segoe UI", 9F);
            this.ClientSize      = new System.Drawing.Size(420, 208);
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.Text            = "Lancar Necessidade da Empresa";
            this.Controls.AddRange(new System.Windows.Forms.Control[]
                { this.lblData, this.dtpData, this.lblCat, this.cmbCategoria,
                  this.lblDesc, this.txtDescricao, this.lblVal, this.numValor,
                  this.btnSal, this.btnCanc });
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DateTimePicker dtpData;
        private System.Windows.Forms.ComboBox       cmbCategoria;
        private System.Windows.Forms.TextBox        txtDescricao;
        private System.Windows.Forms.NumericUpDown  numValor;
        internal System.Windows.Forms.Button        btnSal;
        internal System.Windows.Forms.Button        btnCanc;
        private System.Windows.Forms.Label          lblData;
        private System.Windows.Forms.Label          lblCat;
        private System.Windows.Forms.Label          lblDesc;
        private System.Windows.Forms.Label          lblVal;

        private void BtnCanc_Click(object s, System.EventArgs e) { Close(); }
    }
}
