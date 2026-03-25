using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroGasto : Form
    {
        private readonly GastoMaterialBLL _bll = new GastoMaterialBLL();

        public frmCadastroGasto()
        {
            BuildUI();
            dtpData.Value = DateTime.Today;
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            var obj = new GastoMaterial
            {
                gmatData        = dtpData.Value.Date,
                gmatDescricao   = txtDescricao.Text.Trim(),
                gmatValor       = numValor.Value,
                gmatObservacoes = txtObs.Text.Trim()
            };
            var erro = _bll.Inserir(obj);
            if (!string.IsNullOrEmpty(erro)) { MessageBox.Show("Erro: " + erro); return; }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
