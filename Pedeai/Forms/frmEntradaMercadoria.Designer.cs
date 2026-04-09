using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmEntradaMercadoria
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
            this.gridEntradas = new System.Windows.Forms.DataGridView();
            this.gridItens = new System.Windows.Forms.DataGridView();
            this.gridParcelas = new System.Windows.Forms.DataGridView();
            this.pnlNovaEntrada = new System.Windows.Forms.Panel();
            this.pnlParcelasOuter = new System.Windows.Forms.Panel();
            this.pnlParcelasTop = new System.Windows.Forms.Panel();
            this.lblParcelas = new System.Windows.Forms.Label();
            this.numParcelas = new System.Windows.Forms.NumericUpDown();
            this.lblPrimVenc = new System.Windows.Forms.Label();
            this.dtpPrimVencimento = new System.Windows.Forms.DateTimePicker();
            this.btnGerarParcelas = new System.Windows.Forms.Button();
            this.pnlRodape = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnRemoverItem = new System.Windows.Forms.Button();
            this.btnConfirmarEntrada = new System.Windows.Forms.Button();
            this.btnFecharForm = new System.Windows.Forms.Button();
            this.pnlAddItem = new System.Windows.Forms.Panel();
            this.lblProduto = new System.Windows.Forms.Label();
            this.txtProdCod = new System.Windows.Forms.TextBox();
            this.txtProdNome = new System.Windows.Forms.TextBox();
            this.btnBuscarProd = new System.Windows.Forms.Button();
            this.lblQtde = new System.Windows.Forms.Label();
            this.numQtde = new System.Windows.Forms.NumericUpDown();
            this.chkFracionado = new System.Windows.Forms.CheckBox();
            this.lblFracEntrada = new System.Windows.Forms.Label();
            this.numFracEntrada = new System.Windows.Forms.NumericUpDown();
            this.txtUnidEntrada = new System.Windows.Forms.TextBox();
            this.lblIgual = new System.Windows.Forms.Label();
            this.numFracao = new System.Windows.Forms.NumericUpDown();
            this.txtUnidSaida = new System.Windows.Forms.TextBox();
            this.lblUnidadesEntrada = new System.Windows.Forms.Label();
            this.lblValor = new System.Windows.Forms.Label();
            this.numValorTotal = new System.Windows.Forms.NumericUpDown();
            this.lblCusto = new System.Windows.Forms.Label();
            this.numCustoItem = new System.Windows.Forms.NumericUpDown();
            this.chkAtualizarCusto = new System.Windows.Forms.CheckBox();
            this.btnAdicionarItem = new System.Windows.Forms.Button();
            this.pnlCabecalho = new System.Windows.Forms.Panel();
            this.lblFornecedor = new System.Windows.Forms.Label();
            this.txtFornCod = new System.Windows.Forms.TextBox();
            this.txtFornNome = new System.Windows.Forms.TextBox();
            this.btnBuscarForn = new System.Windows.Forms.Button();
            this.lblData = new System.Windows.Forms.Label();
            this.dtpData = new System.Windows.Forms.DateTimePicker();
            this.lblNumDoc = new System.Windows.Forms.Label();
            this.txtNumDoc = new System.Windows.Forms.TextBox();
            this.lblNovaEntradaTitulo = new System.Windows.Forms.Label();
            this.dtpDe = new System.Windows.Forms.DateTimePicker();
            this.dtpAte = new System.Windows.Forms.DateTimePicker();
            this.btnFiltrar = new System.Windows.Forms.Button();
            this.btnLimparFiltro = new System.Windows.Forms.Button();
            this.btnNovaEntrada = new System.Windows.Forms.Button();
            this.btnCancelarSel = new System.Windows.Forms.Button();
            this.txtObservacoes = new System.Windows.Forms.TextBox();
            this.lblObservacoes = new System.Windows.Forms.Label();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlFiltro = new System.Windows.Forms.Panel();
            this.pnlObs = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.gridEntradas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridItens)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridParcelas)).BeginInit();
            this.pnlNovaEntrada.SuspendLayout();
            this.pnlParcelasOuter.SuspendLayout();
            this.pnlParcelasTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numParcelas)).BeginInit();
            this.pnlRodape.SuspendLayout();
            this.pnlAddItem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQtde)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFracEntrada)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFracao)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numValorTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCustoItem)).BeginInit();
            this.pnlCabecalho.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlFiltro.SuspendLayout();
            this.pnlObs.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridEntradas
            // 
            this.gridEntradas.AllowUserToAddRows = false;
            this.gridEntradas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridEntradas.BackgroundColor = System.Drawing.Color.White;
            this.gridEntradas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridEntradas.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridEntradas.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridEntradas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridEntradas.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gridEntradas.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(185)))), ((int)(((byte)(160)))));
            this.gridEntradas.Location = new System.Drawing.Point(0, 110);
            this.gridEntradas.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gridEntradas.Name = "gridEntradas";
            this.gridEntradas.ReadOnly = true;
            this.gridEntradas.RowHeadersVisible = false;
            this.gridEntradas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridEntradas.Size = new System.Drawing.Size(1342, 133);
            this.gridEntradas.TabIndex = 0;
            this.gridEntradas.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // 
            // gridItens
            // 
            this.gridItens.AllowUserToAddRows = false;
            this.gridItens.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridItens.BackgroundColor = System.Drawing.Color.White;
            this.gridItens.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridItens.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.gridItens.ColumnHeadersHeight = 32;
            this.gridItens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridItens.DefaultCellStyle = dataGridViewCellStyle4;
            this.gridItens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItens.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gridItens.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(185)))), ((int)(((byte)(160)))));
            this.gridItens.Location = new System.Drawing.Point(0, 86);
            this.gridItens.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gridItens.Name = "gridItens";
            this.gridItens.ReadOnly = true;
            this.gridItens.RowHeadersVisible = false;
            this.gridItens.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridItens.Size = new System.Drawing.Size(1342, 288);
            this.gridItens.TabIndex = 0;
            this.gridItens.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // 
            // gridParcelas
            // 
            this.gridParcelas.AllowUserToAddRows = false;
            this.gridParcelas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridParcelas.BackgroundColor = System.Drawing.Color.White;
            this.gridParcelas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridParcelas.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridParcelas.DefaultCellStyle = dataGridViewCellStyle6;
            this.gridParcelas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridParcelas.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gridParcelas.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(185)))), ((int)(((byte)(160)))));
            this.gridParcelas.Location = new System.Drawing.Point(0, 39);
            this.gridParcelas.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gridParcelas.Name = "gridParcelas";
            this.gridParcelas.RowHeadersVisible = false;
            this.gridParcelas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridParcelas.Size = new System.Drawing.Size(1342, 76);
            this.gridParcelas.TabIndex = 0;
            this.gridParcelas.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // 
            // pnlNovaEntrada
            // 
            this.pnlNovaEntrada.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.pnlNovaEntrada.Controls.Add(this.gridItens);
            this.pnlNovaEntrada.Controls.Add(this.pnlParcelasOuter);
            this.pnlNovaEntrada.Controls.Add(this.pnlRodape);
            this.pnlNovaEntrada.Controls.Add(this.pnlAddItem);
            this.pnlNovaEntrada.Controls.Add(this.pnlCabecalho);
            this.pnlNovaEntrada.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlNovaEntrada.Location = new System.Drawing.Point(0, 243);
            this.pnlNovaEntrada.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlNovaEntrada.Name = "pnlNovaEntrada";
            this.pnlNovaEntrada.Size = new System.Drawing.Size(1342, 542);
            this.pnlNovaEntrada.TabIndex = 3;
            this.pnlNovaEntrada.Visible = false;
            // 
            // pnlParcelasOuter
            // 
            this.pnlParcelasOuter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.pnlParcelasOuter.Controls.Add(this.gridParcelas);
            this.pnlParcelasOuter.Controls.Add(this.pnlParcelasTop);
            this.pnlParcelasOuter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlParcelasOuter.Location = new System.Drawing.Point(0, 374);
            this.pnlParcelasOuter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlParcelasOuter.Name = "pnlParcelasOuter";
            this.pnlParcelasOuter.Size = new System.Drawing.Size(1342, 115);
            this.pnlParcelasOuter.TabIndex = 1;
            // 
            // pnlParcelasTop
            // 
            this.pnlParcelasTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(226)))), ((int)(((byte)(208)))));
            this.pnlParcelasTop.Controls.Add(this.lblParcelas);
            this.pnlParcelasTop.Controls.Add(this.numParcelas);
            this.pnlParcelasTop.Controls.Add(this.lblPrimVenc);
            this.pnlParcelasTop.Controls.Add(this.dtpPrimVencimento);
            this.pnlParcelasTop.Controls.Add(this.btnGerarParcelas);
            this.pnlParcelasTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlParcelasTop.Location = new System.Drawing.Point(0, 0);
            this.pnlParcelasTop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlParcelasTop.Name = "pnlParcelasTop";
            this.pnlParcelasTop.Size = new System.Drawing.Size(1342, 39);
            this.pnlParcelasTop.TabIndex = 1;
            // 
            // lblParcelas
            // 
            this.lblParcelas.AutoSize = true;
            this.lblParcelas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblParcelas.Location = new System.Drawing.Point(404, 10);
            this.lblParcelas.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblParcelas.Name = "lblParcelas";
            this.lblParcelas.Size = new System.Drawing.Size(53, 15);
            this.lblParcelas.TabIndex = 0;
            this.lblParcelas.Text = "Parcelas:";
            // 
            // numParcelas
            // 
            this.numParcelas.Location = new System.Drawing.Point(476, 6);
            this.numParcelas.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numParcelas.Maximum = new decimal(new int[] {
            36,
            0,
            0,
            0});
            this.numParcelas.Name = "numParcelas";
            this.numParcelas.Size = new System.Drawing.Size(64, 23);
            this.numParcelas.TabIndex = 1;
            // 
            // lblPrimVenc
            // 
            this.lblPrimVenc.AutoSize = true;
            this.lblPrimVenc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblPrimVenc.Location = new System.Drawing.Point(548, 10);
            this.lblPrimVenc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrimVenc.Name = "lblPrimVenc";
            this.lblPrimVenc.Size = new System.Drawing.Size(48, 15);
            this.lblPrimVenc.TabIndex = 2;
            this.lblPrimVenc.Text = "1ª Data:";
            // 
            // dtpPrimVencimento
            // 
            this.dtpPrimVencimento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpPrimVencimento.Location = new System.Drawing.Point(617, 6);
            this.dtpPrimVencimento.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dtpPrimVencimento.Name = "dtpPrimVencimento";
            this.dtpPrimVencimento.Size = new System.Drawing.Size(134, 23);
            this.dtpPrimVencimento.TabIndex = 3;
            // 
            // btnGerarParcelas
            // 
            this.btnGerarParcelas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnGerarParcelas.FlatAppearance.BorderSize = 0;
            this.btnGerarParcelas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGerarParcelas.ForeColor = System.Drawing.Color.White;
            this.btnGerarParcelas.Location = new System.Drawing.Point(761, 6);
            this.btnGerarParcelas.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGerarParcelas.Name = "btnGerarParcelas";
            this.btnGerarParcelas.Size = new System.Drawing.Size(93, 28);
            this.btnGerarParcelas.TabIndex = 4;
            this.btnGerarParcelas.Text = "↻ Gerar";
            this.btnGerarParcelas.UseVisualStyleBackColor = false;
            // 
            // pnlRodape
            // 
            this.pnlRodape.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.pnlRodape.Controls.Add(this.lblTotal);
            this.pnlRodape.Controls.Add(this.btnRemoverItem);
            this.pnlRodape.Controls.Add(this.btnConfirmarEntrada);
            this.pnlRodape.Controls.Add(this.btnFecharForm);
            this.pnlRodape.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRodape.Location = new System.Drawing.Point(0, 489);
            this.pnlRodape.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlRodape.Name = "pnlRodape";
            this.pnlRodape.Size = new System.Drawing.Size(1342, 53);
            this.pnlRodape.TabIndex = 2;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.lblTotal.Location = new System.Drawing.Point(9, 14);
            this.lblTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(106, 20);
            this.lblTotal.TabIndex = 0;
            this.lblTotal.Text = "Total: R$ 0,00";
            // 
            // btnRemoverItem
            // 
            this.btnRemoverItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnRemoverItem.FlatAppearance.BorderSize = 0;
            this.btnRemoverItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoverItem.ForeColor = System.Drawing.Color.White;
            this.btnRemoverItem.Location = new System.Drawing.Point(350, 10);
            this.btnRemoverItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRemoverItem.Name = "btnRemoverItem";
            this.btnRemoverItem.Size = new System.Drawing.Size(140, 32);
            this.btnRemoverItem.TabIndex = 1;
            this.btnRemoverItem.Text = "− Remover Item";
            this.btnRemoverItem.UseVisualStyleBackColor = false;
            // 
            // btnConfirmarEntrada
            // 
            this.btnConfirmarEntrada.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.btnConfirmarEntrada.FlatAppearance.BorderSize = 0;
            this.btnConfirmarEntrada.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmarEntrada.ForeColor = System.Drawing.Color.White;
            this.btnConfirmarEntrada.Location = new System.Drawing.Point(509, 10);
            this.btnConfirmarEntrada.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnConfirmarEntrada.Name = "btnConfirmarEntrada";
            this.btnConfirmarEntrada.Size = new System.Drawing.Size(187, 32);
            this.btnConfirmarEntrada.TabIndex = 2;
            this.btnConfirmarEntrada.Text = "✔ Confirmar";
            this.btnConfirmarEntrada.UseVisualStyleBackColor = false;
            // 
            // btnFecharForm
            // 
            this.btnFecharForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnFecharForm.FlatAppearance.BorderSize = 0;
            this.btnFecharForm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFecharForm.ForeColor = System.Drawing.Color.White;
            this.btnFecharForm.Location = new System.Drawing.Point(714, 10);
            this.btnFecharForm.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnFecharForm.Name = "btnFecharForm";
            this.btnFecharForm.Size = new System.Drawing.Size(117, 32);
            this.btnFecharForm.TabIndex = 3;
            this.btnFecharForm.Text = "Cancelar";
            this.btnFecharForm.UseVisualStyleBackColor = false;
            // 
            // pnlAddItem
            // 
            this.pnlAddItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(226)))), ((int)(((byte)(208)))));
            this.pnlAddItem.Controls.Add(this.lblProduto);
            this.pnlAddItem.Controls.Add(this.txtProdCod);
            this.pnlAddItem.Controls.Add(this.txtProdNome);
            this.pnlAddItem.Controls.Add(this.btnBuscarProd);
            this.pnlAddItem.Controls.Add(this.lblQtde);
            this.pnlAddItem.Controls.Add(this.numQtde);
            this.pnlAddItem.Controls.Add(this.chkFracionado);
            this.pnlAddItem.Controls.Add(this.lblFracEntrada);
            this.pnlAddItem.Controls.Add(this.numFracEntrada);
            this.pnlAddItem.Controls.Add(this.txtUnidEntrada);
            this.pnlAddItem.Controls.Add(this.lblIgual);
            this.pnlAddItem.Controls.Add(this.numFracao);
            this.pnlAddItem.Controls.Add(this.txtUnidSaida);
            this.pnlAddItem.Controls.Add(this.lblUnidadesEntrada);
            this.pnlAddItem.Controls.Add(this.lblValor);
            this.pnlAddItem.Controls.Add(this.numValorTotal);
            this.pnlAddItem.Controls.Add(this.lblCusto);
            this.pnlAddItem.Controls.Add(this.numCustoItem);
            this.pnlAddItem.Controls.Add(this.chkAtualizarCusto);
            this.pnlAddItem.Controls.Add(this.btnAdicionarItem);
            this.pnlAddItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAddItem.Location = new System.Drawing.Point(0, 42);
            this.pnlAddItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlAddItem.Name = "pnlAddItem";
            this.pnlAddItem.Size = new System.Drawing.Size(1342, 44);
            this.pnlAddItem.TabIndex = 3;
            // 
            // lblProduto
            // 
            this.lblProduto.AutoSize = true;
            this.lblProduto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblProduto.Location = new System.Drawing.Point(5, 13);
            this.lblProduto.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProduto.Name = "lblProduto";
            this.lblProduto.Size = new System.Drawing.Size(53, 15);
            this.lblProduto.TabIndex = 0;
            this.lblProduto.Text = "Produto:";
            // 
            // txtProdCod
            // 
            this.txtProdCod.Location = new System.Drawing.Point(72, 8);
            this.txtProdCod.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtProdCod.Name = "txtProdCod";
            this.txtProdCod.Size = new System.Drawing.Size(63, 23);
            this.txtProdCod.TabIndex = 1;
            // 
            // txtProdNome
            // 
            this.txtProdNome.Location = new System.Drawing.Point(144, 8);
            this.txtProdNome.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtProdNome.Name = "txtProdNome";
            this.txtProdNome.Size = new System.Drawing.Size(233, 23);
            this.txtProdNome.TabIndex = 2;
            // 
            // btnBuscarProd
            // 
            this.btnBuscarProd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnBuscarProd.FlatAppearance.BorderSize = 0;
            this.btnBuscarProd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarProd.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnBuscarProd.ForeColor = System.Drawing.Color.White;
            this.btnBuscarProd.Location = new System.Drawing.Point(384, 8);
            this.btnBuscarProd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnBuscarProd.Name = "btnBuscarProd";
            this.btnBuscarProd.Size = new System.Drawing.Size(33, 25);
            this.btnBuscarProd.TabIndex = 3;
            this.btnBuscarProd.Text = "🔍";
            this.btnBuscarProd.UseVisualStyleBackColor = false;
            // 
            // lblQtde
            // 
            this.lblQtde.AutoSize = true;
            this.lblQtde.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblQtde.Location = new System.Drawing.Point(425, 13);
            this.lblQtde.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblQtde.Name = "lblQtde";
            this.lblQtde.Size = new System.Drawing.Size(36, 15);
            this.lblQtde.TabIndex = 4;
            this.lblQtde.Text = "Qtde:";
            // 
            // numQtde
            // 
            this.numQtde.DecimalPlaces = 2;
            this.numQtde.Location = new System.Drawing.Point(461, 8);
            this.numQtde.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numQtde.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numQtde.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numQtde.Name = "numQtde";
            this.numQtde.Size = new System.Drawing.Size(76, 23);
            this.numQtde.TabIndex = 5;
            this.numQtde.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkFracionado
            // 
            this.chkFracionado.AutoSize = true;
            this.chkFracionado.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkFracionado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(80)))), ((int)(((byte)(50)))));
            this.chkFracionado.Location = new System.Drawing.Point(541, 10);
            this.chkFracionado.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkFracionado.Name = "chkFracionado";
            this.chkFracionado.Size = new System.Drawing.Size(51, 19);
            this.chkFracionado.TabIndex = 6;
            this.chkFracionado.Text = "Fração";
            this.chkFracionado.CheckedChanged += new System.EventHandler(this.chkFracionado_CheckedChanged);
            // 
            // lblFracEntrada
            // 
            this.lblFracEntrada.AutoSize = true;
            this.lblFracEntrada.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFracEntrada.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(80)))), ((int)(((byte)(50)))));
            this.lblFracEntrada.Location = new System.Drawing.Point(5, 54);
            this.lblFracEntrada.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFracEntrada.Name = "lblFracEntrada";
            this.lblFracEntrada.Size = new System.Drawing.Size(30, 15);
            this.lblFracEntrada.TabIndex = 7;
            this.lblFracEntrada.Text = "Ent.:";
            // 
            // numFracEntrada
            // 
            this.numFracEntrada.DecimalPlaces = 2;
            this.numFracEntrada.Location = new System.Drawing.Point(49, 51);
            this.numFracEntrada.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numFracEntrada.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numFracEntrada.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numFracEntrada.Name = "numFracEntrada";
            this.numFracEntrada.Size = new System.Drawing.Size(68, 23);
            this.numFracEntrada.TabIndex = 8;
            this.numFracEntrada.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtUnidEntrada
            // 
            this.txtUnidEntrada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUnidEntrada.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtUnidEntrada.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtUnidEntrada.Location = new System.Drawing.Point(121, 51);
            this.txtUnidEntrada.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtUnidEntrada.MaxLength = 10;
            this.txtUnidEntrada.Name = "txtUnidEntrada";
            this.txtUnidEntrada.Size = new System.Drawing.Size(42, 23);
            this.txtUnidEntrada.TabIndex = 9;
            // 
            // lblIgual
            // 
            this.lblIgual.AutoSize = true;
            this.lblIgual.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblIgual.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(80)))), ((int)(((byte)(50)))));
            this.lblIgual.Location = new System.Drawing.Point(168, 54);
            this.lblIgual.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIgual.Name = "lblIgual";
            this.lblIgual.Size = new System.Drawing.Size(15, 15);
            this.lblIgual.TabIndex = 10;
            this.lblIgual.Text = "×";
            // 
            // numFracao
            // 
            this.numFracao.DecimalPlaces = 4;
            this.numFracao.Location = new System.Drawing.Point(180, 51);
            this.numFracao.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numFracao.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numFracao.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numFracao.Name = "numFracao";
            this.numFracao.Size = new System.Drawing.Size(72, 23);
            this.numFracao.TabIndex = 11;
            this.numFracao.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtUnidSaida
            // 
            this.txtUnidSaida.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUnidSaida.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtUnidSaida.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtUnidSaida.Location = new System.Drawing.Point(257, 51);
            this.txtUnidSaida.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtUnidSaida.MaxLength = 10;
            this.txtUnidSaida.Name = "txtUnidSaida";
            this.txtUnidSaida.Size = new System.Drawing.Size(42, 23);
            this.txtUnidSaida.TabIndex = 12;
            // 
            // lblUnidadesEntrada
            // 
            this.lblUnidadesEntrada.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblUnidadesEntrada.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.lblUnidadesEntrada.Location = new System.Drawing.Point(303, 54);
            this.lblUnidadesEntrada.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUnidadesEntrada.Name = "lblUnidadesEntrada";
            this.lblUnidadesEntrada.Size = new System.Drawing.Size(128, 27);
            this.lblUnidadesEntrada.TabIndex = 13;
            this.lblUnidadesEntrada.Text = "= 0 UN";
            // 
            // lblValor
            // 
            this.lblValor.AutoSize = true;
            this.lblValor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblValor.Location = new System.Drawing.Point(604, 13);
            this.lblValor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValor.Name = "lblValor";
            this.lblValor.Size = new System.Drawing.Size(52, 15);
            this.lblValor.TabIndex = 14;
            this.lblValor.Text = "Valor R$:";
            // 
            // numValorTotal
            // 
            this.numValorTotal.DecimalPlaces = 2;
            this.numValorTotal.Location = new System.Drawing.Point(674, 8);
            this.numValorTotal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numValorTotal.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numValorTotal.Name = "numValorTotal";
            this.numValorTotal.Size = new System.Drawing.Size(99, 23);
            this.numValorTotal.TabIndex = 15;
            // 
            // lblCusto
            // 
            this.lblCusto.AutoSize = true;
            this.lblCusto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblCusto.Location = new System.Drawing.Point(778, 13);
            this.lblCusto.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCusto.Name = "lblCusto";
            this.lblCusto.Size = new System.Drawing.Size(41, 15);
            this.lblCusto.TabIndex = 16;
            this.lblCusto.Text = "Custo:";
            // 
            // numCustoItem
            // 
            this.numCustoItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(232)))), ((int)(((byte)(215)))));
            this.numCustoItem.DecimalPlaces = 2;
            this.numCustoItem.Location = new System.Drawing.Point(827, 8);
            this.numCustoItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numCustoItem.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numCustoItem.Name = "numCustoItem";
            this.numCustoItem.ReadOnly = true;
            this.numCustoItem.Size = new System.Drawing.Size(93, 23);
            this.numCustoItem.TabIndex = 17;
            // 
            // chkAtualizarCusto
            // 
            this.chkAtualizarCusto.AutoSize = true;
            this.chkAtualizarCusto.Checked = false;
            this.chkAtualizarCusto.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.chkAtualizarCusto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.chkAtualizarCusto.Location = new System.Drawing.Point(926, 10);
            this.chkAtualizarCusto.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkAtualizarCusto.Name = "chkAtualizarCusto";
            this.chkAtualizarCusto.Size = new System.Drawing.Size(100, 19);
            this.chkAtualizarCusto.TabIndex = 18;
            this.chkAtualizarCusto.Text = "Atualiza Custo";
            // 
            // btnAdicionarItem
            // 
            this.btnAdicionarItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.btnAdicionarItem.FlatAppearance.BorderSize = 0;
            this.btnAdicionarItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdicionarItem.ForeColor = System.Drawing.Color.White;
            this.btnAdicionarItem.Location = new System.Drawing.Point(1237, 8);
            this.btnAdicionarItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAdicionarItem.Name = "btnAdicionarItem";
            this.btnAdicionarItem.Size = new System.Drawing.Size(99, 25);
            this.btnAdicionarItem.TabIndex = 19;
            this.btnAdicionarItem.Text = "+ Adicionar";
            this.btnAdicionarItem.UseVisualStyleBackColor = false;
            // 
            // pnlCabecalho
            // 
            this.pnlCabecalho.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.pnlCabecalho.Controls.Add(this.lblFornecedor);
            this.pnlCabecalho.Controls.Add(this.txtFornCod);
            this.pnlCabecalho.Controls.Add(this.txtFornNome);
            this.pnlCabecalho.Controls.Add(this.btnBuscarForn);
            this.pnlCabecalho.Controls.Add(this.lblData);
            this.pnlCabecalho.Controls.Add(this.dtpData);
            this.pnlCabecalho.Controls.Add(this.lblNumDoc);
            this.pnlCabecalho.Controls.Add(this.txtNumDoc);
            this.pnlCabecalho.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCabecalho.Location = new System.Drawing.Point(0, 0);
            this.pnlCabecalho.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlCabecalho.Name = "pnlCabecalho";
            this.pnlCabecalho.Size = new System.Drawing.Size(1342, 42);
            this.pnlCabecalho.TabIndex = 4;
            // 
            // lblFornecedor
            // 
            this.lblFornecedor.AutoSize = true;
            this.lblFornecedor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblFornecedor.Location = new System.Drawing.Point(5, 13);
            this.lblFornecedor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFornecedor.Name = "lblFornecedor";
            this.lblFornecedor.Size = new System.Drawing.Size(70, 15);
            this.lblFornecedor.TabIndex = 0;
            this.lblFornecedor.Text = "Fornecedor:";
            // 
            // txtFornCod
            // 
            this.txtFornCod.Location = new System.Drawing.Point(102, 8);
            this.txtFornCod.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFornCod.Name = "txtFornCod";
            this.txtFornCod.Size = new System.Drawing.Size(63, 23);
            this.txtFornCod.TabIndex = 1;
            // 
            // txtFornNome
            // 
            this.txtFornNome.Location = new System.Drawing.Point(173, 8);
            this.txtFornNome.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFornNome.Name = "txtFornNome";
            this.txtFornNome.ReadOnly = true;
            this.txtFornNome.Size = new System.Drawing.Size(233, 23);
            this.txtFornNome.TabIndex = 2;
            // 
            // btnBuscarForn
            // 
            this.btnBuscarForn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnBuscarForn.FlatAppearance.BorderSize = 0;
            this.btnBuscarForn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarForn.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnBuscarForn.ForeColor = System.Drawing.Color.White;
            this.btnBuscarForn.Location = new System.Drawing.Point(413, 8);
            this.btnBuscarForn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnBuscarForn.Name = "btnBuscarForn";
            this.btnBuscarForn.Size = new System.Drawing.Size(33, 25);
            this.btnBuscarForn.TabIndex = 3;
            this.btnBuscarForn.Text = "🔍";
            this.btnBuscarForn.UseVisualStyleBackColor = false;
            // 
            // lblData
            // 
            this.lblData.AutoSize = true;
            this.lblData.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblData.Location = new System.Drawing.Point(460, 13);
            this.lblData.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(34, 15);
            this.lblData.TabIndex = 4;
            this.lblData.Text = "Data:";
            // 
            // dtpData
            // 
            this.dtpData.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpData.Location = new System.Drawing.Point(502, 8);
            this.dtpData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dtpData.Name = "dtpData";
            this.dtpData.Size = new System.Drawing.Size(139, 23);
            this.dtpData.TabIndex = 5;
            // 
            // lblNumDoc
            // 
            this.lblNumDoc.AutoSize = true;
            this.lblNumDoc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblNumDoc.Location = new System.Drawing.Point(651, 13);
            this.lblNumDoc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNumDoc.Name = "lblNumDoc";
            this.lblNumDoc.Size = new System.Drawing.Size(48, 15);
            this.lblNumDoc.TabIndex = 6;
            this.lblNumDoc.Text = "Nº Doc:";
            // 
            // txtNumDoc
            // 
            this.txtNumDoc.Location = new System.Drawing.Point(710, 8);
            this.txtNumDoc.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtNumDoc.Name = "txtNumDoc";
            this.txtNumDoc.Size = new System.Drawing.Size(139, 23);
            this.txtNumDoc.TabIndex = 7;
            // 
            // lblNovaEntradaTitulo
            // 
            this.lblNovaEntradaTitulo.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblNovaEntradaTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblNovaEntradaTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblNovaEntradaTitulo.Name = "lblNovaEntradaTitulo";
            this.lblNovaEntradaTitulo.Size = new System.Drawing.Size(100, 23);
            this.lblNovaEntradaTitulo.TabIndex = 0;
            this.lblNovaEntradaTitulo.Text = "  Nova Entrada de Mercadorias";
            // 
            // dtpDe
            // 
            this.dtpDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDe.Location = new System.Drawing.Point(37, 9);
            this.dtpDe.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dtpDe.Name = "dtpDe";
            this.dtpDe.Size = new System.Drawing.Size(139, 23);
            this.dtpDe.TabIndex = 0;
            // 
            // dtpAte
            // 
            this.dtpAte.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAte.Location = new System.Drawing.Point(216, 9);
            this.dtpAte.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dtpAte.Name = "dtpAte";
            this.dtpAte.Size = new System.Drawing.Size(139, 23);
            this.dtpAte.TabIndex = 1;
            // 
            // btnFiltrar
            // 
            this.btnFiltrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnFiltrar.FlatAppearance.BorderSize = 0;
            this.btnFiltrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFiltrar.ForeColor = System.Drawing.Color.White;
            this.btnFiltrar.Location = new System.Drawing.Point(366, 10);
            this.btnFiltrar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnFiltrar.Name = "btnFiltrar";
            this.btnFiltrar.Size = new System.Drawing.Size(93, 30);
            this.btnFiltrar.TabIndex = 2;
            this.btnFiltrar.Text = "Filtrar";
            this.btnFiltrar.UseVisualStyleBackColor = false;
            // 
            // btnLimparFiltro
            // 
            this.btnLimparFiltro.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(113)))), ((int)(((byte)(42)))));
            this.btnLimparFiltro.FlatAppearance.BorderSize = 0;
            this.btnLimparFiltro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimparFiltro.ForeColor = System.Drawing.Color.White;
            this.btnLimparFiltro.Location = new System.Drawing.Point(468, 10);
            this.btnLimparFiltro.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLimparFiltro.Name = "btnLimparFiltro";
            this.btnLimparFiltro.Size = new System.Drawing.Size(93, 30);
            this.btnLimparFiltro.TabIndex = 3;
            this.btnLimparFiltro.Text = "Limpar";
            this.btnLimparFiltro.UseVisualStyleBackColor = false;
            // 
            // btnNovaEntrada
            // 
            this.btnNovaEntrada.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(120)))), ((int)(((byte)(38)))));
            this.btnNovaEntrada.FlatAppearance.BorderSize = 0;
            this.btnNovaEntrada.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNovaEntrada.ForeColor = System.Drawing.Color.White;
            this.btnNovaEntrada.Location = new System.Drawing.Point(14, 14);
            this.btnNovaEntrada.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnNovaEntrada.Name = "btnNovaEntrada";
            this.btnNovaEntrada.Size = new System.Drawing.Size(158, 35);
            this.btnNovaEntrada.TabIndex = 0;
            this.btnNovaEntrada.Text = "+Nova Entrada";
            this.btnNovaEntrada.UseVisualStyleBackColor = false;
            // 
            // btnCancelarSel
            // 
            this.btnCancelarSel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnCancelarSel.FlatAppearance.BorderSize = 0;
            this.btnCancelarSel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelarSel.ForeColor = System.Drawing.Color.White;
            this.btnCancelarSel.Location = new System.Drawing.Point(190, 14);
            this.btnCancelarSel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancelarSel.Name = "btnCancelarSel";
            this.btnCancelarSel.Size = new System.Drawing.Size(163, 35);
            this.btnCancelarSel.TabIndex = 1;
            this.btnCancelarSel.Text = "❌ Cancelar";
            this.btnCancelarSel.UseVisualStyleBackColor = false;
            // 
            // txtObservacoes
            // 
            this.txtObservacoes.Location = new System.Drawing.Point(47, 5);
            this.txtObservacoes.Name = "txtObservacoes";
            this.txtObservacoes.Size = new System.Drawing.Size(700, 23);
            this.txtObservacoes.TabIndex = 1;
            // 
            // lblObservacoes
            // 
            this.lblObservacoes.AutoSize = true;
            this.lblObservacoes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblObservacoes.Location = new System.Drawing.Point(4, 8);
            this.lblObservacoes.Name = "lblObservacoes";
            this.lblObservacoes.Size = new System.Drawing.Size(34, 15);
            this.lblObservacoes.TabIndex = 0;
            this.lblObservacoes.Text = "Obs.:";
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(110)))), ((int)(((byte)(42)))));
            this.pnlTop.Controls.Add(this.btnNovaEntrada);
            this.pnlTop.Controls.Add(this.btnCancelarSel);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1342, 62);
            this.pnlTop.TabIndex = 2;
            // 
            // pnlFiltro
            // 
            this.pnlFiltro.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(226)))), ((int)(((byte)(208)))));
            this.pnlFiltro.Controls.Add(this.dtpDe);
            this.pnlFiltro.Controls.Add(this.dtpAte);
            this.pnlFiltro.Controls.Add(this.btnFiltrar);
            this.pnlFiltro.Controls.Add(this.btnLimparFiltro);
            this.pnlFiltro.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltro.Location = new System.Drawing.Point(0, 62);
            this.pnlFiltro.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlFiltro.Name = "pnlFiltro";
            this.pnlFiltro.Size = new System.Drawing.Size(1342, 48);
            this.pnlFiltro.TabIndex = 1;
            // 
            // pnlObs
            // 
            this.pnlObs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.pnlObs.Controls.Add(this.lblObservacoes);
            this.pnlObs.Controls.Add(this.txtObservacoes);
            this.pnlObs.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlObs.Location = new System.Drawing.Point(0, 0);
            this.pnlObs.Name = "pnlObs";
            this.pnlObs.Size = new System.Drawing.Size(200, 32);
            this.pnlObs.TabIndex = 0;
            // 
            // frmEntradaMercadoria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(237)))), ((int)(((byte)(216)))));
            this.ClientSize = new System.Drawing.Size(1342, 785);
            this.Controls.Add(this.gridEntradas);
            this.Controls.Add(this.pnlFiltro);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlNovaEntrada);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(1164, 663);
            this.Name = "frmEntradaMercadoria";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Entrada de Mercadorias";
            ((System.ComponentModel.ISupportInitialize)(this.gridEntradas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridItens)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridParcelas)).EndInit();
            this.pnlNovaEntrada.ResumeLayout(false);
            this.pnlParcelasOuter.ResumeLayout(false);
            this.pnlParcelasTop.ResumeLayout(false);
            this.pnlParcelasTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numParcelas)).EndInit();
            this.pnlRodape.ResumeLayout(false);
            this.pnlRodape.PerformLayout();
            this.pnlAddItem.ResumeLayout(false);
            this.pnlAddItem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQtde)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFracEntrada)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFracao)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numValorTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCustoItem)).EndInit();
            this.pnlCabecalho.ResumeLayout(false);
            this.pnlCabecalho.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlFiltro.ResumeLayout(false);
            this.pnlObs.ResumeLayout(false);
            this.pnlObs.PerformLayout();
            this.ResumeLayout(false);

        }

        internal System.Windows.Forms.DataGridView   gridEntradas;
        internal System.Windows.Forms.DataGridView   gridItens;
        internal System.Windows.Forms.DataGridView   gridParcelas;
        internal System.Windows.Forms.Panel          pnlNovaEntrada;
        internal System.Windows.Forms.Label          lblNovaEntradaTitulo;
        internal System.Windows.Forms.Label          lblTotal;
        internal System.Windows.Forms.NumericUpDown  numParcelas;
        internal System.Windows.Forms.DateTimePicker dtpPrimVencimento;
        internal System.Windows.Forms.Button         btnGerarParcelas;
        internal System.Windows.Forms.DateTimePicker dtpDe;
        internal System.Windows.Forms.DateTimePicker dtpAte;
        internal System.Windows.Forms.Button         btnFiltrar;
        internal System.Windows.Forms.Button         btnLimparFiltro;
        internal System.Windows.Forms.Button         btnNovaEntrada;
        internal System.Windows.Forms.Button         btnCancelarSel;
        internal System.Windows.Forms.TextBox        txtFornCod;
        internal System.Windows.Forms.TextBox        txtFornNome;
        internal System.Windows.Forms.DateTimePicker dtpData;
        internal System.Windows.Forms.TextBox        txtNumDoc;
        internal System.Windows.Forms.TextBox        txtObservacoes;
        internal System.Windows.Forms.TextBox        txtProdCod;
        internal System.Windows.Forms.TextBox        txtProdNome;
        internal System.Windows.Forms.NumericUpDown  numQtde;
        internal System.Windows.Forms.TextBox        txtUnidEntrada;
        private  System.Windows.Forms.Label          lblIgual;
        private  System.Windows.Forms.Label          lblFracEntrada;
        internal System.Windows.Forms.NumericUpDown  numFracEntrada;
        internal System.Windows.Forms.NumericUpDown  numFracao;
        internal System.Windows.Forms.TextBox        txtUnidSaida;
        private  System.Windows.Forms.Label          lblUnidadesEntrada;
        private  System.Windows.Forms.Label          lblValor;
        internal System.Windows.Forms.NumericUpDown  numValorTotal;
        internal System.Windows.Forms.CheckBox       chkFracionado;
        internal System.Windows.Forms.NumericUpDown  numCustoItem;
        internal System.Windows.Forms.CheckBox       chkAtualizarCusto;
        internal System.Windows.Forms.Button         btnAdicionarItem;
        internal System.Windows.Forms.Button         btnBuscarForn;
        internal System.Windows.Forms.Button         btnBuscarProd;
        internal System.Windows.Forms.Button         btnRemoverItem;
        internal System.Windows.Forms.Button         btnConfirmarEntrada;
        internal System.Windows.Forms.Button         btnFecharForm;
        private  System.Windows.Forms.Label          lblFornecedor;
        private  System.Windows.Forms.Label          lblData;
        private  System.Windows.Forms.Label          lblNumDoc;
        private  System.Windows.Forms.Label          lblObservacoes;
        private  System.Windows.Forms.Label          lblProduto;
        private  System.Windows.Forms.Label          lblQtde;
        private  System.Windows.Forms.Label          lblCusto;
        private  System.Windows.Forms.Label          lblParcelas;
        private  System.Windows.Forms.Label          lblPrimVenc;
        private  System.Windows.Forms.Panel          pnlTop;
        private  System.Windows.Forms.Panel          pnlFiltro;
        private  System.Windows.Forms.Panel          pnlCabecalho;
        private  System.Windows.Forms.Panel          pnlObs;
        private  System.Windows.Forms.Panel          pnlAddItem;
        private  System.Windows.Forms.Panel          pnlParcelasOuter;
        private  System.Windows.Forms.Panel          pnlParcelasTop;
        private  System.Windows.Forms.Panel          pnlRodape;

        private void Grid_DataError(object s, System.Windows.Forms.DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }
    }
}
