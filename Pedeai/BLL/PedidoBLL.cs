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

        public DataTable Listar(string filtro = null, DateTime? data = null)
            => _dal.Listar(filtro, data);

        public PedidoWeb PesquisaCodigo(int codigo)
            => _dal.PesquisaCodigo(codigo);

        public PedidoWeb PesquisaPorNumero(string numero)
            => _dal.PesquisaPorNumero(numero);

        public DataTable ListarItens(int codigoPedido)
            => _dal.ListarItens(codigoPedido);

        public System.Collections.Generic.List<Modelo.ItemPedidoWeb> ListarItensObjetos(int codigoPedido)
            => _dal.ListarItensObjetos(codigoPedido);

        /// <summary>
        /// Valida se a transicao de estados e permitida.
        /// Maquina de estados: 0→1→2→3→(4→)5  |  0→6 (cancelar pendente sem auth)
        ///                     Confirmado+ so pode ser cancelado com autorizacao gerencial.
        /// </summary>
        public static bool PodeTransicionar(int atual, int novo)
        {
            if (atual == 5 || atual == 6) return false; // estados terminais
            switch (atual)
            {
                case 0: return novo == 1 || novo == 6;   // Pendente → Confirmado ou Cancelar
                case 1: return novo == 2;                 // Confirmado → Em Preparo
                case 2: return novo == 3 || novo == 4; // Em Preparo → Pronto ou Saiu p/ Entrega
                case 3: return novo == 4 || novo == 5;   // Pronto → Saiu ou Entregue
                case 4: return novo == 5;                 // Saiu → Entregue
                default: return false;
            }
        }

        /// <summary>Retorna true se o cancelamento deste pedido requer autorizacao gerencial.</summary>
        public static bool CancelamentoRequerAutorizacao(int situacaoAtual)
            => situacaoAtual > 0; // qualquer estado alem de Pendente precisa de Gerente+

        /// <summary>
        /// Atualiza situacao do pedido. Para cancelamento requer canceladoPor (nome do autorizador).
        /// </summary>
        public string AtualizarSituacao(int codigo, int novaSituacao, string canceladoPor = null)
        {
            if (codigo <= 0)            return "Codigo de pedido invalido.";
            if (novaSituacao < 0 || novaSituacao > 6) return "Situacao invalida.";
            try
            {
                _dal.AtualizarSituacao(codigo, novaSituacao, canceladoPor);
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string FinalizarPedido(int codigo, int novaSituacao, decimal valorPago, string transacao,
                                      decimal pagoDinheiro = 0, decimal pagoCartao = 0, decimal pagoPix = 0,
                                      string autorizador = "")
        {
            if (codigo <= 0) return "Código de pedido inválido.";
            try { _dal.FinalizarPedido(codigo, novaSituacao, valorPago, transacao, pagoDinheiro, pagoCartao, pagoPix, autorizador); return ""; }
            catch (Exception ex) { return ex.Message; }
        }

        public System.Data.DataTable GetFinanceiro(DateTime de, DateTime ate)
            => _dal.GetFinanceiro(de, ate);

        public System.Data.DataTable ListarConsulta(DateTime de, DateTime ate, string cliente = "", string numero = "")
            => _dal.ListarConsulta(de, ate, cliente, numero);

        public System.Data.DataTable GetComprasPorDia(DateTime de, DateTime ate)
            => _dal.GetComprasPorDia(de, ate);

        public System.Data.DataTable GetMovimentacoesDia(DateTime dia)
            => _dal.GetMovimentacoesDia(dia);

        public System.Data.DataTable GetMovimentacoesPeriodo(DateTime de, DateTime ate)
            => _dal.GetMovimentacoesPeriodo(de, ate);

        public string InserirManual(PedidoWeb pedido, System.Collections.Generic.List<ItemPedidoWeb> itens)
        {
            if (string.IsNullOrWhiteSpace(pedido.pediNome_Cliente))
                return "Informe o nome do cliente.";
            if (itens == null || itens.Count == 0)
                return "Adicione ao menos um item ao pedido.";
            return _dal.InserirManual(pedido, itens);
        }

        public static string LabelSituacao(int s)
        {
            switch (s)
            {
                case 0: return "⏸ Pendente";
                case 1: return "✓ Confirmado";
                case 2: return "⏳ Em Preparo";
                case 3: return "✅ Pronto";
                case 4: return "\u2192 Saiu p/ Entrega";
                case 5: return "\u2713 Entregue";
                case 6: return "✕ Cancelado";
                default: return s.ToString();
            }
        }
    }
}
