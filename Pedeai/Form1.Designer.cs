using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.Forms;

namespace Pedeai
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlTopBar   = new Panel();
            pnlSidebar  = new Panel();
            pnlContent  = new Panel();
            pnlDashboard = new Panel();
            pnlPedidos  = new Panel();
            lblTitulo   = new Label();
            lblPedidosHoje = new Label();
            lblFaturamento = new Label();
            lblClientes    = new Label();
            lblPendentes   = new Label();
            gridPedidos    = new DataGridView();
            gridItens      = new DataGridView();
            lblDetalhe     = new Label();
            cmbFiltroPedido  = new ComboBox();
            dtpFiltroPedido  = new DateTimePicker();

            // ── Top bar ──────────────────────────────────────────────────
            pnlTopBar.Dock = DockStyle.Top; pnlTopBar.Height = 52;
            pnlTopBar.BackColor = Color.FromArgb(36, 48, 82);

            lblTitulo.Text = "Dashboard"; lblTitulo.ForeColor = Color.White;
            lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitulo.AutoSize = true; lblTitulo.Left = 16; lblTitulo.Top = 12;
            pnlTopBar.Controls.Add(lblTitulo);

            var btnAtualizar = new Button
            {
                Text = "⟳  Atualizar", Width = 110, Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnAtualizar.Location = new Point(pnlTopBar.Width - 122, 10);
            btnAtualizar.FlatAppearance.BorderSize = 0;
            btnAtualizar.Click += (_, __) => CarregarTudo();
            pnlTopBar.Controls.Add(btnAtualizar);

            // ── Sidebar ──────────────────────────────────────────────────
            pnlSidebar.Dock = DockStyle.Left; pnlSidebar.Width = 210;
            pnlSidebar.BackColor = Color.FromArgb(28, 37, 65);

            var lblLogo = new Label
            {
                Text = "🍕 Pedeai", ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                AutoSize = false, Width = 210, Height = 52, TextAlign = ContentAlignment.MiddleCenter,
                Top = 0, Left = 0
            };
            pnlSidebar.Controls.Add(lblLogo);
            pnlSidebar.Controls.Add(new Panel { Left = 16, Top = 56, Width = 178, Height = 1, BackColor = Color.FromArgb(60, 70, 110) });

            int navY = 68;
            pnlSidebar.Controls.Add(BotaoNav("🏠  Dashboard",   navY, MostrarDashboard));     navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("📋  Pedidos",     navY, MostrarPedidos));        navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("�  Financeiro",  navY, MostrarFinanceiro));     navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("�🛒  Produtos",    navY, () => AbrirForm(new frmCadastroProduto()))); navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("🗂  Categorias",  navY, () => AbrirForm(new frmCadastroCategoria()))); navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("👤  Clientes",    navY, () => AbrirForm(new frmCadastroCliente()))); navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("🏭  Fornecedores",navY, () => AbrirForm(new frmCadastroFornecedor()))); navY += 46;
            pnlSidebar.Controls.Add(BotaoNav("🏷  Cupons",      navY, () => AbrirForm(new frmCadastroCupom())));

            // ── Content ───────────────────────────────────────────────────
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = Color.FromArgb(15, 22, 45);
            pnlContent.Padding = new Padding(20);

            Controls.Add(pnlContent);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlTopBar);

            _timer = new Timer { Interval = 30000 };
            _timer.Tick += (_, __) => CarregarTudo();
            _timer.Start();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 22, 45);
            ClientSize = new Size(1264, 741);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(1024, 680);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Pedeai — Painel de Controle";

            Load += (_, __) => CarregarTudo();
        }

        private Panel      pnlTopBar;
        private Panel      pnlSidebar;
        private Panel      pnlContent;
        private Panel      pnlDashboard;
        private Panel      pnlPedidos;
        private Panel      pnlFinanceiro;
        private Label      lblTitulo;
        private Label      lblPedidosHoje;
        private Label      lblFaturamento;
        private Label      lblClientes;
        private Label      lblPendentes;
        private DataGridView gridPedidos;
        private DataGridView gridItens;
        private DataGridView gridFinanceiro;
        private Label      lblDetalhe;
        private ComboBox   cmbFiltroPedido;
        private DateTimePicker dtpFiltroPedido;
        private DateTimePicker dtpFinDe;
        private DateTimePicker dtpFinAte;
        private Label      lblFinResumo;
        private Timer      _timer;
    }
}

