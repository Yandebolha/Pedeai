using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmSelecionarCliente : Form
    {
        private readonly ClienteBLL _bll = new ClienteBLL();
        private int _codigoEditando = 0;

        /// <summary>Cliente escolhido pelo usuário. Não-nulo somente quando DialogResult == OK.</summary>
        public Cliente ClienteSelecionado { get; private set; }

        public frmSelecionarCliente()
        {
            InitializeComponent();
            if (!DesignMode) CarregarGrid();
        }

        // ── Grid ─────────────────────────────────────────────────────────────

        private void CarregarGrid()
        {
            try { grid.DataSource = _bll.Listar(txtBusca?.Text?.Trim() ?? ""); }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar clientes: " + ex.Message); }
        }

        private void BtnBuscar_Click(object sender, EventArgs e) => CarregarGrid();

        private void TxtBusca_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) CarregarGrid();
        }

        // ── Seleção ───────────────────────────────────────────────────────────

        private void BtnSelecionar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { MessageBox.Show("Selecione um cliente na lista."); return; }
            ConfirmarSelecao();
        }

        private void Grid_DoubleClick(object sender, EventArgs e) => ConfirmarSelecao();

        private void ConfirmarSelecao()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) { MessageBox.Show("Cliente não encontrado."); return; }
            ClienteSelecionado = obj;
            DialogResult = DialogResult.OK;
            Close();
        }

        // ── Novo Cliente (painel embutido) ────────────────────────────────────

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            _codigoEditando = 0;
            txtNomeCad.Clear();
            txtTelefoneCad.Clear();
            txtCelularCad.Clear();
            txtEmailCad.Clear();
            txtCpfCad.Clear();
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Visible = true;
            txtNomeCad.Focus();
        }

        private void BtnSalvarCad_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNomeCad.Text))
            {
                MessageBox.Show("Informe o nome do cliente.");
                return;
            }

            var obj = new Cliente
            {
                Codigo               = _codigoEditando,
                clieNome_RazaoSocial = txtNomeCad.Text.Trim(),
                clieTelefone         = txtTelefoneCad.Text,
                clieCelular          = txtCelularCad.Text,
                clieEmail            = txtEmailCad.Text,
                clieCPF_CNPJ_        = txtCpfCad.Text,
                Situacao             = cmbSituacao.SelectedIndex == 1 ? "I" : "A",
            };

            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }

            pnlForm.Visible = false;
            _codigoEditando = 0;
            CarregarGrid();
        }

        private void BtnCancelarCad_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            _codigoEditando = 0;
        }
    }
}
