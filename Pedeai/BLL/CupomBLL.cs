using System;
using System.Data;
using Pedeai.DAL;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class CupomBLL
    {
        private readonly CupomDAL _dal = new CupomDAL();

        public DataTable Listar() => _dal.Listar();

        public Cupom PesquisaCodigo(int codigo) => _dal.PesquisaCodigo(codigo);

        public string Salvar(Cupom obj)
        {
            if (string.IsNullOrWhiteSpace(obj.cupomCodigo))
                return "Informe o código do cupom.";
            if (obj.cupomValor <= 0)
                return "O valor/percentual do cupom deve ser maior que zero.";
            if (obj.cupomTipo == "PERCENTUAL" && obj.cupomValor > 100)
                return "Percentual de desconto não pode ser maior que 100%.";
            if (obj.cupomValido_Ate < DateTime.Today)
                return "A data de validade não pode estar no passado.";

            return obj.Codigo == 0 ? _dal.Incluir(obj) : _dal.Alterar(obj);
        }
    }
}
