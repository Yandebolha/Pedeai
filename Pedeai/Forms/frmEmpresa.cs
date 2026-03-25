using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmEmpresa : Form
    {
        private readonly EmpresaBLL  _empBLL = new EmpresaBLL();
        private readonly UsuarioBLL  _usrBLL = new UsuarioBLL();
        private Empresa _empresa;
        private Usuario _usuarioEditando;

        public frmEmpresa()
        {
            InitializeComponent();
            if (!DesignMode) { CarregarEmpresa(); CarregarUsuarios(); }
        }

        // ── ABA EMPRESA ──────────────────────────────────────────────────────
        private void CarregarEmpresa()
        {
            _empresa = _empBLL.Carregar();
            txtEmpNome.Text     = _empresa.empNome;
            txtEmpFantasia.Text = _empresa.empNome_Fantasia;
            txtEmpCNPJ.Text     = _empresa.empCNPJ;
            txtEmpTel.Text      = _empresa.empTelefone;
            txtEmpEmail.Text    = _empresa.empEmail;
            txtEmpEnd.Text      = _empresa.empEndereco;
        }

        private void BtnSalvarEmpresa_Click(object sender, EventArgs e)
        {
            _empresa.empNome          = txtEmpNome.Text.Trim();
            _empresa.empNome_Fantasia = txtEmpFantasia.Text.Trim();
            _empresa.empCNPJ          = txtEmpCNPJ.Text.Trim();
            _empresa.empTelefone      = txtEmpTel.Text.Trim();
            _empresa.empEmail         = txtEmpEmail.Text.Trim();
            _empresa.empEndereco      = txtEmpEnd.Text.Trim();
            var erro = _empBLL.Salvar(_empresa);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            MessageBox.Show("Dados da empresa salvos com sucesso!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ── ABA USUARIOS ─────────────────────────────────────────────────────
        private void CarregarUsuarios()
        {
            var lista = _usrBLL.Listar();
            gridUsuarios.DataSource = null;
            var dt = new System.Data.DataTable();
            dt.Columns.Add("Codigo",   typeof(int));
            dt.Columns.Add("Nome",     typeof(string));
            dt.Columns.Add("Login",    typeof(string));
            dt.Columns.Add("Nivel",    typeof(string));
            dt.Columns.Add("Situacao", typeof(string));
            foreach (var u in lista)
                dt.Rows.Add(u.Codigo, u.usuNome, u.usuLogin, LabelNivel(u.usuNivel), u.Situacao);
            gridUsuarios.DataSource = dt;
            if (gridUsuarios.Columns.Contains("Codigo")) gridUsuarios.Columns["Codigo"].Visible = false;
            LimparFormUsuario();
        }

        private string LabelNivel(int nivel)
        {
            return nivel switch { 9 => "Admin", 2 => "Gerente", _ => "Operador" };
        }

        private void GridUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (gridUsuarios.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(gridUsuarios.SelectedRows[0].Cells["Codigo"].Value);
            _usuarioEditando = _usrBLL.PesquisaCodigo(cod);
            if (_usuarioEditando == null) return;
            txtUsrNome.Text         = _usuarioEditando.usuNome;
            txtUsrLogin.Text        = _usuarioEditando.usuLogin;
            txtUsrSenha.Text        = "";
            txtUsrSenhaConf.Text    = "";
            cmbUsrNivel.SelectedIndex = _usuarioEditando.usuNivel == 9 ? 2
                                      : _usuarioEditando.usuNivel == 2 ? 1 : 0;
            cmbUsrSit.SelectedIndex = _usuarioEditando.Situacao == "A" ? 0 : 1;
        }

        private void BtnNovoUsuario_Click(object sender, EventArgs e)
        {
            _usuarioEditando = null;
            LimparFormUsuario();
            txtUsrNome.Focus();
        }

        private void BtnSalvarUsuario_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtUsrSenha.Text) && txtUsrSenha.Text != txtUsrSenhaConf.Text)
            {
                MessageBox.Show("Senhas nao conferem."); return;
            }

            int[] nivelMap = { 1, 2, 9 };
            var obj = new Usuario
            {
                Codigo    = _usuarioEditando?.Codigo ?? 0,
                usuNome   = txtUsrNome.Text.Trim(),
                usuLogin  = txtUsrLogin.Text.Trim(),
                usuSenha  = txtUsrSenha.Text,
                usuNivel  = nivelMap[cmbUsrNivel.SelectedIndex],
                Situacao  = cmbUsrSit.SelectedIndex == 0 ? "A" : "I"
            };

            bool alteraSenha = !string.IsNullOrWhiteSpace(txtUsrSenha.Text);
            var erro = _usrBLL.Salvar(obj, alteraSenha);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }

            MessageBox.Show("Usuario salvo com sucesso!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CarregarUsuarios();
        }

        private void LimparFormUsuario()
        {
            _usuarioEditando     = null;
            txtUsrNome.Text      = "";
            txtUsrLogin.Text     = "";
            txtUsrSenha.Text     = "";
            txtUsrSenhaConf.Text = "";
            cmbUsrNivel.SelectedIndex = 0;
            cmbUsrSit.SelectedIndex   = 0;
        }
    }
}
