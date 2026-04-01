using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroFornecedor
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.grid        = new System.Windows.Forms.DataGridView();
            this.topBar      = new System.Windows.Forms.Panel();
            this.btnN        = new System.Windows.Forms.Button();
            this.btnR        = new System.Windows.Forms.Button();
            this.pnlForm     = new System.Windows.Forms.Panel();
            this.pnlBtns     = new System.Windows.Forms.Panel();
            this.btnS        = new System.Windows.Forms.Button();
            this.btnC        = new System.Windows.Forms.Button();
            this.txtRazao    = new System.Windows.Forms.TextBox();
            this.txtFantasia = new System.Windows.Forms.TextBox();
            this.txtCnpj     = new System.Windows.Forms.TextBox();
            this.txtIe       = new System.Windows.Forms.TextBox();
            this.txtTelefone = new System.Windows.Forms.TextBox();
            this.txtEmail    = new System.Windows.Forms.TextBox();
            this.txtContato  = new System.Windows.Forms.TextBox();
            this.txtCep      = new System.Windows.Forms.TextBox();
            this.txtEndereco = new System.Windows.Forms.TextBox();
            this.txtNumero   = new System.Windows.Forms.TextBox();
            this.txtBairro   = new System.Windows.Forms.TextBox();
            this.txtCidade   = new System.Windows.Forms.TextBox();
            this.txtEstado   = new System.Windows.Forms.TextBox();
            this.txtObs      = new System.Windows.Forms.TextBox();
            this.cmbSituacao = new System.Windows.Forms.ComboBox();
            this.lblRaz      = new System.Windows.Forms.Label();
            this.lblFan      = new System.Windows.Forms.Label();
            this.lblCnpj     = new System.Windows.Forms.Label();
            this.lblSit      = new System.Windows.Forms.Label();
            this.lblIe       = new System.Windows.Forms.Label();
            this.lblTel      = new System.Windows.Forms.Label();
            this.lblEmail    = new System.Windows.Forms.Label();
            this.lblCont     = new System.Windows.Forms.Label();
            this.lblCep      = new System.Windows.Forms.Label();
            this.lblEnd      = new System.Windows.Forms.Label();
            this.lblNum      = new System.Windows.Forms.Label();
            this.lblBai      = new System.Windows.Forms.Label();
            this.lblCid      = new System.Windows.Forms.Label();
            this.lblUF       = new System.Windows.Forms.Label();
            this.lblObs      = new System.Windows.Forms.Label();
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
            this.grid.DoubleClick += new System.EventHandler(this.Grid_DoubleClick);
            this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // topBar
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top; this.topBar.Height = 44;
            this.topBar.BackColor = System.Drawing.Color.FromArgb(45, 22, 10);
            this.topBar.Controls.Add(this.btnN); this.topBar.Controls.Add(this.btnR);
            // btnN
            this.btnN.Text = "+ Novo Fornecedor"; this.btnN.Left = 8; this.btnN.Top = 8; this.btnN.Width = 150; this.btnN.Height = 28;
            this.btnN.BackColor = System.Drawing.Color.FromArgb(120, 80, 25); this.btnN.ForeColor = System.Drawing.Color.White;
            this.btnN.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnN.FlatAppearance.BorderSize = 0; this.btnN.Click += new System.EventHandler(this.BtnNovoForn_Click);
            // btnR
            this.btnR.Text = "Atualizar"; this.btnR.Left = 168; this.btnR.Top = 8; this.btnR.Width = 100; this.btnR.Height = 28;
            this.btnR.BackColor = System.Drawing.Color.FromArgb(200, 70, 20); this.btnR.ForeColor = System.Drawing.Color.White;
            this.btnR.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnR.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnR.FlatAppearance.BorderSize = 0; this.btnR.Click += new System.EventHandler(this.BtnAtualizarForn_Click);
            // pnlForm
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Bottom; this.pnlForm.Height = 240;
            this.pnlForm.BackColor = System.Drawing.Color.FromArgb(55, 30, 12);
            this.pnlForm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlForm.Visible = false;
            this.pnlForm.Controls.Add(this.lblRaz);    this.pnlForm.Controls.Add(this.txtRazao);
            this.pnlForm.Controls.Add(this.lblFan);    this.pnlForm.Controls.Add(this.txtFantasia);
            this.pnlForm.Controls.Add(this.lblCnpj);   this.pnlForm.Controls.Add(this.txtCnpj);
            this.pnlForm.Controls.Add(this.lblSit);    this.pnlForm.Controls.Add(this.cmbSituacao);
            this.pnlForm.Controls.Add(this.lblIe);     this.pnlForm.Controls.Add(this.txtIe);
            this.pnlForm.Controls.Add(this.lblTel);    this.pnlForm.Controls.Add(this.txtTelefone);
            this.pnlForm.Controls.Add(this.lblEmail);  this.pnlForm.Controls.Add(this.txtEmail);
            this.pnlForm.Controls.Add(this.lblCont);   this.pnlForm.Controls.Add(this.txtContato);
            this.pnlForm.Controls.Add(this.lblCep);    this.pnlForm.Controls.Add(this.txtCep);
            this.pnlForm.Controls.Add(this.lblEnd);    this.pnlForm.Controls.Add(this.txtEndereco);
            this.pnlForm.Controls.Add(this.lblNum);    this.pnlForm.Controls.Add(this.txtNumero);
            this.pnlForm.Controls.Add(this.lblBai);    this.pnlForm.Controls.Add(this.txtBairro);
            this.pnlForm.Controls.Add(this.lblCid);    this.pnlForm.Controls.Add(this.txtCidade);
            this.pnlForm.Controls.Add(this.lblUF);     this.pnlForm.Controls.Add(this.txtEstado);
            this.pnlForm.Controls.Add(this.lblObs);    this.pnlForm.Controls.Add(this.txtObs);
            this.pnlForm.Controls.Add(this.pnlBtns);
            // row 1
            this.lblRaz.Text = "Raz\u00e3o Social:"; this.lblRaz.AutoSize=true; this.lblRaz.Left=10; this.lblRaz.Top=11;
            this.txtRazao.Left=90; this.txtRazao.Top=8; this.txtRazao.Width=230;
            this.lblFan.Text = "Nome Fantasia:"; this.lblFan.AutoSize=true; this.lblFan.Left=332; this.lblFan.Top=11;
            this.txtFantasia.Left=430; this.txtFantasia.Top=8; this.txtFantasia.Width=160;
            this.lblCnpj.Text = "CNPJ/CPF:"; this.lblCnpj.AutoSize=true; this.lblCnpj.Left=600; this.lblCnpj.Top=11;
            this.txtCnpj.Left=660; this.txtCnpj.Top=8; this.txtCnpj.Width=130;
            this.lblSit.Text = "Situa\u00e7\u00e3o:"; this.lblSit.AutoSize=true; this.lblSit.Left=800; this.lblSit.Top=11;
            this.cmbSituacao.Left=854; this.cmbSituacao.Top=8; this.cmbSituacao.Width=65;
            this.cmbSituacao.DropDownStyle=System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSituacao.Items.AddRange(new object[]{"A","I"}); this.cmbSituacao.SelectedIndex=0;
            // row 2
            this.lblIe.Text = "Insc. Estadual:"; this.lblIe.AutoSize=true; this.lblIe.Left=10; this.lblIe.Top=43;
            this.txtIe.Left=100; this.txtIe.Top=40; this.txtIe.Width=120;
            this.lblTel.Text = "Telefone:"; this.lblTel.AutoSize=true; this.lblTel.Left=232; this.lblTel.Top=43;
            this.txtTelefone.Left=290; this.txtTelefone.Top=40; this.txtTelefone.Width=120;
            this.lblEmail.Text = "E-mail:"; this.lblEmail.AutoSize=true; this.lblEmail.Left=422; this.lblEmail.Top=43;
            this.txtEmail.Left=467; this.txtEmail.Top=40; this.txtEmail.Width=200;
            this.lblCont.Text = "Contato:"; this.lblCont.AutoSize=true; this.lblCont.Left=678; this.lblCont.Top=43;
            this.txtContato.Left=730; this.txtContato.Top=40; this.txtContato.Width=150;
            // row 3
            this.lblCep.Text = "CEP:"; this.lblCep.AutoSize=true; this.lblCep.Left=10; this.lblCep.Top=75;
            this.txtCep.Left=42; this.txtCep.Top=72; this.txtCep.Width=85;
            this.lblEnd.Text = "Endere\u00e7o:"; this.lblEnd.AutoSize=true; this.lblEnd.Left=138; this.lblEnd.Top=75;
            this.txtEndereco.Left=200; this.txtEndereco.Top=72; this.txtEndereco.Width=220;
            this.lblNum.Text = "N\u00ba:"; this.lblNum.AutoSize=true; this.lblNum.Left=430; this.lblNum.Top=75;
            this.txtNumero.Left=448; this.txtNumero.Top=72; this.txtNumero.Width=60;
            this.lblBai.Text = "Bairro:"; this.lblBai.AutoSize=true; this.lblBai.Left=520; this.lblBai.Top=75;
            this.txtBairro.Left=565; this.txtBairro.Top=72; this.txtBairro.Width=150;
            // row 4
            this.lblCid.Text = "Cidade:"; this.lblCid.AutoSize=true; this.lblCid.Left=10; this.lblCid.Top=107;
            this.txtCidade.Left=58; this.txtCidade.Top=104; this.txtCidade.Width=200;
            this.lblUF.Text = "UF:"; this.lblUF.AutoSize=true; this.lblUF.Left=268; this.lblUF.Top=107;
            this.txtEstado.Left=285; this.txtEstado.Top=104; this.txtEstado.Width=42;
            // row 5
            this.lblObs.Text = "Observa\u00e7\u00f5es:"; this.lblObs.AutoSize=true; this.lblObs.Left=10; this.lblObs.Top=139;
            this.txtObs.Left=92; this.txtObs.Top=136; this.txtObs.Width=450;
            // pnlBtns
            this.pnlBtns.Dock = System.Windows.Forms.DockStyle.Bottom; this.pnlBtns.Height = 48;
            this.pnlBtns.BackColor = System.Drawing.Color.FromArgb(55, 30, 12);
            this.pnlBtns.Controls.Add(this.btnS); this.pnlBtns.Controls.Add(this.btnC);
            this.pnlBtns.SizeChanged += new System.EventHandler(this.PnlBtns_SizeChanged);
            // btnS
            this.btnS.Text="Salvar"; this.btnS.Left=10; this.btnS.Top=10; this.btnS.Width=110; this.btnS.Height=28;
            this.btnS.BackColor=System.Drawing.Color.FromArgb(200, 70, 20); this.btnS.ForeColor=System.Drawing.Color.White;
            this.btnS.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnS.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnS.FlatAppearance.BorderSize=0; this.btnS.Click+=new System.EventHandler(this.BtnSalvar_Click);
            // btnC
            this.btnC.Text="Cancelar"; this.btnC.Left=130; this.btnC.Top=10; this.btnC.Width=110; this.btnC.Height=28;
            this.btnC.BackColor=System.Drawing.Color.FromArgb(150, 125, 100); this.btnC.ForeColor=System.Drawing.Color.White;
            this.btnC.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnC.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnC.FlatAppearance.BorderSize=0; this.btnC.Click+=new System.EventHandler(this.BtnCancelarForn_Click);
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 581);
            this.MinimumSize = new System.Drawing.Size(860, 540);
            this.BackColor = System.Drawing.Color.FromArgb(252, 248, 244);
            this.ForeColor = System.Drawing.Color.FromArgb(42, 20, 8);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Fornecedores";
            this.Controls.Add(this.grid);
            this.Controls.Add(this.topBar);
            this.Controls.Add(this.pnlForm);
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.DataGridView grid;
        internal System.Windows.Forms.Panel        pnlForm;
        internal System.Windows.Forms.TextBox      txtRazao;
        internal System.Windows.Forms.TextBox      txtFantasia;
        internal System.Windows.Forms.TextBox      txtCnpj;
        internal System.Windows.Forms.TextBox      txtIe;
        internal System.Windows.Forms.TextBox      txtTelefone;
        internal System.Windows.Forms.TextBox      txtEmail;
        internal System.Windows.Forms.TextBox      txtContato;
        internal System.Windows.Forms.TextBox      txtCep;
        internal System.Windows.Forms.TextBox      txtEndereco;
        internal System.Windows.Forms.TextBox      txtNumero;
        internal System.Windows.Forms.TextBox      txtBairro;
        internal System.Windows.Forms.TextBox      txtCidade;
        internal System.Windows.Forms.TextBox      txtEstado;
        internal System.Windows.Forms.TextBox      txtObs;
        internal System.Windows.Forms.ComboBox     cmbSituacao;
        internal System.Windows.Forms.Button       btnN;
        internal System.Windows.Forms.Button       btnR;
        internal System.Windows.Forms.Button       btnS;
        internal System.Windows.Forms.Button       btnC;
        private  System.Windows.Forms.Panel        topBar;
        private  System.Windows.Forms.Panel        pnlBtns;
        private  System.Windows.Forms.Label        lblRaz;
        private  System.Windows.Forms.Label        lblFan;
        private  System.Windows.Forms.Label        lblCnpj;
        private  System.Windows.Forms.Label        lblSit;
        private  System.Windows.Forms.Label        lblIe;
        private  System.Windows.Forms.Label        lblTel;
        private  System.Windows.Forms.Label        lblEmail;
        private  System.Windows.Forms.Label        lblCont;
        private  System.Windows.Forms.Label        lblCep;
        private  System.Windows.Forms.Label        lblEnd;
        private  System.Windows.Forms.Label        lblNum;
        private  System.Windows.Forms.Label        lblBai;
        private  System.Windows.Forms.Label        lblCid;
        private  System.Windows.Forms.Label        lblUF;
        private  System.Windows.Forms.Label        lblObs;

        private void Grid_DoubleClick(object s, System.EventArgs e) { CarregarParaEditar(); }
        private void Grid_DataError(object s, System.Windows.Forms.DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }
        private void BtnNovoForn_Click(object s, System.EventArgs e)      { ModoNovo(); }
        private void BtnAtualizarForn_Click(object s, System.EventArgs e)  { CarregarGrid(); }
        private void BtnCancelarForn_Click(object s, System.EventArgs e)   { pnlForm.Visible = false; _codigoEditando = 0; }
        private void PnlBtns_SizeChanged(object s, System.EventArgs e)
        {
            int x = (pnlBtns.Width - 110 * 2 - 10) / 2;
            if (x < 10) x = 10;
            btnS.Left = x; btnC.Left = x + 120;
        }
    }
}
