using System;
using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pedeai.BLL;

namespace Pedeai.Forms
{
    public partial class frmWhatsApp : Form
    {
        // ── Constantes de cor ───────────────────────────────────────────────
        private static readonly Color CorHeader   = Color.FromArgb(176, 110, 42);
        private static readonly Color CorFundo    = Color.FromArgb(245, 237, 216);
        private static readonly Color CorPainel   = Color.FromArgb(235, 226, 208);
        private static readonly Color CorVerde    = Color.FromArgb(39, 174, 96);
        private static readonly Color CorVermelho = Color.FromArgb(192, 57, 43);

        public frmWhatsApp()
        {
            InitializeComponent();
            ConstruirUI();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _promDal = new DAL.PromocaoDAL();
            _promDal.EnsureMigrations();
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

            var tabConexao   = new TabPage { Text = " 🔗  Conexão ",       BackColor = CorFundo };
            var tabConfig    = new TabPage { Text = " ⚙️  Configurações ", BackColor = CorFundo };
            var tabPromocoes = new TabPage { Text = " 🏷️  Promoções ",     BackColor = CorFundo };
            var tabCardapio  = new TabPage { Text = " 🍽️  Cardápio do Dia ", BackColor = CorFundo };

            ConstruirTabConexao(tabConexao);
            ConstruirTabConfig(tabConfig);
            ConstruirTabPromocoes(tabPromocoes);
            ConstruirTabCardapio(tabCardapio);

            tabs.TabPages.Add(tabConexao);
            tabs.TabPages.Add(tabConfig);
            tabs.TabPages.Add(tabPromocoes);
            tabs.TabPages.Add(tabCardapio);

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
                _txtApiUrl.Text  = "https://evolutionapi.rangofood.com.br";
                _txtApiKey.Text  = "pedeaikey";
                _txtInstance.Text = "pedeai";
            };
            pnl.Controls.Add(btnPadrao);
            y += 34;

            pnl.Controls.Add(MkLabel("URL da API:", y + 3));
            _txtApiUrl = MkTxt(y, 380, WhatsAppService.ApiUrl);
            _txtApiUrl.PlaceholderText = "Ex: http://192.168.1.10:8081";
            pnl.Controls.Add(_txtApiUrl); y += 28;

