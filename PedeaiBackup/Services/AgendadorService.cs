using PedeaiBackup.Models;
using System;
using System.Threading;

namespace PedeaiBackup.Services
{
    /// <summary>
    /// Serviço de agendamento de backup. Verifica a cada minuto se chegou a hora
    /// de executar um backup conforme os dias e horários configurados.
    /// </summary>
    public class AgendadorService : IDisposable
    {
        private Timer         _timer;
        private BackupConfig  _config;
        private BackupEstado  _estado;
        private bool          _executandoBackup = false;

        public event Action<string>          OnLog;
        public event Action<BackupResultado> OnBackupConcluido;

        /// <summary>Inicia o monitoramento de agendamento.</summary>
        public void Iniciar(BackupConfig config, BackupEstado estado)
        {
            _config = config;
            _estado = estado;

            // Verifica agendamento a cada 30 segundos
            _timer = new Timer(VerificarAgendamento, null,
                TimeSpan.Zero, TimeSpan.FromSeconds(30));
        }

        /// <summary>Atualiza a configuração e o estado sem reiniciar o timer.</summary>
        public void AtualizarConfig(BackupConfig config, BackupEstado estado)
        {
            _config = config;
            _estado = estado;
        }

        private void VerificarAgendamento(object state)
        {
            if (_config == null) return;
            if (_executandoBackup) return;
            if (_config.DiasAtivos == null || _config.DiasAtivos.Count == 0) return;
            if (_config.Horarios    == null || _config.Horarios.Count    == 0) return;

            var agora = DateTime.Now;
            int diaAtual = (int)agora.DayOfWeek; // 0=Dom … 6=Sáb

            if (!_config.DiasAtivos.Contains(diaAtual)) return;

            string horaAtual = agora.ToString("HH:mm");

            foreach (string horario in _config.Horarios)
            {
                if (horario.Trim() != horaAtual) continue;

                // Evita executar múltiplas vezes no mesmo minuto
                DateTime? ultimoBackup = _estado.UltimoBackupIncremental ?? _estado.UltimoBackupCompleto;
                if (ultimoBackup.HasValue && (agora - ultimoBackup.Value).TotalMinutes < 1)
                    continue;

                ExecutarBackup();
                break;
            }
        }

        /// <summary>Executa o backup em uma thread separada para não bloquear o timer.</summary>
        public void ExecutarBackup(bool forcarCompleto = false)
        {
            if (_executandoBackup) return;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                _executandoBackup = true;
                try
                {
                    Log("Iniciando backup...");
                    var svc = new BackupService(_config);
                    var result = svc.Executar(_estado, forcarCompleto);

                    // Persiste o estado atualizado
                    ConfigManager.SalvarEstado(_estado);

                    if (result.Sucesso)
                        Log($"Backup {result.Tipo} concluído: {result.Arquivo} ({FormatarBytes(result.TamanhoBytes)})");
                    else
                        Log($"Falha no backup {result.Tipo}: {result.Erro}");

                    OnBackupConcluido?.Invoke(result);
                }
                catch (Exception ex)
                {
                    Log($"Erro inesperado no backup: {ex.Message}");
                }
                finally
                {
                    _executandoBackup = false;
                }
            });
        }

        private void Log(string msg)
        {
            OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {msg}");
        }

        private static string FormatarBytes(long bytes)
        {
            if (bytes < 1024)        return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024.0):F1} MB";
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }
    }
}
