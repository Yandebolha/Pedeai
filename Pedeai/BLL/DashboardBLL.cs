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
    }
}
