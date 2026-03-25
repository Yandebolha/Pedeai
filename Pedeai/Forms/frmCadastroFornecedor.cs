using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroFornecedor : Form
    {
        private readonly FornecedorBLL _bll = new FornecedorBLL();
        private int _codigoEditando = 0;

        public frmCadastroFornecedor()
        {
            InitializeComponent();
            BuildUI();
            if (!DesignMode) CarregarGrid();
        }

        private void CarregarGrid()
        {
            try { grid.DataSource = _bll.Listar(); }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            foreach (var c in pnlForm.Controls) if (c is TextBox tb) tb.Clear();
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Visible = true; txtRazao.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando = cod;
            txtRazao.Text    = obj.fornNome_RazaoSocial ?? "";
            txtFantasia.Text = obj.fornApelido_Fantasia ?? "";
            txtCnpj.Text     = obj.fornCPF_CNPJ_ ?? "";
            txtIe.Text       = obj.fornRG_InscricaoEstadual ?? "";
            txtTelefone.Text = obj.fornTelefone ?? "";
            txtEmail.Text    = obj.fornEmail ?? "";
            txtContato.Text  = obj.fornContato ?? "";
            txtCep.Text      = obj.fornCEP ?? "";
            txtEndereco.Text = obj.fornEndereco ?? "";
            txtNumero.Text   = obj.fornNumero ?? "";
            txtBairro.Text   = obj.fornBairro ?? "";
            txtCidade.Text   = obj.fornCidade ?? "";
            txtEstado.Text   = obj.fornEstado ?? "";
            txtObs.Text      = obj.fornObservacoes ?? "";
            cmbSituacao.SelectedItem = obj.Situacao ?? "A";
            pnlForm.Visible = true; txtRazao.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRazao.Text)) { MessageBox.Show("Informe a Razão Social."); return; }
            var obj = new Fornecedor
            {
                Codigo                    = _codigoEditando,
                fornNome_RazaoSocial      = txtRazao.Text.Trim(),
                fornApelido_Fantasia      = txtFantasia.Text,
                fornCPF_CNPJ_             = txtCnpj.Text,
                fornRG_InscricaoEstadual  = txtIe.Text,
                fornTelefone              = txtTelefone.Text,
                fornEmail                 = txtEmail.Text,
                fornContato               = txtContato.Text,
                fornCEP                   = txtCep.Text,
                fornEndereco              = txtEndereco.Text,
                fornNumero                = txtNumero.Text,
                fornBairro                = txtBairro.Text,
                fornCidade                = txtCidade.Text,
                fornEstado                = txtEstado.Text,
                fornObservacoes           = txtObs.Text,
                Situacao                  = cmbSituacao.SelectedItem?.ToString() ?? "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }
    }
}
