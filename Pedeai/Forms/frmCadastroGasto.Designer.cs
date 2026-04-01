using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroGasto
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dtpData     = new System.Windows.Forms.DateTimePicker();
            this.txtDescricao= new System.Windows.Forms.TextBox();
            this.numValor    = new System.Windows.Forms.NumericUpDown();
            this.txtObs      = new System.Windows.Forms.TextBox();
            this.btnSalvar   = new System.Windows.Forms.Button();
            this.btnCan      = new System.Windows.Forms.Button();
            this.lblData     = new System.Windows.Forms.Label();
            this.lblDesc     = new System.Windows.Forms.Label();
            this.lblVal      = new System.Windows.Forms.Label();
            this.lblObs      = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // lblData
            this.lblData.Text = "Data:";
            this.lblData.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblData.AutoSize = true;
            this.lblData.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblData.SetBounds(20, 19, 100, 22);
            // dtpData
            this.dtpData.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpData.SetBounds(128, 16, 280, 26);
            // lblDesc
            this.lblDesc.Text = "Descricao:";
            this.lblDesc.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblDesc.AutoSize = true;
            this.lblDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDesc.SetBounds(20, 63, 100, 22);
            // txtDescricao
            this.txtDescricao.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.txtDescricao.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtDescricao.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDescricao.SetBounds(128, 60, 280, 26);
            // lblVal
            this.lblVal.Text = "Valor (R$):";
            this.lblVal.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblVal.AutoSize = true;
            this.lblVal.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblVal.SetBounds(20, 107, 100, 22);
            // numValor
            this.numValor.DecimalPlaces = 2;
            this.numValor.Maximum = 999999M;
            this.numValor.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.numValor.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.numValor.SetBounds(128, 104, 140, 26);
            // lblObs
            this.lblObs.Text = "Obs:";
            this.lblObs.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblObs.AutoSize = true;
            this.lblObs.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblObs.SetBounds(20, 151, 100, 22);
            // txtObs
            this.txtObs.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.txtObs.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtObs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtObs.SetBounds(128, 148, 280, 26);
            // btnSalvar
            this.btnSalvar.Text = "\u2714 Salvar";
            this.btnSalvar.SetBounds(128, 188, 130, 30);
            this.btnSalvar.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnSalvar.ForeColor = System.Drawing.Color.White;
            this.btnSalvar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalvar.FlatAppearance.BorderSize = 0;
            this.btnSalvar.Click += new System.EventHandler(this.BtnSalvar_Click);
            // btnCan
            this.btnCan.Text = "Cancelar";
            this.btnCan.SetBounds(268, 188, 130, 30);
            this.btnCan.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
            this.btnCan.ForeColor = System.Drawing.Color.White;
            this.btnCan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCan.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCan.FlatAppearance.BorderSize = 0;
            // Form
            this.AcceptButton    = this.btnSalvar;
            this.CancelButton    = this.btnCan;
            this.Text            = "Lancar Gasto de Material";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = System.Drawing.Color.FromArgb(245, 237, 216);
            this.ForeColor       = System.Drawing.Color.White;
            this.Font            = new System.Drawing.Font("Segoe UI", 10F);
            this.ClientSize      = new System.Drawing.Size(420, 240);
            this.Controls.AddRange(new System.Windows.Forms.Control[]
                { this.lblData, this.dtpData, this.lblDesc, this.txtDescricao,
                  this.lblVal, this.numValor, this.lblObs, this.txtObs,
                  this.btnSalvar, this.btnCan });
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.DateTimePicker  dtpData;
        internal System.Windows.Forms.TextBox         txtDescricao;
        internal System.Windows.Forms.NumericUpDown   numValor;
        internal System.Windows.Forms.TextBox         txtObs;
        internal System.Windows.Forms.Button          btnSalvar;
        internal System.Windows.Forms.Button          btnCan;
        private  System.Windows.Forms.Label           lblData;
        private  System.Windows.Forms.Label           lblDesc;
        private  System.Windows.Forms.Label           lblVal;
        private  System.Windows.Forms.Label           lblObs;
    }
}
