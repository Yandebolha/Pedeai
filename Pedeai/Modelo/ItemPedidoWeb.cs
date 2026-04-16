namespace Pedeai.Modelo
{
    public class ItemPedidoWeb
    {
        public int auxCodigo { get; set; }
        public int Codigo { get; set; }
        public int Codigo_Pedido { get; set; }
        public int Codigo_Mercadoria { get; set; }
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
