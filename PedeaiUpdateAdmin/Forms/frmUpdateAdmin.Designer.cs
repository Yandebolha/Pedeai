namespace PedeaiUpdateAdmin.Forms
{
    partial class frmUpdateAdmin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl          = new System.Windows.Forms.TabControl();
            this.tabPublicar         = new System.Windows.Forms.TabPage();
            this.tabClientes         = new System.Windows.Forms.TabPage();
            this.tabPacotes          = new System.Windows.Forms.TabPage();
            this.tabConfig           = new System.Windows.Forms.TabPage();

            // Tab Publicar
            this.grpArquivo          = new System.Windows.Forms.GroupBox();
            this.lblArquivo          = new System.Windows.Forms.Label();
            this.txtArquivo          = new System.Windows.Forms.TextBox();
            this.btnBrowseZip        = new System.Windows.Forms.Button();
            this.lblVersao           = new System.Windows.Forms.Label();
            this.txtVersao           = new System.Windows.Forms.TextBox();
            this.lblNivel            = new System.Windows.Forms.Label();
            this.cboNivel            = new System.Windows.Forms.ComboBox();
            this.lblDescricao        = new System.Windows.Forms.Label();
            this.txtDescricao        = new System.Windows.Forms.TextBox();
            this.btnPublicar         = new System.Windows.Forms.Button();
            this.lblPublicarInfo     = new System.Windows.Forms.Label();
            this.pbUpload            = new System.Windows.Forms.ProgressBar();
            this.lblStatusPublicacao = new System.Windows.Forms.Label();

            // Tab Clientes
            this.dgvClientes         = new System.Windows.Forms.DataGridView();
            this.btnRefreshClientes  = new System.Windows.Forms.Button();
            this.btnAlterarNivel     = new System.Windows.Forms.Button();
            this.btnBloquear         = new System.Windows.Forms.Button();
            this.btnDesbloquear      = new System.Windows.Forms.Button();
            this.cboFiltroNivel      = new System.Windows.Forms.ComboBox();
            this.lblFiltroNivel      = new System.Windows.Forms.Label();

            // Tab Pacotes
            this.dgvPacotes          = new System.Windows.Forms.DataGridView();
            this.btnRefreshPacotes   = new System.Windows.Forms.Button();
            this.btnExcluirPacote    = new System.Windows.Forms.Button();

            // Tab Config
            this.lblVpsUrl           = new System.Windows.Forms.Label();
            this.txtVpsUrl           = new System.Windows.Forms.TextBox();
            this.lblAdminToken       = new System.Windows.Forms.Label();
            this.txtAdminToken       = new System.Windows.Forms.TextBox();
            this.btnSalvarConfig     = new System.Windows.Forms.Button();
            this.btnTestarConexao    = new System.Windows.Forms.Button();

            // Status bar
            this.lblStatusBar        = new System.Windows.Forms.Label();

            this.tabControl.SuspendLayout();
            this.SuspendLayout();

            // ── tabControl ──────────────────────────────────────────────────────────
            this.tabControl.SetBounds(8, 8, 950, 560);
            this.tabControl.TabPages.Add(this.tabPublicar);
            this.tabControl.TabPages.Add(this.tabClientes);
            this.tabControl.TabPages.Add(this.tabPacotes);
            this.tabControl.TabPages.Add(this.tabConfig);

            // ── tabPublicar ──────────────────────────────────────────────────────────
            this.tabPublicar.Text = "Publicar Atualização";
            this.tabPublicar.Controls.Add(this.grpArquivo);
            this.tabPublicar.Controls.Add(this.btnPublicar);
            this.tabPublicar.Controls.Add(this.pbUpload);
            this.tabPublicar.Controls.Add(this.lblStatusPublicacao);

            this.grpArquivo.Text = "Pacote de Atualização";
            this.grpArquivo.SetBounds(8, 8, 590, 260);
            this.grpArquivo.Controls.Add(this.lblArquivo);
            this.grpArquivo.Controls.Add(this.txtArquivo);
            this.grpArquivo.Controls.Add(this.btnBrowseZip);
            this.grpArquivo.Controls.Add(this.lblVersao);
            this.grpArquivo.Controls.Add(this.txtVersao);
            this.grpArquivo.Controls.Add(this.lblNivel);
            this.grpArquivo.Controls.Add(this.cboNivel);
            this.grpArquivo.Controls.Add(this.lblDescricao);
            this.grpArquivo.Controls.Add(this.txtDescricao);
            this.grpArquivo.Controls.Add(this.lblPublicarInfo);

            this.lblArquivo.Text    = "Arquivo ZIP:"; this.lblArquivo.SetBounds(8, 24, 100, 20);
            this.txtArquivo.SetBounds(112, 22, 360, 23); this.txtArquivo.ReadOnly = true;
            this.btnBrowseZip.Text  = "...";         this.btnBrowseZip.SetBounds(478, 22, 34, 23);
            this.btnBrowseZip.Click += new System.EventHandler(this.BtnBrowseZip_Click);

            this.lblVersao.Text     = "Versão:";     this.lblVersao.SetBounds(8, 56, 100, 20);
            this.txtVersao.SetBounds(112, 54, 120, 23);
            this.lblNivel.Text      = "Nível:";      this.lblNivel.SetBounds(8, 86, 100, 20);
            this.cboNivel.SetBounds(112, 84, 200, 23);
            this.cboNivel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNivel.Items.Add("1 — Beta (recebe primeiro)");
            this.cboNivel.Items.Add("2 — Standard (estável)");
            this.cboNivel.Items.Add("3 — Legacy (somente críticos)");
            this.cboNivel.SelectedIndex = 1;

            this.lblDescricao.Text  = "Descrição:"; this.lblDescricao.SetBounds(8, 116, 100, 20);
            this.txtDescricao.SetBounds(112, 114, 400, 60);
            this.txtDescricao.Multiline = true; this.txtDescricao.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            this.lblPublicarInfo.Text = "ℹ️  O ZIP deve conter: pasta files/ com executáveis, e opcionalmente update.sql.";
            this.lblPublicarInfo.SetBounds(8, 190, 570, 40);
            this.lblPublicarInfo.ForeColor = System.Drawing.Color.SteelBlue;

            this.btnPublicar.Text      = "🚀  Publicar Atualização";
            this.btnPublicar.SetBounds(8, 280, 200, 36);
            this.btnPublicar.BackColor = System.Drawing.Color.FromArgb(25, 120, 220);
            this.btnPublicar.ForeColor = System.Drawing.Color.White;
            this.btnPublicar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPublicar.Click += new System.EventHandler(this.BtnPublicar_Click);

            this.pbUpload.SetBounds(8, 324, 590, 16); this.pbUpload.Visible = false;
            this.lblStatusPublicacao.Text = ""; this.lblStatusPublicacao.SetBounds(8, 346, 590, 20);

            // ── tabClientes ──────────────────────────────────────────────────────────
            this.tabClientes.Text = "Clientes";
            this.tabClientes.Controls.Add(this.dgvClientes);
            this.tabClientes.Controls.Add(this.btnRefreshClientes);
            this.tabClientes.Controls.Add(this.btnAlterarNivel);
            this.tabClientes.Controls.Add(this.btnBloquear);
            this.tabClientes.Controls.Add(this.btnDesbloquear);
            this.tabClientes.Controls.Add(this.lblFiltroNivel);
            this.tabClientes.Controls.Add(this.cboFiltroNivel);

            this.dgvClientes.SetBounds(8, 8, 910, 440);
            this.dgvClientes.ReadOnly = true;
            this.dgvClientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClientes.MultiSelect = false;
            this.dgvClientes.AllowUserToAddRows = false;
            this.dgvClientes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            this.lblFiltroNivel.Text  = "Filtrar nível:"; this.lblFiltroNivel.SetBounds(8, 460, 90, 22);
            this.cboFiltroNivel.SetBounds(100, 458, 150, 22);
            this.cboFiltroNivel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFiltroNivel.Items.Add("Todos"); this.cboFiltroNivel.Items.Add("1 — Beta");
            this.cboFiltroNivel.Items.Add("2 — Standard"); this.cboFiltroNivel.Items.Add("3 — Legacy");
            this.cboFiltroNivel.SelectedIndex = 0;
            this.cboFiltroNivel.SelectedIndexChanged += new System.EventHandler(this.CboFiltroNivel_Changed);

            this.btnRefreshClientes.Text     = "🔄 Atualizar"; this.btnRefreshClientes.SetBounds(260, 456, 110, 28);
            this.btnRefreshClientes.Click   += new System.EventHandler(this.BtnRefreshClientes_Click);
            this.btnAlterarNivel.Text        = "Alterar Nível";  this.btnAlterarNivel.SetBounds(380, 456, 120, 28);
            this.btnAlterarNivel.Click      += new System.EventHandler(this.BtnAlterarNivel_Click);
            this.btnBloquear.Text            = "Bloquear";        this.btnBloquear.SetBounds(510, 456, 100, 28);
            this.btnBloquear.BackColor       = System.Drawing.Color.FromArgb(180, 50, 50);
            this.btnBloquear.ForeColor       = System.Drawing.Color.White;
            this.btnBloquear.FlatStyle       = System.Windows.Forms.FlatStyle.Flat;
            this.btnBloquear.Click          += new System.EventHandler(this.BtnBloquear_Click);
            this.btnDesbloquear.Text         = "Desbloquear";     this.btnDesbloquear.SetBounds(620, 456, 110, 28);
            this.btnDesbloquear.BackColor    = System.Drawing.Color.FromArgb(40, 140, 40);
            this.btnDesbloquear.ForeColor    = System.Drawing.Color.White;
            this.btnDesbloquear.FlatStyle    = System.Windows.Forms.FlatStyle.Flat;
            this.btnDesbloquear.Click       += new System.EventHandler(this.BtnDesbloquear_Click);

            // ── tabPacotes ───────────────────────────────────────────────────────────
            this.tabPacotes.Text = "Pacotes";
            this.tabPacotes.Controls.Add(this.dgvPacotes);
            this.tabPacotes.Controls.Add(this.btnRefreshPacotes);
            this.tabPacotes.Controls.Add(this.btnExcluirPacote);

            this.dgvPacotes.SetBounds(8, 8, 910, 460);
            this.dgvPacotes.ReadOnly = true;
            this.dgvPacotes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPacotes.MultiSelect = false;
            this.dgvPacotes.AllowUserToAddRows = false;
            this.dgvPacotes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            this.btnRefreshPacotes.Text  = "🔄 Atualizar"; this.btnRefreshPacotes.SetBounds(8, 476, 110, 28);
            this.btnRefreshPacotes.Click += new System.EventHandler(this.BtnRefreshPacotes_Click);
            this.btnExcluirPacote.Text   = "❌ Desativar Pacote";
            this.btnExcluirPacote.SetBounds(128, 476, 160, 28);
            this.btnExcluirPacote.BackColor = System.Drawing.Color.FromArgb(180, 50, 50);
            this.btnExcluirPacote.ForeColor = System.Drawing.Color.White;
            this.btnExcluirPacote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcluirPacote.Click  += new System.EventHandler(this.BtnExcluirPacote_Click);

            // ── tabConfig ────────────────────────────────────────────────────────────
            this.tabConfig.Text = "Configuração";
            this.tabConfig.Controls.Add(this.lblVpsUrl);
            this.tabConfig.Controls.Add(this.txtVpsUrl);
            this.tabConfig.Controls.Add(this.lblAdminToken);
            this.tabConfig.Controls.Add(this.txtAdminToken);
            this.tabConfig.Controls.Add(this.btnSalvarConfig);
            this.tabConfig.Controls.Add(this.btnTestarConexao);

            this.lblVpsUrl.Text    = "URL da VPS:";            this.lblVpsUrl.SetBounds(8, 24, 130, 20);
            this.txtVpsUrl.SetBounds(140, 22, 400, 23);
            this.lblAdminToken.Text= "Admin Token:";           this.lblAdminToken.SetBounds(8, 56, 130, 20);
            this.txtAdminToken.SetBounds(140, 54, 400, 23);   this.txtAdminToken.PasswordChar = '●';
            this.btnSalvarConfig.Text  = "💾 Salvar";          this.btnSalvarConfig.SetBounds(140, 90, 120, 28);
            this.btnSalvarConfig.Click += new System.EventHandler(this.BtnSalvarConfig_Click);
            this.btnTestarConexao.Text  = "🔌 Testar Conexão"; this.btnTestarConexao.SetBounds(270, 90, 130, 28);
            this.btnTestarConexao.Click += new System.EventHandler(this.BtnTestarConexao_Click);

            // ── Status bar ───────────────────────────────────────────────────────────
            this.lblStatusBar.Text = "Pronto.";
            this.lblStatusBar.SetBounds(8, 578, 950, 20);
            this.lblStatusBar.ForeColor = System.Drawing.Color.DimGray;

            // ── Form ─────────────────────────────────────────────────────────────────
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.lblStatusBar);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 605);
            this.Text = "PedeaiUpdateAdmin — Gerenciador de Atualizações";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TabControl             tabControl;
        private System.Windows.Forms.TabPage                tabPublicar;
        private System.Windows.Forms.TabPage                tabClientes;
        private System.Windows.Forms.TabPage                tabPacotes;
        private System.Windows.Forms.TabPage                tabConfig;
        private System.Windows.Forms.GroupBox               grpArquivo;
        private System.Windows.Forms.Label                  lblArquivo;
        private System.Windows.Forms.TextBox                txtArquivo;
        private System.Windows.Forms.Button                 btnBrowseZip;
        private System.Windows.Forms.Label                  lblVersao;
        private System.Windows.Forms.TextBox                txtVersao;
        private System.Windows.Forms.Label                  lblNivel;
        private System.Windows.Forms.ComboBox               cboNivel;
        private System.Windows.Forms.Label                  lblDescricao;
        private System.Windows.Forms.TextBox                txtDescricao;
        private System.Windows.Forms.Button                 btnPublicar;
        private System.Windows.Forms.Label                  lblPublicarInfo;
        private System.Windows.Forms.ProgressBar            pbUpload;
        private System.Windows.Forms.Label                  lblStatusPublicacao;
        private System.Windows.Forms.DataGridView           dgvClientes;
        private System.Windows.Forms.Button                 btnRefreshClientes;
        private System.Windows.Forms.Button                 btnAlterarNivel;
        private System.Windows.Forms.Button                 btnBloquear;
        private System.Windows.Forms.Button                 btnDesbloquear;
        private System.Windows.Forms.ComboBox               cboFiltroNivel;
        private System.Windows.Forms.Label                  lblFiltroNivel;
        private System.Windows.Forms.DataGridView           dgvPacotes;
        private System.Windows.Forms.Button                 btnRefreshPacotes;
        private System.Windows.Forms.Button                 btnExcluirPacote;
        private System.Windows.Forms.Label                  lblVpsUrl;
        private System.Windows.Forms.TextBox                txtVpsUrl;
        private System.Windows.Forms.Label                  lblAdminToken;
        private System.Windows.Forms.TextBox                txtAdminToken;
        private System.Windows.Forms.Button                 btnSalvarConfig;
        private System.Windows.Forms.Button                 btnTestarConexao;
        private System.Windows.Forms.Label                  lblStatusBar;
    }
}
