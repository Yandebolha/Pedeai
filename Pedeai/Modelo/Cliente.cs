using System;

namespace Pedeai.Modelo
{
    public class Cliente
    {
        public int auxCodigo { get; set; }
        public int Codigo { get; set; }

        public string clieNome_RazaoSocial { get; set; }
        public string clieTelefone { get; set; } = "";
        public string clieCelular { get; set; } = "";
        public string clieEmail { get; set; } = "";
        public string clieCPF_CNPJ_ { get; set; } = "";
        public string clieCEP { get; set; } = "";
        public string clieEndereco { get; set; } = "";
        public string clieNumero { get; set; } = "";
        public string clieComplemento { get; set; } = "";
        public string clieBairro { get; set; } = "";
        public string clieCidade { get; set; } = "";
        public string clieEstado { get; set; } = "";
        public int clieTotalPedidos { get; set; }
        public decimal clieTotalGasto { get; set; }
        public decimal clieGasto_Mensal { get; set; }
        public string clieGasto_Mes_Ref { get; set; } = "";
        public DateTime clieData_Cadastro { get; set; }

        public string Situacao { get; set; } = "A";
        public string Status_Transmissao { get; set; } = "N";
        public string Info { get; set; } = "";
    }
}
