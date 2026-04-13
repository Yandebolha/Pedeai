using System.Collections.Generic;

namespace PedeaiBackup.Models
{
    /// <summary>
    /// Configurações persistidas em %APPDATA%\PedeaiBackup\config.json
    /// </summary>
    public class BackupConfig
    {
        // ── Banco de dados MySQL ───────────────────────────────────────────
        public string DbHost        { get; set; } = "localhost";
        public string DbPorta       { get; set; } = "3306";
        public string DbNome        { get; set; } = "pedeai";
        public string DbUsuario     { get; set; } = "root";
        public string DbSenha       { get; set; } = "";
        /// <summary>Caminho completo para mysqldump.exe ou só "mysqldump" se estiver no PATH.</summary>
        public string MySqlDumpPath { get; set; } = "mysqldump";

        // ── MEGAcmd ────────────────────────────────────────────────────────
        public string MegaEmail   { get; set; } = "";
        public string MegaSenha   { get; set; } = "";
        public string MegaPasta   { get; set; } = "/PedeaiBackups";
        /// <summary>Caminho para o executável do MEGAcmd (mega-cmd.bat ou mega-put), ou "mega-put" se no PATH.</summary>
        public string MegaCmdPath { get; set; } = "mega-cmd";

        // ── Agendamento ────────────────────────────────────────────────────
        /// <summary>Dias da semana ativos (0=Dom, 1=Seg, …, 6=Sab).</summary>
        public List<int> DiasAtivos     { get; set; } = new List<int>();
        /// <summary>Horários no formato "HH:mm" (24h).</summary>
        public List<string> Horarios    { get; set; } = new List<string>();

        // ── Retenção local ─────────────────────────────────────────────────
        /// <summary>Número de dias para manter cópias locais antes de excluir.</summary>
        public int DiasRetencaoLocal    { get; set; } = 7;
    }
}
