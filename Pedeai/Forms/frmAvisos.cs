using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;

namespace Pedeai.Forms
{
    public partial class frmAvisos : Form
    {
        private readonly EntradaMercadoriaBLL _bll = new EntradaMercadoriaBLL();

        public frmAvisos()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            Load += Form_Load;
        }

        private void Form_Load(object sender, EventArgs e) => CarregarGrid();

        private void PnlTop_SizeChanged(object sender, EventArgs e)
        {
            lblTit.Left = (pnlTop.Width - lblTit.Width) / 2;
        }

        private void BtnAtualizar_Click(object sender, EventArgs e) => CarregarGrid();

        private void CmbFiltro_SelectedIndexChanged(object sender, EventArgs e) => CarregarGrid();

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void CarregarGrid()
        {
            try
            {
                var filtro = cmbFiltro.SelectedItem?.ToString() ?? "";
                var dt = _bll.ListarAvisosFiltrados(filtro);
                grid.DataSource = dt;
                ConfigurarColunas();
                AtualizarResumo(dt);
                ColorirLinhas();
            }
            catch (Exception ex) { lblResumo.Text = "Erro: " + ex.Message; }
        }

        private void ConfigurarColunas()
        {
            if (grid.Columns.Count == 0) return;
            foreach (DataGridViewColumn col in grid.Columns)
                col.Visible = false;

            var show = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Codigo"]        = "C\u00f3d.",
                ["CodEntrada"]    = "Entrada #",
                ["Fornecedor"]    = "Fornecedor",
                ["Documento"]     = "N\u00ba Doc",
                ["DataEntrada"]   = "Data Nota",
                ["Parcela"]       = "Parcela",
                ["TotalParcelas"] = "Total",
                ["Vencimento"]    = "Vencimento",
                ["Valor"]         = "Valor R$",
                ["Observacao"]    = "Observa\u00e7\u00e3o",
                ["Situacao"]      = "Situa\u00e7\u00e3o",
                ["DataPagamento"] = "Dt. Pagamento",
            };
            foreach (var kv in show)
                if (grid.Columns.Contains(kv.Key))
                {
                    grid.Columns[kv.Key].Visible = true;
                    grid.Columns[kv.Key].HeaderText = kv.Value;
                }
        }

        private void AtualizarResumo(System.Data.DataTable dt)
        {
            if (dt == null) { lblResumo.Text = ""; return; }
            decimal totalAberto = 0;
            int vencidas = 0;
            foreach (System.Data.DataRow r in dt.Rows)
            {
                if (r["Situacao"]?.ToString() == "A")
                {
                    if (decimal.TryParse(r["Valor"]?.ToString(), out decimal v)) totalAberto += v;
                    if (r["Vencimento"] is DateTime vc && vc.Date < DateTime.Today) vencidas++;
                }
            }
            lblResumo.Text = $"  {dt.Rows.Count} parcelas  |  Total em aberto: R$ {totalAberto:N2}  |  Vencidas: {vencidas}";
        }

        private void ColorirLinhas()
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.DataBoundItem == null) continue;
                var dr = ((System.Data.DataRowView)row.DataBoundItem).Row;
                var sit = dr["Situacao"]?.ToString();
                if (sit == "P") { row.DefaultCellStyle.ForeColor = Color.FromArgb(100, 170, 100); continue; }
                if (!(dr["Vencimento"] is DateTime vcto)) continue;
                var dias = (vcto.Date - DateTime.Today).Days;
                if (dias < 0)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(80, 30, 30);
                else if (dias == 0)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(90, 70, 10);
                else if (dias <= 7)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(70, 55, 10);
            }
        }

        private void Grid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) { }

        private void BtnPago_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma parcela para marcar como paga."); return; }
            var row = grid.SelectedRows[0];
            if (row.DataBoundItem == null) return;
            var dr  = ((System.Data.DataRowView)row.DataBoundItem).Row;
            var cod = Convert.ToInt32(dr["Codigo"]);
            var sit = dr["Situacao"]?.ToString();
            if (sit == "P") { MessageBox.Show("Esta parcela jÃ¡ estÃ¡ paga."); return; }
            if (MessageBox.Show("Confirmar pagamento desta parcela?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var erro = _bll.MarcarParcelaPaga(cod);
            if (!string.IsNullOrEmpty(erro)) MessageBox.Show("Erro: " + erro);
            else CarregarGrid();
        }
    }
}
