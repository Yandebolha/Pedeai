using System;

namespace Pedeai.Modelo
{
    public class Usuario
    {
        public int Codigo { get; set; }
        public int auxCodigo { get; set; }
        public string usuNome { get; set; } = "";
        public string usuLogin { get; set; } = "";
        public string usuSenha { get; set; } = ""; // SHA-256 hash — nunca armazenar texto puro
        public int usuNivel { get; set; } = 1;     // 1=Operador  2=Gerente  9=Admin
        public string Situacao { get; set; } = "A";
        public DateTime usuData_Cadastro { get; set; } = DateTime.Now;
        public string Info { get; set; } = "";
    }
}
