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
        private BLL.EmpresaBLL                   _empBLL;
        private BLL.ConfiguracaoImpressaoBLL     _impBLL;

        private int  _paginaAtual = 0; // 0=Dashboard 1=Pedidos 2=Financeiro

        // -- Cores ------------------------------------------------------------
        private static readonly Color CorSidebar    = Color.FromArgb(28, 37, 65);
        private static readonly Color CorTopBar     = Color.FromArgb(36, 48, 82);
        private static readonly Color CorBotaoAtivo = Color.FromArgb(52, 152, 219);
        private static readonly Color CorCard       = Color.FromArgb(44, 55, 95);
        private static readonly Color CorFundo      = Color.FromArgb(15, 22, 45);

        public Form1()
        {
            InitializeComponent();
            _pedidoBLL = new PedidoBLL();
            _dashBLL   = new DashboardBLL();
            _gastosBLL = new GastoMaterialBLL();
            _empBLL    = new BLL.EmpresaBLL();
            _impBLL    = new BLL.ConfiguracaoImpressaoBLL();
            BuildDashboard();
            BuildPedidos();
            BuildFinanceiro();
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
                ForeColor = Color.FromArgb(200, 210, 240),
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(16, 0, 0, 0),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize       = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 60, 100);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(52, 152, 219);
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

            lblPedidosHoje  = CriarCard(pnlCards, "Pedidos Hoje",     "0",       Color.FromArgb(41, 128, 185));
            lblFaturamento  = CriarCard(pnlCards, "Faturamento Hoje", "R$ 0,00", Color.FromArgb(39, 174, 96));
            lblClientes     = CriarCard(pnlCards, "Total Clientes",   "0",       Color.FromArgb(142, 68, 173));
            lblPendentes    = CriarCard(pnlCards, "Pedidos Pendentes","0",       Color.FromArgb(211, 84, 0));

            // ── Barra de filtro de período ─────────────────────────────────────
            // ── Área de gráficos ──
            Panel  MkOuter() => new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(13, 24, 46) };
            Panel  MkFiltro() => new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.FromArgb(22, 30, 55) };
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
            var filtC  = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.FromArgb(22, 30, 55) };
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
                    BackColor = idx == 0 ? CorBotaoAtivo : Color.FromArgb(40, 55, 95),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor    = Cursors.Hand,
                    Font      = new Font("Segoe UI", 8f)
                };
                bp.FlatAppearance.BorderSize = 0;
                bp.Click += (_, __) =>
                {
                    _periodoCanal = per;
                    foreach (var b in _btnsPeriodo) b.BackColor = Color.FromArgb(40, 55, 95);
                    bp.BackColor = CorBotaoAtivo;
                    CarregarChartCanal();
                };
                _btnsPeriodo[pi] = bp;
                filtC.Controls.Add(bp);
            }
            _pnlChartCanal = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(13, 24, 46), Tag = "Vendas por Canal" };
            _pnlChartCanal.Paint += (s, e) => DesenharBarrasVerticais(e.Graphics, (Panel)s, _dadosCanal, Color.FromArgb(245, 175, 35));
            outerC.Controls.Add(_pnlChartCanal); outerC.Controls.Add(filtC);
            outerC.Controls.Add(new Panel { Height = 4, Dock = DockStyle.Top, BackColor = Color.FromArgb(41, 128, 185) });

            // Gráfico 2: Top 3 Produtos (Diário / Semanal / Mensal / Anual)
            var outerP = MkOuter();
            var filtP  = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.FromArgb(22, 30, 55) };
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
                    BackColor = idx == 0 ? CorBotaoAtivo : Color.FromArgb(40, 55, 95),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor    = Cursors.Hand,
                    Font      = new Font("Segoe UI", 8f)
                };
                bp.FlatAppearance.BorderSize = 0;
                bp.Click += (_, __) =>
                {
                    _periodoProd = per;
                    foreach (var b in _btnsProdPeriodo) b.BackColor = Color.FromArgb(40, 55, 95);
                    bp.BackColor = CorBotaoAtivo;
                    CarregarChartProdutos();
                };
                _btnsProdPeriodo[pi] = bp;
                filtP.Controls.Add(bp);
            }
            _pnlChartProdutos = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(13, 24, 46), Tag = "Top 3 Produtos" };
            _pnlChartProdutos.Paint += (s, e) => DesenharBarrasVerticais(e.Graphics, (Panel)s, _dadosProdutos, Color.FromArgb(245, 175, 35), "0");
            outerP.Controls.Add(_pnlChartProdutos); outerP.Controls.Add(filtP);
            outerP.Controls.Add(new Panel { Height = 4, Dock = DockStyle.Top, BackColor = Color.FromArgb(142, 68, 173) });

            // Gráfico 3: Receita por Dia (Diário / Semanal / Mensal / Anual)
            var outerD = MkOuter();
            var filtD  = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.FromArgb(22, 30, 55) };
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
                    BackColor = idx == 0 ? CorBotaoAtivo : Color.FromArgb(40, 55, 95),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor    = Cursors.Hand,
                    Font      = new Font("Segoe UI", 8f)
                };
                bp.FlatAppearance.BorderSize = 0;
                bp.Click += (_, __) =>
                {
                    _periodoDias = per;
                    foreach (var b in _btnsDiasPeriodo) b.BackColor = Color.FromArgb(40, 55, 95);
                    bp.BackColor = CorBotaoAtivo;
                    CarregarChartDias();
                };
                _btnsDiasPeriodo[pi] = bp;
                filtD.Controls.Add(bp);
            }
            _pnlChartDias = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(13, 24, 46), Tag = "Receita por Dia" };
            _pnlChartDias.Paint += (s, e) => DesenharBarrasVerticais(e.Graphics, (Panel)s, _dadosDias, Color.FromArgb(245, 175, 35));
            outerD.Controls.Add(_pnlChartDias); outerD.Controls.Add(filtD);
            outerD.Controls.Add(new Panel { Height = 4, Dock = DockStyle.Top, BackColor = Color.FromArgb(39, 174, 96) });

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

            using var bgBrush    = new SolidBrush(Color.FromArgb(13, 24, 46));
            using var titleFont  = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var labelFont  = new Font("Segoe UI", 7f);
            using var valFont    = new Font("Segoe UI", 7f, FontStyle.Bold);
            using var whiteBrush = new SolidBrush(Color.White);
            using var grayBrush  = new SolidBrush(Color.FromArgb(140, 165, 205));
            using var barBrush   = new SolidBrush(corBarra);
            using var gridPen    = new System.Drawing.Pen(Color.FromArgb(30, 50, 90), 1f);
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
                string valStr = data[i].value >= 1000
                    ? $"{data[i].value / 1000:0.00}k"
                    : data[i].value.ToString(valueFormat);
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

            using var bgBrush    = new SolidBrush(Color.FromArgb(13, 24, 46));
            using var titleFont  = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var labelFont  = new Font("Segoe UI", 7.5f);
            using var valFont    = new Font("Segoe UI", 7f, FontStyle.Bold);
            using var whiteBrush = new SolidBrush(Color.White);
            using var grayBrush  = new SolidBrush(Color.FromArgb(140, 165, 205));
            using var barBrush   = new SolidBrush(corBarra);
            using var gridPen    = new System.Drawing.Pen(Color.FromArgb(30, 50, 90), 1f);
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

            using var bgBrush    = new SolidBrush(Color.FromArgb(13, 24, 46));
            using var titleFont  = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var labelFont  = new Font("Segoe UI", 7f);
            using var whiteBrush = new SolidBrush(Color.White);
            using var grayBrush  = new SolidBrush(Color.FromArgb(140, 165, 205));
            using var linePen    = new System.Drawing.Pen(corLinha, 2.5f);
            using var dotBrush   = new SolidBrush(corLinha);
            using var gridPen    = new System.Drawing.Pen(Color.FromArgb(30, 50, 90), 1f);
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
                    ? d.ToString("MM/dd")
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
                _dadosCanal = DataTableParaChart(
                    _dashBLL.GetVendasPorPeriodo(_periodoCanal), "Periodo", "TotalVendas");
                _pnlChartCanal.Invalidate();
            }
            catch { }
        }

        private void CarregarChartProdutos()
        {
            if (_pnlChartProdutos == null) return;
            try
            {
                _dadosProdutos = DataTableParaChart(
                    _dashBLL.GetTopProdutosPeriodo(_periodoProd, 3), "Produto", "Quantidade");
                _pnlChartProdutos.Invalidate();
            }
            catch { }
        }

        private void CarregarChartDias()
        {
            if (_pnlChartDias == null) return;
            try
            {
                _dadosDias = DataTableParaChart(
                    _dashBLL.GetVendasPorPeriodo(_periodoDias), "Periodo", "TotalVendas");
                _pnlChartDias.Invalidate();
            }
            catch { }
        }

        private Label CriarCard(FlowLayoutPanel pai, string titulo, string valor, Color cor)
        {
            int w = 230;
            var pnl = new Panel
            {
                Width     = w,
                Height    = 110,
                BackColor = cor,
                Margin    = new Padding(0, 0, 16, 0)
            };

            pnl.Controls.Add(new Label
            {
                Text      = titulo,
                ForeColor = Color.FromArgb(220, 230, 255),
                Font      = new Font("Segoe UI", 9),
                AutoSize  = false,
                Width     = w,
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
                Width     = w,
                Height    = 50,
                Top       = 40,
                Left      = 14,
                TextAlign = ContentAlignment.TopLeft
            };
            pnl.Controls.Add(lblVal);
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

            var lblSit = new Label { Text = "Situacao:", ForeColor = Color.White, Left = 0, Top = 14, AutoSize = true };
            cmbFiltroPedido = new ComboBox
            {
                Left          = 68,
                Top           = 10,
                Width         = 160,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFiltroPedido.Items.AddRange(new object[] { "Todos", "Em Preparo", "Finalizados", "Cancelados" });
            cmbFiltroPedido.SelectedIndex = 0;

            var lblDt = new Label { Text = "Data:", ForeColor = Color.White, Left = 244, Top = 14, AutoSize = true };
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
                BackColor = Color.FromArgb(80, 90, 130),
                ForeColor = Color.White,
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

            pnlFiltros.Controls.AddRange(new System.Windows.Forms.Control[]
                { lblSit, cmbFiltroPedido, lblDt, dtpFiltroPedido, btnFiltrar, btnTodos, btnManual });

            // Detalhe header
            lblDetalhe = new Label
            {
                Text      = "Itens do Pedido",
                ForeColor = Color.White,
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
            _btnSaiu      = MkBtn("\U0001F6B4 Saiu p/ Entrega",Color.FromArgb(52, 152, 219), 4);
            _btnEntregue  = MkBtn("\U0001F4E6 Entregue",       Color.FromArgb(22, 160, 133), 5);
            _btnCancelar  = MkBtn("\u2715 Cancelar",           Color.FromArgb(192, 57, 43),  6);

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
                BackgroundColor        = Color.FromArgb(28, 37, 65),
                GridColor              = Color.FromArgb(50, 60, 100),
                DefaultCellStyle       = { BackColor = Color.FromArgb(28, 37, 65), ForeColor = Color.White, SelectionBackColor = CorBotaoAtivo, SelectionForeColor = Color.White },
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(36, 48, 82) },
                ColumnHeadersDefaultCellStyle   = { BackColor = CorTopBar, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) },
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
            try
            {
                string filtro = null;
                if (cmbFiltroPedido?.SelectedIndex > 0)
                {
                    var sel = cmbFiltroPedido.SelectedItem?.ToString() ?? "";
                    if      (sel == "Em Preparo")  filtro = "emPreparo";
                    else if (sel == "Finalizados") filtro = "finalizados";
                    else if (sel == "Cancelados")  filtro = "cancelados";
                }
                DateTime? dt = dtpFiltroPedido?.Value.Date;
                gridPedidos.DataSource = _pedidoBLL.Listar(filtro, dt);
                AjustarColunasPedidos();
                gridItens.DataSource   = null;
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar pedidos: " + ex.Message); }
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
                ["Numero"]   = "Nº Pedido",
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
                _btnEntregue.Visible  = !retirada && !terminal;
                _btnCancelar.Visible  = !terminal;
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
                else CarregarPedidos();
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
                    if (MostrarDialogPagamento(pedido, out decimal vPago, out string trans))
                    {
                        var eF = _pedidoBLL.FinalizarPedido(cod, novaSit, vPago, trans);
                        if (!string.IsNullOrEmpty(eF)) MessageBox.Show("Erro: " + eF);
                        else CarregarPedidos();
                    }
                    return;
                }
            }

            var erro = _pedidoBLL.AtualizarSituacao(cod, novaSit);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }

            // Ao confirmar (situação 1), imprimir duas vias do cupom
            if (novaSit == 1)
                ImprimirCupomPedido(cod);

            CarregarPedidos();
        }

        private void ImprimirCupomPedido(int codigoPedido)
        {
            try
            {
                var pedido  = _pedidoBLL.PesquisaCodigo(codigoPedido);
                if (pedido == null) return;
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

        private bool MostrarDialogPagamento(PedidoWeb pedido, out decimal valorPago, out string transacao)
        {
            valorPago = pedido.pediValor_Total;
            transacao  = "";
            bool needsTrans = pedido.pediForma_Pagamento > 0; // Cartao ou Pix

            using var frm = new Form();
            frm.Text             = "Finalizar Pedido - Pagamento";
            frm.StartPosition    = FormStartPosition.CenterParent;
            frm.FormBorderStyle  = FormBorderStyle.FixedDialog;
            frm.MaximizeBox      = frm.MinimizeBox = false;
            frm.BackColor        = Color.FromArgb(36, 48, 82);
            frm.ForeColor        = Color.White;
            frm.Font             = new Font("Segoe UI", 9F);
            frm.ClientSize       = new Size(390, needsTrans ? 158 : 110);

            var lblV = new Label { Text = "Valor pago (R$):", Left = 12, Top = 18, AutoSize = true, ForeColor = Color.White };
            var numV = new NumericUpDown { Left = 150, Top = 14, Width = 130, DecimalPlaces = 2, Maximum = 99999M, Value = pedido.pediValor_Total };

            string lblTrans = pedido.pediForma_Pagamento == 2 ? "Codigo Pix:" : "Cod. Transacao:";
            var lblT = new Label { Text = lblTrans, Left = 12, Top = 56, AutoSize = true, ForeColor = Color.White, Visible = needsTrans };
            var txtT = new TextBox { Left = 150, Top = 52, Width = 220, Visible = needsTrans };

            int btnTop = needsTrans ? 108 : 64;
            var btnOk  = new Button { Text = "\u2714 Confirmar", Left = 100, Top = btnTop, Width = 130, Height = 28, DialogResult = DialogResult.OK,  BackColor = Color.FromArgb(39, 174, 96),   ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnCan = new Button { Text = "Cancelar",     Left = 242, Top = btnTop, Width = 90,  Height = 28, DialogResult = DialogResult.Cancel, BackColor = Color.FromArgb(108,117,125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = btnCan.FlatAppearance.BorderSize = 0;

            frm.Controls.AddRange(new Control[] { lblV, numV, lblT, txtT, btnOk, btnCan });
            frm.AcceptButton = btnOk; frm.CancelButton = btnCan;

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                valorPago = numV.Value;
                transacao  = txtT.Text.Trim();

                // Desconto detectado: requer autorização de usuário com permissão
                if (valorPago < pedido.pediValor_Total)
                {
                    decimal desconto = pedido.pediValor_Total - valorPago;
                    MessageBox.Show(
                        $"Valor pago (R$ {valorPago:N2}) é menor que o total (R$ {pedido.pediValor_Total:N2}).\n" +
                        $"Desconto de R$ {desconto:N2} requer autorização.",
                        "Autorização Necessária",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    using var dlgAuth = new Forms.frmAutorizacao();
                    if (dlgAuth.ShowDialog(this) != DialogResult.OK)
                        return false; // autorização negada ou cancelada
                }

                return true;
            }
            return false;
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
            var lblDe  = new Label { Text = "De:",  ForeColor = Color.White, Left = 0,   Top = 14, AutoSize = true };
            dtpFinDe   = new DateTimePicker { Left = 32,  Top = 10, Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-30) };
            var lblAte = new Label { Text = "Ate:", ForeColor = Color.White, Left = 164, Top = 14, AutoSize = true };
            dtpFinAte  = new DateTimePicker { Left = 198, Top = 10, Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            var btnFil = new Button { Text = "Filtrar", Left = 332, Top = 8, Width = 80, Height = 28, BackColor = CorBotaoAtivo, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnFil.FlatAppearance.BorderSize = 0;
            btnFil.Click += (_, __) => CarregarFinanceiro();
            pnlFil.Controls.AddRange(new Control[] { lblDe, dtpFinDe, lblAte, dtpFinAte, btnFil });

            // ── Cards de resumo ──
            _pnlFinCards = new FlowLayoutPanel
            {
                Dock          = DockStyle.Top,
                Height        = 100,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = false,
                BackColor     = Color.Transparent,
                Padding       = new Padding(0, 8, 0, 8)
            };

            // ── Grid principal (receitas do dia) ──
            gridFinanceiro = CriarGrid();
            gridFinanceiro.Dock = DockStyle.Fill;
            gridFinanceiro.DataError += (_, e2) => e2.ThrowException = false;
            gridFinanceiro.Cursor = Cursors.Hand;
            gridFinanceiro.CellDoubleClick += FinanceiroDia_DblClick;

            // ── Rodapé: Gastos de Material / Insumos ──
            var pnlGastos = new Panel { Dock = DockStyle.Bottom, Height = 210, BackColor = Color.FromArgb(22, 30, 55) };

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
                ForeColor = Color.FromArgb(243, 156, 18),
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(22, 30, 55),
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
            int w = 160;
            var pnl = new Panel { Width = w, Height = 80, BackColor = cor, Margin = new Padding(0, 0, 12, 0) };
            pnl.Controls.Add(new Label
            {
                Text      = titulo,
                ForeColor = Color.FromArgb(220, 230, 255),
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
                foreach (DataRow r in dt.Rows)
                {
                    decimal V(string col) => r[col] == DBNull.Value ? 0 : Convert.ToDecimal(r[col]);
                    pedidos    += V("Pedidos");
                    taxaEnt    += V("TaxaEntrega");
                    totalBruto += V("TotalBruto");
                    custoMerc  += V("CustoMercadorias");
                }

                // ── Totais de compras (entradas de mercadoria) ──
                var dtCompras = _pedidoBLL.GetComprasPorDia(de, ate);
                decimal totalCompras = 0;
                foreach (DataRow r in dtCompras.Rows)
                    if (r["TotalCompras"] != DBNull.Value) totalCompras += Convert.ToDecimal(r["TotalCompras"]);

                decimal fatLiquido = totalBruto - custoMerc;
                decimal lucroReal  = totalBruto - totalCompras;
                decimal saidas     = custoMerc + totalCompras;

                // ── Gastos material ──
                decimal gastosMaterial = _gastosBLL.TotalPeriodo(de, ate);
                decimal lucroFinal     = totalBruto - totalCompras - gastosMaterial;

                // ── 6 cards ──
                _pnlFinCards.Controls.Clear();
                CriarCardFin("Total de Pedidos",    pedidos.ToString("N0"),    Color.FromArgb(41,  128, 185));
                CriarCardFin("Entradas (Vendas)",   totalBruto.ToString("C"),  Color.FromArgb(39,  174,  96));
                CriarCardFin("Compras/Entradas",    totalCompras.ToString("C"),Color.FromArgb(192,  57,  43));
                CriarCardFin("Gastos Material",     gastosMaterial.ToString("C"), Color.FromArgb(165, 105, 18));
                CriarCardFin("Taxa de Entrega",     taxaEnt.ToString("C"),     Color.FromArgb(22,  160, 133));
                CriarCardFin("Lucro Estimado",      lucroFinal.ToString("C"),
                    lucroFinal >= 0 ? Color.FromArgb(52, 152, 219) : Color.FromArgb(192, 57, 43));

                // ── Gastos material grid ──
                _gridGastos.DataSource = _gastosBLL.Listar(de, ate);

                lblFinResumo.Text =
                    $"Período: {de:dd/MM/yyyy} a {ate:dd/MM/yyyy}  |  " +
                    $"Entradas: {totalBruto:C}  |  " +
                    $"Compras: {totalCompras:C}  |  " +
                    $"Gastos: {gastosMaterial:C}  |  " +
                    $"Lucro estimado: {lucroFinal:C}  |  " +
                    $"Fat. Líquido (- custo merc.): {fatLiquido:C}";
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar financeiro: " + ex.Message); }
        }

        private void ConfigurarColunasFinanceiro()
        {
            if (gridFinanceiro.Columns.Count == 0) return;
            var hide = new[] { "Subtotal", "Descontos", "ValorEntrega", "ValorRetirada", "Dinheiro" };
            foreach (var col in hide)
                if (gridFinanceiro.Columns.Contains(col))
                    gridFinanceiro.Columns[col].Visible = false;
            var captions = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Dia"]          = "Dia",
                ["Pedidos"]      = "Pedidos",
                ["TaxaEntrega"]  = "Taxa Entrega",
                ["TotalBruto"]   = "Total Bruto",
                ["Pix"]          = "Pix",
                ["Cartao"]       = "Cartão",
                ["CustoMercadorias"] = "Custo Merc.",
                ["TotalLiquido"] = "Total Líquido",
            };
            foreach (var kv in captions)
                if (gridFinanceiro.Columns.Contains(kv.Key))
                    gridFinanceiro.Columns[kv.Key].HeaderText = kv.Value;
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
                using var frm = new Forms.frmMovimentacoesDia(dia, dt);
                frm.ShowDialog(this);
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        // -- Autenticação ---------------------------------------------------- 
        private void BtnLogoff_Click(object sender, EventArgs e)
        {
            UsuarioSessao.Encerrar();
            Application.Restart();
        }

        // Reconstroi os botoes do sidebar conforme permissoes da sessao atual
        public void ReconstruirSidebar()
        {
            // Manter apenas o primeiro controle (accent bar)
            while (pnlSidebar.Controls.Count > 1)
                pnlSidebar.Controls.RemoveAt(1);

            int navY = 8;
            void NavSe(string modulo, string texto, Action acao)
            {
                if (!UsuarioSessao.TemModulo(modulo)) return;
                pnlSidebar.Controls.Add(BotaoNav(texto, navY, acao));
                navY += 46;
            }
            NavSe("Dashboard",    "\U0001F3E0  Dashboard",    MostrarDashboard);
            NavSe("Pedidos",      "\U0001F4CB  Pedidos",      MostrarPedidos);
            NavSe("Financeiro",   "\U0001F4B0  Financeiro",   MostrarFinanceiro);
            NavSe("Produtos",     "\U0001F6D2  Produtos",     () => AbrirForm(new frmCadastroProduto()));
            NavSe("Categorias",   "\U0001F5C2  Categorias",   () => AbrirForm(new frmCadastroCategoria()));
            NavSe("Clientes",     "\U0001F464  Clientes",     () => AbrirForm(new frmCadastroCliente()));
            NavSe("Fornecedores",    "\U0001F3ED  Fornecedores",    () => AbrirForm(new frmCadastroFornecedor()));
            NavSe("Cupons",          "\U0001F3F7  Cupons",          () => AbrirForm(new frmCadastroCupom()));
            NavSe("EntradaMercadoria", "\U0001F4E6  Entrada Mercad.", () => AbrirForm(new frmEntradaMercadoria()));
            NavSe("Avisos",           "\U0001F514  Avisos",          () => AbrirForm(new frmAvisos()));
            NavSe("Empresa",         "\U0001F3E2  Empresa",         MostrarEmpresa);
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

