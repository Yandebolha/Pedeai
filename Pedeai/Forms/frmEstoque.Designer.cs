using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmEstoque
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grid         = new System.Windows.Forms.DataGridView();
            this.topBar       = new System.Windows.Forms.Panel();
            this.txtFiltro    = new System.Windows.Forms.TextBox();
            this.btnPesq      = new System.Windows.Forms.Button();
            this.btnNovo      = new System.Windows.Forms.Button();
            this.btnAjusteQtd = new System.Windows.Forms.Button();
            this.pnlForm      = new System.Windows.Forms.Panel();
            this.lblFormTitulo = new System.Windows.Forms.Label();
            // pnlCadastro controls
            this.pnlCadastro  = new System.Windows.Forms.Panel();
            this.lblNom2      = new System.Windows.Forms.Label();
            this.txtNome      = new System.Windows.Forms.TextBox();
            this.lblUn        = new System.Windows.Forms.Label();
            this.txtUnidade   = new System.Windows.Forms.TextBox();
            this.lblQtde2     = new System.Windows.Forms.Label();
            this.numQtde      = new System.Windows.Forms.NumericUpDown();
            this.lblCusto2    = new System.Windows.Forms.Label();
            this.numCusto     = new System.Windows.Forms.NumericUpDown();
            this.lblMin       = new System.Windows.Forms.Label();
            this.numEstMin    = new System.Windows.Forms.NumericUpDown();
            this.chkEhProduto = new System.Windows.Forms.CheckBox();
            // pnlAjuste controls
            this.pnlAjuste    = new System.Windows.Forms.Panel();
            this.lblAjusteNomeLbl = new System.Windows.Forms.Label();
            this.lblAjusteNome   = new System.Windows.Forms.Label();
            this.lblAjusteAtualLbl = new System.Windows.Forms.Label();
            this.lblAjusteAtual  = new System.Windows.Forms.Label();
            this.lblAjTipo       = new System.Windows.Forms.Label();
            this.cmbAjusteTipo   = new System.Windows.Forms.ComboBox();
            this.lblAjQtde       = new System.Windows.Forms.Label();
            this.numAjusteQtde   = new System.Windows.Forms.NumericUpDown();
            // buttons panel
            this.pnlBtns      = new System.Windows.Forms.Panel();
            this.btnS         = new System.Windows.Forms.Button();
            this.btnD         = new System.Windows.Forms.Button();
            this.btnC         = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // ── grid ──────────────────────────────────────────────────────────
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.ReadOnly = true; this.grid.AllowUserToAddRows = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.MultiSelect = false; this.grid.RowHeadersVisible = false;
            this.grid.BackgroundColor = System.Drawing.Color.FromArgb(250, 245, 238);
            this.grid.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.grid.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.grid.GridColor = System.Drawing.Color.FromArgb(200, 185, 160);
            this.grid.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 230, 202);
            this.grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grid.ColumnHeadersHeight = 34; this.grid.RowTemplate.Height = 28;
            this.grid.DoubleClick += new System.EventHandler(this.Grid_DoubleClick);
            this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler((s, e) => e.ThrowException = false);

            // ── topBar ────────────────────────────────────────────────────────
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top; this.topBar.Height = 44;
            this.topBar.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.topBar.Controls.Add(this.txtFiltro);
            this.topBar.Controls.Add(this.btnPesq);
            this.topBar.Controls.Add(this.btnNovo);
            this.topBar.Controls.Add(this.btnAjusteQtd);

            this.txtFiltro.Left = 8; this.txtFiltro.Top = 9; this.txtFiltro.Width = 240;
            this.txtFiltro.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.txtFiltro.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.txtFiltro.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtFiltro.PlaceholderText = "Pesquisar...";
            this.txtFiltro.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtFiltro_KeyDown);

            this.btnPesq.Text = "Pesquisar"; this.btnPesq.Left = 258; this.btnPesq.Top = 8; this.btnPesq.Width = 90; this.btnPesq.Height = 28;
            this.btnPesq.BackColor = System.Drawing.Color.FromArgb(224, 113, 42); this.btnPesq.ForeColor = System.Drawing.Color.White;
            this.btnPesq.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPesq.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPesq.FlatAppearance.BorderSize = 0;
            this.btnPesq.Click += new System.EventHandler(this.BtnPesq_Click);

            this.btnNovo.Text = "+ Novo Item"; this.btnNovo.Left = 358; this.btnNovo.Top = 8; this.btnNovo.Width = 110; this.btnNovo.Height = 28;
            this.btnNovo.BackColor = System.Drawing.Color.FromArgb(87, 120, 38); this.btnNovo.ForeColor = System.Drawing.Color.White;
            this.btnNovo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNovo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNovo.FlatAppearance.BorderSize = 0;
            this.btnNovo.Click += new System.EventHandler(this.BtnNovo_Click);

            this.btnAjusteQtd.Text = "\u21C5 Ajustar Qtde"; this.btnAjusteQtd.Left = 478; this.btnAjusteQtd.Top = 8; this.btnAjusteQtd.Width = 130; this.btnAjusteQtd.Height = 28;
            this.btnAjusteQtd.BackColor = System.Drawing.Color.FromArgb(52, 100, 170); this.btnAjusteQtd.ForeColor = System.Drawing.Color.White;
            this.btnAjusteQtd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAjusteQtd.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAjusteQtd.FlatAppearance.BorderSize = 0;
            this.btnAjusteQtd.Click += new System.EventHandler(this.BtnAjuste_Click);

            // ── pnlForm ───────────────────────────────────────────────────────
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Bottom; this.pnlForm.Height = 160;
            this.pnlForm.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlForm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlForm.Visible = false;

            this.lblFormTitulo.Text = "Novo Item de Estoque"; this.lblFormTitulo.Left = 10; this.lblFormTitulo.Top = 6;
            this.lblFormTitulo.AutoSize = true;
            this.lblFormTitulo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblFormTitulo.ForeColor = System.Drawing.Color.FromArgb(176, 110, 42);

            // pnlCadastro (field-entry panel)
            this.pnlCadastro.Left = 0; this.pnlCadastro.Top = 28;
            this.pnlCadastro.Width = 900; this.pnlCadastro.Height = 80;
            this.pnlCadastro.BackColor = System.Drawing.Color.Transparent;
            this.pnlCadastro.Anchor = ((System.Windows.Forms.AnchorStyles)(
                System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));

            this.lblNom2.Text = "Nome:"; this.lblNom2.AutoSize = true; this.lblNom2.Left = 10; this.lblNom2.Top = 7;
            this.lblNom2.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.txtNome.Left = 55; this.txtNome.Top = 4; this.txtNome.Width = 220;
            this.txtNome.BackColor = System.Drawing.Color.White; this.txtNome.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);

            this.lblUn.Text = "Unid.:"; this.lblUn.AutoSize = true; this.lblUn.Left = 285; this.lblUn.Top = 7;
            this.lblUn.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.txtUnidade.Left = 326; this.txtUnidade.Top = 4; this.txtUnidade.Width = 55;
            this.txtUnidade.BackColor = System.Drawing.Color.White; this.txtUnidade.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtUnidade.Text = "un";

            this.lblQtde2.Text = "Qtde:"; this.lblQtde2.AutoSize = true; this.lblQtde2.Left = 390; this.lblQtde2.Top = 7;
            this.lblQtde2.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.numQtde.Left = 432; this.numQtde.Top = 4; this.numQtde.Width = 90;
            this.numQtde.DecimalPlaces = 3; this.numQtde.Maximum = 999999; this.numQtde.Minimum = 0;

            this.lblCusto2.Text = "Custo R$:"; this.lblCusto2.AutoSize = true; this.lblCusto2.Left = 530; this.lblCusto2.Top = 7;
            this.lblCusto2.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.numCusto.Left = 596; this.numCusto.Top = 4; this.numCusto.Width = 95;
            this.numCusto.DecimalPlaces = 4; this.numCusto.Maximum = 999999; this.numCusto.Minimum = 0;

            this.lblMin.Text = "Est. Mín:"; this.lblMin.AutoSize = true; this.lblMin.Left = 700; this.lblMin.Top = 7;
            this.lblMin.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.numEstMin.Left = 762; this.numEstMin.Top = 4; this.numEstMin.Width = 90;
            this.numEstMin.DecimalPlaces = 3; this.numEstMin.Maximum = 999999; this.numEstMin.Minimum = 0;

            this.chkEhProduto.Text = "É Produto (sincronizar com catálogo)";
            this.chkEhProduto.Left = 10; this.chkEhProduto.Top = 38; this.chkEhProduto.AutoSize = true;
            this.chkEhProduto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.chkEhProduto.ForeColor = System.Drawing.Color.FromArgb(87, 120, 38);

            this.pnlCadastro.Controls.Add(this.lblNom2);   this.pnlCadastro.Controls.Add(this.txtNome);
            this.pnlCadastro.Controls.Add(this.lblUn);     this.pnlCadastro.Controls.Add(this.txtUnidade);
            this.pnlCadastro.Controls.Add(this.lblQtde2);  this.pnlCadastro.Controls.Add(this.numQtde);
            this.pnlCadastro.Controls.Add(this.lblCusto2); this.pnlCadastro.Controls.Add(this.numCusto);
            this.pnlCadastro.Controls.Add(this.lblMin);    this.pnlCadastro.Controls.Add(this.numEstMin);
            this.pnlCadastro.Controls.Add(this.chkEhProduto);

            // pnlAjuste (qty adjustment panel)
            this.pnlAjuste.Left = 0; this.pnlAjuste.Top = 28;
            this.pnlAjuste.Width = 900; this.pnlAjuste.Height = 60;
            this.pnlAjuste.BackColor = System.Drawing.Color.Transparent;
            this.pnlAjuste.Visible = false;
            this.pnlAjuste.Anchor = ((System.Windows.Forms.AnchorStyles)(
                System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));

            this.lblAjusteNomeLbl.Text = "Item:"; this.lblAjusteNomeLbl.AutoSize = true; this.lblAjusteNomeLbl.Left = 10; this.lblAjusteNomeLbl.Top = 7;
            this.lblAjusteNomeLbl.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblAjusteNome.AutoSize = false; this.lblAjusteNome.Left = 45; this.lblAjusteNome.Top = 7; this.lblAjusteNome.Width = 220; this.lblAjusteNome.Height = 20;
            this.lblAjusteNome.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAjusteNome.ForeColor = System.Drawing.Color.FromArgb(176, 110, 42);

            this.lblAjusteAtualLbl.Text = ""; this.lblAjusteAtualLbl.AutoSize = true; this.lblAjusteAtualLbl.Left = 275; this.lblAjusteAtualLbl.Top = 7;
            this.lblAjusteAtual.AutoSize = true; this.lblAjusteAtual.Left = 275; this.lblAjusteAtual.Top = 7;
            this.lblAjusteAtual.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);

            this.lblAjTipo.Text = "Tipo:"; this.lblAjTipo.AutoSize = true; this.lblAjTipo.Left = 440; this.lblAjTipo.Top = 7;
            this.lblAjTipo.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.cmbAjusteTipo.Left = 477; this.cmbAjusteTipo.Top = 4; this.cmbAjusteTipo.Width = 100;
            this.cmbAjusteTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAjusteTipo.Items.AddRange(new object[] { "Entrada", "Sa\u00edda" });
            this.cmbAjusteTipo.SelectedIndex = 0;

            this.lblAjQtde.Text = "Quantidade:"; this.lblAjQtde.AutoSize = true; this.lblAjQtde.Left = 586; this.lblAjQtde.Top = 7;
            this.lblAjQtde.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.numAjusteQtde.Left = 666; this.numAjusteQtde.Top = 4; this.numAjusteQtde.Width = 100;
            this.numAjusteQtde.DecimalPlaces = 3; this.numAjusteQtde.Maximum = 999999; this.numAjusteQtde.Minimum = 0;

            this.pnlAjuste.Controls.Add(this.lblAjusteNomeLbl);
            this.pnlAjuste.Controls.Add(this.lblAjusteNome);
            this.pnlAjuste.Controls.Add(this.lblAjusteAtual);
            this.pnlAjuste.Controls.Add(this.lblAjTipo);
            this.pnlAjuste.Controls.Add(this.cmbAjusteTipo);
            this.pnlAjuste.Controls.Add(this.lblAjQtde);
            this.pnlAjuste.Controls.Add(this.numAjusteQtde);

            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom; this.pnlBtns.Height = 48;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlBtns.Controls.Add(this.btnS);
            this.pnlBtns.Controls.Add(this.btnD);
            this.pnlBtns.Controls.Add(this.btnC);
            this.pnlBtns.SizeChanged += new System.EventHandler(this.PnlBtns_SizeChanged);

            this.btnS.Text = "Salvar"; this.btnS.Left = 10; this.btnS.Top = 10; this.btnS.Width = 110; this.btnS.Height = 28;
            this.btnS.BackColor = System.Drawing.Color.FromArgb(87, 120, 38); this.btnS.ForeColor = System.Drawing.Color.White;
            this.btnS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnS.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnS.FlatAppearance.BorderSize = 0;
            this.btnS.Click += new System.EventHandler(this.BtnSalvar_Click);

            this.btnD.Text = "Desativar"; this.btnD.Left = 130; this.btnD.Top = 10; this.btnD.Width = 110; this.btnD.Height = 28;
            this.btnD.BackColor = System.Drawing.Color.FromArgb(192, 57, 43); this.btnD.ForeColor = System.Drawing.Color.White;
            this.btnD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnD.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnD.FlatAppearance.BorderSize = 0;
            this.btnD.Click += new System.EventHandler(this.BtnDesativar_Click);

            this.btnC.Text = "Cancelar"; this.btnC.Left = 250; this.btnC.Top = 10; this.btnC.Width = 110; this.btnC.Height = 28;
            this.btnC.BackColor = System.Drawing.Color.FromArgb(224, 113, 42); this.btnC.ForeColor = System.Drawing.Color.White;
            this.btnC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnC.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnC.FlatAppearance.BorderSize = 0;
            this.btnC.Click += new System.EventHandler(this.BtnCancelar_Click);

            this.pnlForm.Controls.Add(this.lblFormTitulo);
            this.pnlForm.Controls.Add(this.pnlCadastro);
            this.pnlForm.Controls.Add(this.pnlAjuste);
            this.pnlForm.Controls.Add(this.pnlBtns);

            // ── Form settings ─────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 600);
            this.MinimumSize = new System.Drawing.Size(820, 540);
            this.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Controle de Estoque";
            this.Controls.Add(this.grid);
            this.Controls.Add(this.topBar);
            this.Controls.Add(this.pnlForm);
            this.ResumeLayout(false);
        }

        private void PnlBtns_SizeChanged(object sender, System.EventArgs e) { }

        internal System.Windows.Forms.DataGridView   grid;
        internal System.Windows.Forms.TextBox        txtFiltro;
        internal System.Windows.Forms.TextBox        txtNome;
        internal System.Windows.Forms.TextBox        txtUnidade;
        internal System.Windows.Forms.NumericUpDown  numQtde;
        internal System.Windows.Forms.NumericUpDown  numCusto;
        internal System.Windows.Forms.NumericUpDown  numEstMin;
        internal System.Windows.Forms.CheckBox       chkEhProduto;
        internal System.Windows.Forms.Label          lblFormTitulo;
        internal System.Windows.Forms.Label          lblAjusteNome;
        internal System.Windows.Forms.Label          lblAjusteAtual;
        internal System.Windows.Forms.ComboBox       cmbAjusteTipo;
        internal System.Windows.Forms.NumericUpDown  numAjusteQtde;
        private  System.Windows.Forms.Panel          topBar;
        private  System.Windows.Forms.Panel          pnlForm;
        private  System.Windows.Forms.Panel          pnlCadastro;
        private  System.Windows.Forms.Panel          pnlAjuste;
        private  System.Windows.Forms.Panel          pnlBtns;
        private  System.Windows.Forms.Button         btnPesq;
        private  System.Windows.Forms.Button         btnNovo;
        private  System.Windows.Forms.Button         btnAjusteQtd;
        private  System.Windows.Forms.Button         btnS;
        private  System.Windows.Forms.Button         btnD;
        private  System.Windows.Forms.Button         btnC;
        private  System.Windows.Forms.Label          lblNom2;
        private  System.Windows.Forms.Label          lblUn;
        private  System.Windows.Forms.Label          lblQtde2;
        private  System.Windows.Forms.Label          lblCusto2;
        private  System.Windows.Forms.Label          lblMin;
        private  System.Windows.Forms.Label          lblAjusteNomeLbl;
        private  System.Windows.Forms.Label          lblAjusteAtualLbl;
        private  System.Windows.Forms.Label          lblAjTipo;
        private  System.Windows.Forms.Label          lblAjQtde;
    }
}
