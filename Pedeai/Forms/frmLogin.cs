using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;

namespace Pedeai.Forms
{
    public partial class frmLogin : Form
    {
        private UsuarioBLL _bll;

        public frmLogin()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new UsuarioBLL();

            // Carrega e recorta a logo num quadrado central — sem letterbox
            try
            {
                string imgPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RanGoFood.png");
                if (System.IO.File.Exists(imgPath))
                {
                    using var full = new Bitmap(imgPath);
                    int size = Math.Min(full.Width, full.Height);
                    int cx   = (full.Width  - size) / 2;
                    int cy   = (full.Height - size) / 2;
                    picLogo.Image = full.Clone(
                        new Rectangle(cx, cy, size, size),
                        full.PixelFormat);
                }
            }
            catch { }

            // Borda arredondada no card
            pnlCard.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rc = new Rectangle(1, 1, pnlCard.Width - 3, pnlCard.Height - 3);
                using var pen = new Pen(Color.FromArgb(200, 165, 110), 2f);
                DrawRoundedRect(g, pen, rc, 18);
            };

            // Fundo arredondado nos campos de input
            pnlLoginCard.Paint += (s, e) => PaintInputPanel(e.Graphics, pnlLoginCard);
            pnlSenhaCard.Paint  += (s, e) => PaintInputPanel(e.Graphics, pnlSenhaCard);
        }

        private static void PaintInputPanel(Graphics g, Panel pnl)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var rc = new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1);
            using var brush = new SolidBrush(Color.FromArgb(238, 234, 227));
            DrawRoundedRectFill(g, brush, rc, 8);
        }

        private static void DrawRoundedRect(Graphics g, Pen pen, Rectangle rc, int radius)
        {
            using var path = RoundedPath(rc, radius);
            g.DrawPath(pen, path);
        }

        private static void DrawRoundedRectFill(Graphics g, Brush brush, Rectangle rc, int radius)
        {
            using var path = RoundedPath(rc, radius);
            g.FillPath(brush, path);
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundedPath(Rectangle rc, int r)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rc.X, rc.Y, r * 2, r * 2, 180, 90);
            path.AddArc(rc.Right - r * 2, rc.Y, r * 2, r * 2, 270, 90);
            path.AddArc(rc.Right - r * 2, rc.Bottom - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(rc.X, rc.Bottom - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void BtnEntrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                lblMensagem.Text = "Preencha login e senha.";
                return;
            }

            var usuario = _bll.Autenticar(txtLogin.Text.Trim(), txtSenha.Text);
            if (usuario == null)
            {
                lblMensagem.Text = "Login ou senha incorretos.";
                txtSenha.Clear();
                txtSenha.Focus();
                return;
            }

            UsuarioSessao.Iniciar(usuario);
            DialogResult = DialogResult.OK;
            Close();
        }

        // Resolve o texto digitado: se for número, busca por código;
        // senão busca por login/nome. Substitui o campo pelo nome encontrado
        // e devolve true para indicar que o foco deve ir para a senha.
        private bool ResolverLogin()
        {
            lblMensagem.Text = "";
            var texto = txtLogin.Text.Trim();
            if (string.IsNullOrEmpty(texto)) { lblNomeUsuario.Text = ""; return false; }

            try
            {
                string nome = "";

                // Tentativa 1: código numérico
                if (int.TryParse(texto, out int cod) && cod > 0)
                    nome = _bll.BuscarNomePorCodigo(cod);

                // Tentativa 2: login ou nome
                if (string.IsNullOrEmpty(nome))
                    nome = _bll.BuscarNomePorLogin(texto);

                if (!string.IsNullOrEmpty(nome))
                {
                    // Substitui o conteúdo do campo pelo nome real
                    txtLogin.Text = nome;
                    txtLogin.SelectionStart = nome.Length;

                    lblNomeUsuario.ForeColor = Color.FromArgb(39, 174, 96);
                    lblNomeUsuario.Text = "\u2713 Usuário identificado";
                    return true;
                }
                else
                {
                    lblNomeUsuario.ForeColor = Color.FromArgb(231, 76, 60);
                    lblNomeUsuario.Text = "Usuário não encontrado";
                    return false;
                }
            }
            catch { lblNomeUsuario.Text = ""; return false; }
        }

        private void TxtLogin_Leave(object sender, EventArgs e) => ResolverLogin();

        private void TxtSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) BtnEntrar_Click(null, null);
        }

        private void TxtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (ResolverLogin())
                    txtSenha.Focus();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void BtnFechar_Click(object s, EventArgs e) { DialogResult = DialogResult.Cancel; Close(); }

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION       = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(System.IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private void FormDrag_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void picLogo_Click(object sender, EventArgs e)
        {

        }
    }
}
