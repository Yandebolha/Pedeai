using System;

namespace Pedeai
{
    /// <summary>
    /// Barramento de eventos cross-thread para notificações entre jobs Quartz e a UI WinForms.
    /// </summary>
    public static class AppEvents
    {
        /// <summary>Disparado quando novos pedidos web foram importados do Supabase.</summary>
        public static event Action<int> NovoPedidoWebRecebido;

        public static void OnNovoPedidoWebRecebido(int count)
            => NovoPedidoWebRecebido?.Invoke(count);

        /// <summary>Disparado quando o Admin altera a configuração de conexão com o site.</summary>
        public static event Action<bool> SiteConectadoChanged;

        public static void OnSiteConectadoChanged(bool habilitado)
            => SiteConectadoChanged?.Invoke(habilitado);
    }
}
