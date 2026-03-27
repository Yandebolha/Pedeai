using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmEmpresa : Form
    {
        private EmpresaBLL  _empBLL;
        private UsuarioBLL  _usrBLL;
        private Empresa _empresa;
        private Usuario _usuarioEditando;

        public frmEmpresa()
        {
            InitializeComponent();
            _empBLL = new EmpresaBLL(); _usrBLL = new UsuarioBLL();
            Load += (_, __) => { CarregarEmpresa(); CarregarUsuarios(); SetModoEdicao(false); };
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
        private System.Collections.Generic.List<Usuario> _allUsuarios = new();
        private System.Collections.Generic.List<Usuario> _filtrados   = new();

        private void CarregarUsuarios()
        {
            _allUsuarios = _usrBLL.Listar();
            FiltrarLista("");
        }

        private void FiltrarLista(string filtro)
        {
            var f = filtro.ToLowerInvariant();
            _filtrados.Clear();
            lstUsuarios.Items.Clear();
            foreach (var u in _allUsuarios)
            {
                if (string.IsNullOrEmpty(f)
                    || u.usuNome.ToLowerInvariant().Contains(f)
                    || u.usuLogin.ToLowerInvariant().Contains(f))
                {
                    _filtrados.Add(u);
                    lstUsuarios.Items.Add($"{u.usuNome}  —  {u.usuLogin}");
                }
            }
        }

        private string LabelNivel(int nivel)
            => nivel switch { 9 => "Admin", 2 => "Gerente", _ => "Operador" };

        private void TxtPesquisa_TextChanged(object sender, EventArgs e)
            => FiltrarLista(txtPesquisa.Text.Trim());

        private void LstUsuarios_DoubleClick(object sender, EventArgs e)
        {
            if (lstUsuarios.SelectedIndex < 0 || lstUsuarios.SelectedIndex >= _filtrados.Count) return;
            CarregarUsuarioSelecionado(_filtrados[lstUsuarios.SelectedIndex].Codigo);
        }

        private void CarregarUsuarioSelecionado(int codigo)
        {
            _usuarioEditando = _usrBLL.PesquisaCodigo(codigo);
            if (_usuarioEditando == null) return;
            txtUsrNome.Text      = _usuarioEditando.usuNome;
            txtUsrLogin.Text     = _usuarioEditando.usuLogin;
            txtUsrSenha.Text     = "";
            txtUsrSenhaConf.Text = "";
            cmbUsrNivel.SelectedIndex = _usuarioEditando.usuNivel == 9 ? 2
                                      : _usuarioEditando.usuNivel == 2 ? 1 : 0;
            cmbUsrSit.SelectedIndex = _usuarioEditando.Situacao == "A" ? 0 : 1;
            CarregarPermissoes(_usuarioEditando.Info);
            pnlBuscaUsuarios.Visible = false;
            SetModoEdicao(true);
        }

        private void BtnPesquisarUsuario_Click(object sender, EventArgs e)
        {
            CarregarUsuarios();
            txtPesquisa.Clear();
            pnlBuscaUsuarios.Visible = !pnlBuscaUsuarios.Visible;
            if (pnlBuscaUsuarios.Visible) txtPesquisa.Focus();
        }

        private void BtnNovoUsuario_Click(object sender, EventArgs e)
        {
            _usuarioEditando = null;
            LimparFormUsuario();
            SetModoEdicao(true);
            txtUsrNome.Focus();
        }

        private void BtnCancelarUsuario_Click(object sender, EventArgs e)
        {
            LimparFormUsuario();
            SetModoEdicao(false);
        }

        private void SetModoEdicao(bool editando)
        {
            // Pesquisar is always visible; Novo shown in idle; Salvar+Cancelar shown when editing
            btnNovoUsr.Visible      = !editando;
            btnPesquisarUsr.Visible = true;           // always visible
            btnSalvUsr.Visible      = editando;
            btnCancelarUsr.Visible  = editando;
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
                Situacao  = cmbUsrSit.SelectedIndex == 0 ? "A" : "I",
                Info      = construirPermissoes()
            };

            bool alteraSenha = !string.IsNullOrWhiteSpace(txtUsrSenha.Text);
            var erro = _usrBLL.Salvar(obj, alteraSenha);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }

            // Se editou o proprio usuario logado, atualiza a sessao e reconstroi o sidebar
            if (UsuarioSessao.UsuarioAtual != null && obj.Codigo == UsuarioSessao.UsuarioAtual.Codigo)
            {
                var atualizado = _usrBLL.PesquisaCodigo(obj.Codigo);
                if (atualizado != null)
                {
                    UsuarioSessao.Iniciar(atualizado);
                    if (Owner is Form1 f1) f1.ReconstruirSidebar();
                    else foreach (Form frm in Application.OpenForms)
                        if (frm is Form1 main) { main.ReconstruirSidebar(); break; }
                }
            }

            MessageBox.Show("Usuario salvo com sucesso!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimparFormUsuario();
            SetModoEdicao(false);
        }

        private string construirPermissoes()
        {
            var mods = new System.Collections.Generic.List<string>();
            if (chkModDashboard.Checked)       mods.Add("Dashboard");
            if (chkModPedidos.Checked)          mods.Add("Pedidos");
            if (chkModFinanceiro.Checked)       mods.Add("Financeiro");
            if (chkModProdutos.Checked)         mods.Add("Produtos");
            if (chkModCategorias.Checked)       mods.Add("Categorias");
            if (chkModClientes.Checked)         mods.Add("Clientes");
            if (chkModFornecedores.Checked)     mods.Add("Fornecedores");
            if (chkModCupons.Checked)           mods.Add("Cupons");
            if (chkModEmpresa.Checked)          mods.Add("Empresa");
            if (chkModCancelarPedidos.Checked)  mods.Add("CancelarPedidos");
            return string.Join(",", mods);
        }

        private void CarregarPermissoes(string info)
        {
            // Admin sem Info configurado: marcar tudo por padrao
            bool adminSemInfo = (_usuarioEditando?.usuNivel >= 9) && string.IsNullOrWhiteSpace(info);
            var ativos = (info ?? "").Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            bool tem(string m) => adminSemInfo || System.Array.Exists(ativos, x => x.Trim().Equals(m, System.StringComparison.OrdinalIgnoreCase));
            chkModDashboard.Checked       = tem("Dashboard");
            chkModPedidos.Checked          = tem("Pedidos");
            chkModFinanceiro.Checked       = tem("Financeiro");
            chkModProdutos.Checked         = tem("Produtos");
            chkModCategorias.Checked       = tem("Categorias");
            chkModClientes.Checked         = tem("Clientes");
            chkModFornecedores.Checked     = tem("Fornecedores");
            chkModCupons.Checked           = tem("Cupons");
            chkModEmpresa.Checked          = tem("Empresa");
            chkModCancelarPedidos.Checked  = tem("CancelarPedidos");
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
            CarregarPermissoes("");
        }
    }
}
