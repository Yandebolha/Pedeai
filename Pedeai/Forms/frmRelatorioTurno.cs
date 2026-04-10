using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmRelatorioTurno : Form
    {
        private readonly TurnoBLL _bll = new TurnoBLL();
        private readonly Turno    _turno;

        public frmRelatorioTurno(Turno turno)
        {
            _turno = turno;
            InitializeComponent();
        }

        private void frmRelatorioTurno_Load(object sender, EventArgs e)
        {
            MontarCabecalho();
            CarregarPedidos();
        }

        private void MontarCabecalho()
        {
            string sit = _turno.turSituacao == 'A' ? "ABERTO" : "FECHADO";
            lblCabecalho.Text =
                $"Turno #{_turno.Codigo}   |   {sit}   |   " +
                $"Abertura: {_turno.turAbertura:dd/MM/yyyy HH:mm}   |   " +
                $"Usu\u00e1rio: {_turno.turUsuario}";

            lblCaixaInicial.Text  = $"Caixa Inicial: R$ {_turno.turCaixa_Inicial:N2}";
            lblFechamento.Text    = _turno.turFechamento.HasValue
                ? $"Fechamento: {_turno.turFechamento.Value:dd/MM/yyyy HH:mm}"
                : "Fechamento: (em aberto)";
            lblCaixaFinal.Text    = _turno.turCaixa_Final.HasValue
                ? $"Caixa Final: R$ {_turno.turCaixa_Final.Value:N2}"
                : "Caixa Final: —";
        }

        private void CarregarPedidos()
        {
            try
            {
                var ate = _turno.turFechamento ?? DateTime.Now;
                var dt  = _bll.GetPedidosTurno(_turno.turAbertura, ate);
                gridPedidos.DataSource = dt;
                ConfigurarColunas(dt);
                MontarResumo(dt);
            }
            catch (Exception ex)
            {
                lblResumo.Text = "Erro: " + ex.Message;
            }
        }

        private void ConfigurarColunas(DataTable dt)
        {
            if (gridPedidos.Columns.Count == 0) return;

            var show = new System.Collections.Generic.Dictionary<string, string>
            {
                ["Codigo"]              = "N\u00ba",
                ["pediNome_Cliente"]    = "Cliente",
                ["pediSituacao"]        = "Sit.",
                ["pediValor_Total"]     = "Total R$",
                ["pediPago_Dinheiro"]   = "Dinheiro",
                ["pediPago_Cartao"]     = "Cart\u00e3o",
                ["pediPago_Pix"]        = "Pix",
                ["pediData_Lancamento"] = "Lan\u00e7amento",
            };

            foreach (DataGridViewColumn col in gridPedidos.Columns)
                col.Visible = false;

            foreach (var kv in show)
                if (gridPedidos.Columns.Contains(kv.Key))
                {
                    gridPedidos.Columns[kv.Key].Visible    = true;
                    gridPedidos.Columns[kv.Key].HeaderText = kv.Value;
                }
        }

        private void MontarResumo(DataTable dt)
        {
            decimal totalVendas = 0, totalDin = 0, totalCar = 0, totalPix = 0;
            int qtdPedidos = dt.Rows.Count;

            decimal C(DataRow r, string c) => r[c] == DBNull.Value ? 0m : Convert.ToDecimal(r[c]);

            foreach (DataRow r in dt.Rows)
            {
                totalVendas += C(r, "pediValor_Total");
                totalDin    += C(r, "pediPago_Dinheiro");
                totalCar    += C(r, "pediPago_Cartao");
                totalPix    += C(r, "pediPago_Pix");
            }

            lblResumo.Text =
                $"Pedidos: {qtdPedidos}   |   " +
                $"Total Vendas: R$ {totalVendas:N2}   |   " +
                $"Dinheiro: R$ {totalDin:N2}   |   " +
                $"Cart\u00e3o: R$ {totalCar:N2}   |   " +
                $"Pix: R$ {totalPix:N2}";

            decimal diferenca = totalVendas - (totalDin + totalCar + totalPix);
            if (Math.Abs(diferenca) > 0.01m)
                lblResumo.Text += $"   |   Dif. pagamento: R$ {diferenca:N2}";
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            // Placeholder p/ impressão futura
            MessageBox.Show("Funcionalidade de impress\u00e3o do relat\u00f3rio em desenvolvimento.",
                            "Imprimir", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }    }
}
