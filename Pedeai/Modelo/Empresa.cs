namespace Pedeai.Modelo
{
    public class Empresa
    {
        public int Codigo { get; set; }
        public string empNome { get; set; } = "";
        public string empNome_Fantasia { get; set; } = "";
        public string empCNPJ { get; set; } = "";
        public string empTelefone { get; set; } = "";
        public string empEmail { get; set; } = "";
        public string empEndereco { get; set; } = "";
        public string Info { get; set; } = "";
        /// <summary>Código único da instalação, gerado automaticamente. Usado para gerar a chave de licença.</summary>
        public string empCodigo_Empresa { get; set; } = "";
        /// <summary>Chave de ativação informada pelo contratante. Validada pelo LicencaService.</summary>
        public string empChave_Licenca { get; set; } = "";
        /// <summary>Data de início do período de graça (null = graça ainda não iniciada).</summary>
        public System.DateTime? empData_Graca { get; set; }
        /// <summary>Nível de atualização sincronizado do Supabase. 1=Beta · 2=Standard · 3=Legacy.</summary>
        public int empNivel_Atualizacao { get; set; } = 2;
    }
}
