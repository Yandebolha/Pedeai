using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Pedeai.DB;
using Pedeai.Forms;

namespace Pedeai
{
public partial class Form1 : Form
{
    // ── Paleta ───────────────────────────────────────────────────────────────
    static readonly Color C_SIDEBAR     = Color.FromArgb(18, 18, 35);
    static readonly Color C_SIDEBAR_HOV = Color.FromArgb(35, 35, 65);
    static readonly Color C_SIDEBAR_SEL = Color.FromArgb(99, 102, 241);
    static readonly Color C_BG          = Color.FromArgb(245, 246, 250);
    static readonly Color C_SURFACE     = Color.White;
    static readonly Color C_HEADER      = Color.FromArgb(25, 25, 50);
    static readonly Color C_TEXT        = Color.FromArgb(30, 30, 60);
    static readonly Color C_MUTED       = Color.FromArgb(140, 140, 165);
    static readonly Color C_ACCENT      = Color.FromArgb(99, 102, 241);
    static readonly Color C_GREEN       = Color.FromArgb(22, 163, 74);
    static readonly Color C_ORANGE      = Color.FromArgb(234, 88, 12);
    static readonly Color C_RED         = Color.FromArgb(220, 38, 38);
    static readonly Color C_TEAL        = Color.FromArgb(13, 148, 136);
    static readonly Color C_BLUE        = Color.FromArgb(37, 99, 235);
    static readonly Font  F_BIG         = new Font("Segoe UI", 22, FontStyle.Bold);
    static readonly Font  F_TITLE       = new Font("Segoe UI", 16, FontStyle.Bold);
    static readonly Font  F_LABEL       = new Font("Segoe UI", 9,  FontStyle.Bold);
    static readonly Font  F_BODY        = new Font("Segoe UI", 9);
    static readonly Font  F_NAV         = new Font("Segoe UI", 10);
    static readonly Font  F_SMALL       = new Font("Segoe UI", 8);

    // ── Controles principais ─────────────────────────────────────────────────
    private Panel pnlContent;
    private Label lblPageTitle, lblPageSub, lblStatus, lblRelogio;
    private Panel _paginaAtiva;
    private Button _navAtivo;
    private Panel pgDashboard, pgPedidos, pgProdutos, pgClientes, pgFornecedores, pgCupons;

    // Dashboard
    private Label lblPedHoje, lblFat, lblCli, lblPend;

    // Pedidos
    private DataGridView gridPedidos, gridItens;
    private ComboBox cmbStatus;
    private Label lblInfo;
    private Button btnConf, btnPrep, btnPron, btnEntr, btnCanc;

    // Outros grids
    private DataGridView gridProdutos, gridClientes, gridForn, gridCupons;
    private TextBox txtBuscaCli;

    // Timer auto-refresh
    private System.Windows.Forms.Timer _timer;

    // ── Construtor ───────────────────────────────────────────────────────────
    public Form1()
    {
        InitializeComponent();
        Text          = "Pedeai — Sistema Local";
        Size          = new Size(1200, 720);
        MinimumSize   = new Size(1000, 640);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor     = C_BG;

        Build();

        Load += (_, __) =>
        {
            Carregar();
            _timer = new System.Windows.Forms.Timer { Interval = 15000 };
            _timer.Tick += (s, e) => CarregarPedidos();
            _timer.Start();
        };

        var clk = new System.Windows.Forms.Timer { Interval = 1000 };
        clk.Tick += (_, __) => { try { lblRelogio.Text = DateTime.Now.ToString("HH:mm:ss"); } catch { } };
        clk.Start();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _timer?.Stop();
        base.OnFormClosed(e);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // BUILD
    // ══════════════════════════════════════════════════════════════════════════
    private void Build()
    {
        SuspendLayout();

        // ── Sidebar (esquerda) ────────────────────────────────────────────────
        var sidebar = new Panel { Dock = DockStyle.Left, Width = 210, BackColor = C_SIDEBAR };
        BuildSidebar(sidebar);

        // ── Área direita ──────────────────────────────────────────────────────
        var pRight = new Panel { Dock = DockStyle.Fill, BackColor = C_BG };

        // Header
        var pHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = C_SURFACE };
        pHeader.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(228, 228, 240));
            e.Graphics.DrawLine(pen, 0, pHeader.Height - 1, pHeader.Width, pHeader.Height - 1);
        };
        lblPageTitle = new Label { Left = 20, Top = 10, AutoSize = true, Font = F_TITLE, ForeColor = C_TEXT, Text = "Dashboard" };
        lblPageSub   = new Label { Left = 22, Top = 36, AutoSize = true, Font = F_BODY, ForeColor = C_MUTED, Text = "Visão geral" };
        lblRelogio   = new Label { Top = 18, AutoSize = true, Font = new Font("Consolas", 13, FontStyle.Bold), ForeColor = C_ACCENT, Text = "00:00:00" };
        pHeader.Resize += (s, e) => lblRelogio.Left = pHeader.Width - lblRelogio.Width - 20;
        pHeader.Controls.AddRange(new Control[] { lblPageTitle, lblPageSub, lblRelogio });

        // Status bar
        var pStatus = new Panel { Dock = DockStyle.Bottom, Height = 24, BackColor = C_HEADER };
        lblStatus = new Label { Left = 10, Top = 4, AutoSize = true, Font = F_SMALL, ForeColor = C_MUTED, Text = "Pronto" };
        var lblVer = new Label { Top = 4, AutoSize = true, Font = F_SMALL, ForeColor = Color.FromArgb(70, 70, 100), Text = "Pedeai v1.0" };
        pStatus.Resize += (s, e) => lblVer.Left = pStatus.Width - lblVer.Width - 10;
        pStatus.Controls.AddRange(new Control[] { lblStatus, lblVer });

