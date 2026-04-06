using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroFornecedor : Form
    {
        private FornecedorBLL _bll;
        private int _codigoEditando = 0;

        public frmCadastroFornecedor()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new FornecedorBLL();
            Load += (_, __) => CarregarGrid();
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
            txtCep.Text      = obj.fornCEP ?? "";
            txtEndereco.Text = obj.fornEndereco ?? "";
            txtNumero.Text   = obj.fornNumero ?? "";
            txtBairro.Text   = obj.fornBairro ?? "";
            txtCidade.Text   = obj.fornCidade ?? "";
            txtEstado.Text   = obj.fornEstado ?? "";
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
                fornContato               = "",
                fornCEP                   = txtCep.Text,
                fornEndereco              = txtEndereco.Text,
                fornNumero                = txtNumero.Text,
                fornBairro                = txtBairro.Text,
                fornCidade                = txtCidade.Text,
                fornEstado                = txtEstado.Text,
                fornObservacoes           = "",
                Situacao                  = cmbSituacao.SelectedItem?.ToString() ?? "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }
        private void TxtCep_Leave(object sender, EventArgs e) => _ = BuscarCepForn();

        private async Task BuscarCepForn()
        {
            var cep = new string(System.Array.FindAll(txtCep.Text.ToCharArray(), char.IsDigit));
            if (cep.Length != 8) return;
            try
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(6) };
                var json = await http.GetStringAsync("https://viacep.com.br/ws/" + cep + "/json/");
                var obj = JObject.Parse(json);
                if (obj["erro"] == null)
                {
                    txtEndereco.Text = obj["logradouro"]?.ToString() ?? "";
                    txtBairro.Text   = obj["bairro"]?.ToString() ?? "";
                    txtCidade.Text   = obj["localidade"]?.ToString() ?? "";
                    txtEstado.Text   = obj["uf"]?.ToString() ?? "";
                    txtNumero.Focus();
                }
            }
            catch { /* ignora falhas de rede */ }
        }
    }
}
