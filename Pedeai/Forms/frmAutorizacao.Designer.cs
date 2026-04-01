using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmAutorizacao
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtLogin       = new System.Windows.Forms.TextBox();
            this.txtSenha       = new System.Windows.Forms.TextBox();
            this.lblNomeUsuario = new System.Windows.Forms.Label();
            this.lblMsg         = new System.Windows.Forms.Label();
            this.btnOk          = new System.Windows.Forms.Button();
            this.btnCancel      = new System.Windows.Forms.Button();
            this.lblTitulo      = new System.Windows.Forms.Label();
            this.lblSub         = new System.Windows.Forms.Label();
            this.lblLogin       = new System.Windows.Forms.Label();
            this.lblSenha       = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // lblTitulo
            this.lblTitulo.Text      = "\u26A0 Esta acao requer autorizacao";
            this.lblTitulo.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(200, 100, 38);
            this.lblTitulo.AutoSize  = false;
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitulo.SetBounds(10, 14, 360, 28);
            // lblSub
            this.lblSub.Text      = "Informe as credenciais de um usuario autorizado:";
            this.lblSub.ForeColor = System.Drawing.Color.FromArgb(195, 158, 120);
            this.lblSub.AutoSize  = false;
            this.lblSub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSub.SetBounds(10, 44, 360, 22);
            // lblLogin
            this.lblLogin.Text      = "Login:";
            this.lblLogin.ForeColor = System.Drawing.Color.FromArgb(195, 158, 120);
            this.lblLogin.SetBounds(40, 76, 60, 22);
            // txtLogin
            this.txtLogin.SetBounds(40, 98, 300, 28);
            this.txtLogin.BackColor   = System.Drawing.Color.FromArgb(252, 248, 244);
            this.txtLogin.ForeColor   = System.Drawing.Color.FromArgb(42, 20, 8);
            this.txtLogin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLogin.Leave      += new System.EventHandler(this.TxtLogin_Leave);
            this.txtLogin.KeyDown    += new System.Windows.Forms.KeyEventHandler(this.TxtLogin_KeyDown);
            // lblNomeUsuario
            this.lblNomeUsuario.Text      = "";
            this.lblNomeUsuario.ForeColor = System.Drawing.Color.FromArgb(120, 80, 25);
            this.lblNomeUsuario.AutoSize  = false;
            this.lblNomeUsuario.SetBounds(40, 128, 300, 18);
            // lblSenha
            this.lblSenha.Text      = "Senha:";
            this.lblSenha.ForeColor = System.Drawing.Color.FromArgb(195, 158, 120);
            this.lblSenha.SetBounds(40, 150, 60, 22);
            // txtSenha
            this.txtSenha.SetBounds(40, 172, 300, 28);
            this.txtSenha.BackColor    = System.Drawing.Color.FromArgb(252, 248, 244);
            this.txtSenha.ForeColor    = System.Drawing.Color.FromArgb(42, 20, 8);
            this.txtSenha.BorderStyle  = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSenha.PasswordChar = '\u2022';
            this.txtSenha.KeyDown     += new System.Windows.Forms.KeyEventHandler(this.TxtSenha_KeyDown);
            // lblMsg
            this.lblMsg.Text      = "";
            this.lblMsg.ForeColor = System.Drawing.Color.FromArgb(178, 38, 20);
            this.lblMsg.AutoSize  = false;
            this.lblMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMsg.SetBounds(40, 204, 300, 20);
            // btnOk
            this.btnOk.Text      = "\u2714 Autorizar";
            this.btnOk.SetBounds(40, 232, 140, 30);
            this.btnOk.BackColor = System.Drawing.Color.FromArgb(120, 80, 25);
            this.btnOk.ForeColor = System.Drawing.Color.White;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnOk.FlatAppearance.BorderSize = 0;
            this.btnOk.Click += new System.EventHandler(this.BtnAutorizar_Click);
            // btnCancel
            this.btnCancel.Text         = "Cancelar";
            this.btnCancel.SetBounds(200, 232, 140, 30);
            this.btnCancel.BackColor    = System.Drawing.Color.FromArgb(160, 135, 110);
            this.btnCancel.ForeColor    = System.Drawing.Color.White;
            this.btnCancel.FlatStyle    = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Cursor       = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            // Form
            this.AcceptButton    = this.btnOk;
            this.CancelButton    = this.btnCancel;
            this.Text            = "Autorizacao Necessaria";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = System.Drawing.Color.FromArgb(252, 248, 244);
            this.ForeColor       = System.Drawing.Color.FromArgb(42, 20, 8);
            this.Font            = new System.Drawing.Font("Segoe UI", 10F);
            this.ClientSize      = new System.Drawing.Size(380, 276);
            this.Controls.AddRange(new System.Windows.Forms.Control[]
                { this.lblTitulo, this.lblSub, this.lblLogin, this.txtLogin,
                  this.lblNomeUsuario, this.lblSenha, this.txtSenha,
                  this.lblMsg, this.btnOk, this.btnCancel });
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.TextBox  txtLogin;
        internal System.Windows.Forms.TextBox  txtSenha;
        internal System.Windows.Forms.Label    lblNomeUsuario;
        internal System.Windows.Forms.Label    lblMsg;
        internal System.Windows.Forms.Button   btnOk;
        internal System.Windows.Forms.Button   btnCancel;
        private  System.Windows.Forms.Label    lblTitulo;
        private  System.Windows.Forms.Label    lblSub;
        private  System.Windows.Forms.Label    lblLogin;
        private  System.Windows.Forms.Label    lblSenha;
    }
}