        // Content
        pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = C_BG, Padding = new Padding(14, 10, 14, 10) };

        // Páginas
        BuildPageDashboard();
        BuildPagePedidos();
        BuildPageProdutos();
        BuildPageClientes();
        BuildPageFornecedores();
        BuildPageCupons();

        pRight.Controls.Add(pnlContent);
        pRight.Controls.Add(pStatus);
        pRight.Controls.Add(pHeader);

        Controls.Add(pRight);
        Controls.Add(sidebar);

        ResumeLayout();
        NavPara("Dashboard", "Visão geral do sistema", pgDashboard, _navAtivo);
    }

    // ── Sidebar ───────────────────────────────────────────────────────────────
    private void BuildSidebar(Panel sidebar)
    {
        // Logo
        var logo = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = C_ACCENT };
        logo.Controls.Add(new Label
        {
            Text = "🍕  Pedeai", Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter
        });
        sidebar.Controls.Add(logo);

        // Separador
        sidebar.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(40, 40, 70) });

        // Itens de navegação (adicionados em ordem, Bottom-up pelo DockStyle.Top)
        var items = new[]
        {
            new[] { "📊   Dashboard",    "Dashboard",     "Visão geral" },
            new[] { "📦   Pedidos",       "Pedidos",       "Gestão de pedidos" },
            new[] { "🛒   Produtos",       "Produtos",      "Catálogo de produtos" },
            new[] { "👥   Clientes",       "Clientes",      "Base de clientes" },
            new[] { "🏭   Fornecedores",   "Fornecedores",  "Base de fornecedores" },
            new[] { "🎟   Cupons",          "Cupons",        "Cupons de desconto" },
        };

        Panel[] pages = null; // será resolvido após criação
        // Usamos closure para obter a página correta
        Button firstBtn = null;
        foreach (var item in items)
        {
            var texto = item[0]; var titulo = item[1]; var sub = item[2];
            var btn = new Button
            {
                Text = texto, Dock = DockStyle.Top, Height = 46,
                FlatStyle = FlatStyle.Flat,
                BackColor = C_SIDEBAR,
                ForeColor = Color.FromArgb(180, 180, 210),
                Font = F_NAV,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(18, 0, 0, 0),
                Cursor = Cursors.Hand,
                Tag = titulo  // título para identificar a página
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = C_SIDEBAR_HOV;
            btn.Click += NavBtn_Click;
            sidebar.Controls.Add(btn);
            if (firstBtn == null) { firstBtn = btn; _navAtivo = btn; }
        }

        // Rodapé
        var foot = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = Color.FromArgb(14, 14, 28) };
        foot.Controls.Add(new Label { Dock = DockStyle.Fill, Text = "Sistema Local", ForeColor = C_MUTED, Font = F_SMALL, TextAlign = ContentAlignment.MiddleCenter });
        sidebar.Controls.Add(foot);
    }

    private void NavBtn_Click(object sender, EventArgs e)
    {
        var btn = (Button)sender;
        var titulo = btn.Tag?.ToString() ?? "";
        Panel pg = null;
        string sub = "";
        switch (titulo)
        {
            case "Dashboard":    pg = pgDashboard;    sub = "Visão geral do sistema"; break;
            case "Pedidos":      pg = pgPedidos;      sub = "Gestão de pedidos em tempo real"; break;
            case "Produtos":     pg = pgProdutos;     sub = "Catálogo de produtos"; break;
            case "Clientes":     pg = pgClientes;     sub = "Base de clientes cadastrados"; break;
            case "Fornecedores": pg = pgFornecedores; sub = "Base de fornecedores"; break;
            case "Cupons":       pg = pgCupons;       sub = "Cupons de desconto"; break;
        }
        if (pg != null) NavPara(titulo, sub, pg, btn);
    }

    private void NavPara(string titulo, string sub, Panel pg, Button btn)
    {
        lblPageTitle.Text = titulo;
        lblPageSub.Text   = sub;

        if (_navAtivo != null) { _navAtivo.BackColor = C_SIDEBAR; _navAtivo.ForeColor = Color.FromArgb(180, 180, 210); }
        btn.BackColor = C_SIDEBAR_SEL;
        btn.ForeColor = Color.White;
        _navAtivo = btn;

        if (_paginaAtiva != null) _paginaAtiva.Visible = false;
        pg.Visible = true;
        _paginaAtiva = pg;
    }

    // ── Páginas ───────────────────────────────────────────────────────────────
    private Panel NewPage()
    {
        var p = new Panel { Dock = DockStyle.Fill, BackColor = C_BG, Visible = false };
        pnlContent.Controls.Add(p);
        return p;
    }

    // ─── Dashboard ────────────────────────────────────────────────────────────
    private void BuildPageDashboard()
    {
        pgDashboard = NewPage();

        // Toolbar de ações
        var tb = Toolbar();
        BtnTb(tb, "⟳  Atualizar", C_ACCENT, (_, __) => Carregar());
        BtnTb(tb, "🌐  Abrir Cardápio Web", C_BLUE, (_, __) => System.Diagnostics.Process.Start("http://localhost:5050"));
        pgDashboard.Controls.Add(tb);

        // Grid de pedidos do dia
        var gridRec = CriarGrid();
        pgDashboard.Controls.Add(gridRec);
        pgDashboard.Controls.Add(SLabel("Pedidos de Hoje"));

        // Cards de KPI
        var pCards = new TableLayoutPanel
        {
            Dock = DockStyle.Top, Height = 110, ColumnCount = 4, RowCount = 1,
            BackColor = Color.Transparent, Padding = new Padding(0, 0, 0, 8)
        };
        for (int i = 0; i < 4; i++) pCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        pCards.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        lblPedHoje = AddCard(pCards, 0, "Pedidos Hoje",      "-",    C_ACCENT);
        lblFat     = AddCard(pCards, 1, "Faturamento",       "R$ -", C_TEAL);
        lblCli     = AddCard(pCards, 2, "Total Clientes",    "-",    C_BLUE);
        lblPend    = AddCard(pCards, 3, "Pendentes",         "-",    C_ORANGE);
        pgDashboard.Controls.Add(pCards);

        pgDashboard.Tag = gridRec;
    }

    private Label AddCard(TableLayoutPanel tbl, int col, string titulo, string val, Color cor)
    {
        var card = new Panel { Dock = DockStyle.Fill, BackColor = C_SURFACE, Margin = new Padding(0, 0, 8, 0) };
        card.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var br = new SolidBrush(cor);
            e.Graphics.FillRectangle(br, 0, 0, 5, card.Height);
        };
        var t = new Label { Text = titulo, Font = F_SMALL, ForeColor = C_MUTED, Left = 14, Top = 12, AutoSize = true };
        var v = new Label { Text = val,   Font = F_BIG,   ForeColor = C_TEXT,  Left = 14, Top = 30, AutoSize = true };
        card.Controls.Add(t); card.Controls.Add(v);
        tbl.Controls.Add(card, col, 0);
        return v;
    }

    // ─── Pedidos ──────────────────────────────────────────────────────────────
    private void BuildPagePedidos()
    {
        pgPedidos = NewPage();

        var tb = Toolbar();
        var lblF = new Label { Text = "Status:", Width = 52, Height = 30, TextAlign = ContentAlignment.MiddleLeft, Font = F_LABEL, ForeColor = C_TEXT };
        cmbStatus = new ComboBox { Width = 160, Height = 28, DropDownStyle = ComboBoxStyle.DropDownList, Font = F_BODY, Margin = new Padding(0, 1, 6, 0) };
        cmbStatus.Items.AddRange(new[] { "Todos", "Pendente", "Confirmado", "Em Preparo", "Pronto", "Saiu p/ Entrega", "Entregue", "Cancelado" });
        cmbStatus.SelectedIndex = 0;
        tb.Controls.AddRange(new Control[] { lblF, cmbStatus });
        BtnTb(tb, "⟳  Atualizar", C_ACCENT, (_, __) => CarregarPedidos());
        pgPedidos.Controls.Add(tb);

        var split = new SplitContainer { Dock = DockStyle.Fill, BackColor = C_BG };
        split.Panel1MinSize = 300;
        split.Panel2MinSize = 320;

        // Esquerda — lista de pedidos
        gridPedidos = CriarGrid();
        gridPedidos.SelectionChanged += GridPedidos_SelectionChanged;
        split.Panel1.Controls.Add(gridPedidos);

        // Direita — detalhe (TableLayout 3 linhas)
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1, BackColor = C_BG };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        // Info card
        var pInfo = new Panel { Dock = DockStyle.Fill, BackColor = C_SURFACE, Margin = new Padding(0, 0, 0, 4) };
        pInfo.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(220, 220, 240));
            e.Graphics.DrawRectangle(pen, 0, 0, pInfo.Width - 1, pInfo.Height - 1);
            using var br = new SolidBrush(C_ACCENT);
            e.Graphics.FillRectangle(br, 0, 0, 4, pInfo.Height);
        };
        lblInfo = new Label
        {
            Dock = DockStyle.Fill, Font = F_BODY, ForeColor = C_TEXT,
            Padding = new Padding(14, 10, 10, 10),
            Text = "← Selecione um pedido na lista para ver os detalhes"
        };
        pInfo.Controls.Add(lblInfo);
        tbl.Controls.Add(pInfo, 0, 0);

        // Grid itens
        gridItens = CriarGrid();
        tbl.Controls.Add(gridItens, 0, 1);

        // Botões status
        var pBtns = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 1, BackColor = C_BG };
        for (int i = 0; i < 5; i++) pBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        pBtns.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        string[] labels = { "✓ Confirmar", "⏳ Preparando", "✅ Pronto", "🚴 Saiu", "✕ Cancelar" };
        Color[]  cores  = { C_BLUE, C_ORANGE, C_GREEN, C_TEAL, C_RED };
        btnConf = MkBtn(labels[0], cores[0]);
        btnPrep = MkBtn(labels[1], cores[1]);
        btnPron = MkBtn(labels[2], cores[2]);
        btnEntr = MkBtn(labels[3], cores[3]);
        btnCanc = MkBtn(labels[4], cores[4]);
        int ci = 0;
        foreach (var b in new[] { btnConf, btnPrep, btnPron, btnEntr, btnCanc })
        {
            b.Click += BtnStatus_Click;
            pBtns.Controls.Add(b, ci++, 0);
        }
        tbl.Controls.Add(pBtns, 0, 2);

        split.Panel2.Controls.Add(tbl);
        pgPedidos.Controls.Add(split);

        Shown += (_, __) =>
        {
            try { if (split.Width > 650) split.SplitterDistance = split.Width - 360; } catch { }
        };
    }

    private Button MkBtn(string texto, Color cor)
    {
        var b = new Button
        {
            Text = texto, Dock = DockStyle.Fill, BackColor = cor, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Font = F_LABEL, Margin = new Padding(3, 6, 3, 6)
        };
        b.FlatAppearance.BorderSize = 0;
        return b;
    }

    // ─── Produtos ─────────────────────────────────────────────────────────────
    private void BuildPageProdutos()
    {
        pgProdutos = NewPage();
        var tb = Toolbar();
        BtnTb(tb, "⟳  Atualizar",        C_ACCENT,  (_, __) => CarregarProdutos());
        BtnTb(tb, "✏  Ativar/Desativar",  C_ORANGE,  (_, __) =>
        {
            if (gridProdutos.SelectedRows.Count == 0) return;
            try { DbHelper.AlternarSituacaoMercadoria(RowInt(gridProdutos, "Codigo")); CarregarProdutos(); }
            catch (Exception ex) { Erro(ex); }
        });
        BtnTb(tb, "+ Cadastrar Produto",  C_GREEN,   (_, __) => { new frmCadastroProduto().ShowDialog(this); CarregarProdutos(); });
        BtnTb(tb, "📂 Categorias",        C_ACCENT,  (_, __) => new frmCadastroCategoria().ShowDialog(this));
        gridProdutos = CriarGrid();
        pgProdutos.Controls.Add(gridProdutos);
        pgProdutos.Controls.Add(SLabel("Catálogo de Produtos"));
        pgProdutos.Controls.Add(tb);
    }

    // ─── Clientes ─────────────────────────────────────────────────────────────
    private void BuildPageClientes()
    {
        pgClientes = NewPage();
        var tb = Toolbar();
        var lblB = new Label { Text = "Buscar:", Width = 52, Height = 30, TextAlign = ContentAlignment.MiddleLeft, Font = F_LABEL, ForeColor = C_TEXT };
        txtBuscaCli = new TextBox { Width = 230, Font = F_BODY, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0, 1, 6, 0) };
        txtBuscaCli.KeyDown += (_, k) => { if (k.KeyCode == Keys.Enter) CarregarClientes(); };
        var btnB = BotaoAcao("Buscar", C_ACCENT); btnB.Click += (_, __) => CarregarClientes();
        tb.Controls.AddRange(new Control[] { lblB, txtBuscaCli, btnB });
        BtnTb(tb, "+ Cadastrar Cliente", C_GREEN, (_, __) => { new frmCadastroCliente().ShowDialog(this); CarregarClientes(); });
        gridClientes = CriarGrid();
        pgClientes.Controls.Add(gridClientes);
        pgClientes.Controls.Add(SLabel("Base de Clientes"));
        pgClientes.Controls.Add(tb);
    }

    // ─── Fornecedores ─────────────────────────────────────────────────────────
    private void BuildPageFornecedores()
    {
        pgFornecedores = NewPage();
        var tb = Toolbar();
        BtnTb(tb, "⟳  Atualizar",          C_ACCENT, (_, __) => CarregarFornecedores());
        BtnTb(tb, "+ Cadastrar Fornecedor", C_GREEN,  (_, __) => { new frmCadastroFornecedor().ShowDialog(this); CarregarFornecedores(); });
        gridForn = CriarGrid();
        pgFornecedores.Controls.Add(gridForn);
        pgFornecedores.Controls.Add(SLabel("Base de Fornecedores"));
        pgFornecedores.Controls.Add(tb);
    }

    // ─── Cupons ───────────────────────────────────────────────────────────────
    private void BuildPageCupons()
    {
        pgCupons = NewPage();
        var tb = Toolbar();
        BtnTb(tb, "⟳  Atualizar",  C_ACCENT, (_, __) => CarregarCupons());
        BtnTb(tb, "+ Novo Cupom",   C_GREEN,  (_, __) => { new frmCadastroCupom().ShowDialog(this); CarregarCupons(); });
        gridCupons = CriarGrid();
        pgCupons.Controls.Add(gridCupons);
        pgCupons.Controls.Add(SLabel("Gestão de Cupons"));
        pgCupons.Controls.Add(tb);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // HELPERS DE UI
    // ══════════════════════════════════════════════════════════════════════════
    private static DataGridView CriarGrid()
    {
        var g = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true, AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            BackgroundColor = C_SURFACE,
            GridColor = Color.FromArgb(235, 235, 245),
            Font = F_BODY, BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            RowTemplate = { Height = 32 },
            AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(248, 248, 255) }
        };
        g.ColumnHeadersDefaultCellStyle.BackColor = C_HEADER;
        g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(190, 190, 215);
        g.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9, FontStyle.Bold);
        g.ColumnHeadersDefaultCellStyle.Padding   = new Padding(8, 0, 0, 0);
        g.ColumnHeadersHeight = 36;
        g.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
        g.EnableHeadersVisualStyles = false;
        return g;
    }

    private static Button BotaoAcao(string texto, Color cor)
    {
        var b = new Button
        {
            Text = texto, BackColor = cor, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Height = 30, Font = F_LABEL,
            Cursor = Cursors.Hand, Margin = new Padding(0, 0, 6, 0),
            AutoSize = false
        };
        b.FlatAppearance.BorderSize = 0;
        b.Width = TextRenderer.MeasureText(texto, F_LABEL).Width + 24;
        return b;
    }

    private static FlowLayoutPanel Toolbar()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Top, Height = 44, BackColor = Color.Transparent,
            Padding = new Padding(0, 6, 0, 0), WrapContents = false
        };
    }

    private static void BtnTb(FlowLayoutPanel tb, string texto, Color cor, EventHandler click)
    {
        var b = BotaoAcao(texto, cor);
        b.Click += click;
        tb.Controls.Add(b);
    }

    private static Label SLabel(string texto)
    {
        return new Label
        {
            Text = texto, Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = C_TEXT, Dock = DockStyle.Top, Height = 32,
            TextAlign = ContentAlignment.BottomLeft, Padding = new Padding(2, 0, 0, 2)
        };
    }

    private static int RowInt(DataGridView g, string col)
    {
        if (g.SelectedRows.Count == 0) return 0;
        var v = g.SelectedRows[0].Cells[col].Value;
        return v is int i ? i : v is long l ? (int)l : Convert.ToInt32(v);
    }

    private void SetStatus(string msg) { try { lblStatus.Text = msg; } catch { } }
    private static void Erro(Exception ex) => MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

    // ══════════════════════════════════════════════════════════════════════════
    // EVENTOS
    // ══════════════════════════════════════════════════════════════════════════
    private void GridPedidos_SelectionChanged(object sender, EventArgs e)
    {
        if (gridPedidos.SelectedRows.Count == 0) return;
        var row = gridPedidos.SelectedRows[0];
        try
        {
            var cod = RowInt(gridPedidos, "Codigo");
            lblInfo.Text =
                $"Pedido #{ row.Cells["Numero"]?.Value }   •   { row.Cells["Cliente"]?.Value }   •   Tel: { row.Cells["Telefone"]?.Value }\n" +
                $"Status: { row.Cells["Status"]?.Value }   •   Total: R$ { row.Cells["Total"]?.Value:F2 }   •   { row.Cells["DataHora"]?.Value }";
            gridItens.DataSource = DbHelper.GetItensPedido(cod);
        }
        catch { }
    }

    private void BtnStatus_Click(object sender, EventArgs e)
    {
        if (gridPedidos.SelectedRows.Count == 0) return;
        var cod = RowInt(gridPedidos, "Codigo");
        int novoStatus = sender == btnConf ? 1
                       : sender == btnPrep ? 2
                       : sender == btnPron ? 3
                       : sender == btnEntr ? 5
                       : sender == btnCanc ? 6 : 0;
        try { DbHelper.AtualizarSituacaoPedido(cod, novoStatus); CarregarPedidos(); }
        catch (Exception ex) { Erro(ex); }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // CARGA DE DADOS
    // ══════════════════════════════════════════════════════════════════════════
    private void Carregar()
    {
        CarregarDashboard();
        CarregarPedidos();
        CarregarProdutos();
        CarregarClientes();
        CarregarFornecedores();
        CarregarCupons();
    }

    private void CarregarDashboard()
    {
        try
        {
            var (p, f, c, pend) = DbHelper.GetEstatisticas();
            lblPedHoje.Text = p.ToString();
            lblFat.Text     = "R$ " + f.ToString("N2");
            lblCli.Text     = c.ToString();
            lblPend.Text    = pend.ToString();
            if (pgDashboard.Tag is DataGridView gr)
                gr.DataSource = DbHelper.ListarPedidos(null, DateTime.Today);
            SetStatus("Atualizado: " + DateTime.Now.ToString("HH:mm:ss"));
        }
        catch (Exception ex) { SetStatus("Erro: " + ex.Message); }
    }

    private void CarregarPedidos()
    {
        try
        {
            int? filtro = cmbStatus.SelectedIndex > 0 ? cmbStatus.SelectedIndex - 1 : (int?)null;
            var dt = DbHelper.ListarPedidos(filtro?.ToString());
            foreach (DataRow row in dt.Rows)
                row["Status"] = StatusLabel(Convert.ToInt32(row["Status"]));
            Action upd = () => gridPedidos.DataSource = dt;
            if (InvokeRequired) Invoke(upd); else upd();
        }
        catch (Exception ex) { SetStatus("Erro pedidos: " + ex.Message); }
    }

    private void CarregarProdutos()
    {
        try { gridProdutos.DataSource = DbHelper.ListarMercadorias(); }
        catch (Exception ex) { SetStatus("Erro produtos: " + ex.Message); }
    }

    private void CarregarClientes()
    {
        try { gridClientes.DataSource = DbHelper.ListarClientes(txtBuscaCli?.Text?.Trim() ?? ""); }
        catch (Exception ex) { SetStatus("Erro clientes: " + ex.Message); }
    }

    private void CarregarFornecedores()
    {
        try { gridForn.DataSource = DbHelper.ListarFornecedores(); }
        catch (Exception ex) { SetStatus("Erro fornecedores: " + ex.Message); }
    }

    private void CarregarCupons()
    {
        try { gridCupons.DataSource = DbHelper.ListarCupons(); }
        catch (Exception ex) { SetStatus("Erro cupons: " + ex.Message); }
    }

    private static string StatusLabel(int s)
    {
        switch (s)
        {
            case 0: return "⏸ Pendente";
            case 1: return "✓ Confirmado";
            case 2: return "⏳ Em Preparo";
            case 3: return "✅ Pronto";
            case 4: return "🚴 Saiu p/ Entrega";
            case 5: return "📦 Entregue";
            case 6: return "✕ Cancelado";
            default: return s.ToString();
        }
    }
}
}


