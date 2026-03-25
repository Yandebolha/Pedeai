using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmLogin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // Form
            this.Text            = "Pedeai — Login";
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = this.MinimizeBox = false;
            this.BackColor       = Color.FromArgb(15, 22, 45);
            this.ForeColor       = Color.White;
            this.Font            = new Font("Segoe UI", 10F);
            this.ClientSize      = new Size(360, 320);

            // Logo / titulo
            var lblTitle = new Label
            {
                Text      = "PEDEAI",
                Font      = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(0, 28, 360, 50)
            };

            var lblSub = new Label
            {
                Text      = "Sistema de Gestao de Pedidos",
                Font      = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(150, 160, 190),
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(0, 75, 360, 24)
            };

            // Linha separadora
            var sep = new Panel
            {
                BackColor = Color.FromArgb(52, 152, 219),
                Bounds    = new System.Drawing.Rectangle(60, 108, 240, 1)
            };

            // Login
            var lblLogin = new Label { Text = "Login:", ForeColor = Color.FromArgb(180, 190, 220), Bounds = new System.Drawing.Rectangle(60, 124, 60, 22) };
            txtLogin = new TextBox
            {
                Bounds        = new System.Drawing.Rectangle(60, 146, 240, 28),
                BackColor     = Color.FromArgb(28, 37, 65),
                ForeColor     = Color.White,
                BorderStyle   = BorderStyle.FixedSingle,
                Font          = new Font("Segoe UI", 10F)
            };
            txtLogin.KeyDown += TxtLogin_KeyDown;
            txtLogin.Leave  += TxtLogin_Leave;

            // Nome do usuario (exibido ao sair do campo login)
            lblNomeUsuario = new Label
            {
                Text      = "",
                ForeColor = Color.FromArgb(52, 200, 120),
                Font      = new Font("Segoe UI", 9F, FontStyle.Italic),
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Bounds    = new System.Drawing.Rectangle(60, 176, 240, 18)
            };

            // Senha
            var lblSenha = new Label { Text = "Senha:", ForeColor = Color.FromArgb(180, 190, 220), Bounds = new System.Drawing.Rectangle(60, 196, 60, 22) };
            txtSenha = new TextBox
            {
                Bounds        = new System.Drawing.Rectangle(60, 218, 240, 28),
                BackColor     = Color.FromArgb(28, 37, 65),
                ForeColor     = Color.White,
                BorderStyle   = BorderStyle.FixedSingle,
                Font          = new Font("Segoe UI", 10F),
                PasswordChar  = '\u2022'
            };
            txtSenha.KeyDown += TxtSenha_KeyDown;

            // Mensagem de erro
            lblMensagem = new Label
            {
                Text      = "",
                ForeColor = Color.FromArgb(231, 76, 60),
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Segoe UI", 9F),
                Bounds    = new System.Drawing.Rectangle(60, 252, 240, 20)
            };

            // Botao Entrar
            var btnEntrar = new Button
            {
                Text      = "Entrar",
                Bounds    = new System.Drawing.Rectangle(60, 278, 240, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnEntrar.FlatAppearance.BorderSize = 0;
            btnEntrar.Click += BtnEntrar_Click;
            this.AcceptButton = btnEntrar;

            this.Controls.AddRange(new System.Windows.Forms.Control[]
                { lblTitle, lblSub, sep, lblLogin, txtLogin, lblNomeUsuario, lblSenha, txtSenha, lblMensagem, btnEntrar });
        }

        internal TextBox txtLogin;
        internal TextBox txtSenha;
        internal Label   lblMensagem;
        internal Label   lblNomeUsuario;
    }
}