            // Aviso + botão de substituição quando URL ainda usa localhost
            var pnlAvisoUrl = new Panel
            {
                Left      = 0, Top = y,
                Width     = 580, Height = 36,
                BackColor = Color.FromArgb(255, 243, 205),
                BorderStyle = BorderStyle.FixedSingle,
                Visible   = ApiUrlUsaLocalhost(),
            };
            var lblAvisoUrl = new Label
            {
                Text      = "⚠  A URL usa 'localhost' — outras máquinas em rede não conseguirão conectar.",
                Left = 6, Top = 4, AutoSize = true,
                Font      = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(130, 80, 10),
            };
            var btnUsarIpServidor = new Button
            {
                Text      = "🌐  Usar IP do servidor",
                Left = 430, Top = 4, Width = 140, Height = 26,
                BackColor = CorHeader,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 8F, FontStyle.Bold),
            };
            btnUsarIpServidor.FlatAppearance.BorderSize = 0;
            btnUsarIpServidor.Click += (_, __) =>
            {
                string ip = ObterIpServidor();
                if (!string.IsNullOrEmpty(ip))
                {
                    string porta = "8081";
                    if (!string.IsNullOrEmpty(_txtApiUrl.Text))
                    {
                        try { porta = new Uri(_txtApiUrl.Text).Port.ToString(); } catch { }
                    }
                    _txtApiUrl.Text = $"http://{ip}:{porta}";
                    pnlAvisoUrl.Visible = ApiUrlUsaLocalhost();
                }
                else
                {
                    MessageBox.Show(
                        "Não foi possível detectar o IP do servidor automaticamente.\n" +
                        "Substitua 'localhost' pelo IP do servidor manualmente na URL da API.\n" +
                        "Exemplo: http://192.168.1.10:8081",
                        "IP não detectado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            _txtApiUrl.TextChanged += (_, __) => pnlAvisoUrl.Visible = ApiUrlUsaLocalhost();
            pnlAvisoUrl.Controls.Add(lblAvisoUrl);
            pnlAvisoUrl.Controls.Add(btnUsarIpServidor);
            pnl.Controls.Add(pnlAvisoUrl); y += 42;

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
                !string.IsNullOrWhiteSpace(WhatsAppService.MsgPreparo)
                    ? WhatsAppService.MsgPreparo
                    : "Olá {Nome}! 🍕 Seu pedido #{Numero} já está sendo preparado. Em breve ficará pronto!",
                true);
            pnl.Controls.Add(_txtMsgPreparo); y += 62;

            pnl.Controls.Add(MkLabel("🛵 Saiu p/ entrega:", y + 3));
            _txtMsgEntrega = MkTxt(y, 450,
                !string.IsNullOrWhiteSpace(WhatsAppService.MsgEntrega)
                    ? WhatsAppService.MsgEntrega
                    : "Olá {Nome}! 🛵 Seu pedido #{Numero} saiu para entrega. Aguarde em breve!",
                true);
            pnl.Controls.Add(_txtMsgEntrega); y += 62;

            pnl.Controls.Add(MkLabel("🎁 Cupom fidelidade:", y + 3));
            _txtMsgCupom = MkTxt(y, 450,
                !string.IsNullOrWhiteSpace(WhatsAppService.MsgCupom)
                    ? WhatsAppService.MsgCupom
                    : "Parabéns {Nome}! 🎉 Você ganhou um cupom: *{CupomCodigo}*\nVálido até {Validade}. Use no próximo pedido!",
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

        // ── Helpers de URL ────────────────────────────────────────────────────

        /// <summary>Retorna true quando a URL da API ainda usa localhost/127.0.0.1.</summary>
        private bool ApiUrlUsaLocalhost()
        {
            string url = _txtApiUrl?.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(url)) return false;
            try
            {
                var uri = new Uri(url);
                string host = uri.Host.ToLower();
                return host == "localhost" || host == "127.0.0.1" || host == "::1";
            }
            catch { return false; }
        }

        /// <summary>
        /// Extrai o host/IP do servidor MySQL da ConnectionString do App.config.
        /// Retorna null se for localhost ou não detectável.
        /// </summary>
        private static string ObterIpServidor()
        {
            try
            {
                string cs = ConfigurationManager.AppSettings["ConnectionString"] ?? "";
                if (string.IsNullOrEmpty(cs)) return null;
                var b = new MySqlConnector.MySqlConnectionStringBuilder(cs);
                string host = b.Server ?? "";
                if (string.IsNullOrEmpty(host)) return null;
                if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                    host == "127.0.0.1" || host == "::1")
                    return null;
                return host;
            }
            catch { return null; }
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

        // ── Tab Promoções ────────────────────────────────────────────────────

        private int _promCodigoAtual = 0;
        private DAL.PromocaoDAL _promDal;

        private void ConstruirTabPromocoes(TabPage tab)
        {
            tab.Padding = new Padding(8);

            // ── Painel form ──────────────────────────────────────────────────
            var pnlForm = new Panel { Dock = DockStyle.Top, Height = 112, BackColor = CorPainel, Padding = new Padding(8) };

            pnlForm.Controls.Add(MkLbl(pnlForm, "Nome:", 8, 4));
            var txtNome = new TextBox { Left = 8, Top = 22, Width = 160, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 9F) };
            pnlForm.Controls.Add(txtNome);

            pnlForm.Controls.Add(MkLbl(pnlForm, "Início:", 176, 4));
            var dtpInicio = new DateTimePicker { Left = 176, Top = 22, Width = 104, Format = DateTimePickerFormat.Short };
            pnlForm.Controls.Add(dtpInicio);

            pnlForm.Controls.Add(MkLbl(pnlForm, "Fim:", 288, 4));
            var dtpFim = new DateTimePicker { Left = 288, Top = 22, Width = 104, Value = DateTime.Today.AddDays(7), Format = DateTimePickerFormat.Short };
            pnlForm.Controls.Add(dtpFim);

            pnlForm.Controls.Add(MkLbl(pnlForm, "Tipo:", 400, 4));
            var cmbTipo = new ComboBox { Left = 400, Top = 22, Width = 100, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) };
            cmbTipo.Items.AddRange(new object[] { "PERCENTUAL", "FIXO" });
            cmbTipo.SelectedIndex = 0;
            pnlForm.Controls.Add(cmbTipo);

            pnlForm.Controls.Add(MkLbl(pnlForm, "Valor:", 508, 4));
            var numValor = new NumericUpDown { Left = 508, Top = 22, Width = 80, DecimalPlaces = 2, Maximum = 9999, Value = 10m, Font = new Font("Segoe UI", 9F), BorderStyle = BorderStyle.FixedSingle };
            pnlForm.Controls.Add(numValor);

            pnlForm.Controls.Add(MkLbl(pnlForm, "Qtd. Cupons:", 596, 4));
            var numLimiteCupons = new NumericUpDown { Left = 596, Top = 22, Width = 80, DecimalPlaces = 0, Minimum = 0, Maximum = 99999, Value = 0, Font = new Font("Segoe UI", 9F), BorderStyle = BorderStyle.FixedSingle };
            pnlForm.Controls.Add(numLimiteCupons);
            pnlForm.Controls.Add(MkLbl(pnlForm, "(0=ilimitado)", 596, 44));
            pnlForm.Height = 112;

            var btnNovaProm    = MkBtn(pnlForm, "➕ Nova",              8,   68, 80,  Color.FromArgb(52, 73, 94));
            var btnSalvarProm  = MkBtn(pnlForm, "💾 Salvar",            96,  68, 100, CorHeader);
            var btnExcluirProm = MkBtn(pnlForm, "🗑 Excluir",           204, 68, 90,  Color.FromArgb(150, 60, 40));
            var btnEnviarProm  = MkBtn(pnlForm, "📲 Enviar Clientes",   302, 68, 140, Color.FromArgb(39, 110, 50));
            pnlForm.Controls.AddRange(new Control[] { btnNovaProm, btnSalvarProm, btnExcluirProm, btnEnviarProm });

            // ── Layout horizontal: lista | itens ─────────────────────────────
            var pnlLista = new Panel { Dock = DockStyle.Left, Width = 320, Padding = new Padding(0, 4, 4, 0) };
            _gridPromocoes = CriarGrid();
            _gridPromocoes.Dock = DockStyle.Fill;
            pnlLista.Controls.Add(_gridPromocoes);

            var pnlItens = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4, 4, 0, 0) };
            var pnlBtnItens = new Panel { Dock = DockStyle.Top, Height = 36 };
            var btnAddProd = MkBtn(pnlBtnItens, "➕ Adicionar Produto", 0,   4, 170, CorHeader);
            var btnRemProd = MkBtn(pnlBtnItens, "➖ Remover",           178, 4, 110, Color.FromArgb(150, 60, 40));
            pnlBtnItens.Controls.AddRange(new Control[] { btnAddProd, btnRemProd });
            _gridPromItens = CriarGrid();
            _gridPromItens.Dock = DockStyle.Fill;
            _gridPromItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo",  Visible = false });
            _gridPromItens.Columns.Add(new DataGridViewTextBoxColumn { Name = "Produto", HeaderText = "Produtos desta Promoção", FillWeight = 100 });
            pnlItens.Controls.Add(_gridPromItens);
            pnlItens.Controls.Add(pnlBtnItens);

            tab.Controls.Add(pnlItens);
            tab.Controls.Add(pnlLista);
            tab.Controls.Add(pnlForm);

            // ── Helpers locais ───────────────────────────────────────────────
            void LimparForm() { txtNome.Text = ""; cmbTipo.SelectedIndex = 0; numValor.Value = 10m; numLimiteCupons.Value = 0; _promCodigoAtual = 0; _gridPromItens.Rows.Clear(); }

            void CarregarForm(DataGridViewRow v)
            {
                if (v == null) return;
                _promCodigoAtual = Convert.ToInt32(v.Cells["Codigo"].Value);
                txtNome.Text         = v.Cells["Nome"]?.Value?.ToString() ?? "";
                string tipo          = v.Cells["TipoDesc"]?.Value?.ToString() ?? "PERCENTUAL";
                cmbTipo.SelectedItem = tipo;
                if (decimal.TryParse(v.Cells["ValorDesc"]?.Value?.ToString(), out var vd)) numValor.Value = vd;
                if (DateTime.TryParse(v.Cells["Inicio"]?.Value?.ToString(), out var vi)) dtpInicio.Value = vi;
                if (DateTime.TryParse(v.Cells["Fim"]?.Value?.ToString(),    out var vf)) dtpFim.Value = vf;
                CarregarGridPromItens();
            }

            // ── Eventos ──────────────────────────────────────────────────────
            tab.Enter += (_, __) => { CarregarGridPromocoes(); LimparForm(); };

            btnNovaProm.Click += (_, __) => LimparForm();

            btnSalvarProm.Click += (_, __) =>
            {
                if (string.IsNullOrWhiteSpace(txtNome.Text)) { MessageBox.Show("Informe o nome."); return; }
                if (dtpFim.Value.Date < dtpInicio.Value.Date) { MessageBox.Show("Data fim menor que início."); return; }
                string tipo = cmbTipo.SelectedItem?.ToString() ?? "PERCENTUAL";
                if (_promCodigoAtual > 0)
                    _promDal.Atualizar(_promCodigoAtual, txtNome.Text.Trim(), dtpInicio.Value, dtpFim.Value, tipo, numValor.Value, (int)numLimiteCupons.Value);
                else
                    _promCodigoAtual = _promDal.Salvar(txtNome.Text.Trim(), dtpInicio.Value, dtpFim.Value, tipo, numValor.Value, (int)numLimiteCupons.Value);
                CarregarGridPromocoes();
                MessageBox.Show(_promCodigoAtual > 0 ? "Promoção salva! Agora adicione produtos." : "Atualizado.",
                    "Promoções", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnExcluirProm.Click += (_, __) =>
            {
                if (_promCodigoAtual <= 0 && _gridPromocoes.CurrentRow == null) return;
                int cod = _promCodigoAtual > 0 ? _promCodigoAtual : Convert.ToInt32(_gridPromocoes.CurrentRow.Cells["Codigo"].Value);
                if (MessageBox.Show("Excluir esta promoção e seus produtos?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                _promDal.Excluir(cod);
                LimparForm();
                CarregarGridPromocoes();
            };

            _gridPromocoes.SelectionChanged += (_, __) => CarregarForm(_gridPromocoes.CurrentRow);

            btnAddProd.Click += (_, __) =>
            {
                if (_promCodigoAtual <= 0) { MessageBox.Show("Salve a promoção primeiro."); return; }
                var (cod, nome) = AbrirSeletorProduto();
                if (cod <= 0) return;
                _promDal.AdicionarItem(_promCodigoAtual, cod, nome);
                CarregarGridPromItens();
            };

            btnRemProd.Click += (_, __) =>
            {
                if (_gridPromItens.CurrentRow == null) return;
                int itemCod = Convert.ToInt32(_gridPromItens.CurrentRow.Cells["Codigo"].Value);
                _promDal.RemoverItem(itemCod);
                CarregarGridPromItens();
            };

            btnEnviarProm.Click += async (_, __) =>
            {
                if (_promCodigoAtual <= 0) { MessageBox.Show("Selecione uma promoção."); return; }
                if (!WhatsAppService.Ativo) { MessageBox.Show("WhatsApp não está ativo."); return; }
                if (MessageBox.Show("Enviar promoção para todos os clientes com telefone?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                var row = _gridPromocoes.CurrentRow;
                if (row == null) return;
                string promNome  = row.Cells["Nome"]?.Value?.ToString() ?? "";
                string tipoD     = row.Cells["TipoDesc"]?.Value?.ToString() ?? "PERCENTUAL";
                decimal valDesc  = 0; decimal.TryParse(row.Cells["ValorDesc"]?.Value?.ToString(), out valDesc);
                string dataFimStr = "";
                try { dataFimStr = Convert.ToDateTime(row.Cells["Fim"].Value).ToString("dd/MM/yyyy"); } catch { }
                var itensComPreco = _promDal.ListarItens(_promCodigoAtual)
                    .ConvertAll(i => (i.nome, i.preco));

                // Gera um cupom compartilhado para esta promoção
                string cupomCodigo = "";
                try
                {
                    cupomCodigo = $"PROM{DateTime.Now:yyyyMMddHHmm}";
                    DateTime dataFimCupom = DateTime.Today;
                    try { dataFimCupom = Convert.ToDateTime(row.Cells["Fim"].Value); } catch { }
                    var cupom = new Modelo.Cupom
                    {
                        cupomCodigo      = cupomCodigo,
                        cupomDescricao   = $"Promoção: {promNome}",
                        cupomTipo        = tipoD,
                        cupomValor       = valDesc,
                        cupomPedido_Minimo = 0,
                        cupomLimite_Usos = (int)numLimiteCupons.Value,
                        cupomValido_Ate  = dataFimCupom,
                        Situacao         = "A",
                        Status_Transmissao = "N",
                        Info             = "",
                    };
                    string erroCupom = new DAL.CupomDAL().Incluir(cupom);
                    if (!string.IsNullOrWhiteSpace(erroCupom)) cupomCodigo = ""; // não bloqueia o envio
                }
                catch { cupomCodigo = ""; }

                var clientes = WhatsAppService.ListarClientesComFone();
                btnEnviarProm.Enabled = false;
                int enviado = 0;
                var falharam = new System.Collections.Generic.List<string>();
                await System.Threading.Tasks.Task.Run(async () =>
                {
                    foreach (var (fone, nome) in clientes)
                    {
                        bool ok = await WhatsAppService.NotificarPromocao(fone, nome, promNome, dataFimStr, tipoD, valDesc, itensComPreco, cupomCodigo).ConfigureAwait(false);
                        if (ok) enviado++; else falharam.Add($"{nome} ({fone})");
                        System.Threading.Thread.Sleep(2000);
                        if (IsDisposed) return;
                        Invoke(new Action(() => btnEnviarProm.Text = $"Enviando {enviado+falharam.Count}/{clientes.Count}..."));
                    }
                }).ConfigureAwait(false);
                if (!IsDisposed) Invoke(new Action(() =>
                {
                    btnEnviarProm.Enabled = true;
                    btnEnviarProm.Text = "📲 Enviar Clientes";
                    string msg = $"Promoção enviada para {enviado} cliente(s) com sucesso.";
                    if (falharam.Count > 0)
                        msg += $"\n\nNão entregues ({falharam.Count}):\n" + string.Join("\n", falharam);
                    MessageBox.Show(msg, "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            };
        }

        private void CarregarGridPromocoes()
        {
            try
            {
                var dt = _promDal.Listar();
                _gridPromocoes.DataSource = dt;
                if (_gridPromocoes.Columns.Count > 0)
                {
                    if (_gridPromocoes.Columns["Codigo"]    != null) _gridPromocoes.Columns["Codigo"].Visible = false;
                    if (_gridPromocoes.Columns["Nome"]      != null) { _gridPromocoes.Columns["Nome"].HeaderText = "Promoção"; _gridPromocoes.Columns["Nome"].FillWeight = 38; }
                    if (_gridPromocoes.Columns["Inicio"]    != null) { _gridPromocoes.Columns["Inicio"].HeaderText = "Início"; _gridPromocoes.Columns["Inicio"].FillWeight = 17; }
                    if (_gridPromocoes.Columns["Fim"]       != null) { _gridPromocoes.Columns["Fim"].HeaderText = "Fim"; _gridPromocoes.Columns["Fim"].FillWeight = 17; }
                    if (_gridPromocoes.Columns["TipoDesc"]     != null) _gridPromocoes.Columns["TipoDesc"].Visible = false;
                    if (_gridPromocoes.Columns["ValorDesc"]    != null) { _gridPromocoes.Columns["ValorDesc"].HeaderText = "Desc."; _gridPromocoes.Columns["ValorDesc"].FillWeight = 14; }
                    if (_gridPromocoes.Columns["Ativo"]        != null) _gridPromocoes.Columns["Ativo"].Visible = false;
                    if (_gridPromocoes.Columns["LimiteCupons"] != null) _gridPromocoes.Columns["LimiteCupons"].Visible = false;
                    if (_gridPromocoes.Columns["Produtos"]     != null) { _gridPromocoes.Columns["Produtos"].HeaderText = "Produtos"; _gridPromocoes.Columns["Produtos"].FillWeight = 14; }
                }
            }
            catch { }
        }

        private void CarregarGridPromItens()
        {
            _gridPromItens.Rows.Clear();
            if (_promCodigoAtual <= 0) return;
            try
            {
                foreach (var (cod, nome, preco) in _promDal.ListarItens(_promCodigoAtual))
                    _gridPromItens.Rows.Add(cod, nome);
            }
            catch { }
        }

        // ── Tab Cardápio do Dia ──────────────────────────────────────────────

        private DataGridView _gridCardapioLista;   // lista de cardápios salvos (esquerda)
        private DataGridView _gridCardapio;        // itens do cardápio selecionado (direita)
        private int          _cardCodigoAtual = 0;
        private readonly DAL.CardapioDiaDAL _cardDal = new DAL.CardapioDiaDAL();

        private void ConstruirTabCardapio(TabPage tab)
        {
            tab.Padding = new Padding(8);

            // ── Form de cabeçalho (topo) ─────────────────────────────────────
            var pnlTop2 = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = CorPainel, Padding = new Padding(8) };

            pnlTop2.Controls.Add(MkLbl(pnlTop2, "Data:", 8, 4));
            var dtpCard = new DateTimePicker { Left = 8, Top = 22, Width = 112, Format = DateTimePickerFormat.Short };
            pnlTop2.Controls.Add(dtpCard);

            pnlTop2.Controls.Add(MkLbl(pnlTop2, "Título / Destaque:", 130, 4));
            var txtTitulo = new TextBox { Left = 130, Top = 22, Width = 230, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 9F), PlaceholderText = "Ex: Cardápio de Segunda..." };
            pnlTop2.Controls.Add(txtTitulo);

            pnlTop2.Controls.Add(MkLbl(pnlTop2, "Observação:", 8, 52));
            var txtObs = new TextBox { Left = 8, Top = 70, Width = 352, Height = 34, Multiline = true, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 9F) };
            pnlTop2.Controls.Add(txtObs);

            var btnNovoCard   = MkBtn(pnlTop2, "➕ Novo",            370, 4,   80,  Color.FromArgb(52, 73, 94));
            var btnSalvarCard = MkBtn(pnlTop2, "💾 Salvar",          370, 36,  80,  CorHeader);
            var btnExcluirCard= MkBtn(pnlTop2, "🗑 Excluir",         370, 68,  80,  Color.FromArgb(150, 60, 40));
            var btnEnviarCard = MkBtn(pnlTop2, "📲 Enviar Clientes", 458, 4,   128, Color.FromArgb(39, 110, 50));
            btnEnviarCard.Height = 100;
            pnlTop2.Controls.AddRange(new Control[] { btnNovoCard, btnSalvarCard, btnExcluirCard, btnEnviarCard });

            // ── Layout horizontal: lista de cardápios (esq) | itens (dir) ────
            var pnlLista = new Panel { Dock = DockStyle.Left, Width = 210, Padding = new Padding(0, 4, 4, 0) };
            _gridCardapioLista = CriarGrid();
            _gridCardapioLista.Dock = DockStyle.Fill;
            _gridCardapioLista.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo", Visible = false });
            _gridCardapioLista.Columns.Add(new DataGridViewTextBoxColumn { Name = "Data",   HeaderText = "Data",   FillWeight = 45 });
            _gridCardapioLista.Columns.Add(new DataGridViewTextBoxColumn { Name = "Titulo", HeaderText = "Título", FillWeight = 55 });
            pnlLista.Controls.Add(_gridCardapioLista);

            var pnlItens = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4, 4, 0, 0) };
            var pnlBtnCard = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = CorFundo };
            var btnAddItem = MkBtn(pnlBtnCard, "➕ Adicionar Produto do Catálogo", 0, 4, 220, CorHeader);
            var btnRemItem = MkBtn(pnlBtnCard, "➖ Remover Item", 228, 4, 130, Color.FromArgb(150, 60, 40));
            pnlBtnCard.Controls.AddRange(new Control[] { btnAddItem, btnRemItem });

            _gridCardapio = CriarGrid();
            _gridCardapio.ReadOnly = false;   // permite editar descrição diretamente
            _gridCardapio.Dock = DockStyle.Fill;
            _gridCardapio.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo",    Visible = false });
            _gridCardapio.Columns.Add(new DataGridViewTextBoxColumn { Name = "CodMerc",   Visible = false });
            _gridCardapio.Columns.Add(new DataGridViewTextBoxColumn { Name = "Produto",   HeaderText = "Produto",   FillWeight = 45, ReadOnly = true });
            _gridCardapio.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descricao", HeaderText = "Descrição", FillWeight = 55 });

            pnlItens.Controls.Add(_gridCardapio);
            pnlItens.Controls.Add(pnlBtnCard);

            tab.Controls.Add(pnlItens);
            tab.Controls.Add(pnlLista);
            tab.Controls.Add(pnlTop2);

            // ── Helpers locais ───────────────────────────────────────────────
            void LimparCardForm() { dtpCard.Value = DateTime.Today; txtTitulo.Text = ""; txtObs.Text = ""; _cardCodigoAtual = 0; _gridCardapio.Rows.Clear(); }

            void CarregarItensGrid()
            {
                _gridCardapio.Rows.Clear();
                if (_cardCodigoAtual <= 0) return;
                try
                {
                    foreach (var (cod, codM, nome, desc) in _cardDal.ListarItens(_cardCodigoAtual))
                        _gridCardapio.Rows.Add(cod, codM, nome, desc);
                }
                catch { }
            }

            // ── Eventos ──────────────────────────────────────────────────────
            tab.Enter += (_, __) => { CarregarListaCardapios(); LimparCardForm(); };

            btnNovoCard.Click += (_, __) => LimparCardForm();

            btnSalvarCard.Click += (_, __) =>
            {
                _cardCodigoAtual = _cardDal.SalvarCabecalho(_cardCodigoAtual, dtpCard.Value, txtTitulo.Text.Trim(), txtObs.Text.Trim());
                // Persiste itens da grid
                _cardDal.LimparItens(_cardCodigoAtual);
                foreach (DataGridViewRow r in _gridCardapio.Rows)
                {
                    if (r.IsNewRow) continue;
                    string nome = r.Cells["Produto"].Value?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(nome)) continue;
                    int codM = 0; int.TryParse(r.Cells["CodMerc"].Value?.ToString(), out codM);
                    _cardDal.AdicionarItem(_cardCodigoAtual, codM, nome, r.Cells["Descricao"].Value?.ToString() ?? "");
                }
                CarregarListaCardapios();
                MessageBox.Show("Cardápio salvo!", "Cardápio do Dia", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnExcluirCard.Click += (_, __) =>
            {
                if (_cardCodigoAtual <= 0) return;
                if (MessageBox.Show("Excluir este cardápio?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                _cardDal.Excluir(_cardCodigoAtual);
                LimparCardForm();
                CarregarListaCardapios();
            };

            _gridCardapioLista.SelectionChanged += (_, __) =>
            {
                if (_gridCardapioLista.CurrentRow == null) return;
                var row = _gridCardapioLista.CurrentRow;
                _cardCodigoAtual = Convert.ToInt32(row.Cells["Codigo"].Value);
                var (_, tit, obs) = _cardDal.BuscarPorCodigo(_cardCodigoAtual);
                if (DateTime.TryParse(row.Cells["Data"].Value?.ToString(), out var d)) dtpCard.Value = d;
                txtTitulo.Text = tit; txtObs.Text = obs;
                CarregarItensGrid();
            };

            btnAddItem.Click += (_, __) =>
            {
                var (cod, nome) = AbrirSeletorProduto();
                if (cod <= 0) return;
                _gridCardapio.Rows.Add(0, cod, nome, "");
            };

            btnRemItem.Click += (_, __) =>
            {
                if (_gridCardapio.CurrentRow == null || _gridCardapio.CurrentRow.IsNewRow) return;
                if (int.TryParse(_gridCardapio.CurrentRow.Cells["Codigo"].Value?.ToString(), out int itemCod) && itemCod > 0)
                    _cardDal.RemoverItem(itemCod);
                _gridCardapio.Rows.Remove(_gridCardapio.CurrentRow);
            };

            btnEnviarCard.Click += async (_, __) =>
            {
                if (!WhatsAppService.Ativo) { MessageBox.Show("WhatsApp não está ativo."); return; }
                if (_gridCardapio.Rows.Count == 0) { MessageBox.Show("Adicione itens ao cardápio."); return; }
                if (MessageBox.Show("Enviar cardápio para todos os clientes com telefone?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                var itens = new System.Collections.Generic.List<(string n, string d)>();
                foreach (DataGridViewRow r in _gridCardapio.Rows)
                {
                    if (r.IsNewRow) continue;
                    itens.Add((r.Cells["Produto"].Value?.ToString() ?? "", r.Cells["Descricao"].Value?.ToString() ?? ""));
                }

                var clientes = WhatsAppService.ListarClientesComFone();
                btnEnviarCard.Enabled = false;
                int enviado = 0;
                var falharam = new System.Collections.Generic.List<string>();
                string titulo = txtTitulo.Text.Trim();
                string obs    = txtObs.Text.Trim();
                string dataStr= DateTime.Today.ToString("dd/MM/yyyy");
                await System.Threading.Tasks.Task.Run(async () =>
                {
                    foreach (var (fone, nome) in clientes)
                    {
                        bool ok = await WhatsAppService.NotificarCardapio(fone, nome, titulo, dataStr, itens, obs).ConfigureAwait(false);
                        if (ok) enviado++; else falharam.Add($"{nome} ({fone})");
                        System.Threading.Thread.Sleep(2000);
                        if (IsDisposed) return;
                        Invoke(new Action(() => btnEnviarCard.Text = $"Enviando {enviado+falharam.Count}/{clientes.Count}..."));
                    }
                }).ConfigureAwait(false);
                if (!IsDisposed) Invoke(new Action(() =>
                {
                    btnEnviarCard.Enabled = true;
                    btnEnviarCard.Text = "📲 Enviar Clientes";
                    string msg = $"Cardápio enviado para {enviado} cliente(s) com sucesso.";
                    if (falharam.Count > 0)
                        msg += $"\n\nNão entregues ({falharam.Count}):\n" + string.Join("\n", falharam);
                    MessageBox.Show(msg, "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            };
        }

        private void CarregarListaCardapios()
        {
            _gridCardapioLista.Rows.Clear();
            try
            {
                var dt = _cardDal.Listar();
                foreach (System.Data.DataRow r in dt.Rows)
                {
                    string dataStr = r["Data"] == System.DBNull.Value ? "" : Convert.ToDateTime(r["Data"]).ToString("dd/MM/yy");
                    _gridCardapioLista.Rows.Add(r["Codigo"], dataStr, r["Titulo"]);
                }
            }
            catch { }
        }

        private void CarregarGridCardapio()
        {
            _gridCardapio.Rows.Clear();
            if (_cardCodigoAtual <= 0) return;
            try
            {
                foreach (var (cod, codM, nome, desc) in _cardDal.ListarItens(_cardCodigoAtual))
                    _gridCardapio.Rows.Add(cod, codM, nome, desc);
            }
            catch { }
        }

        // ── Helpers de UI ────────────────────────────────────────────────────

        private static Label MkLbl(Control parent, string texto, int x, int y)
        {
            var lbl = new Label { Text = texto, Left = x, Top = y, AutoSize = true,
                ForeColor = Color.FromArgb(60, 50, 30), Font = new Font("Segoe UI", 8.5F) };
            return lbl;
        }

        private static Button MkBtn(Control parent, string texto, int x, int y, int w, Color back)
        {
            var btn = new Button
            {
                Text = texto, Left = x, Top = y, Width = w, Height = 28,
                BackColor = back, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand, Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private static DataGridView CriarGrid()
        {
            var g = new DataGridView
            {
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(245, 237, 216),
                GridColor = Color.FromArgb(200, 185, 160),
                BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 9F),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 237, 216), ForeColor = Color.FromArgb(50, 50, 50),
                    SelectionBackColor = Color.FromArgb(224, 113, 42), SelectionForeColor = Color.White
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(176, 110, 42), ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                },
                MultiSelect = false
            };
            return g;
        }

        /// <summary>Abre diálogo de seleção de produto do cardápio. Retorna (cod, nome) ou (0, "") se cancelado.</summary>
        private (int cod, string nome) AbrirSeletorProduto()
        {
            using var dlg = new Form
            {
                Text = "Selecionar Produto", StartPosition = FormStartPosition.CenterParent,
                Size = new Size(520, 430), FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false, MinimizeBox = false, BackColor = Color.FromArgb(245, 237, 216)
            };

            // Filtro de categoria
            var pnlCat = new Panel { Dock = DockStyle.Top, Height = 30, BackColor = Color.FromArgb(235, 226, 208), Padding = new Padding(4, 3, 4, 3) };
            var lblCat = new Label { Text = "Categoria:", Left = 4, Top = 7, AutoSize = true, ForeColor = Color.FromArgb(60, 50, 30), Font = new Font("Segoe UI", 8.5F) };
            var cmbCat = new ComboBox { Left = 72, Top = 4, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F), BackColor = Color.White };
            cmbCat.Items.Add("Todas");
            pnlCat.Controls.Add(lblCat);
            pnlCat.Controls.Add(cmbCat);

            var txtF = new TextBox { Dock = DockStyle.Top, Height = 28, PlaceholderText = "Filtrar...", BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 9F) };
            var grd  = CriarGrid();
            grd.Dock = DockStyle.Fill;
            grd.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo", Visible = false });
            grd.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome", HeaderText = "Produto", FillWeight = 70 });
            grd.Columns.Add(new DataGridViewTextBoxColumn { Name = "Preco", HeaderText = "Preço R$", FillWeight = 30 });

            System.Data.DataTable dtProd = null;
            try { dtProd = new BLL.MercadoriaBLL().Listar(); } catch { }

            // Carrega categorias distintas no ComboBox
            if (dtProd != null)
            {
                var cats = new System.Collections.Generic.HashSet<string>();
                foreach (System.Data.DataRow r in dtProd.Rows)
                {
                    string cat = r["Categoria"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(cat)) cats.Add(cat);
                }
                foreach (var c in cats) cmbCat.Items.Add(c);
            }
            cmbCat.SelectedIndex = 0;

            void Preencher(string filtro)
            {
                grd.Rows.Clear();
                if (dtProd == null) return;
                string catSel = cmbCat.SelectedItem?.ToString() ?? "Todas";
                foreach (System.Data.DataRow r in dtProd.Rows)
                {
                    string nomeP = r["Nome"]?.ToString() ?? "";
                    string catP  = r["Categoria"]?.ToString() ?? "";
                    if (catSel != "Todas" && catP != catSel) continue;
                    if (!string.IsNullOrWhiteSpace(filtro) && nomeP.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    string precoStr = r["Preco"] == System.DBNull.Value ? "" : Convert.ToDecimal(r["Preco"]).ToString("N2");
                    grd.Rows.Add(r["Codigo"], nomeP, precoStr);
                }
            }
            Preencher("");
            txtF.TextChanged += (_, __) => Preencher(txtF.Text.Trim());
            cmbCat.SelectedIndexChanged += (_, __) => Preencher(txtF.Text.Trim());

            (int c, string n) resultado = (0, "");
            void Selecionar() { if (grd.CurrentRow == null) return; resultado = (Convert.ToInt32(grd.CurrentRow.Cells["Codigo"].Value), grd.CurrentRow.Cells["Nome"].Value?.ToString() ?? ""); dlg.DialogResult = DialogResult.OK; }
            grd.CellDoubleClick += (_, __) => Selecionar();

            var btnOk = new Button { Text = "Selecionar", Dock = DockStyle.Bottom, Height = 32, BackColor = CorHeader, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (_, __) => Selecionar();
            dlg.Controls.Add(grd); dlg.Controls.Add(btnOk); dlg.Controls.Add(pnlCat); dlg.Controls.Add(txtF);
            dlg.ShowDialog(this);
            return resultado;
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
