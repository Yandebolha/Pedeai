using System;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmFidelizacao : Form
    {
        private readonly FidelizacaoBLL _bll;

        public frmFidelizacao()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new FidelizacaoBLL();
            Load += Form_Load;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            CarregarConfig();
            CarregarHistorico();
        }

        // ── Configuração ──────────────────────────────────────────────────────

        private void CarregarConfig()
        {
            var cfg = _bll.Carregar();
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
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            var cfg = new ConfigFidelizacao
            {
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

            var erro = _bll.Salvar(cfg);
            if (!string.IsNullOrEmpty(erro))
                MessageBox.Show("Erro ao salvar: " + erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Configuração salva com sucesso!", "Fidelização", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
