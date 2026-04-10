namespace Pedeai.Modelo
{
    public class ConfigFidelizacao
    {
        public int     Codigo            { get; set; }
        public string  fidNome           { get; set; } = "Regra Padrão";
        public bool    fidAtivo          { get; set; } = false;
        public decimal fidMeta_Gasto     { get; set; } = 500m;   // R$ acumulado para ganhar prêmio
        public string  fidPremio_Tipo    { get; set; } = "CUPOM"; // CUPOM | PRODUTO
        // Cupom settings
        public string  fidCupom_Tipo     { get; set; } = "PERCENTUAL"; // PERCENTUAL | FIXO
        public decimal fidCupom_Valor    { get; set; } = 10m;   // % ou R$
        public decimal fidCupom_Minimo   { get; set; } = 0m;    // pedido mínimo para usar o cupom
        public int     fidCupom_Validade { get; set; } = 30;    // dias de validade
        // Produto settings
        public int     fidProduto_Codigo { get; set; } = 0;
        public string  fidProduto_Nome   { get; set; } = "";
        public int     fidProduto_Qtde   { get; set; } = 1;
        // WhatsApp message template
        // Suporta: {Nome} {Meta} {CupomCodigo} {Validade} {Produto} {TotalGasto}
        public string  fidMensagem       { get; set; } =
            "Olá, {Nome}! 🎉\nVocê atingiu R$ {Meta} em pedidos e ganhou um prêmio!\n" +
            "Use o cupom *{CupomCodigo}* no seu próximo pedido.\n" +
            "Válido até {Validade}. Obrigado pela fidelidade!";
        public string  Info              { get; set; } = "";
    }
}
