using System;

namespace Pedeai.Modelo
{
    public class GrupoMercadoria
    {
        public int auxCodigo { get; set; }
        public int Codigo { get; set; }

        public string grmeDescricao_ { get; set; }
        public int grmeOrdem { get; set; }
        public bool grmeHabilitar_Site { get; set; } = false;
        public string grmeImagem_Url { get; set; } = "";
        public DateTime grmeData_Cadastro { get; set; }

        public string Situacao { get; set; } = "A";
        public string Status_Transmissao { get; set; } = "N";
        public string Info { get; set; } = "";
    }
}
