using System;
using System.Configuration;
using System.Windows.Forms;
using Pedeai.DAL;
using Pedeai.DB;
using Pedeai.Forms;

namespace Pedeai
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Testa servidor MySQL e abre configuração se não conectar.
            // Loop: repete até conectar ou usuário cancelar.
            while (!DB.DbHelper.TestarConexao())
            {
                using var config = new Forms.frmConexao();
                config.MotivoErro = "Não foi possível conectar ao servidor MySQL.\nConfigure o endereço do servidor e tente novamente.";
                if (config.ShowDialog() != DialogResult.OK) return;
            }

            // Cria banco (se não existir) + tabelas + dados iniciais
            if (!DbMigrator.Executar()) return;

            // ── Auto-renovação de licença via VPS ───────────────────────────
            var empresa = new EmpresaDAL().Carregar();
            TentarAutoRenovarLicenca(empresa);

            // Recarrega após possível atualização da chave
            empresa = new EmpresaDAL().Carregar();

            bool licencaValida = LicencaService.ValidarChave(empresa.empCodigo_Empresa, empresa.empChave_Licenca);

            if (!licencaValida)
            {
                // Se ainda não há data de início de graça, inicia agora (novo install ou expiração)
                if (empresa.empData_Graca == null)
                {
                    new EmpresaDAL().SalvarDataGraca(empresa.Codigo, DateTime.Today);
                    empresa.empData_Graca = DateTime.Today;
                }

                int diasGraca = (int)(DateTime.Today - empresa.empData_Graca.Value).TotalDays;
                // diasGraca 0-3 = período de graça ativo; >= 4 = bloqueado (botão desabilitado)
                using var lic = new frmLicenca(empresa, diasGraca);
                if (lic.ShowDialog() != DialogResult.OK) return;
            }
            else
            {
                // Aviso de vencimento próximo (últimos 5 dias) — após auto-renovação, só aparece
                // se a VPS estava offline e não conseguiu renovar.
                var exp = LicencaService.ObterExpiracao(empresa.empChave_Licenca);
                if (exp.HasValue && (exp.Value - DateTime.Today).TotalDays <= 5)
                {
                    int dias = (int)(exp.Value - DateTime.Today).TotalDays;
                    string msg = dias == 0
                        ? "⚠️ Sua licença VENCE HOJE! Renove para continuar usando o sistema."
                        : $"⚠️ Sua licença vence em {dias} dia(s) ({exp.Value:dd/MM/yyyy}).\r\nRenove para não ser bloqueado.";
                    MessageBox.Show(msg, "Licença — Aviso de Vencimento",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            // Exibe tela de login; cancela aplicacao se o usuario fechar sem logar
            using (var login = new frmLogin())
            {
                if (login.ShowDialog() != DialogResult.OK) return;
            }

            Application.Run(new Form1());
        }

        /// <summary>
        /// Tenta renovar automaticamente a licença via VPS se ela estiver vencida ou próxima do vencimento.
        /// Falha silenciosamente se a VPS estiver offline ou não configurada.
        /// </summary>
        private static void TentarAutoRenovarLicenca(Modelo.Empresa empresa)
        {
            string vpsUrl = ConfigurationManager.AppSettings["LicencaVpsUrl"] ?? "";
            string apiKey = ConfigurationManager.AppSettings["LicencaApiKey"] ?? "";

            if (string.IsNullOrWhiteSpace(vpsUrl) || string.IsNullOrWhiteSpace(apiKey))
                return; // VPS não configurada — usa fluxo manual

            // Renovar se: licença inválida OU vencendo em até 7 dias
            bool precisaRenovar = !LicencaService.ValidarChave(empresa.empCodigo_Empresa, empresa.empChave_Licenca);
            if (!precisaRenovar)
            {
                var exp = LicencaService.ObterExpiracao(empresa.empChave_Licenca);
                precisaRenovar = exp.HasValue && (exp.Value - DateTime.Today).TotalDays <= 7;
            }

            if (!precisaRenovar) return;

            string novaChave = LicencaApiClient.TentarRenovar(
                empresa.empCodigo_Empresa, apiKey, vpsUrl, diasValidade: 30);

            if (!string.IsNullOrWhiteSpace(novaChave)
                && LicencaService.ValidarChave(empresa.empCodigo_Empresa, novaChave))
            {
                new EmpresaDAL().SalvarLicenca(empresa.Codigo, novaChave);
                // Reseta período de graça se havia sido iniciado
                if (empresa.empData_Graca.HasValue)
                    new EmpresaDAL().SalvarDataGraca(empresa.Codigo, null);
            }
        }
    }
}
