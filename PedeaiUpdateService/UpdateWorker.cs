using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PedeaiUpdateService
{
    /// <summary>
    /// Worker principal do serviço Windows. Roda em background e verifica
    /// atualizações no intervalo configurado.
    /// </summary>
    public class UpdateWorker : BackgroundService
    {
        private readonly IConfiguration _cfg;
        private readonly ILogger<UpdateWorker> _log;
        private Updater _updater;

        public UpdateWorker(IConfiguration cfg, ILogger<UpdateWorker> log)
        {
            _cfg     = cfg;
            _log     = log;
            _updater = new Updater(cfg, log);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _log.LogInformation("PedeaiUpdateService iniciado.");

            // Registra o cliente no servidor, se ainda não tiver ClienteId
            long clienteId = 0;
            try
            {
                clienteId = await _updater.RegistrarAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _log.LogWarning("Não foi possível registrar no servidor: {Msg}. Tentará novamente...", ex.Message);
            }

            int intervalo = _cfg.GetValue<int>("IntervalMinutos");
            if (intervalo < 5) intervalo = 60;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (clienteId == 0)
                    {
                        _log.LogInformation("Tentando registrar...");
                        clienteId = await _updater.RegistrarAsync(stoppingToken);
                    }

                    if (clienteId > 0)
                        await VerificarEAplicarAsync(clienteId, stoppingToken);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Erro no ciclo de verificação.");
                }

                _log.LogInformation("Próxima verificação em {Min} minutos.", intervalo);
                await Task.Delay(TimeSpan.FromMinutes(intervalo), stoppingToken);
            }

            _log.LogInformation("PedeaiUpdateService encerrado.");
        }

        private async Task VerificarEAplicarAsync(long clienteId, CancellationToken ct)
        {
            string versaoAtual = _cfg["VersaoAtual"] ?? "1.0.0";
            _log.LogInformation("Verificando atualização... ClienteId={Id} VersaoAtual={V}", clienteId, versaoAtual);

            var (tem, pacoteId, versao, caminhoArquivo) = await _updater.VerificarAsync(clienteId, versaoAtual, ct);

            if (!tem)
            {
                _log.LogInformation("Nenhuma atualização disponível.");
                return;
            }

            _log.LogInformation("Atualização disponível: v{V} (pacote {P}). Baixando...", versao, pacoteId);

            string zipPath = await _updater.BaixarAsync(clienteId, pacoteId, caminhoArquivo, ct);
            _log.LogInformation("Download concluído. Aplicando...");

            var (ok, erro) = await _updater.AplicarAsync(zipPath, versao, ct);

            if (ok)
            {
                _log.LogInformation("Atualização v{V} aplicada com sucesso.", versao);
                await _updater.ConfirmarAsync(clienteId, pacoteId, true, null, ct);
            }
            else
            {
                _log.LogError("Falha ao aplicar atualização: {Erro}", erro);
                await _updater.ConfirmarAsync(clienteId, pacoteId, false, erro, ct);
            }
        }
    }
}
