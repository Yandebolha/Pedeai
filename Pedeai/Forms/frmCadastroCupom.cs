using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroCupom : Form
    {
        private CupomBLL _bll;
        private int _codigoEditando = 0;

        public frmCadastroCupom()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new CupomBLL();
            Load += (_, __) => CarregarGrid();
        }


        private void CarregarGrid()
        {
            try { grid.DataSource = _bll.Listar(); }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0;
            txtCodigo.Clear(); txtDescricao.Clear();
            cmbTipo.SelectedIndex = 0; cmbSituacao.SelectedIndex = 0;
            numValor.Value = 0; numMinimo.Value = 0; numLimite.Value = 0;
            dtpValido.Value = DateTime.Today.AddMonths(1);
            pnlForm.Visible = true; txtCodigo.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando = cod;
            txtCodigo.Text = obj.cupomCodigo ?? "";
            txtDescricao.Text = obj.cupomDescricao ?? "";
            cmbTipo.SelectedItem = obj.cupomTipo ?? "PERCENTUAL";
            numValor.Value = obj.cupomValor;
            numMinimo.Value = obj.cupomPedido_Minimo;
            numLimite.Value = obj.cupomLimite_Usos;
            dtpValido.Value = obj.cupomValido_Ate > DateTime.MinValue ? obj.cupomValido_Ate : DateTime.Today.AddMonths(1);
            cmbSituacao.SelectedItem = (obj.Situacao == "I") ? "Inativo" : "Ativo";
            pnlForm.Visible = true; txtCodigo.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text)) { MessageBox.Show("Informe o código do cupom."); return; }
            var obj = new Cupom
            {
                Codigo             = _codigoEditando,
                cupomCodigo        = txtCodigo.Text.Trim().ToUpper(),
                cupomDescricao     = txtDescricao.Text,
                cupomTipo          = cmbTipo.SelectedItem?.ToString() ?? "PERCENTUAL",
                cupomValor         = numValor.Value,
                cupomPedido_Minimo = numMinimo.Value,
                cupomLimite_Usos   = (int)numLimite.Value,
                cupomValido_Ate    = dtpValido.Value,
                Situacao           = cmbSituacao.SelectedItem?.ToString() == "Inativo" ? "I" : "A",
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            if (_codigoEditando == 0) { MessageBox.Show("Abra o cupom para edição primeiro."); return; }
            if (MessageBox.Show("Desativar cupom?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var obj = _bll.PesquisaCodigo(_codigoEditando);
            if (obj != null) { obj.Situacao = "I"; _bll.Salvar(obj); }
            pnlForm.Visible = false; _codigoEditando = 0; CarregarGrid();
        }
    }
}
