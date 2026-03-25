using System;

namespace Pedeai.Modelo
{
    public class NecessidadeEmpresa
    {
        public int auxCodigo { get; set; }
        public int Codigo    { get; set; }

        public string  nempDescricao { get; set; } = "";
        public decimal nempValor     { get; set; }
        public string  nempCategoria { get; set; } = "";
        public DateTime nempData     { get; set; }

        public string Situacao { get; set; } = "A";
        public string Info     { get; set; } = "";
    }
}
