using System;

namespace Pedeai.Modelo
{
    public class EntradaMercadoria
    {
        public int      Codigo              { get; set; }
        public int      auxCodigo           { get; set; }
        public int      Codigo_Fornecedor   { get; set; }
        public string   entNome_Fornecedor  { get; set; } = "";
        public DateTime entData             { get; set; } = DateTime.Today;
        public string   entNumeroDoc        { get; set; } = "";
        public string   entObservacoes      { get; set; } = "";
        public decimal  entValorTotal       { get; set; }
        public string   Situacao            { get; set; } = "A";
        public string   Info                { get; set; } = "";
        public DateTime entData_Lancamento  { get; set; } = DateTime.Now;
    }
}
