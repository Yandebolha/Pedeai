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
            this.txtLogin       = new System.Windows.Forms.TextBox();
            this.txtSenha       = new System.Windows.Forms.TextBox();
            this.lblMensagem    = new System.Windows.Forms.Label();
            this.lblNomeUsuario = new System.Windows.Forms.Label();
            this.btnFechar      = new System.Windows.Forms.Button();
            this.btnEntrar      = new System.Windows.Forms.Button();
            this.pnlLeft        = new System.Windows.Forms.Panel();
            this.pnlRight       = new System.Windows.Forms.Panel();
            this.picLogo        = new System.Windows.Forms.PictureBox();
            this.lblBrand       = new System.Windows.Forms.Label();
            this.lblTagline     = new System.Windows.Forms.Label();
            this.lblLoginTitle  = new System.Windows.Forms.Label();
            this.pnlLoginCard   = new System.Windows.Forms.Panel();
            this.lblSenhaTitle  = new System.Windows.Forms.Label();
            this.pnlSenhaCard   = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // pnlLeft
            this.pnlLeft.SetBounds(0, 0, 290, 520);
            this.pnlLeft.BackColor = System.Drawing.Color.FromArgb(28, 37, 65);
            this.pnlLeft.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom;
            this.pnlLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormDrag_MouseDown);
            // picLogo
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.picLogo.SetBounds(45, 60, 200, 200);
            this.picLogo.SizeMode     = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.BackColor    = System.Drawing.Color.Transparent;
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            // lblBrand
            this.lblBrand.Text = "RANGOFOOD";
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblBrand.ForeColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.lblBrand.AutoSize = false;
            this.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBrand.SetBounds(0, 266, 290, 46);
            this.lblBrand.BackColor = System.Drawing.Color.Transparent;
            // lblTagline
            this.lblTagline.Text = "Sistema de Gest\u00e3o\nde Pedidos";
            this.lblTagline.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTagline.ForeColor = System.Drawing.Color.FromArgb(160, 175, 210);
            this.lblTagline.AutoSize = false;
            this.lblTagline.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTagline.SetBounds(0, 316, 290, 56);
            this.lblTagline.BackColor = System.Drawing.Color.Transparent;
            this.pnlLeft.Controls.Add(this.picLogo);
            this.pnlLeft.Controls.Add(this.lblBrand);
            this.pnlLeft.Controls.Add(this.lblTagline);
            // pnlRight
            this.pnlRight.SetBounds(290, 0, 490, 520);
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(15, 22, 45);
            this.pnlRight.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.pnlRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormDrag_MouseDown);
            // btnFechar
            this.btnFechar.Text = "\u2715";
            this.btnFechar.SetBounds(448, 8, 30, 24);
            this.btnFechar.BackColor = System.Drawing.Color.Transparent;
            this.btnFechar.ForeColor = System.Drawing.Color.FromArgb(160, 175, 210);
            this.btnFechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFechar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnFechar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFechar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFechar.FlatAppearance.BorderSize = 0;
            this.btnFechar.Click += new System.EventHandler(this.BtnFechar_Click);
            // lblLoginTitle
            this.lblLoginTitle.Text = "\U0001F464  USU\u00c1RIO (LOGIN OU NOME)";
            this.lblLoginTitle.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblLoginTitle.ForeColor = System.Drawing.Color.FromArgb(160, 175, 210);
            this.lblLoginTitle.AutoSize = true;
            this.lblLoginTitle.SetBounds(50, 154, 300, 16);
            this.lblLoginTitle.BackColor = System.Drawing.Color.Transparent;
            // pnlLoginCard
            this.pnlLoginCard.SetBounds(50, 174, 370, 46);
            this.pnlLoginCard.BackColor = System.Drawing.Color.FromArgb(20, 30, 58);
            // txtLogin
            this.txtLogin.SetBounds(50, 10, 310, 26);
            this.txtLogin.BackColor = System.Drawing.Color.FromArgb(20, 30, 58);
            this.txtLogin.ForeColor = System.Drawing.Color.White;
            this.txtLogin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLogin.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtLogin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtLogin_KeyDown);
            this.txtLogin.Leave += new System.EventHandler(this.TxtLogin_Leave);
            this.pnlLoginCard.Controls.Add(this.txtLogin);
            // lblNomeUsuario
            this.lblNomeUsuario.Text = "";
            this.lblNomeUsuario.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblNomeUsuario.ForeColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.lblNomeUsuario.AutoSize = false;
            this.lblNomeUsuario.SetBounds(50, 226, 370, 18);
            this.lblNomeUsuario.BackColor = System.Drawing.Color.Transparent;
            // lblSenhaTitle
            this.lblSenhaTitle.Text = "\U0001F512  SENHA";
            this.lblSenhaTitle.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSenhaTitle.ForeColor = System.Drawing.Color.FromArgb(160, 175, 210);
            this.lblSenhaTitle.AutoSize = true;
            this.lblSenhaTitle.SetBounds(50, 256, 100, 16);
            this.lblSenhaTitle.BackColor = System.Drawing.Color.Transparent;
            // pnlSenhaCard
            this.pnlSenhaCard.SetBounds(50, 276, 370, 46);
            this.pnlSenhaCard.BackColor = System.Drawing.Color.FromArgb(20, 30, 58);
            // txtSenha
            this.txtSenha.SetBounds(50, 10, 310, 26);
            this.txtSenha.BackColor = System.Drawing.Color.FromArgb(20, 30, 58);
            this.txtSenha.ForeColor = System.Drawing.Color.White;
            this.txtSenha.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSenha.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSenha.PasswordChar = '\u2022';
            this.txtSenha.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtSenha_KeyDown);
            this.pnlSenhaCard.Controls.Add(this.txtSenha);
            // lblMensagem
            this.lblMensagem.Text = "";
            this.lblMensagem.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.lblMensagem.AutoSize = false;
            this.lblMensagem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMensagem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMensagem.SetBounds(50, 330, 370, 20);
            this.lblMensagem.BackColor = System.Drawing.Color.Transparent;
            // btnEntrar
            this.btnEntrar.Text = "Entrar";
            this.btnEntrar.SetBounds(50, 362, 370, 44);
            this.btnEntrar.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnEntrar.ForeColor = System.Drawing.Color.White;
            this.btnEntrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntrar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnEntrar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEntrar.FlatAppearance.BorderSize = 0;
            this.btnEntrar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.btnEntrar.Click += new System.EventHandler(this.BtnEntrar_Click);
            this.pnlRight.Controls.Add(this.btnFechar);
            this.pnlRight.Controls.Add(this.lblLoginTitle);
            this.pnlRight.Controls.Add(this.pnlLoginCard);
            this.pnlRight.Controls.Add(this.lblNomeUsuario);
            this.pnlRight.Controls.Add(this.lblSenhaTitle);
            this.pnlRight.Controls.Add(this.pnlSenhaCard);
            this.pnlRight.Controls.Add(this.lblMensagem);
            this.pnlRight.Controls.Add(this.btnEntrar);
            // Form
            this.AcceptButton    = this.btnEntrar;
            this.Text            = "RanGoFood \u2014 Login";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = System.Drawing.Color.FromArgb(15, 22, 45);
            this.ForeColor       = System.Drawing.Color.White;
            this.Font            = new System.Drawing.Font("Segoe UI", 10F);
            this.ClientSize      = new System.Drawing.Size(780, 520);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlRight);
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.TextBox txtLogin;
        internal System.Windows.Forms.TextBox txtSenha;
        internal System.Windows.Forms.Label   lblMensagem;
        internal System.Windows.Forms.Label   lblNomeUsuario;
        internal System.Windows.Forms.Button  btnFechar;
        internal System.Windows.Forms.Button  btnEntrar;
        private  System.Windows.Forms.Panel   pnlLeft;
        private  System.Windows.Forms.Panel   pnlRight;
        private  System.Windows.Forms.PictureBox picLogo;
        private  System.Windows.Forms.Label   lblBrand;
        private  System.Windows.Forms.Label   lblTagline;
        private  System.Windows.Forms.Label   lblLoginTitle;
        private  System.Windows.Forms.Panel   pnlLoginCard;
        private  System.Windows.Forms.Label   lblSenhaTitle;
        private  System.Windows.Forms.Panel   pnlSenhaCard;

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION       = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(System.IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private void FormDrag_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void BtnFechar_Click(object s, System.EventArgs e) { DialogResult = System.Windows.Forms.DialogResult.Cancel; Close(); }
    }
}
