using System;
using System.Drawing;
using System.Windows.Forms;

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
            pnlTopBar        = new System.Windows.Forms.Panel();
            lblTitulo        = new System.Windows.Forms.Label();
            lblLogoTopBar    = new System.Windows.Forms.Label();
            btnAtualizar     = new System.Windows.Forms.Button();
            btnSair          = new System.Windows.Forms.Button();
            btnLogoff        = new System.Windows.Forms.Button();
            btnClose         = new System.Windows.Forms.Button();
            btnMaximize      = new System.Windows.Forms.Button();
            btnMinimize      = new System.Windows.Forms.Button();
            pnlSidebar       = new System.Windows.Forms.Panel();
            pnlSidebarAccent = new System.Windows.Forms.Panel();
            pnlFooter        = new System.Windows.Forms.Panel();
            lblFooterEmpresa = new System.Windows.Forms.Label();
            lblFooterUsuario = new System.Windows.Forms.Label();
            pnlContent       = new System.Windows.Forms.Panel();
            pnlDashboard     = new System.Windows.Forms.Panel();
            pnlPedidos       = new System.Windows.Forms.Panel();
            lblPedidosHoje   = new System.Windows.Forms.Label();
            lblFaturamento   = new System.Windows.Forms.Label();
            lblClientes      = new System.Windows.Forms.Label();
            lblPendentes     = new System.Windows.Forms.Label();
            gridPedidos      = new System.Windows.Forms.DataGridView();
            gridItens        = new System.Windows.Forms.DataGridView();
            gridFinanceiro   = new System.Windows.Forms.DataGridView();
            lblDetalhe       = new System.Windows.Forms.Label();
            lblFinResumo     = new System.Windows.Forms.Label();
            cmbFiltroPedido  = new System.Windows.Forms.ComboBox();
            dtpFiltroPedido  = new System.Windows.Forms.DateTimePicker();
            _timer           = new System.Windows.Forms.Timer();

            // ── pnlTopBar ──────────────────────────────────────────────────
            pnlTopBar.Dock      = System.Windows.Forms.DockStyle.Top;
            pnlTopBar.Height    = 52;
            pnlTopBar.BackColor = System.Drawing.Color.FromArgb(36, 48, 82);
            pnlTopBar.SizeChanged += new System.EventHandler(this.PnlTopBar_SizeChanged);
            pnlTopBar.MouseDown   += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseDown);
            pnlTopBar.MouseMove   += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseMove);
            pnlTopBar.MouseUp     += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseUp);

            // ── lblTitulo ──────────────────────────────────────────────────
            lblTitulo.Text      = "Dashboard";
            lblTitulo.ForeColor = System.Drawing.Color.White;
            lblTitulo.Font      = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblTitulo.AutoSize  = true;
            lblTitulo.Left      = 16;
            lblTitulo.Top       = 12;
            lblTitulo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseDown);
            lblTitulo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseMove);
            lblTitulo.MouseUp   += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseUp);
            pnlTopBar.Controls.Add(lblTitulo);

            // ── lblLogoTopBar ──────────────────────────────────────────────
            lblLogoTopBar.Text      = "\U0001F355 PedeAi";
            lblLogoTopBar.ForeColor = System.Drawing.Color.White;
            lblLogoTopBar.Font      = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            lblLogoTopBar.AutoSize  = true;
            lblLogoTopBar.Top       = 11;
            lblLogoTopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseDown);
            lblLogoTopBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseMove);
            lblLogoTopBar.MouseUp   += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseUp);
            pnlTopBar.Controls.Add(lblLogoTopBar);

            // ── btnAtualizar ───────────────────────────────────────────────
            btnAtualizar.Text      = "\u27F3  Atualizar";
            btnAtualizar.Width     = 110;
            btnAtualizar.Height    = 30;
            btnAtualizar.Anchor    = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnAtualizar.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnAtualizar.ForeColor = System.Drawing.Color.White;
            btnAtualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAtualizar.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btnAtualizar.Cursor    = System.Windows.Forms.Cursors.Hand;
            btnAtualizar.FlatAppearance.BorderSize = 0;
            btnAtualizar.Click += new System.EventHandler(this.BtnAtualizar_Click);
            pnlTopBar.Controls.Add(btnAtualizar);

            // ── btnSair ────────────────────────────────────────────────────
            btnSair.Text      = "Sair";
            btnSair.Width     = 72;
            btnSair.Height    = 30;
            btnSair.BackColor = System.Drawing.Color.FromArgb(80, 40, 35);
            btnSair.ForeColor = System.Drawing.Color.FromArgb(200, 130, 120);
            btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSair.Font      = new System.Drawing.Font("Segoe UI", 9F);
            btnSair.Cursor    = System.Windows.Forms.Cursors.Hand;
            btnSair.Visible   = false;
            btnSair.FlatAppearance.BorderSize = 0;
            pnlTopBar.Controls.Add(btnSair);

            // ── btnLogoff ──────────────────────────────────────────────────
            btnLogoff.Text      = "\u21AA Trocar Usu\u00E1rio";
            btnLogoff.Width     = 140;
            btnLogoff.Height    = 30;
            btnLogoff.BackColor = System.Drawing.Color.FromArgb(36, 52, 88);
            btnLogoff.ForeColor = System.Drawing.Color.FromArgb(150, 185, 230);
            btnLogoff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnLogoff.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
            btnLogoff.Cursor    = System.Windows.Forms.Cursors.Hand;
            btnLogoff.FlatAppearance.BorderSize  = 1;
            btnLogoff.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(55, 75, 120);
            btnLogoff.Click += new System.EventHandler(this.BtnLogoff_Click);
            pnlTopBar.Controls.Add(btnLogoff);

            // ── btnClose ───────────────────────────────────────────────────
            btnClose.Text      = "\u2715";
            btnClose.Width     = 40;
            btnClose.Height    = 52;
            btnClose.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            btnClose.ForeColor = System.Drawing.Color.White;
            btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnClose.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnClose.Cursor    = System.Windows.Forms.Cursors.Hand;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            pnlTopBar.Controls.Add(btnClose);

            // ── btnMaximize ────────────────────────────────────────────────
            btnMaximize.Text      = "\u25A1";
            btnMaximize.Width     = 36;
            btnMaximize.Height    = 52;
            btnMaximize.BackColor = System.Drawing.Color.FromArgb(52, 68, 105);
            btnMaximize.ForeColor = System.Drawing.Color.White;
            btnMaximize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnMaximize.Font      = new System.Drawing.Font("Segoe UI", 10F);
            btnMaximize.Cursor    = System.Windows.Forms.Cursors.Hand;
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.Click += new System.EventHandler(this.BtnMaximize_Click);
            pnlTopBar.Controls.Add(btnMaximize);

            // ── btnMinimize ────────────────────────────────────────────────
            btnMinimize.Text      = "\u2500";
            btnMinimize.Width     = 36;
            btnMinimize.Height    = 52;
            btnMinimize.BackColor = System.Drawing.Color.FromArgb(52, 68, 105);
            btnMinimize.ForeColor = System.Drawing.Color.White;
            btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnMinimize.Font      = new System.Drawing.Font("Segoe UI", 10F);
            btnMinimize.Cursor    = System.Windows.Forms.Cursors.Hand;
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.Click += new System.EventHandler(this.BtnMinimize_Click);
            pnlTopBar.Controls.Add(btnMinimize);

            // ── pnlSidebar ─────────────────────────────────────────────────
            pnlSidebar.Dock      = System.Windows.Forms.DockStyle.Left;
            pnlSidebar.Width     = 210;
            pnlSidebar.BackColor = System.Drawing.Color.FromArgb(28, 37, 65);

            pnlSidebarAccent.Left      = 0;
            pnlSidebarAccent.Top       = 0;
            pnlSidebarAccent.Width     = 210;
            pnlSidebarAccent.Height    = 4;
            pnlSidebarAccent.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            pnlSidebar.Controls.Add(pnlSidebarAccent);

            // ── pnlFooter ──────────────────────────────────────────────────
            pnlFooter.Dock      = System.Windows.Forms.DockStyle.Bottom;
            pnlFooter.Height    = 28;
            pnlFooter.BackColor = System.Drawing.Color.FromArgb(18, 25, 50);
            pnlFooter.SizeChanged += new System.EventHandler(this.PnlFooter_SizeChanged);

            lblFooterEmpresa.Text      = "";
            lblFooterEmpresa.ForeColor = System.Drawing.Color.FromArgb(90, 120, 170);
            lblFooterEmpresa.Font      = new System.Drawing.Font("Segoe UI", 8F);
            lblFooterEmpresa.AutoSize  = true;
            lblFooterEmpresa.Left      = 12;
            lblFooterEmpresa.Top       = 7;

            lblFooterUsuario.Text      = "\U0001F464  Sistema";
            lblFooterUsuario.ForeColor = System.Drawing.Color.FromArgb(130, 165, 220);
            lblFooterUsuario.Font      = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            lblFooterUsuario.AutoSize  = true;
            lblFooterUsuario.Top       = 7;

            pnlFooter.Controls.Add(lblFooterEmpresa);
            pnlFooter.Controls.Add(lblFooterUsuario);

            // ── pnlContent ─────────────────────────────────────────────────
            pnlContent.Dock      = System.Windows.Forms.DockStyle.Fill;
            pnlContent.BackColor = System.Drawing.Color.FromArgb(15, 22, 45);
            pnlContent.Padding   = new System.Windows.Forms.Padding(20);

            // ── _timer ─────────────────────────────────────────────────────
            _timer.Interval = 30000;
            _timer.Tick += new System.EventHandler(this.Timer_Tick);

            // ── Form ───────────────────────────────────────────────────────
            Controls.Add(pnlContent);
            Controls.Add(pnlFooter);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlTopBar);

            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            BackColor           = System.Drawing.Color.FromArgb(15, 22, 45);
            ClientSize          = new System.Drawing.Size(1100, 660);
            Font                = new System.Drawing.Font("Segoe UI", 9F);
            FormBorderStyle     = System.Windows.Forms.FormBorderStyle.None;
            KeyPreview          = true;
            MinimumSize         = new System.Drawing.Size(920, 560);
            StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text                = "PedeAi \u2014 Painel de Controle";
            WindowState         = System.Windows.Forms.FormWindowState.Maximized;
            KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
        }

        private void BtnClose_Click(object sender, System.EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void BtnMaximize_Click(object sender, System.EventArgs e)
        {
            WindowState = WindowState == System.Windows.Forms.FormWindowState.Maximized
                ? System.Windows.Forms.FormWindowState.Normal
                : System.Windows.Forms.FormWindowState.Maximized;
        }

        private void BtnMinimize_Click(object sender, System.EventArgs e)
        {
            WindowState = System.Windows.Forms.FormWindowState.Minimized;
        }

        private void BtnAtualizar_Click(object sender, System.EventArgs e) { CarregarTudo(); }

        private void Timer_Tick(object sender, System.EventArgs e) { CarregarTudo(); }

        private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Home)
            {
                e.Handled = true;
                BtnLogoff_Click(this, System.EventArgs.Empty);
            }
        }

        private void PnlTopBar_SizeChanged(object sender, System.EventArgs e)
        {
            btnClose.Left    = pnlTopBar.Width - btnClose.Width;
            btnMaximize.Left = btnClose.Left    - btnMaximize.Width - 1;
            btnMinimize.Left = btnMaximize.Left - btnMinimize.Width - 1;
            btnClose.Top = btnMaximize.Top = btnMinimize.Top = 0;
            btnAtualizar.Left = btnMinimize.Left - btnAtualizar.Width - 16;
            btnSair.Left      = btnAtualizar.Left - btnSair.Width - 6;
            btnLogoff.Left    = btnSair.Left      - btnLogoff.Width - 8;
            int acTop = (pnlTopBar.Height - btnAtualizar.Height) / 2;
            btnAtualizar.Top = btnSair.Top = btnLogoff.Top = acTop;
            lblLogoTopBar.Left = (pnlTopBar.Width - lblLogoTopBar.PreferredWidth) / 2;
            lblLogoTopBar.Top  = (pnlTopBar.Height - lblLogoTopBar.PreferredHeight) / 2;
            lblTitulo.Left     = 16;
            lblTitulo.Top      = (pnlTopBar.Height - lblTitulo.PreferredHeight) / 2;
        }

        private void PnlFooter_SizeChanged(object sender, System.EventArgs e)
        {
            lblFooterUsuario.Left = pnlFooter.Width - lblFooterUsuario.PreferredWidth - 12;
        }

        private void TopBar_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _dragging  = true;
                _dragStart = System.Windows.Forms.Cursor.Position;
            }
        }

        private void TopBar_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_dragging && WindowState == System.Windows.Forms.FormWindowState.Normal)
            {
                System.Drawing.Point delta = new System.Drawing.Point(
                    System.Windows.Forms.Cursor.Position.X - _dragStart.X,
                    System.Windows.Forms.Cursor.Position.Y - _dragStart.Y);
                Location   = new System.Drawing.Point(Location.X + delta.X, Location.Y + delta.Y);
                _dragStart = System.Windows.Forms.Cursor.Position;
            }
        }

        private void TopBar_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _dragging = false;
        }

        private System.Windows.Forms.Panel          pnlTopBar;
        private System.Windows.Forms.Panel          pnlSidebar;
        private System.Windows.Forms.Panel          pnlSidebarAccent;
        private System.Windows.Forms.Panel          pnlFooter;
        private System.Windows.Forms.Panel          pnlContent;
        private System.Windows.Forms.Panel          pnlDashboard;
        private System.Windows.Forms.Panel          pnlPedidos;
        private System.Windows.Forms.Panel          pnlFinanceiro;
        private System.Windows.Forms.Label          lblTitulo;
        private System.Windows.Forms.Label          lblLogoTopBar;
        private System.Windows.Forms.Label          lblFooterEmpresa;
        private System.Windows.Forms.Label          lblFooterUsuario;
        private System.Windows.Forms.Label          lblPedidosHoje;
        private System.Windows.Forms.Label          lblFaturamento;
        private System.Windows.Forms.Label          lblClientes;
        private System.Windows.Forms.Label          lblPendentes;
        private System.Windows.Forms.Label          lblDetalhe;
        private System.Windows.Forms.Label          lblFinResumo;
        private System.Windows.Forms.Button         btnLogoff;
        private System.Windows.Forms.Button         btnAtualizar;
        private System.Windows.Forms.Button         btnSair;
        private System.Windows.Forms.Button         btnClose;
        private System.Windows.Forms.Button         btnMaximize;
        private System.Windows.Forms.Button         btnMinimize;
        private System.Windows.Forms.DataGridView   gridPedidos;
        private System.Windows.Forms.DataGridView   gridItens;
        private System.Windows.Forms.DataGridView   gridFinanceiro;
        private System.Windows.Forms.ComboBox       cmbFiltroPedido;
        private System.Windows.Forms.DateTimePicker dtpFiltroPedido;
        private System.Windows.Forms.DateTimePicker dtpFinDe;
        private System.Windows.Forms.DateTimePicker dtpFinAte;
        private System.Windows.Forms.Timer          _timer;
        private bool                                _dragging;
        private System.Drawing.Point                _dragStart;
    }
}
