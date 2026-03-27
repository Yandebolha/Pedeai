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
            this.Text            = "Empresa e Usuários";
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor       = Color.FromArgb(13, 19, 40);
            this.ForeColor       = Color.White;
            this.Font            = new Font("Segoe UI", 9.5F);
            this.MinimumSize     = new System.Drawing.Size(960, 620);
            this.Size            = new System.Drawing.Size(1020, 680);

            // â”€â”€ paleta â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            var cBg      = Color.FromArgb(13,  19,  40);
            var cCard    = Color.FromArgb(22,  31,  58);
            var cPanel   = Color.FromArgb(28,  39,  70);
            var cAccent  = Color.FromArgb(52, 152, 219);
            var cGreen   = Color.FromArgb(39, 174,  96);
            var cInput   = Color.FromArgb(18,  26,  50);
            var cBorder  = Color.FromArgb(44,  57,  95);
            var cLbl     = Color.FromArgb(160, 175, 210);
            var cWhite   = Color.White;
            var fntLbl   = new Font("Segoe UI", 9F);
            var fntInput = new Font("Segoe UI", 9.5F);
            var fntHead  = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            var fntSmall = new Font("Segoe UI", 8F);

            // helpers
            TextBox MakeTxt(bool password = false)
            {
                var t = new TextBox
                {
                    BackColor   = cInput,
                    ForeColor   = cWhite,
                    BorderStyle = BorderStyle.FixedSingle,
                    Font        = fntInput,
                    Height      = 26
                };
                if (password) t.PasswordChar = '\u25CF';
                return t;
            }
            Label MakeLbl(string text, bool small = false) => new Label
            {
                Text      = text,
                ForeColor = small ? Color.FromArgb(100, 115, 155) : cLbl,
                AutoSize  = true,
                Font      = small ? fntSmall : fntLbl
            };
            Panel MakeSeparator() => new Panel
            {
                Height    = 1,
                Dock      = DockStyle.Top,
                BackColor = cBorder
            };
            Label MakeSectionHead(string text) => new Label
            {
                Text      = text,
                ForeColor = cAccent,
                Font      = fntHead,
                AutoSize  = true
            };

            // â”€â”€ TabControl â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            var tab = new TabControl
            {
                Dock      = DockStyle.Fill,
                Font      = new Font("Segoe UI", 10F),
                BackColor = cBg,
                Padding   = new System.Drawing.Point(14, 5)
            };

            // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
            //  ABA EMPRESA
            // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
            var tabEmp = new TabPage("  Dados da Empresa  ")
            {
                BackColor = cBg,
                ForeColor = cWhite,
                Padding   = new Padding(0)
            };

            // cabeçalho decorativo
            var empHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 52,
                BackColor = Color.FromArgb(36, 52, 95)
            };
            var empHeaderTitle = new Label
            {
                Text      = "🏢  Dados da Empresa",
                ForeColor = cWhite,
                Font      = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize  = true,
                Left      = 18,
                Top       = 14
            };
            var empHeaderSub = new Label
            {
                Text      = "Configure as informações da sua empresa",
                ForeColor = Color.FromArgb(140, 165, 210),
                Font      = fntSmall,
                AutoSize  = true,
                Left      = 18,
                Top       = 34
            };
            empHeader.Controls.Add(empHeaderTitle);
            empHeader.Controls.Add(empHeaderSub);

            // card empresa
            var empCard = new Panel
            {
                Left      = 30,
                Top       = 68,
                Width     = 620,
                Height    = 340,
                BackColor = cCard
            };
            empCard.Anchor = System.Windows.Forms.AnchorStyles.Top |
                             System.Windows.Forms.AnchorStyles.Left;

            // Borda esquerda colorida
            var empAccentBar = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 4,
                BackColor = cAccent
            };
            empCard.Controls.Add(empAccentBar);

            int ex = 16, lw = 130, tw = 360, th = 27, gy = 16;

            txtEmpNome     = MakeTxt();
            txtEmpFantasia = MakeTxt();
            txtEmpCNPJ     = MakeTxt();
            txtEmpTel      = MakeTxt();
            txtEmpEmail    = MakeTxt();
            txtEmpEnd      = MakeTxt();

            var empFieldDefs = new[]
            {
                ("Razão Social:",   (System.Windows.Forms.Control)txtEmpNome,     "Nome legal da empresa"),
                ("Nome Fantasia:",  txtEmpFantasia,  "Como é conhecida no mercado"),
                ("CNPJ:",           txtEmpCNPJ,      "Somente números ou com pontuação"),
                ("Telefone:",       txtEmpTel,       ""),
                ("E-mail:",         txtEmpEmail,     ""),
                ("Endereço:",       txtEmpEnd,       "")
            };

            int ey = 18;
            foreach (var (label, ctrl, hint) in empFieldDefs)
            {
                var lbl = MakeLbl(label);
                lbl.SetBounds(ex + 8, ey + 5, lw, 18);
                ctrl.SetBounds(ex + lw + 14, ey, tw, th);
                empCard.Controls.Add(lbl);
                empCard.Controls.Add(ctrl);
                ey += th + gy;
            }

            var btnSalvEmp = new Button
            {
                Text      = "✓  Salvar Empresa",
                Left      = ex + lw + 14,
                Top       = ey + 8,
                Width     = 160,
                Height    = 32,
                BackColor = cGreen,
                ForeColor = cWhite,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnSalvEmp.FlatAppearance.BorderSize = 0;
            btnSalvEmp.Click += BtnSalvarEmpresa_Click;
            empCard.Controls.Add(btnSalvEmp);

            tabEmp.Controls.Add(empCard);
            tabEmp.Controls.Add(empHeader);

            // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
            //  ABA USUÁRIOS
            // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
            var tabUsr = new TabPage("  Usuários  ")
            {
                BackColor = cBg,
                ForeColor = cWhite,
                Padding   = new Padding(0)
            };

            // cabeçalho decorativo
            var usrHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 52,
                BackColor = Color.FromArgb(36, 52, 95)
            };
            var usrHeaderTitle = new Label
            {
                Text      = "👥  Usuários do Sistema",
                ForeColor = cWhite,
                Font      = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize  = true,
                Left      = 18,
                Top       = 14
            };
            var usrHeaderSub = new Label
            {
                Text      = "Gerencie os usuários e seus acessos",
                ForeColor = Color.FromArgb(140, 165, 210),
                Font      = fntSmall,
                AutoSize  = true,
                Left      = 18,
                Top       = 34
            };
            usrHeader.Controls.Add(usrHeaderTitle);
            usrHeader.Controls.Add(usrHeaderSub);

            // container principal (abaixo do header)
            var usrBody = new Panel { Dock = DockStyle.Fill, BackColor = cBg, Padding = new Padding(0) };

            // â”€â”€ Grid (esquerda) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
                        // -- Formulario centralizado -----------------------------------------
            var pnlUsrForm = new Panel { Dock = DockStyle.Fill, BackColor = cBg };

            var pnlFormHead = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = Color.FromArgb(30, 42, 78) };
            pnlFormHead.Controls.Add(new Label { Text = "Dados do Usu\u00e1rio", ForeColor = cLbl,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), AutoSize = true, Left = 14, Top = 9 });

            // Painel de busca (oculto por padrao)
            pnlBuscaUsuarios = new Panel { Dock = DockStyle.Top, Height = 200, BackColor = cCard,
                Visible = false, Padding = new Padding(10, 6, 10, 6) };
            var pnlSrchHead = new Panel { Dock = DockStyle.Top, Height = 28, BackColor = cCard };
            pnlSrchHead.Controls.Add(new Label { Text = "Pesquisar usu\u00e1rio:", ForeColor = cLbl,
                Font = fntLbl, AutoSize = true, Left = 0, Top = 5 });
            var btnFch = new Button { Text = "\u2715", Top = 0, Width = 26, Height = 26,
                BackColor = Color.FromArgb(70, 80, 110), ForeColor = cWhite, FlatStyle = FlatStyle.Flat,
                Font = fntLbl, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnFch.FlatAppearance.BorderSize = 0;
            btnFch.Click += (_, __) => pnlBuscaUsuarios.Visible = false;
            pnlSrchHead.SizeChanged += (_, __) => btnFch.Left = pnlSrchHead.Width - btnFch.Width - 2;
            pnlSrchHead.Controls.Add(btnFch);

            txtPesquisa = new TextBox { Dock = DockStyle.Top, Height = 28, BackColor = cInput,
                ForeColor = cWhite, Font = fntInput, BorderStyle = BorderStyle.FixedSingle };
            txtPesquisa.PlaceholderText = "Digite nome ou login...";
            txtPesquisa.TextChanged += TxtPesquisa_TextChanged;

            lstUsuarios = new ListBox { Dock = DockStyle.Fill, BackColor = cInput, ForeColor = cWhite,
                Font = fntLbl, BorderStyle = BorderStyle.None };
            lstUsuarios.DoubleClick += LstUsuarios_DoubleClick;

            pnlBuscaUsuarios.Controls.Add(lstUsuarios);
            pnlBuscaUsuarios.Controls.Add(txtPesquisa);
            pnlBuscaUsuarios.Controls.Add(pnlSrchHead);

            // Area que centraliza o card
            var pnlCardArea = new Panel { Dock = DockStyle.Fill, BackColor = cBg };

            // Card do formulario
            var frmCard = new Panel { Width = 500, BackColor = cCard };
            frmCard.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 4, BackColor = Color.FromArgb(155, 89, 182) });

            int uf = 14, ulw = 138, utw = 280, uth = 26, ugy = 14, uy = 16;

            txtUsrNome      = MakeTxt();
            txtUsrLogin     = MakeTxt();
            txtUsrSenha     = MakeTxt(true);
            txtUsrSenhaConf = MakeTxt(true);
            cmbUsrNivel = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, BackColor = cInput,
                ForeColor = cWhite, Font = fntInput, FlatStyle = FlatStyle.Flat };
            cmbUsrNivel.Items.AddRange(new object[] { "Operador", "Gerente", "Admin" });
            cmbUsrNivel.SelectedIndex = 0;
            cmbUsrSit = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, BackColor = cInput,
                ForeColor = cWhite, Font = fntInput, FlatStyle = FlatStyle.Flat };
            cmbUsrSit.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbUsrSit.SelectedIndex = 0;

            // Campos Nome, Login, Senha, Confirmar Senha
            var usrFieldDefs = new[]
            {
                ("Nome:",            (System.Windows.Forms.Control)txtUsrNome),
                ("Login:",           txtUsrLogin),
                ("Senha:",           txtUsrSenha),
                ("Confirmar Senha:", txtUsrSenhaConf),
            };
            foreach (var (label, ctrl) in usrFieldDefs)
            {
                var lbl = MakeLbl(label);
                lbl.SetBounds(uf + 8, uy + 5, ulw, 18);
                ctrl.SetBounds(uf + ulw + 14, uy, utw, uth);
                frmCard.Controls.Add(lbl);
                frmCard.Controls.Add(ctrl);
                uy += uth + ugy;
            }
            // Hint de senha logo abaixo de "Confirmar Senha"
            var lblHintSenha = MakeLbl("(deixe em branco para n\u00e3o alterar)", true);
            lblHintSenha.SetBounds(uf + ulw + 14, uy - 8, utw, 16);
            frmCard.Controls.Add(lblHintSenha);
            uy += 6;

            // Campos N\u00edvel e Situa\u00e7\u00e3o
            var usrFieldDefs2 = new[]
            {
                ("N\u00edvel:",    (System.Windows.Forms.Control)cmbUsrNivel),
                ("Situa\u00e7\u00e3o:", cmbUsrSit),
            };
            foreach (var (label, ctrl) in usrFieldDefs2)
            {
                var lbl = MakeLbl(label);
                lbl.SetBounds(uf + 8, uy + 5, ulw, 18);
                ctrl.SetBounds(uf + ulw + 14, uy, utw, uth);
                frmCard.Controls.Add(lbl);
                frmCard.Controls.Add(ctrl);
                uy += uth + ugy;
            }

            var lblModTit = MakeSectionHead("M\u00f3dulos de Acesso");
            lblModTit.SetBounds(uf + 8, uy + 4, 250, 20);
            frmCard.Controls.Add(lblModTit);
            uy += 28;

            var modNames  = new[] { "Dashboard", "Pedidos", "Financeiro", "Produtos", "Categorias", "Clientes", "Fornecedores", "Cupons", "Empresa" };
            var modFields = new CheckBox[9];
            int cbColW = 96, cbRowH = 24, cbX0 = uf + ulw + 14;
            for (int i = 0; i < modNames.Length; i++)
            {
                var chk = new CheckBox { Text = modNames[i], ForeColor = cLbl, BackColor = cCard,
                    Font = fntLbl, AutoSize = false, Width = 94, Height = 22 };
                chk.SetBounds(cbX0 + (i % 3) * cbColW, uy + (i / 3) * cbRowH, 94, 22);
                frmCard.Controls.Add(chk);
                modFields[i] = chk;
            }
            chkModDashboard    = modFields[0];
            chkModPedidos      = modFields[1];
            chkModFinanceiro   = modFields[2];
            chkModProdutos     = modFields[3];
            chkModCategorias   = modFields[4];
            chkModClientes     = modFields[5];
            chkModFornecedores = modFields[6];
            chkModCupons       = modFields[7];
            chkModEmpresa      = modFields[8];

            // Privilegio extra: Cancelar Pedidos
            chkModCancelarPedidos = new CheckBox
            {
                Text      = "Cancelar Pedidos",
                ForeColor = cLbl,
                BackColor = cCard,
                Font      = fntLbl,
                AutoSize  = false,
                Width     = 200,
                Height    = 22
            };
            chkModCancelarPedidos.SetBounds(cbX0, uy + 3 * cbRowH, 200, 22);
            frmCard.Controls.Add(chkModCancelarPedidos);

            uy += 4 * cbRowH + 12;

            int bw = 128, bh = 32;
            var pnlBtns = new Panel { Left = uf + 8, Top = uy + 6, Width = bw * 2 + 8, Height = bh, BackColor = cCard };

            btnNovoUsr = new Button { Text = "+ Novo", Left = 0, Top = 0, Width = bw, Height = bh,
                BackColor = Color.FromArgb(52, 73, 94), ForeColor = cWhite, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnNovoUsr.FlatAppearance.BorderSize = 0;
            btnNovoUsr.Click += BtnNovoUsuario_Click;

            btnPesquisarUsr = new Button { Text = "Pesquisar", Left = bw + 8, Top = 0, Width = bw, Height = bh,
                BackColor = Color.FromArgb(52, 152, 219), ForeColor = cWhite, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnPesquisarUsr.FlatAppearance.BorderSize = 0;
            btnPesquisarUsr.Click += BtnPesquisarUsuario_Click;

            btnSalvUsr = new Button { Text = "\u2713  Salvar", Left = 0, Top = 0, Width = bw, Height = bh,
                BackColor = cGreen, ForeColor = cWhite, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand, Visible = false };
            btnSalvUsr.FlatAppearance.BorderSize = 0;
            btnSalvUsr.Click += BtnSalvarUsuario_Click;

            btnCancelarUsr = new Button { Text = "Cancelar", Left = bw + 8, Top = 0, Width = bw, Height = bh,
                BackColor = Color.FromArgb(108, 117, 125), ForeColor = cWhite, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand, Visible = false };
            btnCancelarUsr.FlatAppearance.BorderSize = 0;
            btnCancelarUsr.Click += BtnCancelarUsuario_Click;

            pnlBtns.Controls.AddRange(new Control[] { btnNovoUsr, btnPesquisarUsr, btnSalvUsr, btnCancelarUsr });
            frmCard.Controls.Add(pnlBtns);
            frmCard.Height = uy + 6 + bh + 10;

            pnlCardArea.SizeChanged += (_, __) =>
            {
                frmCard.Left = System.Math.Max(14, (pnlCardArea.Width  - frmCard.Width)  / 2);
                frmCard.Top  = System.Math.Max(14, (pnlCardArea.Height - frmCard.Height) / 2);
            };
            pnlCardArea.Controls.Add(frmCard);

            pnlUsrForm.Controls.Add(pnlCardArea);
            pnlUsrForm.Controls.Add(pnlBuscaUsuarios);
            pnlUsrForm.Controls.Add(pnlFormHead);
            usrBody.Controls.Add(pnlUsrForm);

            tabUsr.Controls.Add(usrBody);
            tabUsr.Controls.Add(usrHeader);

            tab.TabPages.Add(tabEmp);
            tab.TabPages.Add(tabUsr);
            this.Controls.Add(tab);
        }

        // â”€â”€ Empresa fields â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        internal TextBox txtEmpNome;
        internal TextBox txtEmpFantasia;
        internal TextBox txtEmpCNPJ;
        internal TextBox txtEmpTel;
        internal TextBox txtEmpEmail;
        internal TextBox txtEmpEnd;

        // â”€â”€ Usuario fields â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        internal TextBox txtUsrNome;
        internal TextBox txtUsrLogin;
        internal TextBox txtUsrSenha;
        internal TextBox txtUsrSenhaConf;
        internal ComboBox cmbUsrNivel;
        internal ComboBox cmbUsrSit;
        internal Panel    pnlBuscaUsuarios;
        internal TextBox  txtPesquisa;
        internal ListBox  lstUsuarios;
        internal Button   btnNovoUsr;
        internal Button   btnPesquisarUsr;
        internal Button   btnSalvUsr;
        internal Button   btnCancelarUsr;

        // ── Module checkboxes ────────────────────────────────────────────────
        internal CheckBox chkModDashboard;
        internal CheckBox chkModPedidos;
        internal CheckBox chkModFinanceiro;
        internal CheckBox chkModProdutos;
        internal CheckBox chkModCategorias;
        internal CheckBox chkModClientes;
        internal CheckBox chkModFornecedores;
        internal CheckBox chkModCupons;
        internal CheckBox chkModEmpresa;
        internal CheckBox chkModCancelarPedidos;
    }
}
