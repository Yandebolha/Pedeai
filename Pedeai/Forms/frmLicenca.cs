using System;
using System.Windows.Forms;
using Pedeai.DAL;
using Pedeai.DB;
using Pedeai.Modelo;

namespace Pedeai.Forms
{
    public partial class frmLicenca : Form
    {
        private readonly Empresa    _empresa;
        private readonly EmpresaDAL _dal;
        private readonly int        _diasGraca; // -1 = não está em graça; 0..3 = dias restantes

        /// <param name="diasGraca">
        ///   -1 = licença totalmente inválida/sem graça (só aceita nova chave).
        ///    0..3 = expirada mas dentro dos 4 dias de graça (mostra botão de desbloqueio).
        /// </param>
        public frmLicenca(Empresa empresa, int diasGraca = -1)
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
            _empresa   = empresa;
            _dal       = new EmpresaDAL();
            _diasGraca = diasGraca;
            Load += FrmLicenca_Load;
        }

        private void FrmLicenca_Load(object sender, EventArgs e)
        {
            txtCodigo.Text = _empresa.empCodigo_Empresa;

            // Sempre mostra os controles de graça — apenas altera texto/estado
            lblGracaAviso.Visible = true;
            btnGraca.Visible      = true;

            bool licencaJaUsada = !string.IsNullOrEmpty(_empresa.empChave_Licenca);

            if (_diasGraca >= 0 && _diasGraca <= 3)
            {
                // Dentro do período de graça (0-3 dias decorridos)
                lblTitulo.Text      = licencaJaUsada ? "⚠️  Licença Expirada" : "🔒  Ativação do Sistema";
                lblTitulo.ForeColor = System.Drawing.Color.FromArgb(160, 60, 10);
                lblInstrucao.Text   = licencaJaUsada
                    ? "Sua licença venceu. Informe a nova chave para renovar\r\nou use o período de graça para continuar temporariamente."
                    : "O sistema ainda não está ativado.\r\nForneça o Código da Empresa ao suporte para receber sua chave.";

                int diasRestantes = 4 - _diasGraca;
                lblGracaAviso.Text = diasRestantes == 1
                    ? "⚠️ Último dia do período de graça!"
                    : $"⚠️ Período de graça: {diasRestantes} dia(s) restante(s).";
                btnGraca.Text    = $"⏳  Usar Período de Graça ({diasRestantes} dia(s) restante(s))";
                btnGraca.Enabled = true;
            }
            else
            {
                // Período de graça expirado (>= 4 dias) — mostra botão desabilitado
                lblTitulo.Text      = "❌  Licença Bloqueada";
                lblTitulo.ForeColor = System.Drawing.Color.FromArgb(160, 30, 10);
                lblInstrucao.Text   = "O período de graça expirou. É necessário informar\r\numa nova chave de ativação para continuar.";
                lblGracaAviso.Text  = "❌ Período de graça encerrado. Informe a chave para desbloquear.";
                btnGraca.Text      = "⏳  Período de Graça Expirado";
                btnGraca.Enabled   = false;
                btnGraca.BackColor = System.Drawing.Color.FromArgb(150, 150, 150);
            }
        }

        private void BtnCopiar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                Clipboard.SetText(txtCodigo.Text);
                btnCopiar.Text = "✔ Copiado!";
                var t = new System.Windows.Forms.Timer { Interval = 2000 };
                t.Tick += (_, __) => { btnCopiar.Text = "Copiar"; t.Stop(); t.Dispose(); };
                t.Start();
            }
        }

        private void BtnAtivar_Click(object sender, EventArgs e)
        {
            string chave = txtChave.Text.Trim();
            if (string.IsNullOrWhiteSpace(chave))
            {
                MessageBox.Show("Informe a chave de ativação.", "Ativação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChave.Focus();
                return;
            }

            if (!LicencaService.ValidarChave(_empresa.empCodigo_Empresa, chave))
            {
                MessageBox.Show(
                    "Chave inválida ou expirada para este sistema.\r\n\r\n" +
                    "Verifique se a chave foi gerada para o Código da Empresa mostrado acima " +
                    "e se o mês de validade ainda não passou.",
                    "Ativação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtChave.Focus();
                return;
            }

            try
            {
                _dal.SalvarLicenca(_empresa.Codigo, chave);
                var exp = LicencaService.ObterExpiracao(chave);
                string expStr = exp.HasValue ? $"\r\nVálida até: {exp.Value:dd/MM/yyyy}" : "";
                MessageBox.Show("Sistema ativado com sucesso! Bem-vindo." + expStr, "Ativação",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar licença:\n" + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGraca_Click(object sender, EventArgs e)
        {
            // Permite abrir o sistema sem nova chave — apenas durante o período de graça
            DialogResult = DialogResult.OK;
        }

        private void BtnSair_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}

