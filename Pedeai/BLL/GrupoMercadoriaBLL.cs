using System.Data;
using System.Threading.Tasks;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class GrupoMercadoriaBLL
    {
        private readonly GrupoMercadoriaDAL _dal = new GrupoMercadoriaDAL();

        public DataTable Listar(bool apenasAtivas = false) => _dal.Listar(apenasAtivas);

        public GrupoMercadoria PesquisaCodigo(int codigo) => _dal.PesquisaCodigo(codigo);

        public string Salvar(GrupoMercadoria obj)
        {
            if (string.IsNullOrWhiteSpace(obj.grmeDescricao_))
                return "Informe o nome da categoria.";

            var erro = obj.Codigo == 0 ? _dal.Incluir(obj) : _dal.Alterar(obj);
            if (string.IsNullOrEmpty(erro))
                Task.Run(async () => await Pedeai.DB.SupabaseService.SincronizarGrupoAsync(obj.Codigo));
            return erro;
        }
    }
}
