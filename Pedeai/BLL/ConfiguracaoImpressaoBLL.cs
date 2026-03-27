using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class ConfiguracaoImpressaoBLL
    {
        private readonly ConfiguracaoImpressaoDAL _dal = new ConfiguracaoImpressaoDAL();

        public ConfiguracaoImpressao Carregar() => _dal.Carregar();

        public string Salvar(ConfiguracaoImpressao obj)
        {
            if (obj.larguraCaracteres < 20) obj.larguraCaracteres = 42;
            if (string.IsNullOrWhiteSpace(obj.separador)) obj.separador = "-";
            return _dal.Salvar(obj);
        }
    }
}
