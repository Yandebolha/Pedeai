using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;

namespace Pedeai.Forms
{
    public partial class frmLogin : Form
    {
        private readonly UsuarioBLL _bll = new UsuarioBLL();

        public frmLogin()
        {
            InitializeComponent();
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

        private void TxtLogin_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                lblNomeUsuario.Text = "";
                return;
            }
            try
            {
                var nome = _bll.BuscarNomePorLogin(txtLogin.Text.Trim());
                if (!string.IsNullOrEmpty(nome))
                {
                    lblNomeUsuario.ForeColor = System.Drawing.Color.FromArgb(39, 174, 96);
                    lblNomeUsuario.Text = "\u2713 " + nome;
                }
                else
                {
                    lblNomeUsuario.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);
                    lblNomeUsuario.Text = "Usu\u00e1rio n\u00e3o encontrado";
                }
            }
            catch { lblNomeUsuario.Text = ""; }
        }

        private void TxtSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) BtnEntrar_Click(null, null);
        }

        private void TxtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) txtSenha.Focus();
        }
    }
}
