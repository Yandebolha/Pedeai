using PedeaiUpdateAdmin.Forms;
using System;
using System.Windows.Forms;

namespace PedeaiUpdateAdmin
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, e) =>
                MessageBox.Show(e.Exception.ToString(), "Erro não tratado — ThreadException",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
                MessageBox.Show(e.ExceptionObject?.ToString(), "Erro não tratado — AppDomain",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Run(new frmUpdateAdmin());
        }
    }
}
