using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Forms;

namespace Pedeai
{
    public partial class Form1 : Form
    {
        // ── BLL ─────────────────────────────────────────────────────────────
        private readonly PedidoBLL      _pedidoBLL  = new PedidoBLL();
        private readonly DashboardBLL   _dashBLL    = new DashboardBLL();

        // ── Layout ──────────────────────────────────────────────────────────
        private Panel       pnlSidebar;
        private Panel       pnlContent;
        private Panel       pnlTopBar;
        private Label       lblTitulo;

        // ── Dashboard ────────────────────────────────────────────────────────
        private Panel       pnlDashboard;
        private Label       lblPedidosHoje;
        private Label       lblFaturamento;
        private Label       lblClientes;
        private Label       lblPendentes;

        // ── Pedidos ──────────────────────────────────────────────────────────
        private Panel           pnlPedidos;
        private DataGridView    gridPedidos;
        private ComboBox        cmbFiltroPedido;
        private DateTimePicker  dtpFiltroPedido;
        private DataGridView    gridItens;
        private Label           lblDetalhe;

        // ── Timer status ─────────────────────────────────────────────────────
        private Timer   _timer;
        private int     _paginaAtual = 0; // 0=Dashboard 1=Pedidos

        // ── Cores ────────────────────────────────────────────────────────────
        private static readonly Color CorSidebar    = Color.FromArgb(28, 37, 65);
        private static readonly Color CorTopBar     = Color.FromArgb(36, 48, 82);
        private static readonly Color CorBotaoAtivo = Color.FromArgb(52, 152, 219);
        private static readonly Color CorCard       = Color.FromArgb(44, 55, 95);
        private static readonly Color CorFundo      = Color.FromArgb(15, 22, 45);

        public Form1()
        {
            Text            = "Pedeai — Painel de Controle";
            Size            = new Size(1280, 780);
            MinimumSize     = new Size(1024, 680);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = CorFundo;
            Font            = new Font("Segoe UI", 9);
            BuildUI();
            Load += (_, __) => CarregarTudo();
        }

        // ════════════════════════════════════════════════════════════════════
        // UI BUILD
        // ════════════════════════════════════════════════════════════════════
        private void BuildUI()
        {
            // ── Top bar ──────────────────────────────────────────────────────
            pnlTopBar = new Panel
            {
                Dock        = DockStyle.Top,
                Height      = 52,
                BackColor   = CorTopBar,
                Padding     = new Padding(12, 10, 12, 0)
            };

            lblTitulo = new Label
            {
                Text      = "Dashboard",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize  = true,
                Left      = 16,
                Top       = 12
            };
            pnlTopBar.Controls.Add(lblTitulo);

            var btnAtualizar = new Button
            {
                Text      = "⟳  Atualizar",
                Top       = 10,
                Width     = 110,
                Height    = 30,
                Anchor    = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = CorBotaoAtivo,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnAtualizar.Location = new Point(pnlTopBar.Width - 122, 10);
            btnAtualizar.FlatAppearance.BorderSize = 0;
            btnAtualizar.Click += (_, __) => CarregarTudo();
            pnlTopBar.Controls.Add(btnAtualizar);

            // ── Sidebar ───────────────────────────────────────────────────────
            pnlSidebar = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 210,
                BackColor = CorSidebar,
                Padding   = new Padding(0, 8, 0, 0)
            };

            // Logo
            var lblLogo = new Label
            {
                Text      = "🍕 Pedeai",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 15, FontStyle.Bold),
                AutoSize  = false,
                Width     = 210,
                Height    = 52,
                TextAlign = ContentAlignment.MiddleCenter,
                Top       = 0,
                Left      = 0
            };
            pnlSidebar.Controls.Add(lblLogo);

            var separator = new Panel { Left = 16, Top = 56, Width = 178, Height = 1, BackColor = Color.FromArgb(60, 70, 110) };
            pnlSidebar.Controls.Add(separator);

