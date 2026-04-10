namespace Pedeai.Forms
{
    partial class frmFidelizacao
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabConfig = new System.Windows.Forms.TabPage();
            this.chkAtivo = new System.Windows.Forms.CheckBox();
            this.lblNomeRegra = new System.Windows.Forms.Label();
            this.txtNomeRegra = new System.Windows.Forms.TextBox();
            this.lblMeta = new System.Windows.Forms.Label();
            this.numMeta = new System.Windows.Forms.NumericUpDown();
            this.lblPremio = new System.Windows.Forms.Label();
            this.rdCupom = new System.Windows.Forms.RadioButton();
            this.rdProduto = new System.Windows.Forms.RadioButton();
            this.pnlCupom = new System.Windows.Forms.Panel();
            this.lblCupomTipo = new System.Windows.Forms.Label();
            this.cmbCupomTipo = new System.Windows.Forms.ComboBox();
            this.lblCupomValor = new System.Windows.Forms.Label();
            this.numCupomValor = new System.Windows.Forms.NumericUpDown();
            this.lblCupomMin = new System.Windows.Forms.Label();
            this.numCupomMin = new System.Windows.Forms.NumericUpDown();
            this.lblCupomVal = new System.Windows.Forms.Label();
            this.numCupomValidade = new System.Windows.Forms.NumericUpDown();
            this.pnlProduto = new System.Windows.Forms.Panel();
            this.lblProdNome = new System.Windows.Forms.Label();
            this.txtProdNome = new System.Windows.Forms.TextBox();
            this.btnBuscarProduto = new System.Windows.Forms.Button();
            this.lblProdQtde = new System.Windows.Forms.Label();
            this.numProdQtde = new System.Windows.Forms.NumericUpDown();
            this.lblMsg = new System.Windows.Forms.Label();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.lblTags = new System.Windows.Forms.Label();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.pnlListaBotoes = new System.Windows.Forms.Panel();
            this.btnNovaRegra = new System.Windows.Forms.Button();
            this.btnExcluirRegra = new System.Windows.Forms.Button();
            this.pnlLista = new System.Windows.Forms.Panel();
            this.gridConfigs = new System.Windows.Forms.DataGridView();
            this.tabHistorico = new System.Windows.Forms.TabPage();
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.lblDe = new System.Windows.Forms.Label();
            this.dtpDe = new System.Windows.Forms.DateTimePicker();
            this.lblAte = new System.Windows.Forms.Label();
            this.dtpAte = new System.Windows.Forms.DateTimePicker();
            this.btnFiltrar = new System.Windows.Forms.Button();
            this.gridHistorico = new System.Windows.Forms.DataGridView();
            this.pnlTop.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMeta)).BeginInit();
            this.pnlCupom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomValor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomValidade)).BeginInit();
            this.pnlProduto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numProdQtde)).BeginInit();
            this.pnlListaBotoes.SuspendLayout();
            this.pnlLista.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridConfigs)).BeginInit();
            this.tabHistorico.SuspendLayout();
            this.pnlFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridHistorico)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            this.pnlTop.Controls.Add(this.lblTitulo);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(800, 48);
            this.pnlTop.TabIndex = 2;
            // 
            // lblTitulo
            // 
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblTitulo.Size = new System.Drawing.Size(800, 48);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "🤝  Fidelização de Clientes";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabConfig);
            this.tabControl.Controls.Add(this.tabHistorico);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabControl.Location = new System.Drawing.Point(0, 48);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(14, 5);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(800, 672);
            this.tabControl.TabIndex = 1;
            // 
            // tabConfig
            // 
            this.tabConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(245)))), ((int)(((byte)(240)))));
            this.tabConfig.Controls.Add(this.chkAtivo);
            this.tabConfig.Controls.Add(this.lblNomeRegra);
            this.tabConfig.Controls.Add(this.txtNomeRegra);
            this.tabConfig.Controls.Add(this.lblMeta);
            this.tabConfig.Controls.Add(this.numMeta);
            this.tabConfig.Controls.Add(this.lblPremio);
            this.tabConfig.Controls.Add(this.rdCupom);
            this.tabConfig.Controls.Add(this.rdProduto);
            this.tabConfig.Controls.Add(this.pnlCupom);
            this.tabConfig.Controls.Add(this.pnlProduto);
            this.tabConfig.Controls.Add(this.lblMsg);
            this.tabConfig.Controls.Add(this.txtMsg);
            this.tabConfig.Controls.Add(this.lblTags);
            this.tabConfig.Controls.Add(this.btnSalvar);
            this.tabConfig.Controls.Add(this.pnlListaBotoes);
            this.tabConfig.Controls.Add(this.pnlLista);
            this.tabConfig.Location = new System.Drawing.Point(4, 30);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.Padding = new System.Windows.Forms.Padding(8);
            this.tabConfig.Size = new System.Drawing.Size(792, 638);
            this.tabConfig.TabIndex = 0;
            this.tabConfig.Text = "Configuração";
            // 
            // chkAtivo
            // 
            this.chkAtivo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.chkAtivo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.chkAtivo.Location = new System.Drawing.Point(16, 152);
            this.chkAtivo.Name = "chkAtivo";
            this.chkAtivo.Size = new System.Drawing.Size(220, 26);
            this.chkAtivo.TabIndex = 0;
            this.chkAtivo.Text = "Ativar esta Regra";
            this.chkAtivo.CheckedChanged += new System.EventHandler(this.ChkAtivo_CheckedChanged);
            // 
            // lblNomeRegra
            // 
            this.lblNomeRegra.AutoSize = true;
            this.lblNomeRegra.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblNomeRegra.Location = new System.Drawing.Point(300, 156);
            this.lblNomeRegra.Name = "lblNomeRegra";
            this.lblNomeRegra.Size = new System.Drawing.Size(107, 19);
            this.lblNomeRegra.TabIndex = 1;
            this.lblNomeRegra.Text = "Nome da Regra:";
            // 
            // txtNomeRegra
            // 
            this.txtNomeRegra.BackColor = System.Drawing.Color.White;
            this.txtNomeRegra.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNomeRegra.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtNomeRegra.Location = new System.Drawing.Point(412, 152);
            this.txtNomeRegra.MaxLength = 100;
            this.txtNomeRegra.Name = "txtNomeRegra";
            this.txtNomeRegra.Size = new System.Drawing.Size(260, 25);
            this.txtNomeRegra.TabIndex = 1;
            // 
            // lblMeta
            // 
            this.lblMeta.AutoSize = true;
            this.lblMeta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblMeta.Location = new System.Drawing.Point(16, 192);
            this.lblMeta.Name = "lblMeta";
            this.lblMeta.Size = new System.Drawing.Size(131, 19);
            this.lblMeta.TabIndex = 2;
            this.lblMeta.Text = "Meta de Gasto (R$):";
            // 
            // numMeta
            // 
            this.numMeta.DecimalPlaces = 2;
            this.numMeta.Location = new System.Drawing.Point(164, 188);
            this.numMeta.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numMeta.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMeta.Name = "numMeta";
            this.numMeta.Size = new System.Drawing.Size(130, 25);
            this.numMeta.TabIndex = 1;
            this.numMeta.ThousandsSeparator = true;
            this.numMeta.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // lblPremio
            // 
            this.lblPremio.AutoSize = true;
            this.lblPremio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblPremio.Location = new System.Drawing.Point(16, 232);
            this.lblPremio.Name = "lblPremio";
            this.lblPremio.Size = new System.Drawing.Size(104, 19);
            this.lblPremio.TabIndex = 3;
            this.lblPremio.Text = "Tipo de Prêmio:";
            // 
            // rdCupom
            // 
            this.rdCupom.AutoSize = true;
            this.rdCupom.Checked = true;
            this.rdCupom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.rdCupom.Location = new System.Drawing.Point(164, 230);
            this.rdCupom.Name = "rdCupom";
            this.rdCupom.Size = new System.Drawing.Size(153, 23);
            this.rdCupom.TabIndex = 2;
            this.rdCupom.TabStop = true;
            this.rdCupom.Text = "Cupom de Desconto";
            this.rdCupom.CheckedChanged += new System.EventHandler(this.RdPremio_CheckedChanged);
            // 
            // rdProduto
            // 
            this.rdProduto.AutoSize = true;
            this.rdProduto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.rdProduto.Location = new System.Drawing.Point(320, 230);
            this.rdProduto.Name = "rdProduto";
            this.rdProduto.Size = new System.Drawing.Size(77, 23);
            this.rdProduto.TabIndex = 3;
            this.rdProduto.Text = "Produto";
            this.rdProduto.CheckedChanged += new System.EventHandler(this.RdPremio_CheckedChanged);
            // 
            // pnlCupom
            // 
            this.pnlCupom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(236)))), ((int)(((byte)(224)))));
            this.pnlCupom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCupom.Controls.Add(this.lblCupomTipo);
            this.pnlCupom.Controls.Add(this.cmbCupomTipo);
            this.pnlCupom.Controls.Add(this.lblCupomValor);
            this.pnlCupom.Controls.Add(this.numCupomValor);
            this.pnlCupom.Controls.Add(this.lblCupomMin);
            this.pnlCupom.Controls.Add(this.numCupomMin);
            this.pnlCupom.Controls.Add(this.lblCupomVal);
            this.pnlCupom.Controls.Add(this.numCupomValidade);
            this.pnlCupom.Location = new System.Drawing.Point(16, 266);
            this.pnlCupom.Name = "pnlCupom";
            this.pnlCupom.Size = new System.Drawing.Size(750, 112);
            this.pnlCupom.TabIndex = 4;
            // 
            // lblCupomTipo
            // 
            this.lblCupomTipo.AutoSize = true;
            this.lblCupomTipo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblCupomTipo.Location = new System.Drawing.Point(8, 14);
            this.lblCupomTipo.Name = "lblCupomTipo";
            this.lblCupomTipo.Size = new System.Drawing.Size(70, 19);
            this.lblCupomTipo.TabIndex = 0;
            this.lblCupomTipo.Text = "Desconto:";
            // 
            // cmbCupomTipo
            // 
            this.cmbCupomTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCupomTipo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCupomTipo.Items.AddRange(new object[] {
            "PERCENTUAL",
            "FIXO"});
            this.cmbCupomTipo.Location = new System.Drawing.Point(110, 10);
            this.cmbCupomTipo.Name = "cmbCupomTipo";
            this.cmbCupomTipo.Size = new System.Drawing.Size(160, 25);
            this.cmbCupomTipo.TabIndex = 0;
            // 
            // lblCupomValor
            // 
            this.lblCupomValor.AutoSize = true;
            this.lblCupomValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblCupomValor.Location = new System.Drawing.Point(290, 14);
            this.lblCupomValor.Name = "lblCupomValor";
            this.lblCupomValor.Size = new System.Drawing.Size(43, 19);
            this.lblCupomValor.TabIndex = 1;
            this.lblCupomValor.Text = "Valor:";
            // 
            // numCupomValor
            // 
            this.numCupomValor.DecimalPlaces = 2;
            this.numCupomValor.Location = new System.Drawing.Point(340, 10);
            this.numCupomValor.Name = "numCupomValor";
            this.numCupomValor.Size = new System.Drawing.Size(100, 25);
            this.numCupomValor.TabIndex = 1;
            this.numCupomValor.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblCupomMin
            // 
            this.lblCupomMin.AutoSize = true;
            this.lblCupomMin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblCupomMin.Location = new System.Drawing.Point(8, 54);
            this.lblCupomMin.Name = "lblCupomMin";
            this.lblCupomMin.Size = new System.Drawing.Size(132, 19);
            this.lblCupomMin.TabIndex = 2;
            this.lblCupomMin.Text = "Pedido Mínimo (R$):";
            // 
            // numCupomMin
            // 
            this.numCupomMin.DecimalPlaces = 2;
            this.numCupomMin.Location = new System.Drawing.Point(154, 50);
            this.numCupomMin.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numCupomMin.Name = "numCupomMin";
            this.numCupomMin.Size = new System.Drawing.Size(116, 25);
            this.numCupomMin.TabIndex = 2;
            // 
            // lblCupomVal
            // 
            this.lblCupomVal.AutoSize = true;
            this.lblCupomVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblCupomVal.Location = new System.Drawing.Point(290, 54);
            this.lblCupomVal.Name = "lblCupomVal";
            this.lblCupomVal.Size = new System.Drawing.Size(99, 19);
            this.lblCupomVal.TabIndex = 3;
            this.lblCupomVal.Text = "Validade (dias):";
            // 
            // numCupomValidade
            // 
            this.numCupomValidade.Location = new System.Drawing.Point(410, 50);
            this.numCupomValidade.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.numCupomValidade.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCupomValidade.Name = "numCupomValidade";
            this.numCupomValidade.Size = new System.Drawing.Size(80, 25);
            this.numCupomValidade.TabIndex = 3;
            this.numCupomValidade.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // pnlProduto
            // 
            this.pnlProduto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(236)))), ((int)(((byte)(224)))));
            this.pnlProduto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlProduto.Controls.Add(this.lblProdNome);
            this.pnlProduto.Controls.Add(this.txtProdNome);
            this.pnlProduto.Controls.Add(this.btnBuscarProduto);
            this.pnlProduto.Controls.Add(this.lblProdQtde);
            this.pnlProduto.Controls.Add(this.numProdQtde);
            this.pnlProduto.Location = new System.Drawing.Point(16, 266);
            this.pnlProduto.Name = "pnlProduto";
            this.pnlProduto.Size = new System.Drawing.Size(750, 82);
            this.pnlProduto.TabIndex = 5;
            this.pnlProduto.Visible = false;
            // 
            // lblProdNome
            // 
            this.lblProdNome.AutoSize = true;
            this.lblProdNome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblProdNome.Location = new System.Drawing.Point(8, 13);
            this.lblProdNome.Name = "lblProdNome";
            this.lblProdNome.Size = new System.Drawing.Size(170, 19);
            this.lblProdNome.TabIndex = 0;
            this.lblProdNome.Text = "Nome do Produto Prêmio:";
            // 
            // txtProdNome
            // 
            this.txtProdNome.BackColor = System.Drawing.Color.White;
            this.txtProdNome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProdNome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtProdNome.Location = new System.Drawing.Point(186, 9);
            this.txtProdNome.MaxLength = 149;
            this.txtProdNome.Name = "txtProdNome";
            this.txtProdNome.Size = new System.Drawing.Size(300, 25);
            this.txtProdNome.TabIndex = 0;
            this.txtProdNome.ReadOnly = true;
            // 
            // btnBuscarProduto
            // 
            this.btnBuscarProduto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            this.btnBuscarProduto.FlatAppearance.BorderSize = 0;
            this.btnBuscarProduto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarProduto.ForeColor = System.Drawing.Color.White;
            this.btnBuscarProduto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBuscarProduto.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnBuscarProduto.Location = new System.Drawing.Point(494, 7);
            this.btnBuscarProduto.Name = "btnBuscarProduto";
            this.btnBuscarProduto.Size = new System.Drawing.Size(110, 27);
            this.btnBuscarProduto.TabIndex = 1;
            this.btnBuscarProduto.Text = "🔍 Buscar";
            this.btnBuscarProduto.UseVisualStyleBackColor = false;
            this.btnBuscarProduto.Click += new System.EventHandler(this.BtnBuscarProduto_Click);
            // 
            // lblProdQtde
            // 
            this.lblProdQtde.AutoSize = true;
            this.lblProdQtde.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblProdQtde.Location = new System.Drawing.Point(8, 50);
            this.lblProdQtde.Name = "lblProdQtde";
            this.lblProdQtde.Size = new System.Drawing.Size(170, 19);
            this.lblProdQtde.TabIndex = 2;
            this.lblProdQtde.Text = "Quantidade Prêmio:";
            // 
            // numProdQtde
            // 
            this.numProdQtde.BackColor = System.Drawing.Color.White;
            this.numProdQtde.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numProdQtde.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.numProdQtde.Location = new System.Drawing.Point(186, 47);
            this.numProdQtde.Minimum = new decimal(new int[] {1, 0, 0, 0});
            this.numProdQtde.Maximum = new decimal(new int[] {99, 0, 0, 0});
            this.numProdQtde.Value   = new decimal(new int[] {1, 0, 0, 0});
            this.numProdQtde.Name = "numProdQtde";
            this.numProdQtde.Size = new System.Drawing.Size(80, 25);
            this.numProdQtde.TabIndex = 3;
            this.numProdQtde.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblMsg.Location = new System.Drawing.Point(16, 410);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(148, 19);
            this.lblMsg.TabIndex = 6;
            this.lblMsg.Text = "Mensagem WhatsApp:";
            // 
            // txtMsg
            // 
            this.txtMsg.BackColor = System.Drawing.Color.White;
            this.txtMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtMsg.Location = new System.Drawing.Point(16, 432);
            this.txtMsg.MaxLength = 800;
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMsg.Size = new System.Drawing.Size(750, 100);
            this.txtMsg.TabIndex = 10;
            // 
            // lblTags
            // 
            this.lblTags.AutoSize = true;
            this.lblTags.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblTags.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(100)))), ((int)(((byte)(70)))));
            this.lblTags.Location = new System.Drawing.Point(16, 385);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(407, 13);
            this.lblTags.TabIndex = 11;
            this.lblTags.Text = "Tags disponíveis: {Nome}  {Meta}  {CupomCodigo}  {Validade}  {Produto}  {TotalGas" +
    "to}";
            // 
            // btnSalvar
            // 
            this.btnSalvar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            this.btnSalvar.FlatAppearance.BorderSize = 0;
            this.btnSalvar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSalvar.ForeColor = System.Drawing.Color.White;
            this.btnSalvar.Location = new System.Drawing.Point(320, 538);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(136, 36);
            this.btnSalvar.TabIndex = 11;
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.UseVisualStyleBackColor = false;
            this.btnSalvar.Click += new System.EventHandler(this.BtnSalvar_Click);
            // 
            // pnlListaBotoes
            // 
            this.pnlListaBotoes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(236)))), ((int)(((byte)(224)))));
            this.pnlListaBotoes.Controls.Add(this.btnNovaRegra);
            this.pnlListaBotoes.Controls.Add(this.btnExcluirRegra);
            this.pnlListaBotoes.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlListaBotoes.Location = new System.Drawing.Point(8, 108);
            this.pnlListaBotoes.Name = "pnlListaBotoes";
            this.pnlListaBotoes.Size = new System.Drawing.Size(776, 32);
            this.pnlListaBotoes.TabIndex = 12;
            // 
            // btnNovaRegra
            // 
            this.btnNovaRegra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.btnNovaRegra.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNovaRegra.FlatAppearance.BorderSize = 0;
            this.btnNovaRegra.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNovaRegra.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnNovaRegra.ForeColor = System.Drawing.Color.White;
            this.btnNovaRegra.Location = new System.Drawing.Point(8, 4);
            this.btnNovaRegra.Name = "btnNovaRegra";
            this.btnNovaRegra.Size = new System.Drawing.Size(110, 24);
            this.btnNovaRegra.TabIndex = 0;
            this.btnNovaRegra.Text = "+ Nova Regra";
            this.btnNovaRegra.UseVisualStyleBackColor = false;
            this.btnNovaRegra.Click += new System.EventHandler(this.BtnNovaRegra_Click);
            // 
            // btnExcluirRegra
            // 
            this.btnExcluirRegra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnExcluirRegra.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExcluirRegra.FlatAppearance.BorderSize = 0;
            this.btnExcluirRegra.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcluirRegra.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnExcluirRegra.ForeColor = System.Drawing.Color.White;
            this.btnExcluirRegra.Location = new System.Drawing.Point(126, 4);
            this.btnExcluirRegra.Name = "btnExcluirRegra";
            this.btnExcluirRegra.Size = new System.Drawing.Size(142, 24);
            this.btnExcluirRegra.TabIndex = 1;
            this.btnExcluirRegra.Text = "Excluir Selecionada";
            this.btnExcluirRegra.UseVisualStyleBackColor = false;
            this.btnExcluirRegra.Click += new System.EventHandler(this.BtnExcluirRegra_Click);
            // 
            // pnlLista
            // 
            this.pnlLista.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(245)))), ((int)(((byte)(240)))));
            this.pnlLista.Controls.Add(this.gridConfigs);
            this.pnlLista.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLista.Location = new System.Drawing.Point(8, 8);
            this.pnlLista.Name = "pnlLista";
            this.pnlLista.Size = new System.Drawing.Size(776, 100);
            this.pnlLista.TabIndex = 13;
            // 
            // gridConfigs
            // 
            this.gridConfigs.AllowUserToAddRows = false;
            this.gridConfigs.AllowUserToDeleteRows = false;
            this.gridConfigs.AllowUserToResizeRows = false;
            this.gridConfigs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridConfigs.BackgroundColor = System.Drawing.Color.White;
            this.gridConfigs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridConfigs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridConfigs.ColumnHeadersHeight = 30;
            this.gridConfigs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(40)))), ((int)(((byte)(30)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridConfigs.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridConfigs.Location = new System.Drawing.Point(0, 0);
            this.gridConfigs.MultiSelect = false;
            this.gridConfigs.Name = "gridConfigs";
            this.gridConfigs.ReadOnly = true;
            this.gridConfigs.RowHeadersVisible = false;
            this.gridConfigs.RowTemplate.Height = 24;
            this.gridConfigs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridConfigs.Size = new System.Drawing.Size(776, 100);
            this.gridConfigs.TabIndex = 0;
            this.gridConfigs.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            this.gridConfigs.SelectionChanged += new System.EventHandler(this.GridConfigs_SelectionChanged);
            // 
            // tabHistorico
            // 
            this.tabHistorico.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(245)))), ((int)(((byte)(240)))));
            this.tabHistorico.Controls.Add(this.pnlFiltros);
            this.tabHistorico.Controls.Add(this.gridHistorico);
            this.tabHistorico.Location = new System.Drawing.Point(4, 30);
            this.tabHistorico.Name = "tabHistorico";
            this.tabHistorico.Padding = new System.Windows.Forms.Padding(8);
            this.tabHistorico.Size = new System.Drawing.Size(792, 534);
            this.tabHistorico.TabIndex = 1;
            this.tabHistorico.Text = "Histórico de Prêmios";
            // 
            // pnlFiltros
            // 
            this.pnlFiltros.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(236)))), ((int)(((byte)(224)))));
            this.pnlFiltros.Controls.Add(this.lblDe);
            this.pnlFiltros.Controls.Add(this.dtpDe);
            this.pnlFiltros.Controls.Add(this.lblAte);
            this.pnlFiltros.Controls.Add(this.dtpAte);
            this.pnlFiltros.Controls.Add(this.btnFiltrar);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Location = new System.Drawing.Point(8, 8);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(776, 48);
            this.pnlFiltros.TabIndex = 0;
            // 
            // lblDe
            // 
            this.lblDe.AutoSize = true;
            this.lblDe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblDe.Location = new System.Drawing.Point(8, 14);
            this.lblDe.Name = "lblDe";
            this.lblDe.Size = new System.Drawing.Size(29, 19);
            this.lblDe.TabIndex = 0;
            this.lblDe.Text = "De:";
            // 
            // dtpDe
            // 
            this.dtpDe.CustomFormat = "dd/MM/yyyy";
            this.dtpDe.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDe.Location = new System.Drawing.Point(40, 10);
            this.dtpDe.Name = "dtpDe";
            this.dtpDe.Size = new System.Drawing.Size(120, 25);
            this.dtpDe.TabIndex = 0;
            // 
            // lblAte
            // 
            this.lblAte.AutoSize = true;
            this.lblAte.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblAte.Location = new System.Drawing.Point(174, 14);
            this.lblAte.Name = "lblAte";
            this.lblAte.Size = new System.Drawing.Size(33, 19);
            this.lblAte.TabIndex = 1;
            this.lblAte.Text = "Até:";
            // 
            // dtpAte
            // 
            this.dtpAte.CustomFormat = "dd/MM/yyyy";
            this.dtpAte.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpAte.Location = new System.Drawing.Point(206, 10);
            this.dtpAte.Name = "dtpAte";
            this.dtpAte.Size = new System.Drawing.Size(120, 25);
            this.dtpAte.TabIndex = 1;
            // 
            // btnFiltrar
            // 
            this.btnFiltrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            this.btnFiltrar.FlatAppearance.BorderSize = 0;
            this.btnFiltrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFiltrar.ForeColor = System.Drawing.Color.White;
            this.btnFiltrar.Location = new System.Drawing.Point(340, 10);
            this.btnFiltrar.Name = "btnFiltrar";
            this.btnFiltrar.Size = new System.Drawing.Size(96, 28);
            this.btnFiltrar.TabIndex = 2;
            this.btnFiltrar.Text = "Filtrar";
            this.btnFiltrar.UseVisualStyleBackColor = false;
            this.btnFiltrar.Click += new System.EventHandler(this.BtnFiltrar_Click);
            // 
            // gridHistorico
            // 
            this.gridHistorico.AllowUserToAddRows = false;
            this.gridHistorico.AllowUserToDeleteRows = false;
            this.gridHistorico.AllowUserToResizeRows = false;
            this.gridHistorico.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridHistorico.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(245)))), ((int)(((byte)(240)))));
            this.gridHistorico.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridHistorico.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridHistorico.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridHistorico.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(210)))), ((int)(((byte)(195)))));
            this.gridHistorico.Location = new System.Drawing.Point(8, 8);
            this.gridHistorico.MultiSelect = false;
            this.gridHistorico.Name = "gridHistorico";
            this.gridHistorico.ReadOnly = true;
            this.gridHistorico.RowHeadersVisible = false;
            this.gridHistorico.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridHistorico.Size = new System.Drawing.Size(776, 518);
            this.gridHistorico.TabIndex = 0;
            this.gridHistorico.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // 
            // frmFidelizacao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(245)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(800, 720);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFidelizacao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Fidelização de Clientes";
            this.pnlTop.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabConfig.ResumeLayout(false);
            this.tabConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMeta)).EndInit();
            this.pnlCupom.ResumeLayout(false);
            this.pnlCupom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomValor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomValidade)).EndInit();
            this.pnlProduto.ResumeLayout(false);
            this.pnlProduto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numProdQtde)).EndInit();
            this.pnlListaBotoes.ResumeLayout(false);
            this.pnlLista.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridConfigs)).EndInit();
            this.tabHistorico.ResumeLayout(false);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridHistorico)).EndInit();
            this.ResumeLayout(false);

        }

        // ── Field declarations ────────────────────────────────────────────────
        private System.Windows.Forms.Panel        pnlTop;
        private System.Windows.Forms.Label        lblTitulo;
        private System.Windows.Forms.TabControl   tabControl;
        private System.Windows.Forms.TabPage      tabConfig;
        private System.Windows.Forms.TabPage      tabHistorico;
        private System.Windows.Forms.Panel        pnlLista;
        internal System.Windows.Forms.DataGridView gridConfigs;
        private System.Windows.Forms.Panel        pnlListaBotoes;
        private System.Windows.Forms.Button       btnNovaRegra;
        private System.Windows.Forms.Button       btnExcluirRegra;
        private System.Windows.Forms.CheckBox     chkAtivo;
        private System.Windows.Forms.Label        lblNomeRegra;
        private System.Windows.Forms.TextBox      txtNomeRegra;
        private System.Windows.Forms.Label        lblMeta;
        private System.Windows.Forms.NumericUpDown numMeta;
        private System.Windows.Forms.Label        lblPremio;
        private System.Windows.Forms.RadioButton  rdCupom;
        private System.Windows.Forms.RadioButton  rdProduto;
        private System.Windows.Forms.Panel        pnlCupom;
        private System.Windows.Forms.Label        lblCupomTipo;
        private System.Windows.Forms.ComboBox     cmbCupomTipo;
        private System.Windows.Forms.Label        lblCupomValor;
        private System.Windows.Forms.NumericUpDown numCupomValor;
        private System.Windows.Forms.Label        lblCupomMin;
        private System.Windows.Forms.NumericUpDown numCupomMin;
        private System.Windows.Forms.Label        lblCupomVal;
        private System.Windows.Forms.NumericUpDown numCupomValidade;
        private System.Windows.Forms.Panel        pnlProduto;
        private System.Windows.Forms.Label        lblProdNome;
        private System.Windows.Forms.TextBox      txtProdNome;
        private System.Windows.Forms.Button        btnBuscarProduto;
        private System.Windows.Forms.Label        lblProdQtde;
        private System.Windows.Forms.NumericUpDown numProdQtde;
        private System.Windows.Forms.Label        lblMsg;
        private System.Windows.Forms.TextBox      txtMsg;
        private System.Windows.Forms.Label        lblTags;
        private System.Windows.Forms.Button       btnSalvar;
        private System.Windows.Forms.Panel        pnlFiltros;
        private System.Windows.Forms.Label        lblDe;
        private System.Windows.Forms.DateTimePicker dtpDe;
        private System.Windows.Forms.Label        lblAte;
        private System.Windows.Forms.DateTimePicker dtpAte;
        private System.Windows.Forms.Button       btnFiltrar;
        internal System.Windows.Forms.DataGridView gridHistorico;
    }
}
