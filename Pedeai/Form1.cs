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
        private readonly PedidoBLL        _pedidoBLL   = new PedidoBLL();
        private readonly DashboardBLL     _dashBLL     = new DashboardBLL();
        private readonly GastoMaterialBLL _gastosBLL   = new GastoMaterialBLL();

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
            if (!DesignMode)
            {
                BuildDashboard();
                BuildPedidos();
                BuildFinanceiro();
            }
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
        private DateTimePicker  _dtpChartDe;
        private DateTimePicker  _dtpChartAte;
        private (string label, float value)[] _dadosCanal    = Array.Empty<(string, float)>();
        private (string label, float value)[] _dadosProdutos = Array.Empty<(string, float)>();
        private (string label, float value)[] _dadosDias     = Array.Empty<(string, float)>();

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
            var pnlFiltroChart = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 40,
                BackColor = Color.Transparent
            };
            var lblDe = new Label { Text = "Período:", ForeColor = Color.FromArgb(200, 210, 240), Left = 0, Top = 11, AutoSize = true };
            _dtpChartDe = new DateTimePicker { Left = 66, Top = 6, Width = 115, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-30) };
            var lblAte = new Label { Text = "até", ForeColor = Color.FromArgb(200, 210, 240), Left = 192, Top = 11, AutoSize = true };
            _dtpChartAte = new DateTimePicker { Left = 216, Top = 6, Width = 115, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            var btnFiltChart = new Button
            {
                Text = "Filtrar", Left = 342, Top = 5, Width = 72, Height = 28,
                BackColor = CorBotaoAtivo, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnFiltChart.FlatAppearance.BorderSize = 0;
            btnFiltChart.Click += (_, __) => CarregarCharts();
            pnlFiltroChart.Controls.AddRange(new Control[] { lblDe, _dtpChartDe, lblAte, _dtpChartAte, btnFiltChart });

            // ── Área de gráficos (3 painéis lado a lado) ─────────────────────
            var tbl = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 3,
                RowCount    = 1,
                BackColor   = Color.Transparent,
                Padding     = new Padding(0, 8, 0, 0)
            };
            tbl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.3f));
            tbl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.4f));
            tbl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.3f));
            tbl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));

            _pnlChartCanal    = CriarPainelGrafico("Vendas por Canal", Color.FromArgb(41, 128, 185));
            _pnlChartProdutos = CriarPainelGrafico("Top Produtos",     Color.FromArgb(142, 68, 173));
            _pnlChartDias     = CriarPainelGrafico("Receita por Dia",  Color.FromArgb(39, 174, 96));

            _pnlChartCanal.Paint    += (s, e) => DesenharBarrasVerticais(e.Graphics, (Panel)s, _dadosCanal,    Color.FromArgb(52, 152, 219));
            _pnlChartProdutos.Paint += (s, e) => DesenharBarrasHorizontais(e.Graphics, (Panel)s, _dadosProdutos, Color.FromArgb(155, 89, 182));
            _pnlChartDias.Paint     += (s, e) => DesenharLinha(e.Graphics, (Panel)s, _dadosDias,    Color.FromArgb(46, 204, 113));

            tbl.Controls.Add(_pnlChartCanal, 0, 0);
            tbl.Controls.Add(_pnlChartProdutos, 1, 0);
            tbl.Controls.Add(_pnlChartDias, 2, 0);

            pnlDashboard.Controls.Add(tbl);
            pnlDashboard.Controls.Add(pnlFiltroChart);
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

        private static void DesenharBarrasVerticais(Graphics g, Panel pnl, (string label, float value)[] data, Color corBarra)
        {
            string titulo = pnl.Tag?.ToString() ?? "";
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int W = pnl.Width, H = pnl.Height;

            using var bgBrush    = new SolidBrush(Color.FromArgb(28, 37, 65));
            using var titleFont  = new Font("Segoe UI", 9f, FontStyle.Bold);
            using var labelFont  = new Font("Segoe UI", 7.5f);
            using var whiteBrush = new SolidBrush(Color.White);
            using var grayBrush  = new SolidBrush(Color.FromArgb(140, 160, 200));
            using var barBrush   = new SolidBrush(corBarra);

            g.FillRectangle(bgBrush, 0, 0, W, H);
            g.DrawString(titulo, titleFont, whiteBrush, 10f, 12f);

            if (data == null || data.Length == 0)
            {
                g.DrawString("Sem dados para o período", labelFont, grayBrush, 10f, 40f);
                return;
            }

            float maxV = 0; foreach (var d in data) if (d.value > maxV) maxV = d.value;
            if (maxV <= 0) maxV = 1;

            float chartTop = 38f, chartBottom = H - 36f, chartLeft = 10f, chartRight = W - 10f;
            float chartH = chartBottom - chartTop;
            float slotW = (chartRight - chartLeft) / data.Length;
            float barW  = Math.Max(slotW * 0.55f, 8f);

            for (int i = 0; i < data.Length; i++)
            {
                float x    = chartLeft + i * slotW + (slotW - barW) / 2f;
                float barH = (data[i].value / maxV) * chartH;
                float barY = chartBottom - barH;
                using var br = new SolidBrush(Color.FromArgb(200, corBarra));
                g.FillRectangle(br, x, barY, barW, barH);

                // valor acima da barra
                string valStr = data[i].value >= 1000
                    ? $"R${data[i].value / 1000:0.0}k"
                    : $"R${data[i].value:0}";
                g.DrawString(valStr, labelFont, whiteBrush, x, barY - 16f);

                // label abaixo
                string lbl = data[i].label.Length > 7 ? data[i].label[..7] : data[i].label;
                g.DrawString(lbl, labelFont, grayBrush, x, chartBottom + 4f);
            }
        }

        private static void DesenharBarrasHorizontais(Graphics g, Panel pnl, (string label, float value)[] data, Color corBarra)
        {
            string titulo = pnl.Tag?.ToString() ?? "";
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int W = pnl.Width, H = pnl.Height;

            using var bgBrush    = new SolidBrush(Color.FromArgb(28, 37, 65));
            using var titleFont  = new Font("Segoe UI", 9f, FontStyle.Bold);
            using var labelFont  = new Font("Segoe UI", 7.5f);
            using var whiteBrush = new SolidBrush(Color.White);
            using var grayBrush  = new SolidBrush(Color.FromArgb(140, 160, 200));

            g.FillRectangle(bgBrush, 0, 0, W, H);
            g.DrawString(titulo, titleFont, whiteBrush, 10f, 12f);

            if (data == null || data.Length == 0)
            {
                g.DrawString("Sem dados para o período", labelFont, grayBrush, 10f, 40f);
                return;
            }

            float maxV = 0; foreach (var d in data) if (d.value > maxV) maxV = d.value;
            if (maxV <= 0) maxV = 1;

            float chartTop = 38f, chartLeft = 90f, chartRight = W - 50f, chartBottom = H - 10f;
            int maxItems = Math.Min(data.Length, 8);
            float slotH = (chartBottom - chartTop) / maxItems;
            float barH  = Math.Max(slotH * 0.55f, 6f);

            for (int i = 0; i < maxItems; i++)
            {
                float y       = chartTop + i * slotH + (slotH - barH) / 2f;
                float barW    = (data[i].value / maxV) * (chartRight - chartLeft);
                using var br = new SolidBrush(Color.FromArgb(200, corBarra));
                g.FillRectangle(br, chartLeft, y, barW, barH);

                // nome à esquerda
                string lbl = data[i].label.Length > 11 ? data[i].label[..11] : data[i].label;
                g.DrawString(lbl, labelFont, grayBrush, 4f, y);

                // valor à direita da barra
                string valStr = data[i].value.ToString("0");
                g.DrawString(valStr, labelFont, whiteBrush, chartLeft + barW + 4f, y);
            }
        }

        private static void DesenharLinha(Graphics g, Panel pnl, (string label, float value)[] data, Color corLinha)
        {
            string titulo = pnl.Tag?.ToString() ?? "";
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int W = pnl.Width, H = pnl.Height;

            using var bgBrush    = new SolidBrush(Color.FromArgb(28, 37, 65));
            using var titleFont  = new Font("Segoe UI", 9f, FontStyle.Bold);
            using var labelFont  = new Font("Segoe UI", 7f);
            using var whiteBrush = new SolidBrush(Color.White);
            using var grayBrush  = new SolidBrush(Color.FromArgb(140, 160, 200));
            using var linePen    = new System.Drawing.Pen(corLinha, 2f);
            using var dotBrush   = new SolidBrush(corLinha);

            g.FillRectangle(bgBrush, 0, 0, W, H);
            g.DrawString(titulo, titleFont, whiteBrush, 10f, 12f);

            if (data == null || data.Length < 2)
            {
                if (data != null && data.Length == 1)
                    g.DrawString($"{data[0].label}: R${data[0].value:N0}", labelFont, grayBrush, 10f, 40f);
                else
                    g.DrawString("Sem dados para o período", labelFont, grayBrush, 10f, 40f);
                return;
            }

            float maxV = 0; foreach (var d in data) if (d.value > maxV) maxV = d.value;
            if (maxV <= 0) maxV = 1;

            float chartTop = 38f, chartBottom = H - 36f, chartLeft = 10f, chartRight = W - 10f;
            float chartH = chartBottom - chartTop;
            float stepX  = (chartRight - chartLeft) / (data.Length - 1);

            var pts = new System.Drawing.PointF[data.Length];
            for (int i = 0; i < data.Length; i++)
                pts[i] = new System.Drawing.PointF(chartLeft + i * stepX, chartBottom - (data[i].value / maxV) * chartH);

            // Área preenchida abaixo da linha
            var polyPts = new System.Drawing.PointF[data.Length + 2];
            polyPts[0] = new System.Drawing.PointF(pts[0].X, chartBottom);
            for (int i = 0; i < pts.Length; i++) polyPts[i + 1] = pts[i];
            polyPts[polyPts.Length - 1] = new System.Drawing.PointF(pts[pts.Length - 1].X, chartBottom);
            using var fillBrush = new SolidBrush(Color.FromArgb(40, corLinha));
            g.FillPolygon(fillBrush, polyPts);

            g.DrawLines(linePen, pts);

            // Pontos e labels
            int step = data.Length > 14 ? (int)Math.Ceiling(data.Length / 14.0) : 1;
            for (int i = 0; i < data.Length; i++)
            {
                g.FillEllipse(dotBrush, pts[i].X - 3, pts[i].Y - 3, 6, 6);
                if (i % step == 0)
                {
                    // trim date: show MM/dd
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

        private void CarregarCharts()
        {
            if (_pnlChartCanal == null) return;
            var de  = _dtpChartDe?.Value.Date  ?? DateTime.Today.AddDays(-30);
            var ate = _dtpChartAte?.Value.Date ?? DateTime.Today;
            try
            {
                var dtCanal = _dashBLL.GetVendasPorCanal(de, ate);
                _dadosCanal = DataTableParaChart(dtCanal, "Canal", "TotalVendas");
                _pnlChartCanal.Invalidate();

                var dtProd = _dashBLL.GetTopProdutos(de, ate);
                _dadosProdutos = DataTableParaChart(dtProd, "Produto", "Quantidade");
                _pnlChartProdutos.Invalidate();

                var dtDias = _dashBLL.GetVendasPorDia(de, ate);
                _dadosDias = DataTableParaChart(dtDias, "Dia", "TotalVendas", isDate: true);
                _pnlChartDias.Invalidate();
            }
            catch { /* ignore chart load errors silently */ }
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
            var acoes = new[]
            {
                ("\u2713 Confirmar",    1, Color.FromArgb(39, 174, 96)),
                ("\u23F3 Em Preparo",  2, Color.FromArgb(243, 156, 18)),
                ("\u2705 Pronto",      3, Color.FromArgb(22, 160, 133)),
                ("\U0001F6B4 Saiu",    4, Color.FromArgb(52, 152, 219)),
                ("\U0001F4E6 Entregue",5, Color.FromArgb(41, 128, 185)),
                ("\u2715 Cancelar",    6, Color.FromArgb(192, 57, 43)),
            };
            foreach (var (txt, sit, cor) in acoes)
            {
                int s = sit;
                var b = new Button
                {
                    Text      = txt,
                    Width     = 110,
                    Height    = 30,
                    BackColor = cor,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Margin    = new Padding(0, 4, 8, 0),
                    Cursor    = Cursors.Hand
                };
                b.FlatAppearance.BorderSize = 0;
                b.Click += (_, __) => AtualizarSituacaoPedido(s);
                pnlAcoes.Controls.Add(b);
            }

            // Main grid
            var gridMain = CriarGrid();
            gridMain.Dock = DockStyle.Fill;
            gridMain.SelectionChanged += (_, __) => CarregarItensPedido(gridMain);
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

            // Cancelar
            if (novaSit == 6)
            {
                // Confirmado ou superior requer autorizacao gerencial
                bool precisaAuth = PedidoBLL.CancelamentoRequerAutorizacao(pedido.pediSituacao);
                if (precisaAuth && !UsuarioSessao.TemNivel(2))
                {
                    // Solicita credenciais de gerente
                    using var dlgAuth = new Forms.frmAutorizacao();
                    if (dlgAuth.ShowDialog(this) != DialogResult.OK) return;
                    string canceladoPor = dlgAuth.UsuarioAutorizador.usuNome;
                    var eA = _pedidoBLL.AtualizarSituacao(cod, 6, canceladoPor);
                    if (!string.IsNullOrEmpty(eA)) MessageBox.Show("Erro: " + eA);
                    else CarregarPedidos();
                    return;
                }

                if (MessageBox.Show("Cancelar este pedido?", "Confirmacao",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                string autor = UsuarioSessao.NomeAtual;
                var eC = _pedidoBLL.AtualizarSituacao(cod, 6, autor);
                if (!string.IsNullOrEmpty(eC)) MessageBox.Show("Erro: " + eC);
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
            CarregarPedidos();
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

            // ── Rodape gastos ──
            var pnlGastos = new Panel { Dock = DockStyle.Bottom, Height = 220, BackColor = Color.FromArgb(22, 30, 55) };

            var lblGTitle = new Label
            {
                Text      = "Gastos de Material / Insumos",
                ForeColor = Color.FromArgb(243, 156, 18),
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                Dock      = DockStyle.Top, Height = 32, Padding = new Padding(0, 8, 0, 0)
            };

            var pnlGBtn = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = Color.Transparent };
            var btnAddGasto = new Button
            {
                Text = "+ Lancar Gasto", Left = 0, Top = 4, Width = 140, Height = 28,
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
                Text = "\u2715 Excluir", Left = 148, Top = 4, Width = 100, Height = 28,
                BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnDelGasto.FlatAppearance.BorderSize = 0;
            btnDelGasto.Click += (_, __) =>
            {
                if (_gridGastos.SelectedRows.Count == 0) return;
                var cod = Convert.ToInt32(_gridGastos.SelectedRows[0].Cells["Codigo"].Value);
                if (MessageBox.Show("Excluir este gasto?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _gastosBLL.Excluir(cod);
                    CarregarFinanceiro();
                }
            };
            pnlGBtn.Controls.AddRange(new Control[] { btnAddGasto, btnDelGasto });

            _gridGastos = CriarGrid();
            _gridGastos.Dock = DockStyle.Fill;

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

            pnlGastos.Controls.Add(_gridGastos);
            pnlGastos.Controls.Add(pnlGBtn);
            pnlGastos.Controls.Add(lblGTitle);

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

                // ── Totais acumulados ──
                decimal pedidos = 0, totalBruto = 0, taxaEnt = 0,
                        dinheiro = 0, cartao = 0, pix = 0,
                        custoMerc = 0;
                foreach (DataRow r in dt.Rows)
                {
                    decimal V(string col) => r[col] == DBNull.Value ? 0 : Convert.ToDecimal(r[col]);
                    pedidos    += V("Pedidos");
                    totalBruto += V("TotalBruto");
                    taxaEnt    += V("TaxaEntrega");
                    dinheiro   += V("Dinheiro");
                    cartao     += V("Cartao");
                    pix        += V("Pix");
                    custoMerc  += V("CustoMercadorias");
                }

                // ── Gastos de material ──
                _gridGastos.DataSource = _gastosBLL.Listar(de, ate);
                decimal gastosMaterial = _gastosBLL.TotalPeriodo(de, ate);

                // ── Cálculos derivados ──
                decimal fatLiquido = totalBruto - taxaEnt;    // receita dos produtos sem frete
                decimal despesas   = custoMerc + gastosMaterial; // custo total (mercadorias + material)
                decimal lucro      = totalBruto - despesas;

                // ── Rebuild cards ──
                _pnlFinCards.Controls.Clear();
                CriarCardFin("Total Pedidos",      pedidos.ToString("N0"),       Color.FromArgb(41, 128, 185));
                CriarCardFin("Faturamento Bruto",  totalBruto.ToString("C"),     Color.FromArgb(39, 174, 96));
                CriarCardFin("Taxa de Entrega",    taxaEnt.ToString("C"),        Color.FromArgb(22, 160, 133));
                CriarCardFin("Fat. Líquido",       fatLiquido.ToString("C"),     Color.FromArgb(52, 152, 219));
                CriarCardFin("Custo Mercadorias",  custoMerc.ToString("C"),      Color.FromArgb(211, 84, 0));
                CriarCardFin("Gastos Material",    gastosMaterial.ToString("C"), Color.FromArgb(192, 57, 43));
                CriarCardFin("Despesas Totais",    despesas.ToString("C"),       Color.FromArgb(169, 50, 38));
                CriarCardFin("Dinheiro",           dinheiro.ToString("C"),       Color.FromArgb(39, 174, 96));
                CriarCardFin("Cartão",             cartao.ToString("C"),         Color.FromArgb(142, 68, 173));
                CriarCardFin("Pix",                pix.ToString("C"),            Color.FromArgb(52, 152, 219));
                CriarCardFin("Lucro Líquido",      lucro.ToString("C"),
                    lucro >= 0 ? Color.FromArgb(39, 174, 96) : Color.FromArgb(192, 57, 43));

                lblFinResumo.Text =
                    $"Período: {de:dd/MM/yyyy} a {ate:dd/MM/yyyy}  |  " +
                    $"Fat. Bruto: {totalBruto:C}  |  Custo Merc: {custoMerc:C}  |  " +
                    $"Gastos: {gastosMaterial:C}  |  Despesas: {despesas:C}  |  Lucro: {lucro:C}";
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar financeiro: " + ex.Message); }
        }
    }
}

