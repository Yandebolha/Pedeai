using System.Data;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class BairroBLL
    {
        private readonly BairroDAL _dal = new BairroDAL();

        public DataTable Listar(bool apenasAtivos = false) => _dal.Listar(apenasAtivos);

        public Bairro PesquisaCodigo(int codigo) => _dal.PesquisaCodigo(codigo);

        /// <summary>Busca taxa de entrega pelo nome do bairro e cidade (case-insensitive).</summary>
        public decimal? BuscarTaxa(string cidade, string bairro)
        {
            var obj = _dal.BuscarPorNome(cidade?.Trim(), bairro?.Trim());
            return obj == null ? (decimal?)null : obj.baiTaxa_Entrega;
        }

        public string Salvar(Bairro obj)
        {
            if (string.IsNullOrWhiteSpace(obj.baiNome))
                return "O nome do bairro é obrigatório.";

            if (obj.Codigo == 0)
                return _dal.Incluir(obj);
            else
                return _dal.Alterar(obj);
        }
    }
}
