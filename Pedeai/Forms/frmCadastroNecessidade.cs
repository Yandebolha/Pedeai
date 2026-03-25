using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroNecessidade : Form
    {
        public frmCadastroNecessidade()
        {
            InitializeComponent();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescricao.Text)) { MessageBox.Show("Informe a descrição."); return; }
            if (numValor.Value <= 0) { MessageBox.Show("Informe o valor."); return; }

            var obj = new NecessidadeEmpresa
            {
                nempData      = dtpData.Value.Date,
                nempCategoria = cmbCategoria.SelectedItem?.ToString() ?? "",
                nempDescricao = txtDescricao.Text.Trim(),
                nempValor     = numValor.Value
            };

            var bll  = new NecessidadeEmpresaBLL();
            var erro = bll.Inserir(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
