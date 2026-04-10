using System;
using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pedeai.BLL;

namespace Pedeai.Forms
{
    public class frmWhatsApp : Form
    {
        // ── Campos ───────────────────────────────────────────────────────────
        private System.Windows.Forms.Timer _timerStatus;
        private PictureBox  _picQR;
        private Label       _lblStatus;
        private Label       _lblInstrucao;
        private Button      _btnRefreshQR;
        private Button      _btnDesconectar;
        private TextBox     _txtTelefone;
        private Label       _lblPairingCode;
        private TextBox     _txtApiUrl;
        private TextBox     _txtApiKey;
        private TextBox     _txtInstance;
        private TextBox     _txtMsgPreparo;
        private TextBox     _txtMsgEntrega;
        private TextBox     _txtMsgCupom;

        private static readonly Color CorHeader  = Color.FromArgb(176, 110, 42);
        private static readonly Color CorFundo   = Color.FromArgb(245, 237, 216);
        private static readonly Color CorPainel  = Color.FromArgb(235, 226, 208);
        private static readonly Color CorVerde   = Color.FromArgb(39, 174, 96);
        private static readonly Color CorVermelho= Color.FromArgb(192, 57, 43);

        public frmWhatsApp()
        {
            Text          = "WhatsApp";
            BackColor     = CorFundo;
            ForeColor     = Color.FromArgb(50, 40, 20);
            Font          = new Font("Segoe UI", 9F);
            ClientSize    = new Size(700, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize   = new Size(640, 500);

            ConstruirUI();
            Load += FrmWhatsApp_Load;
        }

        // ── Construção da UI ──────────────────────────────────────────────────

        private void ConstruirUI()
        {
            // Top bar
            var pnlTop = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 48,
                BackColor = CorHeader,
            };
            var lblTitulo = new Label
            {
                Text      = "  \uD83D\uDCF1  WhatsApp",
                Dock      = DockStyle.Fill,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
            };
            pnlTop.Controls.Add(lblTitulo);

            // TabControl
            var tabs = new TabControl
            {
                Dock      = DockStyle.Fill,
                Font      = new Font("Segoe UI", 9.5F),
                BackColor = CorFundo,
            };

            var tabConexao = new TabPage { Text = " 🔗  Conexão ", BackColor = CorFundo };
            var tabConfig  = new TabPage { Text = " ⚙️  Configurações ", BackColor = CorFundo };

            ConstruirTabConexao(tabConexao);
            ConstruirTabConfig(tabConfig);

            tabs.TabPages.Add(tabConexao);
            tabs.TabPages.Add(tabConfig);

            Controls.Add(tabs);
            Controls.Add(pnlTop);
        }

        private void ConstruirTabConexao(TabPage tab)
        {
            // Status bar no topo
            var pnlStatus = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 44,
                BackColor = CorPainel,
            };

            _lblStatus = new Label
            {
                Text      = "  ⏳  Verificando...",
                Left      = 8,
                Top       = 11,
                AutoSize  = true,
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 80, 30),
            };

            _btnRefreshQR = new Button
            {
                Text      = "🔄  Atualizar QR",
                Left      = 350,
                Top       = 8,
                Width     = 130,
                Height    = 28,
                BackColor = CorHeader,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Anchor    = AnchorStyles.Top | AnchorStyles.Right,
            };
            _btnRefreshQR.FlatAppearance.BorderSize = 0;
            _btnRefreshQR.Click += BtnRefreshQR_Click;

            _btnDesconectar = new Button
            {
                Text      = "⏹ Desconectar",
                Left      = 490,
                Top       = 8,
                Width     = 130,
                Height    = 28,
                BackColor = CorVermelho,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Enabled   = false,
                Anchor    = AnchorStyles.Top | AnchorStyles.Right,
            };
            _btnDesconectar.FlatAppearance.BorderSize = 0;
            _btnDesconectar.Click += BtnDesconectar_Click;

            pnlStatus.Controls.Add(_lblStatus);
            pnlStatus.Controls.Add(_btnRefreshQR);
            pnlStatus.Controls.Add(_btnDesconectar);

            // Instrução
            _lblInstrucao = new Label
            {
                Text      = "Abra o WhatsApp no celular → Dispositivos conectados → Conectar dispositivo → Escaneie o QR Code abaixo",
                Dock      = DockStyle.Top,
                Height    = 36,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(255, 248, 225),
                ForeColor = Color.FromArgb(120, 80, 20),
                Font      = new Font("Segoe UI", 8.5F),
                Padding   = new Padding(8, 0, 8, 0),
            };

