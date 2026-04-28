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
            this.txtEmpNome = new System.Windows.Forms.TextBox();
            this.txtEmpFantasia = new System.Windows.Forms.TextBox();
            this.txtEmpCNPJ = new System.Windows.Forms.TextBox();
            this.txtEmpTel = new System.Windows.Forms.TextBox();
            this.txtEmpEmail = new System.Windows.Forms.TextBox();
            this.txtEmpEnd = new System.Windows.Forms.TextBox();
            this.txtEmpCodigo = new System.Windows.Forms.TextBox();
            this.txtEmpImgBB = new System.Windows.Forms.TextBox();
            this.txtUsrNome = new System.Windows.Forms.TextBox();
            this.txtUsrLogin = new System.Windows.Forms.TextBox();
            this.txtUsrSenha = new System.Windows.Forms.TextBox();
            this.txtUsrSenhaConf = new System.Windows.Forms.TextBox();
            this.cmbUsrNivel = new System.Windows.Forms.ComboBox();
            this.cmbUsrSit = new System.Windows.Forms.ComboBox();
            this.pnlBuscaUsuarios = new System.Windows.Forms.Panel();
            this.lstUsuarios = new System.Windows.Forms.ListBox();
            this.txtPesquisa = new System.Windows.Forms.TextBox();
            this.btnNovoUsr = new System.Windows.Forms.Button();
            this.btnPesquisarUsr = new System.Windows.Forms.Button();
            this.btnSalvUsr = new System.Windows.Forms.Button();
            this.btnCancelarUsr = new System.Windows.Forms.Button();
            this.chkModDashboard = new System.Windows.Forms.CheckBox();
            this.chkModPedidos = new System.Windows.Forms.CheckBox();
            this.chkModFinanceiro = new System.Windows.Forms.CheckBox();
            this.chkModProdutos = new System.Windows.Forms.CheckBox();
            this.chkModCategorias = new System.Windows.Forms.CheckBox();
            this.chkModClientes = new System.Windows.Forms.CheckBox();
            this.chkModFornecedores = new System.Windows.Forms.CheckBox();
            this.chkModCupons = new System.Windows.Forms.CheckBox();
            this.chkModEstoque = new System.Windows.Forms.CheckBox();
            this.chkModEmpresa = new System.Windows.Forms.CheckBox();
            this.chkModCancelarPedidos = new System.Windows.Forms.CheckBox();
            this.chkModEntradaMercadoria = new System.Windows.Forms.CheckBox();
            this.chkModAvisos = new System.Windows.Forms.CheckBox();
            this.chkModTurno = new System.Windows.Forms.CheckBox();
            this.chkModConsultarPedido = new System.Windows.Forms.CheckBox();
            this.chkModFidelizacao = new System.Windows.Forms.CheckBox();
            this.chkModWhatsApp = new System.Windows.Forms.CheckBox();
            this.chkModBairros = new System.Windows.Forms.CheckBox();
            this.chkModMarmitas = new System.Windows.Forms.CheckBox();
            this.txtImpNomeEmpresa = new System.Windows.Forms.TextBox();
            this.txtImpEndereco = new System.Windows.Forms.TextBox();
            this.txtImpTelefone = new System.Windows.Forms.TextBox();
            this.txtImpCNPJ = new System.Windows.Forms.TextBox();
            this.txtImpSeparador = new System.Windows.Forms.TextBox();
            this.txtImpAvisoFiscal = new System.Windows.Forms.TextBox();
            this.txtImpRodapeTexto = new System.Windows.Forms.TextBox();
            this.txtImpLblNumero = new System.Windows.Forms.TextBox();
            this.txtImpLblColItem = new System.Windows.Forms.TextBox();
            this.txtImpLblColTotal = new System.Windows.Forms.TextBox();
            this.txtImpLblSubtotal = new System.Windows.Forms.TextBox();
            this.txtImpLblTaxa = new System.Windows.Forms.TextBox();
            this.txtImpLblTotalPagar = new System.Windows.Forms.TextBox();
            this.txtImpLblDesconto   = new System.Windows.Forms.TextBox();
            this.txtImpLblCupom      = new System.Windows.Forms.TextBox();
            this.txtImpLblAtendente = new System.Windows.Forms.TextBox();
            this.cmbImpressora = new System.Windows.Forms.ComboBox();
            this.numLargura = new System.Windows.Forms.NumericUpDown();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabEmp = new System.Windows.Forms.TabPage();
            this.btnSalvEmp = new System.Windows.Forms.Button();
            this.btnSincronizarSite = new System.Windows.Forms.Button();
            this.btnDiagnostico = new System.Windows.Forms.Button();
            this.lblSincStatus = new System.Windows.Forms.Label();
            this.tabUsr = new System.Windows.Forms.TabPage();
            this.tabImp = new System.Windows.Forms.TabPage();
            this.btnSalvImp = new System.Windows.Forms.Button();
            this.btnTesteImp = new System.Windows.Forms.Button();
            this.tabSistema = new System.Windows.Forms.TabPage();
            this.lblResetTitulo = new System.Windows.Forms.Label();
            this.lblResetAviso = new System.Windows.Forms.Label();
            this.btnResetarBanco = new System.Windows.Forms.Button();
            this.pnlBuscaUsuarios.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLargura)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabEmp.SuspendLayout();
            this.tabUsr.SuspendLayout();
            this.tabImp.SuspendLayout();
            this.tabSistema.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtEmpNome
            // 
            this.txtEmpNome.BackColor = System.Drawing.Color.White;
            this.txtEmpNome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpNome.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtEmpNome.Location = new System.Drawing.Point(168, 86);
            this.txtEmpNome.Name = "txtEmpNome";
            this.txtEmpNome.Size = new System.Drawing.Size(360, 25);
            this.txtEmpNome.TabIndex = 0;
            // 
            // txtEmpFantasia
            // 
            this.txtEmpFantasia.BackColor = System.Drawing.Color.White;
            this.txtEmpFantasia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpFantasia.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtEmpFantasia.Location = new System.Drawing.Point(168, 117);
            this.txtEmpFantasia.Name = "txtEmpFantasia";
            this.txtEmpFantasia.Size = new System.Drawing.Size(360, 25);
            this.txtEmpFantasia.TabIndex = 1;
            // 
            // txtEmpCNPJ
            // 
            this.txtEmpCNPJ.BackColor = System.Drawing.Color.White;
            this.txtEmpCNPJ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpCNPJ.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtEmpCNPJ.Location = new System.Drawing.Point(168, 148);
            this.txtEmpCNPJ.Name = "txtEmpCNPJ";
            this.txtEmpCNPJ.Size = new System.Drawing.Size(360, 25);
            this.txtEmpCNPJ.TabIndex = 2;
            // 
            // txtEmpTel
            // 
            this.txtEmpTel.BackColor = System.Drawing.Color.White;
            this.txtEmpTel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpTel.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtEmpTel.Location = new System.Drawing.Point(168, 179);
            this.txtEmpTel.Name = "txtEmpTel";
            this.txtEmpTel.Size = new System.Drawing.Size(360, 25);
            this.txtEmpTel.TabIndex = 3;
            // 
            // txtEmpEmail
            // 
            this.txtEmpEmail.BackColor = System.Drawing.Color.White;
            this.txtEmpEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpEmail.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtEmpEmail.Location = new System.Drawing.Point(168, 210);
            this.txtEmpEmail.Name = "txtEmpEmail";
            this.txtEmpEmail.Size = new System.Drawing.Size(360, 25);
            this.txtEmpEmail.TabIndex = 4;
            // 
            // txtEmpEnd
            // 
            this.txtEmpEnd.BackColor = System.Drawing.Color.White;
            this.txtEmpEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpEnd.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtEmpEnd.Location = new System.Drawing.Point(168, 241);
            this.txtEmpEnd.Name = "txtEmpEnd";
            this.txtEmpEnd.Size = new System.Drawing.Size(360, 25);
            this.txtEmpEnd.TabIndex = 5;
            // 
            // txtEmpCodigo
            // 
            this.txtEmpCodigo.BackColor = System.Drawing.Color.FromArgb(240, 230, 210);
            this.txtEmpCodigo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpCodigo.ForeColor = System.Drawing.Color.FromArgb(120, 60, 10);
            this.txtEmpCodigo.Location = new System.Drawing.Point(168, 272);
            this.txtEmpCodigo.Name = "txtEmpCodigo";
            this.txtEmpCodigo.ReadOnly = true;
            this.txtEmpCodigo.Size = new System.Drawing.Size(140, 25);
            this.txtEmpCodigo.TabIndex = 6;
            this.txtEmpCodigo.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold);
            // 
            // txtEmpImgBB
            // 
            this.txtEmpImgBB.BackColor = System.Drawing.Color.White;
            this.txtEmpImgBB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpImgBB.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtEmpImgBB.Location = new System.Drawing.Point(168, 314);
            this.txtEmpImgBB.Name = "txtEmpImgBB";
            this.txtEmpImgBB.Size = new System.Drawing.Size(360, 25);
            this.txtEmpImgBB.TabIndex = 7;
            this.txtEmpImgBB.PlaceholderText = "Chave API do ImgBB (imgbb.com)";
            // 
            // txtUsrNome
            // 
            this.txtUsrNome.BackColor = System.Drawing.Color.White;
            this.txtUsrNome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsrNome.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtUsrNome.Location = new System.Drawing.Point(168, 20);
            this.txtUsrNome.Name = "txtUsrNome";
            this.txtUsrNome.Size = new System.Drawing.Size(280, 25);
            this.txtUsrNome.TabIndex = 0;
            // 
            // txtUsrLogin
            // 
            this.txtUsrLogin.BackColor = System.Drawing.Color.White;
            this.txtUsrLogin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsrLogin.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtUsrLogin.Location = new System.Drawing.Point(168, 60);
            this.txtUsrLogin.Name = "txtUsrLogin";
            this.txtUsrLogin.Size = new System.Drawing.Size(280, 25);
            this.txtUsrLogin.TabIndex = 1;
            // 
            // txtUsrSenha
            // 
            this.txtUsrSenha.BackColor = System.Drawing.Color.White;
            this.txtUsrSenha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsrSenha.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtUsrSenha.Location = new System.Drawing.Point(168, 100);
            this.txtUsrSenha.Name = "txtUsrSenha";
            this.txtUsrSenha.PasswordChar = '*';
            this.txtUsrSenha.Size = new System.Drawing.Size(280, 25);
            this.txtUsrSenha.TabIndex = 2;
            // 
            // txtUsrSenhaConf
            // 
            this.txtUsrSenhaConf.BackColor = System.Drawing.Color.White;
            this.txtUsrSenhaConf.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsrSenhaConf.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtUsrSenhaConf.Location = new System.Drawing.Point(168, 140);
            this.txtUsrSenhaConf.Name = "txtUsrSenhaConf";
            this.txtUsrSenhaConf.PasswordChar = '*';
            this.txtUsrSenhaConf.Size = new System.Drawing.Size(280, 25);
            this.txtUsrSenhaConf.TabIndex = 3;
            // 
            // cmbUsrNivel
            // 
            this.cmbUsrNivel.BackColor = System.Drawing.Color.White;
            this.cmbUsrNivel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsrNivel.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.cmbUsrNivel.Items.AddRange(new object[] {
            "Operador",
            "Gerente",
            "Admin"});
            this.cmbUsrNivel.Location = new System.Drawing.Point(168, 180);
            this.cmbUsrNivel.Name = "cmbUsrNivel";
            this.cmbUsrNivel.Size = new System.Drawing.Size(280, 25);
            this.cmbUsrNivel.TabIndex = 4;
            // 
            // cmbUsrSit
            // 
            this.cmbUsrSit.BackColor = System.Drawing.Color.White;
            this.cmbUsrSit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsrSit.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.cmbUsrSit.Items.AddRange(new object[] {
            "Ativo",
            "Inativo"});
            this.cmbUsrSit.Location = new System.Drawing.Point(168, 220);
            this.cmbUsrSit.Name = "cmbUsrSit";
            this.cmbUsrSit.Size = new System.Drawing.Size(280, 25);
            this.cmbUsrSit.TabIndex = 5;
            // 
            // pnlBuscaUsuarios
            // 
            this.pnlBuscaUsuarios.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(31)))), ((int)(((byte)(58)))));
            this.pnlBuscaUsuarios.Controls.Add(this.lstUsuarios);
            this.pnlBuscaUsuarios.Controls.Add(this.txtPesquisa);
            this.pnlBuscaUsuarios.Location = new System.Drawing.Point(0, 430);
            this.pnlBuscaUsuarios.Name = "pnlBuscaUsuarios";
            this.pnlBuscaUsuarios.Size = new System.Drawing.Size(600, 170);
            this.pnlBuscaUsuarios.TabIndex = 22;
            this.pnlBuscaUsuarios.Visible = false;
            // 
            // lstUsuarios
            // 
            this.lstUsuarios.BackColor = System.Drawing.Color.White;
            this.lstUsuarios.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstUsuarios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstUsuarios.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.lstUsuarios.ItemHeight = 17;
            this.lstUsuarios.Location = new System.Drawing.Point(0, 25);
            this.lstUsuarios.Name = "lstUsuarios";
            this.lstUsuarios.Size = new System.Drawing.Size(600, 145);
            this.lstUsuarios.TabIndex = 0;
            this.lstUsuarios.DoubleClick += new System.EventHandler(this.LstUsuarios_DoubleClick);
            // 
            // txtPesquisa
            // 
            this.txtPesquisa.BackColor = System.Drawing.Color.White;
            this.txtPesquisa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPesquisa.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtPesquisa.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtPesquisa.Location = new System.Drawing.Point(0, 0);
            this.txtPesquisa.Name = "txtPesquisa";
            this.txtPesquisa.Size = new System.Drawing.Size(460, 25);
            this.txtPesquisa.TabIndex = 1;
            this.txtPesquisa.TextChanged += new System.EventHandler(this.TxtPesquisa_TextChanged);
            // 
            // btnNovoUsr
            // 
            this.btnNovoUsr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnNovoUsr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNovoUsr.FlatAppearance.BorderSize = 0;
            this.btnNovoUsr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNovoUsr.ForeColor = System.Drawing.Color.White;
            this.btnNovoUsr.Location = new System.Drawing.Point(168, 396);
            this.btnNovoUsr.Name = "btnNovoUsr";
            this.btnNovoUsr.Size = new System.Drawing.Size(128, 32);
            this.btnNovoUsr.TabIndex = 18;
            this.btnNovoUsr.Text = "+ Novo";
            this.btnNovoUsr.UseVisualStyleBackColor = false;
            this.btnNovoUsr.Click += new System.EventHandler(this.BtnNovoUsuario_Click);
            // 
            // btnPesquisarUsr
            // 
            this.btnPesquisarUsr.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.btnPesquisarUsr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPesquisarUsr.FlatAppearance.BorderSize = 0;
            this.btnPesquisarUsr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPesquisarUsr.ForeColor = System.Drawing.Color.White;
            this.btnPesquisarUsr.Location = new System.Drawing.Point(304, 396);
            this.btnPesquisarUsr.Name = "btnPesquisarUsr";
            this.btnPesquisarUsr.Size = new System.Drawing.Size(128, 32);
            this.btnPesquisarUsr.TabIndex = 19;
            this.btnPesquisarUsr.Text = "Pesquisar";
            this.btnPesquisarUsr.UseVisualStyleBackColor = false;
            this.btnPesquisarUsr.Click += new System.EventHandler(this.BtnPesquisarUsuario_Click);
            // 
            // btnSalvUsr
            // 
            this.btnSalvUsr.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnSalvUsr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalvUsr.FlatAppearance.BorderSize = 0;
            this.btnSalvUsr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvUsr.ForeColor = System.Drawing.Color.White;
            this.btnSalvUsr.Location = new System.Drawing.Point(168, 396);
            this.btnSalvUsr.Name = "btnSalvUsr";
            this.btnSalvUsr.Size = new System.Drawing.Size(128, 32);
            this.btnSalvUsr.TabIndex = 20;
            this.btnSalvUsr.Text = "Salvar";
            this.btnSalvUsr.UseVisualStyleBackColor = false;
            this.btnSalvUsr.Visible = false;
            this.btnSalvUsr.Click += new System.EventHandler(this.BtnSalvarUsuario_Click);
            // 
            // btnCancelarUsr
            // 
            this.btnCancelarUsr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnCancelarUsr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelarUsr.FlatAppearance.BorderSize = 0;
            this.btnCancelarUsr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelarUsr.ForeColor = System.Drawing.Color.White;
            this.btnCancelarUsr.Location = new System.Drawing.Point(304, 396);
            this.btnCancelarUsr.Name = "btnCancelarUsr";
            this.btnCancelarUsr.Size = new System.Drawing.Size(128, 32);
            this.btnCancelarUsr.TabIndex = 21;
            this.btnCancelarUsr.Text = "Cancelar";
            this.btnCancelarUsr.UseVisualStyleBackColor = false;
            this.btnCancelarUsr.Visible = false;
            this.btnCancelarUsr.Click += new System.EventHandler(this.BtnCancelarUsuario_Click);
            // ── Módulos Liberados — grade 4 colunas × 4 linhas ───────────────
            // Linha 1 (y=260): Dashboard | Pedidos | Financeiro | Turno de Caixa
            // Linha 2 (y=286): Produtos | Categorias | Clientes | Estoque
            // Linha 3 (y=312): Fornecedores | Cupons | Empresa | Ent. Mercadoria
            // Linha 4 (y=338): Cancelar Pedidos | Avisos | Consultar Pedido | Fidelização
            // 
            // chkModDashboard
            // 
            this.chkModDashboard.Location = new System.Drawing.Point(168, 260);
            this.chkModDashboard.Name = "chkModDashboard";
            this.chkModDashboard.Size = new System.Drawing.Size(164, 22);
            this.chkModDashboard.TabIndex = 6;
            this.chkModDashboard.Text = "Dashboard";
            // 
            // chkModPedidos
            // 
            this.chkModPedidos.Location = new System.Drawing.Point(342, 260);
            this.chkModPedidos.Name = "chkModPedidos";
            this.chkModPedidos.Size = new System.Drawing.Size(164, 22);
            this.chkModPedidos.TabIndex = 7;
            this.chkModPedidos.Text = "Pedidos";
            // 
            // chkModFinanceiro
            // 
            this.chkModFinanceiro.Location = new System.Drawing.Point(516, 260);
            this.chkModFinanceiro.Name = "chkModFinanceiro";
            this.chkModFinanceiro.Size = new System.Drawing.Size(164, 22);
            this.chkModFinanceiro.TabIndex = 8;
            this.chkModFinanceiro.Text = "Financeiro";
            // 
            // chkModTurno
            // 
            this.chkModTurno.Location = new System.Drawing.Point(690, 260);
            this.chkModTurno.Name = "chkModTurno";
            this.chkModTurno.Size = new System.Drawing.Size(164, 22);
            this.chkModTurno.TabIndex = 24;
            this.chkModTurno.Text = "Turno de Caixa";
            this.chkModTurno.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            // 
            // chkModProdutos
            // 
            this.chkModProdutos.Location = new System.Drawing.Point(168, 286);
            this.chkModProdutos.Name = "chkModProdutos";
            this.chkModProdutos.Size = new System.Drawing.Size(164, 22);
            this.chkModProdutos.TabIndex = 9;
            this.chkModProdutos.Text = "Produtos";
            // 
            // chkModCategorias
            // 
            this.chkModCategorias.Location = new System.Drawing.Point(342, 286);
            this.chkModCategorias.Name = "chkModCategorias";
            this.chkModCategorias.Size = new System.Drawing.Size(164, 22);
            this.chkModCategorias.TabIndex = 10;
            this.chkModCategorias.Text = "Categorias";
            // 
            // chkModClientes
            // 
            this.chkModClientes.Location = new System.Drawing.Point(516, 286);
            this.chkModClientes.Name = "chkModClientes";
            this.chkModClientes.Size = new System.Drawing.Size(164, 22);
            this.chkModClientes.TabIndex = 11;
            this.chkModClientes.Text = "Clientes";
            // 
            // chkModEstoque
            // 
            this.chkModEstoque.Location = new System.Drawing.Point(690, 286);
            this.chkModEstoque.Name = "chkModEstoque";
            this.chkModEstoque.Size = new System.Drawing.Size(164, 22);
            this.chkModEstoque.TabIndex = 25;
            this.chkModEstoque.Text = "Estoque";
            this.chkModEstoque.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            // 
            // chkModFornecedores
            // 
            this.chkModFornecedores.Location = new System.Drawing.Point(168, 312);
            this.chkModFornecedores.Name = "chkModFornecedores";
            this.chkModFornecedores.Size = new System.Drawing.Size(164, 22);
            this.chkModFornecedores.TabIndex = 12;
            this.chkModFornecedores.Text = "Fornecedores";
            // 
            // chkModCupons
            // 
            this.chkModCupons.Location = new System.Drawing.Point(342, 312);
            this.chkModCupons.Name = "chkModCupons";
            this.chkModCupons.Size = new System.Drawing.Size(164, 22);
            this.chkModCupons.TabIndex = 13;
            this.chkModCupons.Text = "Cupons";
            // 
            // chkModEmpresa
            // 
            this.chkModEmpresa.Location = new System.Drawing.Point(516, 312);
            this.chkModEmpresa.Name = "chkModEmpresa";
            this.chkModEmpresa.Size = new System.Drawing.Size(164, 22);
            this.chkModEmpresa.TabIndex = 14;
            this.chkModEmpresa.Text = "Empresa";
            // 
            // chkModEntradaMercadoria
            // 
            this.chkModEntradaMercadoria.Location = new System.Drawing.Point(690, 312);
            this.chkModEntradaMercadoria.Name = "chkModEntradaMercadoria";
            this.chkModEntradaMercadoria.Size = new System.Drawing.Size(164, 22);
            this.chkModEntradaMercadoria.TabIndex = 16;
            this.chkModEntradaMercadoria.Text = "Entrada Mercadoria";
            // 
            // chkModCancelarPedidos
            // 
            this.chkModCancelarPedidos.Location = new System.Drawing.Point(168, 338);
            this.chkModCancelarPedidos.Name = "chkModCancelarPedidos";
            this.chkModCancelarPedidos.Size = new System.Drawing.Size(164, 22);
            this.chkModCancelarPedidos.TabIndex = 15;
            this.chkModCancelarPedidos.Text = "Cancelar Pedidos";
            // 
            // chkModAvisos
            // 
            this.chkModAvisos.Location = new System.Drawing.Point(342, 338);
            this.chkModAvisos.Name = "chkModAvisos";
            this.chkModAvisos.Size = new System.Drawing.Size(164, 22);
            this.chkModAvisos.TabIndex = 17;
            this.chkModAvisos.Text = "Avisos";
            // 
            // chkModConsultarPedido
            // 
            this.chkModConsultarPedido.Location = new System.Drawing.Point(516, 338);
            this.chkModConsultarPedido.Name = "chkModConsultarPedido";
            this.chkModConsultarPedido.Size = new System.Drawing.Size(164, 22);
            this.chkModConsultarPedido.TabIndex = 26;
            this.chkModConsultarPedido.Text = "Consultar Pedido";
            this.chkModConsultarPedido.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            // 
            // chkModFidelizacao
            // 
            this.chkModFidelizacao.Location = new System.Drawing.Point(690, 338);
            this.chkModFidelizacao.Name = "chkModFidelizacao";
            this.chkModFidelizacao.Size = new System.Drawing.Size(164, 22);
            this.chkModFidelizacao.TabIndex = 27;
            this.chkModFidelizacao.Text = "Fidelização";
            this.chkModFidelizacao.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            // 
            // chkModWhatsApp
            // 
            this.chkModWhatsApp.Location = new System.Drawing.Point(168, 364);
            // 
            // chkModBairros
            // 
            this.chkModBairros.Location = new System.Drawing.Point(342, 364);
            this.chkModBairros.Name = "chkModBairros";
            this.chkModBairros.Size = new System.Drawing.Size(164, 22);
            this.chkModBairros.TabIndex = 29;
            this.chkModBairros.Text = "Bairros / Taxa";
            this.chkModBairros.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.chkModWhatsApp.Name = "chkModWhatsApp";
            this.chkModWhatsApp.Size = new System.Drawing.Size(164, 22);
            this.chkModWhatsApp.TabIndex = 28;
            this.chkModWhatsApp.Text = "WhatsApp";
            this.chkModWhatsApp.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            // 
            // chkModMarmitas
            // 
            this.chkModMarmitas.Location = new System.Drawing.Point(516, 364);
            this.chkModMarmitas.Name = "chkModMarmitas";
            this.chkModMarmitas.Size = new System.Drawing.Size(164, 22);
            this.chkModMarmitas.TabIndex = 30;
            this.chkModMarmitas.Text = "Marmitas";
            this.chkModMarmitas.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            // 
            // txtImpNomeEmpresa
            // 
            this.txtImpNomeEmpresa.BackColor = System.Drawing.Color.White;
            this.txtImpNomeEmpresa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpNomeEmpresa.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpNomeEmpresa.Location = new System.Drawing.Point(208, 30);
            this.txtImpNomeEmpresa.Name = "txtImpNomeEmpresa";
            this.txtImpNomeEmpresa.Size = new System.Drawing.Size(400, 25);
            this.txtImpNomeEmpresa.TabIndex = 0;
            // 
            // txtImpEndereco
            // 
            this.txtImpEndereco.BackColor = System.Drawing.Color.White;
            this.txtImpEndereco.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpEndereco.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpEndereco.Location = new System.Drawing.Point(208, 62);
            this.txtImpEndereco.Name = "txtImpEndereco";
            this.txtImpEndereco.Size = new System.Drawing.Size(400, 25);
            this.txtImpEndereco.TabIndex = 1;
            // 
            // txtImpTelefone
            // 
            this.txtImpTelefone.BackColor = System.Drawing.Color.White;
            this.txtImpTelefone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpTelefone.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpTelefone.Location = new System.Drawing.Point(208, 94);
            this.txtImpTelefone.Name = "txtImpTelefone";
            this.txtImpTelefone.Size = new System.Drawing.Size(400, 25);
            this.txtImpTelefone.TabIndex = 2;
            // 
            // txtImpCNPJ
            // 
            this.txtImpCNPJ.BackColor = System.Drawing.Color.White;
            this.txtImpCNPJ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpCNPJ.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpCNPJ.Location = new System.Drawing.Point(208, 126);
            this.txtImpCNPJ.Name = "txtImpCNPJ";
            this.txtImpCNPJ.Size = new System.Drawing.Size(400, 25);
            this.txtImpCNPJ.TabIndex = 3;
            // 
            // txtImpSeparador
            // 
            this.txtImpSeparador.BackColor = System.Drawing.Color.White;
            this.txtImpSeparador.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpSeparador.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpSeparador.Location = new System.Drawing.Point(208, 158);
            this.txtImpSeparador.Name = "txtImpSeparador";
            this.txtImpSeparador.Size = new System.Drawing.Size(400, 25);
            this.txtImpSeparador.TabIndex = 4;
            // 
            // txtImpAvisoFiscal
            // 
            this.txtImpAvisoFiscal.BackColor = System.Drawing.Color.White;
            this.txtImpAvisoFiscal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpAvisoFiscal.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpAvisoFiscal.Location = new System.Drawing.Point(208, 190);
            this.txtImpAvisoFiscal.Name = "txtImpAvisoFiscal";
            this.txtImpAvisoFiscal.Size = new System.Drawing.Size(400, 25);
            this.txtImpAvisoFiscal.TabIndex = 5;
            // 
            // txtImpRodapeTexto
            // 
            this.txtImpRodapeTexto.BackColor = System.Drawing.Color.White;
            this.txtImpRodapeTexto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpRodapeTexto.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpRodapeTexto.Location = new System.Drawing.Point(208, 222);
            this.txtImpRodapeTexto.Multiline = true;
            this.txtImpRodapeTexto.Name = "txtImpRodapeTexto";
            this.txtImpRodapeTexto.Size = new System.Drawing.Size(400, 64);
            this.txtImpRodapeTexto.TabIndex = 6;
            // 
            // txtImpLblNumero
            // 
            this.txtImpLblNumero.BackColor = System.Drawing.Color.White;
            this.txtImpLblNumero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblNumero.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpLblNumero.Location = new System.Drawing.Point(208, 300);
            this.txtImpLblNumero.Name = "txtImpLblNumero";
            this.txtImpLblNumero.Size = new System.Drawing.Size(200, 25);
            this.txtImpLblNumero.TabIndex = 7;
            // 
            // txtImpLblColItem
            // 
            this.txtImpLblColItem.BackColor = System.Drawing.Color.White;
            this.txtImpLblColItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblColItem.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpLblColItem.Location = new System.Drawing.Point(208, 332);
            this.txtImpLblColItem.Name = "txtImpLblColItem";
            this.txtImpLblColItem.Size = new System.Drawing.Size(200, 25);
            this.txtImpLblColItem.TabIndex = 8;
            // 
            // txtImpLblColTotal
            // 
            this.txtImpLblColTotal.BackColor = System.Drawing.Color.White;
            this.txtImpLblColTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblColTotal.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpLblColTotal.Location = new System.Drawing.Point(208, 364);
            this.txtImpLblColTotal.Name = "txtImpLblColTotal";
            this.txtImpLblColTotal.Size = new System.Drawing.Size(200, 25);
            this.txtImpLblColTotal.TabIndex = 9;
            // 
            // txtImpLblSubtotal
            // 
            this.txtImpLblSubtotal.BackColor = System.Drawing.Color.White;
            this.txtImpLblSubtotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblSubtotal.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpLblSubtotal.Location = new System.Drawing.Point(208, 396);
            this.txtImpLblSubtotal.Name = "txtImpLblSubtotal";
            this.txtImpLblSubtotal.Size = new System.Drawing.Size(200, 25);
            this.txtImpLblSubtotal.TabIndex = 10;
            // 
            // txtImpLblTaxa
            // 
            this.txtImpLblTaxa.BackColor = System.Drawing.Color.White;
            this.txtImpLblTaxa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblTaxa.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpLblTaxa.Location = new System.Drawing.Point(208, 428);
            this.txtImpLblTaxa.Name = "txtImpLblTaxa";
            this.txtImpLblTaxa.Size = new System.Drawing.Size(200, 25);
            this.txtImpLblTaxa.TabIndex = 11;
            // 
            // txtImpLblTotalPagar
            // 
            this.txtImpLblTotalPagar.BackColor = System.Drawing.Color.White;
            this.txtImpLblTotalPagar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblTotalPagar.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpLblTotalPagar.Location = new System.Drawing.Point(208, 524);
            this.txtImpLblTotalPagar.Name = "txtImpLblTotalPagar";
            this.txtImpLblTotalPagar.Size = new System.Drawing.Size(200, 25);
            this.txtImpLblTotalPagar.TabIndex = 14;
            // 
            // txtImpLblDesconto
            // 
            this.txtImpLblDesconto.BackColor = System.Drawing.Color.White;
            this.txtImpLblDesconto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblDesconto.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpLblDesconto.Location = new System.Drawing.Point(208, 460);
            this.txtImpLblDesconto.Name = "txtImpLblDesconto";
            this.txtImpLblDesconto.Size = new System.Drawing.Size(200, 25);
            this.txtImpLblDesconto.TabIndex = 12;
            // 
            // txtImpLblCupom
            // 
            this.txtImpLblCupom.BackColor = System.Drawing.Color.White;
            this.txtImpLblCupom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblCupom.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpLblCupom.Location = new System.Drawing.Point(208, 492);
            this.txtImpLblCupom.Name = "txtImpLblCupom";
            this.txtImpLblCupom.Size = new System.Drawing.Size(200, 25);
            this.txtImpLblCupom.TabIndex = 13;
            // 
            // txtImpLblAtendente
            // 
            this.txtImpLblAtendente.BackColor = System.Drawing.Color.White;
            this.txtImpLblAtendente.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtImpLblAtendente.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.txtImpLblAtendente.Location = new System.Drawing.Point(208, 556);
            this.txtImpLblAtendente.Name = "txtImpLblAtendente";
            this.txtImpLblAtendente.Size = new System.Drawing.Size(200, 25);
            this.txtImpLblAtendente.TabIndex = 15;
            // 
            // cmbImpressora
            // 
            this.cmbImpressora.BackColor = System.Drawing.Color.White;
            this.cmbImpressora.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImpressora.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.cmbImpressora.Items.AddRange(new object[] {
            "(Impressora padr�o do sistema)"});
            this.cmbImpressora.Location = new System.Drawing.Point(208, 594);
            this.cmbImpressora.Name = "cmbImpressora";
            this.cmbImpressora.Size = new System.Drawing.Size(400, 25);
            this.cmbImpressora.TabIndex = 14;
            // 
            // numLargura
            // 
            this.numLargura.BackColor = System.Drawing.Color.White;
            this.numLargura.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.numLargura.Location = new System.Drawing.Point(208, 626);
            this.numLargura.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numLargura.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numLargura.Name = "numLargura";
            this.numLargura.Size = new System.Drawing.Size(80, 25);
            this.numLargura.TabIndex = 15;
            this.numLargura.Value = new decimal(new int[] {
            42,
            0,
            0,
            0});
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabEmp);
            this.tabControl.Controls.Add(this.tabUsr);
            this.tabControl.Controls.Add(this.tabImp);
            this.tabControl.Controls.Add(this.tabSistema);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(14, 5);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1004, 676);
            this.tabControl.TabIndex = 0;
            // 
            // tabEmp
            // 
            this.tabEmp.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.tabEmp.Controls.Add(this.txtEmpNome);
            this.tabEmp.Controls.Add(this.txtEmpFantasia);
            this.tabEmp.Controls.Add(this.txtEmpCNPJ);
            this.tabEmp.Controls.Add(this.txtEmpTel);
            this.tabEmp.Controls.Add(this.txtEmpEmail);
            this.tabEmp.Controls.Add(this.txtEmpEnd);
            this.tabEmp.Controls.Add(this.txtEmpCodigo);
            this.tabEmp.Controls.Add(this.txtEmpImgBB);
            this.tabEmp.Controls.Add(this.btnSalvEmp);
            this.tabEmp.Controls.Add(this.btnSincronizarSite);
            this.tabEmp.Controls.Add(this.btnDiagnostico);
            this.tabEmp.Controls.Add(this.lblSincStatus);
            this.tabEmp.Controls.Add(this.lblEmpNome);
            this.tabEmp.Controls.Add(this.lblEmpFantasia);
            this.tabEmp.Controls.Add(this.lblEmpCNPJ);
            this.tabEmp.Controls.Add(this.lblEmpTel);
            this.tabEmp.Controls.Add(this.lblEmpEmail);
            this.tabEmp.Controls.Add(this.lblEmpEnd);
            this.tabEmp.Controls.Add(this.lblEmpCodigo);
            this.tabEmp.Controls.Add(this.lblEmpImgBB);
            this.tabEmp.Controls.Add(this.lblEmpTitle);
            this.tabEmp.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.tabEmp.Location = new System.Drawing.Point(4, 30);
            this.tabEmp.Name = "tabEmp";
            this.tabEmp.Size = new System.Drawing.Size(996, 607);
            this.tabEmp.TabIndex = 0;
            this.tabEmp.Text = "  Dados da Empresa  ";
            // 
            // btnSalvEmp
            // 
            this.btnSalvEmp.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnSalvEmp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalvEmp.FlatAppearance.BorderSize = 0;
            this.btnSalvEmp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvEmp.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSalvEmp.ForeColor = System.Drawing.Color.White;
            this.btnSalvEmp.Location = new System.Drawing.Point(168, 348);
            this.btnSalvEmp.Name = "btnSalvEmp";
            this.btnSalvEmp.Size = new System.Drawing.Size(160, 32);
            this.btnSalvEmp.TabIndex = 6;
            this.btnSalvEmp.Text = "✓  Salvar Empresa";
            this.btnSalvEmp.UseVisualStyleBackColor = false;
            this.btnSalvEmp.Click += new System.EventHandler(this.BtnSalvarEmpresa_Click);
            // 
            // btnSincronizarSite
            // 
            this.btnSincronizarSite.BackColor = System.Drawing.Color.FromArgb(30, 100, 180);
            this.btnSincronizarSite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSincronizarSite.FlatAppearance.BorderSize = 0;
            this.btnSincronizarSite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSincronizarSite.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSincronizarSite.ForeColor = System.Drawing.Color.White;
            this.btnSincronizarSite.Location = new System.Drawing.Point(340, 348);
            this.btnSincronizarSite.Name = "btnSincronizarSite";
            this.btnSincronizarSite.Size = new System.Drawing.Size(220, 32);
            this.btnSincronizarSite.TabIndex = 7;
            this.btnSincronizarSite.Text = "☁  Enviar Tudo ao Site";
            this.btnSincronizarSite.UseVisualStyleBackColor = false;
            this.btnSincronizarSite.Click += new System.EventHandler(this.BtnSincronizarSite_Click);
            // 
            // btnDiagnostico
            // 
            this.btnDiagnostico.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.btnDiagnostico.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDiagnostico.FlatAppearance.BorderSize = 0;
            this.btnDiagnostico.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDiagnostico.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.btnDiagnostico.ForeColor = System.Drawing.Color.White;
            this.btnDiagnostico.Location = new System.Drawing.Point(572, 348);
            this.btnDiagnostico.Name = "btnDiagnostico";
            this.btnDiagnostico.Size = new System.Drawing.Size(160, 32);
            this.btnDiagnostico.TabIndex = 8;
            this.btnDiagnostico.Text = "🔍  Testar Conexão";
            this.btnDiagnostico.UseVisualStyleBackColor = false;
            this.btnDiagnostico.Click += new System.EventHandler(this.BtnDiagnostico_Click);
            // 
            // lblSincStatus
            // 
            this.lblSincStatus.AutoSize = false;
            this.lblSincStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblSincStatus.ForeColor = System.Drawing.Color.FromArgb(30, 100, 180);
            this.lblSincStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblSincStatus.Location = new System.Drawing.Point(168, 388);
            this.lblSincStatus.Name = "lblSincStatus";
            this.lblSincStatus.Size = new System.Drawing.Size(392, 22);
            this.lblSincStatus.TabIndex = 99;
            this.lblSincStatus.Text = "";
            this.lblSincStatus.Visible = false;
            // 
            // tabUsr
            // 
            this.tabUsr.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.tabUsr.Controls.Add(this.txtUsrNome);
            this.tabUsr.Controls.Add(this.txtUsrLogin);
            this.tabUsr.Controls.Add(this.txtUsrSenha);
            this.tabUsr.Controls.Add(this.txtUsrSenhaConf);
            this.tabUsr.Controls.Add(this.cmbUsrNivel);
            this.tabUsr.Controls.Add(this.cmbUsrSit);
            this.tabUsr.Controls.Add(this.chkModDashboard);
            this.tabUsr.Controls.Add(this.chkModPedidos);
            this.tabUsr.Controls.Add(this.chkModFinanceiro);
            this.tabUsr.Controls.Add(this.chkModProdutos);
            this.tabUsr.Controls.Add(this.chkModCategorias);
            this.tabUsr.Controls.Add(this.chkModClientes);
            this.tabUsr.Controls.Add(this.chkModFornecedores);
            this.tabUsr.Controls.Add(this.chkModCupons);
            this.tabUsr.Controls.Add(this.chkModEstoque);
            this.tabUsr.Controls.Add(this.chkModEmpresa);
            this.tabUsr.Controls.Add(this.chkModCancelarPedidos);
            this.tabUsr.Controls.Add(this.chkModEntradaMercadoria);
            this.tabUsr.Controls.Add(this.chkModAvisos);
            this.tabUsr.Controls.Add(this.chkModTurno);
            this.tabUsr.Controls.Add(this.chkModConsultarPedido);
            this.tabUsr.Controls.Add(this.chkModFidelizacao);
            this.tabUsr.Controls.Add(this.chkModWhatsApp);
            this.tabUsr.Controls.Add(this.chkModBairros);
            this.tabUsr.Controls.Add(this.chkModMarmitas);
            this.tabUsr.Controls.Add(this.btnNovoUsr);
            this.tabUsr.Controls.Add(this.btnPesquisarUsr);
            this.tabUsr.Controls.Add(this.btnSalvUsr);
            this.tabUsr.Controls.Add(this.btnCancelarUsr);
            this.tabUsr.Controls.Add(this.pnlBuscaUsuarios);
            this.tabUsr.Controls.Add(this.lblUsrNome);
            this.tabUsr.Controls.Add(this.lblUsrLogin);
            this.tabUsr.Controls.Add(this.lblUsrSenha);
            this.tabUsr.Controls.Add(this.lblUsrSenhaConf);
            this.tabUsr.Controls.Add(this.lblUsrNivel);
            this.tabUsr.Controls.Add(this.lblUsrSit);
            this.tabUsr.Controls.Add(this.lblUsrModulos);
            this.tabUsr.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.tabUsr.Location = new System.Drawing.Point(4, 30);
            this.tabUsr.Name = "tabUsr";
            this.tabUsr.Size = new System.Drawing.Size(996, 607);
            this.tabUsr.TabIndex = 1;
            this.tabUsr.Text = "  Usuários  ";
            // 
            // tabImp
            // 
            this.tabImp.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.tabImp.Controls.Add(this.txtImpNomeEmpresa);
            this.tabImp.Controls.Add(this.txtImpEndereco);
            this.tabImp.Controls.Add(this.txtImpTelefone);
            this.tabImp.Controls.Add(this.txtImpCNPJ);
            this.tabImp.Controls.Add(this.txtImpSeparador);
            this.tabImp.Controls.Add(this.txtImpAvisoFiscal);
            this.tabImp.Controls.Add(this.txtImpRodapeTexto);
            this.tabImp.Controls.Add(this.txtImpLblNumero);
            this.tabImp.Controls.Add(this.txtImpLblColItem);
            this.tabImp.Controls.Add(this.txtImpLblColTotal);
            this.tabImp.Controls.Add(this.txtImpLblSubtotal);
            this.tabImp.Controls.Add(this.txtImpLblTaxa);
            this.tabImp.Controls.Add(this.txtImpLblDesconto);
            this.tabImp.Controls.Add(this.txtImpLblCupom);
            this.tabImp.Controls.Add(this.txtImpLblTotalPagar);
            this.tabImp.Controls.Add(this.txtImpLblAtendente);
            this.tabImp.Controls.Add(this.cmbImpressora);
            this.tabImp.Controls.Add(this.numLargura);
            this.tabImp.Controls.Add(this.btnSalvImp);
            this.tabImp.Controls.Add(this.btnTesteImp);
            this.tabImp.Controls.Add(this.lblImpNome);
            this.tabImp.Controls.Add(this.lblImpEnd);
            this.tabImp.Controls.Add(this.lblImpTel);
            this.tabImp.Controls.Add(this.lblImpCNPJ);
            this.tabImp.Controls.Add(this.lblImpSep);
            this.tabImp.Controls.Add(this.lblImpAviso);
            this.tabImp.Controls.Add(this.lblImpRodape);
            this.tabImp.Controls.Add(this.lblImpNum);
            this.tabImp.Controls.Add(this.lblImpColItem);
            this.tabImp.Controls.Add(this.lblImpColTotal);
            this.tabImp.Controls.Add(this.lblImpSubtotal);
            this.tabImp.Controls.Add(this.lblImpTaxa);
            this.tabImp.Controls.Add(this.lblImpDesconto);
            this.tabImp.Controls.Add(this.lblImpCupom);
            this.tabImp.Controls.Add(this.lblImpTotalPagar);
            this.tabImp.Controls.Add(this.lblImpAtendente);
            this.tabImp.Controls.Add(this.lblImpressora);
            this.tabImp.Controls.Add(this.lblLargura);
            this.tabImp.Controls.Add(this.lblImpTitle);
            this.tabImp.Controls.Add(this.lblImpRotulos);
            this.tabImp.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.tabImp.Location = new System.Drawing.Point(4, 30);
            this.tabImp.Name = "tabImp";
            this.tabImp.Size = new System.Drawing.Size(996, 642);
            this.tabImp.TabIndex = 2;
            this.tabImp.Text = "  Impressão  ";
            // 
            // btnSalvImp
            // 
            this.btnSalvImp.BackColor = System.Drawing.Color.FromArgb(87, 120, 38);
            this.btnSalvImp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalvImp.FlatAppearance.BorderSize = 0;
            this.btnSalvImp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvImp.ForeColor = System.Drawing.Color.White;
            this.btnSalvImp.Location = new System.Drawing.Point(208, 664);
            this.btnSalvImp.Name = "btnSalvImp";
            this.btnSalvImp.Size = new System.Drawing.Size(200, 32);
            this.btnSalvImp.TabIndex = 16;
            this.btnSalvImp.Text = "✓  Salvar Configuração";
            this.btnSalvImp.UseVisualStyleBackColor = false;
            this.btnSalvImp.Click += new System.EventHandler(this.BtnSalvarImpressao_Click);
            // 
            // btnTesteImp
            // 
            this.btnTesteImp.BackColor = System.Drawing.Color.FromArgb(224, 113, 42);
            this.btnTesteImp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTesteImp.FlatAppearance.BorderSize = 0;
            this.btnTesteImp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTesteImp.ForeColor = System.Drawing.Color.White;
            this.btnTesteImp.Location = new System.Drawing.Point(416, 664);
            this.btnTesteImp.Name = "btnTesteImp";
            this.btnTesteImp.Size = new System.Drawing.Size(140, 32);
            this.btnTesteImp.TabIndex = 17;
            this.btnTesteImp.Text = "Imprimir Teste";
            this.btnTesteImp.UseVisualStyleBackColor = false;
            this.btnTesteImp.Click += new System.EventHandler(this.BtnImprimirTeste_Click);
            // 
            // tabSistema
            // 
            this.tabSistema.Controls.Add(this.lblResetTitulo);
            this.tabSistema.Controls.Add(this.lblResetAviso);
            this.tabSistema.Controls.Add(this.btnResetarBanco);
            this.tabSistema.BackColor = System.Drawing.Color.FromArgb(245, 237, 216);
            this.tabSistema.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.tabSistema.Location = new System.Drawing.Point(4, 30);
            this.tabSistema.Name = "tabSistema";
            this.tabSistema.Size = new System.Drawing.Size(996, 642);
            this.tabSistema.TabIndex = 3;
            this.tabSistema.Text = "  Sistema  ";
            // 
            // lblResetTitulo
            // 
            this.lblResetTitulo.AutoSize = true;
            this.lblResetTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblResetTitulo.ForeColor = System.Drawing.Color.FromArgb(176, 110, 42);
            this.lblResetTitulo.Location = new System.Drawing.Point(30, 30);
            this.lblResetTitulo.Name = "lblResetTitulo";
            this.lblResetTitulo.Text = "Zona de Perigo";
            // 
            // lblResetAviso
            // 
            this.lblResetAviso.AutoSize = false;
            this.lblResetAviso.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblResetAviso.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblResetAviso.Location = new System.Drawing.Point(30, 70);
            this.lblResetAviso.Name = "lblResetAviso";
            this.lblResetAviso.Size = new System.Drawing.Size(600, 80);
            this.lblResetAviso.Text = "Reseta completamente o banco de dados apagando TODOS os registros, incluindo usuários.\r\nApós o reset, o único acesso disponível será: login 'admin' com senha '$up0rte'.\r\nEsta operação é irreversível.";
            // 
            // btnResetarBanco
            // 
            this.btnResetarBanco.BackColor = System.Drawing.Color.FromArgb(180, 40, 40);
            this.btnResetarBanco.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResetarBanco.FlatAppearance.BorderSize = 0;
            this.btnResetarBanco.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetarBanco.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnResetarBanco.ForeColor = System.Drawing.Color.White;
            this.btnResetarBanco.Location = new System.Drawing.Point(30, 165);
            this.btnResetarBanco.Name = "btnResetarBanco";
            this.btnResetarBanco.Size = new System.Drawing.Size(260, 40);
            this.btnResetarBanco.TabIndex = 0;
            this.btnResetarBanco.Text = "⚠  Resetar Banco de Dados";
            this.btnResetarBanco.UseVisualStyleBackColor = false;
            this.btnResetarBanco.Click += new System.EventHandler(this.BtnResetarBanco_Click);
            this.btnResetarBanco.Click += new System.EventHandler(this.BtnResetarBanco_Click);
            // 
            // frmEmpresa
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(19)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1004, 740);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.MinimumSize = new System.Drawing.Size(960, 620);
            this.Name = "frmEmpresa";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Empresa e Usuários";
            this.pnlBuscaUsuarios.ResumeLayout(false);
            this.pnlBuscaUsuarios.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLargura)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabEmp.ResumeLayout(false);
            this.tabEmp.PerformLayout();
            this.tabUsr.ResumeLayout(false);
            this.tabUsr.PerformLayout();
            this.tabImp.ResumeLayout(false);
            this.tabImp.PerformLayout();
            this.tabSistema.ResumeLayout(false);
            this.tabSistema.PerformLayout();
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
        internal System.Windows.Forms.CheckBox chkModEstoque;
        internal System.Windows.Forms.CheckBox chkModAvisos;
        internal System.Windows.Forms.CheckBox chkModTurno;
        internal System.Windows.Forms.CheckBox chkModConsultarPedido;
        internal System.Windows.Forms.CheckBox chkModFidelizacao;
        internal System.Windows.Forms.CheckBox chkModWhatsApp;
        internal System.Windows.Forms.CheckBox chkModBairros;
        internal System.Windows.Forms.CheckBox chkModMarmitas;
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
        internal System.Windows.Forms.TextBox        txtImpLblDesconto;
        internal System.Windows.Forms.TextBox        txtImpLblCupom;
        internal System.Windows.Forms.TextBox        txtImpLblTotalPagar;
        internal System.Windows.Forms.TextBox        txtImpLblAtendente;
        internal System.Windows.Forms.ComboBox       cmbImpressora;
        internal System.Windows.Forms.NumericUpDown  numLargura;
        // Private helpers
        private System.Windows.Forms.TabControl     tabControl;
        private System.Windows.Forms.TabPage        tabEmp;
        private System.Windows.Forms.TabPage        tabUsr;
        private System.Windows.Forms.TabPage        tabImp;
        private System.Windows.Forms.TabPage        tabSistema;
        private System.Windows.Forms.Label          lblResetTitulo;
        private System.Windows.Forms.Label          lblResetAviso;
        internal System.Windows.Forms.Button        btnResetarBanco;
        private System.Windows.Forms.Button         btnSalvEmp;
        private System.Windows.Forms.Button         btnSincronizarSite;
        private System.Windows.Forms.Button         btnDiagnostico;
        private System.Windows.Forms.Label          lblSincStatus;
        private System.Windows.Forms.Button         btnSalvImp;
        private System.Windows.Forms.Button         btnTesteImp;
        // labels empresa
        private System.Windows.Forms.Label lblEmpTitle    = new System.Windows.Forms.Label { Text = "Dados da Empresa", AutoSize = true, Left = 22, Top = 18, Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.FromArgb(176,110,42) };
        private System.Windows.Forms.Label lblEmpNome     = new System.Windows.Forms.Label { Text = "Raz\u00e3o Social:",  AutoSize = true, Left = 22, Top = 92,  ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblEmpFantasia = new System.Windows.Forms.Label { Text = "Nome Fantasia:",  AutoSize = true, Left = 22, Top = 123, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblEmpCNPJ     = new System.Windows.Forms.Label { Text = "CNPJ:",          AutoSize = true, Left = 22, Top = 154, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblEmpTel      = new System.Windows.Forms.Label { Text = "Telefone:",      AutoSize = true, Left = 22, Top = 185, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblEmpEmail    = new System.Windows.Forms.Label { Text = "E-mail:",        AutoSize = true, Left = 22, Top = 216, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblEmpEnd      = new System.Windows.Forms.Label { Text = "Endereço:", AutoSize = true, Left = 22, Top = 247, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblEmpCodigo    = new System.Windows.Forms.Label { Text = "Código da Empresa:", AutoSize = true, Left = 22, Top = 278, ForeColor = System.Drawing.Color.FromArgb(120,60,10), Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold) };
        private System.Windows.Forms.Label lblEmpImgBB     = new System.Windows.Forms.Label { Text = "ImgBB API Key:", AutoSize = true, Left = 22, Top = 318, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        internal System.Windows.Forms.TextBox txtEmpCodigo;
        internal System.Windows.Forms.TextBox txtEmpImgBB;
        // labels usuario
        private System.Windows.Forms.Label lblUsrNome      = new System.Windows.Forms.Label { Text = "Nome:",              AutoSize = true, Left = 22, Top = 25,  ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblUsrLogin     = new System.Windows.Forms.Label { Text = "Login:",             AutoSize = true, Left = 22, Top = 65,  ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblUsrSenha     = new System.Windows.Forms.Label { Text = "Senha:",             AutoSize = true, Left = 22, Top = 105, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblUsrSenhaConf = new System.Windows.Forms.Label { Text = "Confirmar:",         AutoSize = true, Left = 22, Top = 145, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblUsrNivel     = new System.Windows.Forms.Label { Text = "N\u00edvel:",        AutoSize = true, Left = 22, Top = 185, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblUsrSit       = new System.Windows.Forms.Label { Text = "Situa\u00e7\u00e3o:", AutoSize = true, Left = 22, Top = 225, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblUsrModulos   = new System.Windows.Forms.Label { Text = "M\u00f3dulos Liberados:", AutoSize = true, Left = 22, Top = 248, Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.FromArgb(176,110,42) };
        // labels impressao
        private System.Windows.Forms.Label lblImpTitle    = new System.Windows.Forms.Label { Text = "Configura\u00e7\u00e3o de Impress\u00e3o", AutoSize = true, Left = 22, Top = 6, Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.FromArgb(176,110,42) };
        private System.Windows.Forms.Label lblImpRotulos  = new System.Windows.Forms.Label { Text = "R\u00f3tulos do Cupom:", AutoSize = true, Left = 22, Top = 290, Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.FromArgb(176,110,42) };
        private System.Windows.Forms.Label lblImpNome      = new System.Windows.Forms.Label { Text = "Nome (cabe\u00e7alho):", AutoSize = true, Left = 22, Top = 36,  ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpEnd       = new System.Windows.Forms.Label { Text = "Endere\u00e7o:",       AutoSize = true, Left = 22, Top = 68,  ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpTel       = new System.Windows.Forms.Label { Text = "Telefone:",           AutoSize = true, Left = 22, Top = 100, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpCNPJ      = new System.Windows.Forms.Label { Text = "CNPJ:",               AutoSize = true, Left = 22, Top = 132, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpSep       = new System.Windows.Forms.Label { Text = "Separador:",          AutoSize = true, Left = 22, Top = 164, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpAviso     = new System.Windows.Forms.Label { Text = "Aviso fiscal:",       AutoSize = true, Left = 22, Top = 196, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpRodape    = new System.Windows.Forms.Label { Text = "Rodap\u00e9:",        AutoSize = true, Left = 22, Top = 228, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpNum       = new System.Windows.Forms.Label { Text = "N\u00ba Pedido:",     AutoSize = true, Left = 22, Top = 306, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpColItem   = new System.Windows.Forms.Label { Text = "Col. Item:",          AutoSize = true, Left = 22, Top = 338, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpColTotal  = new System.Windows.Forms.Label { Text = "Col. Total:",         AutoSize = true, Left = 22, Top = 370, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpSubtotal  = new System.Windows.Forms.Label { Text = "Subtotal:",           AutoSize = true, Left = 22, Top = 402, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpTaxa      = new System.Windows.Forms.Label { Text = "Taxa entrega:",       AutoSize = true, Left = 22, Top = 434, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpDesconto  = new System.Windows.Forms.Label { Text = "Desconto:",            AutoSize = true, Left = 22, Top = 466, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpCupom     = new System.Windows.Forms.Label { Text = "Cupom:",               AutoSize = true, Left = 22, Top = 498, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpTotalPagar= new System.Windows.Forms.Label { Text = "Total a pagar:",       AutoSize = true, Left = 22, Top = 530, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpAtendente = new System.Windows.Forms.Label { Text = "Atendente:",           AutoSize = true, Left = 22, Top = 562, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblImpressora   = new System.Windows.Forms.Label { Text = "Impressora:",          AutoSize = true, Left = 22, Top = 600, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
        private System.Windows.Forms.Label lblLargura      = new System.Windows.Forms.Label { Text = "Largura (chars):",     AutoSize = true, Left = 22, Top = 632, ForeColor = System.Drawing.Color.FromArgb(70,70,70) };
    }
}
