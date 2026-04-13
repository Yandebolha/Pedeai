using PedeaiBackup.Forms;
using System;
using System.Windows.Forms;

namespace PedeaiBackup
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using var form = new frmBackupConfig();
            Application.Run(form);
        }
    }
}
