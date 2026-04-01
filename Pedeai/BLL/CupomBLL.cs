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

        /// <summary>Valida e retorna o cupom pelo código digitado. Retorna (cupom, erro).</summary>
        public (Cupom cupom, string erro) ValidarEObter(string codigoDigitado, decimal subtotal)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigoDigitado))
                    return (null, "Informe o código do cupom.");
                var c = _dal.BuscarPorTexto(codigoDigitado.Trim().ToUpper());
                if (c == null)
                    return (null, "Cupom não encontrado.");
                if (c.cupomValido_Ate < DateTime.Today)
                    return (null, $"Cupom expirado em {c.cupomValido_Ate:dd/MM/yyyy}.");
                if (c.cupomLimite_Usos > 0 && c.cupomUsos_Realizados >= c.cupomLimite_Usos)
                    return (null, "Cupom esgotado (limite de usos atingido).");
                if (c.cupomPedido_Minimo > 0 && subtotal < c.cupomPedido_Minimo)
                    return (null, $"Pedido mínimo para este cupom: R$ {c.cupomPedido_Minimo:N2}.");
                return (c, "");
            }
            catch (Exception ex) { return (null, ex.Message); }
        }

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
