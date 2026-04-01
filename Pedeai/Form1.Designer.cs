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
            this.components = new System.ComponentModel.Container();
            this.pnlTopBar = new System.Windows.Forms.Panel();
            this.lblLogoTopBar = new System.Windows.Forms.Label();
            this.btnAtualizar = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.btnLogoff = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMaximize = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.pnlSidebarAccent = new System.Windows.Forms.Panel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lblFooterEmpresa = new System.Windows.Forms.Label();
            this.lblFooterUsuario = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlDashboard = new System.Windows.Forms.Panel();
            this.pnlPedidos = new System.Windows.Forms.Panel();
            this.lblPedidosHoje = new System.Windows.Forms.Label();
            this.lblFaturamento = new System.Windows.Forms.Label();
            this.lblClientes = new System.Windows.Forms.Label();
            this.lblPendentes = new System.Windows.Forms.Label();
            this.gridPedidos = new System.Windows.Forms.DataGridView();
            this.gridItens = new System.Windows.Forms.DataGridView();
            this.gridFinanceiro = new System.Windows.Forms.DataGridView();
            this.lblDetalhe = new System.Windows.Forms.Label();
            this.lblFinResumo = new System.Windows.Forms.Label();
            this.cmbFiltroPedido = new System.Windows.Forms.ComboBox();
            this.dtpFiltroPedido = new System.Windows.Forms.DateTimePicker();
            this._timer = new System.Windows.Forms.Timer(this.components);
            this.pnlTopBar.SuspendLayout();
            this.pnlSidebar.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPedidos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridItens)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridFinanceiro)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTopBar
            // 
            this.pnlTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            this.pnlTopBar.Controls.Add(this.btnSair);
            this.pnlTopBar.Controls.Add(this.lblTitulo);
            this.pnlTopBar.Controls.Add(this.lblLogoTopBar);
            this.pnlTopBar.Controls.Add(this.btnAtualizar);
            this.pnlTopBar.Controls.Add(this.btnLogoff);
            this.pnlTopBar.Controls.Add(this.btnClose);
            this.pnlTopBar.Controls.Add(this.btnMaximize);
            this.pnlTopBar.Controls.Add(this.btnMinimize);
            this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTopBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlTopBar.Name = "pnlTopBar";
            this.pnlTopBar.Size = new System.Drawing.Size(1283, 60);
            this.pnlTopBar.TabIndex = 3;
            this.pnlTopBar.SizeChanged += new System.EventHandler(this.PnlTopBar_SizeChanged);
            this.pnlTopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseDown);
            this.pnlTopBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseMove);
            this.pnlTopBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseUp);
            // 
            // lblLogoTopBar
            // 
            this.lblLogoTopBar.AutoSize = true;
            this.lblLogoTopBar.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblLogoTopBar.ForeColor = System.Drawing.Color.White;
            this.lblLogoTopBar.Location = new System.Drawing.Point(551, 8);
            this.lblLogoTopBar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogoTopBar.Name = "lblLogoTopBar";
            this.lblLogoTopBar.Size = new System.Drawing.Size(178, 31);
            this.lblLogoTopBar.TabIndex = 1;
            this.lblLogoTopBar.Text = "🍴 RanGoFood";
            this.lblLogoTopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseDown);
            this.lblLogoTopBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseMove);
            this.lblLogoTopBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseUp);
            // 
            // btnAtualizar
            // 
            this.btnAtualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAtualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnAtualizar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAtualizar.FlatAppearance.BorderSize = 0;
            this.btnAtualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAtualizar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAtualizar.ForeColor = System.Drawing.Color.White;
            this.btnAtualizar.Location = new System.Drawing.Point(979, 14);
            this.btnAtualizar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAtualizar.Name = "btnAtualizar";
            this.btnAtualizar.Size = new System.Drawing.Size(128, 35);
            this.btnAtualizar.TabIndex = 2;
            this.btnAtualizar.Text = "⟳  Atualizar";
            this.btnAtualizar.UseVisualStyleBackColor = false;
            this.btnAtualizar.Click += new System.EventHandler(this.BtnAtualizar_Click);
            // 
            // btnSair
            // 
            this.btnSair.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(40)))), ((int)(((byte)(35)))));
            this.btnSair.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSair.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(130)))), ((int)(((byte)(120)))));
            this.btnSair.Location = new System.Drawing.Point(245, 11);
            this.btnSair.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(84, 35);
            this.btnSair.TabIndex = 3;
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = false;
            this.btnSair.Visible = false;
            // 
            // btnLogoff
            // 
            this.btnLogoff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(52)))), ((int)(((byte)(88)))));
            this.btnLogoff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogoff.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(75)))), ((int)(((byte)(120)))));
            this.btnLogoff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogoff.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnLogoff.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(185)))), ((int)(((byte)(230)))));
            this.btnLogoff.Location = new System.Drawing.Point(787, 14);
            this.btnLogoff.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLogoff.Name = "btnLogoff";
            this.btnLogoff.Size = new System.Drawing.Size(163, 35);
            this.btnLogoff.TabIndex = 4;
            this.btnLogoff.Text = "↪ Trocar Usuário";
            this.btnLogoff.UseVisualStyleBackColor = false;
            this.btnLogoff.Click += new System.EventHandler(this.BtnLogoff_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1236, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(47, 60);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "✕";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // btnMaximize
            // 
            this.btnMaximize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(68)))), ((int)(((byte)(105)))));
            this.btnMaximize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMaximize.FlatAppearance.BorderSize = 0;
            this.btnMaximize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaximize.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnMaximize.ForeColor = System.Drawing.Color.White;
            this.btnMaximize.Location = new System.Drawing.Point(1195, 0);
            this.btnMaximize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMaximize.Name = "btnMaximize";
            this.btnMaximize.Size = new System.Drawing.Size(42, 60);
            this.btnMaximize.TabIndex = 6;
            this.btnMaximize.Text = "□";
            this.btnMaximize.UseVisualStyleBackColor = false;
            this.btnMaximize.Click += new System.EventHandler(this.BtnMaximize_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(68)))), ((int)(((byte)(105)))));
            this.btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnMinimize.ForeColor = System.Drawing.Color.White;
            this.btnMinimize.Location = new System.Drawing.Point(1154, 0);
            this.btnMinimize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(42, 60);
            this.btnMinimize.TabIndex = 7;
            this.btnMinimize.Text = "─";
            this.btnMinimize.UseVisualStyleBackColor = false;
            this.btnMinimize.Click += new System.EventHandler(this.BtnMinimize_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(35, 14);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(109, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Dashboard";
            this.lblTitulo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseDown);
            this.lblTitulo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseMove);
            this.lblTitulo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseUp);
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(37)))), ((int)(((byte)(65)))));
            this.pnlSidebar.Controls.Add(this.pnlSidebarAccent);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 60);
            this.pnlSidebar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(245, 702);
            this.pnlSidebar.TabIndex = 2;
            // 
            // pnlSidebarAccent
            // 
            this.pnlSidebarAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.pnlSidebarAccent.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebarAccent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlSidebarAccent.Name = "pnlSidebarAccent";
            this.pnlSidebarAccent.Size = new System.Drawing.Size(245, 5);
            this.pnlSidebarAccent.TabIndex = 0;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(25)))), ((int)(((byte)(50)))));
            this.pnlFooter.Controls.Add(this.lblFooterEmpresa);
            this.pnlFooter.Controls.Add(this.lblFooterUsuario);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(245, 730);
            this.pnlFooter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1038, 32);
            this.pnlFooter.TabIndex = 1;
            this.pnlFooter.SizeChanged += new System.EventHandler(this.PnlFooter_SizeChanged);
            // 
            // lblFooterEmpresa
            // 
            this.lblFooterEmpresa.AutoSize = true;
            this.lblFooterEmpresa.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFooterEmpresa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(120)))), ((int)(((byte)(170)))));
            this.lblFooterEmpresa.Location = new System.Drawing.Point(14, 8);
            this.lblFooterEmpresa.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFooterEmpresa.Name = "lblFooterEmpresa";
            this.lblFooterEmpresa.Size = new System.Drawing.Size(0, 13);
            this.lblFooterEmpresa.TabIndex = 0;
            // 
            // lblFooterUsuario
            // 
            this.lblFooterUsuario.AutoSize = true;
            this.lblFooterUsuario.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFooterUsuario.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.lblFooterUsuario.Location = new System.Drawing.Point(0, 8);
            this.lblFooterUsuario.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFooterUsuario.Name = "lblFooterUsuario";
            this.lblFooterUsuario.Size = new System.Drawing.Size(66, 13);
            this.lblFooterUsuario.TabIndex = 1;
            this.lblFooterUsuario.Text = "👤  Sistema";
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(22)))), ((int)(((byte)(45)))));
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(245, 60);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(23);
            this.pnlContent.Size = new System.Drawing.Size(1038, 670);
            this.pnlContent.TabIndex = 0;
            // 
            // pnlDashboard
            // 
            this.pnlDashboard.Location = new System.Drawing.Point(0, 0);
            this.pnlDashboard.Name = "pnlDashboard";
            this.pnlDashboard.Size = new System.Drawing.Size(200, 100);
            this.pnlDashboard.TabIndex = 0;
            // 
            // pnlPedidos
            // 
            this.pnlPedidos.Location = new System.Drawing.Point(0, 0);
            this.pnlPedidos.Name = "pnlPedidos";
            this.pnlPedidos.Size = new System.Drawing.Size(200, 100);
            this.pnlPedidos.TabIndex = 0;
            // 
            // lblPedidosHoje
            // 
            this.lblPedidosHoje.Location = new System.Drawing.Point(0, 0);
            this.lblPedidosHoje.Name = "lblPedidosHoje";
            this.lblPedidosHoje.Size = new System.Drawing.Size(100, 23);
            this.lblPedidosHoje.TabIndex = 0;
            // 
            // lblFaturamento
            // 
            this.lblFaturamento.Location = new System.Drawing.Point(0, 0);
            this.lblFaturamento.Name = "lblFaturamento";
            this.lblFaturamento.Size = new System.Drawing.Size(100, 23);
            this.lblFaturamento.TabIndex = 0;
            // 
            // lblClientes
            // 
            this.lblClientes.Location = new System.Drawing.Point(0, 0);
            this.lblClientes.Name = "lblClientes";
            this.lblClientes.Size = new System.Drawing.Size(100, 23);
            this.lblClientes.TabIndex = 0;
            // 
            // lblPendentes
            // 
            this.lblPendentes.Location = new System.Drawing.Point(0, 0);
            this.lblPendentes.Name = "lblPendentes";
            this.lblPendentes.Size = new System.Drawing.Size(100, 23);
            this.lblPendentes.TabIndex = 0;
            // 
            // gridPedidos
            // 
            this.gridPedidos.Location = new System.Drawing.Point(0, 0);
            this.gridPedidos.Name = "gridPedidos";
            this.gridPedidos.Size = new System.Drawing.Size(240, 150);
            this.gridPedidos.TabIndex = 0;
            // 
            // gridItens
            // 
            this.gridItens.Location = new System.Drawing.Point(0, 0);
            this.gridItens.Name = "gridItens";
            this.gridItens.Size = new System.Drawing.Size(240, 150);
            this.gridItens.TabIndex = 0;
            // 
            // gridFinanceiro
            // 
            this.gridFinanceiro.Location = new System.Drawing.Point(0, 0);
            this.gridFinanceiro.Name = "gridFinanceiro";
            this.gridFinanceiro.Size = new System.Drawing.Size(240, 150);
            this.gridFinanceiro.TabIndex = 0;
            // 
            // lblDetalhe
            // 
            this.lblDetalhe.Location = new System.Drawing.Point(0, 0);
            this.lblDetalhe.Name = "lblDetalhe";
            this.lblDetalhe.Size = new System.Drawing.Size(100, 23);
            this.lblDetalhe.TabIndex = 0;
            // 
            // lblFinResumo
            // 
            this.lblFinResumo.Location = new System.Drawing.Point(0, 0);
            this.lblFinResumo.Name = "lblFinResumo";
            this.lblFinResumo.Size = new System.Drawing.Size(100, 23);
            this.lblFinResumo.TabIndex = 0;
            // 
            // cmbFiltroPedido
            // 
            this.cmbFiltroPedido.Location = new System.Drawing.Point(0, 0);
            this.cmbFiltroPedido.Name = "cmbFiltroPedido";
            this.cmbFiltroPedido.Size = new System.Drawing.Size(121, 23);
            this.cmbFiltroPedido.TabIndex = 0;
            // 
            // dtpFiltroPedido
            // 
            this.dtpFiltroPedido.Location = new System.Drawing.Point(0, 0);
            this.dtpFiltroPedido.Name = "dtpFiltroPedido";
            this.dtpFiltroPedido.Size = new System.Drawing.Size(200, 23);
            this.dtpFiltroPedido.TabIndex = 0;
            // 
            // _timer
            // 
            this._timer.Interval = 30000;
            this._timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(22)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(1283, 762);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.pnlTopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(1073, 646);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RanGoFood — Painel de Controle";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.pnlTopBar.ResumeLayout(false);
            this.pnlTopBar.PerformLayout();
            this.pnlSidebar.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPedidos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridItens)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridFinanceiro)).EndInit();
            this.ResumeLayout(false);

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
