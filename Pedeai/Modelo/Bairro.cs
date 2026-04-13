namespace Pedeai.Modelo
{
    public class Bairro
    {
        public int     Codigo          { get; set; }
        public int     auxCodigo       { get; set; } = 1;
        public string  baiCidade       { get; set; } = "";
        public string  baiNome         { get; set; } = "";
        public decimal baiTaxa_Entrega { get; set; }
        public string  Situacao        { get; set; } = "A";
    }
}
