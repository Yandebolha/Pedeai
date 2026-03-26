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
            var usrBody = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = cBg,
                Padding   = new Padding(0)
            };

            // â”€â”€ Grid (esquerda) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            var pnlGrid = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 440,
                BackColor = cCard,
                Padding   = new Padding(0)
            };

            // faixa do título sobre a grid
            var pnlGridHead = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 36,
                BackColor = Color.FromArgb(30, 42, 78)
            };
            var lblGridTitle = new Label
            {
                Text      = "Lista de Usuários",
                ForeColor = cLbl,
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize  = true,
                Left      = 12,
                Top       = 9
            };
            pnlGridHead.Controls.Add(lblGridTitle);

            gridUsuarios = new DataGridView
            {
                Dock                            = DockStyle.Fill,
                ReadOnly                        = true,
                AllowUserToAddRows              = false,
                SelectionMode                   = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible               = false,
                MultiSelect                     = false,
                BackgroundColor                 = cCard,
                GridColor                       = Color.FromArgb(38, 50, 88),
                BorderStyle                     = BorderStyle.None,
                Font                            = new Font("Segoe UI", 9F),
                CellBorderStyle                 = DataGridViewCellBorderStyle.SingleHorizontal,
                AutoSizeColumnsMode             = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate                     = { Height = 28 },
                ColumnHeadersHeight             = 32,
                ColumnHeadersHeightSizeMode     = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };
            gridUsuarios.DefaultCellStyle.BackColor            = cCard;
            gridUsuarios.DefaultCellStyle.ForeColor            = cWhite;
            gridUsuarios.DefaultCellStyle.SelectionBackColor   = cAccent;
            gridUsuarios.DefaultCellStyle.SelectionForeColor   = cWhite;
            gridUsuarios.DefaultCellStyle.Padding              = new Padding(4, 0, 0, 0);
            gridUsuarios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(26, 36, 66);
            gridUsuarios.ColumnHeadersDefaultCellStyle.BackColor   = Color.FromArgb(30, 42, 78);
            gridUsuarios.ColumnHeadersDefaultCellStyle.ForeColor   = cLbl;
            gridUsuarios.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            gridUsuarios.SelectionChanged += GridUsuarios_SelectionChanged;

            pnlGrid.Controls.Add(gridUsuarios);
            pnlGrid.Controls.Add(pnlGridHead);

            // separador vertical
            var vSep = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 6,
                BackColor = Color.FromArgb(20, 28, 52)
            };

            // â”€â”€ FormulÃ¡rio (direita) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
            var pnlForm = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = cBg,
                Padding   = new Padding(0)
            };

            // cabeçalho painel form
            var pnlFormHead = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 36,
                BackColor = Color.FromArgb(30, 42, 78)
            };
            var lblFormHeadTitle = new Label
            {
                Text      = "Dados do Usuário",
                ForeColor = cLbl,
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize  = true,
                Left      = 14,
                Top       = 9
            };
            pnlFormHead.Controls.Add(lblFormHeadTitle);

            // card do formulário
            var frmCard = new Panel
            {
                Left      = 14,
                Top       = 46,
                Width     = 420,
                BackColor = cCard,
                Padding   = new Padding(0)
            };
            frmCard.Anchor = System.Windows.Forms.AnchorStyles.Top |
                             System.Windows.Forms.AnchorStyles.Left;

            // borda esquerda accent
            var frmAccentBar = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 4,
                BackColor = Color.FromArgb(155, 89, 182)
            };
            frmCard.Controls.Add(frmAccentBar);

            int uf = 14, ulw = 138, utw = 240, uth = 26, ugy = 14;
            int uy = 16;

            txtUsrNome      = MakeTxt();
            txtUsrLogin     = MakeTxt();
            txtUsrSenha     = MakeTxt(true);
            txtUsrSenhaConf = MakeTxt(true);

            cmbUsrNivel = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor     = cInput,
                ForeColor     = cWhite,
                Font          = fntInput,
                FlatStyle     = FlatStyle.Flat
            };
            cmbUsrNivel.Items.AddRange(new object[] { "Operador", "Gerente", "Admin" });
            cmbUsrNivel.SelectedIndex = 0;

            cmbUsrSit = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor     = cInput,
                ForeColor     = cWhite,
                Font          = fntInput,
                FlatStyle     = FlatStyle.Flat
            };
            cmbUsrSit.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbUsrSit.SelectedIndex = 0;

            var usrFieldDefs = new[]
            {
                ("Nome:",            (System.Windows.Forms.Control)txtUsrNome),
                ("Login:",           txtUsrLogin),
                ("Senha:",           txtUsrSenha),
                ("Confirmar Senha:", txtUsrSenhaConf),
                ("Nível:",           cmbUsrNivel),
                ("Situação:",        cmbUsrSit)
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

            var lblHintSenha = MakeLbl("(deixe em branco para não alterar)", true);
            lblHintSenha.SetBounds(uf + ulw + 14, uy - ugy + 1, utw, 16);
            frmCard.Controls.Add(lblHintSenha);

            uy += 16;

            // ── Módulos de acesso ─────────────────────────────────────────────
            var lblModTit = MakeSectionHead("Módulos de Acesso");
            lblModTit.SetBounds(uf + 8, uy + 4, 250, 20);
            frmCard.Controls.Add(lblModTit);
            uy += 26;

            var modNames  = new[] { "Dashboard", "Pedidos", "Financeiro", "Produtos", "Categorias", "Clientes", "Fornecedores", "Cupons", "Empresa" };
            var modFields = new CheckBox[9];
            int cbColW = 88, cbRowH = 24;
            int cbX0   = uf + ulw + 14;
            for (int i = 0; i < modNames.Length; i++)
            {
                var chk = new CheckBox
                {
                    Text      = modNames[i],
                    ForeColor = cLbl,
                    BackColor = cCard,
                    Font      = fntLbl,
                    AutoSize  = true
                };
                chk.SetBounds(cbX0 + (i % 3) * cbColW, uy + (i / 3) * cbRowH, cbColW, cbRowH);
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
            uy += 3 * cbRowH + 8;

            uy += 10;

            // botões
            var pnlBtns = new Panel
            {
                Left      = uf + ulw + 14,
                Top       = uy + 6,
                Width     = utw,
                Height    = 34,
                BackColor = cCard
            };

            var btnNovo = new Button
            {
                Text      = "+ Novo",
                Left      = 0,
                Top       = 0,
                Width     = 108,
                Height    = 32,
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = cWhite,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnNovo.FlatAppearance.BorderSize = 0;
            btnNovo.Click += BtnNovoUsuario_Click;

            var btnSalvUsr = new Button
            {
                Text      = "✓  Salvar",
                Left      = 118,
                Top       = 0,
                Width     = 112,
                Height    = 32,
                BackColor = cGreen,
                ForeColor = cWhite,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnSalvUsr.FlatAppearance.BorderSize = 0;
            btnSalvUsr.Click += BtnSalvarUsuario_Click;

            pnlBtns.Controls.Add(btnNovo);
            pnlBtns.Controls.Add(btnSalvUsr);
            frmCard.Controls.Add(pnlBtns);

            frmCard.Height = uy + 6 + 40;
            pnlForm.Controls.Add(frmCard);
            pnlForm.Controls.Add(pnlFormHead);

            usrBody.Controls.Add(pnlForm);
            usrBody.Controls.Add(vSep);
            usrBody.Controls.Add(pnlGrid);

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
        internal DataGridView gridUsuarios;
        internal TextBox txtUsrNome;
        internal TextBox txtUsrLogin;
        internal TextBox txtUsrSenha;
        internal TextBox txtUsrSenhaConf;
        internal ComboBox cmbUsrNivel;
        internal ComboBox cmbUsrSit;

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
    }
}
