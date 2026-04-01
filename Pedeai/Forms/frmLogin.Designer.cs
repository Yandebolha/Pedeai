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
            this.lblIconLogin   = new System.Windows.Forms.Label();
            this.lblSenhaTitle  = new System.Windows.Forms.Label();
            this.pnlSenhaCard   = new System.Windows.Forms.Panel();
            this.lblIconSenha   = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // pnlLeft — espresso escuro, identidade food
            this.pnlLeft.SetBounds(0, 0, 290, 520);
            this.pnlLeft.BackColor = System.Drawing.Color.FromArgb(45, 22, 10);
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
            this.lblBrand.ForeColor = System.Drawing.Color.FromArgb(220, 95, 25);
            this.lblBrand.AutoSize = false;
            this.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBrand.SetBounds(0, 266, 290, 46);
            this.lblBrand.BackColor = System.Drawing.Color.Transparent;
            // lblTagline
            this.lblTagline.Text = "Sistema de Gest\u00e3o\nde Pedidos";
            this.lblTagline.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTagline.ForeColor = System.Drawing.Color.FromArgb(195, 158, 120);
            this.lblTagline.AutoSize = false;
            this.lblTagline.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTagline.SetBounds(0, 316, 290, 56);
            this.lblTagline.BackColor = System.Drawing.Color.Transparent;
            this.pnlLeft.Controls.Add(this.picLogo);
            this.pnlLeft.Controls.Add(this.lblBrand);
            this.pnlLeft.Controls.Add(this.lblTagline);
            // pnlRight — creme quente, fundo limpo
            this.pnlRight.SetBounds(290, 0, 490, 520);
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(252, 248, 244);
            this.pnlRight.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.pnlRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormDrag_MouseDown);
            // btnFechar
            this.btnFechar.Text = "\u2715";
            this.btnFechar.SetBounds(448, 8, 30, 24);
            this.btnFechar.BackColor = System.Drawing.Color.Transparent;
            this.btnFechar.ForeColor = System.Drawing.Color.FromArgb(135, 100, 78);
            this.btnFechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFechar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnFechar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFechar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFechar.FlatAppearance.BorderSize = 0;
            this.btnFechar.Click += new System.EventHandler(this.BtnFechar_Click);
            // lblLoginTitle — sem \u00edcone, apenas texto do r\u00f3tulo
            this.lblLoginTitle.Text = "USU\u00c1RIO";
            this.lblLoginTitle.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblLoginTitle.ForeColor = System.Drawing.Color.FromArgb(125, 88, 65);
            this.lblLoginTitle.AutoSize = true;
            this.lblLoginTitle.SetBounds(50, 154, 300, 16);
            this.lblLoginTitle.BackColor = System.Drawing.Color.Transparent;
            // pnlLoginCard — areia clara
            this.pnlLoginCard.SetBounds(50, 174, 370, 46);
            this.pnlLoginCard.BackColor = System.Drawing.Color.FromArgb(240, 233, 224);
            this.pnlLoginCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // lblIconLogin — \u00edcone de usu\u00e1rio dentro do card
            this.lblIconLogin.Text = "\U0001F464";
            this.lblIconLogin.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.lblIconLogin.ForeColor = System.Drawing.Color.FromArgb(200, 100, 38);
            this.lblIconLogin.AutoSize = false;
            this.lblIconLogin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblIconLogin.SetBounds(6, 7, 30, 28);
            this.lblIconLogin.BackColor = System.Drawing.Color.Transparent;
            // txtLogin — deslocado para dar espa\u00e7o ao \u00edcone
            this.txtLogin.SetBounds(40, 9, 322, 26);
            this.txtLogin.BackColor = System.Drawing.Color.FromArgb(240, 233, 224);
            this.txtLogin.ForeColor = System.Drawing.Color.FromArgb(42, 20, 8);
            this.txtLogin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLogin.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtLogin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtLogin_KeyDown);
            this.txtLogin.Leave += new System.EventHandler(this.TxtLogin_Leave);
            this.pnlLoginCard.Controls.Add(this.lblIconLogin);
            this.pnlLoginCard.Controls.Add(this.txtLogin);
            // lblNomeUsuario
            this.lblNomeUsuario.Text = "";
            this.lblNomeUsuario.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblNomeUsuario.ForeColor = System.Drawing.Color.FromArgb(145, 95, 32);
            this.lblNomeUsuario.AutoSize = false;
            this.lblNomeUsuario.SetBounds(50, 226, 370, 18);
            this.lblNomeUsuario.BackColor = System.Drawing.Color.Transparent;
            // lblSenhaTitle — sem \u00edcone, apenas texto do r\u00f3tulo
            this.lblSenhaTitle.Text = "SENHA";
            this.lblSenhaTitle.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSenhaTitle.ForeColor = System.Drawing.Color.FromArgb(125, 88, 65);
            this.lblSenhaTitle.AutoSize = true;
            this.lblSenhaTitle.SetBounds(50, 256, 100, 16);
            this.lblSenhaTitle.BackColor = System.Drawing.Color.Transparent;
            // pnlSenhaCard — areia clara
            this.pnlSenhaCard.SetBounds(50, 276, 370, 46);
            this.pnlSenhaCard.BackColor = System.Drawing.Color.FromArgb(240, 233, 224);
            this.pnlSenhaCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // lblIconSenha — \u00edcone de cadeado dentro do card
            this.lblIconSenha.Text = "\U0001F512";
            this.lblIconSenha.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.lblIconSenha.ForeColor = System.Drawing.Color.FromArgb(200, 100, 38);
            this.lblIconSenha.AutoSize = false;
            this.lblIconSenha.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblIconSenha.SetBounds(6, 7, 30, 28);
            this.lblIconSenha.BackColor = System.Drawing.Color.Transparent;
            // txtSenha — deslocado para dar espa\u00e7o ao \u00edcone
            this.txtSenha.SetBounds(40, 9, 322, 26);
            this.txtSenha.BackColor = System.Drawing.Color.FromArgb(240, 233, 224);
            this.txtSenha.ForeColor = System.Drawing.Color.FromArgb(42, 20, 8);
            this.txtSenha.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSenha.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSenha.PasswordChar = '\u2022';
            this.txtSenha.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtSenha_KeyDown);
            this.pnlSenhaCard.Controls.Add(this.lblIconSenha);
            this.pnlSenhaCard.Controls.Add(this.txtSenha);
            // lblMensagem
            this.lblMensagem.Text = "";
            this.lblMensagem.ForeColor = System.Drawing.Color.FromArgb(178, 38, 20);
            this.lblMensagem.AutoSize = false;
            this.lblMensagem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMensagem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMensagem.SetBounds(50, 330, 370, 20);
            this.lblMensagem.BackColor = System.Drawing.Color.Transparent;
            // btnEntrar — laranja terracota, apetite
            this.btnEntrar.Text = "Entrar";
            this.btnEntrar.SetBounds(50, 362, 370, 44);
            this.btnEntrar.BackColor = System.Drawing.Color.FromArgb(200, 70, 20);
            this.btnEntrar.ForeColor = System.Drawing.Color.White;
            this.btnEntrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntrar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnEntrar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEntrar.FlatAppearance.BorderSize = 0;
            this.btnEntrar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(168, 55, 14);
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
            this.BackColor       = System.Drawing.Color.FromArgb(252, 248, 244);
            this.ForeColor       = System.Drawing.Color.FromArgb(42, 20, 8);
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
        private  System.Windows.Forms.Label   lblIconLogin;
        private  System.Windows.Forms.Label   lblSenhaTitle;
        private  System.Windows.Forms.Panel   pnlSenhaCard;
        private  System.Windows.Forms.Label   lblIconSenha;

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
