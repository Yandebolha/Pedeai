namespace PedeaiBackup.Forms
{
    partial class frmBackupConfig
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ── Controles ────────────────────────────────────────────────────────────
            this.tabControl            = new System.Windows.Forms.TabControl();
            this.tabConexao            = new System.Windows.Forms.TabPage();
            this.tabAgendamento        = new System.Windows.Forms.TabPage();
            this.tabLog                = new System.Windows.Forms.TabPage();

            // Tab "Conexão"
            this.grpDb                 = new System.Windows.Forms.GroupBox();
            this.lblHost               = new System.Windows.Forms.Label();
            this.txtHost               = new System.Windows.Forms.TextBox();
            this.lblPorta              = new System.Windows.Forms.Label();
            this.txtPorta              = new System.Windows.Forms.TextBox();
            this.lblBanco              = new System.Windows.Forms.Label();
            this.txtBanco              = new System.Windows.Forms.TextBox();
            this.lblUsuario            = new System.Windows.Forms.Label();
            this.txtUsuario            = new System.Windows.Forms.TextBox();
            this.lblSenha              = new System.Windows.Forms.Label();
            this.txtSenha              = new System.Windows.Forms.TextBox();
            this.lblMysqldump          = new System.Windows.Forms.Label();
            this.txtMysqldump          = new System.Windows.Forms.TextBox();
            this.btnBrowseDump         = new System.Windows.Forms.Button();

            this.grpMega               = new System.Windows.Forms.GroupBox();
            this.lblMegaEmail          = new System.Windows.Forms.Label();
            this.txtMegaEmail          = new System.Windows.Forms.TextBox();
            this.lblMegaSenha          = new System.Windows.Forms.Label();
            this.txtMegaSenha          = new System.Windows.Forms.TextBox();
            this.lblMegaPasta          = new System.Windows.Forms.Label();
            this.txtMegaPasta          = new System.Windows.Forms.TextBox();
            this.lblMegaCmd            = new System.Windows.Forms.Label();
            this.txtMegaCmd            = new System.Windows.Forms.TextBox();
            this.btnBrowseMegaCmd      = new System.Windows.Forms.Button();
            this.btnTestarMega         = new System.Windows.Forms.Button();
            this.btnSalvarConexao      = new System.Windows.Forms.Button();

            // Tab "Agendamento"
            this.grpDias               = new System.Windows.Forms.GroupBox();
            this.chkDom                = new System.Windows.Forms.CheckBox();
            this.chkSeg                = new System.Windows.Forms.CheckBox();
            this.chkTer                = new System.Windows.Forms.CheckBox();
            this.chkQua                = new System.Windows.Forms.CheckBox();
            this.chkQui                = new System.Windows.Forms.CheckBox();
            this.chkSex                = new System.Windows.Forms.CheckBox();
            this.chkSab                = new System.Windows.Forms.CheckBox();
            this.grpHorarios           = new System.Windows.Forms.GroupBox();
            this.lblHorarioHint        = new System.Windows.Forms.Label();
            this.txtNovoHorario        = new System.Windows.Forms.TextBox();
            this.btnAddHorario         = new System.Windows.Forms.Button();
            this.btnRemoverHorario     = new System.Windows.Forms.Button();
            this.lstHorarios           = new System.Windows.Forms.ListBox();
            this.btnSalvarAgendamento  = new System.Windows.Forms.Button();

            // Tab "Log"
            this.rtxLog                = new System.Windows.Forms.RichTextBox();
            this.btnLimparLog          = new System.Windows.Forms.Button();

            // Bottom buttons
            this.btnExecutarAgora      = new System.Windows.Forms.Button();
            this.btnExecutarCompleto   = new System.Windows.Forms.Button();
            this.lblStatus             = new System.Windows.Forms.Label();
            this.lblUltimoBackup       = new System.Windows.Forms.Label();
            this.lblProximoBackup      = new System.Windows.Forms.Label();

            // Systray
            this.notifyIcon            = new System.Windows.Forms.NotifyIcon(this.components);
            this.ctxTray               = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAbrir              = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExecutar           = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeparador          = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFechar             = new System.Windows.Forms.ToolStripMenuItem();

            this.tabControl.SuspendLayout();
            this.SuspendLayout();

            // ── tabControl ──────────────────────────────────────────────────────────
            this.tabControl.SetBounds(8, 8, 714, 460);
            this.tabControl.TabPages.Add(this.tabConexao);
            this.tabControl.TabPages.Add(this.tabAgendamento);
            this.tabControl.TabPages.Add(this.tabLog);

            // ── tabConexao ──────────────────────────────────────────────────────────
            this.tabConexao.Text = "Banco && MEGA";
            this.tabConexao.Controls.Add(this.grpDb);
            this.tabConexao.Controls.Add(this.grpMega);
            this.tabConexao.Controls.Add(this.btnSalvarConexao);

            // grpDb
            this.grpDb.Text = "Banco de Dados MySQL";
            this.grpDb.SetBounds(8, 8, 340, 220);
            this.grpDb.Controls.Add(this.lblHost);
            this.grpDb.Controls.Add(this.txtHost);
            this.grpDb.Controls.Add(this.lblPorta);
            this.grpDb.Controls.Add(this.txtPorta);
            this.grpDb.Controls.Add(this.lblBanco);
            this.grpDb.Controls.Add(this.txtBanco);
            this.grpDb.Controls.Add(this.lblUsuario);
            this.grpDb.Controls.Add(this.txtUsuario);
            this.grpDb.Controls.Add(this.lblSenha);
            this.grpDb.Controls.Add(this.txtSenha);
            this.grpDb.Controls.Add(this.lblMysqldump);
            this.grpDb.Controls.Add(this.txtMysqldump);
            this.grpDb.Controls.Add(this.btnBrowseDump);

            this.lblHost.Text = "Host:";        this.lblHost.SetBounds(8, 24, 100, 20);
            this.txtHost.SetBounds(112, 22, 160, 23);
            this.lblPorta.Text = "Porta:";      this.lblPorta.SetBounds(8, 52, 100, 20);
            this.txtPorta.SetBounds(112, 50, 80, 23);
            this.lblBanco.Text = "Banco:";      this.lblBanco.SetBounds(8, 80, 100, 20);
            this.txtBanco.SetBounds(112, 78, 160, 23);
            this.lblUsuario.Text = "Usuário:";  this.lblUsuario.SetBounds(8, 108, 100, 20);
            this.txtUsuario.SetBounds(112, 106, 160, 23);
            this.lblSenha.Text = "Senha:";      this.lblSenha.SetBounds(8, 136, 100, 20);
            this.txtSenha.SetBounds(112, 134, 160, 23);
            this.txtSenha.PasswordChar = '●';
            this.lblMysqldump.Text = "mysqldump:"; this.lblMysqldump.SetBounds(8, 164, 100, 20);
            this.txtMysqldump.SetBounds(112, 162, 130, 23);
            this.btnBrowseDump.Text = "...";    this.btnBrowseDump.SetBounds(248, 162, 30, 23);
            this.btnBrowseDump.Click += new System.EventHandler(this.BtnBrowseDump_Click);

            // grpMega
            this.grpMega.Text = "MEGA (MEGAcmd)";
            this.grpMega.SetBounds(360, 8, 340, 220);
            this.grpMega.Controls.Add(this.lblMegaEmail);
            this.grpMega.Controls.Add(this.txtMegaEmail);
            this.grpMega.Controls.Add(this.lblMegaSenha);
            this.grpMega.Controls.Add(this.txtMegaSenha);
            this.grpMega.Controls.Add(this.lblMegaPasta);
            this.grpMega.Controls.Add(this.txtMegaPasta);
            this.grpMega.Controls.Add(this.lblMegaCmd);
            this.grpMega.Controls.Add(this.txtMegaCmd);
            this.grpMega.Controls.Add(this.btnBrowseMegaCmd);
            this.grpMega.Controls.Add(this.btnTestarMega);

            this.lblMegaEmail.Text = "Email:";     this.lblMegaEmail.SetBounds(8, 24, 100, 20);
            this.txtMegaEmail.SetBounds(112, 22, 210, 23);
            this.lblMegaSenha.Text = "Senha:";     this.lblMegaSenha.SetBounds(8, 52, 100, 20);
            this.txtMegaSenha.SetBounds(112, 50, 210, 23);
            this.txtMegaSenha.PasswordChar = '●';
            this.lblMegaPasta.Text = "Pasta MEGA:"; this.lblMegaPasta.SetBounds(8, 80, 100, 20);
            this.txtMegaPasta.SetBounds(112, 78, 210, 23);
            this.lblMegaCmd.Text = "mega-cmd:";   this.lblMegaCmd.SetBounds(8, 108, 100, 20);
            this.txtMegaCmd.SetBounds(112, 106, 178, 23);
            this.btnBrowseMegaCmd.Text = "...";   this.btnBrowseMegaCmd.SetBounds(295, 106, 30, 23);
            this.btnBrowseMegaCmd.Click += new System.EventHandler(this.BtnBrowseMegaCmd_Click);
            this.btnTestarMega.Text = "Testar MEGA"; this.btnTestarMega.SetBounds(112, 140, 120, 28);
            this.btnTestarMega.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.btnTestarMega.ForeColor = System.Drawing.Color.White;
            this.btnTestarMega.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestarMega.FlatAppearance.BorderSize = 0;
            this.btnTestarMega.Click += new System.EventHandler(this.BtnTestarMega_Click);

            this.btnSalvarConexao.Text = "💾  Salvar Configurações";
            this.btnSalvarConexao.SetBounds(8, 240, 200, 32);
            this.btnSalvarConexao.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnSalvarConexao.ForeColor = System.Drawing.Color.White;
            this.btnSalvarConexao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvarConexao.FlatAppearance.BorderSize = 0;
            this.btnSalvarConexao.Click += new System.EventHandler(this.BtnSalvarConexao_Click);

            // ── tabAgendamento ──────────────────────────────────────────────────────
            this.tabAgendamento.Text = "Agendamento";
            this.tabAgendamento.Controls.Add(this.grpDias);
            this.tabAgendamento.Controls.Add(this.grpHorarios);
            this.tabAgendamento.Controls.Add(this.btnSalvarAgendamento);

            // grpDias
            this.grpDias.Text = "Dias da Semana";
            this.grpDias.SetBounds(8, 8, 690, 56);
            this.grpDias.Controls.Add(this.chkDom);
            this.grpDias.Controls.Add(this.chkSeg);
            this.grpDias.Controls.Add(this.chkTer);
            this.grpDias.Controls.Add(this.chkQua);
            this.grpDias.Controls.Add(this.chkQui);
            this.grpDias.Controls.Add(this.chkSex);
            this.grpDias.Controls.Add(this.chkSab);

            this.chkDom.Text = "Dom"; this.chkDom.SetBounds(8,  22, 80, 22);
            this.chkSeg.Text = "Seg"; this.chkSeg.SetBounds(90, 22, 80, 22);
            this.chkTer.Text = "Ter"; this.chkTer.SetBounds(172,22, 80, 22);
            this.chkQua.Text = "Qua"; this.chkQua.SetBounds(254,22, 80, 22);
            this.chkQui.Text = "Qui"; this.chkQui.SetBounds(336,22, 80, 22);
            this.chkSex.Text = "Sex"; this.chkSex.SetBounds(418,22, 80, 22);
            this.chkSab.Text = "Sáb"; this.chkSab.SetBounds(500,22, 80, 22);

            // grpHorarios
            this.grpHorarios.Text = "Horários de Backup";
            this.grpHorarios.SetBounds(8, 74, 400, 300);
            this.grpHorarios.Controls.Add(this.lblHorarioHint);
            this.grpHorarios.Controls.Add(this.txtNovoHorario);
            this.grpHorarios.Controls.Add(this.btnAddHorario);
            this.grpHorarios.Controls.Add(this.btnRemoverHorario);
            this.grpHorarios.Controls.Add(this.lstHorarios);

            this.lblHorarioHint.Text = "Formato HH:mm (ex: 03:00)";
            this.lblHorarioHint.SetBounds(8, 22, 240, 20);
            this.txtNovoHorario.SetBounds(8, 46, 100, 23);
            this.btnAddHorario.Text = "Adicionar"; this.btnAddHorario.SetBounds(116, 46, 90, 23);
            this.btnAddHorario.Click += new System.EventHandler(this.BtnAddHorario_Click);
            this.btnRemoverHorario.Text = "Remover"; this.btnRemoverHorario.SetBounds(212, 46, 90, 23);
            this.btnRemoverHorario.Click += new System.EventHandler(this.BtnRemoverHorario_Click);
            this.lstHorarios.SetBounds(8, 78, 200, 200);

            this.btnSalvarAgendamento.Text = "💾  Salvar Agendamento";
            this.btnSalvarAgendamento.SetBounds(8, 386, 200, 32);
            this.btnSalvarAgendamento.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnSalvarAgendamento.ForeColor = System.Drawing.Color.White;
            this.btnSalvarAgendamento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvarAgendamento.FlatAppearance.BorderSize = 0;
            this.btnSalvarAgendamento.Click += new System.EventHandler(this.BtnSalvarAgendamento_Click);

            // ── tabLog ──────────────────────────────────────────────────────────────
            this.tabLog.Text = "Log";
            this.tabLog.Controls.Add(this.rtxLog);
            this.tabLog.Controls.Add(this.btnLimparLog);

            this.rtxLog.ReadOnly = true;
            this.rtxLog.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            this.rtxLog.ForeColor = System.Drawing.Color.LightGreen;
            this.rtxLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular);
            this.rtxLog.SetBounds(4, 4, 700, 380);
            this.btnLimparLog.Text = "Limpar Log";
            this.btnLimparLog.SetBounds(4, 390, 120, 28);
            this.btnLimparLog.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnLimparLog.ForeColor = System.Drawing.Color.White;
            this.btnLimparLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimparLog.FlatAppearance.BorderSize = 0;
            this.btnLimparLog.Click += new System.EventHandler(this.BtnLimparLog_Click);

            // ── Botões inferiores ───────────────────────────────────────────────────
            this.btnExecutarAgora.Text = "▶  Backup Incremental Agora";
            this.btnExecutarAgora.SetBounds(8, 476, 220, 36);
            this.btnExecutarAgora.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.btnExecutarAgora.ForeColor = System.Drawing.Color.White;
            this.btnExecutarAgora.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecutarAgora.FlatAppearance.BorderSize = 0;
            this.btnExecutarAgora.Click += new System.EventHandler(this.BtnExecutarAgora_Click);

            this.btnExecutarCompleto.Text = "▶▶  Backup Completo Agora";
            this.btnExecutarCompleto.SetBounds(238, 476, 220, 36);
            this.btnExecutarCompleto.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnExecutarCompleto.ForeColor = System.Drawing.Color.White;
            this.btnExecutarCompleto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecutarCompleto.FlatAppearance.BorderSize = 0;
            this.btnExecutarCompleto.Click += new System.EventHandler(this.BtnExecutarCompleto_Click);

            this.lblStatus.Text = "Status: Aguardando...";
            this.lblStatus.SetBounds(470, 476, 260, 20);
            this.lblUltimoBackup.Text = "Último backup: —";
            this.lblUltimoBackup.SetBounds(470, 496, 260, 18);
            this.lblProximoBackup.Text = "Próximo backup: —";
            this.lblProximoBackup.SetBounds(470, 514, 260, 18);

            // ── NotifyIcon ──────────────────────────────────────────────────────────
            this.notifyIcon.Text = "PedeaiBackup";
            this.notifyIcon.Visible = true;
            this.notifyIcon.Icon = System.Drawing.SystemIcons.Application;
            this.notifyIcon.ContextMenuStrip = this.ctxTray;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.NotifyIcon_DoubleClick);

            this.mnuAbrir.Text = "Abrir";
            this.mnuAbrir.Click += new System.EventHandler(this.MnuAbrir_Click);
            this.mnuExecutar.Text = "Executar Backup Agora";
            this.mnuExecutar.Click += new System.EventHandler(this.MnuExecutar_Click);
            this.mnuFechar.Text = "Fechar";
            this.mnuFechar.Click += new System.EventHandler(this.MnuFechar_Click);
            this.ctxTray.Items.Add(this.mnuAbrir);
            this.ctxTray.Items.Add(this.mnuExecutar);
            this.ctxTray.Items.Add(this.mnuSeparador);
            this.ctxTray.Items.Add(this.mnuFechar);

            // ── Form ────────────────────────────────────────────────────────────────
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnExecutarAgora);
            this.Controls.Add(this.btnExecutarCompleto);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblUltimoBackup);
            this.Controls.Add(this.lblProximoBackup);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 245, 240);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ClientSize = new System.Drawing.Size(730, 520);
            this.MinimumSize = new System.Drawing.Size(746, 559);
            this.Text = "PedeaiBackup — Configuração de Backup";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // ── Controles ────────────────────────────────────────────────────────────────
        private System.Windows.Forms.TabControl             tabControl;
        private System.Windows.Forms.TabPage                tabConexao;
        private System.Windows.Forms.TabPage                tabAgendamento;
        private System.Windows.Forms.TabPage                tabLog;
        private System.Windows.Forms.GroupBox               grpDb;
        private System.Windows.Forms.Label                  lblHost;
        private System.Windows.Forms.TextBox                txtHost;
        private System.Windows.Forms.Label                  lblPorta;
        private System.Windows.Forms.TextBox                txtPorta;
        private System.Windows.Forms.Label                  lblBanco;
        private System.Windows.Forms.TextBox                txtBanco;
        private System.Windows.Forms.Label                  lblUsuario;
        private System.Windows.Forms.TextBox                txtUsuario;
        private System.Windows.Forms.Label                  lblSenha;
        private System.Windows.Forms.TextBox                txtSenha;
        private System.Windows.Forms.Label                  lblMysqldump;
        private System.Windows.Forms.TextBox                txtMysqldump;
        private System.Windows.Forms.Button                 btnBrowseDump;
        private System.Windows.Forms.GroupBox               grpMega;
        private System.Windows.Forms.Label                  lblMegaEmail;
        private System.Windows.Forms.TextBox                txtMegaEmail;
        private System.Windows.Forms.Label                  lblMegaSenha;
        private System.Windows.Forms.TextBox                txtMegaSenha;
        private System.Windows.Forms.Label                  lblMegaPasta;
        private System.Windows.Forms.TextBox                txtMegaPasta;
        private System.Windows.Forms.Label                  lblMegaCmd;
        private System.Windows.Forms.TextBox                txtMegaCmd;
        private System.Windows.Forms.Button                 btnBrowseMegaCmd;
        private System.Windows.Forms.Button                 btnTestarMega;
        private System.Windows.Forms.Button                 btnSalvarConexao;
        private System.Windows.Forms.GroupBox               grpDias;
        private System.Windows.Forms.CheckBox               chkDom;
        private System.Windows.Forms.CheckBox               chkSeg;
        private System.Windows.Forms.CheckBox               chkTer;
        private System.Windows.Forms.CheckBox               chkQua;
        private System.Windows.Forms.CheckBox               chkQui;
        private System.Windows.Forms.CheckBox               chkSex;
        private System.Windows.Forms.CheckBox               chkSab;
        private System.Windows.Forms.GroupBox               grpHorarios;
        private System.Windows.Forms.Label                  lblHorarioHint;
        private System.Windows.Forms.TextBox                txtNovoHorario;
        private System.Windows.Forms.Button                 btnAddHorario;
        private System.Windows.Forms.Button                 btnRemoverHorario;
        private System.Windows.Forms.ListBox                lstHorarios;
        private System.Windows.Forms.Button                 btnSalvarAgendamento;
        private System.Windows.Forms.RichTextBox            rtxLog;
        private System.Windows.Forms.Button                 btnLimparLog;
        private System.Windows.Forms.Button                 btnExecutarAgora;
        private System.Windows.Forms.Button                 btnExecutarCompleto;
        private System.Windows.Forms.Label                  lblStatus;
        private System.Windows.Forms.Label                  lblUltimoBackup;
        private System.Windows.Forms.Label                  lblProximoBackup;
        private System.Windows.Forms.NotifyIcon             notifyIcon;
        private System.Windows.Forms.ContextMenuStrip       ctxTray;
        private System.Windows.Forms.ToolStripMenuItem      mnuAbrir;
        private System.Windows.Forms.ToolStripMenuItem      mnuExecutar;
        private System.Windows.Forms.ToolStripSeparator     mnuSeparador;
        private System.Windows.Forms.ToolStripMenuItem      mnuFechar;
    }
}
