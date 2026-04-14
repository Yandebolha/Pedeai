using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmEstoque : Form
    {
        private readonly EstoqueBLL          _bll      = new EstoqueBLL();
        private readonly GrupoMercadoriaBLL  _grupoBll = new GrupoMercadoriaBLL();
        private int     _codigoEditando  = 0;
        private bool    _modoAjuste      = false; // true = adjusting qty, false = editing/adding

        public frmEstoque()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            Load += (_, __) => { CarregarGrid(); CarregarCategorias(); };
        }

        // ── Grid ─────────────────────────────────────────────────────────────

        private void CarregarGrid()
        {
            try
            {
                grid.DataSource = _bll.Listar(txtFiltro.Text.Trim());
                var hide = new[] { "EhProduto", "Situacao", "Codigo" };
                foreach (DataGridViewColumn c in grid.Columns)
                    c.Visible = !Array.Exists(hide, h => h == c.Name);
                if (grid.Columns["Nome"]    != null) { grid.Columns["Nome"].HeaderText    = "Nome / Item"; }
                if (grid.Columns["Unidade"] != null) { grid.Columns["Unidade"].HeaderText = "Unid."; }
                if (grid.Columns["Qtde"]    != null)
                {
                    grid.Columns["Qtde"].HeaderText = "Qtde";
                    grid.Columns["Qtde"].DefaultCellStyle.Format = "N2";
                }
                if (grid.Columns["Custo"]   != null)
                {
                    grid.Columns["Custo"].HeaderText = "Custo R$";
                    grid.Columns["Custo"].DefaultCellStyle.Format = "N2";
                }
                // All content columns size to content; Nome column fills remaining space
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                if (grid.Columns["Nome"] != null)
                    grid.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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

        private void CarregarCategorias()
        {
            try
            {
                var dt = _grupoBll.Listar(apenasAtivas: true);
                cmbCategoria.Items.Clear();
                cmbCategoria.Items.Add(new CategoriaItem(null, "-- nenhuma --"));
                foreach (System.Data.DataRow row in dt.Rows)
                    cmbCategoria.Items.Add(new CategoriaItem(
                        Convert.ToInt32(row["Codigo"]),
                        row["Nome"]?.ToString() ?? ""));
                if (cmbCategoria.Items.Count > 0) cmbCategoria.SelectedIndex = 0;
            }
            catch { /* silencioso */ }
        }

        // ── Toolbar events ────────────────────────────────────────────────────

        private void TxtFiltro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) CarregarGrid();
        }

        private void BtnPesq_Click(object sender, EventArgs e) => CarregarGrid();

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }

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

        private void ChkEhProduto_CheckedChanged(object sender, EventArgs e)
        {
            bool vis = chkEhProduto.Checked;
            lblFracEntrada.Visible  = vis;
            numFracEntrada.Visible  = vis;
            txtFracEntradaUn.Visible = vis;
            lblFracSaida.Visible    = vis;
            numFracSaida.Visible    = vis;
            txtFracSaidaUn.Visible  = vis;
            lblCategoria.Visible    = vis;
            cmbCategoria.Visible    = vis;
            AtualizarCustoUnit();
        }

        private void NumCustoQtde_ValueChanged(object sender, EventArgs e) => AtualizarCustoUnit();

        private void AtualizarCustoUnit()
        {
            if (!chkEhProduto.Checked || numFracSaida.Value == 0 || numQtde.Value == 0)
            {
                lblCustoUnit.Text = "";
                return;
            }
            decimal totalUnidades = numQtde.Value * numFracSaida.Value;
            decimal custoPorUn   = numCusto.Value / totalUnidades;
            string  unSaida      = string.IsNullOrWhiteSpace(txtFracSaidaUn.Text) ? "un" : txtFracSaidaUn.Text.Trim();
            lblCustoUnit.Text = $"= R$ {custoPorUn:F2} / {unSaida}  ({totalUnidades:N2} {unSaida})";
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
            chkEhProduto.Checked  = obj.estoEh_Produto;
            numFracEntrada.Value   = obj.estoFracao_Entrada > numFracEntrada.Maximum ? numFracEntrada.Maximum : obj.estoFracao_Entrada;
            txtFracEntradaUn.Text  = obj.estoFracao_Entrada_Unidade;
            numFracSaida.Value     = obj.estoFracao_Saida   > numFracSaida.Maximum   ? numFracSaida.Maximum   : obj.estoFracao_Saida;
            txtFracSaidaUn.Text    = obj.estoFracao_Saida_Unidade;
            SelecionarCategoria(obj.Codigo_Grupo);
            // Reverter qtde e custo para os valores de entrada originais
            if (obj.estoEh_Produto && obj.estoFracao_Saida > 0)
            {
                decimal qtdeEntrada  = obj.estoQtde_Atual / obj.estoFracao_Saida;
                decimal custoTotal   = obj.estoPreco_Custo * obj.estoQtde_Atual;
                numQtde.Value  = qtdeEntrada  > numQtde.Maximum  ? numQtde.Maximum  : qtdeEntrada;
                numCusto.Value = custoTotal   > numCusto.Maximum ? numCusto.Maximum : custoTotal;
            }
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
                    estoUnidade     = string.IsNullOrWhiteSpace(txtUnidade.Text) ? "UN" : txtUnidade.Text.Trim(),
                    estoPreco_Custo = numCusto.Value,
                    estoQtde_Atual  = numQtde.Value,
                    estoEh_Produto     = chkEhProduto.Checked,
                    estoFracao_Entrada          = chkEhProduto.Checked ? numFracEntrada.Value : 1,
                    estoFracao_Entrada_Unidade  = chkEhProduto.Checked ? txtFracEntradaUn.Text.Trim() : "",
                    estoFracao_Saida            = chkEhProduto.Checked ? numFracSaida.Value   : 1,
                    estoFracao_Saida_Unidade    = chkEhProduto.Checked ? txtFracSaidaUn.Text.Trim() : "",
                    Codigo_Grupo = chkEhProduto.Checked ? (cmbCategoria.SelectedItem as CategoriaItem)?.Codigo : null,
                };

                // Aplica frações: converte qtde/custo para unidades de saída
                if (chkEhProduto.Checked && obj.estoFracao_Saida > 0)
                {
                    decimal totalUnidades = obj.estoQtde_Atual * obj.estoFracao_Saida;
                    obj.estoQtde_Atual  = totalUnidades;
                    obj.estoPreco_Custo = totalUnidades > 0 ? numCusto.Value / totalUnidades : numCusto.Value;
                }

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
            numQtde.Value = 0; numCusto.Value = 0;
            chkEhProduto.Checked = false;
            numFracEntrada.Value = 1; txtFracEntradaUn.Clear();
            numFracSaida.Value = 1;   txtFracSaidaUn.Clear();
            if (cmbCategoria.Items.Count > 0) cmbCategoria.SelectedIndex = 0;
        }

        private void SelecionarCategoria(int? codigoGrupo)
        {
            if (codigoGrupo == null) { if (cmbCategoria.Items.Count > 0) cmbCategoria.SelectedIndex = 0; return; }
            foreach (CategoriaItem item in cmbCategoria.Items)
                if (item.Codigo == codigoGrupo) { cmbCategoria.SelectedItem = item; return; }
            if (cmbCategoria.Items.Count > 0) cmbCategoria.SelectedIndex = 0;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private sealed class CategoriaItem
        {
            public int?   Codigo { get; }
            private string Nome  { get; }
            public CategoriaItem(int? codigo, string nome) { Codigo = codigo; Nome = nome; }
            public override string ToString() => Nome;
        }
    }
}

