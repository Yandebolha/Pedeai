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
    public partial class frmCadastroCliente : Form
    {
        private ClienteBLL _bll;
        private int _codigoEditando = 0;
        private bool _formatingCpf = false;

        public frmCadastroCliente()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new ClienteBLL();
            Load += (_, __) => CarregarGrid();
        }

        private void TxtCpf_TextChanged(object sender, EventArgs e)
        {
            if (_formatingCpf) return;
            _formatingCpf = true;
            var digits = new string(System.Array.FindAll(txtCpf.Text.ToCharArray(), char.IsDigit));
            if (digits.Length > 14) digits = digits.Substring(0, 14);
            string fmt = FormatCpfCnpj(digits);
            int caret = txtCpf.SelectionStart;
            int diff   = fmt.Length - txtCpf.Text.Length;
            txtCpf.Text = fmt;
            txtCpf.SelectionStart = Math.Min(Math.Max(caret + diff, 0), fmt.Length);
            _formatingCpf = false;
        }

        private static string FormatCpfCnpj(string d)
        {
            if (d.Length <=  3) return d;
            if (d.Length <=  6) return $"{d.Substring(0,3)}.{d.Substring(3)}";
            if (d.Length <=  9) return $"{d.Substring(0,3)}.{d.Substring(3,3)}.{d.Substring(6)}";
            if (d.Length <= 11) return $"{d.Substring(0,3)}.{d.Substring(3,3)}.{d.Substring(6,3)}-{d.Substring(9)}";
            if (d.Length <= 12) return $"{d.Substring(0,2)}.{d.Substring(2,3)}.{d.Substring(5,3)}/{d.Substring(8)}";
            return $"{d.Substring(0,2)}.{d.Substring(2,3)}.{d.Substring(5,3)}/{d.Substring(8,4)}-{d.Substring(12)}";
        }

        private async void TxtCep_Leave(object sender, EventArgs e) => await BuscarCep();

        private async Task BuscarCep()
        {
            var cep = new string(System.Array.FindAll(txtCep.Text.ToCharArray(), char.IsDigit));
            if (cep.Length != 8) return;
            try
            {
                using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(6) })
                {
                    var json = await http.GetStringAsync("https://viacep.com.br/ws/" + cep + "/json/");
                    var obj = JObject.Parse(json);
                    if (obj["erro"] == null)
                    {
                        txtEndereco.Text = obj["logradouro"]?.ToString() ?? "";
                        txtBairro.Text   = obj["bairro"]?.ToString()     ?? "";
                        txtCidade.Text   = obj["localidade"]?.ToString() ?? "";
                        txtEstado.Text   = obj["uf"]?.ToString()         ?? "";
                        txtNumero.Focus();
                    }
                }
            }
            catch { }
        }

        private void CarregarGrid()
        {
            try
            {
                grid.DataSource = _bll.Listar(txtBusca?.Text?.Trim() ?? "");
                FormatarGrid();
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void FormatarGrid()
        {
            if (grid.Columns.Count == 0) return;

            // Hide unused columns before setting fill mode
            if (grid.Columns.Contains("Situacao")) grid.Columns["Situacao"].Visible = false;
            if (grid.Columns.Contains("Telefone")) grid.Columns["Telefone"].Visible = false;
            if (grid.Columns.Contains("Email"))    grid.Columns["Email"].Visible    = false;

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            void Col(string name, string header, float fillWeight, DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleLeft)
            {
                if (!grid.Columns.Contains(name)) return;
                grid.Columns[name].HeaderText = header;
                grid.Columns[name].FillWeight = fillWeight;
                grid.Columns[name].DefaultCellStyle.Alignment = align;
            }

            if (grid.Columns.Contains("Codigo")) grid.Columns["Codigo"].Visible = false;
            Col("Nome",          "Nome / Raz\u00e3o Social", 200);
            Col("Celular",       "Celular",              90,  DataGridViewContentAlignment.MiddleCenter);
            Col("Endereco",      "Endere\u00e7o",           180);
            Col("Cidade",        "Cidade",               90);
            Col("PedidosMensal", "Pedidos M\u00eas",         70,  DataGridViewContentAlignment.MiddleCenter);
            Col("TotalPedidos",  "Pedidos Total",         70,  DataGridViewContentAlignment.MiddleCenter);
            Col("GastoMensal",   "Gasto M\u00eas R$",        90,  DataGridViewContentAlignment.MiddleRight);
            Col("TotalGasto",    "Gasto Total R$",        90,  DataGridViewContentAlignment.MiddleRight);

            // Formata valores monetários
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                if (grid.Columns.Contains("TotalGasto") && row.Cells["TotalGasto"].Value is decimal d)
                    row.Cells["TotalGasto"].Value = d.ToString("N2");
                if (grid.Columns.Contains("GastoMensal") && row.Cells["GastoMensal"].Value is decimal dm)
                    row.Cells["GastoMensal"].Value = dm.ToString("N2");
            }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            foreach (Control c in pnlForm.Controls)
            {
                if (c is TextBox tb) tb.Clear();
                else if (c is MaskedTextBox mtb) mtb.Clear();
            }
            cmbSituacao.SelectedIndex = 0;
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando = cod;
            txtNome.Text        = obj.clieNome_RazaoSocial ?? "";
            txtCelular.Text     = obj.clieCelular ?? "";
            txtEmail.Text       = obj.clieEmail ?? "";
            txtCpf.Text         = obj.clieCPF_CNPJ_ ?? "";
            txtCep.Text         = obj.clieCEP ?? "";
            txtEndereco.Text    = obj.clieEndereco ?? "";
            txtNumero.Text      = obj.clieNumero ?? "";
            txtComplemento.Text = obj.clieComplemento ?? "";
            txtBairro.Text      = obj.clieBairro ?? "";
            txtCidade.Text      = obj.clieCidade ?? "";
            txtEstado.Text      = obj.clieEstado ?? "";
            cmbSituacao.SelectedIndex = (obj.Situacao == "I") ? 1 : 0;
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            // Validações obrigatórias
            if (string.IsNullOrWhiteSpace(txtNome.Text))
                { MessageBox.Show("Informe o nome / razão social.", "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtNome.Focus(); return; }

            var celDigs = new string(System.Array.FindAll(txtCelular.Text.ToCharArray(), char.IsDigit));
            if (celDigs.Length < 10)
                { MessageBox.Show("Informe o celular completo.", "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtCelular.Focus(); return; }

            var cepDigs = new string(System.Array.FindAll(txtCep.Text.ToCharArray(), char.IsDigit));
            if (cepDigs.Length < 8)
                { MessageBox.Show("Informe o CEP completo (8 dígitos).", "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtCep.Focus(); return; }

            if (string.IsNullOrWhiteSpace(txtNumero.Text))
                { MessageBox.Show("Informe o número do endereço.", "Campo obrigatório", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtNumero.Focus(); return; }
            var obj = new Cliente
            {
                Codigo               = _codigoEditando,
                clieNome_RazaoSocial = txtNome.Text.Trim(),
                clieTelefone         = "",
                clieCelular          = txtCelular.Text,
                clieEmail            = txtEmail.Text,
                clieCPF_CNPJ_        = txtCpf.Text,
                clieCEP              = txtCep.Text,
                clieEndereco         = txtEndereco.Text,
                clieNumero           = txtNumero.Text,
                clieComplemento      = txtComplemento.Text,
                clieBairro           = txtBairro.Text,
                clieCidade           = txtCidade.Text,
                clieEstado           = txtEstado.Text,
                Situacao             = cmbSituacao.SelectedIndex == 1 ? "I" : "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }

        private void lblCep_Click(object sender, EventArgs e)
        {

        }

        private void txtCep_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void lblEnd_Click(object sender, EventArgs e)
        {

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
