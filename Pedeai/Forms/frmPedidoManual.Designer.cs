using System.Windows.Forms;
using System.Drawing;

namespace Pedeai.Forms
{
    partial class frmPedidoManual
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.topBar = new System.Windows.Forms.Panel();
            this.lblTituloPed = new System.Windows.Forms.Label();
            this.pnlRodape = new System.Windows.Forms.Panel();
            this.lblTotalTag = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnSal = new System.Windows.Forms.Button();
            this.btnCanc = new System.Windows.Forms.Button();
            this.mainArea = new System.Windows.Forms.Panel();
            this.rightArea = new System.Windows.Forms.Panel();
            this.gridItens = new System.Windows.Forms.DataGridView();
            this.colNome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQtde = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnitario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDesconto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubtotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRemover = new System.Windows.Forms.DataGridViewButtonColumn();
            this.pnlAddItem = new System.Windows.Forms.Panel();
            this.lblSecAddItem = new System.Windows.Forms.Label();
            this.txtBuscaProduto = new System.Windows.Forms.TextBox();
            this.btnBuscarProduto = new System.Windows.Forms.Button();
            this.lQtde = new System.Windows.Forms.Label();
            this.numQtde = new System.Windows.Forms.NumericUpDown();
            this.lVal = new System.Windows.Forms.Label();
            this.numUnitario = new System.Windows.Forms.NumericUpDown();
            this.lDsc = new System.Windows.Forms.Label();
            this.numDescontoItem = new System.Windows.Forms.NumericUpDown();
            this.lblDesconto = new System.Windows.Forms.Label();
            this.btnMeioAMeio = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.sideDiv = new System.Windows.Forms.Panel();
            this.sidebar = new System.Windows.Forms.Panel();
            this.sideContent = new System.Windows.Forms.Panel();
            this.lblSecCliente = new System.Windows.Forms.Label();
            this.lNome = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.btnSelecionarCliente = new System.Windows.Forms.Button();
            this.lTel = new System.Windows.Forms.Label();
            this.txtTelefone = new System.Windows.Forms.TextBox();
            this.pnlDiv1 = new System.Windows.Forms.Panel();
            this.lblSecEntrega = new System.Windows.Forms.Label();
            this.cmbEntrega = new System.Windows.Forms.ComboBox();
            this.lblEndereco = new System.Windows.Forms.Label();
            this.txtEndereco = new System.Windows.Forms.TextBox();
            this.lblTaxa = new System.Windows.Forms.Label();
            this.numTaxa = new System.Windows.Forms.NumericUpDown();
            this.pnlDiv2 = new System.Windows.Forms.Panel();
            this.lblSecPagamento = new System.Windows.Forms.Label();
            this.cmbPagamento = new System.Windows.Forms.ComboBox();
            this.lblTroco = new System.Windows.Forms.Label();
            this.numTroco = new System.Windows.Forms.NumericUpDown();
            this.lblTrocoInfo = new System.Windows.Forms.Label();
            this.pnlDiv3 = new System.Windows.Forms.Panel();
            this.lblSecObs = new System.Windows.Forms.Label();
            this.txtObs = new System.Windows.Forms.TextBox();
            this.pnlDiv4 = new System.Windows.Forms.Panel();
            this.lblSecCupom = new System.Windows.Forms.Label();
            this.txtCupom = new System.Windows.Forms.TextBox();
            this.btnAplicarCupom = new System.Windows.Forms.Button();
            this.lblCupomInfo = new System.Windows.Forms.Label();
            this.topBar.SuspendLayout();
            this.pnlRodape.SuspendLayout();
            this.mainArea.SuspendLayout();
            this.rightArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridItens)).BeginInit();
            this.pnlAddItem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQtde)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnitario)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDescontoItem)).BeginInit();
            this.sidebar.SuspendLayout();
            this.sideContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTaxa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTroco)).BeginInit();
            this.SuspendLayout();
            // 
            // topBar
            // 
            this.topBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            this.topBar.Controls.Add(this.lblTituloPed);
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.topBar.Location = new System.Drawing.Point(0, 0);
            this.topBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.topBar.Name = "topBar";
            this.topBar.Size = new System.Drawing.Size(1225, 51);
            this.topBar.TabIndex = 1;
            // 
            // lblTituloPed
            // 
            this.lblTituloPed.AutoSize = true;
            this.lblTituloPed.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTituloPed.ForeColor = System.Drawing.Color.White;
            this.lblTituloPed.Location = new System.Drawing.Point(19, 12);
            this.lblTituloPed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTituloPed.Name = "lblTituloPed";
            this.lblTituloPed.Size = new System.Drawing.Size(113, 20);
            this.lblTituloPed.TabIndex = 0;
            this.lblTituloPed.Text = "Pedido Manual";
            // 
            // pnlRodape
            // 
            this.pnlRodape.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.pnlRodape.Controls.Add(this.lblTotalTag);
            this.pnlRodape.Controls.Add(this.lblTotal);
            this.pnlRodape.Controls.Add(this.btnSal);
            this.pnlRodape.Controls.Add(this.btnCanc);
            this.pnlRodape.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRodape.Location = new System.Drawing.Point(0, 723);
            this.pnlRodape.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlRodape.Name = "pnlRodape";
            this.pnlRodape.Size = new System.Drawing.Size(1225, 62);
            this.pnlRodape.TabIndex = 2;
            this.pnlRodape.SizeChanged += new System.EventHandler(this.PnlRodape_SizeChanged);
            // 
            // lblTotalTag
            // 
            this.lblTotalTag.AutoSize = true;
            this.lblTotalTag.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTotalTag.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblTotalTag.Location = new System.Drawing.Point(0, 21);
            this.lblTotalTag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalTag.Name = "lblTotalTag";
            this.lblTotalTag.Size = new System.Drawing.Size(33, 15);
            this.lblTotalTag.TabIndex = 0;
            this.lblTotalTag.Text = "Total";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.lblTotal.Location = new System.Drawing.Point(0, 14);
            this.lblTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(91, 30);
            this.lblTotal.TabIndex = 1;
            this.lblTotal.Text = "R$ 0,00";
            // 
            // btnSal
            // 
            this.btnSal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnSal.FlatAppearance.BorderSize = 0;
            this.btnSal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSal.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSal.ForeColor = System.Drawing.Color.White;
            this.btnSal.Location = new System.Drawing.Point(0, 12);
            this.btnSal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSal.Name = "btnSal";
            this.btnSal.Size = new System.Drawing.Size(173, 39);
            this.btnSal.TabIndex = 2;
            this.btnSal.Text = "Salvar Pedido";
            this.btnSal.UseVisualStyleBackColor = false;
            this.btnSal.Click += new System.EventHandler(this.BtnSalvar_Click);
            // 
            // btnCanc
            // 
            this.btnCanc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.btnCanc.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(165)))), ((int)(((byte)(100)))));
            this.btnCanc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCanc.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnCanc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(80)))), ((int)(((byte)(30)))));
            this.btnCanc.Location = new System.Drawing.Point(0, 12);
            this.btnCanc.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCanc.Name = "btnCanc";
            this.btnCanc.Size = new System.Drawing.Size(114, 39);
            this.btnCanc.TabIndex = 3;
            this.btnCanc.Text = "Cancelar";
            this.btnCanc.UseVisualStyleBackColor = false;
            this.btnCanc.Click += new System.EventHandler(this.BtnCanc_Click);
            // 
            // mainArea
            // 
            this.mainArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(16)))), ((int)(((byte)(36)))));
            this.mainArea.Controls.Add(this.rightArea);
            this.mainArea.Controls.Add(this.sideDiv);
            this.mainArea.Controls.Add(this.sidebar);
            this.mainArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainArea.Location = new System.Drawing.Point(0, 51);
            this.mainArea.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mainArea.Name = "mainArea";
            this.mainArea.Size = new System.Drawing.Size(1225, 672);
            this.mainArea.TabIndex = 0;
            // 
            // rightArea
            // 
            this.rightArea.BackColor = System.Drawing.Color.White;
            this.rightArea.Controls.Add(this.gridItens);
            this.rightArea.Controls.Add(this.pnlAddItem);
            this.rightArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightArea.Location = new System.Drawing.Point(363, 0);
            this.rightArea.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rightArea.Name = "rightArea";
            this.rightArea.Size = new System.Drawing.Size(862, 672);
            this.rightArea.TabIndex = 0;
            // 
            // gridItens
            // 
            this.gridItens.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(245)))), ((int)(((byte)(238)))));
            this.gridItens.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridItens.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridItens.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(245)))), ((int)(((byte)(238)))));
            this.gridItens.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridItens.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(230)))), ((int)(((byte)(202)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridItens.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridItens.ColumnHeadersHeight = 30;
            this.gridItens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridItens.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNome,
            this.colQtde,
            this.colUnitario,
            this.colDesconto,
            this.colSubtotal,
            this.colRemover});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridItens.DefaultCellStyle = dataGridViewCellStyle3;
            this.gridItens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItens.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gridItens.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(185)))), ((int)(((byte)(160)))));
            this.gridItens.Location = new System.Drawing.Point(0, 97);
            this.gridItens.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gridItens.MultiSelect = false;
            this.gridItens.Name = "gridItens";
            this.gridItens.ReadOnly = true;
            this.gridItens.RowHeadersVisible = false;
            this.gridItens.RowTemplate.Height = 32;
            this.gridItens.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridItens.Size = new System.Drawing.Size(862, 575);
            this.gridItens.TabIndex = 0;
            this.gridItens.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridItens_CellClick);
            // 
            // colNome
            // 
            this.colNome.FillWeight = 45F;
            this.colNome.HeaderText = "Produto";
            this.colNome.Name = "colNome";
            this.colNome.ReadOnly = true;
            // 
            // colQtde
            // 
            this.colQtde.FillWeight = 8F;
            this.colQtde.HeaderText = "Qtde";
            this.colQtde.Name = "colQtde";
            this.colQtde.ReadOnly = true;
            // 
            // colUnitario
            // 
            this.colUnitario.FillWeight = 12F;
            this.colUnitario.HeaderText = "Valor";
            this.colUnitario.Name = "colUnitario";
            this.colUnitario.ReadOnly = true;
            // 
            // colDesconto
            // 
            this.colDesconto.FillWeight = 10F;
            this.colDesconto.HeaderText = "Desc.";
            this.colDesconto.Name = "colDesconto";
            this.colDesconto.ReadOnly = true;
            // 
            // colSubtotal
            // 
            this.colSubtotal.FillWeight = 14F;
            this.colSubtotal.HeaderText = "Subtotal";
            this.colSubtotal.Name = "colSubtotal";
            this.colSubtotal.ReadOnly = true;
            // 
            // colRemover
            // 
            this.colRemover.FillWeight = 7F;
            this.colRemover.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colRemover.HeaderText = "";
            this.colRemover.MinimumWidth = 42;
            this.colRemover.Name = "colRemover";
            this.colRemover.ReadOnly = true;
            this.colRemover.Text = "✕";
            this.colRemover.UseColumnTextForButtonValue = true;
            // 
            // pnlAddItem
            // 
            this.pnlAddItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.pnlAddItem.Controls.Add(this.lblSecAddItem);
            this.pnlAddItem.Controls.Add(this.txtBuscaProduto);
            this.pnlAddItem.Controls.Add(this.btnBuscarProduto);
            this.pnlAddItem.Controls.Add(this.lQtde);
            this.pnlAddItem.Controls.Add(this.numQtde);
            this.pnlAddItem.Controls.Add(this.lVal);
            this.pnlAddItem.Controls.Add(this.numUnitario);
            this.pnlAddItem.Controls.Add(this.lDsc);
            this.pnlAddItem.Controls.Add(this.numDescontoItem);
            this.pnlAddItem.Controls.Add(this.lblDesconto);
            this.pnlAddItem.Controls.Add(this.btnMeioAMeio);
            this.pnlAddItem.Controls.Add(this.btnAdd);
            this.pnlAddItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAddItem.Location = new System.Drawing.Point(0, 0);
            this.pnlAddItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlAddItem.Name = "pnlAddItem";
            this.pnlAddItem.Size = new System.Drawing.Size(862, 97);
            this.pnlAddItem.TabIndex = 1;
            this.pnlAddItem.SizeChanged += new System.EventHandler(this.PnlAddItem_SizeChanged);
            // 
            // lblSecAddItem
            // 
            this.lblSecAddItem.AutoSize = true;
            this.lblSecAddItem.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSecAddItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblSecAddItem.Location = new System.Drawing.Point(16, 12);
            this.lblSecAddItem.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSecAddItem.Name = "lblSecAddItem";
            this.lblSecAddItem.Size = new System.Drawing.Size(88, 12);
            this.lblSecAddItem.TabIndex = 0;
            this.lblSecAddItem.Text = "ADICIONAR ITEM";
            // 
            // txtBuscaProduto
            // 
            this.txtBuscaProduto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBuscaProduto.BackColor = System.Drawing.Color.White;
            this.txtBuscaProduto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBuscaProduto.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtBuscaProduto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtBuscaProduto.Location = new System.Drawing.Point(16, 32);
            this.txtBuscaProduto.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtBuscaProduto.Name = "txtBuscaProduto";
            this.txtBuscaProduto.PlaceholderText = "Pesquisar produto...";
            this.txtBuscaProduto.Size = new System.Drawing.Size(966, 24);
            this.txtBuscaProduto.TabIndex = 1;
            this.txtBuscaProduto.TextChanged += new System.EventHandler(this.TxtBusca_TextChanged);
            // 
            // btnBuscarProduto
            // 
            this.btnBuscarProduto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuscarProduto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnBuscarProduto.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(46)))), ((int)(((byte)(82)))));
            this.btnBuscarProduto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarProduto.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnBuscarProduto.ForeColor = System.Drawing.Color.White;
            this.btnBuscarProduto.Location = new System.Drawing.Point(499, 0);
            this.btnBuscarProduto.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnBuscarProduto.Name = "btnBuscarProduto";
            this.btnBuscarProduto.Size = new System.Drawing.Size(75, 30);
            this.btnBuscarProduto.TabIndex = 2;
            this.btnBuscarProduto.Text = "Buscar";
            this.btnBuscarProduto.UseVisualStyleBackColor = false;
            this.btnBuscarProduto.Click += new System.EventHandler(this.BtnBuscarProduto_Click);
            // 
            // lQtde
            // 
            this.lQtde.AutoSize = true;
            this.lQtde.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lQtde.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lQtde.Location = new System.Drawing.Point(0, 0);
            this.lQtde.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lQtde.Name = "lQtde";
            this.lQtde.Size = new System.Drawing.Size(33, 15);
            this.lQtde.TabIndex = 3;
            this.lQtde.Text = "Qtde";
            // 
            // numQtde
            // 
            this.numQtde.BackColor = System.Drawing.Color.White;
            this.numQtde.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numQtde.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.numQtde.Location = new System.Drawing.Point(0, 0);
            this.numQtde.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numQtde.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numQtde.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numQtde.Name = "numQtde";
            this.numQtde.Size = new System.Drawing.Size(79, 24);
            this.numQtde.TabIndex = 4;
            this.numQtde.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lVal
            // 
            this.lVal.AutoSize = true;
            this.lVal.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lVal.Location = new System.Drawing.Point(0, 0);
            this.lVal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lVal.Name = "lVal";
            this.lVal.Size = new System.Drawing.Size(33, 15);
            this.lVal.TabIndex = 5;
            this.lVal.Text = "Valor";
            // 
            // numUnitario
            // 
            this.numUnitario.BackColor = System.Drawing.Color.White;
            this.numUnitario.DecimalPlaces = 2;
            this.numUnitario.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numUnitario.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.numUnitario.Location = new System.Drawing.Point(0, 0);
            this.numUnitario.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numUnitario.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numUnitario.Name = "numUnitario";
            this.numUnitario.Size = new System.Drawing.Size(105, 24);
            this.numUnitario.TabIndex = 6;
            this.numUnitario.ValueChanged += new System.EventHandler(this.NumUnitario_ValueChanged);
            // 
            // lDsc
            // 
            this.lDsc.AutoSize = true;
            this.lDsc.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lDsc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lDsc.Location = new System.Drawing.Point(0, 0);
            this.lDsc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lDsc.Name = "lDsc";
            this.lDsc.Size = new System.Drawing.Size(45, 15);
            this.lDsc.TabIndex = 7;
            this.lDsc.Text = "Desc.%";
            // 
            // numDescontoItem
            // 
            this.numDescontoItem.BackColor = System.Drawing.Color.White;
            this.numDescontoItem.DecimalPlaces = 1;
            this.numDescontoItem.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numDescontoItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.numDescontoItem.Location = new System.Drawing.Point(0, 0);
            this.numDescontoItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numDescontoItem.Name = "numDescontoItem";
            this.numDescontoItem.Size = new System.Drawing.Size(79, 24);
            this.numDescontoItem.TabIndex = 8;
            // 
            // lblDesconto
            // 
            this.lblDesconto.AutoSize = true;
            this.lblDesconto.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblDesconto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblDesconto.Location = new System.Drawing.Point(0, 0);
            this.lblDesconto.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDesconto.Name = "lblDesconto";
            this.lblDesconto.Size = new System.Drawing.Size(0, 15);
            this.lblDesconto.TabIndex = 9;
            this.lblDesconto.Visible = false;
            // 
            // btnMeioAMeio
            // 
            this.btnMeioAMeio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMeioAMeio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(90)))), ((int)(((byte)(205)))));
            this.btnMeioAMeio.FlatAppearance.BorderSize = 0;
            this.btnMeioAMeio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMeioAMeio.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnMeioAMeio.ForeColor = System.Drawing.Color.White;
            this.btnMeioAMeio.Location = new System.Drawing.Point(314, 0);
            this.btnMeioAMeio.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMeioAMeio.Name = "btnMeioAMeio";
            this.btnMeioAMeio.Size = new System.Drawing.Size(177, 30);
            this.btnMeioAMeio.TabIndex = 10;
            this.btnMeioAMeio.Text = "½ + ½  Pizza";
            this.btnMeioAMeio.UseVisualStyleBackColor = false;
            this.btnMeioAMeio.Click += new System.EventHandler(this.BtnMeioAMeio_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(592, 0);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(145, 30);
            this.btnAdd.TabIndex = 11;
            this.btnAdd.Text = "+  Adicionar";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.BtnAdicionarItem_Click);
            // 
            // sideDiv
            // 
            this.sideDiv.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(46)))), ((int)(((byte)(82)))));
            this.sideDiv.Dock = System.Windows.Forms.DockStyle.Left;
            this.sideDiv.Location = new System.Drawing.Point(362, 0);
            this.sideDiv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.sideDiv.Name = "sideDiv";
            this.sideDiv.Size = new System.Drawing.Size(1, 672);
            this.sideDiv.TabIndex = 1;
            // 
            // sidebar
            // 
            this.sidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(50)))));
            this.sidebar.Controls.Add(this.sideContent);
            this.sidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidebar.Location = new System.Drawing.Point(0, 0);
            this.sidebar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.sidebar.Name = "sidebar";
            this.sidebar.Size = new System.Drawing.Size(362, 672);
            this.sidebar.TabIndex = 2;
            // 
            // sideContent
            // 
            this.sideContent.AutoScroll = true;
            this.sideContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(50)))));
            this.sideContent.Controls.Add(this.lblSecCliente);
            this.sideContent.Controls.Add(this.lNome);
            this.sideContent.Controls.Add(this.txtNome);
            this.sideContent.Controls.Add(this.btnSelecionarCliente);
            this.sideContent.Controls.Add(this.lTel);
            this.sideContent.Controls.Add(this.txtTelefone);
            this.sideContent.Controls.Add(this.pnlDiv1);
            this.sideContent.Controls.Add(this.lblSecEntrega);
            this.sideContent.Controls.Add(this.cmbEntrega);
            this.sideContent.Controls.Add(this.lblEndereco);
            this.sideContent.Controls.Add(this.txtEndereco);
            this.sideContent.Controls.Add(this.lblTaxa);
            this.sideContent.Controls.Add(this.numTaxa);
            this.sideContent.Controls.Add(this.pnlDiv2);
            this.sideContent.Controls.Add(this.lblSecPagamento);
            this.sideContent.Controls.Add(this.cmbPagamento);
            this.sideContent.Controls.Add(this.lblTroco);
            this.sideContent.Controls.Add(this.numTroco);
            this.sideContent.Controls.Add(this.lblTrocoInfo);
            this.sideContent.Controls.Add(this.pnlDiv3);
            this.sideContent.Controls.Add(this.lblSecObs);
            this.sideContent.Controls.Add(this.txtObs);
            this.sideContent.Controls.Add(this.pnlDiv4);
            this.sideContent.Controls.Add(this.lblSecCupom);
            this.sideContent.Controls.Add(this.txtCupom);
            this.sideContent.Controls.Add(this.btnAplicarCupom);
            this.sideContent.Controls.Add(this.lblCupomInfo);
            this.sideContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sideContent.Location = new System.Drawing.Point(0, 0);
            this.sideContent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.sideContent.Name = "sideContent";
            this.sideContent.Size = new System.Drawing.Size(362, 672);
            this.sideContent.TabIndex = 0;
            // 
            // lblSecCliente
            // 
            this.lblSecCliente.AutoSize = true;
            this.lblSecCliente.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSecCliente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(105)))), ((int)(((byte)(160)))));
            this.lblSecCliente.Location = new System.Drawing.Point(16, 14);
            this.lblSecCliente.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSecCliente.Name = "lblSecCliente";
            this.lblSecCliente.Size = new System.Drawing.Size(43, 12);
            this.lblSecCliente.TabIndex = 0;
            this.lblSecCliente.Text = "CLIENTE";
            // 
            // lNome
            // 
            this.lNome.AutoSize = true;
            this.lNome.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lNome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(130)))), ((int)(((byte)(175)))));
            this.lNome.Location = new System.Drawing.Point(16, 37);
            this.lNome.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lNome.Name = "lNome";
            this.lNome.Size = new System.Drawing.Size(116, 15);
            this.lNome.TabIndex = 1;
            this.lNome.Text = "Nome / Razão Social";
            // 
            // txtNome
            // 
            this.txtNome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(21)))), ((int)(((byte)(46)))));
            this.txtNome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNome.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtNome.ForeColor = System.Drawing.Color.White;
            this.txtNome.Location = new System.Drawing.Point(16, 58);
            this.txtNome.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(249, 24);
            this.txtNome.TabIndex = 2;
            // 
            // btnSelecionarCliente
            // 
            this.btnSelecionarCliente.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnSelecionarCliente.FlatAppearance.BorderSize = 0;
            this.btnSelecionarCliente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelecionarCliente.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSelecionarCliente.ForeColor = System.Drawing.Color.White;
            this.btnSelecionarCliente.Location = new System.Drawing.Point(271, 58);
            this.btnSelecionarCliente.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSelecionarCliente.Name = "btnSelecionarCliente";
            this.btnSelecionarCliente.Size = new System.Drawing.Size(75, 30);
            this.btnSelecionarCliente.TabIndex = 3;
            this.btnSelecionarCliente.Text = "Buscar";
            this.btnSelecionarCliente.UseVisualStyleBackColor = false;
            this.btnSelecionarCliente.Click += new System.EventHandler(this.BtnSelecionarCliente_Click);
            // 
            // lTel
            // 
            this.lTel.AutoSize = true;
            this.lTel.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lTel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(130)))), ((int)(((byte)(175)))));
            this.lTel.Location = new System.Drawing.Point(16, 95);
            this.lTel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lTel.Name = "lTel";
            this.lTel.Size = new System.Drawing.Size(52, 15);
            this.lTel.TabIndex = 4;
            this.lTel.Text = "Telefone";
            // 
            // txtTelefone
            // 
            this.txtTelefone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(21)))), ((int)(((byte)(46)))));
            this.txtTelefone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTelefone.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtTelefone.ForeColor = System.Drawing.Color.White;
            this.txtTelefone.Location = new System.Drawing.Point(16, 115);
            this.txtTelefone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTelefone.Name = "txtTelefone";
            this.txtTelefone.Size = new System.Drawing.Size(329, 24);
            this.txtTelefone.TabIndex = 5;
            // 
            // pnlDiv1
            // 
            this.pnlDiv1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(46)))), ((int)(((byte)(82)))));
            this.pnlDiv1.Location = new System.Drawing.Point(0, 157);
            this.pnlDiv1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlDiv1.Name = "pnlDiv1";
            this.pnlDiv1.Size = new System.Drawing.Size(362, 1);
            this.pnlDiv1.TabIndex = 6;
            // 
            // lblSecEntrega
            // 
            this.lblSecEntrega.AutoSize = true;
            this.lblSecEntrega.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSecEntrega.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(105)))), ((int)(((byte)(160)))));
            this.lblSecEntrega.Location = new System.Drawing.Point(16, 170);
            this.lblSecEntrega.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSecEntrega.Name = "lblSecEntrega";
            this.lblSecEntrega.Size = new System.Drawing.Size(50, 12);
            this.lblSecEntrega.TabIndex = 7;
            this.lblSecEntrega.Text = "ENTREGA";
            // 
            // cmbEntrega
            // 
            this.cmbEntrega.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(21)))), ((int)(((byte)(46)))));
            this.cmbEntrega.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEntrega.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEntrega.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cmbEntrega.ForeColor = System.Drawing.Color.White;
            this.cmbEntrega.Items.AddRange(new object[] {
            "Retirada",
            "Entrega"});
            this.cmbEntrega.Location = new System.Drawing.Point(16, 193);
            this.cmbEntrega.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbEntrega.Name = "cmbEntrega";
            this.cmbEntrega.Size = new System.Drawing.Size(328, 25);
            this.cmbEntrega.TabIndex = 8;
            this.cmbEntrega.SelectedIndexChanged += new System.EventHandler(this.CmbEntrega_SelectedIndexChanged);
            // 
            // lblEndereco
            // 
            this.lblEndereco.AutoSize = true;
            this.lblEndereco.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblEndereco.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(130)))), ((int)(((byte)(175)))));
            this.lblEndereco.Location = new System.Drawing.Point(16, 230);
            this.lblEndereco.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEndereco.Name = "lblEndereco";
            this.lblEndereco.Size = new System.Drawing.Size(56, 15);
            this.lblEndereco.TabIndex = 9;
            this.lblEndereco.Text = "Endereço";
            // 
            // txtEndereco
            // 
            this.txtEndereco.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(21)))), ((int)(((byte)(46)))));
            this.txtEndereco.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEndereco.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtEndereco.ForeColor = System.Drawing.Color.White;
            this.txtEndereco.Location = new System.Drawing.Point(16, 250);
            this.txtEndereco.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtEndereco.Name = "txtEndereco";
            this.txtEndereco.Size = new System.Drawing.Size(329, 24);
            this.txtEndereco.TabIndex = 10;
            // 
            // lblTaxa
            // 
            this.lblTaxa.AutoSize = true;
            this.lblTaxa.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTaxa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(130)))), ((int)(((byte)(175)))));
            this.lblTaxa.Location = new System.Drawing.Point(16, 287);
            this.lblTaxa.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTaxa.Name = "lblTaxa";
            this.lblTaxa.Size = new System.Drawing.Size(113, 15);
            this.lblTaxa.TabIndex = 11;
            this.lblTaxa.Text = "Taxa de Entrega (R$)";
            // 
            // numTaxa
            // 
            this.numTaxa.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(21)))), ((int)(((byte)(46)))));
            this.numTaxa.DecimalPlaces = 2;
            this.numTaxa.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numTaxa.ForeColor = System.Drawing.Color.White;
            this.numTaxa.Location = new System.Drawing.Point(16, 308);
            this.numTaxa.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numTaxa.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numTaxa.Name = "numTaxa";
            this.numTaxa.Size = new System.Drawing.Size(152, 24);
            this.numTaxa.TabIndex = 12;
            this.numTaxa.ValueChanged += new System.EventHandler(this.NumTaxa_ValueChanged);
            // 
            // pnlDiv2
            // 
            this.pnlDiv2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(46)))), ((int)(((byte)(82)))));
            this.pnlDiv2.Location = new System.Drawing.Point(0, 350);
            this.pnlDiv2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlDiv2.Name = "pnlDiv2";
            this.pnlDiv2.Size = new System.Drawing.Size(362, 1);
            this.pnlDiv2.TabIndex = 13;
            // 
            // lblSecPagamento
            // 
            this.lblSecPagamento.AutoSize = true;
            this.lblSecPagamento.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSecPagamento.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(105)))), ((int)(((byte)(160)))));
            this.lblSecPagamento.Location = new System.Drawing.Point(16, 362);
            this.lblSecPagamento.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSecPagamento.Name = "lblSecPagamento";
            this.lblSecPagamento.Size = new System.Drawing.Size(68, 12);
            this.lblSecPagamento.TabIndex = 14;
            this.lblSecPagamento.Text = "PAGAMENTO";
            // 
            // cmbPagamento
            // 
            this.cmbPagamento.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(21)))), ((int)(((byte)(46)))));
            this.cmbPagamento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPagamento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPagamento.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cmbPagamento.ForeColor = System.Drawing.Color.White;
            this.cmbPagamento.Items.AddRange(new object[] {
            "Dinheiro",
            "Cartão",
            "Pix"});
            this.cmbPagamento.Location = new System.Drawing.Point(16, 385);
            this.cmbPagamento.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbPagamento.Name = "cmbPagamento";
            this.cmbPagamento.Size = new System.Drawing.Size(328, 25);
            this.cmbPagamento.TabIndex = 15;
            this.cmbPagamento.SelectedIndexChanged += new System.EventHandler(this.CmbPagamento_SelectedIndexChanged);
            // 
            // lblTroco
            // 
            this.lblTroco.AutoSize = true;
            this.lblTroco.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTroco.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(130)))), ((int)(((byte)(175)))));
            this.lblTroco.Location = new System.Drawing.Point(16, 422);
            this.lblTroco.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTroco.Name = "lblTroco";
            this.lblTroco.Size = new System.Drawing.Size(87, 15);
            this.lblTroco.TabIndex = 16;
            this.lblTroco.Text = "Troco para (R$)";
            // 
            // numTroco
            // 
            this.numTroco.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(21)))), ((int)(((byte)(46)))));
            this.numTroco.DecimalPlaces = 2;
            this.numTroco.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numTroco.ForeColor = System.Drawing.Color.White;
            this.numTroco.Location = new System.Drawing.Point(16, 443);
            this.numTroco.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numTroco.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numTroco.Name = "numTroco";
            this.numTroco.Size = new System.Drawing.Size(152, 24);
            this.numTroco.TabIndex = 17;
            this.numTroco.ValueChanged += new System.EventHandler(this.NumTroco_ValueChanged);
            // 
            // lblTrocoInfo
            // 
            this.lblTrocoInfo.AutoSize = true;
            this.lblTrocoInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTrocoInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.lblTrocoInfo.Location = new System.Drawing.Point(180, 448);
            this.lblTrocoInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTrocoInfo.Name = "lblTrocoInfo";
            this.lblTrocoInfo.Size = new System.Drawing.Size(0, 15);
            this.lblTrocoInfo.TabIndex = 18;
            // 
            // pnlDiv3
            // 
            this.pnlDiv3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(46)))), ((int)(((byte)(82)))));
            this.pnlDiv3.Location = new System.Drawing.Point(0, 485);
            this.pnlDiv3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlDiv3.Name = "pnlDiv3";
            this.pnlDiv3.Size = new System.Drawing.Size(362, 1);
            this.pnlDiv3.TabIndex = 19;
            // 
            // lblSecObs
            // 
            this.lblSecObs.AutoSize = true;
            this.lblSecObs.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSecObs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(105)))), ((int)(((byte)(160)))));
            this.lblSecObs.Location = new System.Drawing.Point(16, 497);
            this.lblSecObs.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSecObs.Name = "lblSecObs";
            this.lblSecObs.Size = new System.Drawing.Size(75, 12);
            this.lblSecObs.TabIndex = 20;
            this.lblSecObs.Text = "OBSERVAÇÕES";
            // 
            // txtObs
            // 
            this.txtObs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(21)))), ((int)(((byte)(46)))));
            this.txtObs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtObs.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtObs.ForeColor = System.Drawing.Color.White;
            this.txtObs.Location = new System.Drawing.Point(16, 520);
            this.txtObs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtObs.Name = "txtObs";
            this.txtObs.Size = new System.Drawing.Size(329, 24);
            this.txtObs.TabIndex = 21;
            // 
            // pnlDiv4
            // 
            this.pnlDiv4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(46)))), ((int)(((byte)(82)))));
            this.pnlDiv4.Location = new System.Drawing.Point(0, 562);
            this.pnlDiv4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlDiv4.Name = "pnlDiv4";
            this.pnlDiv4.Size = new System.Drawing.Size(362, 1);
            this.pnlDiv4.TabIndex = 22;
            // 
            // lblSecCupom
            // 
            this.lblSecCupom.AutoSize = true;
            this.lblSecCupom.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSecCupom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(105)))), ((int)(((byte)(160)))));
            this.lblSecCupom.Location = new System.Drawing.Point(16, 575);
            this.lblSecCupom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSecCupom.Name = "lblSecCupom";
            this.lblSecCupom.Size = new System.Drawing.Size(42, 12);
            this.lblSecCupom.TabIndex = 23;
            this.lblSecCupom.Text = "CUPOM";
            // 
            // txtCupom
            // 
            this.txtCupom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(21)))), ((int)(((byte)(46)))));
            this.txtCupom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCupom.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCupom.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtCupom.ForeColor = System.Drawing.Color.White;
            this.txtCupom.Location = new System.Drawing.Point(16, 598);
            this.txtCupom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtCupom.Name = "txtCupom";
            this.txtCupom.PlaceholderText = "Código do cupom";
            this.txtCupom.Size = new System.Drawing.Size(245, 24);
            this.txtCupom.TabIndex = 24;
            // 
            // btnAplicarCupom
            // 
            this.btnAplicarCupom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnAplicarCupom.FlatAppearance.BorderSize = 0;
            this.btnAplicarCupom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAplicarCupom.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAplicarCupom.ForeColor = System.Drawing.Color.White;
            this.btnAplicarCupom.Location = new System.Drawing.Point(266, 598);
            this.btnAplicarCupom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAplicarCupom.Name = "btnAplicarCupom";
            this.btnAplicarCupom.Size = new System.Drawing.Size(79, 30);
            this.btnAplicarCupom.TabIndex = 25;
            this.btnAplicarCupom.Text = "Aplicar";
            this.btnAplicarCupom.UseVisualStyleBackColor = false;
            this.btnAplicarCupom.Click += new System.EventHandler(this.BtnAplicarCupom_Click);
            // 
            // lblCupomInfo
            // 
            this.lblCupomInfo.AutoSize = true;
            this.lblCupomInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCupomInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.lblCupomInfo.Location = new System.Drawing.Point(16, 635);
            this.lblCupomInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCupomInfo.Name = "lblCupomInfo";
            this.lblCupomInfo.Size = new System.Drawing.Size(0, 15);
            this.lblCupomInfo.TabIndex = 26;
            // 
            // frmPedidoManual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(16)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(1225, 785);
            this.Controls.Add(this.mainArea);
            this.Controls.Add(this.topBar);
            this.Controls.Add(this.pnlRodape);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(1047, 640);
            this.Name = "frmPedidoManual";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Novo Pedido Manual";
            this.topBar.ResumeLayout(false);
            this.topBar.PerformLayout();
            this.pnlRodape.ResumeLayout(false);
            this.pnlRodape.PerformLayout();
            this.mainArea.ResumeLayout(false);
            this.rightArea.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridItens)).EndInit();
            this.pnlAddItem.ResumeLayout(false);
            this.pnlAddItem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQtde)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnitario)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDescontoItem)).EndInit();
            this.sidebar.ResumeLayout(false);
            this.sideContent.ResumeLayout(false);
            this.sideContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTaxa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTroco)).EndInit();
            this.ResumeLayout(false);

        }

        private void PnlRodape_SizeChanged(object sender, System.EventArgs e)
        {
            int cx = (pnlRodape.Width - btnSal.Width - 8 - btnCanc.Width) / 2;
            btnSal.Left = cx; btnCanc.Left = cx + btnSal.Width + 8;
            lblTotal.Left = pnlRodape.Width - lblTotal.PreferredWidth - 20;
            lblTotalTag.Left = lblTotal.Left - lblTotalTag.PreferredWidth - 6;
        }

        private void PnlAddItem_SizeChanged(object sender, System.EventArgs e)
        {
            int pw = pnlAddItem.Width - 14;
            txtBuscaProduto.SetBounds(14, 28, pw - 82, 26);
            btnBuscarProduto.SetBounds(pw - 64, 28, 64, 26);
            int x = 14;
            lQtde.SetBounds(x, 60, 34, 18); x += 36;
            numQtde.SetBounds(x, 57, 68, 26); x += 76;
            lVal.SetBounds(x, 60, 36, 18); x += 38;
            numUnitario.SetBounds(x, 57, 90, 26); x += 98;
            lDsc.SetBounds(x, 60, 44, 18); x += 46;
            numDescontoItem.SetBounds(x, 57, 68, 26); x += 76;
            lblDesconto.SetBounds(x, 61, 120, 16);
            btnMeioAMeio.SetBounds(pw - 270, 57, 152, 26);
            btnAdd.SetBounds(pw - 134, 57, 148, 26);
        }

        private void CmbEntrega_SelectedIndexChanged(object sender, System.EventArgs e) { AtualizarVisibilidade(); }
        private void CmbPagamento_SelectedIndexChanged(object sender, System.EventArgs e) { AtualizarVisibilidade(); }
        private void NumTroco_ValueChanged(object sender, System.EventArgs e) { AtualizarTrocoInfo(); }
        private void NumTaxa_ValueChanged(object sender, System.EventArgs e) { AtualizarTotal(); }
        private void BtnCanc_Click(object sender, System.EventArgs e) { Close(); }

        private System.Windows.Forms.Panel         topBar;
        private System.Windows.Forms.Panel         pnlRodape;
        private System.Windows.Forms.Panel         mainArea;
        private System.Windows.Forms.Panel         sidebar;
        private System.Windows.Forms.Panel         sideContent;
        private System.Windows.Forms.Panel         sideDiv;
        private System.Windows.Forms.Panel         rightArea;
        private System.Windows.Forms.Panel         pnlAddItem;
        private System.Windows.Forms.Panel         pnlDiv1;
        private System.Windows.Forms.Panel         pnlDiv2;
        private System.Windows.Forms.Panel         pnlDiv3;
        private System.Windows.Forms.Label         lblTituloPed;
        private System.Windows.Forms.Label         lblTotalTag;
        private System.Windows.Forms.Label         lblSecCliente;
        private System.Windows.Forms.Label         lblSecEntrega;
        private System.Windows.Forms.Label         lblSecPagamento;
        private System.Windows.Forms.Label         lblSecObs;
        private System.Windows.Forms.Label         lblSecAddItem;
        private System.Windows.Forms.Label         lNome;
        private System.Windows.Forms.Label         lTel;
        private System.Windows.Forms.Label         lQtde;
        private System.Windows.Forms.Label         lVal;
        private System.Windows.Forms.Label         lDsc;
        private System.Windows.Forms.TextBox       txtNome;
        private System.Windows.Forms.Button        btnSelecionarCliente;
        private System.Windows.Forms.TextBox       txtTelefone;
        private System.Windows.Forms.ComboBox      cmbEntrega;
        private System.Windows.Forms.TextBox       txtEndereco;
        private System.Windows.Forms.Label         lblEndereco;
        private System.Windows.Forms.ComboBox      cmbPagamento;
        private System.Windows.Forms.NumericUpDown numTroco;
        private System.Windows.Forms.Label         lblTroco;
        private System.Windows.Forms.Label         lblTrocoInfo;
        private System.Windows.Forms.NumericUpDown numTaxa;
        private System.Windows.Forms.Label         lblTaxa;
        private System.Windows.Forms.TextBox       txtObs;
        private System.Windows.Forms.TextBox       txtBuscaProduto;
        private System.Windows.Forms.Button        btnBuscarProduto;
        private System.Windows.Forms.Label         lblDesconto;
        private System.Windows.Forms.NumericUpDown numQtde;
        private System.Windows.Forms.NumericUpDown numUnitario;
        private System.Windows.Forms.NumericUpDown numDescontoItem;
        private System.Windows.Forms.DataGridView  gridItens;
        private System.Windows.Forms.Label         lblTotal;
        internal System.Windows.Forms.Button                         btnSal;
        internal System.Windows.Forms.Button                         btnCanc;
        internal System.Windows.Forms.Button                         btnAdd;
        private System.Windows.Forms.Panel         pnlDiv4;
        private System.Windows.Forms.Label         lblSecCupom;
        private System.Windows.Forms.TextBox       txtCupom;
        private System.Windows.Forms.Button        btnAplicarCupom;
        private System.Windows.Forms.Label         lblCupomInfo;
        private System.Windows.Forms.Button        btnMeioAMeio;
        private System.Windows.Forms.DataGridViewTextBoxColumn        colNome;
        private System.Windows.Forms.DataGridViewTextBoxColumn        colQtde;
        private System.Windows.Forms.DataGridViewTextBoxColumn        colUnitario;
        private System.Windows.Forms.DataGridViewTextBoxColumn        colDesconto;
        private System.Windows.Forms.DataGridViewTextBoxColumn        colSubtotal;
        private System.Windows.Forms.DataGridViewButtonColumn         colRemover;
    }
}
