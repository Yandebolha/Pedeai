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
            this.lstArquivos         = new System.Windows.Forms.ListBox();
            this.btnAdicionarArquivos = new System.Windows.Forms.Button();
            this.btnLimpar           = new System.Windows.Forms.Button();
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
            this.btnMaxMaquinas      = new System.Windows.Forms.Button();
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
            this.txtServiceRoleKey  = new System.Windows.Forms.TextBox();
            this.lblServiceRoleKey  = new System.Windows.Forms.Label();
            this.btnSalvarConfig     = new System.Windows.Forms.Button();
            this.btnTestarConexao    = new System.Windows.Forms.Button();
            this.lblConfigInfo       = new System.Windows.Forms.Label();

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

            this.grpArquivo.Text = "Arquivos para Atualizar";
            this.grpArquivo.SetBounds(8, 8, 620, 310);
            this.grpArquivo.Controls.Add(this.lblArquivo);
            this.grpArquivo.Controls.Add(this.lstArquivos);
            this.grpArquivo.Controls.Add(this.btnAdicionarArquivos);
            this.grpArquivo.Controls.Add(this.btnLimpar);
            this.grpArquivo.Controls.Add(this.lblVersao);
            this.grpArquivo.Controls.Add(this.txtVersao);
            this.grpArquivo.Controls.Add(this.lblNivel);
            this.grpArquivo.Controls.Add(this.cboNivel);
            this.grpArquivo.Controls.Add(this.lblDescricao);
            this.grpArquivo.Controls.Add(this.txtDescricao);
            this.grpArquivo.Controls.Add(this.lblPublicarInfo);

            this.lblArquivo.Text    = "Arquivos:"; this.lblArquivo.SetBounds(8, 24, 100, 20);

            this.lstArquivos.SetBounds(8, 44, 500, 90);
            this.lstArquivos.SelectionMode = System.Windows.Forms.SelectionMode.None;

            this.btnAdicionarArquivos.Text = "Adicionar...";
            this.btnAdicionarArquivos.SetBounds(514, 44, 96, 26);
            this.btnAdicionarArquivos.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.btnAdicionarArquivos.ForeColor = System.Drawing.Color.White;
            this.btnAdicionarArquivos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdicionarArquivos.FlatAppearance.BorderSize = 0;
            this.btnAdicionarArquivos.Click += new System.EventHandler(this.BtnAdicionarArquivos_Click);

            this.btnLimpar.Text = "Limpar";
            this.btnLimpar.SetBounds(514, 76, 96, 26);
            this.btnLimpar.BackColor = System.Drawing.Color.FromArgb(130, 130, 130);
            this.btnLimpar.ForeColor = System.Drawing.Color.White;
            this.btnLimpar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpar.FlatAppearance.BorderSize = 0;
            this.btnLimpar.Click += new System.EventHandler(this.BtnLimpar_Click);

            this.lblVersao.Text     = "Versão:";     this.lblVersao.SetBounds(8, 144, 100, 20);
            this.txtVersao.SetBounds(112, 142, 120, 23);
            this.lblNivel.Text      = "Nível:";      this.lblNivel.SetBounds(8, 174, 100, 20);
            this.cboNivel.SetBounds(112, 172, 200, 23);
            this.cboNivel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNivel.Items.Add("1 — Beta (recebe primeiro)");
            this.cboNivel.Items.Add("2 — Standard (estável)");
            this.cboNivel.Items.Add("3 — Legacy (somente críticos)");
            this.cboNivel.SelectedIndex = 1;

            this.lblDescricao.Text  = "Descrição:"; this.lblDescricao.SetBounds(8, 204, 100, 20);
            this.txtDescricao.SetBounds(112, 202, 400, 56);
            this.txtDescricao.Multiline = true; this.txtDescricao.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            this.lblPublicarInfo.Text = "ℹ️  Selecione os arquivos modificados. O caminho relativo à pasta em comum será preservado.";
            this.lblPublicarInfo.SetBounds(8, 266, 600, 36);
            this.lblPublicarInfo.ForeColor = System.Drawing.Color.FromArgb(130, 115, 90);

            this.btnPublicar.Text      = "🚀  Publicar Atualização";
            this.btnPublicar.SetBounds(8, 330, 200, 36);
            this.btnPublicar.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.btnPublicar.ForeColor = System.Drawing.Color.White;
            this.btnPublicar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPublicar.FlatAppearance.BorderSize = 0;
            this.btnPublicar.Click += new System.EventHandler(this.BtnPublicar_Click);

            this.pbUpload.SetBounds(8, 374, 620, 16); this.pbUpload.Visible = false;
            this.lblStatusPublicacao.Text = ""; this.lblStatusPublicacao.SetBounds(8, 396, 620, 20);

            // ── tabClientes ──────────────────────────────────────────────────────────
            this.tabClientes.Text = "Clientes";
            this.tabClientes.Controls.Add(this.dgvClientes);
            this.tabClientes.Controls.Add(this.btnRefreshClientes);
            this.tabClientes.Controls.Add(this.btnAlterarNivel);
            this.tabClientes.Controls.Add(this.btnMaxMaquinas);
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
            this.dgvClientes.BackgroundColor = System.Drawing.Color.FromArgb(250, 245, 238);
            this.dgvClientes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvClientes.GridColor = System.Drawing.Color.FromArgb(200, 185, 160);
            this.dgvClientes.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.dgvClientes.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgvClientes.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 230, 202);
            this.dgvClientes.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(80, 60, 30);
            this.dgvClientes.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvClientes.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.dgvClientes.EnableHeadersVisualStyles = false;

            this.lblFiltroNivel.Text  = "Filtrar nível:"; this.lblFiltroNivel.SetBounds(8, 460, 90, 22);
            this.cboFiltroNivel.SetBounds(100, 458, 150, 22);
            this.cboFiltroNivel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFiltroNivel.Items.Add("Todos"); this.cboFiltroNivel.Items.Add("1 — Beta");
            this.cboFiltroNivel.Items.Add("2 — Standard"); this.cboFiltroNivel.Items.Add("3 — Legacy");
            this.cboFiltroNivel.SelectedIndex = 0;
            this.cboFiltroNivel.SelectedIndexChanged += new System.EventHandler(this.CboFiltroNivel_Changed);

            this.btnRefreshClientes.Text     = "🔄 Atualizar"; this.btnRefreshClientes.SetBounds(260, 456, 110, 28);
            this.btnRefreshClientes.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.btnRefreshClientes.ForeColor = System.Drawing.Color.White;
            this.btnRefreshClientes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshClientes.FlatAppearance.BorderSize = 0;
            this.btnRefreshClientes.Click   += new System.EventHandler(this.BtnRefreshClientes_Click);
            this.btnAlterarNivel.Text        = "Alterar Nível";  this.btnAlterarNivel.SetBounds(380, 456, 120, 28);
            this.btnAlterarNivel.BackColor   = System.Drawing.Color.FromArgb(176, 110, 42);
            this.btnAlterarNivel.ForeColor   = System.Drawing.Color.White;
            this.btnAlterarNivel.FlatStyle   = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlterarNivel.FlatAppearance.BorderSize = 0;
            this.btnAlterarNivel.Click      += new System.EventHandler(this.BtnAlterarNivel_Click);
            this.btnMaxMaquinas.Text         = "🖥 Máx. Máq.";  this.btnMaxMaquinas.SetBounds(510, 456, 110, 28);
            this.btnMaxMaquinas.BackColor    = System.Drawing.Color.FromArgb(100, 100, 180);
            this.btnMaxMaquinas.ForeColor    = System.Drawing.Color.White;
            this.btnMaxMaquinas.FlatStyle    = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaxMaquinas.FlatAppearance.BorderSize = 0;
            this.btnMaxMaquinas.Click       += new System.EventHandler(this.BtnMaxMaquinas_Click);
            this.btnBloquear.Text            = "Bloquear";        this.btnBloquear.SetBounds(630, 456, 100, 28);
            this.btnBloquear.BackColor       = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnBloquear.ForeColor       = System.Drawing.Color.White;
            this.btnBloquear.FlatStyle       = System.Windows.Forms.FlatStyle.Flat;
            this.btnBloquear.FlatAppearance.BorderSize = 0;
            this.btnBloquear.Click          += new System.EventHandler(this.BtnBloquear_Click);
            this.btnDesbloquear.Text         = "Desbloquear";     this.btnDesbloquear.SetBounds(740, 456, 110, 28);
            this.btnDesbloquear.BackColor    = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnDesbloquear.ForeColor    = System.Drawing.Color.White;
            this.btnDesbloquear.FlatStyle    = System.Windows.Forms.FlatStyle.Flat;
            this.btnDesbloquear.FlatAppearance.BorderSize = 0;
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
            this.dgvPacotes.BackgroundColor = System.Drawing.Color.FromArgb(250, 245, 238);
            this.dgvPacotes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPacotes.GridColor = System.Drawing.Color.FromArgb(200, 185, 160);
            this.dgvPacotes.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.dgvPacotes.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgvPacotes.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 230, 202);
            this.dgvPacotes.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(80, 60, 30);
            this.dgvPacotes.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvPacotes.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.dgvPacotes.EnableHeadersVisualStyles = false;

            this.btnRefreshPacotes.Text  = "🔄 Atualizar"; this.btnRefreshPacotes.SetBounds(8, 476, 110, 28);
            this.btnRefreshPacotes.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.btnRefreshPacotes.ForeColor = System.Drawing.Color.White;
            this.btnRefreshPacotes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshPacotes.FlatAppearance.BorderSize = 0;
            this.btnRefreshPacotes.Click += new System.EventHandler(this.BtnRefreshPacotes_Click);
            this.btnExcluirPacote.Text   = "❌ Desativar Pacote";
            this.btnExcluirPacote.SetBounds(128, 476, 160, 28);
            this.btnExcluirPacote.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnExcluirPacote.ForeColor = System.Drawing.Color.White;
            this.btnExcluirPacote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcluirPacote.FlatAppearance.BorderSize = 0;
            this.btnExcluirPacote.Click  += new System.EventHandler(this.BtnExcluirPacote_Click);

            // ── tabConfig ────────────────────────────────────────────────────────────
            this.tabConfig.Text = "Configuração";
            this.tabConfig.Controls.Add(this.lblVpsUrl);
            this.tabConfig.Controls.Add(this.txtVpsUrl);
            this.tabConfig.Controls.Add(this.lblAdminToken);
            this.tabConfig.Controls.Add(this.txtAdminToken);
            this.tabConfig.Controls.Add(this.lblServiceRoleKey);
            this.tabConfig.Controls.Add(this.txtServiceRoleKey);
            this.tabConfig.Controls.Add(this.btnSalvarConfig);
            this.tabConfig.Controls.Add(this.btnTestarConexao);
            this.tabConfig.Controls.Add(this.lblConfigInfo);

            this.lblVpsUrl.Text    = "URL do Supabase:";       this.lblVpsUrl.SetBounds(8, 24, 130, 20);
            this.txtVpsUrl.SetBounds(140, 22, 400, 23);
            this.lblAdminToken.Text= "service_role Key:";       this.lblAdminToken.SetBounds(8, 56, 130, 20);
            this.txtAdminToken.SetBounds(140, 54, 400, 23);   this.txtAdminToken.PasswordChar = '●';
            this.btnSalvarConfig.Text  = "💾 Salvar";          this.btnSalvarConfig.SetBounds(140, 90, 120, 28);
            this.btnSalvarConfig.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnSalvarConfig.ForeColor = System.Drawing.Color.White;
            this.btnSalvarConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvarConfig.FlatAppearance.BorderSize = 0;
            this.btnSalvarConfig.Click += new System.EventHandler(this.BtnSalvarConfig_Click);
            this.btnTestarConexao.Text  = "🔌 Testar Conexão"; this.btnTestarConexao.SetBounds(270, 90, 130, 28);
            this.btnTestarConexao.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.btnTestarConexao.ForeColor = System.Drawing.Color.White;
            this.btnTestarConexao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestarConexao.FlatAppearance.BorderSize = 0;
            this.btnTestarConexao.Click += new System.EventHandler(this.BtnTestarConexao_Click);

            this.lblConfigInfo.Text =
                "ℹ️  Supabase Dashboard → Settings → API → \"Project API keys\"\r\n" +
                "    • URL do Supabase: campo \"Project URL\"\r\n" +
                "    • service_role Key: chave \"service_role\" (começa com eyJ...)\r\n" +
                "    ATENÇÃO: NÃO use a chave sb_publishable_* — ela não é um JWT válido para REST.";
            this.lblConfigInfo.SetBounds(8, 130, 700, 80);
            this.lblConfigInfo.ForeColor = System.Drawing.Color.FromArgb(120, 80, 30);

            // ── Status bar ───────────────────────────────────────────────────────────
            this.lblStatusBar.Text = "Pronto.";
            this.lblStatusBar.SetBounds(8, 578, 950, 20);
            this.lblStatusBar.ForeColor = System.Drawing.Color.DimGray;

            // ── Form ─────────────────────────────────────────────────────────────────
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.lblStatusBar);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 245, 240);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
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
        private System.Windows.Forms.ListBox                lstArquivos;
        private System.Windows.Forms.Button                 btnAdicionarArquivos;
        private System.Windows.Forms.Button                 btnLimpar;
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
        private System.Windows.Forms.Button                 btnMaxMaquinas;
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
        private System.Windows.Forms.Label                  lblServiceRoleKey;
        private System.Windows.Forms.TextBox                txtServiceRoleKey;
        private System.Windows.Forms.Label                  lblConfigInfo;
        private System.Windows.Forms.Button                 btnSalvarConfig;
        private System.Windows.Forms.Button                 btnTestarConexao;
        private System.Windows.Forms.Label                  lblStatusBar;
    }
}
