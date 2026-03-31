using System;
using System.IO;

namespace Pedeai
{
    /// <summary>Registra ações do sistema em Log.txt na pasta do executável.</summary>
    public static class Logger
    {
        private static readonly object _lock = new object();

        private static string FilePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log.txt");

        public static void Log(string acao, string detalhe = "")
        {
            try
            {
                string usuario = UsuarioSessao.NomeAtual;
                string linha   = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{usuario}] {acao}";
                if (!string.IsNullOrWhiteSpace(detalhe))
                    linha += $" | {detalhe}";

                lock (_lock)
                    File.AppendAllText(FilePath, linha + Environment.NewLine,
                                       System.Text.Encoding.UTF8);
            }
            catch { /* nunca lança exceção - log não deve derrubar a app */ }
        }
    }
}