namespace Pedeai
{
public partial class Form1 : Form
{
    // ── Timers ───────────────────────────────────────────────────────────────
    private System.Timers.Timer _timerPedidos = new(15_000); // 15 segundos

    // ── Pedidos Tab ──────────────────────────────────────────────────────────
    private TabControl tabMain = new();
    private TabPage tabPedidos = new("Pedidos");
    private TabPage tabProdutos = new("Produtos");
    private TabPage tabClientes = new("Clientes");
    private TabPage tabFornecedores = new("Fornecedores");
    private TabPage tabCupons = new("Cupons");
    private TabPage tabDashboard = new("Dashboard");

    private DataGridView gridPedidos = new();
    private DataGridView gridItens = new();
    private ComboBox cmbStatusPedido = new();
    private Label lblPedidoInfo = new();
    private Button btnConfirmar = new(), btnEmPreparo = new(),
                   btnPronto = new(), btnEntregue = new(), btnCancelar = new();

    private DataGridView gridProdutos = new();
    private Button btnToggleProduto = new();

    private DataGridView gridClientes = new();
    private TextBox txtBuscaCliente = new();

    private DataGridView gridFornecedores = new();
    private DataGridView gridCupons = new();

    // Dashboard labels
    private Label lblPedidosHoje = new(), lblFaturamento = new(),
                  lblClientes = new(), lblPendentes = new();

