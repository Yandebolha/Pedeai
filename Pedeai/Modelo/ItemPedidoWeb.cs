namespace Pedeai.Modelo
{
    public class ItemPedidoWeb
    {
        public int auxCodigo { get; set; }
        public int Codigo { get; set; }
        public int Codigo_Pedido { get; set; }
        public int Codigo_Mercadoria { get; set; }
        /// <summary>
        /// Usado apenas em runtime para pedidos fracionados (½A + ½B).
        /// Não é persistido no banco — serve só para baixa dupla de estoque.
        /// </summary>
        public int Codigo_Mercadoria2 { get; set; } = 0;
        public int itpwCodigo_Marmita { get; set; } = 0;

        public string itpwNome_Mercadoria { get; set; }
        public decimal itpwQtde { get; set; }
        public decimal itpwPreco_Unitario { get; set; }
        public decimal itpwDesconto_Pct  { get; set; } = 0m;   // % de desconto do item
        public decimal itpwSubtotal { get; set; }
        public string itpwObservacoes { get; set; } = "";

        public string Status_Transmissao { get; set; } = "N";
        public string Info { get; set; } = "";
    }
}
