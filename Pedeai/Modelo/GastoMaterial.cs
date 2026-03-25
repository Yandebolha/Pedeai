using System;

namespace Pedeai.Modelo
{
    public class GastoMaterial
    {
        public int Codigo { get; set; }
        public int auxCodigo { get; set; }
        public DateTime gmatData { get; set; } = DateTime.Today;
        public string gmatDescricao { get; set; } = "";
        public decimal gmatValor { get; set; }
        public string gmatObservacoes { get; set; } = "";
        public string Situacao { get; set; } = "A";
        public string Info { get; set; } = "";
    }
}
