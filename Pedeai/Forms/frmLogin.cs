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
            // Carrega o logo recortando o círculo central e removendo os cantos brancos
            try
            {
                string imgPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RanGoFood.png");
                if (System.IO.File.Exists(imgPath))
                {
                    using var original = Image.FromFile(imgPath);
                    // Corte quadrado centralizado
                    int size = original.Height;
                    int x    = (original.Width - size) / 2;
                    // Bitmap ARGB para suportar transparência nos cantos
                    var circled = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using var g = System.Drawing.Graphics.FromImage(circled);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    // Clip circular – tudo fora da elipse fica transparente
                    using var path = new System.Drawing.Drawing2D.GraphicsPath();
                    path.AddEllipse(0, 0, size, size);
                    g.SetClip(path);
                    g.DrawImage(original,
                        new System.Drawing.Rectangle(0, 0, size, size),
                        new System.Drawing.Rectangle(x, 0, size, size),
                        System.Drawing.GraphicsUnit.Pixel);
                    picLogo.Image = circled;
                }
            }
            catch { /* sem imagem, exibe em branco */ }
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
    }
}
