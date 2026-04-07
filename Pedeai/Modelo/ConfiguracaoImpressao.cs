namespace Pedeai.Modelo
{
    /// <summary>
    /// Configuracoes editaveis do cupom de impressao do pedido.
    /// Todos os campos de texto sao exibidos no comprovante e podem ser personalizados.
    /// </summary>
    public class ConfiguracaoImpressao
    {
        // Cabecalho
        public string cabNomeEmpresa    { get; set; } = "";
        public string cabEndereco       { get; set; } = "";
        public string cabTelefone       { get; set; } = "";
        public string cabCNPJ           { get; set; } = "";

        // Linha separadora (char repetido)
        public string separador         { get; set; } = "-";

        // Aviso fiscal
        public string rodapeAvisoFiscal { get; set; } = "*** NAO E DOCUMENTO FISCAL ***";

        // Rodape livre (pode ter varias linhas separadas por \n)
        public string rodapeTextoLivre  { get; set; } = "Obrigado pela preferencia!";

        // Etiqueta do numero do pedido
        public string lblNumeroPedido   { get; set; } = "Pedido N.:";

        // Etiqueta da coluna de item
        public string lblColunaItem     { get; set; } = "ITEM (V.Unit)";

        // Etiqueta da coluna de total
        public string lblColunaTotal    { get; set; } = "Total";

        // Etiqueta subtotal
        public string lblSubtotal       { get; set; } = "TOTAL:";

        // Etiqueta taxa de entrega
        public string lblTaxaEntrega    { get; set; } = "+ ENTREGA:";

        // Etiqueta desconto
        public string lblDesconto       { get; set; } = "- DESCONTO:";

        // Etiqueta cupom
        public string lblCupom          { get; set; } = "- CUPOM:";

        // Etiqueta total a pagar
        public string lblTotalPagar     { get; set; } = "= TOTAL A PAGAR:";

        // Etiqueta do atendente
        public string lblAtendente      { get; set; } = "Atendente:";

        // Largura da impressao em caracteres (para impressoras termicas 80mm ~ 42 chars)
        public int    larguraCaracteres { get; set; } = 42;

        // Nome da impressora (em branco = impressora padrao)
        public string impressoraNome    { get; set; } = "";
    }
}
