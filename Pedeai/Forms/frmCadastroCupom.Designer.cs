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
            this.ResumeLayout(false);
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
