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
            Load += (_, __) => Carregar();
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { e.Handled = true; Close(); return; }
            base.OnKeyDown(e);
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            txtCidade.Clear();
            txtBairro.Clear();
            numTaxa.Value = 0;
            cmbSituacao.SelectedItem = "A";
            pnlForm.Visible = true;
            txtCidade.Focus();
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
                Situacao        = _codigoEditando == 0 ? "A" : (cmbSituacao.SelectedItem?.ToString() ?? "A"),
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false;
            _codigoEditando = 0;
            Carregar();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            _codigoEditando = 0;
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { pnlForm.Visible = false; _codigoEditando = 0; return true; }
            if (keyData == Keys.F2)     { ModoNovo(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
