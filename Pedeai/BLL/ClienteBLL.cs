using System.Data;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class ClienteBLL
    {
        private readonly ClienteDAL _dal = new ClienteDAL();

        public DataTable Listar(string busca = "") => _dal.Listar(busca);

        public Cliente PesquisaCodigo(int codigo) => _dal.PesquisaCodigo(codigo);

        public string Salvar(Cliente obj)
        {
            if (string.IsNullOrWhiteSpace(obj.clieNome_RazaoSocial))
                return "Informe o nome do cliente.";

            return obj.Codigo == 0 ? _dal.Incluir(obj) : _dal.Alterar(obj);
        }

        public void IncrementarTotais(int codigoCliente, decimal valorPedido)
            => _dal.IncrementarTotais(codigoCliente, valorPedido);
    }
}
