using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmTurno : Form
    {
        private readonly TurnoBLL _bll = new TurnoBLL();
        private Turno _turnoAtivo;

        public frmTurno()
        {
            InitializeComponent();
        }

        private void frmTurno_Load(object sender, EventArgs e)
        {
            AtualizarEstado();
        }

        private void AtualizarEstado()
        {
            _turnoAtivo = _bll.GetAtivo();

            if (_turnoAtivo != null)
            {
                lblStatus.Text      = $"TURNO ABERTO  #{_turnoAtivo.Codigo}  |  Abertura: {_turnoAtivo.turAbertura:dd/MM/yyyy HH:mm}  |  Usu\u00e1rio: {_turnoAtivo.turUsuario}";
                lblStatus.BackColor = Color.FromArgb(39, 174, 96);
                lblCaixaIni.Text    = $"Caixa inicial: R$ {_turnoAtivo.turCaixa_Inicial:N2}";

                pnlAbrir.Visible   = false;
                pnlFechar.Visible  = true;
                numCaixaFinal.Value = _turnoAtivo.turCaixa_Inicial;
            }
            else
            {
                lblStatus.Text      = "NENHUM TURNO ABERTO";
                lblStatus.BackColor = Color.FromArgb(192, 57, 43);
                lblCaixaIni.Text    = "";

                pnlAbrir.Visible   = true;
                pnlFechar.Visible  = false;
                numCaixaInicial.Value = 0;
                txtObsAbrir.Text      = "";
            }

            CarregarHistorico();
        }

        private void BtnAbrir_Click(object sender, EventArgs e)
        {
            decimal caixaIni = numCaixaInicial.Value;
            string obs       = txtObsAbrir.Text.Trim();

            var erro = _bll.Abrir(caixaIni, obs);
            if (!string.IsNullOrEmpty(erro))
            {
                MessageBox.Show(erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Log("Turno aberto", $"Caixa inicial: R$ {caixaIni:N2}");
            MessageBox.Show("Turno aberto com sucesso!", "Turno", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AtualizarEstado();
        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            decimal caixaFin = numCaixaFinal.Value;
            string obs       = txtObsFechar.Text.Trim();

            if (MessageBox.Show(
                    $"Confirma o fechamento do turno #{_turnoAtivo?.Codigo}?\n\nCaixa final: R$ {caixaFin:N2}",
                    "Fechar Turno", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var erro = _bll.Fechar(caixaFin, obs);
            if (!string.IsNullOrEmpty(erro))
            {
                MessageBox.Show(erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Log("Turno fechado", $"Caixa final: R$ {caixaFin:N2}");
            MessageBox.Show("Turno fechado com sucesso!", "Turno", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AtualizarEstado();
        }

        private void BtnRelatorio_Click(object sender, EventArgs e)
        {
            if (gridHistorico.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um turno na lista para ver as movimenta\u00e7\u00f5es.",
                                "Turno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var row = gridHistorico.SelectedRows[0].DataBoundItem as System.Data.DataRowView;
            if (row == null) return;
            int cod = Convert.ToInt32(row.Row["Codigo"]);

            // Recarrega o modelo completo para o relatório
            var todos = _bll.Listar(new DateTime(2000, 1, 1), DateTime.Today.AddDays(1));
            Modelo.Turno turno = null;
            foreach (System.Data.DataRow r in todos.Rows)
            {
                if (Convert.ToInt32(r["Codigo"]) == cod)
                {
                    turno = new Modelo.Turno
                    {
                        Codigo          = cod,
                        turAbertura     = Convert.ToDateTime(r["turAbertura"]),
                        turFechamento   = r["turFechamento"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["turFechamento"]),
                        turUsuario      = r["turUsuario"]?.ToString() ?? "",
                        turCaixa_Inicial = Convert.ToDecimal(r["turCaixa_Inicial"]),
                        turCaixa_Final  = r["turCaixa_Final"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["turCaixa_Final"]),
                        turObservacao   = r["turObservacao"]?.ToString() ?? "",
                        turSituacao     = r["turSituacao"]?.ToString()?[0] ?? 'F'
                    };
                    break;
                }
            }
            if (turno == null) return;
            using var frm = new frmRelatorioTurno(turno);
            frm.ShowDialog(this);
        }

        private void BtnFiltrar_Click(object sender, EventArgs e)
        {
            CarregarHistorico();
        }

        private void CarregarHistorico()
        {
            try
            {
                var de  = dtpDe.Value.Date;
                var ate = dtpAte.Value.Date;
                gridHistorico.DataSource = _bll.Listar(de, ate);
                ConfigurarColunas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar hist\u00f3rico: " + ex.Message);
            }
        }

        private void ConfigurarColunas()
        {
            if (gridHistorico.Columns.Count == 0) return;

            var caps = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Codigo"]           = "N\u00ba",
                ["turAbertura"]      = "Abertura",
                ["turFechamento"]    = "Fechamento",
                ["turUsuario"]       = "Usu\u00e1rio",
                ["turCaixa_Inicial"] = "Caixa Inicial",
                ["turCaixa_Final"]   = "Caixa Final",
                ["turObservacao"]    = "Observa\u00e7\u00e3o",
                ["turSituacao"]      = "Sit.",
            };
            foreach (var kv in caps)
                if (gridHistorico.Columns.Contains(kv.Key))
                    gridHistorico.Columns[kv.Key].HeaderText = kv.Value;
        }
    }
}
