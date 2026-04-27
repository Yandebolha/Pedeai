using System;
using System.Threading.Tasks;
using Quartz;

namespace Pedeai.DB
{
    /// <summary>
    /// Job Quartz executado a cada 10 segundos para importar pedidos pendentes do Supabase.
    /// </summary>
    [DisallowConcurrentExecution]
    public class WebOrderPollingJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var pedidos = await SupabaseService.BuscarPedidosPendentesAsync();
                int importados = 0;

                foreach (var pedido in pedidos)
                {
                    var erro = await SupabaseService.ImportarPedidoAsync(pedido);
                    if (string.IsNullOrEmpty(erro))
                    {
                        importados++;
                        // Mark as received in Supabase so we don't re-import
                        await SupabaseService.AtualizarStatusPedidoWebAsync(pedido.Id, "recebido");
                    }
                    else
                    {
                        Logger.Log("WebOrderPollingJob", "Execute",
                            $"Erro ao importar pedido {pedido.Id}: {erro}", null);
                    }
                }

                if (importados > 0)
                    AppEvents.OnNovoPedidoWebRecebido(importados); // silent grid refresh only
            }
            catch (Exception ex)
            {
                Logger.Log("WebOrderPollingJob", "Execute", "Erro no polling de pedidos web", ex);
            }
        }
    }
}
