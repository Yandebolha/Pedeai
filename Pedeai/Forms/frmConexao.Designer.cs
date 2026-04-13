namespace Pedeai.Forms
{
    partial class frmConexao
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // ── Alocar controles de campo ────────────────────────────────────────
            this.txtServidor    = new System.Windows.Forms.TextBox();
            this.txtPorta       = new System.Windows.Forms.TextBox();
            this.txtBanco       = new System.Windows.Forms.TextBox();
            this.txtUsuario     = new System.Windows.Forms.TextBox();
            this.txtSenha       = new System.Windows.Forms.TextBox();
            this.lstServidores  = new System.Windows.Forms.ListBox();
            this.btnVarrer      = new System.Windows.Forms.Button();
            this.btnTestar      = new System.Windows.Forms.Button();
            this.btnSalvar      = new System.Windows.Forms.Button();
            this.btnCancelar    = new System.Windows.Forms.Button();
            this.lblStatus      = new System.Windows.Forms.Label();
            this.progressVarrer = new System.Windows.Forms.ProgressBar();

            // ── Labels de layout (variáveis locais) ──────────────────────────────
            System.Windows.Forms.Label lblTitulo = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblSrv    = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblPrt    = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblBd     = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblUsr    = new System.Windows.Forms.Label();
            System.Windows.Forms.Label lblPwd    = new System.Windows.Forms.Label();
            System.Windows.Forms.Label sep       = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // ── lblTitulo ────────────────────────────────────────────────────────
            lblTitulo.Text      = "⚙️  Conexão ao Banco de Dados";
            lblTitulo.Font      = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            lblTitulo.ForeColor = System.Drawing.Color.FromArgb(120, 60, 10);
            lblTitulo.Location  = new System.Drawing.Point(0, 14);
            lblTitulo.Size      = new System.Drawing.Size(520, 34);
            lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── Linha: Servidor ──────────────────────────────────────────────────
            lblSrv.Text      = "Servidor (IP/hostname):";
            lblSrv.Location  = new System.Drawing.Point(20, 60);
            lblSrv.AutoSize  = true;
            lblSrv.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblSrv.ForeColor = System.Drawing.Color.FromArgb(60, 50, 30);

            this.txtServidor.Location    = new System.Drawing.Point(160, 58);
            this.txtServidor.Size        = new System.Drawing.Size(200, 24);
            this.txtServidor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // ── Linha: Porta ─────────────────────────────────────────────────────
            lblPrt.Text      = "Porta MySQL:";
            lblPrt.Location  = new System.Drawing.Point(20, 96);
            lblPrt.AutoSize  = true;
            lblPrt.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblPrt.ForeColor = System.Drawing.Color.FromArgb(60, 50, 30);

            this.txtPorta.Location    = new System.Drawing.Point(160, 94);
            this.txtPorta.Size        = new System.Drawing.Size(70, 24);
            this.txtPorta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPorta.Text        = "3306";

            // ── Linha: Banco ─────────────────────────────────────────────────────
            lblBd.Text      = "Nome do Banco:";
            lblBd.Location  = new System.Drawing.Point(20, 132);
            lblBd.AutoSize  = true;
            lblBd.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblBd.ForeColor = System.Drawing.Color.FromArgb(60, 50, 30);

            this.txtBanco.Location    = new System.Drawing.Point(160, 130);
            this.txtBanco.Size        = new System.Drawing.Size(200, 24);
            this.txtBanco.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // ── Linha: Usuário ───────────────────────────────────────────────────
            lblUsr.Text      = "Usuário:";
            lblUsr.Location  = new System.Drawing.Point(20, 168);
            lblUsr.AutoSize  = true;
            lblUsr.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblUsr.ForeColor = System.Drawing.Color.FromArgb(60, 50, 30);

            this.txtUsuario.Location    = new System.Drawing.Point(160, 166);
            this.txtUsuario.Size        = new System.Drawing.Size(200, 24);
            this.txtUsuario.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // ── Linha: Senha ─────────────────────────────────────────────────────
            lblPwd.Text      = "Senha:";
            lblPwd.Location  = new System.Drawing.Point(20, 204);
            lblPwd.AutoSize  = true;
            lblPwd.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblPwd.ForeColor = System.Drawing.Color.FromArgb(60, 50, 30);

            this.txtSenha.Location              = new System.Drawing.Point(160, 202);
            this.txtSenha.Size                  = new System.Drawing.Size(200, 24);
            this.txtSenha.BorderStyle           = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSenha.UseSystemPasswordChar = true;

            // ── Botões ───────────────────────────────────────────────────────────
            this.btnTestar.Text                        = "🔍 Testar Conexão";
            this.btnTestar.Location                    = new System.Drawing.Point(20, 248);
            this.btnTestar.Size                        = new System.Drawing.Size(155, 30);
            this.btnTestar.BackColor                   = System.Drawing.Color.FromArgb(52, 100, 160);
            this.btnTestar.ForeColor                   = System.Drawing.Color.White;
            this.btnTestar.FlatStyle                   = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestar.Font                        = new System.Drawing.Font("Segoe UI", 9F);
            this.btnTestar.Cursor                      = System.Windows.Forms.Cursors.Hand;
            this.btnTestar.FlatAppearance.BorderSize   = 0;
            this.btnTestar.Click                      += new System.EventHandler(this.BtnTestar_Click);

            this.btnSalvar.Text                        = "💾 Salvar";
            this.btnSalvar.Location                    = new System.Drawing.Point(185, 248);
            this.btnSalvar.Size                        = new System.Drawing.Size(100, 30);
            this.btnSalvar.BackColor                   = System.Drawing.Color.FromArgb(87, 140, 38);
            this.btnSalvar.ForeColor                   = System.Drawing.Color.White;
            this.btnSalvar.FlatStyle                   = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvar.Font                        = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSalvar.Cursor                      = System.Windows.Forms.Cursors.Hand;
            this.btnSalvar.FlatAppearance.BorderSize   = 0;
            this.btnSalvar.Click                      += new System.EventHandler(this.BtnSalvar_Click);

            this.btnCancelar.Text                      = "Cancelar";
            this.btnCancelar.Location                  = new System.Drawing.Point(295, 248);
            this.btnCancelar.Size                      = new System.Drawing.Size(90, 30);
            this.btnCancelar.BackColor                 = System.Drawing.Color.FromArgb(150, 60, 40);
            this.btnCancelar.ForeColor                 = System.Drawing.Color.White;
            this.btnCancelar.FlatStyle                 = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font                      = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancelar.Cursor                    = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.Click                    += new System.EventHandler(this.BtnCancelar_Click);

            // ── lblStatus ────────────────────────────────────────────────────────
            this.lblStatus.Location  = new System.Drawing.Point(20, 296);
            this.lblStatus.Size      = new System.Drawing.Size(480, 22);
            this.lblStatus.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(80, 60, 30);

            // ── Separador ────────────────────────────────────────────────────────
            sep.Text      = "Servidores MySQL encontrados na rede:";
            sep.Location  = new System.Drawing.Point(20, 326);
            sep.AutoSize  = true;
            sep.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            sep.ForeColor = System.Drawing.Color.FromArgb(60, 50, 30);

            // ── progressVarrer ───────────────────────────────────────────────────
            this.progressVarrer.Location = new System.Drawing.Point(20, 348);
            this.progressVarrer.Size     = new System.Drawing.Size(380, 14);
            this.progressVarrer.Style    = System.Windows.Forms.ProgressBarStyle.Blocks;
            this.progressVarrer.Visible  = true;

            // ── lstServidores ────────────────────────────────────────────────────
            this.lstServidores.Location    = new System.Drawing.Point(20, 368);
            this.lstServidores.Size        = new System.Drawing.Size(380, 90);
            this.lstServidores.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstServidores.Font        = new System.Drawing.Font("Consolas", 9.5F);
            this.lstServidores.BackColor   = System.Drawing.Color.FromArgb(255, 252, 245);
            this.lstServidores.Items.Add("localhost");
            this.lstServidores.DoubleClick += new System.EventHandler(this.LstServidores_DoubleClick);

            // ── btnVarrer ────────────────────────────────────────────────────────
            this.btnVarrer.Text                      = "🔎 Varrer Rede";
            this.btnVarrer.Location                  = new System.Drawing.Point(410, 368);
            this.btnVarrer.Size                      = new System.Drawing.Size(110, 30);
            this.btnVarrer.BackColor                 = System.Drawing.Color.FromArgb(180, 110, 20);
            this.btnVarrer.ForeColor                 = System.Drawing.Color.White;
            this.btnVarrer.FlatStyle                 = System.Windows.Forms.FlatStyle.Flat;
            this.btnVarrer.Font                      = new System.Drawing.Font("Segoe UI", 9F);
            this.btnVarrer.Cursor                    = System.Windows.Forms.Cursors.Hand;
            this.btnVarrer.FlatAppearance.BorderSize = 0;
            this.btnVarrer.Click                    += new System.EventHandler(this.BtnVarrer_Click);

            // ── Adicionar ao Form ────────────────────────────────────────────────
            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblSrv);
            this.Controls.Add(this.txtServidor);
            this.Controls.Add(lblPrt);
            this.Controls.Add(this.txtPorta);
            this.Controls.Add(lblBd);
            this.Controls.Add(this.txtBanco);
            this.Controls.Add(lblUsr);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(lblPwd);
            this.Controls.Add(this.txtSenha);
            this.Controls.Add(this.btnTestar);
            this.Controls.Add(this.btnSalvar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(sep);
            this.Controls.Add(this.progressVarrer);
            this.Controls.Add(this.lstServidores);
            this.Controls.Add(this.btnVarrer);

            // ── Form ─────────────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(250, 242, 225);
            this.ClientSize          = new System.Drawing.Size(520, 476);
            this.Font                = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox         = false;
            this.MinimizeBox         = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text                = "Configuração de Conexão ao Banco de Dados";

            this.ResumeLayout(false);
        }

        // ── Campos ───────────────────────────────────────────────────────────────
        private System.Windows.Forms.TextBox      txtServidor;
        private System.Windows.Forms.TextBox      txtPorta;
        private System.Windows.Forms.TextBox      txtBanco;
        private System.Windows.Forms.TextBox      txtUsuario;
        private System.Windows.Forms.TextBox      txtSenha;
        private System.Windows.Forms.ListBox      lstServidores;
        private System.Windows.Forms.Button       btnVarrer;
        private System.Windows.Forms.Button       btnTestar;
        private System.Windows.Forms.Button       btnSalvar;
        private System.Windows.Forms.Button       btnCancelar;
        private System.Windows.Forms.Label        lblStatus;
        private System.Windows.Forms.ProgressBar  progressVarrer;
    }
}
