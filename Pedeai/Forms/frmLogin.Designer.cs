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
            this.txtLogin      = new System.Windows.Forms.TextBox();
            this.txtSenha      = new System.Windows.Forms.TextBox();
            this.lblMensagem   = new System.Windows.Forms.Label();
            this.lblNomeUsuario= new System.Windows.Forms.Label();
            this.btnFechar     = new System.Windows.Forms.Button();
            this.btnEntrar     = new System.Windows.Forms.Button();
            this.pnlCard       = new System.Windows.Forms.Panel();
            this.lblBrand      = new System.Windows.Forms.Label();
            this.lblLoginTitle = new System.Windows.Forms.Label();
            this.pnlLoginCard  = new System.Windows.Forms.Panel();
            this.lblSenhaTitle = new System.Windows.Forms.Label();
            this.pnlSenhaCard  = new System.Windows.Forms.Panel();
            this.pnlCard.SuspendLayout();
            this.pnlLoginCard.SuspendLayout();
            this.pnlSenhaCard.SuspendLayout();
            this.SuspendLayout();

            // ── txtLogin ──────────────────────────────────────────────────────
            this.txtLogin.BackColor  = System.Drawing.Color.FromArgb(240, 233, 220);
            this.txtLogin.BorderStyle= System.Windows.Forms.BorderStyle.None;
            this.txtLogin.Font       = new System.Drawing.Font("Segoe UI", 11F);
            this.txtLogin.ForeColor  = System.Drawing.Color.FromArgb(60, 60, 60);
            this.txtLogin.Location   = new System.Drawing.Point(12, 12);
            this.txtLogin.Size       = new System.Drawing.Size(370, 20);
            this.txtLogin.TabIndex   = 0;
            this.txtLogin.KeyDown   += new System.Windows.Forms.KeyEventHandler(this.TxtLogin_KeyDown);
            this.txtLogin.Leave     += new System.EventHandler(this.TxtLogin_Leave);

            // ── txtSenha ──────────────────────────────────────────────────────
            this.txtSenha.BackColor   = System.Drawing.Color.FromArgb(240, 233, 220);
            this.txtSenha.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSenha.Font        = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSenha.ForeColor   = System.Drawing.Color.FromArgb(60, 60, 60);
            this.txtSenha.Location    = new System.Drawing.Point(12, 12);
            this.txtSenha.PasswordChar= '•';
            this.txtSenha.Size        = new System.Drawing.Size(370, 20);
            this.txtSenha.TabIndex    = 0;
            this.txtSenha.KeyDown    += new System.Windows.Forms.KeyEventHandler(this.TxtSenha_KeyDown);

            // ── pnlLoginCard ──────────────────────────────────────────────────
            this.pnlLoginCard.BackColor = System.Drawing.Color.FromArgb(240, 233, 220);
            this.pnlLoginCard.Controls.Add(this.txtLogin);
            this.pnlLoginCard.Location  = new System.Drawing.Point(40, 116);
            this.pnlLoginCard.Size      = new System.Drawing.Size(400, 46);
            this.pnlLoginCard.TabIndex  = 2;

            // ── pnlSenhaCard ──────────────────────────────────────────────────
            this.pnlSenhaCard.BackColor = System.Drawing.Color.FromArgb(240, 233, 220);
            this.pnlSenhaCard.Controls.Add(this.txtSenha);
            this.pnlSenhaCard.Location  = new System.Drawing.Point(40, 208);
            this.pnlSenhaCard.Size      = new System.Drawing.Size(400, 46);
            this.pnlSenhaCard.TabIndex  = 4;

            // ── lblBrand ──────────────────────────────────────────────────────
            this.lblBrand.BackColor  = System.Drawing.Color.Transparent;
            this.lblBrand.Font       = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblBrand.ForeColor  = System.Drawing.Color.FromArgb(176, 110, 42);
            this.lblBrand.Location   = new System.Drawing.Point(0, 30);
            this.lblBrand.Size       = new System.Drawing.Size(480, 48);
            this.lblBrand.TabIndex   = 0;
            this.lblBrand.Text       = "RanGoFood";
            this.lblBrand.TextAlign  = System.Drawing.ContentAlignment.MiddleCenter;

            // ── lblLoginTitle ─────────────────────────────────────────────────
            this.lblLoginTitle.AutoSize  = true;
            this.lblLoginTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblLoginTitle.Font      = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblLoginTitle.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblLoginTitle.Location  = new System.Drawing.Point(40, 96);
            this.lblLoginTitle.TabIndex  = 1;
            this.lblLoginTitle.Text      = "👤  USUÁRIO";

            // ── lblSenhaTitle ─────────────────────────────────────────────────
            this.lblSenhaTitle.AutoSize  = true;
            this.lblSenhaTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSenhaTitle.Font      = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblSenhaTitle.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblSenhaTitle.Location  = new System.Drawing.Point(40, 188);
            this.lblSenhaTitle.TabIndex  = 3;
            this.lblSenhaTitle.Text      = "🔒  SENHA";

            // ── lblNomeUsuario ────────────────────────────────────────────────
            this.lblNomeUsuario.BackColor = System.Drawing.Color.Transparent;
            this.lblNomeUsuario.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblNomeUsuario.ForeColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.lblNomeUsuario.Location  = new System.Drawing.Point(40, 166);
            this.lblNomeUsuario.Size      = new System.Drawing.Size(400, 20);
            this.lblNomeUsuario.TabIndex  = 5;

            // ── lblMensagem ───────────────────────────────────────────────────
            this.lblMensagem.BackColor = System.Drawing.Color.Transparent;
            this.lblMensagem.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMensagem.ForeColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.lblMensagem.Location  = new System.Drawing.Point(40, 262);
            this.lblMensagem.Size      = new System.Drawing.Size(400, 22);
            this.lblMensagem.TabIndex  = 6;
            this.lblMensagem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── btnEntrar ─────────────────────────────────────────────────────
            this.btnEntrar.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.btnEntrar.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnEntrar.FlatAppearance.BorderSize          = 0;
            this.btnEntrar.FlatAppearance.MouseOverBackColor  = System.Drawing.Color.FromArgb(196, 90, 20);
            this.btnEntrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntrar.Font      = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnEntrar.ForeColor = System.Drawing.Color.White;
            this.btnEntrar.Location  = new System.Drawing.Point(40, 292);
            this.btnEntrar.Size      = new System.Drawing.Size(400, 52);
            this.btnEntrar.TabIndex  = 7;
            this.btnEntrar.Text      = "Entrar";
            this.btnEntrar.UseVisualStyleBackColor = false;
            this.btnEntrar.Click    += new System.EventHandler(this.BtnEntrar_Click);

            // ── btnFechar ─────────────────────────────────────────────────────
            this.btnFechar.BackColor   = System.Drawing.Color.Transparent;
            this.btnFechar.Cursor      = System.Windows.Forms.Cursors.Hand;
            this.btnFechar.DialogResult= System.Windows.Forms.DialogResult.Cancel;
            this.btnFechar.FlatAppearance.BorderSize = 0;
            this.btnFechar.FlatStyle   = System.Windows.Forms.FlatStyle.Flat;
            this.btnFechar.Font        = new System.Drawing.Font("Segoe UI", 10F);
            this.btnFechar.ForeColor   = System.Drawing.Color.FromArgb(120, 120, 120);
            this.btnFechar.Location    = new System.Drawing.Point(444, 8);
            this.btnFechar.Size        = new System.Drawing.Size(30, 30);
            this.btnFechar.TabIndex    = 8;
            this.btnFechar.Text        = "✕";
            this.btnFechar.UseVisualStyleBackColor = false;
            this.btnFechar.Click      += new System.EventHandler(this.BtnFechar_Click);

            // ── pnlCard ───────────────────────────────────────────────────────
            this.pnlCard.Anchor    = System.Windows.Forms.AnchorStyles.None;
            this.pnlCard.BackColor = System.Drawing.Color.FromArgb(200, 185, 160);
            this.pnlCard.Controls.Add(this.btnFechar);
            this.pnlCard.Controls.Add(this.lblBrand);
            this.pnlCard.Controls.Add(this.lblLoginTitle);
            this.pnlCard.Controls.Add(this.pnlLoginCard);
            this.pnlCard.Controls.Add(this.lblNomeUsuario);
            this.pnlCard.Controls.Add(this.lblSenhaTitle);
            this.pnlCard.Controls.Add(this.pnlSenhaCard);
            this.pnlCard.Controls.Add(this.lblMensagem);
            this.pnlCard.Controls.Add(this.btnEntrar);
            this.pnlCard.Location = new System.Drawing.Point(0, 0);
            this.pnlCard.Size     = new System.Drawing.Size(480, 368);
            this.pnlCard.TabIndex = 0;
            this.pnlCard.Paint   += new System.Windows.Forms.PaintEventHandler(this.pnlCard_Paint);
            this.pnlCard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormDrag_MouseDown);

            // ── frmLogin ──────────────────────────────────────────────────────
            this.AcceptButton      = this.btnEntrar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode     = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor         = System.Drawing.Color.FromArgb(210, 195, 170);
            this.ClientSize        = new System.Drawing.Size(480, 368);
            this.Controls.Add(this.pnlCard);
            this.Font              = new System.Drawing.Font("Segoe UI", 10F);
            this.ForeColor         = System.Drawing.Color.FromArgb(60, 60, 60);
            this.FormBorderStyle   = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox       = false;
            this.MinimizeBox       = false;
            this.Name              = "frmLogin";
            this.StartPosition     = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text              = "RanGoFood — Login";
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.pnlLoginCard.ResumeLayout(false);
            this.pnlLoginCard.PerformLayout();
            this.pnlSenhaCard.ResumeLayout(false);
            this.pnlSenhaCard.PerformLayout();
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.TextBox txtLogin;
        internal System.Windows.Forms.TextBox txtSenha;
        internal System.Windows.Forms.Label   lblMensagem;
        internal System.Windows.Forms.Label   lblNomeUsuario;
        internal System.Windows.Forms.Button  btnFechar;
        internal System.Windows.Forms.Button  btnEntrar;
        private  System.Windows.Forms.Panel   pnlCard;
        private  System.Windows.Forms.Label   lblBrand;
        private  System.Windows.Forms.Label   lblLoginTitle;
        private  System.Windows.Forms.Panel   pnlLoginCard;
        private  System.Windows.Forms.Label   lblSenhaTitle;
        private  System.Windows.Forms.Panel   pnlSenhaCard;
    }
}
