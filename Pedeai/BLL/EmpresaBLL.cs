using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class EmpresaBLL
    {
        private readonly EmpresaDAL _dal = new EmpresaDAL();

        public Empresa Carregar() => _dal.Carregar();

        public string Salvar(Empresa obj)
        {
            if (string.IsNullOrWhiteSpace(obj.empNome)) return "Informe o nome da empresa.";
            return _dal.Salvar(obj);
        }
    }
}
