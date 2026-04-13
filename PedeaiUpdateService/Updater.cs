using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PedeaiUpdateService
{
    /// <summary>
    /// Núcleo do serviço de atualização automática.
    /// Responsável por: registro, verificação, download, aplicação e confirmação.
    /// </summary>
    public class Updater
    {
        private readonly IConfiguration _cfg;
        private readonly HttpClient     _http;
        private readonly Microsoft.Extensions.Logging.ILogger _log;

        public Updater(IConfiguration cfg, Microsoft.Extensions.Logging.ILogger log)
        {
            _cfg = cfg;
            _log = log;
            _http = new HttpClient { Timeout = TimeSpan.FromMinutes(30) };
            _http.DefaultRequestHeaders.Add("X-Api-Key", cfg["UpdateApiKey"] ?? "");
        }

        // ── Registro do cliente ───────────────────────────────────────────────────

        public async Task<long> RegistrarAsync(CancellationToken ct)
        {
            long id = _cfg.GetValue<long>("ClienteId");
            if (id > 0) return id;

            // Obtém o CodigoEmpresa do banco de dados local
            string codigoEmpresa = ObterCodigoEmpresaLocal();
            string nomeEmpresa   = ObterNomeEmpresaLocal();

            var body = JsonConvert.SerializeObject(new
            {
                codigoEmpresa = codigoEmpresa ?? "DESCONHECIDO",
                nomeEmpresa   = nomeEmpresa ?? ""
            });
            var resp = await _http.PostAsync(
                $"{BaseUrl}/api/update/registrar",
                new StringContent(body, Encoding.UTF8, "application/json"), ct);

            resp.EnsureSuccessStatusCode();
            dynamic result = JsonConvert.DeserializeObject(await resp.Content.ReadAsStringAsync(ct));
            id = (long)result.clienteId;

            // Persiste o ClienteId no appsettings.json
            SalvarClienteId(id);
            _log.LogInformation("Registrado no servidor. ClienteId = {Id}", id);
            return id;
        }

        // ── Verificação de atualização ────────────────────────────────────────────

        public async Task<(bool temUpdate, long pacoteId, string versao)> VerificarAsync(
            long clienteId, string versaoAtual, CancellationToken ct)
        {
            string url = $"{BaseUrl}/api/update/verificar?clienteId={clienteId}&versaoAtual={Uri.EscapeDataString(versaoAtual)}";
            var resp = await _http.GetAsync(url, ct);
            resp.EnsureSuccessStatusCode();
            dynamic r = JsonConvert.DeserializeObject(await resp.Content.ReadAsStringAsync(ct));
            bool tem = (bool)r.temAtualizacao;
            if (!tem) return (false, 0, null);
            return (true, (long)r.pacoteId, (string)r.versao);
        }

        // ── Download do pacote ────────────────────────────────────────────────────

        public async Task<string> BaixarAsync(long clienteId, long pacoteId, CancellationToken ct)
        {
            string url  = $"{BaseUrl}/api/update/download/{pacoteId}?clienteId={clienteId}";
            string dest = Path.Combine(Path.GetTempPath(), $"pedeai_update_{pacoteId}.zip");

            using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();
            using var fs = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None);
            await resp.Content.CopyToAsync(fs, ct);
            return dest;
        }

        // ── Aplicação do pacote ───────────────────────────────────────────────────

        public async Task<(bool ok, string erro)> AplicarAsync(
            string zipPath, string versao, CancellationToken ct)
        {
            string stagingDir = Path.Combine(Path.GetTempPath(), $"pedeai_staging_{versao}");
            try
            {
                // 1. Extrai
                if (Directory.Exists(stagingDir)) Directory.Delete(stagingDir, true);
                ZipFile.ExtractToDirectory(zipPath, stagingDir);

                // 2. Para a aplicação principal
                PararAplicacao();

                // 3. Copia arquivos (pasta files/ → AppDir)
                string filesDir = Path.Combine(stagingDir, "files");
                string appDir   = _cfg["AppDir"] ?? AppDomain.CurrentDomain.BaseDirectory;
                if (Directory.Exists(filesDir))
                    CopiarDiretorio(filesDir, appDir);

                // 4. Executa script SQL, se houver
                string sqlFile = Path.Combine(stagingDir, "update.sql");
                if (File.Exists(sqlFile))
                {
                    string erro = await ExecutarSqlAsync(sqlFile, ct);
                    if (!string.IsNullOrEmpty(erro))
                    {
                        await IniciarAplicacao();
                        return (false, $"Erro ao executar SQL: {erro}");
                    }
                }

                // 5. Reinicia aplicação
                await IniciarAplicacao();

                // 6. Atualiza versão no appsettings
                SalvarVersaoAtual(versao);
                return (true, null);
            }
            catch (Exception ex)
            {
                await IniciarAplicacao(); // tenta reiniciar mesmo em caso de falha
                return (false, ex.Message);
            }
            finally
            {
                if (Directory.Exists(stagingDir)) try { Directory.Delete(stagingDir, true); } catch { }
                if (File.Exists(zipPath))           try { File.Delete(zipPath); }            catch { }
            }
        }

        // ── Confirmação ───────────────────────────────────────────────────────────

        public async Task ConfirmarAsync(long clienteId, long pacoteId, bool sucesso, string detalhe, CancellationToken ct)
        {
            var body = JsonConvert.SerializeObject(new
            {
                status  = sucesso ? "aplicado" : "erro",
                detalhe = detalhe ?? ""
            });
            await _http.PostAsync(
                $"{BaseUrl}/api/update/confirmar/{pacoteId}?clienteId={clienteId}",
                new StringContent(body, Encoding.UTF8, "application/json"), ct);
        }

        // ── Utilitários ───────────────────────────────────────────────────────────

        private string BaseUrl => (_cfg["UpdateVpsUrl"] ?? "http://localhost:5001").TrimEnd('/');

        private void PararAplicacao()
        {
            string nome = _cfg["AppProcessName"] ?? "RanGoFood";
            foreach (Process p in Process.GetProcessesByName(nome))
                try { p.Kill(); p.WaitForExit(5000); } catch { }
            Thread.Sleep(1000);
        }

        private async Task IniciarAplicacao()
        {
            await Task.Delay(2000); // aguarda arquivos serem liberados
            string appDir = _cfg["AppDir"] ?? AppDomain.CurrentDomain.BaseDirectory;
            string appExe = Path.Combine(appDir, _cfg["AppExe"] ?? "RanGoFood.exe");
            if (File.Exists(appExe))
                Process.Start(new ProcessStartInfo(appExe) { UseShellExecute = true });
        }

        private static void CopiarDiretorio(string origem, string destino)
        {
            Directory.CreateDirectory(destino);
            foreach (string arq in Directory.GetFiles(origem, "*", SearchOption.AllDirectories))
            {
                string relativo = arq.Substring(origem.Length).TrimStart(Path.DirectorySeparatorChar);
                string destArq  = Path.Combine(destino, relativo);
                Directory.CreateDirectory(Path.GetDirectoryName(destArq));
                // Tentativas em caso de arquivo em uso
                for (int i = 0; i < 5; i++)
                {
                    try { File.Copy(arq, destArq, true); break; }
                    catch { Thread.Sleep(500); }
                }
            }
        }

        private async Task<string> ExecutarSqlAsync(string sqlFile, CancellationToken ct)
        {
            try
            {
                string connStr = _cfg["ConnectionString"];
                if (string.IsNullOrWhiteSpace(connStr)) return null;

                string sql = await File.ReadAllTextAsync(sqlFile, ct);
                using var conn = new MySqlConnection(connStr);
                await conn.OpenAsync(ct);
                using var cmd = new MySqlCommand(sql, conn);
                await cmd.ExecuteNonQueryAsync(ct);
                return null;
            }
            catch (Exception ex) { return ex.Message; }
        }

        private string ObterCodigoEmpresaLocal()
        {
            try
            {
                using var conn = new MySqlConnection(_cfg["ConnectionString"]);
                conn.Open();
                using var cmd = new MySqlCommand("SELECT empCodigo_Empresa FROM empresa WHERE Codigo=1 LIMIT 1", conn);
                return cmd.ExecuteScalar()?.ToString();
            }
            catch { return null; }
        }

        private string ObterNomeEmpresaLocal()
        {
            try
            {
                using var conn = new MySqlConnection(_cfg["ConnectionString"]);
                conn.Open();
                using var cmd = new MySqlCommand("SELECT COALESCE(empNome_Fantasia,empNome) FROM empresa WHERE Codigo=1 LIMIT 1", conn);
                return cmd.ExecuteScalar()?.ToString();
            }
            catch { return null; }
        }

        private void SalvarClienteId(long id)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (!File.Exists(path)) return;
                dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(path));
                json.ClienteId = id;
                File.WriteAllText(path, JsonConvert.SerializeObject(json, Formatting.Indented));
            }
            catch { /* não crítico */ }
        }

        private void SalvarVersaoAtual(string versao)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (!File.Exists(path)) return;
                dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(path));
                json.VersaoAtual = versao;
                File.WriteAllText(path, JsonConvert.SerializeObject(json, Formatting.Indented));
            }
            catch { /* não crítico */ }
        }
    }
}
