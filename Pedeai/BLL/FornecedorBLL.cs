using System.Data;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class FornecedorBLL
    {
        private readonly FornecedorDAL _dal = new FornecedorDAL();

        public DataTable Listar() => _dal.Listar();

        public Fornecedor PesquisaCodigo(int codigo) => _dal.PesquisaCodigo(codigo);

        public string Salvar(Fornecedor obj)
        {
            if (string.IsNullOrWhiteSpace(obj.fornNome_RazaoSocial))
                return "Informe a Razão Social do fornecedor.";

            return obj.Codigo == 0 ? _dal.Incluir(obj) : _dal.Alterar(obj);
        }
    }
}
