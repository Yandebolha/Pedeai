using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmEmpresa
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.txtEmpNome          = new System.Windows.Forms.TextBox();
            this.txtEmpFantasia      = new System.Windows.Forms.TextBox();
            this.txtEmpCNPJ          = new System.Windows.Forms.TextBox();
            this.txtEmpTel           = new System.Windows.Forms.TextBox();
            this.txtEmpEmail         = new System.Windows.Forms.TextBox();
            this.txtEmpEnd           = new System.Windows.Forms.TextBox();
            this.txtUsrNome          = new System.Windows.Forms.TextBox();
            this.txtUsrLogin         = new System.Windows.Forms.TextBox();
            this.txtUsrSenha         = new System.Windows.Forms.TextBox();
            this.txtUsrSenhaConf     = new System.Windows.Forms.TextBox();
            this.cmbUsrNivel         = new System.Windows.Forms.ComboBox();
            this.cmbUsrSit           = new System.Windows.Forms.ComboBox();
            this.pnlBuscaUsuarios    = new System.Windows.Forms.Panel();
            this.txtPesquisa         = new System.Windows.Forms.TextBox();
            this.lstUsuarios         = new System.Windows.Forms.ListBox();
            this.btnNovoUsr          = new System.Windows.Forms.Button();
            this.btnPesquisarUsr     = new System.Windows.Forms.Button();
            this.btnSalvUsr          = new System.Windows.Forms.Button();
            this.btnCancelarUsr      = new System.Windows.Forms.Button();
            this.chkModDashboard     = new System.Windows.Forms.CheckBox();
            this.chkModPedidos       = new System.Windows.Forms.CheckBox();
            this.chkModFinanceiro    = new System.Windows.Forms.CheckBox();
            this.chkModProdutos      = new System.Windows.Forms.CheckBox();
            this.chkModCategorias    = new System.Windows.Forms.CheckBox();
            this.chkModClientes      = new System.Windows.Forms.CheckBox();
            this.chkModFornecedores  = new System.Windows.Forms.CheckBox();
            this.chkModCupons        = new System.Windows.Forms.CheckBox();
            this.chkModEmpresa       = new System.Windows.Forms.CheckBox();
            this.chkModCancelarPedidos   = new System.Windows.Forms.CheckBox();
            this.chkModEntradaMercadoria = new System.Windows.Forms.CheckBox();
            this.chkModAvisos        = new System.Windows.Forms.CheckBox();
            this.txtImpNomeEmpresa   = new System.Windows.Forms.TextBox();
            this.txtImpEndereco      = new System.Windows.Forms.TextBox();
            this.txtImpTelefone      = new System.Windows.Forms.TextBox();
            this.txtImpCNPJ          = new System.Windows.Forms.TextBox();
            this.txtImpSeparador     = new System.Windows.Forms.TextBox();
            this.txtImpAvisoFiscal   = new System.Windows.Forms.TextBox();
            this.txtImpRodapeTexto   = new System.Windows.Forms.TextBox();
            this.txtImpLblNumero     = new System.Windows.Forms.TextBox();
            this.txtImpLblColItem    = new System.Windows.Forms.TextBox();
            this.txtImpLblColTotal   = new System.Windows.Forms.TextBox();
            this.txtImpLblSubtotal   = new System.Windows.Forms.TextBox();
            this.txtImpLblTaxa       = new System.Windows.Forms.TextBox();
            this.txtImpLblTotalPagar = new System.Windows.Forms.TextBox();
            this.txtImpLblAtendente  = new System.Windows.Forms.TextBox();
            this.cmbImpressora       = new System.Windows.Forms.ComboBox();
            this.numLargura          = new System.Windows.Forms.NumericUpDown();
            this.tabControl          = new System.Windows.Forms.TabControl();
            this.tabEmp              = new System.Windows.Forms.TabPage();
            this.tabUsr              = new System.Windows.Forms.TabPage();
            this.tabImp              = new System.Windows.Forms.TabPage();
            this.btnSalvEmp          = new System.Windows.Forms.Button();
            this.btnSalvImp          = new System.Windows.Forms.Button();
            this.btnTesteImp         = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // tabControl
            //
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tabControl.Padding = new System.Drawing.Point(14, 5);
            this.tabControl.TabPages.Add(this.tabEmp);
            this.tabControl.TabPages.Add(this.tabUsr);
            this.tabControl.TabPages.Add(this.tabImp);
            //
            // tabEmp - Dados da Empresa
            //
            this.tabEmp.Text = "  Dados da Empresa  ";
            this.tabEmp.BackColor = System.Drawing.Color.FromArgb(13, 19, 40);
            this.tabEmp.ForeColor = System.Drawing.Color.White;
            // Empresa fields
            this.txtEmpNome.SetBounds(168, 86, 360, 27);
            this.txtEmpNome.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.txtEmpNome.ForeColor = System.Drawing.Color.White;
            this.txtEmpNome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpFantasia.SetBounds(168, 117, 360, 27);
            this.txtEmpFantasia.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.txtEmpFantasia.ForeColor = System.Drawing.Color.White;
            this.txtEmpFantasia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpCNPJ.SetBounds(168, 148, 360, 27);
            this.txtEmpCNPJ.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.txtEmpCNPJ.ForeColor = System.Drawing.Color.White;
            this.txtEmpCNPJ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpTel.SetBounds(168, 179, 360, 27);
            this.txtEmpTel.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.txtEmpTel.ForeColor = System.Drawing.Color.White;
            this.txtEmpTel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpEmail.SetBounds(168, 210, 360, 27);
            this.txtEmpEmail.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.txtEmpEmail.ForeColor = System.Drawing.Color.White;
            this.txtEmpEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpEnd.SetBounds(168, 241, 360, 27);
            this.txtEmpEnd.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.txtEmpEnd.ForeColor = System.Drawing.Color.White;
            this.txtEmpEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btnSalvEmp.Text = "\u2713  Salvar Empresa";
            this.btnSalvEmp.SetBounds(168, 282, 160, 32);
            this.btnSalvEmp.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnSalvEmp.ForeColor = System.Drawing.Color.White;
            this.btnSalvEmp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvEmp.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSalvEmp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalvEmp.FlatAppearance.BorderSize = 0;
            this.btnSalvEmp.Click += new System.EventHandler(this.BtnSalvarEmpresa_Click);
            this.tabEmp.Controls.Add(this.lblEmpNome);
            this.tabEmp.Controls.Add(this.txtEmpNome);
            this.tabEmp.Controls.Add(this.lblEmpFantasia);
            this.tabEmp.Controls.Add(this.txtEmpFantasia);
            this.tabEmp.Controls.Add(this.lblEmpCNPJ);
            this.tabEmp.Controls.Add(this.txtEmpCNPJ);
            this.tabEmp.Controls.Add(this.lblEmpTel);
            this.tabEmp.Controls.Add(this.txtEmpTel);
            this.tabEmp.Controls.Add(this.lblEmpEmail);
            this.tabEmp.Controls.Add(this.txtEmpEmail);
            this.tabEmp.Controls.Add(this.lblEmpEnd);
            this.tabEmp.Controls.Add(this.txtEmpEnd);
            this.tabEmp.Controls.Add(this.btnSalvEmp);
            //
            // tabUsr - Usuarios
            //
            this.tabUsr.Text = "  Usu\u00e1rios  ";
            this.tabUsr.BackColor = System.Drawing.Color.FromArgb(13, 19, 40);
            this.tabUsr.ForeColor = System.Drawing.Color.White;
            // user fields
            this.txtUsrNome.SetBounds(168, 20, 280, 26);
            this.txtUsrNome.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.txtUsrNome.ForeColor = System.Drawing.Color.White;
            this.txtUsrNome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsrLogin.SetBounds(168, 60, 280, 26);
            this.txtUsrLogin.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.txtUsrLogin.ForeColor = System.Drawing.Color.White;
            this.txtUsrLogin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsrSenha.SetBounds(168, 100, 280, 26);
            this.txtUsrSenha.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.txtUsrSenha.ForeColor = System.Drawing.Color.White;
            this.txtUsrSenha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsrSenha.PasswordChar = '\u25CF';
            this.txtUsrSenhaConf.SetBounds(168, 140, 280, 26);
            this.txtUsrSenhaConf.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.txtUsrSenhaConf.ForeColor = System.Drawing.Color.White;
            this.txtUsrSenhaConf.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsrSenhaConf.PasswordChar = '\u25CF';
            this.cmbUsrNivel.SetBounds(168, 180, 280, 26);
            this.cmbUsrNivel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsrNivel.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.cmbUsrNivel.ForeColor = System.Drawing.Color.White;
            this.cmbUsrNivel.Items.AddRange(new object[] { "Operador", "Gerente", "Admin" });
            this.cmbUsrNivel.SelectedIndex = 0;
            this.cmbUsrSit.SetBounds(168, 220, 280, 26);
            this.cmbUsrSit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsrSit.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.cmbUsrSit.ForeColor = System.Drawing.Color.White;
            this.cmbUsrSit.Items.AddRange(new object[] { "Ativo", "Inativo" });
            this.cmbUsrSit.SelectedIndex = 0;
            // checkboxes
            this.chkModDashboard.Text         = "Dashboard";       this.chkModDashboard.SetBounds(168, 260, 102, 22);
            this.chkModPedidos.Text           = "Pedidos";         this.chkModPedidos.SetBounds(274, 260, 102, 22);
            this.chkModFinanceiro.Text        = "Financeiro";      this.chkModFinanceiro.SetBounds(380, 260, 102, 22);
            this.chkModProdutos.Text          = "Produtos";        this.chkModProdutos.SetBounds(168, 286, 102, 22);
            this.chkModCategorias.Text        = "Categorias";      this.chkModCategorias.SetBounds(274, 286, 102, 22);
            this.chkModClientes.Text          = "Clientes";        this.chkModClientes.SetBounds(380, 286, 102, 22);
            this.chkModFornecedores.Text      = "Fornecedores";    this.chkModFornecedores.SetBounds(168, 312, 102, 22);
            this.chkModCupons.Text            = "Cupons";          this.chkModCupons.SetBounds(274, 312, 102, 22);
            this.chkModEmpresa.Text           = "Empresa";         this.chkModEmpresa.SetBounds(380, 312, 102, 22);
            this.chkModCancelarPedidos.Text   = "Cancelar Pedidos"; this.chkModCancelarPedidos.SetBounds(168, 338, 102, 22);
            this.chkModEntradaMercadoria.Text = "Ent. Mercadoria"; this.chkModEntradaMercadoria.SetBounds(274, 338, 102, 22);
            this.chkModAvisos.Text            = "Avisos";          this.chkModAvisos.SetBounds(380, 338, 102, 22);
            // buttons
            this.btnNovoUsr.Text = "+ Novo"; this.btnNovoUsr.SetBounds(168, 374, 128, 32);
            this.btnNovoUsr.BackColor = System.Drawing.Color.FromArgb(52,73,94); this.btnNovoUsr.ForeColor = System.Drawing.Color.White;
            this.btnNovoUsr.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnNovoUsr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNovoUsr.FlatAppearance.BorderSize = 0; this.btnNovoUsr.Click += new System.EventHandler(this.BtnNovoUsuario_Click);
            this.btnPesquisarUsr.Text = "Pesquisar"; this.btnPesquisarUsr.SetBounds(304, 374, 128, 32);
            this.btnPesquisarUsr.BackColor = System.Drawing.Color.FromArgb(52,152,219); this.btnPesquisarUsr.ForeColor = System.Drawing.Color.White;
            this.btnPesquisarUsr.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnPesquisarUsr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPesquisarUsr.FlatAppearance.BorderSize = 0; this.btnPesquisarUsr.Click += new System.EventHandler(this.BtnPesquisarUsuario_Click);
            this.btnSalvUsr.Text = "\u2713  Salvar"; this.btnSalvUsr.SetBounds(168, 374, 128, 32);
            this.btnSalvUsr.BackColor = System.Drawing.Color.FromArgb(39,174,96); this.btnSalvUsr.ForeColor = System.Drawing.Color.White;
            this.btnSalvUsr.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnSalvUsr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalvUsr.FlatAppearance.BorderSize = 0; this.btnSalvUsr.Visible = false;
            this.btnSalvUsr.Click += new System.EventHandler(this.BtnSalvarUsuario_Click);
            this.btnCancelarUsr.Text = "Cancelar"; this.btnCancelarUsr.SetBounds(304, 374, 128, 32);
            this.btnCancelarUsr.BackColor = System.Drawing.Color.FromArgb(108,117,125); this.btnCancelarUsr.ForeColor = System.Drawing.Color.White;
            this.btnCancelarUsr.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnCancelarUsr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelarUsr.FlatAppearance.BorderSize = 0; this.btnCancelarUsr.Visible = false;
            this.btnCancelarUsr.Click += new System.EventHandler(this.BtnCancelarUsuario_Click);
            // pnlBuscaUsuarios
            this.pnlBuscaUsuarios.SetBounds(0, 0, 460, 200);
            this.pnlBuscaUsuarios.BackColor = System.Drawing.Color.FromArgb(22, 31, 58);
            this.pnlBuscaUsuarios.Visible = false;
            this.txtPesquisa.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtPesquisa.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.txtPesquisa.ForeColor = System.Drawing.Color.White;
            this.txtPesquisa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPesquisa.TextChanged += new System.EventHandler(this.TxtPesquisa_TextChanged);
            this.lstUsuarios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstUsuarios.BackColor = System.Drawing.Color.FromArgb(18, 26, 50);
            this.lstUsuarios.ForeColor = System.Drawing.Color.White;
            this.lstUsuarios.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstUsuarios.DoubleClick += new System.EventHandler(this.LstUsuarios_DoubleClick);
            this.pnlBuscaUsuarios.Controls.Add(this.lstUsuarios);
            this.pnlBuscaUsuarios.Controls.Add(this.txtPesquisa);
            this.tabUsr.Controls.Add(this.lblUsrNome);
            this.tabUsr.Controls.Add(this.txtUsrNome);
            this.tabUsr.Controls.Add(this.lblUsrLogin);
            this.tabUsr.Controls.Add(this.txtUsrLogin);
            this.tabUsr.Controls.Add(this.lblUsrSenha);
            this.tabUsr.Controls.Add(this.txtUsrSenha);
            this.tabUsr.Controls.Add(this.lblUsrSenhaConf);
            this.tabUsr.Controls.Add(this.txtUsrSenhaConf);
            this.tabUsr.Controls.Add(this.lblUsrNivel);
            this.tabUsr.Controls.Add(this.cmbUsrNivel);
            this.tabUsr.Controls.Add(this.lblUsrSit);
            this.tabUsr.Controls.Add(this.cmbUsrSit);
            this.tabUsr.Controls.Add(this.chkModDashboard);
            this.tabUsr.Controls.Add(this.chkModPedidos);
            this.tabUsr.Controls.Add(this.chkModFinanceiro);
            this.tabUsr.Controls.Add(this.chkModProdutos);
            this.tabUsr.Controls.Add(this.chkModCategorias);
            this.tabUsr.Controls.Add(this.chkModClientes);
            this.tabUsr.Controls.Add(this.chkModFornecedores);
            this.tabUsr.Controls.Add(this.chkModCupons);
            this.tabUsr.Controls.Add(this.chkModEmpresa);
            this.tabUsr.Controls.Add(this.chkModCancelarPedidos);
            this.tabUsr.Controls.Add(this.chkModEntradaMercadoria);
            this.tabUsr.Controls.Add(this.chkModAvisos);
            this.tabUsr.Controls.Add(this.btnNovoUsr);
            this.tabUsr.Controls.Add(this.btnPesquisarUsr);
            this.tabUsr.Controls.Add(this.btnSalvUsr);
            this.tabUsr.Controls.Add(this.btnCancelarUsr);
            this.tabUsr.Controls.Add(this.pnlBuscaUsuarios);
            //
            // tabImp - Impressao
            //
            this.tabImp.Text = "  Impress\u00e3o  ";
            this.tabImp.BackColor = System.Drawing.Color.FromArgb(13, 19, 40);
            this.tabImp.ForeColor = System.Drawing.Color.White;
            // imp fields
            this.txtImpNomeEmpresa.SetBounds(208, 30, 400, 26); this.txtImpNomeEmpresa.BackColor = System.Drawing.Color.FromArgb(18,26,50); this.txtImpNomeEmpresa.ForeColor = System.Drawing.Color.White; this.txtImpNomeEmpresa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpEndereco.SetBounds(208, 62, 400, 26);    this.txtImpEndereco.BackColor = System.Drawing.Color.FromArgb(18,26,50);    this.txtImpEndereco.ForeColor = System.Drawing.Color.White;    this.txtImpEndereco.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpTelefone.SetBounds(208, 94, 400, 26);    this.txtImpTelefone.BackColor = System.Drawing.Color.FromArgb(18,26,50);    this.txtImpTelefone.ForeColor = System.Drawing.Color.White;    this.txtImpTelefone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpCNPJ.SetBounds(208, 126, 400, 26);       this.txtImpCNPJ.BackColor = System.Drawing.Color.FromArgb(18,26,50);       this.txtImpCNPJ.ForeColor = System.Drawing.Color.White;       this.txtImpCNPJ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpSeparador.SetBounds(208, 158, 400, 26);  this.txtImpSeparador.BackColor = System.Drawing.Color.FromArgb(18,26,50);  this.txtImpSeparador.ForeColor = System.Drawing.Color.White;  this.txtImpSeparador.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpAvisoFiscal.SetBounds(208, 190, 400, 26);this.txtImpAvisoFiscal.BackColor = System.Drawing.Color.FromArgb(18,26,50);this.txtImpAvisoFiscal.ForeColor = System.Drawing.Color.White;this.txtImpAvisoFiscal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpRodapeTexto.SetBounds(208, 222, 400, 64);this.txtImpRodapeTexto.BackColor = System.Drawing.Color.FromArgb(18,26,50);this.txtImpRodapeTexto.ForeColor = System.Drawing.Color.White;this.txtImpRodapeTexto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpRodapeTexto.Multiline = true;
            this.txtImpLblNumero.SetBounds(208, 300, 200, 26);        this.txtImpLblNumero.BackColor = System.Drawing.Color.FromArgb(18,26,50);     this.txtImpLblNumero.ForeColor = System.Drawing.Color.White;     this.txtImpLblNumero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblColItem.SetBounds(208, 332, 200, 26);       this.txtImpLblColItem.BackColor = System.Drawing.Color.FromArgb(18,26,50);    this.txtImpLblColItem.ForeColor = System.Drawing.Color.White;    this.txtImpLblColItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblColTotal.SetBounds(208, 364, 200, 26);      this.txtImpLblColTotal.BackColor = System.Drawing.Color.FromArgb(18,26,50);   this.txtImpLblColTotal.ForeColor = System.Drawing.Color.White;   this.txtImpLblColTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblSubtotal.SetBounds(208, 396, 200, 26);      this.txtImpLblSubtotal.BackColor = System.Drawing.Color.FromArgb(18,26,50);   this.txtImpLblSubtotal.ForeColor = System.Drawing.Color.White;   this.txtImpLblSubtotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblTaxa.SetBounds(208, 428, 200, 26);          this.txtImpLblTaxa.BackColor = System.Drawing.Color.FromArgb(18,26,50);       this.txtImpLblTaxa.ForeColor = System.Drawing.Color.White;       this.txtImpLblTaxa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblTotalPagar.SetBounds(208, 460, 200, 26);    this.txtImpLblTotalPagar.BackColor = System.Drawing.Color.FromArgb(18,26,50); this.txtImpLblTotalPagar.ForeColor = System.Drawing.Color.White; this.txtImpLblTotalPagar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblAtendente.SetBounds(208, 492, 200, 26);     this.txtImpLblAtendente.BackColor = System.Drawing.Color.FromArgb(18,26,50);  this.txtImpLblAtendente.ForeColor = System.Drawing.Color.White;  this.txtImpLblAtendente.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cmbImpressora.SetBounds(208, 530, 400, 26);
            this.cmbImpressora.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImpressora.BackColor = System.Drawing.Color.FromArgb(18,26,50);
            this.cmbImpressora.ForeColor = System.Drawing.Color.White;
            this.cmbImpressora.Items.Add("(Impressora padr\u00e3o do sistema)");
            this.cmbImpressora.SelectedIndex = 0;
            this.numLargura.SetBounds(208, 562, 80, 26);
            this.numLargura.Minimum = 20; this.numLargura.Maximum = 120; this.numLargura.Value = 42;
            this.numLargura.BackColor = System.Drawing.Color.FromArgb(18,26,50);
            this.numLargura.ForeColor = System.Drawing.Color.White;
            this.btnSalvImp.Text = "\u2713  Salvar Configura\u00e7\u00e3o";
            this.btnSalvImp.SetBounds(208, 600, 200, 32);
            this.btnSalvImp.BackColor = System.Drawing.Color.FromArgb(39,174,96); this.btnSalvImp.ForeColor = System.Drawing.Color.White;
            this.btnSalvImp.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnSalvImp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalvImp.FlatAppearance.BorderSize = 0; this.btnSalvImp.Click += new System.EventHandler(this.BtnSalvarImpressao_Click);
            this.btnTesteImp.Text = "Imprimir Teste";
            this.btnTesteImp.SetBounds(416, 600, 140, 32);
            this.btnTesteImp.BackColor = System.Drawing.Color.FromArgb(52,152,219); this.btnTesteImp.ForeColor = System.Drawing.Color.White;
            this.btnTesteImp.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnTesteImp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTesteImp.FlatAppearance.BorderSize = 0; this.btnTesteImp.Click += new System.EventHandler(this.BtnImprimirTeste_Click);
            this.tabImp.Controls.Add(this.lblImpNome);      this.tabImp.Controls.Add(this.txtImpNomeEmpresa);
            this.tabImp.Controls.Add(this.lblImpEnd);       this.tabImp.Controls.Add(this.txtImpEndereco);
            this.tabImp.Controls.Add(this.lblImpTel);       this.tabImp.Controls.Add(this.txtImpTelefone);
            this.tabImp.Controls.Add(this.lblImpCNPJ);      this.tabImp.Controls.Add(this.txtImpCNPJ);
            this.tabImp.Controls.Add(this.lblImpSep);       this.tabImp.Controls.Add(this.txtImpSeparador);
            this.tabImp.Controls.Add(this.lblImpAviso);     this.tabImp.Controls.Add(this.txtImpAvisoFiscal);
            this.tabImp.Controls.Add(this.lblImpRodape);    this.tabImp.Controls.Add(this.txtImpRodapeTexto);
            this.tabImp.Controls.Add(this.lblImpNum);       this.tabImp.Controls.Add(this.txtImpLblNumero);
            this.tabImp.Controls.Add(this.lblImpColItem);   this.tabImp.Controls.Add(this.txtImpLblColItem);
            this.tabImp.Controls.Add(this.lblImpColTotal);  this.tabImp.Controls.Add(this.txtImpLblColTotal);
            this.tabImp.Controls.Add(this.lblImpSubtotal);  this.tabImp.Controls.Add(this.txtImpLblSubtotal);
            this.tabImp.Controls.Add(this.lblImpTaxa);      this.tabImp.Controls.Add(this.txtImpLblTaxa);
            this.tabImp.Controls.Add(this.lblImpTotalPagar);this.tabImp.Controls.Add(this.txtImpLblTotalPagar);
            this.tabImp.Controls.Add(this.lblImpAtendente); this.tabImp.Controls.Add(this.txtImpLblAtendente);
            this.tabImp.Controls.Add(this.lblImpressora);   this.tabImp.Controls.Add(this.cmbImpressora);
            this.tabImp.Controls.Add(this.lblLargura);      this.tabImp.Controls.Add(this.numLargura);
            this.tabImp.Controls.Add(this.btnSalvImp);
            this.tabImp.Controls.Add(this.btnTesteImp);
            // Form
            this.Text            = "Empresa e Usu\u00e1rios";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.BackColor       = System.Drawing.Color.FromArgb(13, 19, 40);
            this.ForeColor       = System.Drawing.Color.White;
            this.Font            = new System.Drawing.Font("Segoe UI", 9.5F);
            this.MinimumSize     = new System.Drawing.Size(960, 620);
            this.Size            = new System.Drawing.Size(1020, 680);
            this.Controls.Add(this.tabControl);
            this.ResumeLayout(false);
        }

        // Empresa
        internal System.Windows.Forms.TextBox txtEmpNome;
        internal System.Windows.Forms.TextBox txtEmpFantasia;
        internal System.Windows.Forms.TextBox txtEmpCNPJ;
        internal System.Windows.Forms.TextBox txtEmpTel;
        internal System.Windows.Forms.TextBox txtEmpEmail;
        internal System.Windows.Forms.TextBox txtEmpEnd;
        // Usuario
        internal System.Windows.Forms.TextBox    txtUsrNome;
        internal System.Windows.Forms.TextBox    txtUsrLogin;
        internal System.Windows.Forms.TextBox    txtUsrSenha;
        internal System.Windows.Forms.TextBox    txtUsrSenhaConf;
        internal System.Windows.Forms.ComboBox   cmbUsrNivel;
        internal System.Windows.Forms.ComboBox   cmbUsrSit;
        internal System.Windows.Forms.Panel      pnlBuscaUsuarios;
        internal System.Windows.Forms.TextBox    txtPesquisa;
        internal System.Windows.Forms.ListBox    lstUsuarios;
        internal System.Windows.Forms.Button     btnNovoUsr;
        internal System.Windows.Forms.Button     btnPesquisarUsr;
        internal System.Windows.Forms.Button     btnSalvUsr;
        internal System.Windows.Forms.Button     btnCancelarUsr;
        // Checkboxes
        internal System.Windows.Forms.CheckBox chkModDashboard;
        internal System.Windows.Forms.CheckBox chkModPedidos;
        internal System.Windows.Forms.CheckBox chkModFinanceiro;
        internal System.Windows.Forms.CheckBox chkModProdutos;
        internal System.Windows.Forms.CheckBox chkModCategorias;
        internal System.Windows.Forms.CheckBox chkModClientes;
        internal System.Windows.Forms.CheckBox chkModFornecedores;
        internal System.Windows.Forms.CheckBox chkModCupons;
        internal System.Windows.Forms.CheckBox chkModEmpresa;
        internal System.Windows.Forms.CheckBox chkModCancelarPedidos;
        internal System.Windows.Forms.CheckBox chkModEntradaMercadoria;
        internal System.Windows.Forms.CheckBox chkModAvisos;
        // Impressao
        internal System.Windows.Forms.TextBox        txtImpNomeEmpresa;
        internal System.Windows.Forms.TextBox        txtImpEndereco;
        internal System.Windows.Forms.TextBox        txtImpTelefone;
        internal System.Windows.Forms.TextBox        txtImpCNPJ;
        internal System.Windows.Forms.TextBox        txtImpSeparador;
        internal System.Windows.Forms.TextBox        txtImpAvisoFiscal;
        internal System.Windows.Forms.TextBox        txtImpRodapeTexto;
        internal System.Windows.Forms.TextBox        txtImpLblNumero;
        internal System.Windows.Forms.TextBox        txtImpLblColItem;
        internal System.Windows.Forms.TextBox        txtImpLblColTotal;
        internal System.Windows.Forms.TextBox        txtImpLblSubtotal;
        internal System.Windows.Forms.TextBox        txtImpLblTaxa;
        internal System.Windows.Forms.TextBox        txtImpLblTotalPagar;
        internal System.Windows.Forms.TextBox        txtImpLblAtendente;
        internal System.Windows.Forms.ComboBox       cmbImpressora;
        internal System.Windows.Forms.NumericUpDown  numLargura;
        // Private helpers
        private System.Windows.Forms.TabControl     tabControl;
        private System.Windows.Forms.TabPage        tabEmp;
        private System.Windows.Forms.TabPage        tabUsr;
        private System.Windows.Forms.TabPage        tabImp;
        private System.Windows.Forms.Button         btnSalvEmp;
        private System.Windows.Forms.Button         btnSalvImp;
        private System.Windows.Forms.Button         btnTesteImp;
        // labels empresa
        private System.Windows.Forms.Label lblEmpNome     = new System.Windows.Forms.Label { Text = "Raz\u00e3o Social:",  AutoSize = true, Left = 22, Top = 92,  ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblEmpFantasia = new System.Windows.Forms.Label { Text = "Nome Fantasia:",     AutoSize = true, Left = 22, Top = 123, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblEmpCNPJ     = new System.Windows.Forms.Label { Text = "CNPJ:",             AutoSize = true, Left = 22, Top = 154, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblEmpTel      = new System.Windows.Forms.Label { Text = "Telefone:",         AutoSize = true, Left = 22, Top = 185, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblEmpEmail    = new System.Windows.Forms.Label { Text = "E-mail:",           AutoSize = true, Left = 22, Top = 216, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblEmpEnd      = new System.Windows.Forms.Label { Text = "Endere\u00e7o:",    AutoSize = true, Left = 22, Top = 247, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        // labels usuario
        private System.Windows.Forms.Label lblUsrNome     = new System.Windows.Forms.Label { Text = "Nome:",            AutoSize = true, Left = 22, Top = 25,  ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblUsrLogin    = new System.Windows.Forms.Label { Text = "Login:",           AutoSize = true, Left = 22, Top = 65,  ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblUsrSenha    = new System.Windows.Forms.Label { Text = "Senha:",           AutoSize = true, Left = 22, Top = 105, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblUsrSenhaConf= new System.Windows.Forms.Label { Text = "Confirmar:",       AutoSize = true, Left = 22, Top = 145, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblUsrNivel    = new System.Windows.Forms.Label { Text = "N\u00edvel:",      AutoSize = true, Left = 22, Top = 185, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblUsrSit      = new System.Windows.Forms.Label { Text = "Situa\u00e7\u00e3o:", AutoSize = true, Left = 22, Top = 225, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        // labels impressao
        private System.Windows.Forms.Label lblImpNome      = new System.Windows.Forms.Label { Text = "Nome (cab.):",    AutoSize = true, Left = 22, Top = 36,  ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpEnd       = new System.Windows.Forms.Label { Text = "Endere\u00e7o:", AutoSize = true, Left = 22, Top = 68,  ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpTel       = new System.Windows.Forms.Label { Text = "Telefone:",      AutoSize = true, Left = 22, Top = 100, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpCNPJ      = new System.Windows.Forms.Label { Text = "CNPJ:",          AutoSize = true, Left = 22, Top = 132, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpSep       = new System.Windows.Forms.Label { Text = "Separador:",     AutoSize = true, Left = 22, Top = 164, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpAviso     = new System.Windows.Forms.Label { Text = "Aviso fiscal:",  AutoSize = true, Left = 22, Top = 196, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpRodape    = new System.Windows.Forms.Label { Text = "Rodap\u00e9:",   AutoSize = true, Left = 22, Top = 228, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpNum       = new System.Windows.Forms.Label { Text = "N\u00ba Pedido:",AutoSize = true, Left = 22, Top = 306, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpColItem   = new System.Windows.Forms.Label { Text = "Col. Item:",     AutoSize = true, Left = 22, Top = 338, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpColTotal  = new System.Windows.Forms.Label { Text = "Col. Total:",    AutoSize = true, Left = 22, Top = 370, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpSubtotal  = new System.Windows.Forms.Label { Text = "Subtotal:",      AutoSize = true, Left = 22, Top = 402, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpTaxa      = new System.Windows.Forms.Label { Text = "Taxa:",          AutoSize = true, Left = 22, Top = 434, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpTotalPagar= new System.Windows.Forms.Label { Text = "Total a Pagar:", AutoSize = true, Left = 22, Top = 466, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpAtendente = new System.Windows.Forms.Label { Text = "Atendente:",     AutoSize = true, Left = 22, Top = 498, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblImpressora   = new System.Windows.Forms.Label { Text = "Impressora:",    AutoSize = true, Left = 22, Top = 536, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
        private System.Windows.Forms.Label lblLargura      = new System.Windows.Forms.Label { Text = "Largura:",       AutoSize = true, Left = 22, Top = 568, ForeColor = System.Drawing.Color.FromArgb(160,175,210) };
    }
}
