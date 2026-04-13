namespace PedeaiLicencaServer.Models
{
    public class ValidarRequest
    {
        public string CodigoEmpresa { get; set; }
        public string Chave         { get; set; }
    }

    public class RenovarRequest
    {
        /// <summary>Código único da instalação (empCodigo_Empresa da tabela empresa).</summary>
        public string CodigoEmpresa { get; set; }

        /// <summary>Validade em dias a partir de hoje. Padrão: 30 dias.</summary>
        public int DiasValidade { get; set; } = 30;
    }

    public class RenovarResponse
    {
        public string CodigoEmpresa { get; set; }
        public string Chave          { get; set; }
        public string Expiracao      { get; set; }  // "yyyy-MM-dd"
    }
}
