using System;

namespace Pedeai.Modelo
{
    public class PedidoWeb
    {
        public int auxCodigo { get; set; }
        public int Codigo { get; set; }

        public string pediNumero { get; set; }
        public string pediNome_Cliente { get; set; }
        public string pediTelefone_Cliente { get; set; }
        public int pediSituacao { get; set; }
        public string pediForma_Pagamento { get; set; }
        public string pediTipo_Entrega { get; set; }
        public decimal pediValor_Total { get; set; }
        public string pediOrigem { get; set; }
        public DateTime pediData_Lancamento { get; set; }
        public DateTime? pediData_Atualizacao { get; set; }

        // Campos padrão ConstruFarma
        public string Situacao { get; set; } = "A";
        public string Status_Transmissao { get; set; } = "N";
        public string Info { get; set; } = "";
    }
}
