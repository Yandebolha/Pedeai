using System;
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

            // Testa a conexão com o banco antes de qualquer coisa.
            // Se falhar, abre a tela de configuração para o usuário apontar o servidor correto.
            while (!DB.DbHelper.TestarConexao())
            {
                var msg = MessageBox.Show(
                    "Não foi possível conectar ao banco de dados.\n\n" +
                    "Verifique se o servidor MySQL está acessível e configure a conexão.",
                    "Falha de Conexão",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);
                if (msg == DialogResult.Cancel) return;

                using var config = new Forms.frmConexao();
                if (config.ShowDialog() != DialogResult.OK) return;
            }

            // Cria tabelas, colunas e dados iniciais automaticamente
            if (!DbMigrator.Executar()) return;

            // Verifica licença antes de abrir o login
            var empresa = new EmpresaDAL().Carregar();
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
                // Aviso de vencimento próximo (últimos 5 dias do mês)
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
    }
}
