namespace Pedeai.Modelo
{
    public class WhatsAppConfig
    {
        public string ApiUrl     { get; set; } = "";
        public string ApiKey     { get; set; } = "";
        public string Instance   { get; set; } = "pedeai";
        public string MsgPreparo { get; set; } = "Olá {Nome}! 🍕 Seu pedido #{Numero} já está sendo preparado. Em breve ficará pronto!";
        public string MsgEntrega { get; set; } = "Olá {Nome}! 🛵 Seu pedido #{Numero} saiu para entrega. Aguarde em breve!";
        public string MsgCupom   { get; set; } = "Parabéns {Nome}! 🎉 Você ganhou um cupom de desconto: *{CupomCodigo}*\nVálido até {Validade}. Use no seu próximo pedido!";
    }
}
