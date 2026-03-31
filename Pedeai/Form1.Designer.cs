using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
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
            btnAtualizar.FlatAppearance.BorderSize = 0;
            btnAtualizar.Click += (_, __) => CarregarTudo();
            pnlTopBar.Controls.Add(btnAtualizar);

            var btnSair = new Button
            {
                Text = "Sair", Width = 72, Height = 30,
                BackColor = Color.FromArgb(80, 40, 35), ForeColor = Color.FromArgb(200, 130, 120),
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F), Cursor = Cursors.Hand,
                Visible = false  // substituido pelo botao X no canto
            };
            btnSair.FlatAppearance.BorderSize = 0;
            pnlTopBar.Controls.Add(btnSair);

            btnLogoff = new Button
            {
                Text = "\u21aa Trocar Usu\u00e1rio", Width = 140, Height = 30,
                BackColor = Color.FromArgb(36, 52, 88), ForeColor = Color.FromArgb(150, 185, 230),
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8.5F), Cursor = Cursors.Hand
            };
            btnLogoff.FlatAppearance.BorderSize = 1;
            btnLogoff.FlatAppearance.BorderColor = Color.FromArgb(55, 75, 120);
            btnLogoff.Click += BtnLogoff_Click;
            pnlTopBar.Controls.Add(btnLogoff);

            // ── Botoes de janela (fechar / maximizar / minimizar) ─────────────
            var btnClose = new Button
            {
                Text      = "✕",
                Width     = 40, Height = 52,
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (_, __) => Application.Exit();

            var btnMaximize = new Button
            {
                Text      = "□",
                Width     = 36, Height = 52,
                BackColor = Color.FromArgb(52, 68, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10F),
                Cursor    = Cursors.Hand
            };
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.Click += (_, __) =>
            {
                WindowState = WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;
            };

            var btnMinimize = new Button
            {
                Text      = "─",
                Width     = 36, Height = 52,
                BackColor = Color.FromArgb(52, 68, 105),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10F),
                Cursor    = Cursors.Hand
            };
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.Click += (_, __) => WindowState = FormWindowState.Minimized;

            pnlTopBar.Controls.Add(btnClose);
            pnlTopBar.Controls.Add(btnMaximize);
            pnlTopBar.Controls.Add(btnMinimize);

            // ── Logo centralizada no TopBar ───────────────────────────────
            lblLogoTopBar = new Label
            {
                Text      = "🍕 PedeAi",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 17F, FontStyle.Bold),
                AutoSize  = true,
                Top       = 11
            };
            pnlTopBar.Controls.Add(lblLogoTopBar);

            pnlTopBar.SizeChanged += (_, __) =>
            {
                // Botões janela — direita, colados ao topo
                btnClose.Left    = pnlTopBar.Width - btnClose.Width;
                btnMaximize.Left = btnClose.Left   - btnMaximize.Width - 1;
                btnMinimize.Left = btnMaximize.Left- btnMinimize.Width - 1;
                btnClose.Top = btnMaximize.Top = btnMinimize.Top = 0;

                // Botões de ação — à esquerda dos botões de janela
                btnAtualizar.Left = btnMinimize.Left - btnAtualizar.Width - 16;
                btnSair.Left      = btnAtualizar.Left - btnSair.Width - 6;
                btnLogoff.Left    = btnSair.Left      - btnLogoff.Width - 8;
                int acTop = (pnlTopBar.Height - btnAtualizar.Height) / 2;
                btnAtualizar.Top = btnSair.Top = btnLogoff.Top = acTop;

                // Logo PedeAi — centralizada
                lblLogoTopBar.Left = (pnlTopBar.Width - lblLogoTopBar.PreferredWidth) / 2;
                lblLogoTopBar.Top  = (pnlTopBar.Height - lblLogoTopBar.PreferredHeight) / 2;

                // Título da página — alinhado à esquerda (respeita sidebar)
                lblTitulo.Left = 16;
                lblTitulo.Top  = (pnlTopBar.Height - lblTitulo.PreferredHeight) / 2;
            };

            // Arrastar janela pelo top bar
            bool _dragging = false;
            System.Drawing.Point _dragStart = System.Drawing.Point.Empty;
            MouseEventHandler mdHandler = (s, me) => { if (me.Button == MouseButtons.Left) { _dragging = true; _dragStart = System.Windows.Forms.Cursor.Position; } };
            MouseEventHandler mmHandler = (s, me) =>
            {
                if (_dragging && WindowState == FormWindowState.Normal)
                {
                    var delta = new System.Drawing.Point(System.Windows.Forms.Cursor.Position.X - _dragStart.X, System.Windows.Forms.Cursor.Position.Y - _dragStart.Y);
                    Location  = new System.Drawing.Point(Location.X + delta.X, Location.Y + delta.Y);
                    _dragStart = System.Windows.Forms.Cursor.Position;
                }
            };
            MouseEventHandler muHandler = (s, me) => { _dragging = false; };
            pnlTopBar.MouseDown += mdHandler;
            pnlTopBar.MouseMove += mmHandler;
            pnlTopBar.MouseUp   += muHandler;
            lblTitulo.MouseDown    += mdHandler;
            lblTitulo.MouseMove    += mmHandler;
            lblTitulo.MouseUp      += muHandler;
            lblLogoTopBar.MouseDown += mdHandler;
            lblLogoTopBar.MouseMove += mmHandler;
            lblLogoTopBar.MouseUp   += muHandler;

            // ── Sidebar ──────────────────────────────────────────────────
            pnlSidebar.Dock = DockStyle.Left; pnlSidebar.Width = 210;
            pnlSidebar.BackColor = Color.FromArgb(28, 37, 65);

            // Linha colorida no topo da sidebar
            pnlSidebar.Controls.Add(new Panel { Left = 0, Top = 0, Width = 210, Height = 4, BackColor = Color.FromArgb(52, 152, 219) });

            int navY = 8;
            void NavSe(string modulo, string texto, Action acao)
            {
                if (!UsuarioSessao.TemModulo(modulo)) return;
                pnlSidebar.Controls.Add(BotaoNav(texto, navY, acao));
                navY += 46;
            }
            NavSe("Dashboard",   "\U0001F3E0  Dashboard",    MostrarDashboard);
            NavSe("Pedidos",     "\U0001F4CB  Pedidos",      MostrarPedidos);
            NavSe("Financeiro",  "\U0001F4B0  Financeiro",   MostrarFinanceiro);
            NavSe("Produtos",    "\U0001F6D2  Produtos",     () => AbrirForm(new frmCadastroProduto()));
            NavSe("Categorias",  "\U0001F5C2  Categorias",   () => AbrirForm(new frmCadastroCategoria()));
            NavSe("Clientes",    "\U0001F464  Clientes",     () => AbrirForm(new frmCadastroCliente()));
            NavSe("Fornecedores","\U0001F3ED  Fornecedores", () => AbrirForm(new frmCadastroFornecedor()));
            NavSe("Cupons",          "\U0001F3F7  Cupons",          () => AbrirForm(new frmCadastroCupom()));
            NavSe("EntradaMercadoria", "\U0001F4E6  Entrada Mercad.", () => AbrirForm(new frmEntradaMercadoria()));
            NavSe("Avisos",           "\U0001F514  Avisos",          () => AbrirForm(new frmAvisos()));
            NavSe("Empresa",         "\U0001F3E2  Empresa",         MostrarEmpresa);

            // ── Footer ───────────────────────────────────────────────────
            pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = Color.FromArgb(18, 25, 50) };
            string _empNom = "";
            try { _empNom = new EmpresaBLL().Carregar()?.empNome_Fantasia ?? ""; } catch { }
            lblFooterEmpresa = new Label
            {
                Text = _empNom, ForeColor = Color.FromArgb(90, 120, 170),
                Font = new Font("Segoe UI", 8F), AutoSize = true, Left = 12, Top = 7
            };
            lblFooterUsuario = new Label
            {
                Text = "\U0001F464  " + UsuarioSessao.NomeAtual,
                ForeColor = Color.FromArgb(130, 165, 220),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold), AutoSize = true, Top = 7
            };
            pnlFooter.SizeChanged += (_, __) =>
                lblFooterUsuario.Left = pnlFooter.Width - lblFooterUsuario.PreferredWidth - 12;
            pnlFooter.Controls.Add(lblFooterEmpresa);
            pnlFooter.Controls.Add(lblFooterUsuario);

            // ── Content ───────────────────────────────────────────────────
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = Color.FromArgb(15, 22, 45);
            pnlContent.Padding = new Padding(20);

            Controls.Add(pnlContent);
            Controls.Add(pnlFooter);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlTopBar);

            _timer = new Timer { Interval = 30000 };
            _timer.Tick += (_, __) => CarregarTudo();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 22, 45);
            ClientSize = new Size(1100, 660);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(920, 560);
            StartPosition = FormStartPosition.CenterScreen;            FormBorderStyle = FormBorderStyle.None;            Text = "PedeAi — Painel de Controle";            WindowState = FormWindowState.Maximized;
            KeyPreview = true;
            KeyDown += (_, ke) => { if (ke.KeyCode == Keys.Home) { ke.Handled = true; BtnLogoff_Click(this, EventArgs.Empty); } };

        }

        private Panel      pnlTopBar;
        private Panel      pnlFooter;
        private Label      lblFooterEmpresa;
        private Label      lblFooterUsuario;
        private Button     btnLogoff;
        private Panel      pnlSidebar;
        private Panel      pnlContent;
        private Panel      pnlDashboard;
        private Panel      pnlPedidos;
        private Panel      pnlFinanceiro;
        private Label      lblTitulo;
        private Label      lblLogoTopBar;
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

