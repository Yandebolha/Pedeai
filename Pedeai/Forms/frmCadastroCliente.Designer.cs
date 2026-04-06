using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmCadastroCliente
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
            this.txtBusca      = new System.Windows.Forms.TextBox();
            this.btnB          = new System.Windows.Forms.Button();
            this.btnN          = new System.Windows.Forms.Button();
            this.pnlForm       = new System.Windows.Forms.Panel();
            this.pnlBtns       = new System.Windows.Forms.Panel();
            this.btnS          = new System.Windows.Forms.Button();
            this.btnC          = new System.Windows.Forms.Button();
            this.txtNome       = new System.Windows.Forms.TextBox();
            this.txtEmail      = new System.Windows.Forms.TextBox();
            this.txtCpf        = new System.Windows.Forms.TextBox();
            this.txtEndereco   = new System.Windows.Forms.TextBox();
            this.txtNumero     = new System.Windows.Forms.TextBox();
            this.txtComplemento= new System.Windows.Forms.TextBox();
            this.txtBairro     = new System.Windows.Forms.TextBox();
            this.txtCidade     = new System.Windows.Forms.TextBox();
            this.txtEstado     = new System.Windows.Forms.TextBox();
            this.txtCelular    = new System.Windows.Forms.MaskedTextBox();
            this.txtCep        = new System.Windows.Forms.MaskedTextBox();
            this.cmbSituacao   = new System.Windows.Forms.ComboBox();
            this.lblB          = new System.Windows.Forms.Label();
            this.lblNome       = new System.Windows.Forms.Label();
            this.lblCpf        = new System.Windows.Forms.Label();
            this.lblSit        = new System.Windows.Forms.Label();
            this.lblCel        = new System.Windows.Forms.Label();
            this.lblEmail      = new System.Windows.Forms.Label();
            this.lblCep        = new System.Windows.Forms.Label();
            this.lblEnd        = new System.Windows.Forms.Label();
            this.lblNum        = new System.Windows.Forms.Label();
            this.lblComp       = new System.Windows.Forms.Label();
            this.lblBai        = new System.Windows.Forms.Label();
            this.lblCid        = new System.Windows.Forms.Label();
            this.lblUF         = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // grid
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.ReadOnly = true; this.grid.AllowUserToAddRows = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.RowHeadersVisible = false;
            this.grid.BackgroundColor = System.Drawing.Color.FromArgb(250, 245, 238);
            this.grid.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.grid.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.grid.GridColor = System.Drawing.Color.FromArgb(200, 185, 160);
            this.grid.Font = new System.Drawing.Font("Segoe UI", 9F); this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 230, 202);
            this.grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grid.ColumnHeadersHeight = 34; this.grid.RowTemplate.Height = 28;
            this.grid.DoubleClick += new System.EventHandler(this.Grid_DoubleClick);
            this.grid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            // topBar
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top; this.topBar.Height = 44;
            this.topBar.BackColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.topBar.Controls.Add(this.lblB); this.topBar.Controls.Add(this.txtBusca);
            this.topBar.Controls.Add(this.btnB); this.topBar.Controls.Add(this.btnN);
            this.lblB.Text="Buscar:"; this.lblB.ForeColor=System.Drawing.Color.White; this.lblB.Left=8; this.lblB.Top=12; this.lblB.AutoSize=true;
            this.txtBusca.Left=65; this.txtBusca.Top=8; this.txtBusca.Width=220;
            this.txtBusca.BackColor=System.Drawing.Color.FromArgb(245,237,216); this.txtBusca.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.txtBusca.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtBusca_KeyDown);
            this.btnB.Text="Buscar"; this.btnB.Left=295; this.btnB.Top=8; this.btnB.Width=100; this.btnB.Height=28;
            this.btnB.BackColor=System.Drawing.Color.FromArgb(224,113,42); this.btnB.ForeColor=System.Drawing.Color.White;
            this.btnB.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnB.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnB.FlatAppearance.BorderSize=0; this.btnB.Click+=new System.EventHandler(this.BtnBuscar_Click);
            this.btnN.Text="+ Novo Cliente"; this.btnN.Left=405; this.btnN.Top=8; this.btnN.Width=120; this.btnN.Height=28;
            this.btnN.BackColor=System.Drawing.Color.FromArgb(87,120,38); this.btnN.ForeColor=System.Drawing.Color.White;
            this.btnN.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnN.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnN.FlatAppearance.BorderSize=0; this.btnN.Click+=new System.EventHandler(this.BtnNovo_Click);
            // pnlForm
            this.pnlForm.Dock=System.Windows.Forms.DockStyle.Bottom; this.pnlForm.Height=210;
            this.pnlForm.BackColor=System.Drawing.Color.FromArgb(245,237,216);
            this.pnlForm.BorderStyle=System.Windows.Forms.BorderStyle.FixedSingle; this.pnlForm.Visible=false;
            this.pnlForm.Controls.Add(this.lblNome); this.pnlForm.Controls.Add(this.txtNome);
            this.pnlForm.Controls.Add(this.lblCpf);  this.pnlForm.Controls.Add(this.txtCpf);
            this.pnlForm.Controls.Add(this.lblSit);  this.pnlForm.Controls.Add(this.cmbSituacao);
            this.pnlForm.Controls.Add(this.lblCel);  this.pnlForm.Controls.Add(this.txtCelular);
            this.pnlForm.Controls.Add(this.lblEmail); this.pnlForm.Controls.Add(this.txtEmail);
            this.pnlForm.Controls.Add(this.lblCep);  this.pnlForm.Controls.Add(this.txtCep);
            this.pnlForm.Controls.Add(this.lblEnd);  this.pnlForm.Controls.Add(this.txtEndereco);
            this.pnlForm.Controls.Add(this.lblNum);  this.pnlForm.Controls.Add(this.txtNumero);
            this.pnlForm.Controls.Add(this.lblComp); this.pnlForm.Controls.Add(this.txtComplemento);
            this.pnlForm.Controls.Add(this.lblBai);  this.pnlForm.Controls.Add(this.txtBairro);
            this.pnlForm.Controls.Add(this.lblCid);  this.pnlForm.Controls.Add(this.txtCidade);
            this.pnlForm.Controls.Add(this.lblUF);   this.pnlForm.Controls.Add(this.txtEstado);
            this.pnlForm.Controls.Add(this.pnlBtns);
            // row 1
            this.lblNome.Text="Nome / Razao Social:"; this.lblNome.AutoSize=true; this.lblNome.Left=10; this.lblNome.Top=11; this.lblNome.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.txtNome.Left=155; this.txtNome.Top=8; this.txtNome.Width=255; this.txtNome.BackColor=System.Drawing.Color.White; this.txtNome.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.lblCpf.Text="CPF / CNPJ:"; this.lblCpf.AutoSize=true; this.lblCpf.Left=422; this.lblCpf.Top=11; this.lblCpf.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.txtCpf.Left=500; this.txtCpf.Top=8; this.txtCpf.Width=160; this.txtCpf.BackColor=System.Drawing.Color.White; this.txtCpf.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.txtCpf.TextChanged+=new System.EventHandler(this.TxtCpf_TextChanged);
            this.lblSit.Text="Situacao:"; this.lblSit.AutoSize=true; this.lblSit.Left=672; this.lblSit.Top=11; this.lblSit.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.cmbSituacao.Left=730; this.cmbSituacao.Top=8; this.cmbSituacao.Width=95;
            this.cmbSituacao.DropDownStyle=System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSituacao.BackColor=System.Drawing.Color.White; this.cmbSituacao.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.cmbSituacao.Items.AddRange(new object[]{"Ativo","Inativo"}); this.cmbSituacao.SelectedIndex=0;
            // row 2
            this.lblCel.Text="Celular:"; this.lblCel.AutoSize=true; this.lblCel.Left=10; this.lblCel.Top=47; this.lblCel.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.txtCelular.Mask="(00) 00000-0000"; this.txtCelular.Left=72; this.txtCelular.Top=44; this.txtCelular.Width=140; this.txtCelular.BackColor=System.Drawing.Color.White; this.txtCelular.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.lblEmail.Text="E-mail:"; this.lblEmail.AutoSize=true; this.lblEmail.Left=420; this.lblEmail.Top=47; this.lblEmail.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.txtEmail.Left=462; this.txtEmail.Top=44; this.txtEmail.Width=265; this.txtEmail.BackColor=System.Drawing.Color.White; this.txtEmail.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            // row 3
            this.lblCep.Text="CEP:"; this.lblCep.AutoSize=true; this.lblCep.Left=10; this.lblCep.Top=83; this.lblCep.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.txtCep.Mask="00000-000"; this.txtCep.Left=44; this.txtCep.Top=80; this.txtCep.Width=95; this.txtCep.BackColor=System.Drawing.Color.White; this.txtCep.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.txtCep.Leave += new System.EventHandler(this.TxtCep_Leave);
            this.lblEnd.Text="Endereco:"; this.lblEnd.AutoSize=true; this.lblEnd.Left=152; this.lblEnd.Top=83; this.lblEnd.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.txtEndereco.Left=218; this.txtEndereco.Top=80; this.txtEndereco.Width=225; this.txtEndereco.BackColor=System.Drawing.Color.White; this.txtEndereco.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.lblNum.Text="N\u00ba:"; this.lblNum.AutoSize=true; this.lblNum.Left=456; this.lblNum.Top=83; this.lblNum.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.txtNumero.Left=476; this.txtNumero.Top=80; this.txtNumero.Width=58; this.txtNumero.BackColor=System.Drawing.Color.White; this.txtNumero.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.lblComp.Text="Compl.:"; this.lblComp.AutoSize=true; this.lblComp.Left=547; this.lblComp.Top=83; this.lblComp.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.txtComplemento.Left=594; this.txtComplemento.Top=80; this.txtComplemento.Width=130; this.txtComplemento.BackColor=System.Drawing.Color.White; this.txtComplemento.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            // row 4
            this.lblBai.Text="Bairro:"; this.lblBai.AutoSize=true; this.lblBai.Left=10; this.lblBai.Top=119; this.lblBai.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.txtBairro.Left=58; this.txtBairro.Top=116; this.txtBairro.Width=188; this.txtBairro.BackColor=System.Drawing.Color.White; this.txtBairro.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.lblCid.Text="Cidade:"; this.lblCid.AutoSize=true; this.lblCid.Left=260; this.lblCid.Top=119; this.lblCid.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.txtCidade.Left=308; this.txtCidade.Top=116; this.txtCidade.Width=188; this.txtCidade.BackColor=System.Drawing.Color.White; this.txtCidade.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            this.lblUF.Text="UF:"; this.lblUF.AutoSize=true; this.lblUF.Left=508; this.lblUF.Top=119; this.lblUF.ForeColor=System.Drawing.Color.FromArgb(60,80,120);
            this.txtEstado.Left=528; this.txtEstado.Top=116; this.txtEstado.Width=50; this.txtEstado.BackColor=System.Drawing.Color.White; this.txtEstado.ForeColor=System.Drawing.Color.FromArgb(50,50,50);
            // pnlBtns
            this.pnlBtns.Dock=System.Windows.Forms.DockStyle.Bottom; this.pnlBtns.Height=48;
            this.pnlBtns.BackColor=System.Drawing.Color.FromArgb(245,237,216);
            this.pnlBtns.Controls.Add(this.btnS); this.pnlBtns.Controls.Add(this.btnC);
            this.pnlBtns.SizeChanged+=new System.EventHandler(this.PnlBtns_SizeChanged);
            this.btnS.Text="Salvar"; this.btnS.Left=10; this.btnS.Top=10; this.btnS.Width=110; this.btnS.Height=28;
            this.btnS.BackColor=System.Drawing.Color.FromArgb(87,120,38); this.btnS.ForeColor=System.Drawing.Color.White;
            this.btnS.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnS.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnS.FlatAppearance.BorderSize=0; this.btnS.Click+=new System.EventHandler(this.BtnSalvar_Click);
            this.btnC.Text="Cancelar"; this.btnC.Left=130; this.btnC.Top=10; this.btnC.Width=110; this.btnC.Height=28;
            this.btnC.BackColor=System.Drawing.Color.FromArgb(224,113,42); this.btnC.ForeColor=System.Drawing.Color.White;
            this.btnC.FlatStyle=System.Windows.Forms.FlatStyle.Flat; this.btnC.Font=new System.Drawing.Font("Segoe UI",9F,System.Drawing.FontStyle.Bold);
            this.btnC.FlatAppearance.BorderSize=0; this.btnC.Click+=new System.EventHandler(this.BtnCancelarCliente_Click);
            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 530);
            this.MinimumSize = new System.Drawing.Size(780, 460);
            this.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Clientes";
            this.Controls.Add(this.grid);
            this.Controls.Add(this.topBar);
            this.Controls.Add(this.pnlForm);
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.DataGridView   grid;
        internal System.Windows.Forms.Panel          pnlForm;
        internal System.Windows.Forms.TextBox        txtBusca;
        internal System.Windows.Forms.TextBox        txtNome;
        internal System.Windows.Forms.TextBox        txtEmail;
        internal System.Windows.Forms.TextBox        txtCpf;
        internal System.Windows.Forms.TextBox        txtEndereco;
        internal System.Windows.Forms.TextBox        txtNumero;
        internal System.Windows.Forms.TextBox        txtComplemento;
        internal System.Windows.Forms.TextBox        txtBairro;
        internal System.Windows.Forms.TextBox        txtCidade;
        internal System.Windows.Forms.TextBox        txtEstado;
        internal System.Windows.Forms.MaskedTextBox  txtCelular;
        internal System.Windows.Forms.MaskedTextBox  txtCep;
        internal System.Windows.Forms.ComboBox       cmbSituacao;
        internal System.Windows.Forms.Button         btnB;
        internal System.Windows.Forms.Button         btnN;
        internal System.Windows.Forms.Button         btnS;
        internal System.Windows.Forms.Button         btnC;
        private  System.Windows.Forms.Panel          topBar;
        private  System.Windows.Forms.Panel          pnlBtns;
        private  System.Windows.Forms.Label          lblB;
        private  System.Windows.Forms.Label          lblNome;
        private  System.Windows.Forms.Label          lblCpf;
        private  System.Windows.Forms.Label          lblSit;
        private  System.Windows.Forms.Label          lblCel;
        private  System.Windows.Forms.Label          lblEmail;
        private  System.Windows.Forms.Label          lblCep;
        private  System.Windows.Forms.Label          lblEnd;
        private  System.Windows.Forms.Label          lblNum;
        private  System.Windows.Forms.Label          lblComp;
        private  System.Windows.Forms.Label          lblBai;
        private  System.Windows.Forms.Label          lblCid;
        private  System.Windows.Forms.Label          lblUF;

        private void Grid_DoubleClick(object s, System.EventArgs e) { CarregarParaEditar(); }
        private void Grid_DataError(object s, System.Windows.Forms.DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }
        private void TxtBusca_KeyDown(object s, System.Windows.Forms.KeyEventArgs e) { if (e.KeyCode == System.Windows.Forms.Keys.Enter) CarregarGrid(); }
        private void BtnBuscar_Click(object s, System.EventArgs e) { CarregarGrid(); }
        private void BtnNovo_Click(object s, System.EventArgs e)   { ModoNovo(); }
        private void BtnCancelarCliente_Click(object s, System.EventArgs e) { pnlForm.Visible = false; _codigoEditando = 0; }
        private void PnlBtns_SizeChanged(object s, System.EventArgs e)
        {
            int x = (pnlBtns.Width - 110 * 2 - 10) / 2;
            if (x < 10) x = 10;
            btnS.Left = x; btnC.Left = x + 120;
        }
    }
}
