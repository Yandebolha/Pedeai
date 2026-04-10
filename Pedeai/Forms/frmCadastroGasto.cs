using System;
using System.Drawing;
using System.Windows.Forms;
using Pedeai.BLL;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmCadastroGasto : Form
    {
        private GastoMaterialBLL _bll;

        public frmCadastroGasto()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _bll = new GastoMaterialBLL();
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
