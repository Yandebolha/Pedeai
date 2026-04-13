using PedeaiBackup.Models;
using PedeaiBackup.Services;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PedeaiBackup.Forms
{
    public partial class frmBackupConfig : Form
    {
        private BackupConfig    _config;
        private BackupEstado    _estado;
        private AgendadorService _agendador;

        public frmBackupConfig()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode ==
                System.ComponentModel.LicenseUsageMode.Designtime) return;

            _config   = ConfigManager.CarregarConfig();
            _estado   = ConfigManager.CarregarEstado();
            _agendador = new AgendadorService();

            _agendador.OnLog += AdicionarLog;
            _agendador.OnBackupConcluido += OnBackupConcluido;

            Load += FrmBackupConfig_Load;
            FormClosing += FrmBackupConfig_FormClosing;
        }

        // ── Ciclo de vida ─────────────────────────────────────────────────────────────

        private void FrmBackupConfig_Load(object sender, EventArgs e)
        {
            CarregarUiDeConfig();
            _agendador.Iniciar(_config, _estado);
            AtualizarStatusBar();
        }

        private void FrmBackupConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ao fechar, minimiza para systray em vez de sair (a menos que seja pelo menu)
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Minimize();
            }
        }

        private void Minimize()
        {
            Hide();
            notifyIcon.BalloonTipTitle = "PedeaiBackup";
            notifyIcon.BalloonTipText  = "Backup em execução em segundo plano.";
            notifyIcon.ShowBalloonTip(2000);
        }

        // ── UI → Config ───────────────────────────────────────────────────────────────

        private void CarregarUiDeConfig()
        {
            // Conexão DB
            txtHost.Text      = _config.DbHost;
            txtPorta.Text     = _config.DbPorta;
            txtBanco.Text     = _config.DbNome;
            txtUsuario.Text   = _config.DbUsuario;
            txtSenha.Text     = _config.DbSenha;
            txtMysqldump.Text = _config.MySqlDumpPath;

            // MEGA
            txtMegaEmail.Text = _config.MegaEmail;
            txtMegaSenha.Text = _config.MegaSenha;
            txtMegaPasta.Text = _config.MegaPasta;
            txtMegaCmd.Text   = _config.MegaCmdPath;

            // Agendamento — dias
            chkDom.Checked = _config.DiasAtivos.Contains(0);
            chkSeg.Checked = _config.DiasAtivos.Contains(1);
            chkTer.Checked = _config.DiasAtivos.Contains(2);
            chkQua.Checked = _config.DiasAtivos.Contains(3);
            chkQui.Checked = _config.DiasAtivos.Contains(4);
            chkSex.Checked = _config.DiasAtivos.Contains(5);
            chkSab.Checked = _config.DiasAtivos.Contains(6);

            // Agendamento — horários
            lstHorarios.Items.Clear();
            foreach (string h in _config.Horarios)
                lstHorarios.Items.Add(h);
        }

        private void ConfigDeUi()
        {
            _config.DbHost        = txtHost.Text.Trim();
            _config.DbPorta       = txtPorta.Text.Trim();
            _config.DbNome        = txtBanco.Text.Trim();
            _config.DbUsuario     = txtUsuario.Text.Trim();
            _config.DbSenha       = txtSenha.Text;
            _config.MySqlDumpPath = string.IsNullOrWhiteSpace(txtMysqldump.Text) ? "mysqldump" : txtMysqldump.Text.Trim();

            _config.MegaEmail   = txtMegaEmail.Text.Trim();
            _config.MegaSenha   = txtMegaSenha.Text;
            _config.MegaPasta   = txtMegaPasta.Text.Trim();
            _config.MegaCmdPath = string.IsNullOrWhiteSpace(txtMegaCmd.Text) ? "mega-cmd" : txtMegaCmd.Text.Trim();
        }

        private void AgendamentoDeUi()
        {
            _config.DiasAtivos.Clear();
            if (chkDom.Checked) _config.DiasAtivos.Add(0);
            if (chkSeg.Checked) _config.DiasAtivos.Add(1);
            if (chkTer.Checked) _config.DiasAtivos.Add(2);
            if (chkQua.Checked) _config.DiasAtivos.Add(3);
            if (chkQui.Checked) _config.DiasAtivos.Add(4);
            if (chkSex.Checked) _config.DiasAtivos.Add(5);
            if (chkSab.Checked) _config.DiasAtivos.Add(6);

            _config.Horarios.Clear();
            foreach (object item in lstHorarios.Items)
                _config.Horarios.Add(item.ToString());
        }

        // ── Eventos Conexão/MEGA ──────────────────────────────────────────────────────

        private void BtnBrowseDump_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Title  = "Localizar mysqldump.exe";
            dlg.Filter = "Executáveis|*.exe|Todos|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
                txtMysqldump.Text = dlg.FileName;
        }

        private void BtnBrowseMegaCmd_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Title  = "Localizar mega-cmd.exe ou mega-cmd.bat";
            dlg.Filter = "Executáveis|*.exe;*.bat|Todos|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
                txtMegaCmd.Text = dlg.FileName;
        }

        private void BtnTestarMega_Click(object sender, EventArgs e)
        {
            ConfigDeUi();
            btnTestarMega.Enabled = false;
            btnTestarMega.Text    = "Testando...";

            bool logado = MegaService.EstaLogado(_config.MegaCmdPath);

            if (!logado)
            {
                if (string.IsNullOrWhiteSpace(_config.MegaEmail) || string.IsNullOrWhiteSpace(_config.MegaSenha))
                {
                    MessageBox.Show("Informe Email e Senha do MEGA antes de testar.", "MEGA",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnTestarMega.Enabled = true;
                    btnTestarMega.Text    = "Testar MEGA";
                    return;
                }

                var (ok, msg) = MegaService.Login(_config.MegaCmdPath, _config.MegaEmail, _config.MegaSenha);
                if (!ok)
                {
                    MessageBox.Show($"Falha no login MEGA:\n{msg}", "MEGA",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnTestarMega.Enabled = true;
                    btnTestarMega.Text    = "Testar MEGA";
                    return;
                }
                logado = true;
            }

            if (logado)
                MessageBox.Show("MEGAcmd conectado com sucesso!", "MEGA",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnTestarMega.Enabled = true;
            btnTestarMega.Text    = "Testar MEGA";
        }

        private void BtnSalvarConexao_Click(object sender, EventArgs e)
        {
            ConfigDeUi();
            ConfigManager.SalvarConfig(_config);
            _agendador.AtualizarConfig(_config, _estado);
            MessageBox.Show("Configurações salvas!", "PedeaiBackup",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ── Eventos Agendamento ───────────────────────────────────────────────────────

        private void BtnAddHorario_Click(object sender, EventArgs e)
        {
            string horario = txtNovoHorario.Text.Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(horario, @"^\d{2}:\d{2}$"))
            {
                MessageBox.Show("Formato inválido. Use HH:mm (ex: 03:00).", "Horário",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!lstHorarios.Items.Contains(horario))
                lstHorarios.Items.Add(horario);
            txtNovoHorario.Clear();
        }

        private void BtnRemoverHorario_Click(object sender, EventArgs e)
        {
            if (lstHorarios.SelectedIndex >= 0)
                lstHorarios.Items.RemoveAt(lstHorarios.SelectedIndex);
        }

        private void BtnSalvarAgendamento_Click(object sender, EventArgs e)
        {
            AgendamentoDeUi();
            ConfigManager.SalvarConfig(_config);
            _agendador.AtualizarConfig(_config, _estado);
            AtualizarStatusBar();
            MessageBox.Show("Agendamento salvo!", "PedeaiBackup",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ── Eventos botões principais ─────────────────────────────────────────────────

        private void BtnExecutarAgora_Click(object sender, EventArgs e)
        {
            ConfigDeUi();
            SetBotoesBloqueados(true);
            lblStatus.Text = "Status: Executando backup incremental...";
            _agendador.ExecutarBackup(forcarCompleto: false);
        }

        private void BtnExecutarCompleto_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja executar um backup COMPLETO agora?\n" +
                                "Isso pode demorar dependendo do tamanho do banco.",
                "Backup Completo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            ConfigDeUi();
            SetBotoesBloqueados(true);
            lblStatus.Text = "Status: Executando backup completo...";
            _agendador.ExecutarBackup(forcarCompleto: true);
        }

        private void BtnLimparLog_Click(object sender, EventArgs e)
        {
            rtxLog.Clear();
        }

        // ── Systray ───────────────────────────────────────────────────────────────────

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void MnuAbrir_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void MnuExecutar_Click(object sender, EventArgs e)
        {
            _agendador.ExecutarBackup(forcarCompleto: false);
        }

        private void MnuFechar_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            _agendador?.Dispose();
            Application.Exit();
        }

        // ── Callbacks do agendador ────────────────────────────────────────────────────

        private void AdicionarLog(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AdicionarLog), msg);
                return;
            }
            rtxLog.AppendText(msg + Environment.NewLine);
            rtxLog.ScrollToCaret();
        }

        private void OnBackupConcluido(BackupResultado resultado)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<BackupResultado>(OnBackupConcluido), resultado);
                return;
            }
            SetBotoesBloqueados(false);
            _estado = ConfigManager.CarregarEstado();
            AtualizarStatusBar();

            if (resultado.Sucesso)
                notifyIcon.ShowBalloonTip(3000, "PedeaiBackup",
                    $"Backup {resultado.Tipo} concluído: {resultado.Arquivo}", ToolTipIcon.Info);
            else
                notifyIcon.ShowBalloonTip(5000, "PedeaiBackup — Falha",
                    $"Erro no backup: {resultado.Erro}", ToolTipIcon.Error);
        }

        // ── Utilitários UI ────────────────────────────────────────────────────────────

        private void AtualizarStatusBar()
        {
            DateTime? ultimo = _estado.UltimoBackupIncremental ?? _estado.UltimoBackupCompleto;
            lblUltimoBackup.Text = ultimo.HasValue
                ? $"Último backup: {ultimo.Value:dd/MM/yyyy HH:mm}"
                : "Último backup: —";
            lblStatus.Text = "Status: Aguardando agendamento...";
        }

        private void SetBotoesBloqueados(bool bloqueado)
        {
            btnExecutarAgora.Enabled    = !bloqueado;
            btnExecutarCompleto.Enabled = !bloqueado;
            if (!bloqueado)
                lblStatus.Text = "Status: Aguardando agendamento...";
        }
    }
}
