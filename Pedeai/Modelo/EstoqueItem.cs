using System;

namespace Pedeai.Modelo
{
    public class EstoqueItem
    {
        public int     Codigo            { get; set; }
        public string  estoNome          { get; set; } = "";
        public string  estoUnidade       { get; set; } = "un";
        public decimal estoQtde_Atual    { get; set; }
        public decimal estoPreco_Custo   { get; set; }
        public bool    estoEh_Produto             { get; set; }
        public decimal estoFracao_Entrada          { get; set; } = 1;
        public string  estoFracao_Entrada_Unidade  { get; set; } = "";
        public decimal estoFracao_Saida            { get; set; } = 1;
        public string  estoFracao_Saida_Unidade    { get; set; } = "";
        public int?    Codigo_Grupo        { get; set; }
        public int?    Codigo_Mercadoria   { get; set; }
        public string  Situacao          { get; set; } = "A";
        public DateTime estoData_Cadastro { get; set; } = DateTime.Now;
    }
}
