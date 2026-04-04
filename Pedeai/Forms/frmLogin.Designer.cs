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
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.txtSenha = new System.Windows.Forms.TextBox();
            this.lblMensagem = new System.Windows.Forms.Label();
            this.lblNomeUsuario = new System.Windows.Forms.Label();
            this.btnFechar = new System.Windows.Forms.Button();
            this.btnEntrar = new System.Windows.Forms.Button();
            this.pnlCard = new System.Windows.Forms.Panel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblTagline = new System.Windows.Forms.Label();
            this.lblLoginTitle = new System.Windows.Forms.Label();
            this.pnlLoginCard = new System.Windows.Forms.Panel();
            this.lblSenhaTitle = new System.Windows.Forms.Label();
            this.pnlSenhaCard = new System.Windows.Forms.Panel();
            this.pnlCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.pnlLoginCard.SuspendLayout();
            this.pnlSenhaCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLogin
            // 
            this.txtLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(234)))), ((int)(((byte)(227)))));
            this.txtLogin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLogin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(50)))), ((int)(((byte)(35)))));
            this.txtLogin.Location = new System.Drawing.Point(10, 8);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.PlaceholderText = "LOGIN OU NOME";
            this.txtLogin.Size = new System.Drawing.Size(340, 18);
            this.txtLogin.TabIndex = 0;
            this.txtLogin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtLogin_KeyDown);
            this.txtLogin.Leave += new System.EventHandler(this.TxtLogin_Leave);
            // 
            // txtSenha
            // 
            this.txtSenha.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(234)))), ((int)(((byte)(227)))));
            this.txtSenha.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSenha.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtSenha.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(50)))), ((int)(((byte)(35)))));
            this.txtSenha.Location = new System.Drawing.Point(10, 8);
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.PasswordChar = '•';
            this.txtSenha.Size = new System.Drawing.Size(340, 18);
            this.txtSenha.TabIndex = 0;
            this.txtSenha.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtSenha_KeyDown);
            // 
            // lblMensagem
            // 
            this.lblMensagem.BackColor = System.Drawing.Color.Transparent;
            this.lblMensagem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblMensagem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.lblMensagem.Location = new System.Drawing.Point(60, 398);
            this.lblMensagem.Name = "lblMensagem";
            this.lblMensagem.Size = new System.Drawing.Size(360, 18);
            this.lblMensagem.TabIndex = 8;
            this.lblMensagem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNomeUsuario
            // 
            this.lblNomeUsuario.BackColor = System.Drawing.Color.Transparent;
            this.lblNomeUsuario.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblNomeUsuario.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.lblNomeUsuario.Location = new System.Drawing.Point(60, 312);
            this.lblNomeUsuario.Name = "lblNomeUsuario";
            this.lblNomeUsuario.Size = new System.Drawing.Size(360, 16);
            this.lblNomeUsuario.TabIndex = 5;
            // 
            // btnFechar
            // 
            this.btnFechar.BackColor = System.Drawing.Color.Transparent;
            this.btnFechar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFechar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFechar.FlatAppearance.BorderSize = 0;
            this.btnFechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFechar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnFechar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(120)))), ((int)(((byte)(95)))));
            this.btnFechar.Location = new System.Drawing.Point(442, 8);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(28, 28);
            this.btnFechar.TabIndex = 0;
            this.btnFechar.Text = "✕";
            this.btnFechar.UseVisualStyleBackColor = false;
            this.btnFechar.Click += new System.EventHandler(this.BtnFechar_Click);
            // 
            // btnEntrar
            // 
            this.btnEntrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnEntrar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEntrar.FlatAppearance.BorderSize = 0;
            this.btnEntrar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(90)))), ((int)(((byte)(20)))));
            this.btnEntrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntrar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnEntrar.ForeColor = System.Drawing.Color.White;
            this.btnEntrar.Location = new System.Drawing.Point(60, 422);
            this.btnEntrar.Name = "btnEntrar";
            this.btnEntrar.Size = new System.Drawing.Size(360, 46);
            this.btnEntrar.TabIndex = 9;
            this.btnEntrar.Text = "Entrar";
            this.btnEntrar.UseVisualStyleBackColor = false;
            this.btnEntrar.Click += new System.EventHandler(this.BtnEntrar_Click);
            // 
            // pnlCard
            // 
            this.pnlCard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.Controls.Add(this.btnFechar);
            this.pnlCard.Controls.Add(this.picLogo);
            this.pnlCard.Controls.Add(this.lblTagline);
            this.pnlCard.Controls.Add(this.lblLoginTitle);
            this.pnlCard.Controls.Add(this.pnlLoginCard);
            this.pnlCard.Controls.Add(this.lblNomeUsuario);
            this.pnlCard.Controls.Add(this.lblSenhaTitle);
            this.pnlCard.Controls.Add(this.pnlSenhaCard);
            this.pnlCard.Controls.Add(this.lblMensagem);
            this.pnlCard.Controls.Add(this.btnEntrar);
            this.pnlCard.Location = new System.Drawing.Point(0, 0);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(480, 560);
            this.pnlCard.TabIndex = 0;
            this.pnlCard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormDrag_MouseDown);
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.White;
            this.picLogo.Location = new System.Drawing.Point(142, 8);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(200, 200);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 1;
            this.picLogo.TabStop = false;
            // 
            // lblTagline
            // 
            this.lblTagline.BackColor = System.Drawing.Color.Transparent;
            this.lblTagline.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTagline.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(110)))), ((int)(((byte)(95)))));
            this.lblTagline.Location = new System.Drawing.Point(0, 211);
            this.lblTagline.Name = "lblTagline";
            this.lblTagline.Size = new System.Drawing.Size(480, 24);
            this.lblTagline.TabIndex = 2;
            this.lblTagline.Text = "Sistema de Gestão de Pedidos";
            this.lblTagline.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLoginTitle
            // 
            this.lblLoginTitle.AutoSize = true;
            this.lblLoginTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblLoginTitle.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblLoginTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(70)))), ((int)(((byte)(55)))));
            this.lblLoginTitle.Location = new System.Drawing.Point(60, 248);
            this.lblLoginTitle.Name = "lblLoginTitle";
            this.lblLoginTitle.Size = new System.Drawing.Size(74, 13);
            this.lblLoginTitle.TabIndex = 3;
            this.lblLoginTitle.Text = "👤  USUÁRIO";
            // 
            // pnlLoginCard
            // 
            this.pnlLoginCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(234)))), ((int)(((byte)(227)))));
            this.pnlLoginCard.Controls.Add(this.txtLogin);
            this.pnlLoginCard.Location = new System.Drawing.Point(60, 268);
            this.pnlLoginCard.Name = "pnlLoginCard";
            this.pnlLoginCard.Size = new System.Drawing.Size(360, 40);
            this.pnlLoginCard.TabIndex = 4;
            // 
            // lblSenhaTitle
            // 
            this.lblSenhaTitle.AutoSize = true;
            this.lblSenhaTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSenhaTitle.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSenhaTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(70)))), ((int)(((byte)(55)))));
            this.lblSenhaTitle.Location = new System.Drawing.Point(60, 334);
            this.lblSenhaTitle.Name = "lblSenhaTitle";
            this.lblSenhaTitle.Size = new System.Drawing.Size(63, 13);
            this.lblSenhaTitle.TabIndex = 6;
            this.lblSenhaTitle.Text = "🔒  SENHA";
            // 
            // pnlSenhaCard
            // 
            this.pnlSenhaCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(234)))), ((int)(((byte)(227)))));
            this.pnlSenhaCard.Controls.Add(this.txtSenha);
            this.pnlSenhaCard.Location = new System.Drawing.Point(60, 354);
            this.pnlSenhaCard.Name = "pnlSenhaCard";
            this.pnlSenhaCard.Size = new System.Drawing.Size(360, 40);
            this.pnlSenhaCard.TabIndex = 7;
            // 
            // frmLogin
            // 
            this.AcceptButton = this.btnEntrar;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(218)))), ((int)(((byte)(205)))));
            this.ClientSize = new System.Drawing.Size(480, 560);
            this.Controls.Add(this.pnlCard);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RanGoFood — Login";
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlLoginCard.ResumeLayout(false);
            this.pnlLoginCard.PerformLayout();
            this.pnlSenhaCard.ResumeLayout(false);
            this.pnlSenhaCard.PerformLayout();
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
