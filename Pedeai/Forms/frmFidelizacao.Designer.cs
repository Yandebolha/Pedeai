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
            // ── Allocate ────────────────────────────────────────────────────
            this.pnlTop          = new System.Windows.Forms.Panel();
            this.lblTitulo       = new System.Windows.Forms.Label();
            this.tabControl      = new System.Windows.Forms.TabControl();
            this.tabConfig       = new System.Windows.Forms.TabPage();
            this.tabHistorico    = new System.Windows.Forms.TabPage();

            // Tab 1 – Configuração
            this.chkAtivo        = new System.Windows.Forms.CheckBox();
            this.lblMeta         = new System.Windows.Forms.Label();
            this.numMeta         = new System.Windows.Forms.NumericUpDown();
            this.lblPremio       = new System.Windows.Forms.Label();
            this.rdCupom         = new System.Windows.Forms.RadioButton();
            this.rdProduto       = new System.Windows.Forms.RadioButton();
            this.pnlCupom        = new System.Windows.Forms.Panel();
            this.lblCupomTipo    = new System.Windows.Forms.Label();
            this.cmbCupomTipo    = new System.Windows.Forms.ComboBox();
            this.lblCupomValor   = new System.Windows.Forms.Label();
            this.numCupomValor   = new System.Windows.Forms.NumericUpDown();
            this.lblCupomMin     = new System.Windows.Forms.Label();
            this.numCupomMin     = new System.Windows.Forms.NumericUpDown();
            this.lblCupomVal     = new System.Windows.Forms.Label();
            this.numCupomValidade = new System.Windows.Forms.NumericUpDown();
            this.pnlProduto      = new System.Windows.Forms.Panel();
            this.lblProdNome     = new System.Windows.Forms.Label();
            this.txtProdNome     = new System.Windows.Forms.TextBox();
            this.lblMsg          = new System.Windows.Forms.Label();
            this.txtMsg          = new System.Windows.Forms.TextBox();
            this.lblTags         = new System.Windows.Forms.Label();
            this.btnSalvar       = new System.Windows.Forms.Button();

            // Tab 2 – Histórico
            this.pnlFiltros      = new System.Windows.Forms.Panel();
            this.lblDe           = new System.Windows.Forms.Label();
            this.dtpDe           = new System.Windows.Forms.DateTimePicker();
            this.lblAte          = new System.Windows.Forms.Label();
            this.dtpAte          = new System.Windows.Forms.DateTimePicker();
            this.btnFiltrar      = new System.Windows.Forms.Button();
            this.gridHistorico   = new System.Windows.Forms.DataGridView();

            // ── SuspendLayout ───────────────────────────────────────────────
            this.tabControl.SuspendLayout();
            this.tabConfig.SuspendLayout();
            this.tabHistorico.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMeta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomValor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomValidade)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridHistorico)).BeginInit();
            this.SuspendLayout();

            // ── pnlTop ──────────────────────────────────────────────────────
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height = 48;
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Controls.Add(this.lblTitulo);

            this.lblTitulo.AutoSize = false;
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Text = "🤝  Fidelização de Clientes";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitulo.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);

            // ── tabControl ──────────────────────────────────────────────────
            this.tabControl.Controls.Add(this.tabConfig);
            this.tabControl.Controls.Add(this.tabHistorico);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabControl.Location = new System.Drawing.Point(0, 48);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(14, 5);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(800, 568);
            this.tabControl.TabIndex = 1;

            // ── tabConfig ───────────────────────────────────────────────────
            this.tabConfig.BackColor = System.Drawing.Color.FromArgb(248, 245, 240);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.Padding = new System.Windows.Forms.Padding(8);
            this.tabConfig.Text = "Configuração";
            this.tabConfig.Controls.Add(this.chkAtivo);
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

            // ── tabHistorico ─────────────────────────────────────────────────
            this.tabHistorico.BackColor = System.Drawing.Color.FromArgb(248, 245, 240);
            this.tabHistorico.Name = "tabHistorico";
            this.tabHistorico.Padding = new System.Windows.Forms.Padding(8);
            this.tabHistorico.Text = "Histórico de Prêmios";
            this.tabHistorico.Controls.Add(this.pnlFiltros);
            this.tabHistorico.Controls.Add(this.gridHistorico);

            // ── chkAtivo ────────────────────────────────────────────────────
            this.chkAtivo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.chkAtivo.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.chkAtivo.Location = new System.Drawing.Point(16, 20);
            this.chkAtivo.Name = "chkAtivo";
            this.chkAtivo.Size = new System.Drawing.Size(220, 26);
            this.chkAtivo.TabIndex = 0;
            this.chkAtivo.Text = "Ativar Fidelização";
            this.chkAtivo.CheckedChanged += new System.EventHandler(this.ChkAtivo_CheckedChanged);

            // ── lblMeta ──────────────────────────────────────────────────────
            this.lblMeta.AutoSize = true;
            this.lblMeta.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.lblMeta.Location = new System.Drawing.Point(16, 60);
            this.lblMeta.Name = "lblMeta";
            this.lblMeta.Text = "Meta de Gasto (R$):";

            // ── numMeta ──────────────────────────────────────────────────────
            this.numMeta.DecimalPlaces = 2;
            this.numMeta.Location = new System.Drawing.Point(164, 56);
            this.numMeta.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.numMeta.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numMeta.Name = "numMeta";
            this.numMeta.Size = new System.Drawing.Size(130, 27);
            this.numMeta.TabIndex = 1;
            this.numMeta.ThousandsSeparator = true;
            this.numMeta.Value = new decimal(new int[] { 500, 0, 0, 0 });

            // ── lblPremio ─────────────────────────────────────────────────────
            this.lblPremio.AutoSize = true;
            this.lblPremio.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.lblPremio.Location = new System.Drawing.Point(16, 100);
            this.lblPremio.Name = "lblPremio";
            this.lblPremio.Text = "Tipo de Prêmio:";

            // ── rdCupom ───────────────────────────────────────────────────────
            this.rdCupom.AutoSize = true;
            this.rdCupom.Checked = true;
            this.rdCupom.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.rdCupom.Location = new System.Drawing.Point(164, 98);
            this.rdCupom.Name = "rdCupom";
            this.rdCupom.Size = new System.Drawing.Size(140, 23);
            this.rdCupom.TabIndex = 2;
            this.rdCupom.TabStop = true;
            this.rdCupom.Text = "Cupom de Desconto";
            this.rdCupom.CheckedChanged += new System.EventHandler(this.RdPremio_CheckedChanged);

            // ── rdProduto ─────────────────────────────────────────────────────
            this.rdProduto.AutoSize = true;
            this.rdProduto.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.rdProduto.Location = new System.Drawing.Point(320, 98);
            this.rdProduto.Name = "rdProduto";
            this.rdProduto.Size = new System.Drawing.Size(100, 23);
            this.rdProduto.TabIndex = 3;
            this.rdProduto.Text = "Produto";
            this.rdProduto.CheckedChanged += new System.EventHandler(this.RdPremio_CheckedChanged);

            // ── pnlCupom ──────────────────────────────────────────────────────
            this.pnlCupom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCupom.Location = new System.Drawing.Point(16, 134);
            this.pnlCupom.Name = "pnlCupom";
            this.pnlCupom.Size = new System.Drawing.Size(750, 112);
            this.pnlCupom.BackColor = System.Drawing.Color.FromArgb(240, 236, 224);
            this.pnlCupom.Controls.Add(this.lblCupomTipo);
            this.pnlCupom.Controls.Add(this.cmbCupomTipo);
            this.pnlCupom.Controls.Add(this.lblCupomValor);
            this.pnlCupom.Controls.Add(this.numCupomValor);
            this.pnlCupom.Controls.Add(this.lblCupomMin);
            this.pnlCupom.Controls.Add(this.numCupomMin);
            this.pnlCupom.Controls.Add(this.lblCupomVal);
            this.pnlCupom.Controls.Add(this.numCupomValidade);

            // ── lblCupomTipo ──────────────────────────────────────────────────
            this.lblCupomTipo.AutoSize = true;
            this.lblCupomTipo.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.lblCupomTipo.Location = new System.Drawing.Point(8, 14);
            this.lblCupomTipo.Name = "lblCupomTipo";
            this.lblCupomTipo.Text = "Desconto:";

            // ── cmbCupomTipo ──────────────────────────────────────────────────
            this.cmbCupomTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCupomTipo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCupomTipo.Location = new System.Drawing.Point(110, 10);
            this.cmbCupomTipo.Name = "cmbCupomTipo";
            this.cmbCupomTipo.Size = new System.Drawing.Size(160, 27);
            this.cmbCupomTipo.TabIndex = 0;
            this.cmbCupomTipo.Items.Add("PERCENTUAL");
            this.cmbCupomTipo.Items.Add("FIXO");
            this.cmbCupomTipo.SelectedIndex = 0;

            // ── lblCupomValor ─────────────────────────────────────────────────
            this.lblCupomValor.AutoSize = true;
            this.lblCupomValor.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.lblCupomValor.Location = new System.Drawing.Point(290, 14);
            this.lblCupomValor.Name = "lblCupomValor";
            this.lblCupomValor.Text = "Valor:";

            // ── numCupomValor ─────────────────────────────────────────────────
            this.numCupomValor.DecimalPlaces = 2;
            this.numCupomValor.Location = new System.Drawing.Point(340, 10);
            this.numCupomValor.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numCupomValor.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numCupomValor.Name = "numCupomValor";
            this.numCupomValor.Size = new System.Drawing.Size(100, 27);
            this.numCupomValor.TabIndex = 1;
            this.numCupomValor.Value = new decimal(new int[] { 10, 0, 0, 0 });

            // ── lblCupomMin ───────────────────────────────────────────────────
            this.lblCupomMin.AutoSize = true;
            this.lblCupomMin.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.lblCupomMin.Location = new System.Drawing.Point(8, 54);
            this.lblCupomMin.Name = "lblCupomMin";
            this.lblCupomMin.Text = "Pedido Mínimo (R$):";

            // ── numCupomMin ───────────────────────────────────────────────────
            this.numCupomMin.DecimalPlaces = 2;
            this.numCupomMin.Location = new System.Drawing.Point(154, 50);
            this.numCupomMin.Maximum = new decimal(new int[] { 99999, 0, 0, 0 });
            this.numCupomMin.Name = "numCupomMin";
            this.numCupomMin.Size = new System.Drawing.Size(116, 27);
            this.numCupomMin.TabIndex = 2;
            this.numCupomMin.Value = new decimal(new int[] { 0, 0, 0, 0 });

            // ── lblCupomVal ───────────────────────────────────────────────────
            this.lblCupomVal.AutoSize = true;
            this.lblCupomVal.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.lblCupomVal.Location = new System.Drawing.Point(290, 54);
            this.lblCupomVal.Name = "lblCupomVal";
            this.lblCupomVal.Text = "Validade (dias):";

            // ── numCupomValidade ──────────────────────────────────────────────
            this.numCupomValidade.Location = new System.Drawing.Point(410, 50);
            this.numCupomValidade.Maximum = new decimal(new int[] { 365, 0, 0, 0 });
            this.numCupomValidade.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numCupomValidade.Name = "numCupomValidade";
            this.numCupomValidade.Size = new System.Drawing.Size(80, 27);
            this.numCupomValidade.TabIndex = 3;
            this.numCupomValidade.Value = new decimal(new int[] { 30, 0, 0, 0 });

            // ── pnlProduto ─────────────────────────────────────────────────────
            this.pnlProduto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlProduto.Location = new System.Drawing.Point(16, 134);
            this.pnlProduto.Name = "pnlProduto";
            this.pnlProduto.Size = new System.Drawing.Size(750, 48);
            this.pnlProduto.BackColor = System.Drawing.Color.FromArgb(240, 236, 224);
            this.pnlProduto.Visible = false;
            this.pnlProduto.Controls.Add(this.lblProdNome);
            this.pnlProduto.Controls.Add(this.txtProdNome);

            // ── lblProdNome ───────────────────────────────────────────────────
            this.lblProdNome.AutoSize = true;
            this.lblProdNome.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.lblProdNome.Location = new System.Drawing.Point(8, 13);
            this.lblProdNome.Name = "lblProdNome";
            this.lblProdNome.Text = "Nome do Produto Prêmio:";

            // ── txtProdNome ───────────────────────────────────────────────────
            this.txtProdNome.BackColor = System.Drawing.Color.White;
            this.txtProdNome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProdNome.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtProdNome.Location = new System.Drawing.Point(186, 9);
            this.txtProdNome.MaxLength = 149;
            this.txtProdNome.Name = "txtProdNome";
            this.txtProdNome.Size = new System.Drawing.Size(350, 27);
            this.txtProdNome.TabIndex = 0;

            // ── lblMsg ─────────────────────────────────────────────────────────
            this.lblMsg.AutoSize = true;
            this.lblMsg.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.lblMsg.Location = new System.Drawing.Point(16, 258);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Text = "Mensagem WhatsApp:";

            // ── txtMsg ─────────────────────────────────────────────────────────
            this.txtMsg.BackColor = System.Drawing.Color.White;
            this.txtMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMsg.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtMsg.Location = new System.Drawing.Point(16, 280);
            this.txtMsg.MaxLength = 800;
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMsg.Size = new System.Drawing.Size(750, 100);
            this.txtMsg.TabIndex = 10;

            // ── lblTags ────────────────────────────────────────────────────────
            this.lblTags.AutoSize = true;
            this.lblTags.ForeColor = System.Drawing.Color.FromArgb(120, 100, 70);
            this.lblTags.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblTags.Location = new System.Drawing.Point(16, 385);
            this.lblTags.Name = "lblTags";
            this.lblTags.Text = "Tags disponíveis: {Nome}  {Meta}  {CupomCodigo}  {Validade}  {Produto}  {TotalGasto}";

            // ── btnSalvar ──────────────────────────────────────────────────────
            this.btnSalvar.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.btnSalvar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvar.ForeColor = System.Drawing.Color.White;
            this.btnSalvar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSalvar.Location = new System.Drawing.Point(630, 410);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(136, 36);
            this.btnSalvar.TabIndex = 11;
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.FlatAppearance.BorderSize = 0;
            this.btnSalvar.Click += new System.EventHandler(this.BtnSalvar_Click);

            // ── pnlFiltros ─────────────────────────────────────────────────────
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Height = 48;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.BackColor = System.Drawing.Color.FromArgb(240, 236, 224);
            this.pnlFiltros.Controls.Add(this.lblDe);
            this.pnlFiltros.Controls.Add(this.dtpDe);
            this.pnlFiltros.Controls.Add(this.lblAte);
            this.pnlFiltros.Controls.Add(this.dtpAte);
            this.pnlFiltros.Controls.Add(this.btnFiltrar);

            // ── lblDe ──────────────────────────────────────────────────────────
            this.lblDe.AutoSize = true;
            this.lblDe.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.lblDe.Location = new System.Drawing.Point(8, 14);
            this.lblDe.Name = "lblDe";
            this.lblDe.Text = "De:";

            // ── dtpDe ──────────────────────────────────────────────────────────
            this.dtpDe.CustomFormat = "dd/MM/yyyy";
            this.dtpDe.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDe.Location = new System.Drawing.Point(40, 10);
            this.dtpDe.Name = "dtpDe";
            this.dtpDe.Size = new System.Drawing.Size(120, 27);
            this.dtpDe.TabIndex = 0;

            // ── lblAte ─────────────────────────────────────────────────────────
            this.lblAte.AutoSize = true;
            this.lblAte.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.lblAte.Location = new System.Drawing.Point(174, 14);
            this.lblAte.Name = "lblAte";
            this.lblAte.Text = "Até:";

            // ── dtpAte ─────────────────────────────────────────────────────────
            this.dtpAte.CustomFormat = "dd/MM/yyyy";
            this.dtpAte.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpAte.Location = new System.Drawing.Point(206, 10);
            this.dtpAte.Name = "dtpAte";
            this.dtpAte.Size = new System.Drawing.Size(120, 27);
            this.dtpAte.TabIndex = 1;

            // ── btnFiltrar ─────────────────────────────────────────────────────
            this.btnFiltrar.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.btnFiltrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFiltrar.ForeColor = System.Drawing.Color.White;
            this.btnFiltrar.Location = new System.Drawing.Point(340, 10);
            this.btnFiltrar.Name = "btnFiltrar";
            this.btnFiltrar.Size = new System.Drawing.Size(96, 28);
            this.btnFiltrar.TabIndex = 2;
            this.btnFiltrar.Text = "Filtrar";
            this.btnFiltrar.FlatAppearance.BorderSize = 0;
            this.btnFiltrar.Click += new System.EventHandler(this.BtnFiltrar_Click);

            // ── gridHistorico ──────────────────────────────────────────────────
            this.gridHistorico.AllowUserToAddRows = false;
            this.gridHistorico.AllowUserToDeleteRows = false;
            this.gridHistorico.AllowUserToResizeRows = false;
            this.gridHistorico.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridHistorico.BackgroundColor = System.Drawing.Color.FromArgb(248, 245, 240);
            this.gridHistorico.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridHistorico.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridHistorico.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridHistorico.GridColor = System.Drawing.Color.FromArgb(220, 210, 195);
            this.gridHistorico.MultiSelect = false;
            this.gridHistorico.Name = "gridHistorico";
            this.gridHistorico.ReadOnly = true;
            this.gridHistorico.RowHeadersVisible = false;
            this.gridHistorico.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridHistorico.TabIndex = 0;
            this.gridHistorico.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);

            // ── Form ────────────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 245, 240);
            this.ClientSize = new System.Drawing.Size(800, 616);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFidelizacao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Fidelização de Clientes";

            // ── ResumeLayout ────────────────────────────────────────────────────
            ((System.ComponentModel.ISupportInitialize)(this.numMeta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomValor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCupomValidade)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridHistorico)).EndInit();
            this.tabConfig.ResumeLayout(false);
            this.tabConfig.PerformLayout();
            this.tabHistorico.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // ── Field declarations ────────────────────────────────────────────────
        private System.Windows.Forms.Panel        pnlTop;
        private System.Windows.Forms.Label        lblTitulo;
        private System.Windows.Forms.TabControl   tabControl;
        private System.Windows.Forms.TabPage      tabConfig;
        private System.Windows.Forms.TabPage      tabHistorico;
        private System.Windows.Forms.CheckBox     chkAtivo;
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
