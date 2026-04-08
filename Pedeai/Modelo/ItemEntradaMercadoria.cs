namespace Pedeai.Modelo
{
    public class ItemEntradaMercadoria
    {
        public int      Codigo              { get; set; }
        public int      auxCodigo           { get; set; }
        public int      Codigo_Entrada      { get; set; }
        public int      Codigo_Mercadoria   { get; set; }
        public string   itmNome_Mercadoria  { get; set; } = "";
        public decimal  itmQtde             { get; set; }
        public decimal  itmFracao           { get; set; } = 1m;
        public string   itmUnid_Entrada     { get; set; } = "";
        public string   itmUnid_Saida       { get; set; } = "";
        public decimal  itmPreco_Custo      { get; set; }
        public decimal  itmSubtotal         { get; set; }
        public bool     itmAtualizar_Custo  { get; set; } = true;
        public string   Situacao            { get; set; } = "A";
    }
}
