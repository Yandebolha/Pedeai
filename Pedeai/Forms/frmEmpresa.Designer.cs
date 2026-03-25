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

        private void BuildUI()
        {
            // ── Form ─────────────────────────────────────────────────────────
            this.Text            = "Empresa e Usuarios";
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor       = Color.FromArgb(15, 22, 45);
            this.ForeColor       = Color.White;
            this.Font            = new Font("Segoe UI", 10F);
            this.MinimumSize     = new System.Drawing.Size(900, 580);
            this.Size            = new System.Drawing.Size(960, 640);

            var tab = new TabControl
            {
                Dock      = DockStyle.Fill,
                Font      = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(28, 37, 65)
            };

            // ╔══════════════════════════════════════════════════════════════╗
            // ║  ABA EMPRESA                                                 ║
            // ╚══════════════════════════════════════════════════════════════╝
            var tabEmp = new TabPage("Dados da Empresa")
            {
                BackColor = Color.FromArgb(15, 22, 45),
                ForeColor = Color.White
            };

            Color clr = Color.FromArgb(28, 37, 65);
            Color txt = Color.White;
            Font  fnt = new Font("Segoe UI", 10F);

            TextBox MakeTxt(string placeholder) => new TextBox
            {
                BackColor   = clr,
                ForeColor   = txt,
                BorderStyle = BorderStyle.FixedSingle,
                Font        = fnt
            };

            Label MakeLbl(string t) => new Label
            {
                Text      = t,
                ForeColor = Color.FromArgb(180, 190, 220),
                AutoSize  = true,
                Font      = fnt
            };

            int ex = 20, lw = 140, tw = 420, lh = 22, th = 26, gy = 18;

            txtEmpNome     = MakeTxt("Nome");
            txtEmpFantasia = MakeTxt("Nome Fantasia");
            txtEmpCNPJ     = MakeTxt("CNPJ");
            txtEmpTel      = MakeTxt("Telefone");
            txtEmpEmail    = MakeTxt("E-mail");
            txtEmpEnd      = MakeTxt("Endereco");

            var fields = new[]
            {
                ("Razao Social:",    (System.Windows.Forms.Control)txtEmpNome),
                ("Nome Fantasia:",   txtEmpFantasia),
                ("CNPJ:",            txtEmpCNPJ),
                ("Telefone:",        txtEmpTel),
                ("E-mail:",          txtEmpEmail),
                ("Endereco:",        txtEmpEnd)
            };

            int y = 20;
            foreach (var (label, ctrl) in fields)
            {
                var lbl = MakeLbl(label);
                lbl.SetBounds(ex, y + 3, lw, lh);
                ctrl.SetBounds(ex + lw + 8, y, tw, th);
                tabEmp.Controls.Add(lbl);
                tabEmp.Controls.Add(ctrl);
                y += th + gy;
            }

            var btnSalvEmp = new Button
            {
                Text      = "\u2714 Salvar Empresa",
                Bounds    = new System.Drawing.Rectangle(ex + lw + 8, y + 10, 180, 30),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnSalvEmp.FlatAppearance.BorderSize = 0;
            btnSalvEmp.Click += BtnSalvarEmpresa_Click;
            tabEmp.Controls.Add(btnSalvEmp);

            // ╔══════════════════════════════════════════════════════════════╗
            // ║  ABA USUARIOS                                                ║
            // ╚══════════════════════════════════════════════════════════════╝
            var tabUsr = new TabPage("Usuarios")
            {
                BackColor = Color.FromArgb(15, 22, 45),
                ForeColor = Color.White
            };

            // Grid usuarios (esquerda)
            gridUsuarios = new DataGridView
            {
                Bounds                 = new System.Drawing.Rectangle(0, 0, 440, 0),
                Dock                   = DockStyle.Left,
                Width                  = 440,
                ReadOnly               = true,
                AllowUserToAddRows     = false,
                SelectionMode          = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible      = false,
                MultiSelect            = false,
                BackgroundColor        = Color.FromArgb(28, 37, 65),
                GridColor              = Color.FromArgb(50, 60, 100),
                DefaultCellStyle       = { BackColor = Color.FromArgb(28, 37, 65), ForeColor = Color.White, SelectionBackColor = Color.FromArgb(52,152,219), SelectionForeColor = Color.White },
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(36, 48, 82) },
                ColumnHeadersDefaultCellStyle   = { BackColor = Color.FromArgb(36, 48, 82), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) },
                AutoSizeColumnsMode    = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle            = BorderStyle.None,
                Font                   = new Font("Segoe UI", 9F),
                RowTemplate            = { Height = 27 }
            };
            gridUsuarios.SelectionChanged += GridUsuarios_SelectionChanged;

            // Form de edicao (direita)
            var pnlForm = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(22, 30, 55),
                Padding   = new Padding(16)
            };

            int uf = 16, ulw = 120, utw = 260, uth = 26, ugy = 14;
            int uy = 16;

            txtUsrNome     = MakeTxt("Nome");
            txtUsrLogin    = MakeTxt("Login");
            txtUsrSenha    = MakeTxt("Senha"); txtUsrSenha.PasswordChar = '\u2022';
            txtUsrSenhaConf = MakeTxt("Confirmar Senha"); txtUsrSenhaConf.PasswordChar = '\u2022';

            cmbUsrNivel = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, BackColor = clr, ForeColor = txt, Font = fnt };
            cmbUsrNivel.Items.AddRange(new object[] { "Operador", "Gerente", "Admin" });
            cmbUsrNivel.SelectedIndex = 0;

            cmbUsrSit = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, BackColor = clr, ForeColor = txt, Font = fnt };
            cmbUsrSit.Items.AddRange(new object[] { "Ativo", "Inativo" });
            cmbUsrSit.SelectedIndex = 0;

            var usrFields = new[]
            {
                ("Nome:",           (System.Windows.Forms.Control)txtUsrNome),
                ("Login:",          txtUsrLogin),
                ("Senha:",          txtUsrSenha),
                ("Confirmar Senha:",txtUsrSenhaConf),
                ("Nivel:",          cmbUsrNivel),
                ("Situacao:",       cmbUsrSit)
            };

            foreach (var (label, ctrl) in usrFields)
            {
                var lbl = MakeLbl(label);
                lbl.SetBounds(uf, uy + 3, ulw, lh);
                ctrl.SetBounds(uf + ulw + 8, uy, utw, uth);
                pnlForm.Controls.Add(lbl);
                pnlForm.Controls.Add(ctrl);
                uy += uth + ugy;
            }

            var lblHintSenha = new Label
            {
                Text      = "(deixe em branco para nao alterar)",
                ForeColor = Color.FromArgb(120, 130, 160),
                Font      = new Font("Segoe UI", 8F),
                Bounds    = new System.Drawing.Rectangle(uf + ulw + 8, uy - ugy - 4, utw, 16)
            };
            pnlForm.Controls.Add(lblHintSenha);

            var btnNovo = new Button
            {
                Text      = "+ Novo",
                Bounds    = new System.Drawing.Rectangle(uf, uy + 10, 110, 30),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnNovo.FlatAppearance.BorderSize = 0;
            btnNovo.Click += BtnNovoUsuario_Click;

            var btnSalvUsr = new Button
            {
                Text      = "\u2714 Salvar",
                Bounds    = new System.Drawing.Rectangle(uf + 120, uy + 10, 110, 30),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btnSalvUsr.FlatAppearance.BorderSize = 0;
            btnSalvUsr.Click += BtnSalvarUsuario_Click;

            pnlForm.Controls.Add(btnNovo);
            pnlForm.Controls.Add(btnSalvUsr);

            tabUsr.Controls.Add(pnlForm);
            tabUsr.Controls.Add(gridUsuarios);

            tab.TabPages.Add(tabEmp);
            tab.TabPages.Add(tabUsr);
            this.Controls.Add(tab);
        }

        // ── Empresa fields ───────────────────────────────────────────────────
        internal TextBox txtEmpNome;
        internal TextBox txtEmpFantasia;
        internal TextBox txtEmpCNPJ;
        internal TextBox txtEmpTel;
        internal TextBox txtEmpEmail;
        internal TextBox txtEmpEnd;

        // ── Usuario fields ───────────────────────────────────────────────────
        internal DataGridView gridUsuarios;
        internal TextBox txtUsrNome;
        internal TextBox txtUsrLogin;
        internal TextBox txtUsrSenha;
        internal TextBox txtUsrSenhaConf;
        internal ComboBox cmbUsrNivel;
        internal ComboBox cmbUsrSit;
    }
}
