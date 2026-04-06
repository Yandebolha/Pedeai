using System;
using System.Data;
using MySqlConnector;
using Pedeai.DAL;

namespace Pedeai.BLL
{
    public class EstoqueBLL
    {
        private readonly MercadoriaDAL _dal = new MercadoriaDAL();

        /// <summary>Lista todos os produtos com estoque controlado.</summary>
        public DataTable Listar(string filtro = "")
        {
            return _dal.ListarEstoque(filtro);
        }

        /// <summary>Ajusta o estoque de um produto (+entrada / -saida).</summary>
        public string Ajustar(int codigoProduto, decimal delta, string tipo, string obs)
        {
            if (delta == 0) return "Informe uma quantidade maior que zero.";
            return _dal.AjustarEstoque(codigoProduto, delta, tipo, obs);
        }
    }
}
