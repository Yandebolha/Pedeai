using System;
using System.Data;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class NecessidadeEmpresaBLL
    {
        private readonly NecessidadeEmpresaDAL _dal = new NecessidadeEmpresaDAL();

        public DataTable Listar(DateTime de, DateTime ate)         => _dal.Listar(de, ate);
        public decimal   TotalPeriodo(DateTime de, DateTime ate)   => _dal.TotalPeriodo(de, ate);

        public string Inserir(NecessidadeEmpresa obj)
        {
            if (string.IsNullOrWhiteSpace(obj.nempDescricao)) return "Informe a descricao.";
            if (obj.nempValor <= 0)                           return "Valor deve ser maior que zero.";
            return _dal.Inserir(obj);
        }

        public string Excluir(int codigo) => _dal.Excluir(codigo);
    }
}
