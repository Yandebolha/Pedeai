using System;
using System.Data;
using Pedeai.DAL;

namespace Pedeai.BLL
{
    public class DashboardBLL
    {
        private readonly DashboardDAL _dal = new DashboardDAL();

        public (int pedidosHoje, decimal faturamentoHoje, int clientesTotal, int pedidosPendentes)
            GetEstatisticas() => _dal.GetEstatisticas();

        public DataRow GetLoja() => _dal.GetLoja();

        public DataTable GetVendasPorCanal(DateTime de, DateTime ate) => _dal.GetVendasPorCanal(de, ate);
        public DataTable GetTopProdutos(DateTime de, DateTime ate, int top = 8) => _dal.GetTopProdutos(de, ate, top);
        public DataTable GetVendasPorDia(DateTime de, DateTime ate)   => _dal.GetVendasPorDia(de, ate);
        public DataTable GetVendasPorPeriodo(string periodo)          => _dal.GetVendasPorPeriodo(periodo);
        public DataTable GetTopProdutosHoje(int top = 3)              => _dal.GetTopProdutosHoje(top);
        public DataTable GetTopProdutosPeriodo(string periodo, int top = 3) => _dal.GetTopProdutosPeriodo(periodo, top);
    }
}
