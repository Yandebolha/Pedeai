using System;

namespace Pedeai.Modelo
{
    public class Mercadoria
    {
        public int auxCodigo { get; set; }
        public int Codigo { get; set; }
        public int Codigo_Grupo { get; set; }

        public string mercMercadoria { get; set; }
        public string mercApresentacao { get; set; } = "";
        public decimal mercPreco_Venda { get; set; }
        public decimal mercPreco_Custo { get; set; }
        public decimal mercPreco_Promocional { get; set; }
        public decimal mercEstoque_Atual { get; set; }
        public bool mercControla_Estoque { get; set; }
        public string mercImagem_Url { get; set; } = "";
        public bool mercDestaque { get; set; }
        public int mercOrdem { get; set; }
        public bool mercHabilitar_Ifood { get; set; }
        public DateTime mercData_Cadastro { get; set; }

        public string Situacao { get; set; } = "A";
        public string Status_Transmissao { get; set; } = "N";
        public string Info { get; set; } = "";
    }
}
