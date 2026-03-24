using System;

namespace Pedeai.Modelo
{
    public class Cupom
    {
        public int auxCodigo { get; set; }
        public int Codigo { get; set; }

        public string cupomCodigo { get; set; }
        public string cupomDescricao { get; set; } = "";
        public string cupomTipo { get; set; } = "PERCENTUAL"; // PERCENTUAL ou VALOR
        public decimal cupomValor { get; set; }
        public decimal cupomPedido_Minimo { get; set; }
        public int cupomLimite_Usos { get; set; }
        public int cupomUsos_Realizados { get; set; }
        public DateTime cupomValido_Ate { get; set; }
        public DateTime cupomData_Cadastro { get; set; }

        public string Situacao { get; set; } = "A";
        public string Status_Transmissao { get; set; } = "N";
        public string Info { get; set; } = "";
    }
}
