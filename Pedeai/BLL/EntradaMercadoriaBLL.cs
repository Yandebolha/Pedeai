using System;
using System.Collections.Generic;
using System.Data;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    public class EntradaMercadoriaBLL
    {
        private readonly DAL.EntradaMercadoriaDAL         _dal        = new DAL.EntradaMercadoriaDAL();
        private readonly DAL.ParcelaEntradaMercadoriaDAL  _parcelaDal = new DAL.ParcelaEntradaMercadoriaDAL();

        public DataTable Listar(DateTime? de = null, DateTime? ate = null, int? codigoFornecedor = null)
            => _dal.Listar(de, ate, codigoFornecedor);

        public EntradaMercadoria PesquisaCodigo(int codigo) => _dal.PesquisaCodigo(codigo);

        public List<ItemEntradaMercadoria> ListarItens(int codigoEntrada) => _dal.ListarItens(codigoEntrada);

        public List<ParcelaEntradaMercadoria> ListarParcelas(int codigoEntrada)
            => _parcelaDal.ListarPorEntrada(codigoEntrada);

        public DataTable ListarAvisos(int diasAdiantados = 30)
            => _parcelaDal.ListarTodas("A");

        public DataTable ListarAvisosFiltrados(string filtro)
        {
            switch (filtro)
            {
                case "Vencidas":         return _parcelaDal.ListarPorVencimento("vencida");
                case "Vencem Hoje":      return _parcelaDal.ListarPorVencimento("hoje");
                case "Próximos 7 dias":  return _parcelaDal.ListarPorVencimento("7dias");
                case "Próximos 30 dias": return _parcelaDal.ListarPorVencimento("30dias");
                case "Pagas":            return _parcelaDal.ListarTodas("P");
                default:                 return _parcelaDal.ListarTodas("A");
            }
        }

        public int ContarParcelasVencendo(int dias = 7)
            => _parcelaDal.ContarVencendo(dias);

        public string MarcarParcelaPaga(int codigoParcela)
            => _parcelaDal.MarcarPago(codigoParcela);

        public string Inserir(EntradaMercadoria entrada, List<ItemEntradaMercadoria> itens,
            List<ParcelaEntradaMercadoria> parcelas = null)
        {
            if (string.IsNullOrWhiteSpace(entrada.entNome_Fornecedor))
                return "Informe o fornecedor.";
            if (itens == null || itens.Count == 0)
                return "Adicione pelo menos um item.";
            foreach (var item in itens)
            {
                if (item.itmQtde <= 0)
                    return $"Quantidade inválida para o item '{item.itmNome_Mercadoria}'.";
                if (item.itmPreco_Custo < 0)
                    return $"Custo inválido para o item '{item.itmNome_Mercadoria}'.";
            }
            return _dal.Inserir(entrada, itens, parcelas);
        }

        public string Cancelar(int codigo) => _dal.Cancelar(codigo);
    }
}
