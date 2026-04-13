using System;
using System.Diagnostics;
using System.IO;

namespace PedeaiBackup.Services
{
    /// <summary>
    /// Integração com MEGAcmd CLI para upload de arquivos na nuvem MEGA.
    ///
    /// Pré-requisito: MEGAcmd instalado (https://mega.io/cmd).
    /// No Windows o instalador adiciona mega-cmd ao PATH, ou configure o caminho manualmente.
    /// </summary>
    public static class MegaService
    {
        /// <summary>
        /// Executa um sub-comando MEGAcmd e retorna (saída, erro, exitCode).
        /// </summary>
        private static (string stdout, string stderr, int exitCode) RunCmd(string megaCmdPath, string subCommand)
        {
            // MEGAcmd no Windows é invocado como:  mega-cmd.exe login email password
            // Ou via script batch gerado no PATH pelo instalador.
            string exe;
            string args;

            // Se o caminho termina com .exe ou .bat, usa diretamente com sub-comando
            if (megaCmdPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                || megaCmdPath.EndsWith(".bat", StringComparison.OrdinalIgnoreCase))
            {
                exe  = megaCmdPath;
                args = subCommand;
            }
            else
            {
                // Assume que 'mega-cmd' está no PATH: adiciona o sub-comando como primeiro argumento
                // Formato alternativo: mega-login, mega-put etc. como executáveis separados
                exe  = megaCmdPath;
                args = subCommand;
            }

            var psi = new ProcessStartInfo(exe, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true
            };

            using var proc = Process.Start(psi);
            if (proc == null)
                return (string.Empty, "Não foi possível iniciar o processo MEGAcmd.", -1);

            string stdout = proc.StandardOutput.ReadToEnd();
            string stderr = proc.StandardError.ReadToEnd();
            proc.WaitForExit(60_000); // timeout 60 s
            return (stdout, stderr, proc.ExitCode);
        }

        /// <summary>
        /// Verifica se há uma sessão ativa no MEGAcmd (whoami).
        /// </summary>
        public static bool EstaLogado(string megaCmdPath)
        {
            try
            {
                var (stdout, _, exitCode) = RunCmd(megaCmdPath, "whoami");
                return exitCode == 0 && !string.IsNullOrWhiteSpace(stdout);
            }
            catch { return false; }
        }

        /// <summary>
        /// Faz login no MEGAcmd com email e senha.
        /// </summary>
        public static (bool sucesso, string mensagem) Login(string megaCmdPath, string email, string senha)
        {
            try
            {
                // Escapa os argumentos para evitar injeção de comandos
                string emailSafe = email.Replace("\"", "").Replace("'", "").Trim();
                string senhaSafe = senha.Replace("\"", "").Replace("'", "").Trim();

                var (stdout, stderr, exitCode) = RunCmd(megaCmdPath, $"login \"{emailSafe}\" \"{senhaSafe}\"");
                if (exitCode == 0)
                    return (true, "Login realizado com sucesso.");

                string msg = string.IsNullOrWhiteSpace(stderr) ? stdout : stderr;
                return (false, $"Falha no login (código {exitCode}): {msg.Trim()}");
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao iniciar MEGAcmd: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria a pasta remota se não existir.
        /// </summary>
        public static void CriarPastaSeNecessario(string megaCmdPath, string pasta)
        {
            try { RunCmd(megaCmdPath, $"mkdir -p \"{pasta}\""); }
            catch { /* ignora */ }
        }

        /// <summary>
        /// Faz upload de um arquivo local para a pasta remota no MEGA.
        /// </summary>
        public static (bool sucesso, string mensagem) Upload(string megaCmdPath, string arquivoLocal, string pastaRemota)
        {
            if (!File.Exists(arquivoLocal))
                return (false, $"Arquivo não encontrado: {arquivoLocal}");

            try
            {
                // Garante que a pasta existe
                CriarPastaSeNecessario(megaCmdPath, pastaRemota);

                // Se não está logado, retorna erro informativo
                if (!EstaLogado(megaCmdPath))
                    return (false, "MEGAcmd não está logado. Verifique as credenciais em 'Testar MEGA'.");

                string localSafe  = arquivoLocal.Replace("\"", "");
                string remoteSafe = pastaRemota.TrimEnd('/') + "/";

                var (stdout, stderr, exitCode) = RunCmd(megaCmdPath, $"put \"{localSafe}\" \"{remoteSafe}\"");

                if (exitCode == 0)
                    return (true, $"Upload concluído: {Path.GetFileName(arquivoLocal)}");

                string msg = string.IsNullOrWhiteSpace(stderr) ? stdout : stderr;
                return (false, $"Falha no upload (código {exitCode}): {msg.Trim()}");
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao fazer upload: {ex.Message}");
            }
        }
    }
}