    public Form1()
    {
        InitializeComponent();
        Text = "Pedeai — Sistema Local";
        Size = new Size(1100, 700);
        MinimumSize = new Size(1000, 640);
        StartPosition = FormStartPosition.CenterScreen;
        Icon = SystemIcons.Application;

        BuildUI();

        Load += (_, __) =>
        {
            CarregarTudo();
            _timerPedidos.Elapsed += (s, e) => InvokeOnUI(CarregarPedidos);
            _timerPedidos.AutoReset = true;
            _timerPedidos.Start();
        };

        Shown += (_, __) =>
        {
            try
            {
                foreach (Control ctrl in tabPedidos.Controls)
                    if (ctrl is SplitContainer sc)
                        sc.SplitterDistance = (int)(sc.Width * 0.65);
            }
            catch { }
        };
    }

    // Helper to marshal calls to UI thread
    private void InvokeOnUI(Action action)
    {
        if (IsHandleCreated && !IsDisposed)
            BeginInvoke(action);
    }

    // ── Build UI ─────────────────────────────────────────────────────────────

    private void BuildUI()
    {
        tabMain.Dock = DockStyle.Fill;
        tabMain.Font = new Font("Segoe UI", 10);
        tabMain.TabPages.AddRange(new[] { tabDashboard, tabPedidos, tabProdutos, tabClientes, tabFornecedores, tabCupons });
        Controls.Add(tabMain);

        BuildDashboardTab();
        BuildPedidosTab();
        BuildProdutosTab();
        BuildClientesTab();
        BuildFornecedoresTab();
        BuildCuponsTab();
    }

