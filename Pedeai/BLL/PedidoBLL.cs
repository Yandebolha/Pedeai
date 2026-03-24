using System;
using System.Data;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    /// <summary>
    /// Regras de negócio para Pedidos Web.
    /// Padrão ConstruFarma: BLL encapsula DAL e adiciona validações.
    /// </summary>
    public class PedidoBLL
    {
        private readonly PedidoDAL _dal = new PedidoDAL();

        public DataTable Listar(string situacao = null, DateTime? data = null)
            => _dal.Listar(situacao, data);

        public PedidoWeb PesquisaCodigo(int codigo)
            => _dal.PesquisaCodigo(codigo);

        public DataTable ListarItens(int codigoPedido)
            => _dal.ListarItens(codigoPedido);

        /// <summary>
        /// Atualiza situação do pedido com validação de transição de estados.
        /// 0=Pendente, 1=Confirmado, 2=Em Preparo, 3=Pronto, 4=Saiu p/ Entrega, 5=Entregue, 6=Cancelado
        /// </summary>
        public string AtualizarSituacao(int codigo, int novaSituacao)
        {
            if (codigo <= 0) return "Código de pedido inválido.";
            if (novaSituacao < 0 || novaSituacao > 6) return "Situação inválida.";
            try
            {
                _dal.AtualizarSituacao(codigo, novaSituacao);
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public static string LabelSituacao(int s)
        {
            switch (s)
            {
                case 0: return "⏸ Pendente";
                case 1: return "✓ Confirmado";
                case 2: return "⏳ Em Preparo";
                case 3: return "✅ Pronto";
                case 4: return "🚴 Saiu p/ Entrega";
                case 5: return "📦 Entregue";
                case 6: return "✕ Cancelado";
                default: return s.ToString();
            }
        }
    }
}
