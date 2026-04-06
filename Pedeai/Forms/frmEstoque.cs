using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;

namespace Pedeai.Forms
{
    public partial class frmEstoque : Form
    {
        private readonly EstoqueBLL _bll = new EstoqueBLL();
        private int _codigoSelecionado = 0;

        public frmEstoque()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            Load += (_, __) => CarregarGrid();
        }

        private void CarregarGrid()
        {
            try
            {
                var filtro = txtFiltro.Text.Trim();
                grid.DataSource = _bll.Listar(filtro);
                var show = new[] { "Produto", "Categoria", "Estoque" };
                foreach (DataGridViewColumn c in grid.Columns)
                    c.Visible = Array.Exists(show, s => s == c.Name);
                if (grid.Columns["Produto"]  != null) { grid.Columns["Produto"].HeaderText  = "Produto";    grid.Columns["Produto"].FillWeight  = 40; }
                if (grid.Columns["Categoria"]!= null) { grid.Columns["Categoria"].HeaderText= "Categoria";  grid.Columns["Categoria"].FillWeight= 30; }
                if (grid.Columns["Estoque"]  != null) { grid.Columns["Estoque"].HeaderText  = "Estoque";    grid.Columns["Estoque"].FillWeight  = 15; }
                pnlForm.Visible = false;
                _codigoSelecionado = 0;
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void TxtFiltro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) CarregarGrid();
        }

        private void BtnPesq_Click(object sender, EventArgs e) => CarregarGrid();

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { _codigoSelecionado = 0; return; }
            var row = grid.SelectedRows[0];
            _codigoSelecionado = Convert.ToInt32(row.Cells["Codigo"].Value);
            lblProdutoSel.Text = row.Cells["Produto"].Value?.ToString() ?? "";
            var est = row.Cells["Estoque"].Value;
            lblEstoqueAtual.Text = "Estoque atual: " + (est == DBNull.Value ? "0" : Convert.ToDecimal(est).ToString("N2"));
            numQtde.Value = 1;
            cmbTipo.SelectedIndex = 0;
            txtObs.Clear();
            pnlForm.Visible = true;
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (_codigoSelecionado == 0) { MessageBox.Show("Selecione um produto."); return; }
            if (numQtde.Value == 0)      { MessageBox.Show("Informe a quantidade."); return; }

            decimal delta = cmbTipo.SelectedIndex == 0 ? numQtde.Value : -numQtde.Value;
            string tipo   = cmbTipo.SelectedIndex == 0 ? "Entrada" : "Sa\u00edda";

            var erro = _bll.Ajustar(_codigoSelecionado, delta, tipo, txtObs.Text.Trim());
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }

            CarregarGrid();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            _codigoSelecionado = 0;
        }
    }
}
