using System.Collections.Generic;
using System.Data;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class MarmitaBLL
    {
        private readonly MarmitaDAL _dal = new MarmitaDAL();

        public DataTable Listar(bool apenasAtivas = false) => _dal.Listar(apenasAtivas);

        public Marmita PesquisaCodigo(int codigo) => _dal.PesquisaCodigo(codigo);

        public string Salvar(Marmita obj)
        {
            if (string.IsNullOrWhiteSpace(obj.marDescricao))
                return "Informe a descrição da marmita.";
            if (obj.marValor <= 0)
                return "O valor da marmita deve ser maior que zero.";
            return obj.Codigo == 0 ? _dal.Incluir(obj) : _dal.Alterar(obj);
        }

        public string Excluir(int codigo) => _dal.Excluir(codigo);

        // ── Itens ──────────────────────────────────────────────────────────────

        public List<MarmitaItem> ListarItens(int codigoMarmita)
            => _dal.ListarItens(codigoMarmita);

        public string AdicionarItem(MarmitaItem item)
        {
            if (item.Codigo_Marmita <= 0) return "Marmita inválida.";
            if (string.IsNullOrWhiteSpace(item.maritmNome)) return "Informe o nome do produto.";
            if (item.maritmQtde <= 0) return "A quantidade deve ser maior que zero.";
            return _dal.AdicionarItem(item);
        }

        public string RemoverItem(int codigoItem) => _dal.RemoverItem(codigoItem);
    }
}