            // QR code
            _picQR = new PictureBox
            {
                Dock      = DockStyle.Fill,
                SizeMode  = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                Padding   = new Padding(20),
            };

            // ── Painel de pareamento por código ──────────────────────────────
            var pnlPairing = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 72,
                BackColor = Color.FromArgb(240, 230, 210),
                Padding   = new Padding(8, 6, 8, 6),
            };

            var lblPairingTitle = new Label
            {
                Text      = "📲  Pareamento por código (não precisa escanear):",
                Left = 8, Top = 4, AutoSize = true,
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = CorHeader,
            };

            _txtTelefone = new TextBox
            {
                Left = 8, Top = 24, Width = 170, Height = 22,
                PlaceholderText = "DDD + número (ex: 11999999999)",
                Font = new Font("Segoe UI", 8.5F),
                BorderStyle = BorderStyle.FixedSingle,
            };

            var btnGerarCodigo = new Button
            {
                Text      = "🔑  Gerar Código",
                Left = 184, Top = 22, Width = 130, Height = 26,
                BackColor = CorHeader,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            };
            btnGerarCodigo.FlatAppearance.BorderSize = 0;
            btnGerarCodigo.Click += BtnGerarCodigo_Click;

            _lblPairingCode = new Label
            {
                Text      = "",
                Left = 320, Top = 22, AutoSize = true,
                Font      = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = CorVerde,
            };

            pnlPairing.Controls.Add(lblPairingTitle);
            pnlPairing.Controls.Add(_txtTelefone);
            pnlPairing.Controls.Add(btnGerarCodigo);
            pnlPairing.Controls.Add(_lblPairingCode);

            tab.Controls.Add(_picQR);
            tab.Controls.Add(_lblInstrucao);
            tab.Controls.Add(pnlPairing);
            tab.Controls.Add(pnlStatus);
        }

        private void ConstruirTabConfig(TabPage tab)
        {
            var pnl = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(16) };

            int y = 12;

            Label MkTitle(string texto)
            {
                return new Label
                {
                    Text      = texto,
                    Left      = 0, Top = y,
                    AutoSize  = true,
                    Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    ForeColor = CorHeader,
                };
            }

            Label MkLabel(string texto, int top)
            {
                return new Label { Text = texto, Left = 0, Top = top, AutoSize = true,
                    ForeColor = Color.FromArgb(60, 50, 30) };
            }

            TextBox MkTxt(int top, int width, string valor, bool multiline = false)
            {
                var tb = new TextBox
                {
                    Left        = 180, Top = top, Width = width,
                    BackColor   = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Text        = valor ?? "",
                };
                if (multiline)
                {
                    tb.Multiline = true;
                    tb.Height   = 54;
                    tb.ScrollBars = ScrollBars.Vertical;
                }
                return tb;
            }

            // ─ Conexão API ─
            var lblSec1 = MkTitle("Conexão com Evolution API");
            lblSec1.Top = y; pnl.Controls.Add(lblSec1); y += 26;

            // Painel de ajuda — como obter as credenciais
            var pnlAjuda = new Panel
            {
                Left      = 0, Top = y,
                Width     = 620, Height = 116,
                BackColor = Color.FromArgb(255, 248, 225),
                BorderStyle = BorderStyle.FixedSingle,
            };

            var lblAjudaTitulo = new Label
            {
                Text      = "❓  O que preencher aqui?",
                Left      = 8, Top = 6, AutoSize = true,
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = CorHeader,
            };

            var lblAjudaTexto = new Label
            {
                Text      = "Você precisa do serviço Evolution API rodando (gratuito e local).\r\n" +
                            "1. Instale o Docker Desktop: https://docker.com/products/docker-desktop\r\n" +
                            "2. Abra o PowerShell na pasta do projeto e execute: docker compose up -d\r\n" +
                            "3. Preencha: URL = http://localhost:8081  |  API Key = pedeaikey  |  Instância = pedeai",
                Left      = 8, Top = 24, Width = 590,
                Font      = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(60, 50, 20),
                AutoSize  = false, Height = 56,
            };

            // Compose file fica na raiz do repositório (um nível acima do executável em Debug/Release)
            string composeDir = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "..", "..", "..", ".."));
            string composeFile = System.IO.Path.Combine(composeDir, "docker-compose.yml");
            // Fallback: pasta corrente do executável
            if (!System.IO.File.Exists(composeFile))
                composeFile = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "docker-compose.yml");
            string DockerCmd = $"docker compose -f \"{composeFile}\" up -d";

            var btnCopiarDocker = new Button
            {
                Text      = "📋  Copiar comando Docker",
                Left      = 8, Top = 84,
                Width     = 190, Height = 26,
                BackColor = Color.FromArgb(52, 100, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 8F),
            };
            btnCopiarDocker.FlatAppearance.BorderSize = 0;
            btnCopiarDocker.Click += (_, __) =>
            {
                Clipboard.SetText(DockerCmd);
                btnCopiarDocker.Text = "✔  Copiado!";
                var t = new System.Windows.Forms.Timer { Interval = 2000 };
                t.Tick += (s, e) => { btnCopiarDocker.Text = "📋  Copiar comando Docker"; t.Stop(); t.Dispose(); };
                t.Start();
            };

            var btnAbrirDocker = new Button
            {
                Text      = "🌐  Baixar Docker",
                Left      = 206, Top = 84,
                Width     = 130, Height = 26,
                BackColor = Color.FromArgb(87, 120, 38),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 8F),
            };
            btnAbrirDocker.FlatAppearance.BorderSize = 0;
            btnAbrirDocker.Click += (_, __) =>
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    { FileName = "https://docker.com/products/docker-desktop", UseShellExecute = true });

            pnlAjuda.Controls.Add(lblAjudaTitulo);
            pnlAjuda.Controls.Add(lblAjudaTexto);
            pnlAjuda.Controls.Add(btnCopiarDocker);
            pnlAjuda.Controls.Add(btnAbrirDocker);
            pnl.Controls.Add(pnlAjuda);
            y += 124;

            // Botão preencher padrão
            var btnPadrao = new Button
            {
                Text      = "⚡  Preencher padrão local",
                Left      = 0, Top = y,
                Width     = 180, Height = 26,
                BackColor = CorHeader,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 8F),
            };
            btnPadrao.FlatAppearance.BorderSize = 0;
            btnPadrao.Click += (_, __) =>
            {
                _txtApiUrl.Text  = "http://localhost:8081";
                _txtApiKey.Text  = "pedeaikey";
                _txtInstance.Text = "pedeai";
            };
            pnl.Controls.Add(btnPadrao);
            y += 34;

            pnl.Controls.Add(MkLabel("URL da API:", y + 3));
            _txtApiUrl = MkTxt(y, 380, WhatsAppService.ApiUrl);
            _txtApiUrl.PlaceholderText = "Ex: http://localhost:8081";
            pnl.Controls.Add(_txtApiUrl); y += 28;

            pnl.Controls.Add(MkLabel("API Key:", y + 3));
            _txtApiKey = MkTxt(y, 380, WhatsAppService.ApiKey);
            _txtApiKey.PlaceholderText = "Ex: pedeaikey";
            pnl.Controls.Add(_txtApiKey); y += 28;

            pnl.Controls.Add(MkLabel("Instância:", y + 3));
            _txtInstance = MkTxt(y, 200, string.IsNullOrWhiteSpace(WhatsAppService.Instance) ? "pedeai" : WhatsAppService.Instance);
            pnl.Controls.Add(_txtInstance); y += 36;

            // Separator
            var sep1 = new Panel { Left = 0, Top = y, Width = 580, Height = 1, BackColor = Color.FromArgb(200, 185, 160) };
            pnl.Controls.Add(sep1); y += 12;

            // ─ Templates de mensagens ─
            var lblSec2 = MkTitle("Mensagens Automáticas");
            lblSec2.Top = y; pnl.Controls.Add(lblSec2); y += 24;

            var lblTagsDisp = new Label
            {
                Text      = "Tags: {Nome}  {Numero}  {Total}  {CupomCodigo}  {Validade}",
                Left      = 0, Top = y, AutoSize = true,
                Font      = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(100, 80, 50),
            };
            pnl.Controls.Add(lblTagsDisp); y += 22;

            pnl.Controls.Add(MkLabel("🍕 Em preparo:", y + 3));
            _txtMsgPreparo = MkTxt(y, 450,
                ConfigurationManager.AppSettings["WhatsAppMsgPreparo"]
                ?? "Olá {Nome}! 🍕 Seu pedido #{Numero} já está sendo preparado. Em breve ficará pronto!",
                true);
            pnl.Controls.Add(_txtMsgPreparo); y += 62;

            pnl.Controls.Add(MkLabel("🛵 Saiu p/ entrega:", y + 3));
            _txtMsgEntrega = MkTxt(y, 450,
                ConfigurationManager.AppSettings["WhatsAppMsgEntrega"]
                ?? "Olá {Nome}! 🛵 Seu pedido #{Numero} saiu para entrega. Aguarde em breve!",
                true);
            pnl.Controls.Add(_txtMsgEntrega); y += 62;

            pnl.Controls.Add(MkLabel("🎁 Cupom fidelidade:", y + 3));
            _txtMsgCupom = MkTxt(y, 450,
                ConfigurationManager.AppSettings["WhatsAppMsgCupom"]
                ?? "Parabéns {Nome}! 🎉 Você ganhou um cupom: *{CupomCodigo}*\nVálido até {Validade}. Use no próximo pedido!",
                true);
            pnl.Controls.Add(_txtMsgCupom); y += 68;

            // Botões
            var btnSalvar = new Button
            {
                Text      = "💾  Salvar Configurações",
                Left      = 0, Top = y,
                Width     = 200, Height = 32,
                BackColor = Color.FromArgb(87, 120, 38),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            btnSalvar.Click += BtnSalvar_Click;

            var btnTestar = new Button
            {
                Text      = "🔌  Testar Conexão",
                Left      = 212, Top = y,
                Width     = 160, Height = 32,
                BackColor = Color.FromArgb(52, 100, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
            };
            btnTestar.FlatAppearance.BorderSize = 0;
            btnTestar.Click += BtnTestar_Click;

            pnl.Controls.Add(btnSalvar);
            pnl.Controls.Add(btnTestar);

            tab.Controls.Add(pnl);
        }

        // ── Eventos ───────────────────────────────────────────────────────────

        private void FrmWhatsApp_Load(object sender, EventArgs e)
        {
            if (!WhatsAppService.Ativo)
            {
                AtualizarStatus("desconectado");
                _lblInstrucao.Text = "⚙️  Configure a URL da API na aba Configurações para começar.";
                _lblInstrucao.Visible = true;
                return;
            }

            // Inicia timer de status (30s)
            _timerStatus = new System.Windows.Forms.Timer { Interval = 30000 };
            _timerStatus.Tick += TimerStatus_Tick;
            _timerStatus.Start();

            // Verifica status imediatamente
            Task.Run(CarregarQRAsync);
        }

        private async void TimerStatus_Tick(object sender, EventArgs e)
        {
            string st = await WhatsAppService.ObterStatusAsync().ConfigureAwait(false);
            if (IsDisposed) return;
            Invoke(new Action(() => AtualizarStatus(st)));

            if (st == "open")
            {
                _timerStatus.Interval = 30000;
            }
            else
            {
                _timerStatus.Interval = 15000;
                await Task.Run(CarregarQRAsync).ConfigureAwait(false);
            }
        }

        private async void BtnGerarCodigo_Click(object sender, EventArgs e)
        {
            if (!WhatsAppService.Ativo)
            {
                MessageBox.Show("Configure a URL da API na aba Configurações antes de gerar o código.",
                    "WhatsApp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string tel = _txtTelefone.Text.Trim();
            if (string.IsNullOrWhiteSpace(tel))
            {
                MessageBox.Show("Digite seu número com DDD (ex: 11999999999).",
                    "WhatsApp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _lblPairingCode.Text = "⏳ Gerando...";
            string code = await WhatsAppService.ObterPairingCodeAsync(tel).ConfigureAwait(false);
            if (IsDisposed) return;
            Invoke(new Action(() =>
            {
                if (string.IsNullOrEmpty(code))
                {
                    _lblPairingCode.Text = "❌ Erro";
                    MessageBox.Show("Não foi possível gerar o código. Verifique se a API está rodando e o número está correto.",
                        "WhatsApp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Formata o código com traço no meio (ex: ABCD-5678)
                    string fmt = code.Length == 8 ? code.Substring(0, 4) + "-" + code.Substring(4) : code;
                    _lblPairingCode.Text = fmt;
                    _lblInstrucao.Text = $"No WhatsApp: Configurações → Dispositivos conectados → Vincular com número → Digite: {fmt}";
                    // Refresh QR to show the new one generated for this session
                    Task.Run(CarregarQRAsync);
                    // Ensure the timer polls every 15s to keep the code fresh
                    if (_timerStatus == null)
                    {
                        _timerStatus = new System.Windows.Forms.Timer { Interval = 15000 };
                        _timerStatus.Tick += TimerStatus_Tick;
                        _timerStatus.Start();
                    }
                    else
                    {
                        _timerStatus.Interval = 15000;
                    }
                }
            }));
        }

        private async void BtnRefreshQR_Click(object sender, EventArgs e)
        {
            if (!WhatsAppService.Ativo)
            {
                MessageBox.Show("Configure a URL da API na aba Configurações antes de conectar.",
                    "WhatsApp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            await Task.Run(CarregarQRAsync);
        }

        private async void BtnDesconectar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja desconectar o WhatsApp?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            _btnDesconectar.Enabled = false;
            bool ok = await WhatsAppService.DesconectarAsync().ConfigureAwait(false);
            if (IsDisposed) return;
            Invoke(new Action(() =>
            {
                if (ok)
                {
                    AtualizarStatus("desconectado");
                    _picQR.Image = null;
                    Task.Run(CarregarQRAsync);
                }
                else
                {
                    MessageBox.Show("Erro ao desconectar.", "WhatsApp");
                }
            }));
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                WhatsAppService.SalvarConfiguracao(
                    _txtApiUrl.Text.Trim(),
                    _txtApiKey.Text.Trim(),
                    string.IsNullOrWhiteSpace(_txtInstance.Text) ? "pedeai" : _txtInstance.Text.Trim(),
                    _txtMsgPreparo.Text.Trim(),
                    _txtMsgEntrega.Text.Trim(),
                    _txtMsgCupom.Text.Trim());
                MessageBox.Show("Configurações salvas! Clique em 'Atualizar QR' para conectar.",
                    "WhatsApp", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnTestar_Click(object sender, EventArgs e)
        {
            if (!WhatsAppService.Ativo)
            {
                MessageBox.Show("Informe a URL da API e salve antes de testar.",
                    "WhatsApp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string st = await WhatsAppService.ObterStatusAsync().ConfigureAwait(false);
            if (IsDisposed) return;
            Invoke(new Action(() =>
            {
                AtualizarStatus(st);
                string stLabel = st == "open" ? "🟢 Conectado" : $"🔴 {st}";
                MessageBox.Show($"Status da conexão: {stLabel}", "Teste WhatsApp",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        // ── Lógica de UI ─────────────────────────────────────────────────────

        private async Task CarregarQRAsync()
        {
            if (!WhatsAppService.Ativo) return;
            var result = await WhatsAppService.ObterQRAsync().ConfigureAwait(false);
            if (IsDisposed) return;
            Invoke(new Action(() =>
            {
                AtualizarStatus(result.status);
                if (result.qrImage != null)
                {
                    _picQR.Image = result.qrImage;
                    _lblInstrucao.Visible = result.status != "open";
                }
                else if (!string.IsNullOrEmpty(result.erro))
                {
                    _lblInstrucao.Text = "⚠️ " + result.erro;
                }

                // Update pairing code label if a fresh code is available
                if (!string.IsNullOrEmpty(result.pairingCode))
                {
                    string fmt = result.pairingCode.Length == 8
                        ? result.pairingCode.Substring(0, 4) + "-" + result.pairingCode.Substring(4)
                        : result.pairingCode;
                    _lblPairingCode.Text = fmt;
                    _lblInstrucao.Text = $"No WhatsApp: Configurações → Dispositivos conectados → Vincular com número → Digite: {fmt}";
                }
            }));
        }

        private void AtualizarStatus(string status)
        {
            string texto;
            Color cor;

            switch (status?.ToLower())
            {
                case "open":
                    texto = "  🟢  Conectado";
                    cor   = CorVerde;
                    _btnDesconectar.Enabled = true;
                    _btnRefreshQR.Enabled   = false;
                    _lblInstrucao.Visible   = false;
                    _picQR.Image            = null;
                    break;
                case "connecting":
                    texto = "  🟡  Conectando...";
                    cor   = Color.FromArgb(230, 150, 20);
                    _btnDesconectar.Enabled = false;
                    _btnRefreshQR.Enabled   = true;
                    break;
                case "close":
                case "desconectado":
                    texto = "  🔴  Desconectado — escaneie o QR Code";
                    cor   = CorVermelho;
                    _btnDesconectar.Enabled = false;
                    _btnRefreshQR.Enabled   = true;
                    _lblInstrucao.Visible   = true;
                    break;
                default:
                    texto = $"  ⏳  {status}";
                    cor   = Color.FromArgb(100, 80, 30);
                    _btnRefreshQR.Enabled = true;
                    break;
            }

            _lblStatus.Text      = texto;
            _lblStatus.ForeColor = cor;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _timerStatus?.Stop();
            _timerStatus?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
