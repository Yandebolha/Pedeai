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
            BuildUI();
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
