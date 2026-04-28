using System;
using System.Data;
using System.Threading.Tasks;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class MercadoriaBLL
    {
        private readonly MercadoriaDAL _dal = new MercadoriaDAL();

        public DataTable Listar() => _dal.Listar();

        public Mercadoria PesquisaCodigo(int codigo) => _dal.PesquisaCodigo(codigo);

        public string Salvar(Mercadoria obj)
        {
            if (string.IsNullOrWhiteSpace(obj.mercMercadoria))
                return "Informe o nome do produto.";
            if (obj.mercPreco_Venda <= 0)
                return "Preço de venda deve ser maior que zero.";
            if (obj.Codigo_Grupo <= 0)
                return "Selecione uma categoria.";

            var erro = obj.Codigo == 0 ? _dal.Incluir(obj) : _dal.Alterar(obj);
            if (string.IsNullOrEmpty(erro))
                Task.Run(async () => await Pedeai.DB.SupabaseService.SincronizarProdutoAsync(obj.Codigo));
            return erro;
        }

        public void AlternarSituacao(int codigo)
        {
            _dal.AlternarSituacao(codigo);
            Task.Run(async () => await Pedeai.DB.SupabaseService.SincronizarProdutoAsync(codigo));
        }
    }
}
