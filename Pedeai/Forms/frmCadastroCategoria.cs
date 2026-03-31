using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroCategoria : Form
    {
        private GrupoMercadoriaBLL _bll;
        private int _codigoEditando = 0;

        public frmCadastroCategoria()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new GrupoMercadoriaBLL();
            Load += (_, __) => Carregar();
        }

        private void Carregar()
        {
            try
            {
                grid.DataSource = _bll.Listar();
                // show only Codigo (hidden, for editing) and Nome
                foreach (DataGridViewColumn col in grid.Columns)
                    col.Visible = col.Name == "Codigo" || col.Name == "Nome";
                if (grid.Columns.Contains("Codigo")) grid.Columns["Codigo"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void ModoNovo()
        {
            _codigoEditando = 0; txtNome.Clear();
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void CarregarParaEditar()
        {
            if (grid.SelectedRows.Count == 0) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            _codigoEditando = cod;
            txtNome.Text = obj.grmeDescricao_ ?? "";
            cmbSituacao.SelectedItem = obj.Situacao;
            pnlForm.Visible = true; txtNome.Focus();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            var obj = new GrupoMercadoria
            {
                Codigo         = _codigoEditando,
                grmeDescricao_ = txtNome.Text.Trim(),
                grmeOrdem      = 0,
                Situacao       = _codigoEditando == 0 ? "A" : (cmbSituacao.SelectedItem?.ToString() ?? "A"),
            };
            var erro = _bll.Salvar(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            pnlForm.Visible = false; _codigoEditando = 0; Carregar();
        }

        private void BtnDesativar_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            if (MessageBox.Show("Desativar categoria?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var cod = Convert.ToInt32(grid.SelectedRows[0].Cells["Codigo"].Value);
            var obj = _bll.PesquisaCodigo(cod);
            if (obj == null) return;
            obj.Situacao = "I";
            _bll.Salvar(obj);
            pnlForm.Visible = false; Carregar();
        }
    }
}
