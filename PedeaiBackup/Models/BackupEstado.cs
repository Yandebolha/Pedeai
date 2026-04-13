using System;
using System.Collections.Generic;

namespace PedeaiBackup.Models
{
    /// <summary>
    /// Estado persistido em %APPDATA%\PedeaiBackup\estado.json.
    /// Controla qual foi o último backup e o máximo de PK de cada tabela,
    /// permitindo gerar backups incrementais (somente novas linhas).
    /// </summary>
    public class BackupEstado
    {
        /// <summary>True após o primeiro backup completo ser concluído.</summary>
        public bool BackupCompletoRealizado    { get; set; } = false;

        public DateTime? UltimoBackupCompleto  { get; set; }
        public DateTime? UltimoBackupIncremental { get; set; }

        /// <summary>
        /// Máximo de Codigo (PK) de cada tabela de transação no momento
        /// do último backup. Usado para filtrar apenas novos registros.
        /// Chave = nome da tabela; Valor = MAX(Codigo).
        /// </summary>
        public Dictionary<string, long> MaxCodigo { get; set; } = new Dictionary<string, long>();
    }

    public class BackupResultado
    {
        public bool   Sucesso     { get; set; }
        public string Arquivo     { get; set; }
        public string Tipo        { get; set; } // "completo" | "incremental"
        public string Erro        { get; set; }
        public long   TamanhoBytes { get; set; }
        public DateTime Timestamp  { get; set; } = DateTime.Now;
    }
}
