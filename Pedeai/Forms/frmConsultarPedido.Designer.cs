using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmConsultarPedido
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlBusca = new System.Windows.Forms.Panel();
            this.lblDe = new System.Windows.Forms.Label();
            this.dtpDe = new System.Windows.Forms.DateTimePicker();
            this.lblAte = new System.Windows.Forms.Label();
            this.dtpAte = new System.Windows.Forms.DateTimePicker();
            this.lblCapCliente2 = new System.Windows.Forms.Label();
            this.txtCliente = new System.Windows.Forms.TextBox();
            this.lblLblNum = new System.Windows.Forms.Label();
            this.txtNumPedido = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnTodos = new System.Windows.Forms.Button();
            this.pnlFoot = new System.Windows.Forms.Panel();
            this.btnFechar = new System.Windows.Forms.Button();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.gridPedidos = new System.Windows.Forms.DataGridView();
            this.gridItens = new System.Windows.Forms.DataGridView();
            this.lblItensTitle = new System.Windows.Forms.Label();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.lblInfoTit = new System.Windows.Forms.Label();
            this.pnlInfoSep = new System.Windows.Forms.Panel();
            this.lblCapCliente = new System.Windows.Forms.Label();
            this.lblCliente = new System.Windows.Forms.Label();
            this.lblCapTaxaEnt = new System.Windows.Forms.Label();
            this.lblTaxaEnt = new System.Windows.Forms.Label();
            this.lblCapCondPgto = new System.Windows.Forms.Label();
            this.lblCondPgto = new System.Windows.Forms.Label();
            this.lblCapValorPedido = new System.Windows.Forms.Label();
            this.lblValorPedido = new System.Windows.Forms.Label();
            this.lblCapValorPago = new System.Windows.Forms.Label();
            this.lblValorPago = new System.Windows.Forms.Label();
            this.lblCapDesconto = new System.Windows.Forms.Label();
            this.lblDesconto = new System.Windows.Forms.Label();
            this.lblCapAutorizador = new System.Windows.Forms.Label();
            this.lblAutorizador = new System.Windows.Forms.Label();
            this.pnlTop.SuspendLayout();
            this.pnlBusca.SuspendLayout();
            this.pnlFoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPedidos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridItens)).BeginInit();
            this.pnlInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            this.pnlTop.Controls.Add(this.lblTitulo);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(960, 48);
            this.pnlTop.TabIndex = 3;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(16, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(172, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "🔍  Consultar Pedido";
            // 
            // pnlBusca
            // 
            this.pnlBusca.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(228)))), ((int)(((byte)(213)))));
            this.pnlBusca.Controls.Add(this.lblDe);
            this.pnlBusca.Controls.Add(this.dtpDe);
            this.pnlBusca.Controls.Add(this.lblAte);
            this.pnlBusca.Controls.Add(this.dtpAte);
            this.pnlBusca.Controls.Add(this.lblCapCliente2);
            this.pnlBusca.Controls.Add(this.txtCliente);
            this.pnlBusca.Controls.Add(this.lblLblNum);
            this.pnlBusca.Controls.Add(this.txtNumPedido);
            this.pnlBusca.Controls.Add(this.btnBuscar);
            this.pnlBusca.Controls.Add(this.btnTodos);
            this.pnlBusca.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBusca.Location = new System.Drawing.Point(0, 48);
            this.pnlBusca.Name = "pnlBusca";
            this.pnlBusca.Size = new System.Drawing.Size(960, 56);
            this.pnlBusca.TabIndex = 2;
            // 
            // lblDe
            // 
            this.lblDe.AutoSize = true;
            this.lblDe.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblDe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(50)))), ((int)(((byte)(35)))));
            this.lblDe.Location = new System.Drawing.Point(3, 19);
            this.lblDe.Name = "lblDe";
            this.lblDe.Size = new System.Drawing.Size(27, 17);
            this.lblDe.TabIndex = 0;
            this.lblDe.Text = "De:";
            // 
            // dtpDe
            // 
            this.dtpDe.CustomFormat = "dd/MM/yyyy";
            this.dtpDe.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDe.Location = new System.Drawing.Point(32, 16);
            this.dtpDe.Name = "dtpDe";
            this.dtpDe.Size = new System.Drawing.Size(102, 23);
            this.dtpDe.TabIndex = 1;
            // 
            // lblAte
            // 
            this.lblAte.AutoSize = true;
            this.lblAte.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblAte.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(50)))), ((int)(((byte)(35)))));
            this.lblAte.Location = new System.Drawing.Point(138, 17);
            this.lblAte.Name = "lblAte";
            this.lblAte.Size = new System.Drawing.Size(30, 17);
            this.lblAte.TabIndex = 2;
            this.lblAte.Text = "Até:";
            // 
            // dtpAte
            // 
            this.dtpAte.CustomFormat = "dd/MM/yyyy";
            this.dtpAte.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpAte.Location = new System.Drawing.Point(170, 15);
            this.dtpAte.Name = "dtpAte";
            this.dtpAte.Size = new System.Drawing.Size(102, 23);
            this.dtpAte.TabIndex = 3;
            // 
            // lblCapCliente2
            // 
            this.lblCapCliente2.AutoSize = true;
            this.lblCapCliente2.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCapCliente2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(50)))), ((int)(((byte)(35)))));
            this.lblCapCliente2.Location = new System.Drawing.Point(277, 17);
            this.lblCapCliente2.Name = "lblCapCliente2";
            this.lblCapCliente2.Size = new System.Drawing.Size(50, 17);
            this.lblCapCliente2.TabIndex = 4;
            this.lblCapCliente2.Text = "Cliente:";
            // 
            // txtCliente
            // 
            this.txtCliente.BackColor = System.Drawing.Color.White;
            this.txtCliente.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCliente.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtCliente.Location = new System.Drawing.Point(330, 15);
            this.txtCliente.Name = "txtCliente";
            this.txtCliente.Size = new System.Drawing.Size(148, 24);
            this.txtCliente.TabIndex = 5;
            this.txtCliente.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtNumPedido_KeyDown);
            // 
            // lblLblNum
            // 
            this.lblLblNum.AutoSize = true;
            this.lblLblNum.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblLblNum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(50)))), ((int)(((byte)(35)))));
            this.lblLblNum.Location = new System.Drawing.Point(482, 19);
            this.lblLblNum.Name = "lblLblNum";
            this.lblLblNum.Size = new System.Drawing.Size(27, 17);
            this.lblLblNum.TabIndex = 6;
            this.lblLblNum.Text = "Nº:";
            // 
            // txtNumPedido
            // 
            this.txtNumPedido.BackColor = System.Drawing.Color.White;
            this.txtNumPedido.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNumPedido.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtNumPedido.Location = new System.Drawing.Point(512, 15);
            this.txtNumPedido.Name = "txtNumPedido";
            this.txtNumPedido.Size = new System.Drawing.Size(92, 24);
            this.txtNumPedido.TabIndex = 7;
            this.txtNumPedido.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtNumPedido_KeyDown);
            // 
            // btnBuscar
            // 
            this.btnBuscar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            this.btnBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBuscar.FlatAppearance.BorderSize = 0;
            this.btnBuscar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnBuscar.ForeColor = System.Drawing.Color.White;
            this.btnBuscar.Location = new System.Drawing.Point(620, 14);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(90, 28);
            this.btnBuscar.TabIndex = 8;
            this.btnBuscar.Text = "🔍  Buscar";
            this.btnBuscar.UseVisualStyleBackColor = false;
            this.btnBuscar.Click += new System.EventHandler(this.BtnBuscar_Click);
            // 
            // btnTodos
            // 
            this.btnTodos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(100)))), ((int)(((byte)(68)))));
            this.btnTodos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTodos.FlatAppearance.BorderSize = 0;
            this.btnTodos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTodos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnTodos.ForeColor = System.Drawing.Color.White;
            this.btnTodos.Location = new System.Drawing.Point(726, 14);
            this.btnTodos.Name = "btnTodos";
            this.btnTodos.Size = new System.Drawing.Size(84, 28);
            this.btnTodos.TabIndex = 9;
            this.btnTodos.Text = "Ver Todos";
            this.btnTodos.UseVisualStyleBackColor = false;
            this.btnTodos.Click += new System.EventHandler(this.BtnTodos_Click);
            // 
            // pnlFoot
            // 
            this.pnlFoot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(228)))), ((int)(((byte)(213)))));
            this.pnlFoot.Controls.Add(this.btnFechar);
            this.pnlFoot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFoot.Location = new System.Drawing.Point(0, 656);
            this.pnlFoot.Name = "pnlFoot";
            this.pnlFoot.Size = new System.Drawing.Size(960, 44);
            this.pnlFoot.TabIndex = 1;
            // 
            // btnFechar
            // 
            this.btnFechar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnFechar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFechar.FlatAppearance.BorderSize = 0;
            this.btnFechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFechar.ForeColor = System.Drawing.Color.White;
            this.btnFechar.Location = new System.Drawing.Point(430, 8);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(100, 28);
            this.btnFechar.TabIndex = 0;
            this.btnFechar.Text = "Fechar";
            this.btnFechar.UseVisualStyleBackColor = false;
            this.btnFechar.Click += new System.EventHandler(this.BtnFechar_Click);
            // 
            // splitMain
            // 
            this.splitMain.BackColor = System.Drawing.Color.Transparent;
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 104);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.gridPedidos);
            this.splitMain.Panel1MinSize = 80;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.gridItens);
            this.splitMain.Panel2.Controls.Add(this.lblItensTitle);
            this.splitMain.Panel2.Controls.Add(this.pnlInfo);
            this.splitMain.Panel2MinSize = 80;
            this.splitMain.Size = new System.Drawing.Size(960, 552);
            this.splitMain.SplitterDistance = 170;
            this.splitMain.TabIndex = 0;
            // 
            // gridPedidos
            // 
            this.gridPedidos.AllowUserToAddRows = false;
            this.gridPedidos.AllowUserToDeleteRows = false;
            this.gridPedidos.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(247)))), ((int)(((byte)(242)))));
            this.gridPedidos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridPedidos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridPedidos.BackgroundColor = System.Drawing.Color.White;
            this.gridPedidos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridPedidos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridPedidos.ColumnHeadersHeight = 34;
            this.gridPedidos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(40)))), ((int)(((byte)(30)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridPedidos.DefaultCellStyle = dataGridViewCellStyle3;
            this.gridPedidos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPedidos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gridPedidos.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(210)))), ((int)(((byte)(195)))));
            this.gridPedidos.Location = new System.Drawing.Point(0, 0);
            this.gridPedidos.MultiSelect = false;
            this.gridPedidos.Name = "gridPedidos";
            this.gridPedidos.ReadOnly = true;
            this.gridPedidos.RowHeadersVisible = false;
            this.gridPedidos.RowTemplate.Height = 26;
            this.gridPedidos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridPedidos.Size = new System.Drawing.Size(960, 170);
            this.gridPedidos.TabIndex = 0;
            this.gridPedidos.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            this.gridPedidos.SelectionChanged += new System.EventHandler(this.GridPedidos_SelectionChanged);
            // 
            // gridItens
            // 
            this.gridItens.AllowUserToAddRows = false;
            this.gridItens.AllowUserToDeleteRows = false;
            this.gridItens.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(247)))), ((int)(((byte)(242)))));
            this.gridItens.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.gridItens.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridItens.BackgroundColor = System.Drawing.Color.White;
            this.gridItens.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridItens.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.gridItens.ColumnHeadersHeight = 34;
            this.gridItens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(40)))), ((int)(((byte)(30)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridItens.DefaultCellStyle = dataGridViewCellStyle6;
            this.gridItens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItens.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gridItens.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(210)))), ((int)(((byte)(195)))));
            this.gridItens.Location = new System.Drawing.Point(0, 196);
            this.gridItens.MultiSelect = false;
            this.gridItens.Name = "gridItens";
            this.gridItens.ReadOnly = true;
            this.gridItens.RowHeadersVisible = false;
            this.gridItens.RowTemplate.Height = 26;
            this.gridItens.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridItens.Size = new System.Drawing.Size(960, 182);
            this.gridItens.TabIndex = 0;
            this.gridItens.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // 
            // lblItensTitle
            // 
            this.lblItensTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.lblItensTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblItensTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblItensTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(40)))), ((int)(((byte)(25)))));
            this.lblItensTitle.Location = new System.Drawing.Point(0, 168);
            this.lblItensTitle.Name = "lblItensTitle";
            this.lblItensTitle.Padding = new System.Windows.Forms.Padding(4, 6, 0, 0);
            this.lblItensTitle.Size = new System.Drawing.Size(960, 28);
            this.lblItensTitle.TabIndex = 1;
            this.lblItensTitle.Text = "Itens do Pedido";
            // 
            // pnlInfo
            // 
            this.pnlInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.pnlInfo.Controls.Add(this.lblInfoTit);
            this.pnlInfo.Controls.Add(this.pnlInfoSep);
            this.pnlInfo.Controls.Add(this.lblCapCliente);
            this.pnlInfo.Controls.Add(this.lblCliente);
            this.pnlInfo.Controls.Add(this.lblCapTaxaEnt);
            this.pnlInfo.Controls.Add(this.lblTaxaEnt);
            this.pnlInfo.Controls.Add(this.lblCapCondPgto);
            this.pnlInfo.Controls.Add(this.lblCondPgto);
            this.pnlInfo.Controls.Add(this.lblCapValorPedido);
            this.pnlInfo.Controls.Add(this.lblValorPedido);
            this.pnlInfo.Controls.Add(this.lblCapValorPago);
            this.pnlInfo.Controls.Add(this.lblValorPago);
            this.pnlInfo.Controls.Add(this.lblCapDesconto);
            this.pnlInfo.Controls.Add(this.lblDesconto);
            this.pnlInfo.Controls.Add(this.lblCapAutorizador);
            this.pnlInfo.Controls.Add(this.lblAutorizador);
            this.pnlInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlInfo.Location = new System.Drawing.Point(0, 0);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(960, 168);
            this.pnlInfo.TabIndex = 2;
            this.pnlInfo.Visible = false;
            // 
            // lblInfoTit
            // 
            this.lblInfoTit.AutoSize = true;
            this.lblInfoTit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblInfoTit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            this.lblInfoTit.Location = new System.Drawing.Point(16, 8);
            this.lblInfoTit.Name = "lblInfoTit";
            this.lblInfoTit.Size = new System.Drawing.Size(135, 15);
            this.lblInfoTit.TabIndex = 0;
            this.lblInfoTit.Text = "Informações do Pedido";
            // 
            // pnlInfoSep
            // 
            this.pnlInfoSep.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(185)))), ((int)(((byte)(160)))));
            this.pnlInfoSep.Location = new System.Drawing.Point(16, 30);
            this.pnlInfoSep.Name = "pnlInfoSep";
            this.pnlInfoSep.Size = new System.Drawing.Size(760, 1);
            this.pnlInfoSep.TabIndex = 1;
            // 
            // lblCapCliente
            // 
            this.lblCapCliente.AutoSize = true;
            this.lblCapCliente.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCapCliente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(80)))), ((int)(((byte)(50)))));
            this.lblCapCliente.Location = new System.Drawing.Point(16, 40);
            this.lblCapCliente.Name = "lblCapCliente";
            this.lblCapCliente.Size = new System.Drawing.Size(49, 15);
            this.lblCapCliente.TabIndex = 2;
            this.lblCapCliente.Text = "Cliente:";
            // 
            // lblCliente
            // 
            this.lblCliente.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCliente.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(30)))), ((int)(((byte)(20)))));
            this.lblCliente.Location = new System.Drawing.Point(80, 40);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(640, 23);
            this.lblCliente.TabIndex = 3;
            this.lblCliente.Text = "—";
            // 
            // lblCapTaxaEnt
            // 
            this.lblCapTaxaEnt.AutoSize = true;
            this.lblCapTaxaEnt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCapTaxaEnt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(80)))), ((int)(((byte)(50)))));
            this.lblCapTaxaEnt.Location = new System.Drawing.Point(16, 68);
            this.lblCapTaxaEnt.Name = "lblCapTaxaEnt";
            this.lblCapTaxaEnt.Size = new System.Drawing.Size(81, 15);
            this.lblCapTaxaEnt.TabIndex = 4;
            this.lblCapTaxaEnt.Text = "Taxa Entrega:";
            // 
            // lblTaxaEnt
            // 
            this.lblTaxaEnt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTaxaEnt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(30)))), ((int)(((byte)(20)))));
            this.lblTaxaEnt.Location = new System.Drawing.Point(130, 68);
            this.lblTaxaEnt.Name = "lblTaxaEnt";
            this.lblTaxaEnt.Size = new System.Drawing.Size(120, 23);
            this.lblTaxaEnt.TabIndex = 5;
            this.lblTaxaEnt.Text = "—";
            // 
            // lblCapCondPgto
            // 
            this.lblCapCondPgto.AutoSize = true;
            this.lblCapCondPgto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCapCondPgto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(80)))), ((int)(((byte)(50)))));
            this.lblCapCondPgto.Location = new System.Drawing.Point(280, 68);
            this.lblCapCondPgto.Name = "lblCapCondPgto";
            this.lblCapCondPgto.Size = new System.Drawing.Size(107, 15);
            this.lblCapCondPgto.TabIndex = 6;
            this.lblCapCondPgto.Text = "Cond. Pagamento:";
            // 
            // lblCondPgto
            // 
            this.lblCondPgto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCondPgto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(30)))), ((int)(((byte)(20)))));
            this.lblCondPgto.Location = new System.Drawing.Point(412, 68);
            this.lblCondPgto.Name = "lblCondPgto";
            this.lblCondPgto.Size = new System.Drawing.Size(460, 23);
            this.lblCondPgto.TabIndex = 7;
            this.lblCondPgto.Text = "—";
            // 
            // lblCapValorPedido
            // 
            this.lblCapValorPedido.AutoSize = true;
            this.lblCapValorPedido.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCapValorPedido.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(80)))), ((int)(((byte)(50)))));
            this.lblCapValorPedido.Location = new System.Drawing.Point(16, 96);
            this.lblCapValorPedido.Name = "lblCapValorPedido";
            this.lblCapValorPedido.Size = new System.Drawing.Size(79, 15);
            this.lblCapValorPedido.TabIndex = 8;
            this.lblCapValorPedido.Text = "Valor Pedido:";
            // 
            // lblValorPedido
            // 
            this.lblValorPedido.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblValorPedido.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.lblValorPedido.Location = new System.Drawing.Point(130, 96);
            this.lblValorPedido.Name = "lblValorPedido";
            this.lblValorPedido.Size = new System.Drawing.Size(120, 23);
            this.lblValorPedido.TabIndex = 9;
            this.lblValorPedido.Text = "—";
            // 
            // lblCapValorPago
            // 
            this.lblCapValorPago.AutoSize = true;
            this.lblCapValorPago.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCapValorPago.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(80)))), ((int)(((byte)(50)))));
            this.lblCapValorPago.Location = new System.Drawing.Point(280, 96);
            this.lblCapValorPago.Name = "lblCapValorPago";
            this.lblCapValorPago.Size = new System.Drawing.Size(68, 15);
            this.lblCapValorPago.TabIndex = 10;
            this.lblCapValorPago.Text = "Valor Pago:";
            // 
            // lblValorPago
            // 
            this.lblValorPago.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblValorPago.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.lblValorPago.Location = new System.Drawing.Point(380, 96);
            this.lblValorPago.Name = "lblValorPago";
            this.lblValorPago.Size = new System.Drawing.Size(150, 23);
            this.lblValorPago.TabIndex = 11;
            this.lblValorPago.Text = "—";
            // 
            // lblCapDesconto
            // 
            this.lblCapDesconto.AutoSize = true;
            this.lblCapDesconto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCapDesconto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(80)))), ((int)(((byte)(50)))));
            this.lblCapDesconto.Location = new System.Drawing.Point(16, 124);
            this.lblCapDesconto.Name = "lblCapDesconto";
            this.lblCapDesconto.Size = new System.Drawing.Size(63, 15);
            this.lblCapDesconto.TabIndex = 12;
            this.lblCapDesconto.Text = "Desconto:";
            // 
            // lblDesconto
            // 
            this.lblDesconto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblDesconto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.lblDesconto.Location = new System.Drawing.Point(86, 124);
            this.lblDesconto.Name = "lblDesconto";
            this.lblDesconto.Size = new System.Drawing.Size(240, 23);
            this.lblDesconto.TabIndex = 13;
            this.lblDesconto.Text = "—";
            // 
            // lblCapAutorizador
            // 
            this.lblCapAutorizador.AutoSize = true;
            this.lblCapAutorizador.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCapAutorizador.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(80)))), ((int)(((byte)(50)))));
            this.lblCapAutorizador.Location = new System.Drawing.Point(350, 124);
            this.lblCapAutorizador.Name = "lblCapAutorizador";
            this.lblCapAutorizador.Size = new System.Drawing.Size(93, 15);
            this.lblCapAutorizador.TabIndex = 14;
            this.lblCapAutorizador.Text = "Autorizado por:";
            // 
            // lblAutorizador
            // 
            this.lblAutorizador.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblAutorizador.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(30)))), ((int)(((byte)(20)))));
            this.lblAutorizador.Location = new System.Drawing.Point(454, 124);
            this.lblAutorizador.Name = "lblAutorizador";
            this.lblAutorizador.Size = new System.Drawing.Size(280, 23);
            this.lblAutorizador.TabIndex = 15;
            this.lblAutorizador.Text = "—";
            // 
            // frmConsultarPedido
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.ClientSize = new System.Drawing.Size(960, 700);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlFoot);
            this.Controls.Add(this.pnlBusca);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(900, 540);
            this.Name = "frmConsultarPedido";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Consultar Pedido";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlBusca.ResumeLayout(false);
            this.pnlBusca.PerformLayout();
            this.pnlFoot.ResumeLayout(false);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridPedidos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridItens)).EndInit();
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        // ── Field declarations ─────────────────────────────────────────────────
        private System.Windows.Forms.Panel        pnlTop;
        private System.Windows.Forms.Label        lblTitulo;
        private System.Windows.Forms.Panel        pnlBusca;
        private System.Windows.Forms.Label        lblDe;
        private System.Windows.Forms.DateTimePicker dtpDe;
        private System.Windows.Forms.Label        lblAte;
        private System.Windows.Forms.DateTimePicker dtpAte;
        private System.Windows.Forms.Label        lblCapCliente2;
        private System.Windows.Forms.TextBox      txtCliente;
        private System.Windows.Forms.Label        lblLblNum;
        private System.Windows.Forms.TextBox      txtNumPedido;
        private System.Windows.Forms.Button       btnBuscar;
        private System.Windows.Forms.Button       btnTodos;
        private System.Windows.Forms.Panel        pnlFoot;
        private System.Windows.Forms.Button       btnFechar;
        private System.Windows.Forms.SplitContainer splitMain;
        internal System.Windows.Forms.DataGridView  gridPedidos;
        private System.Windows.Forms.Panel        pnlInfo;
        private System.Windows.Forms.Label        lblInfoTit;
        private System.Windows.Forms.Panel        pnlInfoSep;
        private System.Windows.Forms.Label        lblCapCliente;
        internal System.Windows.Forms.Label       lblCliente;
        private System.Windows.Forms.Label        lblCapTaxaEnt;
        internal System.Windows.Forms.Label       lblTaxaEnt;
        private System.Windows.Forms.Label        lblCapCondPgto;
        internal System.Windows.Forms.Label       lblCondPgto;
        private System.Windows.Forms.Label        lblCapValorPedido;
        internal System.Windows.Forms.Label       lblValorPedido;
        private System.Windows.Forms.Label        lblCapValorPago;
        internal System.Windows.Forms.Label       lblValorPago;
        private System.Windows.Forms.Label        lblCapDesconto;
        internal System.Windows.Forms.Label       lblDesconto;
        private System.Windows.Forms.Label        lblCapAutorizador;
        internal System.Windows.Forms.Label       lblAutorizador;
        private System.Windows.Forms.Label        lblItensTitle;
        internal System.Windows.Forms.DataGridView gridItens;
    }
}
