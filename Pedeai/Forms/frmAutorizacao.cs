using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    /// <summary>
    /// Dialog de autorizacao: solicita credenciais de um usuario Gerente ou Admin (nivel >= 2).
    /// </summary>
    public partial class frmAutorizacao : Form
    {
        private UsuarioBLL _bll;

        /// <summary>Usuario que autorizou a acao (preenchido apos OK).</summary>
        public Usuario UsuarioAutorizador { get; private set; }

        public frmAutorizacao()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new UsuarioBLL();
        }

        // ── Resolucao do login (igual ao formulario de login) ─────────────────
        private bool ResolverLogin()
        {
            lblMsg.Text = "";
            var texto = txtLogin.Text.Trim();
            if (string.IsNullOrEmpty(texto)) { lblNomeUsuario.Text = ""; return false; }

            try
            {
                string nome = "";

                if (int.TryParse(texto, out int cod) && cod > 0)
                    nome = _bll.BuscarNomePorCodigo(cod);

                if (string.IsNullOrEmpty(nome))
                    nome = _bll.BuscarNomePorLogin(texto);

                if (!string.IsNullOrEmpty(nome))
                {
                    txtLogin.Text = nome;
                    txtLogin.SelectionStart = nome.Length;
                    lblNomeUsuario.ForeColor = Color.FromArgb(39, 174, 96);
                    lblNomeUsuario.Text = "\u2713 Usuario identificado";
                    return true;
                }
                else
                {
                    lblNomeUsuario.ForeColor = Color.FromArgb(231, 76, 60);
                    lblNomeUsuario.Text = "Usuario nao encontrado";
                    return false;
                }
            }
            catch { lblNomeUsuario.Text = ""; return false; }
        }

        private void TxtLogin_Leave(object sender, EventArgs e) => ResolverLogin();

        private void TxtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (ResolverLogin()) txtSenha.Focus();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void BtnAutorizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                lblMsg.Text = "Preencha login e senha.";
                return;
            }

            var usu = _bll.Autenticar(txtLogin.Text.Trim(), txtSenha.Text);
            if (usu == null)
            {
                lblMsg.Text = "Credenciais incorretas.";
                txtSenha.Clear();
                return;
            }

            if (usu.usuNivel < 2)
            {
                lblMsg.Text = "Usuário não possui essa permissão.";
                txtSenha.Clear();
                return;
            }

            UsuarioAutorizador = usu;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void TxtSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) BtnAutorizar_Click(null, null);
        }

        /// <summary>Verifica se o usuario possui o modulo informado no campo Info.</summary>
        private static bool TemPermissao(Usuario usu, string modulo)
        {
            if (usu.usuNivel >= 9 && string.IsNullOrWhiteSpace(usu.Info)) return true;
            if (string.IsNullOrWhiteSpace(usu.Info)) return false;
            foreach (var p in usu.Info.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                if (p.Trim().Equals(modulo, System.StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
