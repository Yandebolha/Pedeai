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
            this.pnlCard        = new System.Windows.Forms.Panel();
            this.picLogo        = new System.Windows.Forms.PictureBox();
            this.lblBrand       = new System.Windows.Forms.Label();
            this.lblTagline     = new System.Windows.Forms.Label();
            this.lblLoginTitle  = new System.Windows.Forms.Label();
            this.pnlLoginCard   = new System.Windows.Forms.Panel();
            this.lblSenhaTitle  = new System.Windows.Forms.Label();
            this.pnlSenhaCard   = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // ── pnlCard (white card centered on form) ─────────────────────
            this.pnlCard.SetBounds(0, 0, 480, 580);
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.Anchor    = System.Windows.Forms.AnchorStyles.None;
            this.pnlCard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormDrag_MouseDown);
            // btnFechar (X at top-right of card)
            this.btnFechar.Text = "\u2715";
            this.btnFechar.SetBounds(446, 8, 26, 26);
            this.btnFechar.BackColor = System.Drawing.Color.Transparent;
            this.btnFechar.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.btnFechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFechar.Font      = new System.Drawing.Font("Segoe UI", 10F);
            this.btnFechar.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnFechar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFechar.FlatAppearance.BorderSize = 0;
            this.btnFechar.Click += new System.EventHandler(this.BtnFechar_Click);
            // picLogo (circular logo area)
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.picLogo.SetBounds(165, 36, 150, 150);
            this.picLogo.SizeMode  = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.BackColor = System.Drawing.Color.Transparent;
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            // lblBrand
            this.lblBrand.Text      = "RanGoFood";
            this.lblBrand.Font      = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblBrand.ForeColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.lblBrand.AutoSize  = false;
            this.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBrand.SetBounds(0, 192, 480, 38);
            this.lblBrand.BackColor = System.Drawing.Color.Transparent;
            // lblTagline
            this.lblTagline.Text      = "Sistema de Gest\u00e3o de Pedidos";
            this.lblTagline.Font      = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTagline.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.lblTagline.AutoSize  = false;
            this.lblTagline.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTagline.SetBounds(0, 230, 480, 24);
            this.lblTagline.BackColor = System.Drawing.Color.Transparent;
            // lblLoginTitle
            this.lblLoginTitle.Text      = "\U0001F464  USU\u00c1RIO";
            this.lblLoginTitle.Font      = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblLoginTitle.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblLoginTitle.AutoSize  = true;
            this.lblLoginTitle.SetBounds(60, 270, 200, 16);
            this.lblLoginTitle.BackColor = System.Drawing.Color.Transparent;
            // pnlLoginCard (text box container, light gray bg)
            this.pnlLoginCard.SetBounds(60, 290, 360, 38);
            this.pnlLoginCard.BackColor    = System.Drawing.Color.FromArgb(240, 240, 240);
            this.pnlLoginCard.BorderStyle  = System.Windows.Forms.BorderStyle.FixedSingle;
            // txtLogin
            this.txtLogin.SetBounds(8, 6, 340, 24);
            this.txtLogin.BackColor    = System.Drawing.Color.FromArgb(240, 240, 240);
            this.txtLogin.ForeColor    = System.Drawing.Color.FromArgb(60, 60, 60);
            this.txtLogin.BorderStyle  = System.Windows.Forms.BorderStyle.None;
            this.txtLogin.Font         = new System.Drawing.Font("Segoe UI", 11F);
            this.txtLogin.KeyDown     += new System.Windows.Forms.KeyEventHandler(this.TxtLogin_KeyDown);
            this.txtLogin.Leave       += new System.EventHandler(this.TxtLogin_Leave);
            this.pnlLoginCard.Controls.Add(this.txtLogin);
            // lblNomeUsuario
            this.lblNomeUsuario.Text      = "";
            this.lblNomeUsuario.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblNomeUsuario.ForeColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.lblNomeUsuario.AutoSize  = false;
            this.lblNomeUsuario.SetBounds(60, 332, 360, 16);
            this.lblNomeUsuario.BackColor = System.Drawing.Color.Transparent;
            // lblSenhaTitle
            this.lblSenhaTitle.Text      = "\U0001F512  SENHA";
            this.lblSenhaTitle.Font      = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSenhaTitle.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblSenhaTitle.AutoSize  = true;
            this.lblSenhaTitle.SetBounds(60, 354, 100, 16);
            this.lblSenhaTitle.BackColor = System.Drawing.Color.Transparent;
            // pnlSenhaCard
            this.pnlSenhaCard.SetBounds(60, 374, 360, 38);
            this.pnlSenhaCard.BackColor   = System.Drawing.Color.FromArgb(240, 240, 240);
            this.pnlSenhaCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // txtSenha
            this.txtSenha.SetBounds(8, 6, 340, 24);
            this.txtSenha.BackColor    = System.Drawing.Color.FromArgb(240, 240, 240);
            this.txtSenha.ForeColor    = System.Drawing.Color.FromArgb(60, 60, 60);
            this.txtSenha.BorderStyle  = System.Windows.Forms.BorderStyle.None;
            this.txtSenha.Font         = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSenha.PasswordChar = '\u2022';
            this.txtSenha.KeyDown     += new System.Windows.Forms.KeyEventHandler(this.TxtSenha_KeyDown);
            this.pnlSenhaCard.Controls.Add(this.txtSenha);
            // lblMensagem
            this.lblMensagem.Text      = "";
            this.lblMensagem.ForeColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.lblMensagem.AutoSize  = false;
            this.lblMensagem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMensagem.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMensagem.SetBounds(60, 416, 360, 18);
            this.lblMensagem.BackColor = System.Drawing.Color.Transparent;
            // btnEntrar
            this.btnEntrar.Text      = "Entrar";
            this.btnEntrar.SetBounds(60, 440, 360, 44);
            this.btnEntrar.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.btnEntrar.ForeColor = System.Drawing.Color.White;
            this.btnEntrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntrar.Font      = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnEntrar.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnEntrar.FlatAppearance.BorderSize = 0;
            this.btnEntrar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(196, 90, 20);
            this.btnEntrar.Click += new System.EventHandler(this.BtnEntrar_Click);
            // assemble pnlCard
            this.pnlCard.Controls.Add(this.btnFechar);
            this.pnlCard.Controls.Add(this.picLogo);
            this.pnlCard.Controls.Add(this.lblBrand);
            this.pnlCard.Controls.Add(this.lblTagline);
            this.pnlCard.Controls.Add(this.lblLoginTitle);
            this.pnlCard.Controls.Add(this.pnlLoginCard);
            this.pnlCard.Controls.Add(this.lblNomeUsuario);
            this.pnlCard.Controls.Add(this.lblSenhaTitle);
            this.pnlCard.Controls.Add(this.pnlSenhaCard);
            this.pnlCard.Controls.Add(this.lblMensagem);
            this.pnlCard.Controls.Add(this.btnEntrar);
            // Form
            this.AcceptButton    = this.btnEntrar;
            this.Text            = "RanGoFood \u2014 Login";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = System.Drawing.Color.FromArgb(235, 228, 212);
            this.ForeColor       = System.Drawing.Color.FromArgb(60, 60, 60);
            this.Font            = new System.Drawing.Font("Segoe UI", 10F);
            this.ClientSize      = new System.Drawing.Size(480, 580);
            this.Controls.Add(this.pnlCard);
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.TextBox   txtLogin;
        internal System.Windows.Forms.TextBox   txtSenha;
        internal System.Windows.Forms.Label     lblMensagem;
        internal System.Windows.Forms.Label     lblNomeUsuario;
        internal System.Windows.Forms.Button    btnFechar;
        internal System.Windows.Forms.Button    btnEntrar;
        private  System.Windows.Forms.Panel     pnlCard;
        private  System.Windows.Forms.PictureBox picLogo;
        private  System.Windows.Forms.Label     lblBrand;
        private  System.Windows.Forms.Label     lblTagline;
        private  System.Windows.Forms.Label     lblLoginTitle;
        private  System.Windows.Forms.Panel     pnlLoginCard;
        private  System.Windows.Forms.Label     lblSenhaTitle;
        private  System.Windows.Forms.Panel     pnlSenhaCard;

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
