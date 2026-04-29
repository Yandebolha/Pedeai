using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroBairro : Form
    {
        private BairroBLL   _bll;
        private int         _codigoEditando = 0;
        private BindingSource _bs = new BindingSource();

        public frmCadastroBairro()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new BairroBLL();
            Load += (_, __) =>
            {
                _bll.EnsureMigrations();
                Carregar();
            };
        }

        private void Carregar()
        {
            try
            {
                var dt = _bll.Listar();
                _bs.DataSource = dt;
                grid.DataSource = _bs;
                foreach (DataGridViewColumn col in grid.Columns)
                    col.Visible = col.Name != "Codigo";
                AplicarFiltro();
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void AplicarFiltro()
        {
            string termoBairro = txtBusca.Text.Trim();
            string termoCidade = txtCidFiltro?.Text.Trim() ?? "";
            if (_bs.DataSource is DataTable dt)
            {
                var filtros = new System.Collections.Generic.List<string>();
                if (!string.IsNullOrEmpty(termoBairro))
                    filtros.Add($"Bairro LIKE '%{termoBairro.Replace("'", "''")}%'");
                if (!string.IsNullOrEmpty(termoCidade))
                    filtros.Add($"Cidade LIKE '%{termoCidade.Replace("'", "''")}%'");
                dt.DefaultView.RowFilter = string.Join(" AND ", filtros);
            }
        }

        private void TxtBusca_TextChanged(object sender, EventArgs e) => AplicarFiltro();

        private void BtnPesquisar_Click(object sender, EventArgs e) => AplicarFiltro();

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                if (pnlForm.Visible) { pnlForm.Visible = false; _codigoEditando = 0; return true; }
                Close(); return true;
            }
            if (keyData == Keys.F2) { ModoNovo(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async void TxtCEP_Leave(object sender, EventArgs e) => await BuscarCepViaCep();

        private void TxtCEP_TextChanged(object sender, EventArgs e)
        {
            var digits = new string(System.Array.FindAll(txtCEP.Text.ToCharArray(), char.IsDigit));
            // Format as 00000-000 while typing
            if (digits.Length > 5)
            {
                string fmt = digits.Substring(0, 5) + "-" + digits.Substring(5, Math.Min(3, digits.Length - 5));
                if (txtCEP.Text != fmt) { txtCEP.Text = fmt; txtCEP.SelectionStart = fmt.Length; }
            }
            if (digits.Length == 8)
                _ = BuscarCepViaCep();
        }

        private async System.Threading.Tasks.Task BuscarCepViaCep()
        {
            var cep = new string(System.Array.FindAll(txtCEP.Text.ToCharArray(), char.IsDigit));
            if (cep.Length != 8) return;
            try
            {
                using var http = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(6) };
                var json = await http.GetStringAsync("https://viacep.com.br/ws/" + cep + "/json/");
                var obj  = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (obj["erro"] == null)
                {
                    txtCidade.Text = obj["localidade"]?.ToString() ?? txtCidade.Text;
                    txtBairro.Text = obj["bairro"]?.ToString()     ?? txtBairro.Text;
                    numTaxa.Focus();
                }
            }
            catch { }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            txtCEP.Clear();
            txtCidade.Clear();
            txtBairro.Clear();
            numTaxa.Value = 0;
            cmbSituacao.SelectedItem = "A";
            pnlForm.Visible = true;
            txtCEP.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando      = cod;
            txtCidade.Text       = obj.baiCidade ?? "";
            txtBairro.Text       = obj.baiNome ?? "";
            numTaxa.Value        = obj.baiTaxa_Entrega;
            txtCEP.Text          = obj.baiCEP ?? "";
            cmbSituacao.SelectedItem = obj.Situacao ?? "A";
            pnlForm.Visible      = true;
            txtBairro.Focus();
        }

        private void BtnNovo_Click(object sender, EventArgs e) => ModoNovo();

        private void BtnEditar_Click(object sender, EventArgs e) => CarregarParaEditar();

        private void Grid_DoubleClick(object sender, EventArgs e) => CarregarParaEditar();

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e) { }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            var obj = new Bairro
            {
                Codigo          = _codigoEditando,
                baiCidade       = txtCidade.Text.Trim(),
                baiNome         = txtBairro.Text.Trim(),
                baiTaxa_Entrega = numTaxa.Value,
                baiCEP          = txtCEP.Text.Replace("-", "").Trim(),
                Situacao        = _codigoEditando == 0 ? "A" : (cmbSituacao.SelectedItem?.ToString() ?? "A"),
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false;
            _codigoEditando = 0;
            Carregar();
            // Sync to Supabase (always, regardless of CEP)
            int codSalvo = obj.Codigo > 0 ? obj.Codigo : BuscarCodigoRecem(obj.baiCidade, obj.baiNome);
            if (codSalvo > 0)
                System.Threading.Tasks.Task.Run(async () =>
                {
                    var erroSync = await DB.SupabaseService.SincronizarBairroAsync(codSalvo);
                    if (!string.IsNullOrWhiteSpace(erroSync))
                        Logger.Log("frmCadastroBairro", "Salvar", $"Erro sync bairro {codSalvo}: {erroSync}");
                });
        }

        private int BuscarCodigoRecem(string cidade, string nome)
        {
            try
            {
                var bairro = _bll.Listar();
                foreach (System.Data.DataRow r in bairro.Rows)
                    if (r["Cidade"]?.ToString() == cidade && r["Bairro"]?.ToString() == nome)
                        return Convert.ToInt32(r["Codigo"]);
            }
            catch { }
            return 0;
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            _codigoEditando = 0;
        }

        private async void BtnSincronizar_Click(object sender, EventArgs e)
        {
            btnSincronizar.Enabled = false;
            btnSincronizar.Text    = "Sincronizando...";
            try
            {
                await System.Threading.Tasks.Task.Run(async () =>
                    await DB.SupabaseService.SincronizarTodosBairrosAsync());
                MessageBox.Show("Sincronização concluída!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao sincronizar: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSincronizar.Enabled = true;
                btnSincronizar.Text    = "\u2601 Sincronizar Site";
            }
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Desativar este bairro?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            obj.Situacao = "I";
            _bll.Salvar(obj);
            pnlForm.Visible = false;
            Carregar();
        }

    }
}
