using PedeaiBackup.Models;
using System;
using System.IO;
using System.Text.Json;

namespace PedeaiBackup.Services
{
    /// <summary>
    /// Lê e salva as configurações e o estado de backup em %APPDATA%\PedeaiBackup\.
    /// </summary>
    public static class ConfigManager
    {
        private static readonly JsonSerializerOptions Opts = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = null  // PascalCase
        };

        private static string PastaApp()
        {
            string pasta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PedeaiBackup");
            Directory.CreateDirectory(pasta);
            return pasta;
        }

        private static string ArquivoConfig() => Path.Combine(PastaApp(), "config.json");
        private static string ArquivoEstado() => Path.Combine(PastaApp(), "estado.json");

        // ── Config ────────────────────────────────────────────────────────────────────

        public static BackupConfig CarregarConfig()
        {
            string path = ArquivoConfig();
            if (!File.Exists(path)) return new BackupConfig();
            try
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<BackupConfig>(json, Opts) ?? new BackupConfig();
            }
            catch { return new BackupConfig(); }
        }

        public static void SalvarConfig(BackupConfig config)
        {
            string json = JsonSerializer.Serialize(config, Opts);
            File.WriteAllText(ArquivoConfig(), json);
        }

        // ── Estado ────────────────────────────────────────────────────────────────────

        public static BackupEstado CarregarEstado()
        {
            string path = ArquivoEstado();
            if (!File.Exists(path)) return new BackupEstado();
            try
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<BackupEstado>(json, Opts) ?? new BackupEstado();
            }
            catch { return new BackupEstado(); }
        }

        public static void SalvarEstado(BackupEstado estado)
        {
            string json = JsonSerializer.Serialize(estado, Opts);
            File.WriteAllText(ArquivoEstado(), json);
        }
    }
}
