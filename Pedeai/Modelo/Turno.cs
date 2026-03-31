using System;

namespace Pedeai.Modelo
{
    public class Turno
    {
        public int      Codigo         { get; set; }
        public int      auxCodigo      { get; set; } = 1;
        public DateTime turAbertura    { get; set; }
        public DateTime? turFechamento { get; set; }
        public string   turUsuario     { get; set; }
        public decimal  turCaixa_Inicial  { get; set; }
        public decimal? turCaixa_Final    { get; set; }
        public string   turObservacao  { get; set; }
        /// <summary>A=Aberto  F=Fechado</summary>
        public char     turSituacao    { get; set; } = 'A';
    }
}
