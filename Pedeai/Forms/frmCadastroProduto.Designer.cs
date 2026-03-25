using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroProduto
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
            this.ClientSize = new System.Drawing.Size(904, 581);
            this.MinimumSize = new System.Drawing.Size(820, 540);
            this.BackColor = System.Drawing.Color.FromArgb(15, 22, 45);
            this.ForeColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Produtos";
            this.ResumeLayout(false);
        }


        private DataGridView  grid;
        private Panel         pnlForm;
        private ComboBox      cmbCategoria;
        private TextBox       txtNome;
        private ComboBox      cmbSituacao;
        private TextBox       txtDescricao;
        private Label         lblImagem;
        private NumericUpDown numPreco;
        private NumericUpDown numCusto;
        private NumericUpDown numPromo;
        private NumericUpDown numEstoque;
        private CheckBox      chkControlaEstoque;
        private CheckBox      chkDestaque;
        private CheckBox      chkIfood;
        private CheckBox      chkSite;
    }
}
