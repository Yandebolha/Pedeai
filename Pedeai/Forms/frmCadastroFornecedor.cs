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
        private System.Data.DataTable _allFornecedores;
        private System.Windows.Forms.TextBox _txtBuscaForn;

        public frmCadastroFornecedor()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new FornecedorBLL();
            Load += (_, __) => { AdicionarPainelBusca(); CarregarGrid(); };
        }

        private void AdicionarPainelBusca()
        {
            var pnlSearch = new System.Windows.Forms.Panel
            { Dock = System.Windows.Forms.DockStyle.Top, Height = 38,
              BackColor = System.Drawing.Color.FromArgb(235, 226, 208) };
            _txtBuscaForn = new System.Windows.Forms.TextBox
            { Left = 8, Top = 8, Width = 300,
              Font = new System.Drawing.Font("Segoe UI", 9.5F),
              PlaceholderText = "Buscar por razão social ou nome fantasia..." };
            var btnBuscar = new System.Windows.Forms.Button
            { Left = 316, Top = 7, Width = 90, Height = 26, Text = "Buscar",
              BackColor = System.Drawing.Color.FromArgb(224, 113, 42),
              ForeColor = System.Drawing.Color.White,
              FlatStyle = System.Windows.Forms.FlatStyle.Flat,
              Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
              Cursor = Cursors.Hand };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += (_, __) => FiltrarGrid(_txtBuscaForn.Text);
            _txtBuscaForn.KeyDown += (s, ev) => { if (ev.KeyCode == System.Windows.Forms.Keys.Enter) FiltrarGrid(_txtBuscaForn.Text); };
            pnlSearch.Controls.Add(_txtBuscaForn);
            pnlSearch.Controls.Add(btnBuscar);
            Controls.Add(pnlSearch);
            Controls.SetChildIndex(pnlSearch, 1);
        }

        private void CarregarGrid()
        {
            try
            {
                _allFornecedores = _bll.Listar();
                FiltrarGrid(_txtBuscaForn?.Text ?? "");
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void FiltrarGrid(string filtro)
        {
            if (_allFornecedores == null) return;
            var dv = new System.Data.DataView(_allFornecedores);
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var f = filtro.Replace("'", "''");
                dv.RowFilter = $"RazaoSocial LIKE '%{f}%' OR NomeFantasia LIKE '%{f}%'";
            }
            grid.DataSource = dv;
            if (grid.Columns.Contains("Codigo"))  grid.Columns["Codigo"].Visible  = false;
            if (grid.Columns.Contains("Contato")) grid.Columns["Contato"].Visible = false;
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