            // Nav buttons
            int navY = 68;
            pnlSidebar.Controls.Add(BotaoNav("🏠  Dashboard",  navY, () => MostrarDashboard()));       navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("📋  Pedidos",    navY, () => MostrarPedidos()));          navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("🛒  Produtos",   navY, () => AbrirForm(new frmCadastroProduto()))); navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("🗂  Categorias", navY, () => AbrirForm(new frmCadastroCategoria()))); navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("👤  Clientes",   navY, () => AbrirForm(new frmCadastroCliente()))); navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("🏭  Fornecedores", navY, () => AbrirForm(new frmCadastroFornecedor()))); navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("🏷  Cupons",     navY, () => AbrirForm(new frmCadastroCupom())));

            // ── Content ───────────────────────────────────────────────────────
            pnlContent = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = CorFundo,
                Padding   = new Padding(20)
            };

            BuildDashboard();
            BuildPedidos();

            Controls.Add(pnlContent);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlTopBar);

            // ── Timer ─────────────────────────────────────────────────────────
            _timer = new Timer { Interval = 30000 };
            _timer.Tick += (_, __) => CarregarTudo();
            _timer.Start();
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

        // ════════════════════════════════════════════════════════════════════
        // DASHBOARD
        // ════════════════════════════════════════════════════════════════════
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

            // Tabela de últimos pedidos
            var lblUltimos = new Label
            {
                Text      = "Últimos Pedidos",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize  = true,
                Dock      = DockStyle.Top,
                Padding   = new Padding(0, 0, 0, 8)
            };

            gridPedidos = CriarGrid();
            gridPedidos.Dock = DockStyle.Fill;

            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0) };
            pnlGrid.Controls.Add(gridPedidos);
            pnlGrid.Controls.Add(lblUltimos);

            pnlDashboard.Controls.Add(pnlGrid);
            pnlDashboard.Controls.Add(pnlCards);
            pnlContent.Controls.Add(pnlDashboard);
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

        // ════════════════════════════════════════════════════════════════════
        // PEDIDOS
        // ════════════════════════════════════════════════════════════════════
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

            var lblSit = new Label { Text = "Situação:", ForeColor = Color.White, Left = 0, Top = 14, AutoSize = true };
            cmbFiltroPedido = new ComboBox
            {
                Left          = 68,
                Top           = 10,
                Width         = 160,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFiltroPedido.Items.AddRange(new object[] { "Todos", "0", "1", "2", "3", "4", "5", "6" });
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

            pnlFiltros.Controls.AddRange(new System.Windows.Forms.Control[]
                { lblSit, cmbFiltroPedido, lblDt, dtpFiltroPedido, btnFiltrar, btnTodos });

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

            // Ações
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
                ("✓ Confirmar",    1, Color.FromArgb(39, 174, 96)),
                ("⏳ Em Preparo",  2, Color.FromArgb(243, 156, 18)),
                ("✅ Pronto",      3, Color.FromArgb(22, 160, 133)),
                ("🚴 Saiu",        4, Color.FromArgb(52, 152, 219)),
                ("📦 Entregue",    5, Color.FromArgb(41, 128, 185)),
                ("✕ Cancelar",     6, Color.FromArgb(192, 57, 43)),
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

        // ════════════════════════════════════════════════════════════════════
        // NAVEGAÇÃO
        // ════════════════════════════════════════════════════════════════════
        private void MostrarDashboard()
        {
            pnlDashboard.Visible = true;
            pnlPedidos.Visible   = false;
            lblTitulo.Text       = "Dashboard";
            _paginaAtual         = 0;
            CarregarDashboard();
        }

        private void MostrarPedidos()
        {
            pnlDashboard.Visible = false;
            pnlPedidos.Visible   = true;
            lblTitulo.Text       = "Pedidos";
            _paginaAtual         = 1;
            CarregarPedidos();
        }

        private void AbrirForm(Form f)
        {
            f.ShowDialog(this);
        }

        // ════════════════════════════════════════════════════════════════════
        // CARGA DE DADOS
        // ════════════════════════════════════════════════════════════════════
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

                // Últimos pedidos no topo da dashboard (reusa gridPedidos se visível)
                if (_paginaAtual == 0)
                    gridPedidos.DataSource = _pedidoBLL.Listar();
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
                string sit  = cmbFiltroPedido?.SelectedIndex > 0 ? cmbFiltroPedido.SelectedItem?.ToString() : null;
                DateTime? dt = dtpFiltroPedido?.Value.Date;
                gridPedidos.DataSource = _pedidoBLL.Listar(sit, dt);
                gridItens.DataSource   = null;
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar pedidos: " + ex.Message); }
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
                    lblDetalhe.Text = $"Itens — Pedido #{pedido.pediNumero}  |  {PedidoBLL.LabelSituacao(pedido.pediSituacao)}  |  Total: {pedido.pediValor_Total:C}";
            }
            catch { }
        }

        private void AtualizarSituacaoPedido(int novaSit)
        {
            if (gridPedidos.SelectedRows.Count == 0) { MessageBox.Show("Selecione um pedido."); return; }
            var cod  = Convert.ToInt32(gridPedidos.SelectedRows[0].Cells["Codigo"].Value);
            var erro = _pedidoBLL.AtualizarSituacao(cod, novaSit);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            CarregarPedidos();
        }
    }
}

