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
            this.Text            = "Autorizacao Necessaria";
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = this.MinimizeBox = false;
            this.BackColor       = Color.FromArgb(28, 37, 65);
            this.ForeColor       = Color.White;
            this.Font            = new Font("Segoe UI", 10F);
            this.ClientSize      = new Size(380, 276);

            var lblTitulo = new Label
            {
                Text      = "\u26A0 Esta acao requer autorizacao",
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(243, 156, 18),
                AutoSize  = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(10, 14, 360, 28)
            };

            var lblSub = new Label
            {
                Text      = "Informe as credenciais de um usuario autorizado:",
                ForeColor = Color.FromArgb(180, 190, 220),
                AutoSize  = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(10, 44, 360, 22)
            };

            var lblLogin = new Label { Text = "Login:", ForeColor = Color.FromArgb(180, 190, 220), Bounds = new System.Drawing.Rectangle(40, 76, 60, 22) };
            txtLogin = new TextBox
            {
                Bounds      = new System.Drawing.Rectangle(40, 98, 300, 28),
                BackColor   = Color.FromArgb(15, 22, 45),
                ForeColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtLogin.Leave   += TxtLogin_Leave;
            txtLogin.KeyDown += TxtLogin_KeyDown;

            lblNomeUsuario = new Label
            {
                Text      = "",
                ForeColor = Color.FromArgb(39, 174, 96),
                AutoSize  = false,
                Bounds    = new System.Drawing.Rectangle(40, 128, 300, 18)
            };

            var lblSenha = new Label { Text = "Senha:", ForeColor = Color.FromArgb(180, 190, 220), Bounds = new System.Drawing.Rectangle(40, 150, 60, 22) };
            txtSenha = new TextBox
            {
                Bounds        = new System.Drawing.Rectangle(40, 172, 300, 28),
                BackColor     = Color.FromArgb(15, 22, 45),
                ForeColor     = Color.White,
                BorderStyle   = BorderStyle.FixedSingle,
                PasswordChar  = '\u2022'
            };
            txtSenha.KeyDown += TxtSenha_KeyDown;

            lblMsg = new Label
            {
                Text      = "",
                ForeColor = Color.FromArgb(231, 76, 60),
                AutoSize  = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(40, 204, 300, 20)
            };

            btnOk = new Button
            {
                Text      = "\u2714 Autorizar",
                Bounds    = new System.Drawing.Rectangle(40, 232, 140, 30),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += BtnAutorizar_Click;

            btnCancel = new Button
            {
                Text         = "Cancelar",
                Bounds       = new System.Drawing.Rectangle(200, 232, 140, 30),
                BackColor    = Color.FromArgb(108, 117, 125),
                ForeColor    = Color.White,
                FlatStyle    = FlatStyle.Flat,
                Cursor       = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            this.Controls.AddRange(new System.Windows.Forms.Control[]
                { lblTitulo, lblSub, lblLogin, txtLogin, lblNomeUsuario, lblSenha, txtSenha, lblMsg, btnOk, btnCancel });
        }

        internal TextBox  txtLogin;
        internal TextBox  txtSenha;
        internal Label    lblNomeUsuario;
        internal Label    lblMsg;
        internal Button   btnOk;
        internal Button   btnCancel;
    }
}
