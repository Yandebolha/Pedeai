using System;
using System.Collections.Generic;
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
        private ConfiguracaoImpressaoBLL _impBLL;
        private Empresa _empresa;
        private Usuario _usuarioEditando;

        public frmEmpresa()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _empBLL = new EmpresaBLL();
            _usrBLL = new UsuarioBLL();
            _impBLL = new ConfiguracaoImpressaoBLL();
            Load += (_, __) =>
            {
                CarregarEmpresa();
                CarregarUsuarios();
                CarregarConfiguracaoImpressao();
                AjustarNiveisCombo(isUsuario1: false);
                SetModoEdicao(false);
                // Aba "Sistema" visível apenas para Admin (nivel 9)
                if (!UsuarioSessao.TemNivel(9))
                    tabControl.TabPages.Remove(tabSistema);
            };
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

        /// Garante que "Admin" só aparece no combo quando editando o usuário 1.
        private void AjustarNiveisCombo(bool isUsuario1)
        {
            bool temAdmin = cmbUsrNivel.Items.Count == 3;
            if (isUsuario1 && !temAdmin)
                cmbUsrNivel.Items.Add("Admin");
            else if (!isUsuario1 && temAdmin)
                cmbUsrNivel.Items.RemoveAt(2);
        }

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
            AjustarNiveisCombo(isUsuario1: _usuarioEditando.Codigo == 1);
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
            AjustarNiveisCombo(isUsuario1: false);
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
            if (chkModEstoque.Checked)          mods.Add("Estoque");
            if (chkModCancelarPedidos.Checked)   mods.Add("CancelarPedidos");
            if (chkModEntradaMercadoria.Checked) mods.Add("EntradaMercadoria");
            if (chkModAvisos.Checked)           mods.Add("Avisos");
            if (chkModTurno.Checked)            mods.Add("Turno");
            if (chkModConsultarPedido.Checked)  mods.Add("ConsultarPedido");
            if (chkModFidelizacao.Checked)       mods.Add("Fidelizacao");
            if (chkModWhatsApp.Checked)            mods.Add("WhatsApp");
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
            chkModEstoque.Checked          = tem("Estoque");
            chkModCancelarPedidos.Checked   = tem("CancelarPedidos");
            chkModEntradaMercadoria.Checked = tem("EntradaMercadoria");
            chkModAvisos.Checked            = tem("Avisos");
            chkModTurno.Checked             = tem("Turno");
            chkModConsultarPedido.Checked    = tem("ConsultarPedido");
            chkModFidelizacao.Checked        = tem("Fidelizacao");
            chkModWhatsApp.Checked             = tem("WhatsApp");
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

        // ── ABA IMPRESSAO ─────────────────────────────────────────────────────

        private void CarregarConfiguracaoImpressao()
        {
            var cfg = _impBLL.Carregar();
            txtImpNomeEmpresa.Text  = cfg.cabNomeEmpresa;
            txtImpEndereco.Text     = cfg.cabEndereco;
            txtImpTelefone.Text     = cfg.cabTelefone;
            txtImpCNPJ.Text         = cfg.cabCNPJ;
            txtImpSeparador.Text    = cfg.separador;
            txtImpAvisoFiscal.Text  = cfg.rodapeAvisoFiscal;
            txtImpRodapeTexto.Text  = cfg.rodapeTextoLivre;
            txtImpLblNumero.Text    = cfg.lblNumeroPedido;
            txtImpLblColItem.Text   = cfg.lblColunaItem;
            txtImpLblColTotal.Text  = cfg.lblColunaTotal;
            txtImpLblSubtotal.Text  = cfg.lblSubtotal;
            txtImpLblTaxa.Text      = cfg.lblTaxaEntrega;
            txtImpLblDesconto.Text  = cfg.lblDesconto;
            txtImpLblCupom.Text     = cfg.lblCupom;
            txtImpLblTotalPagar.Text= cfg.lblTotalPagar;
            txtImpLblAtendente.Text = cfg.lblAtendente;
            numLargura.Value        = System.Math.Max(20, cfg.larguraCaracteres);

            // Selecionar impressora salva
            cmbImpressora.SelectedIndex = 0;
            if (!string.IsNullOrWhiteSpace(cfg.impressoraNome))
            {
                for (int i = 1; i < cmbImpressora.Items.Count; i++)
                {
                    if (cmbImpressora.Items[i].ToString() == cfg.impressoraNome)
                    {
                        cmbImpressora.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private ConfiguracaoImpressao ObterConfiguracaoImpressao()
        {
            return new ConfiguracaoImpressao
            {
                cabNomeEmpresa    = txtImpNomeEmpresa.Text.Trim(),
                cabEndereco       = txtImpEndereco.Text.Trim(),
                cabTelefone       = txtImpTelefone.Text.Trim(),
                cabCNPJ           = txtImpCNPJ.Text.Trim(),
                separador         = txtImpSeparador.Text.Trim().Length > 0 ? txtImpSeparador.Text.Trim() : "-",
                rodapeAvisoFiscal = txtImpAvisoFiscal.Text.Trim(),
                rodapeTextoLivre  = txtImpRodapeTexto.Text.Trim(),
                lblNumeroPedido   = txtImpLblNumero.Text.Trim(),
                lblColunaItem     = txtImpLblColItem.Text.Trim(),
                lblColunaTotal    = txtImpLblColTotal.Text.Trim(),
                lblSubtotal       = txtImpLblSubtotal.Text.Trim(),
                lblTaxaEntrega    = txtImpLblTaxa.Text.Trim(),
                lblDesconto       = txtImpLblDesconto.Text.Trim(),
                lblCupom          = txtImpLblCupom.Text.Trim(),
                lblTotalPagar     = txtImpLblTotalPagar.Text.Trim(),
                lblAtendente      = txtImpLblAtendente.Text.Trim(),
                larguraCaracteres = (int)numLargura.Value,
                impressoraNome    = cmbImpressora.SelectedIndex <= 0 ? "" : cmbImpressora.SelectedItem.ToString()
            };
        }

        private void BtnSalvarImpressao_Click(object sender, EventArgs e)
        {
            var cfg  = ObterConfiguracaoImpressao();
            var erro = _impBLL.Salvar(cfg);
            if (!string.IsNullOrEmpty(erro))
                MessageBox.Show("Erro ao salvar: " + erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("Configuração de impressão salva!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnImprimirTeste_Click(object sender, EventArgs e)
        {
            var cfg     = ObterConfiguracaoImpressao();
            var empresa = _empBLL.Carregar();

            // Pedido de teste ficticio
            var pedTeste = new PedidoWeb
            {
                pediNumero            = "TESTE001",
                pediNome_Cliente      = "Cliente Teste",
                pediTelefone_Cliente  = "(11) 99999-9999",
                pediTipo_Entrega      = 1,
                pediEndereco_Entrega  = "Rua Exemplo, 100 - Centro",
                pediSubtotal          = 32.90m,
                pediTaxa_Entrega      = 5.00m,
                pediValor_Total       = 37.90m,
                pediObservacoes       = "Sem cebola",
                pediData_Lancamento   = DateTime.Now,
                pediForma_Pagamento   = 0
            };
            var itensTeste = new List<ItemPedidoWeb>
            {
                new ItemPedidoWeb
                {
                    itpwNome_Mercadoria = "Chesse Barbecue",
                    itpwQtde            = 1,
                    itpwPreco_Unitario  = 32.90m,
                    itpwSubtotal        = 32.90m
                }
            };

            var erro = ImpressaoPedido.Imprimir(pedTeste, itensTeste, cfg, empresa, "Atendente");
            if (!string.IsNullOrEmpty(erro))
                MessageBox.Show("Erro ao imprimir: " + erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void frmEmpresa_Load(object sender, EventArgs e)
        {

        }

        // ── ABA SISTEMA ───────────────────────────────────────────────────────
        private void BtnResetarBanco_Click(object sender, EventArgs e)
        {
            if (!UsuarioSessao.TemNivel(9))
            {
                MessageBox.Show("Apenas administradores podem executar esta ação.", "Acesso negado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ── Solicitar senha do admin ──────────────────────────────────────
            string senhaDigitada;
            using (var dlg = new Form())
            {
                dlg.Text = "Confirmar Identidade";
                dlg.Size = new Size(360, 150);
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.MaximizeBox = false; dlg.MinimizeBox = false;
                var lbl = new Label { Text = "Senha do administrador:", Left = 20, Top = 18, AutoSize = true };
                var txt = new TextBox { Left = 20, Top = 40, Width = 300, UseSystemPasswordChar = true };
                var btnOk  = new Button { Text = "Confirmar", DialogResult = DialogResult.OK,     Left = 130, Top = 76, Width = 100, Height = 28 };
                var btnCan = new Button { Text = "Cancelar",  DialogResult = DialogResult.Cancel,  Left = 240, Top = 76, Width = 80,  Height = 28 };
                dlg.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCan });
                dlg.AcceptButton = btnOk; dlg.CancelButton = btnCan;
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                senhaDigitada = txt.Text;
            }
            if (new UsuarioBLL().Autenticar("admin", senhaDigitada) == null)
            {
                MessageBox.Show("Senha do administrador incorreta.", "Acesso negado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var r1 = MessageBox.Show(
                "ATENÇÃO!\n\nEsta ação irá apagar TODOS os dados do sistema:\n" +
                "• Pedidos\n• Clientes\n• Fornecedores\n• Mercadorias\n• Entradas\n" +
                "• Estoque\n• Cupons\n• Gastos\n• Turnos\n• Todos os usuários\n\n" +
                "Após o reset, o único acesso será:\n" +
                "  Login: admin  |  Senha: $up0rte\n\nDeseja continuar?",
                "Confirmar Reset do Banco",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (r1 != DialogResult.Yes) return;

            var r2 = MessageBox.Show(
                "ÚLTIMA CONFIRMAÇÃO!\n\nEsta operação é IRREVERSÍVEL.\n" +
                "Todos os dados serão perdidos permanentemente.\n\nTem absoluta certeza?",
                "Confirmar Reset — Última Chance",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop);
            if (r2 != DialogResult.Yes) return;

            Cursor = Cursors.WaitCursor;
            var erro = DB.DbMigrator.ResetarBanco();
            Cursor = Cursors.Default;

            if (!string.IsNullOrEmpty(erro))
            {
                MessageBox.Show("Erro ao resetar banco:\n" + erro, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Banco de dados resetado com sucesso!\nO sistema está pronto para uso.", "Concluído",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CarregarEmpresa();
                CarregarConfiguracaoImpressao();
            }
        }
    }
}
