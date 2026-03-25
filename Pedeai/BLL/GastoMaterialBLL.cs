using System;
using System.Data;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class GastoMaterialBLL
    {
        private readonly GastoMaterialDAL _dal = new GastoMaterialDAL();

        public DataTable Listar(DateTime de, DateTime ate)
            => _dal.Listar(de, ate);

        public decimal TotalPeriodo(DateTime de, DateTime ate)
            => _dal.TotalPeriodo(de, ate);

        public string Inserir(GastoMaterial obj)
        {
            if (string.IsNullOrWhiteSpace(obj.gmatDescricao)) return "Informe a descricao.";
            if (obj.gmatValor <= 0)                           return "Valor deve ser maior que zero.";
            return _dal.Inserir(obj);
        }

        public string Excluir(int codigo)
            => _dal.Excluir(codigo);
    }
}
