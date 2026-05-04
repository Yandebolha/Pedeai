using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Forms;
using Pedeai.Modelo;

namespace Pedeai
{
    public partial class Form1 : Form
    {
        // -- BLL -------------------------------------------------------------
        private PedidoBLL        _pedidoBLL;
        private DashboardBLL     _dashBLL;
        private GastoMaterialBLL _gastosBLL;
        private BLL.EntradaMercadoriaBLL         _entradaBLL;
        private BLL.EmpresaBLL                   _empBLL;
        private BLL.ConfiguracaoImpressaoBLL     _impBLL;

        private int  _paginaAtual = 0; // 0=Dashboard 1=Pedidos 2=Financeiro
        private System.Threading.Timer _syncCatalogoTimer;

        // -- Cores ------------------------------------------------------------
        private static readonly Color CorSidebar    = Color.FromArgb(18, 20, 25);
        private static readonly Color CorTopBar     = Color.White;
        private static readonly Color CorBotaoAtivo = Color.FromArgb(176, 110, 42);
        private static readonly Color CorCard       = Color.FromArgb(165, 105, 42);
        private static readonly Color CorFundo      = Color.FromArgb(248, 245, 240);

        public Form1()
        {
            InitializeComponent();
            _pedidoBLL   = new PedidoBLL();
            _dashBLL     = new DashboardBLL();
            _gastosBLL   = new GastoMaterialBLL();
            _entradaBLL  = new BLL.EntradaMercadoriaBLL();
            _empBLL      = new BLL.EmpresaBLL();
            _impBLL      = new BLL.ConfiguracaoImpressaoBLL();
            BuildDashboard();
            BuildPedidos();
            BuildFinanceiro();
            AppEvents.NovoPedidoWebRecebido += OnNovoPedidoWebRecebido;
            if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime)
                Load += Form1_Load;
        }

