using System;

namespace Pedeai.Modelo
{
    public class PedidoWeb
    {
        public int auxCodigo { get; set; }
        public int Codigo { get; set; }

        public string pediNumero { get; set; }
        public int Codigo_Cliente { get; set; }
        public string pediNome_Cliente { get; set; }
        public string pediTelefone_Cliente { get; set; }
        public int pediSituacao { get; set; }
        public int pediTipo_Entrega { get; set; }      // 0=Retirada 1=Entrega
        public int pediForma_Pagamento { get; set; }   // 0=Dinheiro 1=Cartão 2=Pix
        public int pediOrigem { get; set; }            // 0=Web 1=App 2=Manual
        public decimal pediSubtotal { get; set; }
        public decimal pediTaxa_Entrega { get; set; }
        public decimal pediDesconto { get; set; }
        public decimal pediValor_Total { get; set; }
        public decimal? pediTroco_Para { get; set; }
        public string pediEndereco_Entrega { get; set; } = "";
        public string pediObservacoes { get; set; } = "";
        public decimal? pediValor_Pago { get; set; }
        public string pediCodigo_Transacao { get; set; } = "";
        public DateTime pediData_Lancamento { get; set; }
        public DateTime? pediData_Atualizacao { get; set; }

        // Campos padrão ConstruFarma
        public string Situacao { get; set; } = "A";
        public string Info { get; set; } = "";
    }
}
