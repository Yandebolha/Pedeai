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
        private bool _formatandoCnpj = false;

        public frmCadastroFornecedor()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new FornecedorBLL();
            Load += (_, __) => CarregarGrid();
        }

        private void CarregarGrid()
        {
            try
            {
                grid.DataSource = _bll.Listar();
                if (grid.Columns.Contains("Codigo")) grid.Columns["Codigo"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            foreach (Control c in pnlForm.Controls)
            {
                if (c is System.Windows.Forms.TextBoxBase tb) tb.Clear();
            }
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

        private void AplicarMascaraCnpj()
        {
            if (_formatandoCnpj) return;
            _formatandoCnpj = true;
            var digits = new string(System.Array.FindAll(txtCnpj.Text.ToCharArray(), char.IsDigit));
            if (digits.Length > 14) digits = digits.Substring(0, 14);
            string fmt = FormatarCpfCnpj(digits);
            int diff = fmt.Length - txtCnpj.Text.Length;
            int caret = txtCnpj.SelectionStart;
            txtCnpj.Text = fmt;
            txtCnpj.SelectionStart = Math.Min(Math.Max(caret + diff, 0), fmt.Length);
            _formatandoCnpj = false;
        }

        private static string FormatarCpfCnpj(string d)
        {
            if (d.Length <=  3) return d;
            if (d.Length <=  6) return $"{d.Substring(0,3)}.{d.Substring(3)}";
            if (d.Length <=  9) return $"{d.Substring(0,3)}.{d.Substring(3,3)}.{d.Substring(6)}";
            if (d.Length <= 11) return $"{d.Substring(0,3)}.{d.Substring(3,3)}.{d.Substring(6,3)}-{d.Substring(9)}";
            if (d.Length <= 12) return $"{d.Substring(0,2)}.{d.Substring(2,3)}.{d.Substring(5,3)}/{d.Substring(8)}";
            return $"{d.Substring(0,2)}.{d.Substring(2,3)}.{d.Substring(5,3)}/{d.Substring(8,4)}-{d.Substring(12)}";
        }

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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
