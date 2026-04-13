using System.Collections.Generic;

namespace PedeaiUpdateServer.Models
{
    // ── Pacote de atualização ──────────────────────────────────────────────────
    public class Pacote
    {
        public long   Id              { get; set; }
        public string Versao          { get; set; }
        public int    Nivel           { get; set; } = 2;
        public string Descricao       { get; set; }
        public string CaminhoArquivo  { get; set; }
        public long   TamanhoBytes    { get; set; }
        public bool   TemSQL          { get; set; }
        public string DataPublicacao  { get; set; }
        public bool   Ativo           { get; set; } = true;
    }

    // ── Cliente (instalação) ──────────────────────────────────────────────────
    public class Cliente
    {
        public long   Id              { get; set; }
        public string CodigoEmpresa   { get; set; }
        public string NomeEmpresa     { get; set; }
        /// <summary>1 = Beta | 2 = Standard | 3 = Legacy</summary>
        public int    Nivel           { get; set; } = 2;
        public string VersaoAtual     { get; set; }
        public bool   Bloqueado       { get; set; } = false;
        public string DataRegistro    { get; set; }
        public string UltimaConsulta  { get; set; }
    }

    // ── Registro de aplicação de update ───────────────────────────────────────
    public class AplicacaoUpdate
    {
        public long   Id            { get; set; }
        public long   ClienteId     { get; set; }
        public long   PacoteId      { get; set; }
        public string DataDownload  { get; set; }
        public string DataAplicada  { get; set; }
        public string Status        { get; set; } // "baixado" | "aplicado" | "erro"
        public string Detalhe       { get; set; }
    }

    // ── Requests/Responses ────────────────────────────────────────────────────
    public class RegistrarClienteRequest
    {
        public string CodigoEmpresa { get; set; }
        public string NomeEmpresa   { get; set; }
    }

    public class RegistrarClienteResponse
    {
        public long   ClienteId     { get; set; }
        public string Mensagem      { get; set; }
    }

    public class VerificarUpdateResponse
    {
        public bool   TemAtualizacao { get; set; }
        public long?  PacoteId       { get; set; }
        public string Versao         { get; set; }
        public string Descricao      { get; set; }
        public bool   TemSQL         { get; set; }
        public long   TamanhoBytes   { get; set; }
    }

    public class ConfirmarUpdateRequest
    {
        public string Status  { get; set; } // "aplicado" | "erro"
        public string Detalhe { get; set; }
    }

    public class AlterarNivelRequest
    {
        public int Nivel { get; set; }
    }

    public class PublicarResponse
    {
        public long   PacoteId { get; set; }
        public string Versao   { get; set; }
        public string Mensagem { get; set; }
    }
}
