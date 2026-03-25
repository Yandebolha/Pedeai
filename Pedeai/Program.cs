using System;
using System.Windows.Forms;
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

            // Cria tabelas, colunas e dados iniciais automaticamente
            if (!DbMigrator.Executar()) return;

            // Exibe tela de login; cancela aplicacao se o usuario fechar sem logar
            using (var login = new frmLogin())
            {
                if (login.ShowDialog() != DialogResult.OK) return;
            }

            Application.Run(new Form1());
        }
    }
}
