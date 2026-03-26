using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    /// <summary>
    /// Dialog de autorizacao: solicita credenciais de um usuario Gerente ou Admin
    /// para liberar acoes que o operador atual nao pode realizar.
    /// </summary>
    public partial class frmAutorizacao : Form
    {
        private UsuarioBLL _bll;

        /// <summary>Usuario que autorizou a acao (preenchido apos OK).</summary>
        public Usuario UsuarioAutorizador { get; private set; }

        public frmAutorizacao()
        {
            InitializeComponent();
            if (!DesignMode) _bll = new UsuarioBLL();
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
                lblMsg.Text = "Usuario sem permissao para cancelar pedidos.";
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
    }
}
