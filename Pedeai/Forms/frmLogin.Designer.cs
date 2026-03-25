using System.Drawing;
using System.Windows.Forms;

namespace Pedeai.Forms
{
    partial class frmLogin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // ── Cores da paleta principal ────────────────────────────────────
            var clrBg      = Color.FromArgb(15,  22,  45);   // fundo geral
            var clrSide    = Color.FromArgb(28,  37,  65);   // painel lateral
            var clrCard    = Color.FromArgb(36,  48,  82);   // card / top bar
            var clrInput   = Color.FromArgb(20,  30,  58);   // fundo dos inputs
            var clrAccent  = Color.FromArgb(52,  152, 219);  // azul accent
            var clrSuccess = Color.FromArgb(39,  174,  96);  // verde (nome encontrado)
            var clrMuted   = Color.FromArgb(160, 175, 210);  // labels secundários
            var clrDanger  = Color.FromArgb(231,  76,  60);  // vermelho erro

            // ── Form ─────────────────────────────────────────────────────────
            this.Text            = "Pedeai \u2014 Login";
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;   // sem borda = visual moderno
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = clrBg;
            this.ForeColor       = Color.White;
            this.Font            = new Font("Segoe UI", 10F);
            this.ClientSize      = new Size(780, 520);

            // ── Painel esquerdo (branding) ────────────────────────────────────
            var pnlLeft = new Panel
            {
                Bounds    = new System.Drawing.Rectangle(0, 0, 290, 520),
                BackColor = clrSide
            };

            // Faixa accent no topo do painel esquerdo
            var topAccent = new Panel
            {
                Bounds    = new System.Drawing.Rectangle(0, 0, 290, 5),
                BackColor = clrAccent
            };

            var lblIco = new Label
            {
                Text      = "\U0001F355",   // 🍕
                Font      = new Font("Segoe UI Emoji", 52F),
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(0, 80, 290, 80),
                BackColor = Color.Transparent
            };

            var lblBrand = new Label
            {
                Text      = "PEDEAI",
                Font      = new Font("Segoe UI", 30F, FontStyle.Bold),
                ForeColor = clrAccent,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(0, 168, 290, 50),
                BackColor = Color.Transparent
            };

            var lblTagline = new Label
            {
                Text      = "Sistema de Gestao\nde Pedidos",
                Font      = new Font("Segoe UI", 11F),
                ForeColor = clrMuted,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(0, 222, 290, 56),
                BackColor = Color.Transparent
            };

            // Linha decorativa
            var sepLeft = new Panel
            {
                Bounds    = new System.Drawing.Rectangle(60, 298, 170, 1),
                BackColor = Color.FromArgb(60, 75, 115)
            };

            var lblVer = new Label
            {
                Text      = "v2.0",
                Font      = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(90, 110, 155),
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(0, 308, 290, 20),
                BackColor = Color.Transparent
            };

            pnlLeft.Controls.AddRange(new Control[] { topAccent, lblIco, lblBrand, lblTagline, sepLeft, lblVer });

            // ── Painel direito (formulário) ───────────────────────────────────
            var pnlRight = new Panel
            {
                Bounds    = new System.Drawing.Rectangle(290, 0, 490, 520),
                BackColor = clrBg
            };

            // Faixa accent topo direito
            var topAccentR = new Panel
            {
                Bounds    = new System.Drawing.Rectangle(0, 0, 490, 5),
                BackColor = clrAccent
            };

            // Botão fechar (X) no canto
            var btnFechar = new Button
            {
                Text         = "\u2715",
                Bounds       = new System.Drawing.Rectangle(448, 8, 30, 24),
                BackColor    = Color.Transparent,
                ForeColor    = clrMuted,
                FlatStyle    = FlatStyle.Flat,
                Font         = new Font("Segoe UI", 10F),
                Cursor       = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnFechar.FlatAppearance.BorderSize   = 0;
            btnFechar.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            btnFechar.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };

            var lblWelcome = new Label
            {
                Text      = "Bem-vindo",
                Font      = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Bounds    = new System.Drawing.Rectangle(50, 56, 360, 44),
                BackColor = Color.Transparent
            };

            var lblSubWelcome = new Label
            {
                Text      = "Identifique-se para continuar",
                Font      = new Font("Segoe UI", 10F),
                ForeColor = clrMuted,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Bounds    = new System.Drawing.Rectangle(50, 100, 360, 22),
                BackColor = Color.Transparent
            };

            var sepTop = new Panel
            {
                Bounds    = new System.Drawing.Rectangle(50, 132, 370, 1),
                BackColor = Color.FromArgb(45, 60, 100)
            };

            // ── Campo: Usuário (login ou nome) ────────────────────────────────
            var lblLoginTitle = new Label
            {
                Text      = "USUÁRIO (LOGIN OU NOME)",
                Font      = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = clrMuted,
                AutoSize  = true,
                Bounds    = new System.Drawing.Rectangle(50, 154, 300, 16),
                BackColor = Color.Transparent
            };

