using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroProduto
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.grid               = new System.Windows.Forms.DataGridView();
            this.topBar             = new System.Windows.Forms.Panel();
            this.btnNovo            = new System.Windows.Forms.Button();
            this.btnEditar          = new System.Windows.Forms.Button();
            this.btnCat             = new System.Windows.Forms.Button();
            this.pnlSearch          = new System.Windows.Forms.Panel();
            this._txtFiltro         = new System.Windows.Forms.TextBox();
            this.btnPesq            = new System.Windows.Forms.Button();
            this.pnlForm            = new System.Windows.Forms.Panel();
            this.pnlBtns            = new System.Windows.Forms.Panel();
            this.btnS               = new System.Windows.Forms.Button();
            this.btnC               = new System.Windows.Forms.Button();
            this.btnD               = new System.Windows.Forms.Button();
            this.cmbCategoria       = new System.Windows.Forms.ComboBox();
            this.txtNome            = new System.Windows.Forms.TextBox();
            this.cmbSituacao        = new System.Windows.Forms.ComboBox();
            this.btnImg             = new System.Windows.Forms.Button();
            this.lblImagem          = new System.Windows.Forms.Label();
            this.numPreco           = new System.Windows.Forms.NumericUpDown();
            this.numCusto           = new System.Windows.Forms.NumericUpDown();
            this.numPromo           = new System.Windows.Forms.NumericUpDown();
            this.numEstoque         = new System.Windows.Forms.NumericUpDown();
            this.chkControlaEstoque = new System.Windows.Forms.CheckBox();
            this.chkSite            = new System.Windows.Forms.CheckBox();
            this.chkDestaque        = new System.Windows.Forms.CheckBox();
            this.lblCat             = new System.Windows.Forms.Label();
            this.lblNom             = new System.Windows.Forms.Label();
            this.lblSit             = new System.Windows.Forms.Label();
            this.lblPrc             = new System.Windows.Forms.Label();
            this.lblCst             = new System.Windows.Forms.Label();
            this.lblPrm             = new System.Windows.Forms.Label();
            this.lblEst             = new System.Windows.Forms.Label();
            this.lblPub             = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // grid
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.ReadOnly = true; this.grid.AllowUserToAddRows = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.RowHeadersVisible = false;
            this.grid.BackgroundColor = System.Drawing.Color.FromArgb(248, 242, 235);
            this.grid.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 242, 235);
            this.grid.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(42, 20, 8);
            this.grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(200, 70, 20);
            this.grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(243, 237, 229);
            this.grid.GridColor = System.Drawing.Color.FromArgb(215, 198, 178);
            this.grid.Font = new System.Drawing.Font("Segoe UI", 9F); this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(45, 22, 10);
            this.grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grid.ColumnHeadersHeight = 34; this.grid.RowTemplate.Height = 28;
            this.grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_CellDoubleClick);
            this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // topBar
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top; this.topBar.Height = 44;
            this.topBar.BackColor = System.Drawing.Color.FromArgb(45, 22, 10);
            this.topBar.Controls.Add(this.btnNovo); this.topBar.Controls.Add(this.btnEditar); this.topBar.Controls.Add(this.btnCat);
            this.btnNovo.Text="+ Novo Produto"; this.btnNovo.Left=8; this.btnNovo.Top=8; this.btnNovo.Width=115; this.btnNovo.Height=28;
            this.btnNovo.BackColor=System.Drawing.Color.FromArgb(120, 80, 25); this.btnNovo.ForeColor=System.Drawing.Color.White;
            this.btnNovo.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnNovo.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnNovo.FlatAppearance.BorderSize=0; this.btnNovo.Click+=new System.EventHandler(this.BtnNovoProd_Click);
            this.btnEditar.Text="\u270F Editar"; this.btnEditar.Left=133; this.btnEditar.Top=8; this.btnEditar.Width=95; this.btnEditar.Height=28;
            this.btnEditar.BackColor=System.Drawing.Color.FromArgb(160, 100, 30); this.btnEditar.ForeColor=System.Drawing.Color.White;
            this.btnEditar.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnEditar.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnEditar.FlatAppearance.BorderSize=0; this.btnEditar.Click+=new System.EventHandler(this.BtnEditarProd_Click);
            this.btnCat.Text="Categorias"; this.btnCat.Left=238; this.btnCat.Top=8; this.btnCat.Width=100; this.btnCat.Height=28;
            this.btnCat.BackColor=System.Drawing.Color.FromArgb(200, 70, 20); this.btnCat.ForeColor=System.Drawing.Color.White;
            this.btnCat.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnCat.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnCat.FlatAppearance.BorderSize=0; this.btnCat.Click+=new System.EventHandler(this.BtnCatProd_Click);
            // pnlSearch
            this.pnlSearch.Dock=System.Windows.Forms.DockStyle.Top; this.pnlSearch.Height=38;
            this.pnlSearch.BackColor=System.Drawing.Color.FromArgb(65, 35, 15);
            this.pnlSearch.Controls.Add(this._txtFiltro); this.pnlSearch.Controls.Add(this.btnPesq);
            this._txtFiltro.Left=8; this._txtFiltro.Top=7; this._txtFiltro.Width=260; this._txtFiltro.Font=new System.Drawing.Font("Segoe UI",9.5F);
            this._txtFiltro.KeyDown+=new System.Windows.Forms.KeyEventHandler(this.TxtFiltro_KeyDown);
            this.btnPesq.Text="Pesquisar"; this.btnPesq.Left=276; this.btnPesq.Top=6; this.btnPesq.Width=90; this.btnPesq.Height=26;
            this.btnPesq.BackColor=System.Drawing.Color.FromArgb(200, 70, 20); this.btnPesq.ForeColor=System.Drawing.Color.White;
            this.btnPesq.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnPesq.Cursor=System.Windows.Forms.Cursors.Hand;
            this.btnPesq.FlatAppearance.BorderSize=0; this.btnPesq.Click+=new System.EventHandler(this.BtnPesqProd_Click);
            // pnlForm
            this.pnlForm.Dock=System.Windows.Forms.DockStyle.Bottom; this.pnlForm.Height=225;
            this.pnlForm.BackColor=System.Drawing.Color.FromArgb(252, 248, 244);
            this.pnlForm.BorderStyle=System.Windows.Forms.BorderStyle.FixedSingle; this.pnlForm.Visible=false;
            this.pnlForm.Controls.Add(this.lblCat);  this.pnlForm.Controls.Add(this.cmbCategoria);
            this.pnlForm.Controls.Add(this.lblNom);  this.pnlForm.Controls.Add(this.txtNome);
            this.pnlForm.Controls.Add(this.lblSit);  this.pnlForm.Controls.Add(this.cmbSituacao);
            this.pnlForm.Controls.Add(this.btnImg);  this.pnlForm.Controls.Add(this.lblImagem);
            this.pnlForm.Controls.Add(this.lblPrc);  this.pnlForm.Controls.Add(this.numPreco);
            this.pnlForm.Controls.Add(this.lblCst);  this.pnlForm.Controls.Add(this.numCusto);
            this.pnlForm.Controls.Add(this.lblPrm);  this.pnlForm.Controls.Add(this.numPromo);
            this.pnlForm.Controls.Add(this.lblEst);  this.pnlForm.Controls.Add(this.numEstoque);
            this.pnlForm.Controls.Add(this.chkControlaEstoque);
            this.pnlForm.Controls.Add(this.lblPub);  this.pnlForm.Controls.Add(this.chkSite); this.pnlForm.Controls.Add(this.chkDestaque);
            this.pnlForm.Controls.Add(this.pnlBtns);
            // row 1
            this.lblCat.Text="Categoria:"; this.lblCat.AutoSize=true; this.lblCat.Left=10; this.lblCat.Top=11;
            this.cmbCategoria.Left=80; this.cmbCategoria.Top=8; this.cmbCategoria.Width=160;
            this.cmbCategoria.DropDownStyle=System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbCategoria.AutoCompleteMode=System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbCategoria.AutoCompleteSource=System.Windows.Forms.AutoCompleteSource.ListItems;
            this.lblNom.Text="Nome do Produto:"; this.lblNom.AutoSize=true; this.lblNom.Left=252; this.lblNom.Top=11;
            this.txtNome.Left=365; this.txtNome.Top=8; this.txtNome.Width=270;
            this.lblSit.Text="Situa\u00e7\u00e3o:"; this.lblSit.AutoSize=true; this.lblSit.Left=647; this.lblSit.Top=11;
            this.cmbSituacao.Left=702; this.cmbSituacao.Top=8; this.cmbSituacao.Width=90;
            this.cmbSituacao.DropDownStyle=System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSituacao.Items.AddRange(new object[]{"Ativo","Inativo"}); this.cmbSituacao.SelectedIndex=0;
            // row 2
            this.btnImg.Text="Imagem"; this.btnImg.Left=10; this.btnImg.Top=41; this.btnImg.Width=85; this.btnImg.Height=24;
            this.btnImg.BackColor=System.Drawing.Color.FromArgb(160, 100, 30); this.btnImg.ForeColor=System.Drawing.Color.White;
            this.btnImg.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnImg.FlatAppearance.BorderSize=0;
            this.btnImg.Click+=new System.EventHandler(this.BtnImagem_Click);
            this.lblImagem.Text="nenhuma imagem selecionada"; this.lblImagem.Left=105; this.lblImagem.Top=45;
            this.lblImagem.Width=690; this.lblImagem.ForeColor=System.Drawing.Color.Gray; this.lblImagem.AutoSize=false;
            // row 3
            this.lblPrc.Text="Pre\u00e7o R$:"; this.lblPrc.AutoSize=true; this.lblPrc.Left=10; this.lblPrc.Top=79;
            this.numPreco.Left=73; this.numPreco.Top=76; this.numPreco.Width=85;
            this.numPreco.DecimalPlaces=2; this.numPreco.Maximum=999999;
            this.lblCst.Text="Custo R$:"; this.lblCst.AutoSize=true; this.lblCst.Left=166; this.lblCst.Top=79;
            this.numCusto.Left=228; this.numCusto.Top=76; this.numCusto.Width=85;
            this.numCusto.DecimalPlaces=2; this.numCusto.Maximum=999999;
            this.lblPrm.Text="Promo R$:"; this.lblPrm.AutoSize=true; this.lblPrm.Left=320; this.lblPrm.Top=79;
            this.numPromo.Left=382; this.numPromo.Top=76; this.numPromo.Width=85;
            this.numPromo.DecimalPlaces=2; this.numPromo.Maximum=999999;
            this.lblEst.Text="Estoque:"; this.lblEst.AutoSize=true; this.lblEst.Left=474; this.lblEst.Top=79;
            this.numEstoque.Left=530; this.numEstoque.Top=76; this.numEstoque.Width=75;
            this.numEstoque.DecimalPlaces=2; this.numEstoque.Minimum=-999999; this.numEstoque.Maximum=999999;
            this.chkControlaEstoque.Text="Controla estoque"; this.chkControlaEstoque.Left=615; this.chkControlaEstoque.Top=78; this.chkControlaEstoque.AutoSize=true;
            // row 4
            this.lblPub.Text="Publica\u00e7\u00f5es:"; this.lblPub.AutoSize=true; this.lblPub.Left=10; this.lblPub.Top=119; this.lblPub.ForeColor=System.Drawing.Color.FromArgb(180,190,220);
            this.chkSite.Text="No site"; this.chkSite.Left=90; this.chkSite.Top=116; this.chkSite.AutoSize=true;
            this.chkDestaque.Text="Destaque"; this.chkDestaque.Left=180; this.chkDestaque.Top=116; this.chkDestaque.AutoSize=true;
            // pnlBtns
            this.pnlBtns.Dock=System.Windows.Forms.DockStyle.Bottom; this.pnlBtns.Height=48;
            this.pnlBtns.BackColor=System.Drawing.Color.FromArgb(240, 233, 224);
            this.pnlBtns.Controls.Add(this.btnS); this.pnlBtns.Controls.Add(this.btnC); this.pnlBtns.Controls.Add(this.btnD);
            this.pnlBtns.SizeChanged+=new System.EventHandler(this.PnlBtns_SizeChanged);
            this.btnS.Text="Salvar"; this.btnS.Left=10; this.btnS.Top=10; this.btnS.Width=110; this.btnS.Height=28;
            this.btnS.BackColor=System.Drawing.Color.FromArgb(200, 70, 20); this.btnS.ForeColor=System.Drawing.Color.White;
            this.btnS.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnS.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnS.FlatAppearance.BorderSize=0; this.btnS.Click+=new System.EventHandler(this.BtnSalvar_Click);
            this.btnC.Text="Cancelar"; this.btnC.Left=130; this.btnC.Top=10; this.btnC.Width=110; this.btnC.Height=28;
            this.btnC.BackColor=System.Drawing.Color.FromArgb(150, 125, 100); this.btnC.ForeColor=System.Drawing.Color.White;
            this.btnC.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnC.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnC.FlatAppearance.BorderSize=0; this.btnC.Click+=new System.EventHandler(this.BtnCancelarProd_Click);
            this.btnD.Text="Desativar"; this.btnD.Left=250; this.btnD.Top=10; this.btnD.Width=110; this.btnD.Height=28;
            this.btnD.BackColor=System.Drawing.Color.FromArgb(178, 38, 20); this.btnD.ForeColor=System.Drawing.Color.White;
            this.btnD.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnD.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnD.FlatAppearance.BorderSize=0; this.btnD.Click+=new System.EventHandler(this.BtnDesativar_Click);
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 581);
            this.MinimumSize = new System.Drawing.Size(820, 540);
            this.BackColor = System.Drawing.Color.FromArgb(252, 248, 244);
            this.ForeColor = System.Drawing.Color.FromArgb(42, 20, 8);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Produtos";
            this.Controls.Add(this.grid);
            this.Controls.Add(this.pnlSearch);
            this.Controls.Add(this.topBar);
            this.Controls.Add(this.pnlForm);
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.DataGridView  grid;
        internal System.Windows.Forms.Panel         pnlForm;
        internal System.Windows.Forms.ComboBox      cmbCategoria;
        internal System.Windows.Forms.TextBox       txtNome;
        internal System.Windows.Forms.ComboBox      cmbSituacao;
        internal System.Windows.Forms.Label         lblImagem;
        internal System.Windows.Forms.NumericUpDown numPreco;
        internal System.Windows.Forms.NumericUpDown numCusto;
        internal System.Windows.Forms.NumericUpDown numPromo;
        internal System.Windows.Forms.NumericUpDown numEstoque;
        internal System.Windows.Forms.CheckBox      chkControlaEstoque;
        internal System.Windows.Forms.CheckBox      chkDestaque;
        internal System.Windows.Forms.CheckBox      chkSite;
        internal System.Windows.Forms.Button        btnNovo;
        internal System.Windows.Forms.Button        btnEditar;
        internal System.Windows.Forms.Button        btnCat;
        internal System.Windows.Forms.Button        btnPesq;
        internal System.Windows.Forms.Button        btnImg;
        internal System.Windows.Forms.Button        btnS;
        internal System.Windows.Forms.Button        btnC;
        internal System.Windows.Forms.Button        btnD;
        internal System.Windows.Forms.TextBox       _txtFiltro;
        private  System.Windows.Forms.Panel         topBar;
        private  System.Windows.Forms.Panel         pnlSearch;
        private  System.Windows.Forms.Panel         pnlBtns;
        private  System.Windows.Forms.Label         lblCat;
        private  System.Windows.Forms.Label         lblNom;
        private  System.Windows.Forms.Label         lblSit;
        private  System.Windows.Forms.Label         lblPrc;
        private  System.Windows.Forms.Label         lblCst;
        private  System.Windows.Forms.Label         lblPrm;
        private  System.Windows.Forms.Label         lblEst;
        private  System.Windows.Forms.Label         lblPub;

        private void Grid_CellDoubleClick(object s, System.Windows.Forms.DataGridViewCellEventArgs e) { CarregarParaEditar(); }
        private void Grid_DataError(object s, System.Windows.Forms.DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }
        private void BtnNovoProd_Click(object s, System.EventArgs e)  { ModoNovo(); }
        private void BtnEditarProd_Click(object s, System.EventArgs e) { CarregarParaEditar(); }
        private void BtnCatProd_Click(object s, System.EventArgs e)
        { new frmCadastroCategoria().ShowDialog(this); CarrecarComboCategorias(); }
        private void BtnPesqProd_Click(object s, System.EventArgs e)  { FiltrarGrid(_txtFiltro.Text); }
        private void TxtFiltro_KeyDown(object s, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == System.Windows.Forms.Keys.Enter) FiltrarGrid(_txtFiltro.Text); }
        private void BtnCancelarProd_Click(object s, System.EventArgs e) { pnlForm.Visible = false; _codigoEditando = 0; }
        private void PnlBtns_SizeChanged(object s, System.EventArgs e)
        {
            int x = (pnlBtns.Width - 110 * 3 - 20) / 2;
            if (x < 10) x = 10;
            btnS.Left = x; btnC.Left = x + 120; btnD.Left = x + 240;
        }
    }
}
