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
            this.gridEntradas      = new System.Windows.Forms.DataGridView();
            this.gridItens         = new System.Windows.Forms.DataGridView();
            this.gridParcelas      = new System.Windows.Forms.DataGridView();
            this.pnlNovaEntrada    = new System.Windows.Forms.Panel();
            this.lblNovaEntradaTitulo = new System.Windows.Forms.Label();
            this.lblTotal          = new System.Windows.Forms.Label();
            this.numParcelas       = new System.Windows.Forms.NumericUpDown();
            this.dtpPrimVencimento = new System.Windows.Forms.DateTimePicker();
            this.btnGerarParcelas  = new System.Windows.Forms.Button();
            this.dtpDe             = new System.Windows.Forms.DateTimePicker();
            this.dtpAte            = new System.Windows.Forms.DateTimePicker();
            this.btnFiltrar        = new System.Windows.Forms.Button();
            this.btnLimparFiltro   = new System.Windows.Forms.Button();
            this.btnNovaEntrada    = new System.Windows.Forms.Button();
            this.btnCancelarSel    = new System.Windows.Forms.Button();
            this.txtFornCod        = new System.Windows.Forms.TextBox();
            this.txtFornNome       = new System.Windows.Forms.TextBox();
            this.dtpData           = new System.Windows.Forms.DateTimePicker();
            this.txtNumDoc         = new System.Windows.Forms.TextBox();
            this.txtObservacoes    = new System.Windows.Forms.TextBox();
            this.txtProdCod        = new System.Windows.Forms.TextBox();
            this.txtProdNome       = new System.Windows.Forms.TextBox();
            this.numQtde           = new System.Windows.Forms.NumericUpDown();
            this.numCustoItem      = new System.Windows.Forms.NumericUpDown();
            this.chkAtualizarCusto = new System.Windows.Forms.CheckBox();
            this.btnAdicionarItem  = new System.Windows.Forms.Button();
            this.btnRemoverItem    = new System.Windows.Forms.Button();
            this.btnConfirmarEntrada = new System.Windows.Forms.Button();
            this.btnFecharForm     = new System.Windows.Forms.Button();
            this.lblFornecedor     = new System.Windows.Forms.Label();
            this.lblData           = new System.Windows.Forms.Label();
            this.lblNumDoc         = new System.Windows.Forms.Label();
            this.lblObservacoes    = new System.Windows.Forms.Label();
            this.lblProduto        = new System.Windows.Forms.Label();
            this.lblQtde           = new System.Windows.Forms.Label();
            this.lblCusto          = new System.Windows.Forms.Label();
            this.lblParcelas       = new System.Windows.Forms.Label();
            this.lblPrimVenc       = new System.Windows.Forms.Label();
            this.pnlTop            = new System.Windows.Forms.Panel();
            this.pnlFiltro         = new System.Windows.Forms.Panel();
            this.pnlCabecalho      = new System.Windows.Forms.Panel();
            this.pnlObs            = new System.Windows.Forms.Panel();
            this.pnlAddItem        = new System.Windows.Forms.Panel();
            this.btnBuscarForn     = new System.Windows.Forms.Button();
            this.btnBuscarProd     = new System.Windows.Forms.Button();
            this.pnlParcelasOuter  = new System.Windows.Forms.Panel();
            this.pnlParcelasTop    = new System.Windows.Forms.Panel();
            this.pnlRodape         = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // gridEntradas
            this.gridEntradas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridEntradas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridEntradas.ReadOnly = true; this.gridEntradas.AllowUserToAddRows = false;
            this.gridEntradas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridEntradas.RowHeadersVisible = false;
            this.gridEntradas.BackgroundColor = System.Drawing.Color.White;
            this.gridEntradas.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.gridEntradas.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.gridEntradas.GridColor = System.Drawing.Color.FromArgb(200,185,160);
            this.gridEntradas.Font = new System.Drawing.Font("Segoe UI",9F); this.gridEntradas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridEntradas.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(176,110,42);
            this.gridEntradas.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.gridEntradas.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // pnlTop
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top; this.pnlTop.Height = 48;
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(176,110,42);
            this.btnNovaEntrada.Text="+Nova Entrada"; this.btnNovaEntrada.Left=8; this.btnNovaEntrada.Top=10; this.btnNovaEntrada.Width=130; this.btnNovaEntrada.Height=28;
            this.btnNovaEntrada.BackColor=System.Drawing.Color.FromArgb(87,120,38); this.btnNovaEntrada.ForeColor=System.Drawing.Color.White;
            this.btnNovaEntrada.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnNovaEntrada.FlatAppearance.BorderSize=0;
            this.btnCancelarSel.Text="\u274C Cancelar"; this.btnCancelarSel.Left=148; this.btnCancelarSel.Top=10; this.btnCancelarSel.Width=140; this.btnCancelarSel.Height=28;
            this.btnCancelarSel.BackColor=System.Drawing.Color.FromArgb(192,57,43); this.btnCancelarSel.ForeColor=System.Drawing.Color.White;
            this.btnCancelarSel.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnCancelarSel.FlatAppearance.BorderSize=0;
            this.pnlTop.Controls.Add(this.btnNovaEntrada); this.pnlTop.Controls.Add(this.btnCancelarSel);
            // pnlFiltro
            this.pnlFiltro.Dock=System.Windows.Forms.DockStyle.Top; this.pnlFiltro.Height=42;
            this.pnlFiltro.BackColor=System.Drawing.Color.FromArgb(235,226,208);
            this.dtpDe.Left=32; this.dtpDe.Top=8; this.dtpDe.Width=120; this.dtpDe.Format=System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAte.Left=185; this.dtpAte.Top=8; this.dtpAte.Width=120; this.dtpAte.Format=System.Windows.Forms.DateTimePickerFormat.Short;
            this.btnFiltrar.Text="Filtrar"; this.btnFiltrar.Left=314; this.btnFiltrar.Top=9; this.btnFiltrar.Width=80; this.btnFiltrar.Height=26;
            this.btnFiltrar.BackColor=System.Drawing.Color.FromArgb(224,113,42);  this.btnFiltrar.ForeColor=System.Drawing.Color.White;
            this.btnFiltrar.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnFiltrar.FlatAppearance.BorderSize=0;
            this.btnLimparFiltro.Text="Limpar"; this.btnLimparFiltro.Left=401; this.btnLimparFiltro.Top=9; this.btnLimparFiltro.Width=80; this.btnLimparFiltro.Height=26;
            this.btnLimparFiltro.BackColor=System.Drawing.Color.FromArgb(224,113,42); this.btnLimparFiltro.ForeColor=System.Drawing.Color.White;
            this.btnLimparFiltro.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnLimparFiltro.FlatAppearance.BorderSize=0;
            this.pnlFiltro.Controls.Add(this.dtpDe); this.pnlFiltro.Controls.Add(this.dtpAte);
            this.pnlFiltro.Controls.Add(this.btnFiltrar); this.pnlFiltro.Controls.Add(this.btnLimparFiltro);
            // pnlNovaEntrada
            this.pnlNovaEntrada.Dock=System.Windows.Forms.DockStyle.Bottom; this.pnlNovaEntrada.Height=470;
            this.pnlNovaEntrada.BackColor=System.Drawing.Color.FromArgb(245,237,216); this.pnlNovaEntrada.Visible=false;
            // pnlCabecalho
            this.pnlCabecalho.Dock=System.Windows.Forms.DockStyle.Top; this.pnlCabecalho.Height=36;
            this.pnlCabecalho.BackColor=System.Drawing.Color.FromArgb(245,237,216);
            this.lblFornecedor.Text="Fornecedor:"; this.lblFornecedor.Left=4; this.lblFornecedor.Top=11; this.lblFornecedor.AutoSize=true;
            this.lblFornecedor.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.txtFornCod.Left=87; this.txtFornCod.Top=7; this.txtFornCod.Width=55;
            this.txtFornNome.Left=148; this.txtFornNome.Top=7; this.txtFornNome.Width=200; this.txtFornNome.ReadOnly=true;
            this.btnBuscarForn.Text="🔍"; this.btnBuscarForn.Left=354; this.btnBuscarForn.Top=7; this.btnBuscarForn.Width=28; this.btnBuscarForn.Height=22;
            this.btnBuscarForn.BackColor=System.Drawing.Color.FromArgb(224,113,42); this.btnBuscarForn.ForeColor=System.Drawing.Color.White;
            this.btnBuscarForn.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnBuscarForn.FlatAppearance.BorderSize=0;
            this.btnBuscarForn.Font=new System.Drawing.Font("Segoe UI",8F);
            this.lblData.Text="Data:"; this.lblData.Left=394; this.lblData.Top=11; this.lblData.AutoSize=true;
            this.lblData.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.dtpData.Left=430; this.dtpData.Top=7; this.dtpData.Width=120; this.dtpData.Format=System.Windows.Forms.DateTimePickerFormat.Short;
            this.lblNumDoc.Text="N\u00ba Doc:"; this.lblNumDoc.Left=558; this.lblNumDoc.Top=11; this.lblNumDoc.AutoSize=true;
            this.lblNumDoc.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.txtNumDoc.Left=609; this.txtNumDoc.Top=7; this.txtNumDoc.Width=120;
            this.pnlCabecalho.Controls.Add(this.lblFornecedor); this.pnlCabecalho.Controls.Add(this.txtFornCod); this.pnlCabecalho.Controls.Add(this.txtFornNome);
            this.pnlCabecalho.Controls.Add(this.btnBuscarForn);
            this.pnlCabecalho.Controls.Add(this.lblData);       this.pnlCabecalho.Controls.Add(this.dtpData);
            this.pnlCabecalho.Controls.Add(this.lblNumDoc);     this.pnlCabecalho.Controls.Add(this.txtNumDoc);
            // pnlObs
            this.pnlObs.Dock=System.Windows.Forms.DockStyle.Top; this.pnlObs.Height=32;
            this.pnlObs.BackColor=System.Drawing.Color.FromArgb(245,237,216);
            this.lblObservacoes.Text="Obs.:"; this.lblObservacoes.Left=4; this.lblObservacoes.Top=8; this.lblObservacoes.AutoSize=true;
            this.lblObservacoes.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.txtObservacoes.Left=47; this.txtObservacoes.Top=5; this.txtObservacoes.Width=700; this.txtObservacoes.Height=22;
            this.pnlObs.Controls.Add(this.lblObservacoes); this.pnlObs.Controls.Add(this.txtObservacoes);
            // pnlAddItem
            this.pnlAddItem.Dock=System.Windows.Forms.DockStyle.Top; this.pnlAddItem.Height=36;
            this.pnlAddItem.BackColor=System.Drawing.Color.FromArgb(235,226,208);
            this.lblProduto.Text="Produto:"; this.lblProduto.Left=4; this.lblProduto.Top=11; this.lblProduto.AutoSize=true;
            this.lblProduto.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.txtProdCod.Left=62; this.txtProdCod.Top=7; this.txtProdCod.Width=55;
            this.txtProdNome.Left=123; this.txtProdNome.Top=7; this.txtProdNome.Width=200;
            this.btnBuscarProd.Text="\uD83D\uDD0D"; this.btnBuscarProd.Left=329; this.btnBuscarProd.Top=7; this.btnBuscarProd.Width=28; this.btnBuscarProd.Height=22;
            this.btnBuscarProd.BackColor=System.Drawing.Color.FromArgb(224,113,42); this.btnBuscarProd.ForeColor=System.Drawing.Color.White;
            this.btnBuscarProd.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnBuscarProd.FlatAppearance.BorderSize=0;
            this.btnBuscarProd.Font=new System.Drawing.Font("Segoe UI",8F);
            this.lblQtde.Text="Qtde:"; this.lblQtde.Left=364; this.lblQtde.Top=11; this.lblQtde.AutoSize=true;
            this.lblQtde.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.numQtde.Left=400; this.numQtde.Top=7; this.numQtde.Width=75;
            this.numQtde.DecimalPlaces=2; this.numQtde.Minimum=0.01m; this.numQtde.Maximum=99999; this.numQtde.Value=1;
            this.lblCusto.Text="Custo R$:"; this.lblCusto.Left=482; this.lblCusto.Top=11; this.lblCusto.AutoSize=true;
            this.lblCusto.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.numCustoItem.Left=545; this.numCustoItem.Top=7; this.numCustoItem.Width=90;
            this.numCustoItem.DecimalPlaces=2; this.numCustoItem.Maximum=99999;
            this.chkAtualizarCusto.Text="Atualizar custo"; this.chkAtualizarCusto.Left=642; this.chkAtualizarCusto.Top=9; this.chkAtualizarCusto.AutoSize=true;
            this.chkAtualizarCusto.ForeColor=System.Drawing.Color.FromArgb(50,50,50); this.chkAtualizarCusto.Checked=true;
            this.btnAdicionarItem.Text="+ Adicionar"; this.btnAdicionarItem.Left=756; this.btnAdicionarItem.Top=7; this.btnAdicionarItem.Width=95; this.btnAdicionarItem.Height=26;
            this.btnAdicionarItem.BackColor=System.Drawing.Color.FromArgb(87,120,38); this.btnAdicionarItem.ForeColor=System.Drawing.Color.White;
            this.btnAdicionarItem.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnAdicionarItem.FlatAppearance.BorderSize=0;
            this.pnlAddItem.Controls.Add(this.lblProduto); this.pnlAddItem.Controls.Add(this.txtProdCod); this.pnlAddItem.Controls.Add(this.txtProdNome);
            this.pnlAddItem.Controls.Add(this.btnBuscarProd);
            this.pnlAddItem.Controls.Add(this.lblQtde);    this.pnlAddItem.Controls.Add(this.numQtde);
            this.pnlAddItem.Controls.Add(this.lblCusto);   this.pnlAddItem.Controls.Add(this.numCustoItem);
            this.pnlAddItem.Controls.Add(this.chkAtualizarCusto); this.pnlAddItem.Controls.Add(this.btnAdicionarItem);
            // gridItens (fill)
            this.gridItens.Dock=System.Windows.Forms.DockStyle.Fill;
            this.gridItens.AutoSizeColumnsMode=System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridItens.ReadOnly=true; this.gridItens.AllowUserToAddRows=false;
            this.gridItens.SelectionMode=System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridItens.RowHeadersVisible=false;
            this.gridItens.BackgroundColor=System.Drawing.Color.White;
            this.gridItens.DefaultCellStyle.BackColor=System.Drawing.Color.White;
            this.gridItens.DefaultCellStyle.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.gridItens.GridColor=System.Drawing.Color.FromArgb(200,185,160);
            this.gridItens.Font=new System.Drawing.Font("Segoe UI",9F); this.gridItens.BorderStyle=System.Windows.Forms.BorderStyle.None;
            this.gridItens.ColumnHeadersDefaultCellStyle.BackColor=System.Drawing.Color.FromArgb(176,110,42);
            this.gridItens.ColumnHeadersDefaultCellStyle.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.gridItens.DataError+=new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // pnlParcelasOuter
            this.pnlParcelasOuter.Dock=System.Windows.Forms.DockStyle.Bottom; this.pnlParcelasOuter.Height=100;
            this.pnlParcelasOuter.BackColor=System.Drawing.Color.FromArgb(245,237,216);
            // pnlParcelasTop
            this.pnlParcelasTop.Dock=System.Windows.Forms.DockStyle.Top; this.pnlParcelasTop.Height=34;
            this.pnlParcelasTop.BackColor=System.Drawing.Color.FromArgb(235,226,208);
            this.lblParcelas.Text="Parcelas:"; this.lblParcelas.Left=346; this.lblParcelas.Top=9; this.lblParcelas.AutoSize=true;
            this.lblParcelas.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.numParcelas.Left=408; this.numParcelas.Top=5; this.numParcelas.Width=55;
            this.numParcelas.Minimum=0; this.numParcelas.Maximum=36;
            this.lblPrimVenc.Text="1\u00aa Data:"; this.lblPrimVenc.Left=470; this.lblPrimVenc.Top=9; this.lblPrimVenc.AutoSize=true;
            this.lblPrimVenc.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.dtpPrimVencimento.Left=529; this.dtpPrimVencimento.Top=5; this.dtpPrimVencimento.Width=115;
            this.dtpPrimVencimento.Format=System.Windows.Forms.DateTimePickerFormat.Short;
            this.btnGerarParcelas.Text="\u21BB Gerar"; this.btnGerarParcelas.Left=652; this.btnGerarParcelas.Top=5; this.btnGerarParcelas.Width=80; this.btnGerarParcelas.Height=24;
            this.btnGerarParcelas.BackColor=System.Drawing.Color.FromArgb(224,113,42); this.btnGerarParcelas.ForeColor=System.Drawing.Color.White;
            this.btnGerarParcelas.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnGerarParcelas.FlatAppearance.BorderSize=0;
            this.pnlParcelasTop.Controls.Add(this.lblParcelas); this.pnlParcelasTop.Controls.Add(this.numParcelas); this.pnlParcelasTop.Controls.Add(this.lblPrimVenc); this.pnlParcelasTop.Controls.Add(this.dtpPrimVencimento); this.pnlParcelasTop.Controls.Add(this.btnGerarParcelas);
            // gridParcelas
            this.gridParcelas.Dock=System.Windows.Forms.DockStyle.Fill;
            this.gridParcelas.AutoSizeColumnsMode=System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridParcelas.AllowUserToAddRows=false;
            this.gridParcelas.SelectionMode=System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridParcelas.RowHeadersVisible=false;
            this.gridParcelas.BackgroundColor=System.Drawing.Color.White;
            this.gridParcelas.DefaultCellStyle.BackColor=System.Drawing.Color.White;
            this.gridParcelas.DefaultCellStyle.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.gridParcelas.GridColor=System.Drawing.Color.FromArgb(200,185,160);
            this.gridParcelas.Font=new System.Drawing.Font("Segoe UI",9F); this.gridParcelas.BorderStyle=System.Windows.Forms.BorderStyle.None;
            this.gridParcelas.ColumnHeadersDefaultCellStyle.BackColor=System.Drawing.Color.FromArgb(176,110,42);
            this.gridParcelas.ColumnHeadersDefaultCellStyle.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.gridParcelas.DataError+=new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            this.pnlParcelasOuter.Controls.Add(this.gridParcelas);
            this.pnlParcelasOuter.Controls.Add(this.pnlParcelasTop);
            // pnlRodape
            this.pnlRodape.Dock=System.Windows.Forms.DockStyle.Bottom; this.pnlRodape.Height=46;
            this.pnlRodape.BackColor=System.Drawing.Color.FromArgb(245,237,216);
            this.lblTotal.Text="Total: R$ 0,00"; this.lblTotal.Left=8; this.lblTotal.Top=12;
            this.lblTotal.Font=new System.Drawing.Font("Segoe UI",11F,System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor=System.Drawing.Color.FromArgb(87,120,38); this.lblTotal.AutoSize=true;
            this.lblNovaEntradaTitulo.Text="  Nova Entrada de Mercadorias";
            this.lblNovaEntradaTitulo.Font=new System.Drawing.Font("Segoe UI",9.5F,System.Drawing.FontStyle.Bold);
            this.lblNovaEntradaTitulo.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.btnRemoverItem.Text="\u2212 Remover Item"; this.btnRemoverItem.Left=300; this.btnRemoverItem.Top=10; this.btnRemoverItem.Width=120; this.btnRemoverItem.Height=28;
            this.btnRemoverItem.BackColor=System.Drawing.Color.FromArgb(192,57,43); this.btnRemoverItem.ForeColor=System.Drawing.Color.White;
            this.btnRemoverItem.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnRemoverItem.FlatAppearance.BorderSize=0;
            this.btnConfirmarEntrada.Text="\u2714 Confirmar"; this.btnConfirmarEntrada.Left=430; this.btnConfirmarEntrada.Top=10; this.btnConfirmarEntrada.Width=160; this.btnConfirmarEntrada.Height=28;
            this.btnConfirmarEntrada.BackColor=System.Drawing.Color.FromArgb(87,120,38); this.btnConfirmarEntrada.ForeColor=System.Drawing.Color.White;
            this.btnConfirmarEntrada.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnConfirmarEntrada.FlatAppearance.BorderSize=0;
            this.btnFecharForm.Text="Cancelar"; this.btnFecharForm.Left=598; this.btnFecharForm.Top=10; this.btnFecharForm.Width=100; this.btnFecharForm.Height=28;
            this.btnFecharForm.BackColor=System.Drawing.Color.FromArgb(224,113,42); this.btnFecharForm.ForeColor=System.Drawing.Color.White;
            this.btnFecharForm.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnFecharForm.FlatAppearance.BorderSize=0;
            this.pnlRodape.Controls.Add(this.lblTotal); this.pnlRodape.Controls.Add(this.btnRemoverItem);
            this.pnlRodape.Controls.Add(this.btnConfirmarEntrada); this.pnlRodape.Controls.Add(this.btnFecharForm);
            // assemble pnlNovaEntrada
            this.pnlNovaEntrada.Controls.Add(this.gridItens);
            this.pnlNovaEntrada.Controls.Add(this.pnlParcelasOuter);
            this.pnlNovaEntrada.Controls.Add(this.pnlRodape);
            this.pnlNovaEntrada.Controls.Add(this.pnlAddItem);
            this.pnlNovaEntrada.Controls.Add(this.pnlCabecalho);
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 680);
            this.MinimumSize = new System.Drawing.Size(900, 580);
            this.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Entrada de Mercadorias";
            this.Controls.Add(this.gridEntradas);
            this.Controls.Add(this.pnlFiltro);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlNovaEntrada);
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
