using System;
using System.Data;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using Pedeai.DB;
using Pedeai.Forms;

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
        Size = new Size(1280, 800);
        MinimumSize = new Size(900, 600);
        StartPosition = FormStartPosition.CenterScreen;
        Icon = SystemIcons.Application;

        BuildUI();
        CarregarTudo();

        _timerPedidos.Elapsed += (_, _) => InvokeOnUI(CarregarPedidos);
        _timerPedidos.AutoReset = true;
        _timerPedidos.Start();
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
        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 700 };

        // Painel esquerdo — lista de pedidos
        var topBar = new Panel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(4) };
        var lblFiltro = new Label { Text = "Status:", Width = 55, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Left };
        cmbStatusPedido = new ComboBox { Dock = DockStyle.Left, Width = 160 };
        cmbStatusPedido.Items.AddRange(new[] { "Todos", "Pendente", "Confirmado", "Em Preparo", "Pronto", "Saiu p/ Entrega", "Entregue", "Cancelado" });
        cmbStatusPedido.SelectedIndex = 0;
        var btnRefresh = CriarBotao("⟳ Atualizar", Color.FromArgb(63, 81, 181));
        btnRefresh.Width = 100;
        btnRefresh.Click += (_, _) => CarregarPedidos();
        topBar.Controls.AddRange(new Control[] { lblFiltro, cmbStatusPedido, btnRefresh });

        gridPedidos = CriarGrid();
        gridPedidos.SelectionChanged += GridPedidos_SelectionChanged;

        split.Panel1.Controls.Add(gridPedidos);
        split.Panel1.Controls.Add(topBar);

        // Painel direito — detalhes do pedido
        var pDetail = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        lblPedidoInfo = new Label { Dock = DockStyle.Top, Height = 80, Font = new Font("Segoe UI", 9), AutoSize = false };

        gridItens = CriarGrid();
        gridItens.Height = 200;
        gridItens.Dock = DockStyle.Bottom;

        var pBotoes = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 42, Padding = new Padding(2) };
        btnConfirmar = CriarBotao("Confirmar", Color.FromArgb(33, 150, 243));
        btnEmPreparo = CriarBotao("Em Preparo", Color.FromArgb(255, 152, 0));
        btnPronto = CriarBotao("Pronto", Color.FromArgb(76, 175, 80));
        btnEntregue = CriarBotao("Entregue", Color.FromArgb(0, 150, 136));
        btnCancelar = CriarBotao("Cancelar", Color.FromArgb(244, 67, 54));
        foreach (var b in new[] { btnConfirmar, btnEmPreparo, btnPronto, btnEntregue, btnCancelar })
        {
            b.Width = 95;
            b.Click += BtnStatus_Click;
            pBotoes.Controls.Add(b);
        }

        pDetail.Controls.Add(gridItens);
        pDetail.Controls.Add(pBotoes);
        pDetail.Controls.Add(lblPedidoInfo);
        split.Panel2.Controls.Add(pDetail);

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
        var topBar = new Panel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(4) };
        var btnRefresh = CriarBotao("⟳ Atualizar", Color.FromArgb(63, 81, 181));
        btnRefresh.Width = 100;
        btnRefresh.Click += (_, _) => CarregarProdutos();
        btnToggleProduto = CriarBotao("Ativar/Desativar", Color.FromArgb(255, 152, 0));
        btnToggleProduto.Width = 140;
        btnToggleProduto.Click += BtnToggleProduto_Click;
        var btnCadastrar = CriarBotao("Cadastrar", Color.FromArgb(0, 150, 136));
        btnCadastrar.Width = 110;
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
        var topBar = new Panel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(4) };
        var lblBusca = new Label { Text = "Buscar:", Width = 55, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Left };
        txtBuscaCliente = new TextBox { Dock = DockStyle.Left, Width = 250 };
        var btnBuscar = CriarBotao("Buscar", Color.FromArgb(63, 81, 181));
        btnBuscar.Width = 80;
        btnBuscar.Click += (_, _) => CarregarClientes();
        txtBuscaCliente.KeyDown += (_, k) => { if (k.KeyCode == Keys.Enter) CarregarClientes(); };
        var btnCadastrar = CriarBotao("Cadastrar", Color.FromArgb(0, 150, 136));
        btnCadastrar.Width = 110;
        btnCadastrar.Click += (_, _) => { new frmCadastroCliente().ShowDialog(this); CarregarClientes(); };
        topBar.Controls.AddRange(new Control[] { lblBusca, txtBuscaCliente, btnBuscar, btnCadastrar });

        gridClientes = CriarGrid();
        tabClientes.Controls.Add(gridClientes);
        tabClientes.Controls.Add(topBar);
    }

    // ─── Fornecedores ─────────────────────────────────────────────────────────

    private void BuildFornecedoresTab()
    {
        var topBar = new Panel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(4) };
        var btnRefresh = CriarBotao("⟳ Atualizar", Color.FromArgb(63, 81, 181));
        btnRefresh.Width = 100;
        btnRefresh.Click += (_, _) => CarregarFornecedores();
        var btnCadastrar = CriarBotao("Cadastrar", Color.FromArgb(0, 150, 136));
        btnCadastrar.Width = 110;
        btnCadastrar.Click += (_, _) => { new frmCadastroFornecedor().ShowDialog(this); CarregarFornecedores(); };
        topBar.Controls.AddRange(new Control[] { btnRefresh, btnCadastrar });

        gridFornecedores = CriarGrid();
        tabFornecedores.Controls.Add(gridFornecedores);
        tabFornecedores.Controls.Add(topBar);
    }

    // ─── Cupons ───────────────────────────────────────────────────────────────

    private void BuildCuponsTab()
    {
        var topBar = new Panel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(4) };
        var btnRefresh = CriarBotao("⟳ Atualizar", Color.FromArgb(63, 81, 181));
        btnRefresh.Width = 100;
        btnRefresh.Click += (_, _) => CarregarCupons();
        var btnCadastrar = CriarBotao("Cadastrar", Color.FromArgb(0, 150, 136));
        btnCadastrar.Width = 110;
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

    private void StatusBarMensagem(string msg) =>
        BeginInvoke(new Action(() => Text = $"Pedeai — {msg}"));

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _timerPedidos.Stop();
        base.OnFormClosed(e);
    }
}
}

