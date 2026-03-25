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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 581);
            this.MinimumSize = new System.Drawing.Size(860, 540);
            this.BackColor = System.Drawing.Color.FromArgb(15, 22, 45);
            this.ForeColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Clientes";
            this.ResumeLayout(false);
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
