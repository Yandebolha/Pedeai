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
            this.grid          = new System.Windows.Forms.DataGridView();
            this.topBar        = new System.Windows.Forms.Panel();
            this.txtFiltro     = new System.Windows.Forms.TextBox();
            this.btnPesq       = new System.Windows.Forms.Button();
            this.pnlForm       = new System.Windows.Forms.Panel();
            this.pnlBtns       = new System.Windows.Forms.Panel();
            this.btnS          = new System.Windows.Forms.Button();
            this.btnC          = new System.Windows.Forms.Button();
            this.lblProdSel    = new System.Windows.Forms.Label();
            this.lblProdutoSel = new System.Windows.Forms.Label();
            this.lblEstAtual   = new System.Windows.Forms.Label();
            this.lblEstoqueAtual = new System.Windows.Forms.Label();
            this.lblTipo       = new System.Windows.Forms.Label();
            this.cmbTipo       = new System.Windows.Forms.ComboBox();
            this.lblQtde       = new System.Windows.Forms.Label();
            this.numQtde       = new System.Windows.Forms.NumericUpDown();
            this.lblObs2       = new System.Windows.Forms.Label();
            this.txtObs        = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

            // ── grid ──────────────────────────────────────────────────────────
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.ReadOnly = true; this.grid.AllowUserToAddRows = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.MultiSelect = false;
            this.grid.RowHeadersVisible = false;
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
            this.grid.SelectionChanged += new System.EventHandler(this.Grid_SelectionChanged);
            this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler((s, e) => e.ThrowException = false);

            // ── topBar ────────────────────────────────────────────────────────
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top; this.topBar.Height = 44;
            this.topBar.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.topBar.Controls.Add(this.txtFiltro);
            this.topBar.Controls.Add(this.btnPesq);
            this.txtFiltro.Left = 8; this.txtFiltro.Top = 9; this.txtFiltro.Width = 260;
            this.txtFiltro.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.txtFiltro.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.txtFiltro.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtFiltro.PlaceholderText = "Filtrar por nome...";
            this.txtFiltro.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtFiltro_KeyDown);
            this.btnPesq.Text = "Pesquisar"; this.btnPesq.Left = 278; this.btnPesq.Top = 8; this.btnPesq.Width = 95; this.btnPesq.Height = 28;
            this.btnPesq.BackColor = System.Drawing.Color.FromArgb(224, 113, 42); this.btnPesq.ForeColor = System.Drawing.Color.White;
            this.btnPesq.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPesq.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPesq.FlatAppearance.BorderSize = 0;
            this.btnPesq.Click += new System.EventHandler(this.BtnPesq_Click);

            // ── pnlForm ───────────────────────────────────────────────────────
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Bottom; this.pnlForm.Height = 130;
            this.pnlForm.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlForm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlForm.Visible = false;

            // produto selecionado info
            this.lblProdSel.Text = "Produto:"; this.lblProdSel.AutoSize = true;
            this.lblProdSel.Left = 10; this.lblProdSel.Top = 11;
            this.lblProdSel.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblProdutoSel.AutoSize = false; this.lblProdutoSel.Left = 70; this.lblProdutoSel.Top = 11;
            this.lblProdutoSel.Width = 340; this.lblProdutoSel.Height = 20;
            this.lblProdutoSel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProdutoSel.ForeColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.lblEstAtual.Text = ""; this.lblEstAtual.AutoSize = true;
            this.lblEstAtual.Left = 420; this.lblEstAtual.Top = 11;
            this.lblEstAtual.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblEstoqueAtual.AutoSize = true; this.lblEstoqueAtual.Left = 420; this.lblEstoqueAtual.Top = 11;
            this.lblEstoqueAtual.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstoqueAtual.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);

            // row 2: tipo / qtde / obs
            this.lblTipo.Text = "Tipo:"; this.lblTipo.AutoSize = true; this.lblTipo.Left = 10; this.lblTipo.Top = 47;
            this.lblTipo.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.cmbTipo.Left = 48; this.cmbTipo.Top = 44; this.cmbTipo.Width = 110;
            this.cmbTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipo.Items.AddRange(new object[] { "Entrada", "Sa\u00edda" });
            this.cmbTipo.SelectedIndex = 0;
            this.lblQtde.Text = "Quantidade:"; this.lblQtde.AutoSize = true; this.lblQtde.Left = 168; this.lblQtde.Top = 47;
            this.lblQtde.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.numQtde.Left = 248; this.numQtde.Top = 44; this.numQtde.Width = 95;
            this.numQtde.DecimalPlaces = 2; this.numQtde.Minimum = 0; this.numQtde.Maximum = 99999;
            this.numQtde.Value = 1;
            this.lblObs2.Text = "Observa\u00e7\u00e3o:"; this.lblObs2.AutoSize = true; this.lblObs2.Left = 353; this.lblObs2.Top = 47;
            this.lblObs2.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.txtObs.Left = 433; this.txtObs.Top = 44; this.txtObs.Width = 260;
            this.txtObs.BackColor = System.Drawing.Color.White;
            this.txtObs.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);

            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom; this.pnlBtns.Height = 48;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.pnlBtns.Controls.Add(this.btnS); this.pnlBtns.Controls.Add(this.btnC);
            this.pnlBtns.SizeChanged += new System.EventHandler(this.PnlBtns_SizeChanged);
            this.btnS.Text = "Confirmar Ajuste"; this.btnS.Left = 10; this.btnS.Top = 10; this.btnS.Width = 140; this.btnS.Height = 28;
            this.btnS.BackColor = System.Drawing.Color.FromArgb(87, 120, 38); this.btnS.ForeColor = System.Drawing.Color.White;
            this.btnS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnS.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnS.FlatAppearance.BorderSize = 0; this.btnS.Click += new System.EventHandler(this.BtnSalvar_Click);
            this.btnC.Text = "Cancelar"; this.btnC.Left = 160; this.btnC.Top = 10; this.btnC.Width = 110; this.btnC.Height = 28;
            this.btnC.BackColor = System.Drawing.Color.FromArgb(224, 113, 42); this.btnC.ForeColor = System.Drawing.Color.White;
            this.btnC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnC.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnC.FlatAppearance.BorderSize = 0; this.btnC.Click += new System.EventHandler(this.BtnCancelar_Click);

            this.pnlForm.Controls.Add(this.lblProdSel);
            this.pnlForm.Controls.Add(this.lblProdutoSel);
            this.pnlForm.Controls.Add(this.lblEstoqueAtual);
            this.pnlForm.Controls.Add(this.lblTipo);
            this.pnlForm.Controls.Add(this.cmbTipo);
            this.pnlForm.Controls.Add(this.lblQtde);
            this.pnlForm.Controls.Add(this.numQtde);
            this.pnlForm.Controls.Add(this.lblObs2);
            this.pnlForm.Controls.Add(this.txtObs);
            this.pnlForm.Controls.Add(this.pnlBtns);

            // ── Form settings ─────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 581);
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
        internal System.Windows.Forms.TextBox        txtObs;
        internal System.Windows.Forms.Label          lblProdutoSel;
        internal System.Windows.Forms.Label          lblEstoqueAtual;
        internal System.Windows.Forms.ComboBox       cmbTipo;
        internal System.Windows.Forms.NumericUpDown  numQtde;
        private  System.Windows.Forms.Panel          topBar;
        private  System.Windows.Forms.Panel          pnlForm;
        private  System.Windows.Forms.Panel          pnlBtns;
        private  System.Windows.Forms.Button         btnPesq;
        private  System.Windows.Forms.Button         btnS;
        private  System.Windows.Forms.Button         btnC;
        private  System.Windows.Forms.Label          lblProdSel;
        private  System.Windows.Forms.Label          lblEstAtual;
        private  System.Windows.Forms.Label          lblTipo;
        private  System.Windows.Forms.Label          lblQtde;
        private  System.Windows.Forms.Label          lblObs2;
    }
}
