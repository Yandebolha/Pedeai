using System;

namespace Pedeai.Modelo
{
    public class ParcelaEntradaMercadoria
    {
        public int     Codigo           { get; set; }
        public int     auxCodigo        { get; set; }
        public int     Codigo_Entrada   { get; set; }
        public int     parNumero        { get; set; }   // 1, 2, 3…
        public DateTime parVencimento   { get; set; }
        public decimal parValor         { get; set; }
        public string  parObservacao    { get; set; } = "";

        // A=aberta, P=paga, C=cancelada
        public string  Situacao         { get; set; } = "A";
        public DateTime? parData_Pagamento { get; set; }
    }
}