            // Card container do campo login
            var pnlLoginCard = new Panel
            {
                Bounds    = new System.Drawing.Rectangle(50, 174, 370, 46),
                BackColor = clrInput
            };
            // Ícone no card
            var lblLoginIcon = new Label
            {
                Text      = "\uD83D\uDC64",  // 👤
                Font      = new Font("Segoe UI Emoji", 13F),
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(0, 0, 46, 46),
                BackColor = Color.Transparent,
                ForeColor = clrAccent
            };
            txtLogin = new TextBox
            {
                Bounds        = new System.Drawing.Rectangle(50, 10, 310, 26),
                BackColor     = clrInput,
                ForeColor     = Color.White,
                BorderStyle   = BorderStyle.None,
                Font          = new Font("Segoe UI", 11F)
            };
            txtLogin.KeyDown += TxtLogin_KeyDown;
            txtLogin.Leave   += TxtLogin_Leave;
            pnlLoginCard.Controls.AddRange(new Control[] { lblLoginIcon, txtLogin });

            // Borda inferior accent no card (muda cor ao focar)
            var brdLogin = new Panel
            {
                Bounds    = new System.Drawing.Rectangle(50, 220, 370, 2),
                BackColor = clrAccent
            };

            // Nome identificado
            lblNomeUsuario = new Label
            {
                Text      = "",
                Font      = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = clrSuccess,
                AutoSize  = false,
                Bounds    = new System.Drawing.Rectangle(50, 226, 370, 18),
                BackColor = Color.Transparent
            };

            // ── Campo: Senha ──────────────────────────────────────────────────
            var lblSenhaTitle = new Label
            {
                Text      = "SENHA",
                Font      = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = clrMuted,
                AutoSize  = true,
                Bounds    = new System.Drawing.Rectangle(50, 256, 100, 16),
                BackColor = Color.Transparent
            };

            var pnlSenhaCard = new Panel
            {
                Bounds    = new System.Drawing.Rectangle(50, 276, 370, 46),
                BackColor = clrInput
            };
            var lblSenhaIcon = new Label
            {
                Text      = "\uD83D\uDD12",  // 🔒
                Font      = new Font("Segoe UI Emoji", 13F),
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(0, 0, 46, 46),
                BackColor = Color.Transparent,
                ForeColor = clrAccent
            };
            txtSenha = new TextBox
            {
                Bounds        = new System.Drawing.Rectangle(50, 10, 310, 26),
                BackColor     = clrInput,
                ForeColor     = Color.White,
                BorderStyle   = BorderStyle.None,
                Font          = new Font("Segoe UI", 11F),
                PasswordChar  = '\u2022'
            };
            txtSenha.KeyDown += TxtSenha_KeyDown;
            pnlSenhaCard.Controls.AddRange(new Control[] { lblSenhaIcon, txtSenha });

            var brdSenha = new Panel
            {
                Bounds    = new System.Drawing.Rectangle(50, 322, 370, 2),
                BackColor = clrAccent
            };

            // ── Mensagem de erro ──────────────────────────────────────────────
            lblMensagem = new Label
            {
                Text      = "",
                ForeColor = clrDanger,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = new Font("Segoe UI", 9F),
                Bounds    = new System.Drawing.Rectangle(50, 330, 370, 20),
                BackColor = Color.Transparent
            };

            // ── Botão Entrar ──────────────────────────────────────────────────
            var btnEntrar = new Button
            {
                Text      = "Entrar",
                Bounds    = new System.Drawing.Rectangle(50, 362, 370, 44),
                BackColor = clrAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnEntrar.FlatAppearance.BorderSize = 0;
            btnEntrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnEntrar.Click += BtnEntrar_Click;
            this.AcceptButton = btnEntrar;

            var lblCopy = new Label
            {
                Text      = "\u00A9 Pedeai 2025",
                Font      = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(70, 88, 130),
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds    = new System.Drawing.Rectangle(50, 476, 370, 18),
                BackColor = Color.Transparent
            };

            pnlRight.Controls.AddRange(new Control[]
            {
                topAccentR, btnFechar,
                lblWelcome, lblSubWelcome, sepTop,
                lblLoginTitle, pnlLoginCard, brdLogin, lblNomeUsuario,
                lblSenhaTitle, pnlSenhaCard, brdSenha,
                lblMensagem, btnEntrar,
                lblCopy
            });

            // ── Linha divisória vertical ──────────────────────────────────────
            var divider = new Panel
            {
                Bounds    = new System.Drawing.Rectangle(289, 0, 1, 520),
                BackColor = Color.FromArgb(45, 60, 100)
            };

            this.Controls.AddRange(new Control[] { pnlLeft, pnlRight, divider });

            // Permite arrastar o form sem borda
            pnlLeft.MouseDown  += FormDrag_MouseDown;
            pnlRight.MouseDown += FormDrag_MouseDown;
        }

        // Suporte a arrastar form sem borda
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION       = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(System.IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private void FormDrag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        internal TextBox txtLogin;
        internal TextBox txtSenha;
        internal Label   lblMensagem;
        internal Label   lblNomeUsuario;
    }
}
