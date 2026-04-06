using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmEstoque : Form
    {
        private readonly EstoqueBLL _bll = new EstoqueBLL();
        private int     _codigoEditando  = 0;
        private bool    _modoAjuste      = false; // true = adjusting qty, false = editing/adding

        public frmEstoque()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            Load += (_, __) => CarregarGrid();
        }

        // ── Grid ─────────────────────────────────────────────────────────────

        private void CarregarGrid()
        {
            try
            {
                grid.DataSource = _bll.Listar(txtFiltro.Text.Trim());
                var hide = new[] { "EhProduto", "Situacao" };
                foreach (DataGridViewColumn c in grid.Columns)
                    c.Visible = !Array.Exists(hide, h => h == c.Name);
                if (grid.Columns["Codigo"]    != null) { grid.Columns["Codigo"].HeaderText    = "Cód.";         grid.Columns["Codigo"].FillWeight    = 6; }
                if (grid.Columns["Nome"]      != null) { grid.Columns["Nome"].HeaderText      = "Nome / Item";  grid.Columns["Nome"].FillWeight      = 40; }
                if (grid.Columns["Unidade"]   != null) { grid.Columns["Unidade"].HeaderText   = "Unid.";        grid.Columns["Unidade"].FillWeight   = 8; }
                if (grid.Columns["Qtde"]      != null) { grid.Columns["Qtde"].HeaderText      = "Qtde";         grid.Columns["Qtde"].FillWeight      = 10; }
                if (grid.Columns["Custo"]     != null) { grid.Columns["Custo"].HeaderText     = "Custo R$";     grid.Columns["Custo"].FillWeight     = 12; }
                if (grid.Columns["EstMinimo"] != null) { grid.Columns["EstMinimo"].HeaderText = "Est. Mínimo";  grid.Columns["EstMinimo"].FillWeight = 12; }
                ModoNeutro();
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNeutro()
        {
            pnlForm.Visible = false;
            _codigoEditando = 0;
            _modoAjuste     = false;
        }

        // ── Toolbar events ────────────────────────────────────────────────────

        private void TxtFiltro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) CarregarGrid();
        }

        private void BtnPesq_Click(object sender, EventArgs e) => CarregarGrid();

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            _codigoEditando = 0;
            _modoAjuste     = false;
            LimparForm();
            pnlAjuste.Visible  = false;
            pnlCadastro.Visible = true;
            lblFormTitulo.Text = "Novo Item de Estoque";
            pnlForm.Visible    = true;
            txtNome.Focus();
        }

        // ── Grid double-click → edit ──────────────────────────────────────────

        private void Grid_DoubleClick(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando = cod;
            _modoAjuste     = false;

            txtNome.Text        = obj.estoNome;
            txtUnidade.Text     = obj.estoUnidade;
            numQtde.Value       = obj.estoQtde_Atual  > numQtde.Maximum  ? numQtde.Maximum  : obj.estoQtde_Atual;
            numCusto.Value      = obj.estoPreco_Custo > numCusto.Maximum ? numCusto.Maximum : obj.estoPreco_Custo;
            numEstMin.Value     = obj.estoEstoque_Min > numEstMin.Maximum ? numEstMin.Maximum: obj.estoEstoque_Min;
            chkEhProduto.Checked = obj.estoEh_Produto;
            lblFormTitulo.Text  = "Editar Item";
            pnlAjuste.Visible   = false;
            pnlCadastro.Visible = true;
            pnlForm.Visible     = true;
            txtNome.Focus();
        }

        // ── Ajuste de quantidade ──────────────────────────────────────────────

        private void BtnAjuste_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
            { MessageBox.Show("Selecione um item na lista.", "Ajuste de Estoque"); return; }

            var cod  = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var nome = grid.SelectedRows[0].Cells["Nome"].Value?.ToString() ?? "";
            var qtd  = grid.SelectedRows[0].Cells["Qtde"].Value;
            _codigoEditando       = cod;
            _modoAjuste           = true;
            lblAjusteNome.Text    = nome;
            lblAjusteAtual.Text   = "Qtde atual: " + (qtd == DBNull.Value ? "0" : Convert.ToDecimal(qtd).ToString("N2"));
            numAjusteQtde.Value   = 0;
            cmbAjusteTipo.SelectedIndex = 0;
            pnlCadastro.Visible   = false;
            pnlAjuste.Visible     = true;
            lblFormTitulo.Text    = "Ajuste de Quantidade";
            pnlForm.Visible       = true;
            numAjusteQtde.Focus();
        }

        // ── Save / Cancel ─────────────────────────────────────────────────────

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (_modoAjuste)
            {
                if (numAjusteQtde.Value == 0) { MessageBox.Show("Informe a quantidade."); return; }
                decimal delta = cmbAjusteTipo.SelectedIndex == 0 ? numAjusteQtde.Value : -numAjusteQtde.Value;
                var err = _bll.AjustarQuantidade(_codigoEditando, delta);
                if (!string.IsNullOrEmpty(err)) { MessageBox.Show("Erro: " + err); return; }
            }
            else
            {
                var obj = new EstoqueItem
                {
                    Codigo          = _codigoEditando,
                    estoNome        = txtNome.Text.Trim(),
                    estoUnidade     = string.IsNullOrWhiteSpace(txtUnidade.Text) ? "un" : txtUnidade.Text.Trim(),
                    estoQtde_Atual  = numQtde.Value,
                    estoPreco_Custo = numCusto.Value,
                    estoEstoque_Min = numEstMin.Value,
                    estoEh_Produto  = chkEhProduto.Checked,
                };

                // Preserve existing Codigo_Mercadoria if editing
                if (_codigoEditando > 0)
                {
                    var existing = _bll.PesquisaCodigo(_codigoEditando);
                    obj.Codigo_Mercadoria = existing?.Codigo_Mercadoria;
                }

                var err = _bll.Salvar(obj);
                if (!string.IsNullOrEmpty(err)) { MessageBox.Show("Erro: " + err); return; }

                if (chkEhProduto.Checked)
                    MessageBox.Show("Item salvo e sincronizado como produto no catálogo.",
                        "Estoque", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            CarregarGrid();
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (_codigoEditando == 0) return;
            if (MessageBox.Show("Desativar este item?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            _bll.Desativar(_codigoEditando);
            CarregarGrid();
        }

        private void BtnCancelar_Click(object sender, EventArgs e) => ModoNeutro();

        private void LimparForm()
        {
            txtNome.Clear(); txtUnidade.Text = "un";
            numQtde.Value = 0; numCusto.Value = 0; numEstMin.Value = 0;
            chkEhProduto.Checked = false;
        }
    }
}