    private static DataGridView CriarGrid()
    {
        var g = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            Font = new Font("Segoe UI", 9),
            BorderStyle = BorderStyle.None
        };
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 80);
        g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        return g;
    }

    private static Button CriarBotao(string texto, Color cor)
    {
        return new Button
        {
            Text = texto,
            BackColor = cor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Height = 32,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
    }

    // ─── Dashboard ────────────────────────────────────────────────────────────

    private void BuildDashboardTab()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), FlowDirection = FlowDirection.LeftToRight };

        panel.Controls.Add(CriarCard("Pedidos Hoje", ref lblPedidosHoje, Color.FromArgb(63, 81, 181)));
        panel.Controls.Add(CriarCard("Faturamento Hoje", ref lblFaturamento, Color.FromArgb(0, 150, 136)));
        panel.Controls.Add(CriarCard("Total Clientes", ref lblClientes, Color.FromArgb(244, 67, 54)));
        panel.Controls.Add(CriarCard("Pedidos Pendentes", ref lblPendentes, Color.FromArgb(255, 152, 0)));

        var btnAtualizar = CriarBotao("  Atualizar", Color.FromArgb(63, 81, 181));
        btnAtualizar.Width = 150;
        btnAtualizar.Click += (_, _) => CarregarDashboard();
        panel.Controls.Add(btnAtualizar);

        tabDashboard.Controls.Add(panel);
    }

    private Panel CriarCard(string titulo, ref Label lblValor, Color cor)
    {
        var p = new Panel { Width = 200, Height = 120, Margin = new Padding(10) };
        p.BackColor = cor;
        var t = new Label { Text = titulo, ForeColor = Color.White, Font = new Font("Segoe UI", 9), Dock = DockStyle.Top, Height = 30, TextAlign = ContentAlignment.MiddleCenter };
        lblValor = new Label { Text = "-", ForeColor = Color.White, Font = new Font("Segoe UI", 22, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
        p.Controls.Add(lblValor);
        p.Controls.Add(t);
        return p;
    }

    // ─── Pedidos ──────────────────────────────────────────────────────────────

    private void BuildPedidosTab()
    {
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical
        };

        // ── Panel esquerdo: filtro + grid de pedidos ───────────────────────────
        var topBar = new Panel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(6, 5, 4, 0) };
        var lblFiltro = new Label { Text = "Status:", Left = 0, Top = 5, Width = 52, Height = 22, TextAlign = ContentAlignment.MiddleLeft };
        cmbStatusPedido = new ComboBox { Left = 54, Top = 3, Width = 155, Height = 24, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbStatusPedido.Items.AddRange(new[] { "Todos", "Pendente", "Confirmado", "Em Preparo", "Pronto", "Saiu p/ Entrega", "Entregue", "Cancelado" });
        cmbStatusPedido.SelectedIndex = 0;
        var btnRefresh = CriarBotao("⟳ Atualizar", Color.FromArgb(63, 81, 181));
        btnRefresh.SetBounds(216, 2, 100, 26);
        btnRefresh.Click += (_, __) => CarregarPedidos();
        topBar.Controls.AddRange(new Control[] { lblFiltro, cmbStatusPedido, btnRefresh });

        gridPedidos = CriarGrid();
        gridPedidos.SelectionChanged += GridPedidos_SelectionChanged;
        split.Panel1.Controls.Add(gridPedidos);
        split.Panel1.Controls.Add(topBar);

        // ── Panel direito: usa TableLayoutPanel para 3 linhas fixas ───────────
        // Row0=info (auto), Row1=itens (fill), Row2=botões (auto)
        var tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));   // info
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));    // itens
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));   // botões

        // Linha 0 — info do pedido
        lblPedidoInfo = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9),
            AutoSize = false,
            Padding = new Padding(6, 4, 6, 4),
            BackColor = Color.FromArgb(240, 240, 255),
            BorderStyle = BorderStyle.FixedSingle,
            Text = "Selecione um pedido na lista ao lado."
        };
        tbl.Controls.Add(lblPedidoInfo, 0, 0);

        // Linha 1 — grid de itens
        gridItens = CriarGrid();
        gridItens.Dock = DockStyle.Fill;
        tbl.Controls.Add(gridItens, 0, 1);

        // Linha 2 — botões de status em TableLayout para distribuir igualmente
        var pBotoes = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 1
        };
        for (int i = 0; i < 5; i++)
            pBotoes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        pBotoes.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        btnConfirmar = CriarBotao("Confirmar",  Color.FromArgb(33, 150, 243));
        btnEmPreparo = CriarBotao("Em Preparo", Color.FromArgb(255, 152, 0));
        btnPronto    = CriarBotao("Pronto",      Color.FromArgb(76, 175, 80));
        btnEntregue  = CriarBotao("Entregue",    Color.FromArgb(0, 150, 136));
        btnCancelar  = CriarBotao("Cancelar",    Color.FromArgb(244, 67, 54));
        int col = 0;
        foreach (var b in new[] { btnConfirmar, btnEmPreparo, btnPronto, btnEntregue, btnCancelar })
        {
            b.Dock = DockStyle.Fill;
            b.Margin = new Padding(2, 4, 2, 4);
            b.Click += BtnStatus_Click;
            pBotoes.Controls.Add(b, col++, 0);
        }
        tbl.Controls.Add(pBotoes, 0, 2);

        split.Panel2.Controls.Add(tbl);
        tabPedidos.Controls.Add(split);
    }

    private void GridPedidos_SelectionChanged(object sender, EventArgs e)
    {
        if (gridPedidos.SelectedRows.Count == 0) return;
        var row = gridPedidos.SelectedRows[0];
        var codigo = row.Cells["Codigo"].Value as int? ?? (int)(long)row.Cells["Codigo"].Value;

        lblPedidoInfo.Text =
            $"#{row.Cells["Numero"]?.Value}  |  {row.Cells["Cliente"]?.Value}  |  " +
            $"Tel: {row.Cells["Telefone"]?.Value}\nStatus: {SituacaoLabel((int?)row.Cells["Status"]?.Value ?? 0)}" +
            $"  |  Total: R$ {row.Cells["Total"]?.Value:F2}" +
            $"  |  {row.Cells["DataHora"]?.Value}";

        gridItens.DataSource = DbHelper.GetItensPedido(codigo);
    }

    private void BtnStatus_Click(object sender, EventArgs e)
    {
        if (gridPedidos.SelectedRows.Count == 0) return;
        var row = gridPedidos.SelectedRows[0];
        var codigo = row.Cells["Codigo"].Value as int? ?? (int)(long)row.Cells["Codigo"].Value;

        int novoStatus = sender == btnConfirmar ? 1
                       : sender == btnEmPreparo ? 2
                       : sender == btnPronto ? 3
                       : sender == btnEntregue ? 5
                       : sender == btnCancelar ? 6 : 0;

        try
        {
            DbHelper.AtualizarSituacaoPedido(codigo, novoStatus);
            CarregarPedidos();
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    // ─── Produtos ─────────────────────────────────────────────────────────────

    private void BuildProdutosTab()
    {
        var topBar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(4, 4, 4, 0), WrapContents = false, AutoSize = false };
        var btnRefresh = CriarBotao("⟳ Atualizar", Color.FromArgb(63, 81, 181));
        btnRefresh.Width = 100; btnRefresh.Height = 28; btnRefresh.Margin = new Padding(0, 0, 4, 0);
        btnRefresh.Click += (_, _) => CarregarProdutos();
        btnToggleProduto = CriarBotao("Ativar/Desativar", Color.FromArgb(255, 152, 0));
        btnToggleProduto.Width = 130; btnToggleProduto.Height = 28; btnToggleProduto.Margin = new Padding(0, 0, 4, 0);
        btnToggleProduto.Click += BtnToggleProduto_Click;
        var btnCadastrar = CriarBotao("+ Cadastrar", Color.FromArgb(0, 150, 136));
        btnCadastrar.Width = 110; btnCadastrar.Height = 28;
        btnCadastrar.Click += (_, _) => { new frmCadastroProduto().ShowDialog(this); CarregarProdutos(); };
        topBar.Controls.AddRange(new Control[] { btnRefresh, btnToggleProduto, btnCadastrar });

        gridProdutos = CriarGrid();
        tabProdutos.Controls.Add(gridProdutos);
        tabProdutos.Controls.Add(topBar);
    }

    private void BtnToggleProduto_Click(object sender, EventArgs e)
    {
        if (gridProdutos.SelectedRows.Count == 0) return;
        var row = gridProdutos.SelectedRows[0];
        var codigo = row.Cells["Codigo"].Value as int? ?? (int)(long)row.Cells["Codigo"].Value;
        try { DbHelper.AlternarSituacaoMercadoria(codigo); CarregarProdutos(); }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    // ─── Clientes ─────────────────────────────────────────────────────────────

    private void BuildClientesTab()
    {
        var topBar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(4, 4, 4, 0), WrapContents = false, AutoSize = false };
        var lblBusca = new Label { Text = "Buscar:", Width = 52, Height = 28, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(0) };
        txtBuscaCliente = new TextBox { Width = 220, Height = 24, Margin = new Padding(0, 2, 4, 0) };
        var btnBuscar = CriarBotao("Buscar", Color.FromArgb(63, 81, 181));
        btnBuscar.Width = 80; btnBuscar.Height = 28; btnBuscar.Margin = new Padding(0, 0, 4, 0);
        btnBuscar.Click += (_, _) => CarregarClientes();
        txtBuscaCliente.KeyDown += (_, k) => { if (k.KeyCode == Keys.Enter) CarregarClientes(); };
        var btnCadastrar = CriarBotao("+ Cadastrar", Color.FromArgb(0, 150, 136));
        btnCadastrar.Width = 110; btnCadastrar.Height = 28;
        btnCadastrar.Click += (_, _) => { new frmCadastroCliente().ShowDialog(this); CarregarClientes(); };
        topBar.Controls.AddRange(new Control[] { lblBusca, txtBuscaCliente, btnBuscar, btnCadastrar });

        gridClientes = CriarGrid();
        tabClientes.Controls.Add(gridClientes);
        tabClientes.Controls.Add(topBar);
    }

    // ─── Fornecedores ─────────────────────────────────────────────────────────

    private void BuildFornecedoresTab()
    {
        var topBar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(4, 4, 4, 0), WrapContents = false, AutoSize = false };
        var btnRefresh = CriarBotao("⟳ Atualizar", Color.FromArgb(63, 81, 181));
        btnRefresh.Width = 100; btnRefresh.Height = 28; btnRefresh.Margin = new Padding(0, 0, 4, 0);
        btnRefresh.Click += (_, _) => CarregarFornecedores();
        var btnCadastrar = CriarBotao("+ Cadastrar", Color.FromArgb(0, 150, 136));
        btnCadastrar.Width = 110; btnCadastrar.Height = 28;
        btnCadastrar.Click += (_, _) => { new frmCadastroFornecedor().ShowDialog(this); CarregarFornecedores(); };
        topBar.Controls.AddRange(new Control[] { btnRefresh, btnCadastrar });

        gridFornecedores = CriarGrid();
        tabFornecedores.Controls.Add(gridFornecedores);
        tabFornecedores.Controls.Add(topBar);
    }

    // ─── Cupons ───────────────────────────────────────────────────────────────

    private void BuildCuponsTab()
    {
        var topBar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(4, 4, 4, 0), WrapContents = false, AutoSize = false };
        var btnRefresh = CriarBotao("⟳ Atualizar", Color.FromArgb(63, 81, 181));
        btnRefresh.Width = 100; btnRefresh.Height = 28; btnRefresh.Margin = new Padding(0, 0, 4, 0);
        btnRefresh.Click += (_, _) => CarregarCupons();
        var btnCadastrar = CriarBotao("+ Cadastrar", Color.FromArgb(0, 150, 136));
        btnCadastrar.Width = 110; btnCadastrar.Height = 28;
        btnCadastrar.Click += (_, _) => { new frmCadastroCupom().ShowDialog(this); CarregarCupons(); };
        topBar.Controls.AddRange(new Control[] { btnRefresh, btnCadastrar });

        gridCupons = CriarGrid();
        tabCupons.Controls.Add(gridCupons);
        tabCupons.Controls.Add(topBar);
    }

    // ── Carga de dados ────────────────────────────────────────────────────────

    private void CarregarTudo()
    {
        CarregarDashboard();
        CarregarPedidos();
        CarregarProdutos();
        CarregarClientes();
        CarregarFornecedores();
        CarregarCupons();
    }

    private void CarregarDashboard()
    {
        try
        {
            var (pedidos, fat, clientes, pendentes) = DbHelper.GetEstatisticas();
            lblPedidosHoje.Text = pedidos.ToString();
            lblFaturamento.Text = $"R$ {fat:N2}";
            lblClientes.Text = clientes.ToString();
            lblPendentes.Text = pendentes.ToString();
        }
        catch (Exception ex) { StatusBarMensagem($"Erro dashboard: {ex.Message}"); }
    }

    private void CarregarPedidos()
    {
        try
        {
            int? filtroSit = cmbStatusPedido.SelectedIndex > 0 ? cmbStatusPedido.SelectedIndex - 1 : (int?)null;
            var dt = DbHelper.ListarPedidos(filtroSit?.ToString());
            // Traduz status
            foreach (DataRow row in dt.Rows)
                row["Status"] = SituacaoLabel(Convert.ToInt32(row["Status"]));
            gridPedidos.DataSource = dt;
        }
        catch (Exception ex) { StatusBarMensagem($"Erro pedidos: {ex.Message}"); }
    }

    private void CarregarProdutos()
    {
        try { gridProdutos.DataSource = DbHelper.ListarMercadorias(); }
        catch (Exception ex) { StatusBarMensagem($"Erro produtos: {ex.Message}"); }
    }

    private void CarregarClientes()
    {
        try { gridClientes.DataSource = DbHelper.ListarClientes(txtBuscaCliente.Text.Trim()); }
        catch (Exception ex) { StatusBarMensagem($"Erro clientes: {ex.Message}"); }
    }

    private void CarregarFornecedores()
    {
        try { gridFornecedores.DataSource = DbHelper.ListarFornecedores(); }
        catch (Exception ex) { StatusBarMensagem($"Erro fornecedores: {ex.Message}"); }
    }

    private void CarregarCupons()
    {
        try { gridCupons.DataSource = DbHelper.ListarCupons(); }
        catch (Exception ex) { StatusBarMensagem($"Erro cupons: {ex.Message}"); }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string SituacaoLabel(int s) => s switch
    {
        0 => "Pendente",
        1 => "Confirmado",
        2 => "Em Preparo",
        3 => "Pronto",
        4 => "Saiu p/ Entrega",
        5 => "Entregue",
        6 => "Cancelado",
        _ => s.ToString()
    };

    private void StatusBarMensagem(string msg)
    {
        if (IsHandleCreated && !IsDisposed)
            BeginInvoke(new Action(() => Text = $"Pedeai — {msg}"));
        else
            Text = $"Pedeai — {msg}";
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _timerPedidos.Stop();
        base.OnFormClosed(e);
    }
}
}

