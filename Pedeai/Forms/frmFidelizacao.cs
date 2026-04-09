using System;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmFidelizacao : Form
    {
        private readonly FidelizacaoBLL _bll;
        private int _codigoConfigEditando = 0;
        private bool _carregandoConfigs = false;

        public frmFidelizacao()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new FidelizacaoBLL();
            Load += Form_Load;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            CarregarListaConfigs();
            CarregarHistorico();
        }

        // ── Configuração ──────────────────────────────────────────────────────

        private void CarregarListaConfigs()
        {
            _carregandoConfigs = true;
            try
            {
                gridConfigs.DataSource = _bll.Listar();
                if (gridConfigs.Columns.Count > 0)
                {
                    if (gridConfigs.Columns["Codigo"] != null) gridConfigs.Columns["Codigo"].Visible = false;
                    if (gridConfigs.Columns["Nome"]   != null) { gridConfigs.Columns["Nome"].HeaderText = "Regra"; gridConfigs.Columns["Nome"].FillWeight = 45; }
                    if (gridConfigs.Columns["Ativo"]  != null) { gridConfigs.Columns["Ativo"].HeaderText = "Ativa"; gridConfigs.Columns["Ativo"].FillWeight = 10; }
                    if (gridConfigs.Columns["Meta"]   != null) { gridConfigs.Columns["Meta"].HeaderText = "Meta R$"; gridConfigs.Columns["Meta"].FillWeight = 18;
                                                                  gridConfigs.Columns["Meta"].DefaultCellStyle.Format = "N2"; }
                    if (gridConfigs.Columns["Tipo"]   != null) { gridConfigs.Columns["Tipo"].HeaderText = "Prêmio"; gridConfigs.Columns["Tipo"].FillWeight = 18; }
                }
                if (gridConfigs.Rows.Count > 0)
                    gridConfigs.Rows[0].Selected = true;
            }
            finally { _carregandoConfigs = false; }
        }

        private void GridConfigs_SelectionChanged(object sender, EventArgs e)
        {
            if (_carregandoConfigs) return;
            if (gridConfigs.SelectedRows.Count == 0) return;
            var row = gridConfigs.SelectedRows[0];
            if (row.Cells["Codigo"]?.Value == null || row.Cells["Codigo"].Value == DBNull.Value) return;
            int cod = Convert.ToInt32(row.Cells["Codigo"].Value);
            CarregarConfigDetalhe(cod);
        }

        private void CarregarConfigDetalhe(int codigo)
        {
            var cfg = _bll.Carregar(codigo);
            _codigoConfigEditando = cfg.Codigo;
            txtNomeRegra.Text       = cfg.fidNome;
            chkAtivo.Checked        = cfg.fidAtivo;
            numMeta.Value           = cfg.fidMeta_Gasto > 0 ? cfg.fidMeta_Gasto : 500m;
            rdCupom.Checked         = cfg.fidPremio_Tipo != "PRODUTO";
            rdProduto.Checked       = cfg.fidPremio_Tipo == "PRODUTO";
            cmbCupomTipo.SelectedIndex = cfg.fidCupom_Tipo == "FIXO" ? 1 : 0;
            numCupomValor.Value     = cfg.fidCupom_Valor > 0 ? Math.Min(cfg.fidCupom_Valor, numCupomValor.Maximum) : 10m;
            numCupomMin.Value       = cfg.fidCupom_Minimo >= 0 ? cfg.fidCupom_Minimo : 0m;
            numCupomValidade.Value  = cfg.fidCupom_Validade > 0 ? Math.Min(cfg.fidCupom_Validade, 365) : 30;
            txtProdNome.Text        = cfg.fidProduto_Nome ?? "";
            txtMsg.Text             = string.IsNullOrWhiteSpace(cfg.fidMensagem)
                                        ? "Parabéns {Nome}! Você atingiu R$ {Meta} em compras e ganhou um cupom {CupomCodigo} válido até {Validade}."
                                        : cfg.fidMensagem;
            AtualizarPainelPremio();
            btnSalvar.Text = "\u270E Salvar Edição";
        }

        private void BtnNovaRegra_Click(object sender, EventArgs e)
        {
            _codigoConfigEditando = 0;
            txtNomeRegra.Text       = "Nova Regra";
            chkAtivo.Checked        = true;
            numMeta.Value           = 500m;
            rdCupom.Checked         = true;
            rdProduto.Checked       = false;
            cmbCupomTipo.SelectedIndex = 0;
            numCupomValor.Value     = 10m;
            numCupomMin.Value       = 0m;
            numCupomValidade.Value  = 30;
            txtProdNome.Text        = "";
            txtMsg.Text             = "Parabéns {Nome}! Você atingiu R$ {Meta} em compras e ganhou um cupom {CupomCodigo} válido até {Validade}.";
            gridConfigs.ClearSelection();
            AtualizarPainelPremio();
            btnSalvar.Text = "Salvar Nova Regra";
            txtNomeRegra.Focus();
            txtNomeRegra.SelectAll();
        }

        private void BtnExcluirRegra_Click(object sender, EventArgs e)
        {
            if (gridConfigs.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma regra para excluir.", "Excluir Regra", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            int cod = Convert.ToInt32(gridConfigs.SelectedRows[0].Cells["Codigo"].Value);
            string nome = gridConfigs.SelectedRows[0].Cells["Nome"]?.Value?.ToString() ?? "";
            if (MessageBox.Show($"Excluir a regra \"{nome}\"?", "Confirmar Exclusão",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var erro = _bll.Excluir(cod);
            if (!string.IsNullOrEmpty(erro))
                MessageBox.Show("Erro ao excluir: " + erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                CarregarListaConfigs();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigFidelizacao
            {
                Codigo            = _codigoConfigEditando,
                fidNome           = txtNomeRegra.Text.Trim(),
                fidAtivo          = chkAtivo.Checked,
                fidMeta_Gasto     = numMeta.Value,
                fidPremio_Tipo    = rdProduto.Checked ? "PRODUTO" : "CUPOM",
                fidCupom_Tipo     = cmbCupomTipo.SelectedIndex == 1 ? "FIXO" : "PERCENTUAL",
                fidCupom_Valor    = numCupomValor.Value,
                fidCupom_Minimo   = numCupomMin.Value,
                fidCupom_Validade = (int)numCupomValidade.Value,
                fidProduto_Nome   = txtProdNome.Text.Trim(),
                fidMensagem       = txtMsg.Text.Trim(),
            };

            string erro = _codigoConfigEditando == 0 ? _bll.Incluir(cfg) : _bll.Alterar(cfg);
            if (!string.IsNullOrEmpty(erro))
            {
                MessageBox.Show("Erro ao salvar: " + erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("Configuração salva com sucesso!", "Fidelização", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _codigoConfigEditando = cfg.Codigo;
            CarregarListaConfigs();
            // Re-select saved row
            foreach (DataGridViewRow r in gridConfigs.Rows)
            {
                if (r.Cells["Codigo"]?.Value != null && Convert.ToInt32(r.Cells["Codigo"].Value) == cfg.Codigo)
                { gridConfigs.ClearSelection(); r.Selected = true; break; }
            }
        }

        private void ChkAtivo_CheckedChanged(object sender, EventArgs e)
        {
            // Visual feedback — no additional logic needed
        }

        private void RdPremio_CheckedChanged(object sender, EventArgs e)
        {
            AtualizarPainelPremio();
        }

        private void AtualizarPainelPremio()
        {
            pnlCupom.Visible   = rdCupom.Checked;
            pnlProduto.Visible = rdProduto.Checked;
        }

        // ── Histórico ──────────────────────────────────────────────────────────

        private void CarregarHistorico()
        {
            try
            {
                gridHistorico.DataSource = _bll.ListarHistorico(dtpDe.Value, dtpAte.Value);
                AjustarColunas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar histórico: " + ex.Message);
            }
        }

        private void BtnFiltrar_Click(object sender, EventArgs e)
        {
            CarregarHistorico();
        }

        private void AjustarColunas()
        {
            if (gridHistorico.Columns.Count == 0) return;
            foreach (DataGridViewColumn c in gridHistorico.Columns)
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            if (gridHistorico.Columns.Contains("Codigo"))
                gridHistorico.Columns["Codigo"].Visible = false;
            if (gridHistorico.Columns.Contains("Data"))
            {
                gridHistorico.Columns["Data"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                gridHistorico.Columns["Data"].FillWeight = 60;
            }
            if (gridHistorico.Columns.Contains("Cliente"))
                gridHistorico.Columns["Cliente"].FillWeight = 120;
            if (gridHistorico.Columns.Contains("Telefone"))
                gridHistorico.Columns["Telefone"].FillWeight = 70;
            if (gridHistorico.Columns.Contains("Cupom"))
                gridHistorico.Columns["Cupom"].FillWeight = 80;
            if (gridHistorico.Columns.Contains("Descricao"))
                gridHistorico.Columns["Descricao"].FillWeight = 150;
        }

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
    }
}
