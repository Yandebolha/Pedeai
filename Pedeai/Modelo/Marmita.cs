namespace Pedeai.Modelo
{
    public class Marmita
    {
        public int     Codigo              { get; set; }
        public int     auxCodigo           { get; set; }
        public string  marDescricao        { get; set; } = "";
        public decimal marValor            { get; set; }
        public decimal marCusto            { get; set; }
        public bool    marHabilitar_Site   { get; set; }
        public bool    marDestaque         { get; set; }
        public string  marImagem_Url       { get; set; } = "";
        public string  supabase_uuid       { get; set; } = "";
        public char    Situacao            { get; set; } = 'A';
    }

    public class MarmitaItem
    {
        public int     Codigo              { get; set; }
        public int     auxCodigo           { get; set; }
        public int     Codigo_Marmita      { get; set; }
        public int     maritmCodigo_Merc   { get; set; }
        public string  maritmNome          { get; set; } = "";
        public decimal maritmQtde          { get; set; } = 1;
    }
}
