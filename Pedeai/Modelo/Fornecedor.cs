using System;

namespace Pedeai.Modelo
{
    public class Fornecedor
    {
        public int auxCodigo { get; set; }
        public int Codigo { get; set; }

        public string fornNome_RazaoSocial { get; set; }
        public string fornApelido_Fantasia { get; set; } = "";
        public string fornCPF_CNPJ_ { get; set; } = "";
        public string fornRG_InscricaoEstadual { get; set; } = "";
        public string fornTelefone { get; set; } = "";
        public string fornEmail { get; set; } = "";
        public string fornContato { get; set; } = "";
        public string fornCEP { get; set; } = "";
        public string fornEndereco { get; set; } = "";
        public string fornNumero { get; set; } = "";
        public string fornBairro { get; set; } = "";
        public string fornCidade { get; set; } = "";
        public string fornEstado { get; set; } = "";
        public string fornObservacoes { get; set; } = "";
        public DateTime fornData_Cadastro { get; set; }

        public string Situacao { get; set; } = "A";
        public string Status_Transmissao { get; set; } = "N";
        public string Info { get; set; } = "";
    }
}