        private void OnNovoPedidoWebRecebido(int count)
        {
            // Refresh grid, then play alert sound on a background thread (non-blocking)
            BeginInvoke(new Action(() =>
            {
                try { CarregarPedidosSemPerderSelecao(); } catch { }
            }));
            // Toca o som uma única vez para notificar novo pedido web
            try { System.Media.SystemSounds.Exclamation.Play(); } catch { }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            AppEvents.NovoPedidoWebRecebido -= OnNovoPedidoWebRecebido;
            try { _syncCatalogoTimer?.Dispose(); } catch { }
            try { DB.QuartzSchedulerService.StopAsync().GetAwaiter().GetResult(); } catch { }
            base.OnFormClosed(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReconstruirSidebar();
            CarregarTudo();
            NavIniciarPrimeiro();
            _timer.Start();
            Logger.Log("Form1", "Form1_Load", $"Login | Usu\u00e1rio: {UsuarioSessao.NomeAtual}");
            BeginInvoke(new Action(VerificarAvisosIniciais));

            // Periodic catalog sync — runs every 5 minutes as catch-all for MySQL→Supabase
            _syncCatalogoTimer = new System.Threading.Timer(
                async _ => { try { await DB.SupabaseService.SincronizarCatalogoAsync(); } catch { } },
                null,
                System.TimeSpan.FromMinutes(3),
                System.TimeSpan.FromMinutes(3));

            // Start web order polling + initial sync
            System.Threading.Tasks.Task.Run(async () =>
            {
                await DB.QuartzSchedulerService.StartAsync();
                await DB.SupabaseService.SincronizarTudoAsync();
            });
        }

        private void VerificarAvisosIniciais()
        {
            try
            {
                int count = _entradaBLL.ContarParcelasVencendo(1);
                if (count > 0)
                    using (var frm = new Forms.frmAvisos())
                        frm.ShowDialog(this);
            }
            catch { }
        }

        private Button BotaoNav(string texto, int y, Action onClick)
        {
            var btn = new Button
            {
                Text      = texto,
                Left      = 0,
                Top       = y,
                Width     = 210,
                Height    = 42,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(195, 185, 165),
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(16, 0, 0, 0),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize         = 0;
            btn.FlatAppearance.MouseOverBackColor  = Color.FromArgb(35, 38, 48);
            btn.FlatAppearance.MouseDownBackColor  = Color.FromArgb(176, 110, 42);
            btn.Click += (_, __) => onClick();
            return btn;
        }

        // Chart panels and data
        private Panel           _pnlChartCanal;
        private Panel           _pnlChartProdutos;
        private Panel           _pnlChartDias;
        private string          _periodoCanal  = "dia";
        private string          _periodoProd   = "dia";
        private string          _periodoDias   = "dia";
        private Button[]        _btnsPeriodo;
        private Button[]        _btnsProdPeriodo;
        private Button[]        _btnsDiasPeriodo;
        private DateTimePicker  _dtpIniCanal, _dtpFimCanal;
        private DateTimePicker  _dtpIniProd,  _dtpFimProd;
        private DateTimePicker  _dtpIniDias,  _dtpFimDias;
        private (string label, float value)[] _dadosCanal    = Array.Empty<(string, float)>();
        private (string label, float value)[] _dadosProdutos = Array.Empty<(string, float)>();
        private (string label, float value)[] _dadosDias     = Array.Empty<(string, float)>();

        // Botoes de acao da tela de pedidos
        private Button _btnConfirmar;
        private Button _btnEmPreparo;
        private Button _btnPronto;
        private Button _btnSaiu;
        private Button _btnEntregue;
        private Button _btnCancelar;
        private Button _btnReimprimir;

        // --------------------------------------------------------------------
        // DASHBOARD
        // --------------------------------------------------------------------
        private void BuildDashboard()
        {
            pnlDashboard = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            var pnlCards = new FlowLayoutPanel
            {
                Dock          = DockStyle.Top,
                Height        = 140,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = false,
                BackColor     = Color.Transparent,
                Padding       = new Padding(0, 0, 0, 16)
            };

            lblPedidosHoje  = CriarCard(pnlCards, "Pedidos Hoje",     "0",       Color.FromArgb(176, 110, 42), () => MostrarPedidos());
            lblFaturamento  = CriarCard(pnlCards, "Faturamento Hoje", "R$ 0,00", Color.FromArgb(115, 140, 50), () => MostrarFinanceiro());
            lblClientes     = CriarCard(pnlCards, "Total Clientes",   "0",       Color.FromArgb(155, 95, 40), () =>
            {
                using (var frm = new Forms.frmCadastroCliente())
                    frm.ShowDialog(this);
            });
            lblPendentes    = CriarCard(pnlCards, "Pedidos Pendentes","0",       Color.FromArgb(190, 100, 30), () =>
            {
                MostrarPedidos();
                if (cmbFiltroPedido != null)
                {
                    for (int i = 0; i < cmbFiltroPedido.Items.Count; i++)
                        if (cmbFiltroPedido.Items[i].ToString() == "Pendentes") { cmbFiltroPedido.SelectedIndex = i; break; }
                    CarregarPedidos();
                }
            });

            // ── Barra de filtro de período ─────────────────────────────────────
            // ── Área de gráficos ──
            Panel  MkOuter() => new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(248, 245, 240) };
            Panel  MkFiltro() => new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.FromArgb(235, 228, 213) };
            Label  MkLbl(string t, int x) => new Label { Text = t, ForeColor = Color.FromArgb(180, 200, 240), Left = x, Top = 9, AutoSize = true };
            DateTimePicker MkDtp(int x, DateTime v) => new DateTimePicker { Left = x, Top = 6, Width = 105, Format = DateTimePickerFormat.Short, Value = v };
            Button MkBtn(int x, Action fn)
            {
                var b = new Button { Text = "Filtrar", Left = x, Top = 5, Width = 62, Height = 24, BackColor = CorBotaoAtivo, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
                b.FlatAppearance.BorderSize = 0; b.Click += (_, __) => fn(); return b;
            }

            // ── Área de gráficos (3 painéis empilhados verticalmente) ──────────
            var tbl = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 1,
                RowCount    = 3,
                BackColor   = Color.Transparent,
                Padding     = new Padding(0, 4, 0, 0)
            };
            tbl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            tbl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.3f));
            tbl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.4f));
            tbl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.3f));

            // Gráfico 1: Vendas por Canal  (Diário / Semanal / Mensal / Anual)
            var outerC = MkOuter();
            var filtC  = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.FromArgb(235, 228, 213) };
            var periodos = new[] { ("Diário", "dia"), ("Semanal", "semana"), ("Mensal", "mes"), ("Anual", "ano") };
            _btnsPeriodo = new Button[4];
            for (int pi = 0; pi < periodos.Length; pi++)
            {
                int idx = pi; string per = periodos[pi].Item2;
                var bp = new Button
                {
                    Text      = periodos[pi].Item1,
                    Left      = 4 + idx * 76,
                    Top       = 5,
                    Width     = 70,
                    Height    = 24,
                    BackColor = idx == 0 ? CorBotaoAtivo : Color.FromArgb(200, 192, 170),
                    ForeColor = idx == 0 ? Color.White : Color.FromArgb(70, 60, 45),
                    FlatStyle = FlatStyle.Flat,
                    Cursor    = Cursors.Hand,
                    Font      = new Font("Segoe UI", 8f)
                };
                bp.FlatAppearance.BorderSize = 0;
                bp.Click += (_, __) =>
                {
                    _periodoCanal = per;
                    foreach (var b in _btnsPeriodo) { b.BackColor = Color.FromArgb(200, 192, 170); b.ForeColor = Color.FromArgb(70, 60, 45); }
                    bp.BackColor = CorBotaoAtivo; bp.ForeColor = Color.White;
                    CarregarChartCanal();
                };
                _btnsPeriodo[pi] = bp;
                filtC.Controls.Add(bp);
            }
            filtC.Height = 34;
            _dtpIniCanal = new DateTimePicker { Left = 344, Top = 5, Width = 90, Height = 24, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-30) };
            _dtpFimCanal = new DateTimePicker { Left = 472, Top = 5, Width = 90, Height = 24, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            filtC.Controls.Add(new Label { Text = "De:",  ForeColor = Color.FromArgb(80, 60, 40), Font = new Font("Segoe UI", 8f), Left = 308, Top = 9, AutoSize = true });
            filtC.Controls.Add(_dtpIniCanal);
            filtC.Controls.Add(new Label { Text = "Até:", ForeColor = Color.FromArgb(80, 60, 40), Font = new Font("Segoe UI", 8f), Left = 440, Top = 9, AutoSize = true });
            filtC.Controls.Add(_dtpFimCanal);
            var btnFiltrarC = new Button { Text = "Filtrar", Left = 568, Top = 5, Width = 62, Height = 24, BackColor = CorBotaoAtivo, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFiltrarC.FlatAppearance.BorderSize = 0;
            btnFiltrarC.Click += (_, __) =>
            {
                _periodoCanal = "custom";
                foreach (var b in _btnsPeriodo) { b.BackColor = Color.FromArgb(200, 192, 170); b.ForeColor = Color.FromArgb(70, 60, 45); }
                CarregarChartCanal();
            };
            filtC.Controls.Add(btnFiltrarC);
            _pnlChartCanal = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(248, 245, 240), Tag = "Vendas por Canal" };
            _pnlChartCanal.Paint += (s, e) => DesenharBarrasVerticais(e.Graphics, (Panel)s, _dadosCanal, Color.FromArgb(176, 110, 42));
            outerC.Controls.Add(_pnlChartCanal); outerC.Controls.Add(filtC);
            outerC.Controls.Add(new Panel { Height = 4, Dock = DockStyle.Top, BackColor = Color.FromArgb(176, 110, 42) });

            // Gráfico 2: Top 3 Produtos (Diário / Semanal / Mensal / Anual)
            var outerP = MkOuter();
            var filtP  = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.FromArgb(235, 228, 213) };
            var periodosProd = new[] { ("Diário", "dia"), ("Semanal", "semana"), ("Mensal", "mes"), ("Anual", "ano") };
            _btnsProdPeriodo = new Button[4];
            for (int pi = 0; pi < periodosProd.Length; pi++)
            {
                int idx = pi; string per = periodosProd[pi].Item2;
                var bp = new Button
                {
                    Text      = periodosProd[pi].Item1,
                    Left      = 4 + idx * 76,
                    Top       = 5,
                    Width     = 70,
                    Height    = 24,
                    BackColor = idx == 0 ? CorBotaoAtivo : Color.FromArgb(200, 192, 170),
                    ForeColor = idx == 0 ? Color.White : Color.FromArgb(70, 60, 45),
                    FlatStyle = FlatStyle.Flat,
                    Cursor    = Cursors.Hand,
                    Font      = new Font("Segoe UI", 8f)
                };
                bp.FlatAppearance.BorderSize = 0;
                bp.Click += (_, __) =>
                {
                    _periodoProd = per;
                    foreach (var b in _btnsProdPeriodo) { b.BackColor = Color.FromArgb(200, 192, 170); b.ForeColor = Color.FromArgb(70, 60, 45); }
                    bp.BackColor = CorBotaoAtivo; bp.ForeColor = Color.White;
                    CarregarChartProdutos();
                };
                _btnsProdPeriodo[pi] = bp;
                filtP.Controls.Add(bp);
            }
            filtP.Height = 34;
            _dtpIniProd = new DateTimePicker { Left = 344, Top = 5, Width = 90, Height = 24, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-30) };
            _dtpFimProd = new DateTimePicker { Left = 472, Top = 5, Width = 90, Height = 24, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            filtP.Controls.Add(new Label { Text = "De:",  ForeColor = Color.FromArgb(80, 60, 40), Font = new Font("Segoe UI", 8f), Left = 308, Top = 9, AutoSize = true });
            filtP.Controls.Add(_dtpIniProd);
            filtP.Controls.Add(new Label { Text = "Até:", ForeColor = Color.FromArgb(80, 60, 40), Font = new Font("Segoe UI", 8f), Left = 440, Top = 9, AutoSize = true });
            filtP.Controls.Add(_dtpFimProd);
            var btnFiltrarP = new Button { Text = "Filtrar", Left = 568, Top = 5, Width = 62, Height = 24, BackColor = CorBotaoAtivo, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFiltrarP.FlatAppearance.BorderSize = 0;
            btnFiltrarP.Click += (_, __) =>
            {
                _periodoProd = "custom";
                foreach (var b in _btnsProdPeriodo) { b.BackColor = Color.FromArgb(200, 192, 170); b.ForeColor = Color.FromArgb(70, 60, 45); }
                CarregarChartProdutos();
            };
            filtP.Controls.Add(btnFiltrarP);
            _pnlChartProdutos = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(248, 245, 240), Tag = "Top 5 Produtos" };
            _pnlChartProdutos.Paint += (s, e) => DesenharBarrasVerticais(e.Graphics, (Panel)s, _dadosProdutos, Color.FromArgb(160, 100, 38), "0");
            outerP.Controls.Add(_pnlChartProdutos); outerP.Controls.Add(filtP);
            outerP.Controls.Add(new Panel { Height = 4, Dock = DockStyle.Top, BackColor = Color.FromArgb(155, 95, 38) });

            // Gráfico 3: Receita por Dia (Diário / Semanal / Mensal / Anual)
            var outerD = MkOuter();
            var filtD  = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.FromArgb(235, 228, 213) };
            var periodosDias = new[] { ("Diário", "dia"), ("Semanal", "semana"), ("Mensal", "mes"), ("Anual", "ano") };
            _btnsDiasPeriodo = new Button[4];
            for (int pi = 0; pi < periodosDias.Length; pi++)
            {
                int idx = pi; string per = periodosDias[pi].Item2;
                var bp = new Button
                {
                    Text      = periodosDias[pi].Item1,
                    Left      = 4 + idx * 76,
                    Top       = 5,
                    Width     = 70,
                    Height    = 24,
                    BackColor = idx == 0 ? CorBotaoAtivo : Color.FromArgb(200, 192, 170),
                    ForeColor = idx == 0 ? Color.White : Color.FromArgb(70, 60, 45),
                    FlatStyle = FlatStyle.Flat,
                    Cursor    = Cursors.Hand,
                    Font      = new Font("Segoe UI", 8f)
                };
                bp.FlatAppearance.BorderSize = 0;
                bp.Click += (_, __) =>
                {
                    _periodoDias = per;
                    foreach (var b in _btnsDiasPeriodo) { b.BackColor = Color.FromArgb(200, 192, 170); b.ForeColor = Color.FromArgb(70, 60, 45); }
                    bp.BackColor = CorBotaoAtivo; bp.ForeColor = Color.White;
                    CarregarChartDias();
                };
                _btnsDiasPeriodo[pi] = bp;
                filtD.Controls.Add(bp);
            }
            filtD.Height = 34;
            _dtpIniDias = new DateTimePicker { Left = 344, Top = 5, Width = 90, Height = 24, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-30) };
            _dtpFimDias = new DateTimePicker { Left = 472, Top = 5, Width = 90, Height = 24, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            filtD.Controls.Add(new Label { Text = "De:",  ForeColor = Color.FromArgb(80, 60, 40), Font = new Font("Segoe UI", 8f), Left = 308, Top = 9, AutoSize = true });
            filtD.Controls.Add(_dtpIniDias);
            filtD.Controls.Add(new Label { Text = "Até:", ForeColor = Color.FromArgb(80, 60, 40), Font = new Font("Segoe UI", 8f), Left = 440, Top = 9, AutoSize = true });
            filtD.Controls.Add(_dtpFimDias);
            var btnFiltrarD = new Button { Text = "Filtrar", Left = 568, Top = 5, Width = 62, Height = 24, BackColor = CorBotaoAtivo, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFiltrarD.FlatAppearance.BorderSize = 0;
            btnFiltrarD.Click += (_, __) =>
            {
                _periodoDias = "custom";
                foreach (var b in _btnsDiasPeriodo) { b.BackColor = Color.FromArgb(200, 192, 170); b.ForeColor = Color.FromArgb(70, 60, 45); }
                CarregarChartDias();
            };
            filtD.Controls.Add(btnFiltrarD);
            _pnlChartDias = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(248, 245, 240), Tag = "Receita por Dia" };
            _pnlChartDias.Paint += (s, e) => DesenharBarrasVerticais(e.Graphics, (Panel)s, _dadosDias, Color.FromArgb(115, 140, 50));
            outerD.Controls.Add(_pnlChartDias); outerD.Controls.Add(filtD);
            outerD.Controls.Add(new Panel { Height = 4, Dock = DockStyle.Top, BackColor = Color.FromArgb(115, 140, 50) });

            tbl.Controls.Add(outerC, 0, 0);
            tbl.Controls.Add(outerP, 0, 1);
            tbl.Controls.Add(outerD, 0, 2);

            pnlDashboard.Controls.Add(tbl);
            pnlDashboard.Controls.Add(pnlCards);
            pnlContent.Controls.Add(pnlDashboard);
        }

        private Panel CriarPainelGrafico(string titulo, Color corBorda)
        {
            var pnl = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(28, 37, 65),
                Margin    = new Padding(0, 0, 8, 0),
                Tag       = titulo
            };
            // Barra colorida no topo
            var barra = new Panel
            {
                Height    = 4,
                Dock      = DockStyle.Top,
                BackColor = corBorda
            };
            pnl.Controls.Add(barra);
            return pnl;
        }

        // ── Desenho GDI+ ─────────────────────────────────────────────────────

        private static void DesenharBarrasVerticais(Graphics g, Panel pnl, (string label, float value)[] data, Color corBarra, string valueFormat = "0.00")
        {
            string titulo = pnl.Tag?.ToString() ?? "";
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            int W = pnl.Width, H = pnl.Height;

            using var bgBrush    = new SolidBrush(Color.FromArgb(248, 245, 240));
            using var titleFont  = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var labelFont  = new Font("Segoe UI", 7f);
            using var valFont    = new Font("Segoe UI", 7f, FontStyle.Bold);
            using var whiteBrush = new SolidBrush(Color.FromArgb(50, 40, 20));
            using var grayBrush  = new SolidBrush(Color.FromArgb(130, 115, 90));
            using var barBrush   = new SolidBrush(corBarra);
            using var gridPen    = new System.Drawing.Pen(Color.FromArgb(215, 205, 190), 1f);
            gridPen.DashStyle    = System.Drawing.Drawing2D.DashStyle.Dot;

            g.FillRectangle(bgBrush, 0, 0, W, H);

            // Title centered, uppercase
            using var sfCenter = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(titulo.ToUpper(), titleFont, whiteBrush, W / 2f, 10f, sfCenter);

            if (data == null || data.Length == 0)
            {
                g.DrawString("Sem dados para o período", labelFont, grayBrush, 10f, 40f);
                return;
            }

            float maxV = 0; foreach (var d in data) if (d.value > maxV) maxV = d.value;
            if (maxV <= 0) maxV = 1;

            float yAxisW      = 68f;
            float xLblH       = 28f;
            float chartTop    = 44f;
            float chartBottom = H - xLblH;
            float chartLeft   = yAxisW;
            float chartRight  = W - 14f;
            float chartH      = Math.Max(chartBottom - chartTop, 1f);
            float chartW      = chartRight - chartLeft;

            // Adaptive grid count: keep at least 18px between lines
            int gridCount = (int)(chartH / 18);
            gridCount = Math.Max(2, Math.Min(5, gridCount));
            for (int gi = 0; gi <= gridCount; gi++)
            {
                float frac = (float)gi / gridCount;
                float yVal = maxV * frac;
                float yPos = chartBottom - frac * chartH;
                g.DrawLine(gridPen, chartLeft, yPos, chartRight, yPos);
                string yLbl = yVal >= 1000 ? $"{yVal / 1000:0.##}k" : yVal.ToString(valueFormat);
                var ySize = g.MeasureString(yLbl, labelFont);
                if (gi == 0 || frac * chartH > 14f)
                    g.DrawString(yLbl, labelFont, grayBrush, chartLeft - ySize.Width - 4f, yPos - ySize.Height / 2f);
            }

            float slotW = chartW / data.Length;
            float barW  = Math.Max(Math.Min(slotW * 0.65f, 40f), 8f);

            using var sfXLabel = new StringFormat { Alignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };

            for (int i = 0; i < data.Length; i++)
            {
                float cx   = chartLeft + i * slotW + slotW / 2f;
                float x    = cx - barW / 2f;
                float barH = (data[i].value / maxV) * chartH;
                float barY = chartBottom - barH;

                g.FillRectangle(barBrush, x, barY, barW, barH);
                using var hl = new SolidBrush(Color.FromArgb(55, 255, 255, 255));
                if (barH > 3) g.FillRectangle(hl, x, barY, barW, 3f);

                // value above bar, centered
                string valStr = data[i].value.ToString("N2");
                var vSize = g.MeasureString(valStr, valFont);
                float valY = barY - vSize.Height - 2f;
                if (valY < 2f) valY = 2f;
                g.DrawString(valStr, valFont, whiteBrush, cx - vSize.Width / 2f, valY);

                // Horizontal label centered below bar
                var lblRect = new System.Drawing.RectangleF(cx - slotW / 2f, chartBottom + 4f, slotW, xLblH - 4f);
                g.DrawString(data[i].label, labelFont, grayBrush, lblRect, sfXLabel);
            }
        }

        private static void DesenharBarrasHorizontais(Graphics g, Panel pnl, (string label, float value)[] data, Color corBarra)
        {
            string titulo = pnl.Tag?.ToString() ?? "";
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            int W = pnl.Width, H = pnl.Height;

            using var bgBrush    = new SolidBrush(Color.FromArgb(248, 245, 240));
            using var titleFont  = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var labelFont  = new Font("Segoe UI", 7.5f);
            using var valFont    = new Font("Segoe UI", 7f, FontStyle.Bold);
            using var whiteBrush = new SolidBrush(Color.FromArgb(50, 40, 20));
            using var grayBrush  = new SolidBrush(Color.FromArgb(130, 115, 90));
            using var barBrush   = new SolidBrush(corBarra);
            using var gridPen    = new System.Drawing.Pen(Color.FromArgb(215, 205, 190), 1f);
            gridPen.DashStyle    = System.Drawing.Drawing2D.DashStyle.Dot;

            g.FillRectangle(bgBrush, 0, 0, W, H);

            using var sfCenter = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(titulo.ToUpper(), titleFont, whiteBrush, W / 2f, 10f, sfCenter);

            if (data == null || data.Length == 0)
            {
                g.DrawString("Sem dados para o período", labelFont, grayBrush, 10f, 40f);
                return;
            }

            float maxV = 0; foreach (var d in data) if (d.value > maxV) maxV = d.value;
            if (maxV <= 0) maxV = 1;

            float chartTop = 38f, chartLeft = 115f, chartRight = W - 52f, chartBottom = H - 10f;
            int maxItems = Math.Min(data.Length, 8);
            float slotH = (chartBottom - chartTop) / maxItems;
            float barH  = Math.Max(slotH * 0.55f, 6f);

            for (int i = 0; i < maxItems; i++)
            {
                float cy   = chartTop + i * slotH + slotH / 2f;
                float y    = cy - barH / 2f;
                float barW = (data[i].value / maxV) * (chartRight - chartLeft);

                g.FillRectangle(barBrush, chartLeft, y, barW, barH);
                using var hl = new SolidBrush(Color.FromArgb(55, 255, 255, 255));
                if (barW > 4) g.FillRectangle(hl, chartLeft, y, 4f, barH);

                // name on the left, right-aligned to chartLeft
                string lbl = data[i].label.Length > 15 ? data[i].label[..15] : data[i].label;
                var lSize = g.MeasureString(lbl, labelFont);
                g.DrawString(lbl, labelFont, grayBrush, chartLeft - lSize.Width - 5f, cy - lSize.Height / 2f);

                // value to the right of the bar
                string valStr = data[i].value.ToString("0.00");
                g.DrawString(valStr, valFont, whiteBrush, chartLeft + barW + 5f, cy - 7f);
            }
        }

        private static void DesenharLinha(Graphics g, Panel pnl, (string label, float value)[] data, Color corLinha)
        {
            string titulo = pnl.Tag?.ToString() ?? "";
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            int W = pnl.Width, H = pnl.Height;

            using var bgBrush    = new SolidBrush(Color.FromArgb(248, 245, 240));
            using var titleFont  = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var labelFont  = new Font("Segoe UI", 7f);
            using var whiteBrush = new SolidBrush(Color.FromArgb(50, 40, 20));
            using var grayBrush  = new SolidBrush(Color.FromArgb(130, 115, 90));
            using var linePen    = new System.Drawing.Pen(corLinha, 2.5f);
            using var dotBrush   = new SolidBrush(corLinha);
            using var gridPen    = new System.Drawing.Pen(Color.FromArgb(215, 205, 190), 1f);
            gridPen.DashStyle    = System.Drawing.Drawing2D.DashStyle.Dot;

            g.FillRectangle(bgBrush, 0, 0, W, H);

            using var sfCenter = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(titulo.ToUpper(), titleFont, whiteBrush, W / 2f, 10f, sfCenter);

            if (data == null || data.Length < 2)
            {
                if (data != null && data.Length == 1)
                    g.DrawString($"{data[0].label}: R${data[0].value:N2}", labelFont, grayBrush, 10f, 40f);
                else
                    g.DrawString("Sem dados para o período", labelFont, grayBrush, 10f, 40f);
                return;
            }

            float maxV = 0; foreach (var d in data) if (d.value > maxV) maxV = d.value;
            if (maxV <= 0) maxV = 1;

            float yAxisW      = 68f;
            float chartTop    = 44f;
            float chartBottom = H - 36f;
            float chartLeft   = yAxisW;
            float chartRight  = W - 14f;
            float chartH = chartBottom - chartTop;
            float stepX  = (chartRight - chartLeft) / Math.Max(data.Length - 1, 1);

            // Horizontal grid lines + Y-axis labels
            int gridCount = chartH > 0 ? Math.Min(5, Math.Max(2, (int)(chartH / 18))) : 5;
            for (int gi = 0; gi <= gridCount; gi++)
            {
                float frac = (float)gi / gridCount;
                float yVal = maxV * frac;
                float yPos = chartBottom - frac * chartH;
                g.DrawLine(gridPen, chartLeft, yPos, chartRight, yPos);
                string yLbl = yVal >= 1000 ? $"R${yVal / 1000:0.00}k" : $"R${yVal:0.00}";
                var ySize = g.MeasureString(yLbl, labelFont);
                if (gi == 0 || frac * chartH > 14f)
                    g.DrawString(yLbl, labelFont, grayBrush, chartLeft - ySize.Width - 4f, yPos - ySize.Height / 2f);
            }

            var pts = new System.Drawing.PointF[data.Length];
            for (int i = 0; i < data.Length; i++)
                pts[i] = new System.Drawing.PointF(chartLeft + i * stepX, chartBottom - (data[i].value / maxV) * chartH);

            // Area fill
            var polyPts = new System.Drawing.PointF[data.Length + 2];
            polyPts[0] = new System.Drawing.PointF(pts[0].X, chartBottom);
            for (int i = 0; i < pts.Length; i++) polyPts[i + 1] = pts[i];
            polyPts[polyPts.Length - 1] = new System.Drawing.PointF(pts[pts.Length - 1].X, chartBottom);
            using var fillBrush = new SolidBrush(Color.FromArgb(50, corLinha));
            g.FillPolygon(fillBrush, polyPts);

            g.DrawLines(linePen, pts);

            // Dots and labels
            int step = data.Length > 14 ? (int)Math.Ceiling(data.Length / 14.0) : 1;
            for (int i = 0; i < data.Length; i++)
            {
                g.FillEllipse(dotBrush, pts[i].X - 3.5f, pts[i].Y - 3.5f, 7f, 7f);
                if (i % step == 0)
                {
                    string lbl = data[i].label.Length > 5 ? data[i].label[5..] : data[i].label;
                    g.DrawString(lbl, labelFont, grayBrush, pts[i].X - 12f, chartBottom + 4f);
                }
            }
        }

        private static (string label, float value)[] DataTableParaChart(DataTable dt, string colLabel, string colValue, bool isDate = false)
        {
            if (dt == null || dt.Rows.Count == 0) return Array.Empty<(string, float)>();
            var result = new (string label, float value)[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string lbl = isDate && dt.Rows[i][colLabel] is DateTime d
                    ? d.ToString("dd/MM")
                    : dt.Rows[i][colLabel]?.ToString() ?? "";
                float val  = dt.Rows[i][colValue] == DBNull.Value ? 0f : Convert.ToSingle(dt.Rows[i][colValue]);
                result[i]  = (lbl, val);
            }
            return result;
        }

        private void CarregarCharts() { CarregarChartCanal(); CarregarChartProdutos(); CarregarChartDias(); }

        private void CarregarChartCanal()
        {
            if (_pnlChartCanal == null) return;
            try
            {
                _dadosCanal = _periodoCanal == "custom"
                    ? DataTableParaChart(_dashBLL.GetVendasPorCanal(_dtpIniCanal.Value, _dtpFimCanal.Value), "Canal", "TotalVendas")
                    : DataTableParaChart(_dashBLL.GetVendasPorPeriodo(_periodoCanal), "Periodo", "TotalVendas");
                _pnlChartCanal.Refresh();
            }
            catch (Exception ex) { MessageBox.Show("Erro Canal: " + ex.Message); }
        }

        private void CarregarChartProdutos()
        {
            if (_pnlChartProdutos == null) return;
            try
            {
                _dadosProdutos = _periodoProd == "custom"
                    ? DataTableParaChart(_dashBLL.GetTopProdutos(_dtpIniProd.Value, _dtpFimProd.Value, 5), "Produto", "Quantidade")
                    : DataTableParaChart(_dashBLL.GetTopProdutosPeriodo(_periodoProd, 5), "Produto", "Quantidade");
                _pnlChartProdutos.Refresh();
            }
            catch (Exception ex) { MessageBox.Show("Erro Produtos: " + ex.Message); }
        }

        private void CarregarChartDias()
        {
            if (_pnlChartDias == null) return;
            try
            {
                _dadosDias = _periodoDias == "custom"
                    ? DataTableParaChart(_dashBLL.GetVendasPorDia(_dtpIniDias.Value, _dtpFimDias.Value), "Dia", "TotalVendas", true)
                    : DataTableParaChart(_dashBLL.GetVendasPorPeriodo(_periodoDias), "Periodo", "TotalVendas");
                _pnlChartDias.Refresh();
            }
            catch (Exception ex) { MessageBox.Show("Erro Dias: " + ex.Message); }
        }

        private Label CriarCard(FlowLayoutPanel pai, string titulo, string valor, Color cor, Action onClick = null)
        {
            int w = 230;
            var pnl = new Panel
            {
                Width     = w,
                Height    = 110,
                BackColor = cor,
                Margin    = new Padding(0, 0, 16, 0),
                Cursor    = onClick != null ? Cursors.Hand : Cursors.Default
            };
            if (onClick != null)
            {
                pnl.Click += (_, __) => onClick();
            }

            pnl.Controls.Add(new Label
            {
                Text      = titulo,
                ForeColor = Color.FromArgb(235, 215, 185),
                Font      = new Font("Segoe UI", 9),
                AutoSize  = false,
                Width     = w - 20,
                Height    = 28,
                Top       = 14,
                Left      = 14,
                TextAlign = ContentAlignment.TopLeft
            });

            var lblVal = new Label
            {
                Text      = valor,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize  = false,
                Width     = w - 20,
                Height    = 50,
                Top       = 40,
                Left      = 14,
                TextAlign = ContentAlignment.TopLeft
            };
            pnl.Controls.Add(lblVal);
            if (onClick != null)
                foreach (Control c in pnl.Controls)
                    c.Click += (_, __) => onClick();
            pai.Controls.Add(pnl);
            return lblVal;
        }

        // --------------------------------------------------------------------
        // PEDIDOS
        // --------------------------------------------------------------------
        private void BuildPedidos()
        {
            pnlPedidos = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Visible = false };

            // Top filters
            var pnlFiltros = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 48,
                BackColor = Color.Transparent
            };

            var lblSit = new Label { Text = "Situacao:", ForeColor = Color.FromArgb(50, 40, 25), Left = 0, Top = 14, AutoSize = true };
            cmbFiltroPedido = new ComboBox
            {
                Left          = 68,
                Top           = 10,
                Width         = 160,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFiltroPedido.Items.AddRange(new object[] { "Todos", "Pendentes", "Em Preparo", "Finalizados", "Cancelados" });
            cmbFiltroPedido.SelectedIndex = 0;

            var lblDt = new Label { Text = "Data:", ForeColor = Color.FromArgb(50, 40, 25), Left = 244, Top = 14, AutoSize = true };
            dtpFiltroPedido = new DateTimePicker
            {
                Left   = 286,
                Top    = 10,
                Width  = 120,
                Format = DateTimePickerFormat.Short,
                Value  = DateTime.Today
            };

            var btnFiltrar = new Button
            {
                Text      = "Filtrar",
                Left      = 420,
                Top       = 8,
                Width     = 80,
                Height    = 28,
                BackColor = CorBotaoAtivo,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnFiltrar.FlatAppearance.BorderSize = 0;
            btnFiltrar.Click += (_, __) => CarregarPedidos();

            var btnTodos = new Button
            {
                Text      = "Ver Todos",
                Left      = 510,
                Top       = 8,
                Width     = 80,
                Height    = 28,
                BackColor = Color.FromArgb(200, 192, 170),
                ForeColor = Color.FromArgb(60, 50, 35),
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnTodos.FlatAppearance.BorderSize = 0;
            btnTodos.Click += (_, __) => { cmbFiltroPedido.SelectedIndex = 0; CarregarPedidos(); };

            var btnManual = new Button
            {
                Text      = "\u270F Pedido Manual",
                Left      = 602,
                Top       = 8,
                Width     = 128,
                Height    = 28,
                BackColor = Color.FromArgb(142, 68, 173),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnManual.FlatAppearance.BorderSize = 0;
            btnManual.Click += (_, __) =>
            {
                using (var frm = new Forms.frmPedidoManual())
                {
                    if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        CarregarPedidos();
                }
            };

            var btnBuscarWeb = new Button
            {
                Text      = "\u2193 Buscar Web",
                Left      = 738,
                Top       = 8,
                Width     = 110,
                Height    = 28,
                BackColor = Color.FromArgb(39, 130, 57),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold)
            };
            btnBuscarWeb.FlatAppearance.BorderSize = 0;
            btnBuscarWeb.Click += async (_, __) =>
            {
                btnBuscarWeb.Enabled = false;
                btnBuscarWeb.Text    = "Buscando...";
                try
                {
                    var diag = await System.Threading.Tasks.Task.Run(
                        () => DB.SupabaseService.ImportarPedidosManuaisAsync());
                    CarregarPedidos();
                    MessageBox.Show(diag, "Buscar Pedidos Web",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                finally { btnBuscarWeb.Enabled = true; btnBuscarWeb.Text = "\u2193 Buscar Web"; }
            };

            pnlFiltros.Controls.AddRange(new System.Windows.Forms.Control[]
                { lblSit, cmbFiltroPedido, lblDt, dtpFiltroPedido, btnFiltrar, btnTodos, btnManual, btnBuscarWeb });

            // Detalhe header
            lblDetalhe = new Label
            {
                Text      = "Itens do Pedido",
                ForeColor = Color.FromArgb(50, 40, 25),
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock      = DockStyle.Top,
                Height    = 28,
                BackColor = Color.Transparent
            };

            gridItens = CriarGrid();
            gridItens.Dock = DockStyle.Fill;

            // Acoes
            var pnlAcoes = new FlowLayoutPanel
            {
                Dock          = DockStyle.Bottom,
                Height        = 40,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = false,
                BackColor     = Color.Transparent
            };

            Button MkBtn(string text, Color bg, int sit)
            {
                var b = new Button
                {
                    Text      = text,
                    Width     = 128,
                    Height    = 30,
                    BackColor = bg,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Margin    = new Padding(0, 4, 8, 0),
                    Cursor    = Cursors.Hand,
                    Visible   = false
                };
                b.FlatAppearance.BorderSize = 0;
                b.Click += (_, __) => AtualizarSituacaoPedido(sit);
                pnlAcoes.Controls.Add(b);
                return b;
            }

            _btnConfirmar = MkBtn("\u2713 Confirmar",          Color.FromArgb(39, 174, 96),  1);
            _btnEmPreparo = MkBtn("\u23F3 Em Preparo",         Color.FromArgb(243, 156, 18), 2);
            _btnPronto    = MkBtn("\u2705 Pronto",             Color.FromArgb(22, 160, 133), 3);
            _btnSaiu      = MkBtn("\u2192 Saiu p/ Entrega",Color.FromArgb(52, 152, 219), 4);
            _btnEntregue  = MkBtn("\u2713 Entregue",       Color.FromArgb(22, 160, 133), 5);
            _btnCancelar  = MkBtn("\u2715 Cancelar",           Color.FromArgb(192, 57, 43),  6);

            // Botao reimprimir (separado do MkBtn pois nao muda situacao)
            _btnReimprimir = new Button
            {
                Text      = "\uD83D\uDDC8 Reimprimir",
                Width     = 128,
                Height    = 30,
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin    = new Padding(0, 4, 8, 0),
                Cursor    = Cursors.Hand,
                Visible   = false
            };
            _btnReimprimir.FlatAppearance.BorderSize = 0;
            _btnReimprimir.Click += (_, __) =>
            {
                if (gridPedidos?.SelectedRows.Count > 0)
                    try { ImprimirCupomPedido(Convert.ToInt32(gridPedidos.SelectedRows[0].Cells["Codigo"].Value)); }
                    catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
            };
            pnlAcoes.Controls.Add(_btnReimprimir);

            // Main grid
            var gridMain = CriarGrid();
            gridMain.Dock = DockStyle.Fill;
            gridMain.SelectionChanged += (_, __) => { CarregarItensPedido(gridMain); AtualizarBotoesPedido(); };
            // Keep ref for save action
            gridPedidos = gridMain;

            var split = new SplitContainer
            {
                Dock         = DockStyle.Fill,
                Orientation  = Orientation.Horizontal,
                BackColor    = Color.Transparent,
                Panel1MinSize = 80,
                Panel2MinSize = 80,
            };
            split.Panel1.Controls.Add(gridMain);
            split.Panel2.Controls.Add(gridItens);
            split.Panel2.Controls.Add(lblDetalhe);

            pnlPedidos.Controls.Add(split);
            pnlPedidos.Controls.Add(pnlAcoes);
            pnlPedidos.Controls.Add(pnlFiltros);
            Shown += (_, __) =>
            {
                try { split.SplitterDistance = split.Height / 2; } catch { }
            };
            pnlContent.Controls.Add(pnlPedidos);
        }

        private DataGridView CriarGrid()
        {
            var g = new DataGridView
            {
                AutoSizeColumnsMode    = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly               = true,
                AllowUserToAddRows     = false,
                SelectionMode          = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible      = false,
                BackgroundColor        = Color.White,
                GridColor              = Color.FromArgb(220, 210, 195),
                DefaultCellStyle       = { BackColor = Color.White, ForeColor = Color.FromArgb(50, 40, 30), SelectionBackColor = Color.FromArgb(176, 110, 42), SelectionForeColor = Color.White },
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(250, 247, 242) },
                ColumnHeadersDefaultCellStyle   = { BackColor = Color.FromArgb(36, 48, 82), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) },
                ColumnHeadersHeightSizeMode     = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight             = 36,
                RowTemplate                     = { Height = 28 },
                Font                            = new Font("Segoe UI", 9),
                BorderStyle                     = BorderStyle.None,
                MultiSelect                     = false
            };
            return g;
        }

        // --------------------------------------------------------------------
        // NAVEGACAO
        // --------------------------------------------------------------------
        private void MostrarDashboard()
        {
            pnlDashboard.Visible  = true;
            pnlPedidos.Visible    = false;
            if (pnlFinanceiro != null) pnlFinanceiro.Visible = false;
            lblTitulo.Text        = "Dashboard";
            _paginaAtual          = 0;
            CarregarDashboard();
        }

        private void MostrarPedidos()
        {
            pnlDashboard.Visible  = false;
            pnlPedidos.Visible    = true;
            if (pnlFinanceiro != null) pnlFinanceiro.Visible = false;
            lblTitulo.Text        = "Pedidos";
            _paginaAtual          = 1;
            CarregarPedidos();
        }

        private void MostrarEmpresa()
        {
            using (var frm = new Forms.frmEmpresa()) frm.ShowDialog(this);
        }

        private void AbrirForm(Form f)
        {
            f.ShowDialog(this);
        }

        // --------------------------------------------------------------------
        // CARGA DE DADOS
        // --------------------------------------------------------------------
        private void CarregarTudo()
        {
            CarregarDashboard();
            if (_paginaAtual == 1) CarregarPedidos();
        }

        private void CarregarDashboard()
        {
            try
            {
                var (pedHoje, fat, cli, pend) = _dashBLL.GetEstatisticas();
                lblPedidosHoje.Text = pedHoje.ToString();
                lblFaturamento.Text = fat.ToString("C");
                lblClientes.Text    = cli.ToString();
                lblPendentes.Text   = pend.ToString();

                CarregarCharts();
            }
            catch (Exception ex)
            {
                if (IsHandleCreated)
                    BeginInvoke(new Action(() =>
                        MessageBox.Show("Erro ao carregar dashboard: " + ex.Message)));
            }
        }

        private void CarregarPedidos()
        {
            // Preserve currently selected order
            int selectedCod = 0;
            if (gridPedidos?.SelectedRows.Count > 0)
                try { selectedCod = Convert.ToInt32(gridPedidos.SelectedRows[0].Cells["Codigo"].Value); } catch { }

            try
            {
                string filtro = null;
                if (cmbFiltroPedido?.SelectedIndex > 0)
                {
                    var sel = cmbFiltroPedido.SelectedItem?.ToString() ?? "";
                    if      (sel == "Pendentes")   filtro = "pendentes";
                    else if (sel == "Em Preparo")  filtro = "emPreparo";
                    else if (sel == "Finalizados") filtro = "finalizados";
                    else if (sel == "Cancelados")  filtro = "cancelados";
                }
                DateTime? dt = dtpFiltroPedido?.Value.Date;
                gridPedidos.DataSource = _pedidoBLL.Listar(filtro, dt);
                AjustarColunasPedidos();
                gridItens.DataSource   = null;
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar pedidos: " + ex.Message); }

            if (selectedCod > 0) RestaurarSelecaoPedido(selectedCod);
        }

        private void CarregarPedidosSemPerderSelecao()
        {
            int selectedCod = 0;
            if (gridPedidos?.SelectedRows.Count > 0)
                try { selectedCod = Convert.ToInt32(gridPedidos.SelectedRows[0].Cells["Codigo"].Value); } catch { }

            try
            {
                string filtro = null;
                if (cmbFiltroPedido?.SelectedIndex > 0)
                {
                    var sel = cmbFiltroPedido.SelectedItem?.ToString() ?? "";
                    if      (sel == "Pendentes")   filtro = "pendentes";
                    else if (sel == "Em Preparo")  filtro = "emPreparo";
                    else if (sel == "Finalizados") filtro = "finalizados";
                    else if (sel == "Cancelados")  filtro = "cancelados";
                }
                DateTime? dt = dtpFiltroPedido?.Value.Date;
                gridPedidos.DataSource = _pedidoBLL.Listar(filtro, dt);
                AjustarColunasPedidos();
                // Do NOT reset gridItens — keep items visible for selected order
            }
            catch { }

            if (selectedCod > 0) RestaurarSelecaoPedido(selectedCod);
        }

        private void AjustarColunasPedidos()
        {
            if (gridPedidos.Columns.Count == 0) return;

            // Ocultar colunas internas
            if (gridPedidos.Columns["Codigo"] != null)  gridPedidos.Columns["Codigo"].Visible  = false;
            if (gridPedidos.Columns["Origem"] != null)  gridPedidos.Columns["Origem"].Visible  = false;

            // Nomes de exibição
            var nomes = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Numero"]   = "N\u00ba Pedido",
                ["Cliente"]  = "Cliente",
                ["Telefone"] = "Telefone",
                ["Status"]   = "Status",
                ["Itens"]    = "Itens",
                ["Pagamento"]= "Pagamento",
                ["Entrega"]  = "Entrega",
                ["Total"]    = "Total",
                ["DataHora"] = "Data / Hora"
            };
            foreach (var kv in nomes)
                if (gridPedidos.Columns[kv.Key] != null)
                    gridPedidos.Columns[kv.Key].HeaderText = kv.Value;

            // Ordem de exibição
            string[] ordem = { "Numero", "Cliente", "Telefone", "Status", "Itens", "Pagamento", "Entrega", "Total", "DataHora" };
            for (int i = 0; i < ordem.Length; i++)
                if (gridPedidos.Columns[ordem[i]] != null)
                    gridPedidos.Columns[ordem[i]].DisplayIndex = i;

            // Larguras relativas (FillWeight)
            var fills = new System.Collections.Generic.Dictionary<string, int>
            {
                ["Numero"]   = 80,
                ["Cliente"]  = 160,
                ["Telefone"] = 100,
                ["Status"]   = 100,
                ["Itens"]    = 45,
                ["Pagamento"]= 80,
                ["Entrega"]  = 70,
                ["Total"]    = 80,
                ["DataHora"] = 120
            };
            foreach (var kv in fills)
                if (gridPedidos.Columns[kv.Key] != null)
                    gridPedidos.Columns[kv.Key].FillWeight = kv.Value;
        }

        private void CarregarItensPedido(DataGridView grid)
        {
            if (grid.SelectedRows.Count == 0) return;
            try
            {
                var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
                gridItens.DataSource = _pedidoBLL.ListarItens(cod);
                var pedido = _pedidoBLL.PesquisaCodigo(cod);
                if (pedido != null)
                    lblDetalhe.Text = $"Itens - Pedido #{pedido.pediNumero}  |  {PedidoBLL.LabelSituacao(pedido.pediSituacao)}  |  Total: {pedido.pediValor_Total:C}";
            }
            catch { }
        }

        private void AtualizarBotoesPedido()
        {
            if (gridPedidos == null || gridPedidos.SelectedRows.Count == 0)
            {
                if (_btnConfirmar != null)
                {
                    _btnConfirmar.Visible = _btnEmPreparo.Visible = _btnPronto.Visible =
                        _btnSaiu.Visible = _btnEntregue.Visible = _btnCancelar.Visible = false;
                    if (_btnReimprimir != null) _btnReimprimir.Visible = false;
                }
                return;
            }

            try
            {
                var cod    = Convert.ToInt32(gridPedidos.SelectedRows[0].Cells["Codigo"].Value);
                var pedido = _pedidoBLL.PesquisaCodigo(cod);
                if (pedido == null) return;

                bool retirada = pedido.pediTipo_Entrega == 0;
                bool terminal = pedido.pediSituacao == 5 || pedido.pediSituacao == 6;

                _btnConfirmar.Visible = !terminal;
                _btnEmPreparo.Visible = !terminal;
                _btnPronto.Visible    = retirada && !terminal;
                _btnSaiu.Visible      = !retirada && !terminal;
                _btnEntregue.Visible  = !terminal;   // visível para retirada e entrega
                _btnCancelar.Visible  = !terminal;
                if (_btnReimprimir != null) _btnReimprimir.Visible = true;
            }
            catch { }
        }

        private void AtualizarSituacaoPedido(int novaSit)
        {
            if (gridPedidos.SelectedRows.Count == 0) { MessageBox.Show("Selecione um pedido na lista."); return; }
            var cod   = Convert.ToInt32(gridPedidos.SelectedRows[0].Cells["Codigo"].Value);
            var pedido = _pedidoBLL.PesquisaCodigo(cod);
            if (pedido == null) { MessageBox.Show("Pedido nao encontrado."); return; }

            // Estados terminais: nenhuma alteracao permitida
            if (pedido.pediSituacao == 5 || pedido.pediSituacao == 6)
            {
                MessageBox.Show("Este pedido ja esta em estado terminal e nao pode ser alterado.");
                return;
            }

            // Cancelar — sempre solicita autorizacao
            if (novaSit == 6)
            {
                using var dlgAuth = new Forms.frmAutorizacao();
                if (dlgAuth.ShowDialog(this) != DialogResult.OK) return;
                string canceladoPor = dlgAuth.UsuarioAutorizador.usuNome;
                var eA = _pedidoBLL.AtualizarSituacao(cod, 6, canceladoPor);
                if (!string.IsNullOrEmpty(eA)) MessageBox.Show("Erro: " + eA);
                else
                {
                    Logger.Log("Form1", "AtualizarSituacaoPedido", $"Pedido cancelado | #{cod} | Autorizado por: {canceladoPor}");
                    CarregarPedidos();
                    RestaurarSelecaoPedido(cod);
                }
                return;
            }

            // Validar transicao de estados
            if (!PedidoBLL.PodeTransicionar(pedido.pediSituacao, novaSit))
            {
                MessageBox.Show($"Nao e possivel ir de '{PedidoBLL.LabelSituacao(pedido.pediSituacao)}' para '{PedidoBLL.LabelSituacao(novaSit)}'.");
                return;
            }

            // Pronto (3) para Retirada / Entregue (5) para Entrega: dialog de pagamento
            if (novaSit == 3 || novaSit == 5)
            {
                bool retirada = pedido.pediTipo_Entrega == 0;
                bool finaliza = (novaSit == 3 && retirada) || (novaSit == 5 && !retirada);
                if (finaliza)
                {
                    if (MostrarDialogPagamento(pedido, out decimal vPago, out string trans,
                                               out decimal din, out decimal car, out decimal pix,
                                               out string autNome))
                    {
                        var eF = _pedidoBLL.FinalizarPedido(cod, novaSit, vPago, trans, din, car, pix, autNome);
                        if (!string.IsNullOrEmpty(eF)) MessageBox.Show("Erro: " + eF);
                        else
                        {
                            string logExtra = $"#{cod} | Sit={novaSit} | Total={pedido.pediValor_Total:N2} | Pago={vPago:N2} | Din={din:N2} Car={car:N2} Pix={pix:N2}";
                            if (!string.IsNullOrEmpty(autNome))
                                logExtra += $" | DESCONTO R$ {(pedido.pediValor_Total - vPago):N2} autorizado por {autNome}";
                            Logger.Log("Form1", "AtualizarSituacaoPedido", $"Pedido finalizado | {logExtra}");
                            CarregarPedidos();
                            RestaurarSelecaoPedido(cod);
                            // Troco em dinheiro
                            decimal troco = vPago - pedido.pediValor_Total;
                            if (din > 0 && troco > 0.005m)
                            {
                                using var dlgTroco = new Form
                                {
                                    Text            = "Troco",
                                    StartPosition   = FormStartPosition.CenterParent,
                                    FormBorderStyle = FormBorderStyle.FixedDialog,
                                    MaximizeBox     = false, MinimizeBox = false,
                                    BackColor       = Color.FromArgb(245, 237, 216),
                                    ClientSize      = new Size(320, 148),
                                    Font            = new Font("Segoe UI", 9F)
                                };
                                var pnlTrocoTop = new Panel { Left = 0, Top = 0, Width = 320, Height = 36, BackColor = Color.FromArgb(176, 110, 42) };
                                pnlTrocoTop.Controls.Add(new Label { Text = "Troco para o Cliente", ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), AutoSize = true, Left = 12, Top = 8 });
                                dlgTroco.Controls.Add(pnlTrocoTop);
                                dlgTroco.Controls.Add(new Label
                                {
                                    Text      = $"💰  Devolver  R$ {troco:N2}  em dinheiro",
                                    Left = 20, Top = 52, AutoSize = true,
                                    Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
                                    ForeColor = Color.FromArgb(39, 127, 56)
                                });
                                dlgTroco.Controls.Add(new Label
                                {
                                    Text = $"Recebido: R$ {din:N2}   |   Total: R$ {pedido.pediValor_Total:N2}",
                                    Left = 20, Top = 86, AutoSize = true,
                                    ForeColor = Color.FromArgb(100, 80, 40)
                                });
                                var btnOkTroco = new Button { Text = "OK", Left = 110, Top = 106, Width = 100, Height = 30,
                                    BackColor = Color.FromArgb(87, 120, 38), ForeColor = Color.White,
                                    FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.OK };
                                btnOkTroco.FlatAppearance.BorderSize = 0;
                                dlgTroco.Controls.Add(btnOkTroco);
                                dlgTroco.AcceptButton = btnOkTroco;
                                dlgTroco.ShowDialog(this);
                            }
                            // Reimprimir cupom quando há desconto autorizado no pagamento
                            if (!string.IsNullOrEmpty(autNome))
                                ImprimirCupomPedido(cod);
                            // Verificar fidelização
                            if (pedido.Codigo_Cliente > 0)
                            {
                                try
                                {
                                    var _clienteBllFid = new BLL.ClienteBLL();
                                    _clienteBllFid.IncrementarTotais(pedido.Codigo_Cliente, vPago);
                                    var premioMsg = new BLL.FidelizacaoBLL().VerificarEDispararPremio(pedido.Codigo_Cliente, cod);
                                    if (!string.IsNullOrEmpty(premioMsg))
                                        MessageBox.Show(premioMsg, "Fidelização", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                catch { }
                            }
                        }
                    }
                    return;
                }
            }

            var erro = _pedidoBLL.AtualizarSituacao(cod, novaSit);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }

            // Notificações WhatsApp
            if (novaSit == 2)
                BLL.WhatsAppService.NotificarPreparo(pedido.pediTelefone_Cliente, pedido.pediNome_Cliente, pedido.pediNumero);
            else if (novaSit == 4)
                BLL.WhatsAppService.NotificarEntrega(pedido.pediTelefone_Cliente, pedido.pediNome_Cliente, pedido.pediNumero);

            // Ao confirmar (situação 1), imprimir duas vias do cupom e enviar comanda no WhatsApp
            if (novaSit == 1)
            {
                ImprimirCupomPedido(cod);

                if (BLL.WhatsAppService.Ativo && !string.IsNullOrWhiteSpace(pedido.pediTelefone_Cliente))
                {
                    try
                    {
                        var itensCf = _pedidoBLL.ListarItensObjetos(cod);
                        var cfgCf   = _impBLL.Carregar();
                        var empCf   = _empBLL.Carregar();
                        string texto = BLL.ImpressaoPedido.GerarTextoWhatsApp(
                            pedido, itensCf, cfgCf, empCf, UsuarioSessao.NomeAtual);
                        BLL.WhatsAppService.NotificarConfirmacao(pedido.pediTelefone_Cliente, texto);
                    }
                    catch { }
                }
            }

            CarregarPedidos();
            RestaurarSelecaoPedido(cod);
        }

        private void RestaurarSelecaoPedido(int codigo)
        {
            if (gridPedidos == null || gridPedidos.Rows.Count == 0) return;
            foreach (DataGridViewRow row in gridPedidos.Rows)
            {
                if (row.Cells["Codigo"] == null) break;
                if (Convert.ToInt32(row.Cells["Codigo"].Value) == codigo)
                {
                    gridPedidos.ClearSelection();
                    row.Selected = true;
                    if (gridPedidos.FirstDisplayedScrollingRowIndex != row.Index)
                        try { gridPedidos.FirstDisplayedScrollingRowIndex = row.Index; } catch { }
                    break;
                }
            }
        }

        private void ImprimirCupomPedido(int codigoPedido)
        {
            try
            {
                var pedido  = _pedidoBLL.PesquisaCodigo(codigoPedido);
                if (pedido == null) return;

                // Se o pedido tem cliente vinculado e o complemento não está no endereço, acrescenta
                if (pedido.Codigo_Cliente > 0 && pedido.pediTipo_Entrega == 1
                    && !string.IsNullOrWhiteSpace(pedido.pediEndereco_Entrega))
                {
                    try
                    {
                        var cli = new BLL.ClienteBLL().PesquisaCodigo(pedido.Codigo_Cliente);
                        if (cli != null && !string.IsNullOrWhiteSpace(cli.clieComplemento))
                        {
                            string comp = cli.clieComplemento.Trim();
                            if (!pedido.pediEndereco_Entrega.Contains(comp))
                                pedido.pediEndereco_Entrega += " - " + comp;
                        }
                    }
                    catch { }
                }

                var itens   = _pedidoBLL.ListarItensObjetos(codigoPedido);
                var cfg     = _impBLL.Carregar();
                var empresa = _empBLL.Carregar();
                var erro    = BLL.ImpressaoPedido.Imprimir(pedido, itens, cfg, empresa, UsuarioSessao.NomeAtual);
                if (!string.IsNullOrEmpty(erro))
                    MessageBox.Show("Aviso: nao foi possivel imprimir o cupom.\n" + erro,
                                    "Impressao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao imprimir cupom: " + ex.Message,
                                "Impressao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool MostrarDialogPagamento(PedidoWeb pedido,
                                             out decimal valorPago, out string transacao,
                                             out decimal pagoDinheiro, out decimal pagoCartao, out decimal pagoPix,
                                             out string autorizadorNome)
        {
            valorPago       = pedido.pediValor_Total;
            transacao       = "";
            pagoDinheiro    = 0m;
            pagoCartao      = 0m;
            pagoPix         = 0m;
            autorizadorNome = "";
            decimal total = pedido.pediValor_Total;

            while (true)   // loop: "Voltar" reinicia o formulário de pagamento
            {
                decimal tmpDin = 0, tmpCar = 0, tmpPix = 0;
                string  tmpTrans = "";
                bool    confirmed = false;

                using (var frm = new Form())
                {
                    frm.Text            = "Finalizar Pedido \u2014 Pagamento";
                    frm.StartPosition   = FormStartPosition.CenterParent;
                    frm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    frm.MaximizeBox     = frm.MinimizeBox = false;
                    frm.BackColor       = Color.FromArgb(245, 237, 216);
                    frm.ForeColor       = Color.FromArgb(50, 50, 50);
                    frm.Font            = new Font("Segoe UI", 9F);
                    frm.ClientSize      = new Size(420, 280);

                    // Barra de título interna
                    var pnlTop = new Panel { Left = 0, Top = 0, Width = 420, Height = 36,
                        BackColor = Color.FromArgb(176, 110, 42) };
                    var lblTit = new Label { Text = $"Total do pedido:  R$ {total:N2}",
                        ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                        AutoSize = true, Left = 12, Top = 8 };
                    pnlTop.Controls.Add(lblTit);
                    frm.Controls.Add(pnlTop);

                    void Lbl(string t, int x, int y)
                    {
                        frm.Controls.Add(new Label { Text = t, Left = x, Top = y, AutoSize = true,
                            ForeColor = Color.FromArgb(70, 70, 70) });
                    }

                    Lbl("Divida o pagamento por forma (deixe 0 se n\u00e3o usar):", 12, 46);
                    Lbl("Dinheiro (R$):", 12, 80);
                    var numDin = new NumericUpDown { Left = 160, Top = 76, Width = 120, DecimalPlaces = 2, Maximum = 99999M, Value = 0M,
                        BackColor = Color.White, ForeColor = Color.FromArgb(50, 50, 50) };
                    Lbl("Cart\u00e3o (R$):", 12, 116);
                    var numCar = new NumericUpDown { Left = 160, Top = 112, Width = 120, DecimalPlaces = 2, Maximum = 99999M, Value = 0M,
                        BackColor = Color.White, ForeColor = Color.FromArgb(50, 50, 50) };
                    Lbl("Pix (R$):", 12, 152);
                    var numPix = new NumericUpDown { Left = 160, Top = 148, Width = 120, DecimalPlaces = 2, Maximum = 99999M, Value = 0M,
                        BackColor = Color.White, ForeColor = Color.FromArgb(50, 50, 50) };

                    if (pedido.pediForma_Pagamento == 1) numCar.Value = total;
                    else if (pedido.pediForma_Pagamento == 2) numPix.Value = total;
                    else numDin.Value = total;  // 0=Dinheiro ou qualquer outro (ex: -1 de pedidos externos)

                    Lbl("C\u00f3d. Transa\u00e7\u00e3o (cart\u00e3o/Pix):", 12, 188);
                    var txtTrans = new TextBox { Left = 240, Top = 184, Width = 164,
                        BackColor = Color.White, ForeColor = Color.FromArgb(50, 50, 50),
                        BorderStyle = BorderStyle.FixedSingle };

                    var lblSoma = new Label
                    {
                        Left = 12, Top = 218, Width = 300, AutoSize = false,
                        ForeColor = Color.FromArgb(176, 110, 42),
                        Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                        Text = ""
                    };
                    frm.Controls.Add(lblSoma);

                    void AtualizarSoma()
                    {
                        decimal soma = numDin.Value + numCar.Value + numPix.Value;
                        decimal diff = soma - total;
                        string sinal = diff >= 0 ? "Troco: R$ " + diff.ToString("N2") : "Falta: R$ " + (-diff).ToString("N2");
                        lblSoma.Text = $"Soma: R$ {soma:N2}  |  {sinal}";
                        lblSoma.ForeColor = diff >= 0 ? Color.FromArgb(87, 120, 38) : Color.FromArgb(192, 57, 43);
                    }

                    numDin.ValueChanged += (_, __) => AtualizarSoma();
                    numCar.ValueChanged += (_, __) => AtualizarSoma();
                    numPix.ValueChanged += (_, __) => AtualizarSoma();
                    AtualizarSoma();

                    frm.Controls.AddRange(new Control[] { numDin, numCar, numPix, txtTrans });

                    var btnOk  = new Button { Text = "\u2714 Confirmar", Left = 100, Top = 243, Width = 140, Height = 28, DialogResult = DialogResult.OK,
                        BackColor = Color.FromArgb(87, 120, 38), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                    var btnCan = new Button { Text = "Cancelar",     Left = 252, Top = 243, Width = 100, Height = 28, DialogResult = DialogResult.Cancel,
                        BackColor = Color.FromArgb(224, 113, 42), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                    btnOk.FlatAppearance.BorderSize = btnCan.FlatAppearance.BorderSize = 0;
                    frm.Controls.AddRange(new Control[] { btnOk, btnCan });
                    frm.AcceptButton = btnOk;
                    frm.CancelButton = btnCan;

                    confirmed = frm.ShowDialog(this) == DialogResult.OK;
                    if (confirmed)
                    {
                        tmpDin   = numDin.Value;
                        tmpCar   = numCar.Value;
                        tmpPix   = numPix.Value;
                        tmpTrans = txtTrans.Text.Trim();
                    }
                }

                if (!confirmed) return false;

                pagoDinheiro = tmpDin;
                pagoCartao   = tmpCar;
                pagoPix      = tmpPix;
                valorPago    = pagoDinheiro + pagoCartao + pagoPix;
                transacao    = tmpTrans;

                if (valorPago >= total) return true;

                // Valor pago menor que o total — mostrar aviso com opção de voltar ou autorizar
                decimal desconto = total - valorPago;
                string  opcao    = "cancelar";

                using (var dlgAviso = new Form())
                {
                    dlgAviso.Text            = "Autoriza\u00e7\u00e3o Necess\u00e1ria";
                    dlgAviso.StartPosition   = FormStartPosition.CenterParent;
                    dlgAviso.FormBorderStyle = FormBorderStyle.FixedDialog;
                    dlgAviso.MaximizeBox     = dlgAviso.MinimizeBox = false;
                    dlgAviso.BackColor       = Color.FromArgb(245, 237, 216);
                    dlgAviso.ClientSize      = new Size(430, 172);
                    dlgAviso.Font            = new Font("Segoe UI", 9F);

                    var ico = new Label
                    {
                        Text = "\u26A0", Left = 14, Top = 14, AutoSize = true,
                        ForeColor = Color.FromArgb(243, 156, 18),
                        Font = new Font("Segoe UI", 20F)
                    };
                    var msg = new Label
                    {
                        Text      = $"Total pago (R$ {valorPago:N2}) \u00e9 menor que o total (R$ {total:N2}).\n" +
                                    $"Desconto de R$ {desconto:N2} requer autoriza\u00e7\u00e3o de gerente.",
                        Left = 58, Top = 16, Width = 358, Height = 60, AutoSize = false,
                        ForeColor = Color.FromArgb(50, 50, 50), Font = new Font("Segoe UI", 9.5F)
                    };
                    var btnVoltar = new Button
                    {
                        Text = "\u2190 Voltar e Corrigir", Left = 14, Top = 124, Width = 184, Height = 32,
                        BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
                    };
                    var btnAut = new Button
                    {
                        Text = "Autorizar com Gerente", Left = 206, Top = 124, Width = 210, Height = 32,
                        BackColor = Color.FromArgb(176, 110, 42), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
                    };
                    btnVoltar.FlatAppearance.BorderSize = btnAut.FlatAppearance.BorderSize = 0;
                    btnVoltar.Click += (_, __) => { opcao = "voltar";    dlgAviso.Close(); };
                    btnAut.Click    += (_, __) => { opcao = "autorizar"; dlgAviso.Close(); };
                    dlgAviso.Controls.AddRange(new Control[] { ico, msg, btnVoltar, btnAut });
                    dlgAviso.ShowDialog(this);
                }

                if (opcao == "voltar")    continue;    // reinicia o loop — mostra o formulário de pagamento novamente
                if (opcao != "autorizar") return false; // fechou com X

                using var dlgAuth = new Forms.frmAutorizacao();
                if (dlgAuth.ShowDialog(this) != DialogResult.OK) return false;
                autorizadorNome = dlgAuth.UsuarioAutorizador.usuNome;
                return true;
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // FINANCEIRO  (modernizado)
        // ────────────────────────────────────────────────────────────────────
        // Campos de UI do painel financeiro
        private FlowLayoutPanel _pnlFinCards;
        private DataGridView    _gridGastos;

        private void MostrarFinanceiro()
        {
            pnlDashboard.Visible  = false;
            pnlPedidos.Visible    = false;
            pnlFinanceiro.Visible = true;
            lblTitulo.Text        = "Financeiro";
            _paginaAtual          = 2;
            CarregarFinanceiro();
        }

        private void BuildFinanceiro()
        {
            pnlFinanceiro = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Visible = false };

            // ── Filtro ──
            var pnlFil = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Color.Transparent };
            var lblDe  = new Label { Text = "De:",  ForeColor = Color.FromArgb(50, 40, 25), Left = 0,   Top = 14, AutoSize = true };
            dtpFinDe   = new DateTimePicker { Left = 32,  Top = 10, Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-30) };
            var lblAte = new Label { Text = "Ate:", ForeColor = Color.FromArgb(50, 40, 25), Left = 164, Top = 14, AutoSize = true };
            dtpFinAte  = new DateTimePicker { Left = 198, Top = 10, Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            var btnFil = new Button { Text = "Filtrar", Left = 332, Top = 8, Width = 80, Height = 28, BackColor = CorBotaoAtivo, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFil.FlatAppearance.BorderSize = 0;
            btnFil.Click += (_, __) => CarregarFinanceiro();
            var btnHoje = new Button { Text = "💰 Vendas por Período", Left = 422, Top = 8, Width = 150, Height = 28, BackColor = Color.FromArgb(52, 100, 160), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnHoje.FlatAppearance.BorderSize = 0;
            btnHoje.Click += (_, __) =>
            {
                using var frm = new Forms.frmRelatorioFinanceiro(_pedidoBLL, _gastosBLL, _entradaBLL,
                    de: DateTime.Today, ate: DateTime.Today);
                frm.Text = $"Vendas por Período — {DateTime.Today:dd/MM/yyyy}";
                frm.ShowDialog(this);
            };
            pnlFil.Controls.AddRange(new Control[] { lblDe, dtpFinDe, lblAte, dtpFinAte, btnFil, btnHoje });

            // ── Cards de resumo ──
            _pnlFinCards = new FlowLayoutPanel
            {
                Dock          = DockStyle.Top,
                Height        = 108,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = true,
                AutoSize      = true,
                AutoSizeMode  = AutoSizeMode.GrowAndShrink,
                BackColor     = Color.Transparent,
                Padding       = new Padding(0, 10, 0, 6)
            };

            // ── Grid principal (receitas do dia) ──
            gridFinanceiro = CriarGrid();
            gridFinanceiro.Dock = DockStyle.Fill;
            gridFinanceiro.DataError += (_, e2) => e2.ThrowException = false;
            gridFinanceiro.Cursor = Cursors.Hand;
            gridFinanceiro.CellClick += FinanceiroDia_DblClick;

            // ── Rodapé: Gastos de Material / Insumos ──
            var pnlGastos = new Panel { Dock = DockStyle.Bottom, Height = 210, BackColor = Color.FromArgb(235, 228, 213) };

            var lblGTitle = new Label
            {
                Text      = "Gastos de Material / Insumos",
                ForeColor = Color.FromArgb(243, 156, 18),
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                Dock      = DockStyle.Top, Height = 32, Padding = new Padding(4, 8, 0, 0)
            };
            var pnlGBtn = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = Color.Transparent };
            var btnAddGasto = new Button
            {
                Text = "+ Lancar Gasto", Left = 4, Top = 4, Width = 140, Height = 28,
                BackColor = Color.FromArgb(52, 73, 94), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnAddGasto.FlatAppearance.BorderSize = 0;
            btnAddGasto.Click += (_, __) =>
            {
                using var frm = new Forms.frmCadastroGasto();
                if (frm.ShowDialog(this) == DialogResult.OK) CarregarFinanceiro();
            };
            var btnDelGasto = new Button
            {
                Text = "\u2715 Excluir", Left = 152, Top = 4, Width = 100, Height = 28,
                BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnDelGasto.FlatAppearance.BorderSize = 0;
            btnDelGasto.Click += (_, __) =>
            {
                if (_gridGastos.SelectedRows.Count == 0) return;
                var cod = Convert.ToInt32(_gridGastos.SelectedRows[0].Cells["Codigo"].Value);
                if (MessageBox.Show("Excluir este gasto?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                { _gastosBLL.Excluir(cod); CarregarFinanceiro(); }
            };
            pnlGBtn.Controls.AddRange(new Control[] { btnAddGasto, btnDelGasto });
            _gridGastos = CriarGrid();
            _gridGastos.Dock = DockStyle.Fill;
            pnlGastos.Controls.Add(_gridGastos);
            pnlGastos.Controls.Add(pnlGBtn);
            pnlGastos.Controls.Add(lblGTitle);

            lblFinResumo = new Label
            {
                Dock      = DockStyle.Bottom,
                Height    = 28,
                ForeColor = Color.FromArgb(140, 90, 25),
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(235, 228, 213),
                Padding   = new Padding(4, 6, 0, 0),
                Text      = ""
            };

            pnlFinanceiro.Controls.Add(gridFinanceiro);
            pnlFinanceiro.Controls.Add(_pnlFinCards);
            pnlFinanceiro.Controls.Add(pnlFil);
            pnlFinanceiro.Controls.Add(pnlGastos);
            pnlFinanceiro.Controls.Add(lblFinResumo);
            pnlContent.Controls.Add(pnlFinanceiro);
        }

        private Label CriarCardFin(string titulo, string valor, Color cor)
        {
            int w = 141;
            var pnl = new Panel { Width = w, Height = 84, BackColor = cor, Margin = new Padding(0, 0, 7, 0) };
            pnl.Controls.Add(new Label
            {
                Text      = titulo,
                ForeColor = Color.FromArgb(235, 215, 185),
                Font      = new Font("Segoe UI", 8F),
                AutoSize  = false, Width = w, Height = 22, Top = 8, Left = 8,
            });
            var lbl = new Label
            {
                Text      = valor,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
                AutoSize  = false, Width = w, Height = 36, Top = 30, Left = 8
            };
            pnl.Controls.Add(lbl);
            _pnlFinCards.Controls.Add(pnl);
            return lbl;
        }

        private void CarregarFinanceiro()
        {
            try
            {
                var de  = dtpFinDe.Value.Date;
                var ate = dtpFinAte.Value.Date;

                // ── Grid de receitas ──
                var dt = _pedidoBLL.GetFinanceiro(de, ate);
                gridFinanceiro.DataSource = dt;
                ConfigurarColunasFinanceiro();

                // ── Totais de vendas ──
                decimal pedidos = 0, taxaEnt = 0, totalBruto = 0, custoMerc = 0;
                decimal totalDinheiro = 0, totalCartao = 0, totalPix = 0;
                foreach (DataRow r in dt.Rows)
                {
                    decimal V(string col) => r[col] == DBNull.Value ? 0 : Convert.ToDecimal(r[col]);
                    pedidos       += V("Pedidos");
                    taxaEnt       += V("TaxaEntrega");
                    totalBruto    += V("TotalBruto");
                    custoMerc     += V("CustoMercadorias");
                    totalDinheiro += V("Dinheiro");
                    totalCartao   += V("Cartao");
                    totalPix      += V("Pix");
                }

                // ── Totais de compras (entradas de mercadoria) ──
                decimal totalCompras = _entradaBLL.TotalPeriodo(de, ate);

                decimal fatLiquido = totalBruto - custoMerc;
                decimal lucroReal  = totalBruto - totalCompras;
                decimal saidas     = custoMerc + totalCompras;

                // ── Gastos material ──
                decimal gastosMaterial = _gastosBLL.TotalPeriodo(de, ate);
                decimal lucroFinal     = totalBruto - totalCompras - gastosMaterial;

                // ── 9 cards ──
                _pnlFinCards.Controls.Clear();
                CriarCardFin("Total de Pedidos",    pedidos.ToString("N0"),    Color.FromArgb(176, 110, 42));
                CriarCardFin("Venda Bruta",          totalBruto.ToString("C"),  Color.FromArgb(115, 140, 50));
                CriarCardFin("Compras",              totalCompras.ToString("C"),Color.FromArgb(180, 70, 55));
                CriarCardFin("Outros Gastos",        gastosMaterial.ToString("C"), Color.FromArgb(160, 100, 38));
                CriarCardFin("Taxa de Entrega",      taxaEnt.ToString("C"),     Color.FromArgb(130, 100, 48));
                CriarCardFin("Venda L\u00edquida",   fatLiquido.ToString("C"),
                    fatLiquido >= 0 ? Color.FromArgb(115, 140, 50) : Color.FromArgb(180, 70, 55));
                // Movimentação por forma de pagamento
                CriarCardFin("Dinheiro",            totalDinheiro.ToString("C"), Color.FromArgb(155, 130, 48));
                CriarCardFin("Cart\u00e3o",         totalCartao.ToString("C"),   Color.FromArgb(73, 110, 160));
                CriarCardFin("Pix",                 totalPix.ToString("C"),      Color.FromArgb(80, 130, 110));

                // ── Gastos material grid ──
                _gridGastos.DataSource = _gastosBLL.Listar(de, ate);

                lblFinResumo.Text =
                    $"Período: {de:dd/MM/yyyy} a {ate:dd/MM/yyyy}  |  " +
                    $"Venda Bruta: {totalBruto:C}  |  " +
                    $"Compras: {totalCompras:C}  |  " +
                    $"Outros Gastos: {gastosMaterial:C}  |  " +
                    $"Venda L\u00edquida (Bruta - Custo Merc.): {fatLiquido:C}";
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar financeiro: " + ex.Message); }
        }

        private void ConfigurarColunasFinanceiro()
        {
            if (gridFinanceiro.Columns.Count == 0) return;
            var hide = new[] { "Subtotal", "Descontos", "ValorEntrega", "ValorRetirada" };
            foreach (var col in hide)
                if (gridFinanceiro.Columns.Contains(col))
                    gridFinanceiro.Columns[col].Visible = false;
            var captions = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Dia"]          = "Dia",
                ["Pedidos"]      = "Pedidos",
                ["TaxaEntrega"]  = "Taxa Entrega",
                ["TotalBruto"]   = "Total Bruto",
                ["Dinheiro"]     = "Dinheiro",
                ["Pix"]          = "Pix",
                ["Cartao"]       = "Cart\u00e3o",
                ["CustoMercadorias"] = "Custo Merc.",
                ["TotalLiquido"] = "Total L\u00edquido",
            };
            foreach (var kv in captions)
                if (gridFinanceiro.Columns.Contains(kv.Key))
                    gridFinanceiro.Columns[kv.Key].HeaderText = kv.Value;

            // Formato N2 para colunas decimais
            foreach (var col in new[] { "TaxaEntrega", "TotalBruto", "Dinheiro", "Pix", "Cartao", "CustoMercadorias", "TotalLiquido" })
                if (gridFinanceiro.Columns.Contains(col))
                    gridFinanceiro.Columns[col].DefaultCellStyle.Format = "N2";
        }

        private void FinanceiroDia_DblClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = gridFinanceiro.Rows[e.RowIndex];
            if (row.DataBoundItem == null) return;
            var drv = (System.Data.DataRowView)row.DataBoundItem;
            if (drv.Row["Dia"] == DBNull.Value) return;
            var dia = Convert.ToDateTime(drv.Row["Dia"]);
            try
            {
                var dt = _pedidoBLL.GetMovimentacoesDia(dia);
                using var frm = new Forms.frmMovimentacoesDia(dia, dt, _pedidoBLL.ListarItens);
                frm.ShowDialog(this);
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        // -- Autenticação ---------------------------------------------------- 
        private void BtnLogoff_Click(object sender, EventArgs e)
        {
            Logger.Log("Form1", "BtnLogoff_Click", "Logoff");
            UsuarioSessao.Encerrar();
            Application.Restart();
        }

        // Reconstroi os botoes do sidebar conforme permissoes da sessao atual
        public void ReconstruirSidebar()
        {
            // Atualizar rodapé com nome da empresa e usuário logado
            try
            {
                var emp = _empBLL.Carregar();
                string nomeEmp = !string.IsNullOrWhiteSpace(emp?.empNome_Fantasia)
                    ? emp.empNome_Fantasia
                    : emp?.empNome ?? "";
                lblFooterEmpresa.Text = "\U0001F3E2  " + nomeEmp;
            }
            catch { }
            lblFooterUsuario.Text = "\U0001F464  " + UsuarioSessao.NomeAtual;
            // Reposicionar label direito no footer
            lblFooterUsuario.Left = pnlFooter.Width - lblFooterUsuario.PreferredWidth - 12;

            // Manter apenas o primeiro controle (accent bar)
            while (pnlSidebar.Controls.Count > 1)
                pnlSidebar.Controls.RemoveAt(1);

            // -- Logo no topo do sidebar --
            var pnlLogo = new Panel { Left = 0, Top = 5, Width = 245, Height = 72, BackColor = Color.Transparent };
            var lblLogoSide = new Label
            {
                Text      = "RanGoFood",
                Font      = new Font("Segoe UI", 13, FontStyle.Bold),
                AutoSize  = true,
                Left      = 16,
                Top       = 20,
                ForeColor = Color.FromArgb(210, 185, 140)
            };
            var sepLogo = new Panel { Left = 0, Top = 66, Width = 245, Height = 1, BackColor = Color.FromArgb(40, 42, 52) };
            pnlLogo.Controls.Add(lblLogoSide);
            pnlLogo.Controls.Add(sepLogo);
            pnlSidebar.Controls.Add(pnlLogo);

            int navY = 80;

            void Secao(string titulo)
            {
                if (navY > 80) navY += 4;
                var lbl = new Label
                {
                    Text      = titulo,
                    ForeColor = Color.FromArgb(90, 82, 68),
                    Font      = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                    AutoSize  = false,
                    Left      = 14,
                    Top       = navY,
                    Width     = 186,
                    Height    = 20,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                pnlSidebar.Controls.Add(lbl);
                navY += 20;
                var sep = new Panel
                {
                    Left      = 14,
                    Top       = navY,
                    Width     = 182,
                    Height    = 1,
                    BackColor = Color.FromArgb(40, 42, 52)
                };
                pnlSidebar.Controls.Add(sep);
                navY += 5;
            }

            void NavSe(string modulo, string texto, Action acao)
            {
                if (!UsuarioSessao.TemModulo(modulo)) return;
                pnlSidebar.Controls.Add(BotaoNav(texto, navY, acao));
                navY += 44;
            }

            bool temOperacional = UsuarioSessao.TemModulo("Dashboard")
                               || UsuarioSessao.TemModulo("Pedidos")
                               || UsuarioSessao.TemModulo("Financeiro")
                               || UsuarioSessao.TemModulo("Turno");
            if (temOperacional)
            {
                Secao("OPERACIONAL");
                NavSe("Dashboard",       "\U0001F3E0  Dashboard",        MostrarDashboard);
                NavSe("Pedidos",         "\U0001F4CB  Pedidos",           MostrarPedidos);
                NavSe("ConsultarPedido", "\U0001F50D  Consultar Pedido",  () => AbrirForm(new Forms.frmConsultarPedido()));
                NavSe("Fidelizacao",     "\U0001F91D  Fidelização",         () => AbrirForm(new Forms.frmFidelizacao()));
                NavSe("WhatsApp",        "\U0001F4AC  WhatsApp",             () => AbrirForm(new Forms.frmWhatsApp()));
                NavSe("Financeiro",      "\U0001F4B0  Financeiro",        MostrarFinanceiro);
                NavSe("Turno",           "\U0001F551  Turno de Caixa",    () => AbrirForm(new Forms.frmTurno()));
            }

            bool temCadastros = UsuarioSessao.TemModulo("Produtos")
                             || UsuarioSessao.TemModulo("Categorias")
                             || UsuarioSessao.TemModulo("Clientes")
                             || UsuarioSessao.TemModulo("Fornecedores")
                             || UsuarioSessao.TemModulo("Cupons")
                             || UsuarioSessao.TemModulo("Estoque")
                             || UsuarioSessao.TemModulo("EntradaMercadoria")
                             || UsuarioSessao.TemModulo("Bairros")
                             || UsuarioSessao.TemModulo("Avisos");
            if (temCadastros)
            {
                Secao("CADASTROS");
                NavSe("Produtos",          "\U0001F6D2  Produtos",         () => AbrirForm(new frmCadastroProduto()));
                NavSe("Categorias",        "\U0001F5C2  Categorias",       () => AbrirForm(new frmCadastroCategoria()));
                NavSe("Clientes",          "\U0001F464  Clientes",         () => AbrirForm(new frmCadastroCliente()));
                NavSe("Fornecedores",      "\U0001F3ED  Fornecedores",     () => AbrirForm(new frmCadastroFornecedor()));
                NavSe("Cupons",            "\U0001F3F7  Cupons",           () => AbrirForm(new frmCadastroCupom()));
                NavSe("Estoque",           "\U0001F4E6  Estoque",           () => AbrirForm(new frmEstoque()));
                NavSe("EntradaMercadoria", "\U0001F69A  Entrada Mercad.",  () => AbrirForm(new frmEntradaMercadoria()));
                NavSe("Bairros",           "\U0001F4CD  Bairros / Taxa",   () => AbrirForm(new frmCadastroBairro()));
                NavSe("Marmitas",          "\U0001F96B  Marmitas",          () => AbrirForm(new Forms.frmCadastroMarmita()));
                NavSe("Avisos",            "\U0001F514  Avisos",           () => AbrirForm(new frmAvisos()));
            }

            if (UsuarioSessao.TemModulo("Empresa"))
            {
                Secao("CONFIGURA\u00C7\u00D5ES");
                NavSe("Empresa", "\U0001F3E2  Empresa", MostrarEmpresa);
            }
        }

        private void NavIniciarPrimeiro()
        {
            if      (UsuarioSessao.TemModulo("Dashboard"))    MostrarDashboard();
            else if (UsuarioSessao.TemModulo("Pedidos"))      MostrarPedidos();
            else if (UsuarioSessao.TemModulo("Financeiro"))   MostrarFinanceiro();
            else if (UsuarioSessao.TemModulo("Produtos"))     AbrirForm(new frmCadastroProduto());
            else if (UsuarioSessao.TemModulo("Categorias"))   AbrirForm(new frmCadastroCategoria());
            else if (UsuarioSessao.TemModulo("Clientes"))     AbrirForm(new frmCadastroCliente());
            else if (UsuarioSessao.TemModulo("Fornecedores")) AbrirForm(new frmCadastroFornecedor());
            else if (UsuarioSessao.TemModulo("Cupons"))       AbrirForm(new frmCadastroCupom());
            else if (UsuarioSessao.TemModulo("Empresa"))      MostrarEmpresa();
        }
    }
}


