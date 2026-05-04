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
        public bool mercHabilitar_Site   { get; set; }
        public DateTime mercData_Cadastro { get; set; }

        // Precificação específica quando usado como Adicional (não altera preço de estoque)
        public decimal mercPreco_Adicional   { get; set; }
        // Quantidade máxima que pode ser adicionada por pedido (0 = ilimitado)
        public int     mercAdicional_Qtd_Max { get; set; } = 1;

        // Produto fracionado (múltiplos sabores, ex: pizza)
        public bool mercFracionado    { get; set; }
        public int  mercQtd_Sabores   { get; set; } = 1;

        // Sabores manuais: qtd máxima quando o produto tem seleção manual de sabores
        public int  mercQtd_Sabores_Manual { get; set; } = 0;

        public string Situacao { get; set; } = "A";
        public string Status_Transmissao { get; set; } = "N";
        public string Info { get; set; } = "";
    }
}
