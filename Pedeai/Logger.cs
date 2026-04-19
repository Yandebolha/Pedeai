using System;
using System.IO;

namespace Pedeai
{
    /// <summary>Registra ações e erros do sistema em Log_sistema.txt na pasta C:\Pedeai.</summary>
    public static class Logger
    {
        private static readonly object _lock = new object();

        private static string FilePath
        {
            get
            {
                string dir = @"C:\Pedeai";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return Path.Combine(dir, "Log_sistema.txt");
            }
        }

        /// <summary>
        /// Grava uma entrada no log no formato:
        /// dd/MM/yyyy HH:mm:ss - {source} - {method} - {message}
        ///    em {StackTrace}  (apenas quando ex != null)
        /// </summary>
        public static void Log(string source, string method, string message, Exception ex = null)
        {
            try
            {
                string linha = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {source} - {method} - {message}";
                if (ex != null)
                    linha += Environment.NewLine + ex.ToString();

                lock (_lock)
                    File.AppendAllText(FilePath, linha + Environment.NewLine,
                                       System.Text.Encoding.UTF8);
            }
            catch { /* nunca lança exceção - log não deve derrubar a app */ }
        }
    }
}
