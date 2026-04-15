using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PedeaiUpdateService
{
    /// <summary>
    /// Núcleo do serviço de atualização automática.
    /// Fala diretamente com Supabase REST API + Storage — sem VPS intermediária.
    /// </summary>
    public class Updater
    {
        private readonly IConfiguration _cfg;
        private readonly HttpClient     _http;
        private readonly Microsoft.Extensions.Logging.ILogger _log;

        private string RestBase    => SupabaseUrl + "/rest/v1/";
        private string StorageBase => SupabaseUrl + "/storage/v1/";
        private string SupabaseUrl => (_cfg["SupabaseUrl"] ?? "").TrimEnd('/');
        private const  string BUCKET = "pacotes";

        public Updater(IConfiguration cfg, Microsoft.Extensions.Logging.ILogger log)
        {
            _cfg = cfg;
            _log = log;
            _http = new HttpClient { Timeout = TimeSpan.FromMinutes(30) };
            string key = cfg["SupabaseKey"] ?? "";
            _http.DefaultRequestHeaders.Add("apikey", key);
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", key);
        }

        // ── Registro do cliente ───────────────────────────────────────────────────

        public async Task<long> RegistrarAsync(CancellationToken ct)
        {
            long id = _cfg.GetValue<long>("ClienteId");
            if (id > 0) return id;

            string cod  = ObterCodigoEmpresaLocal() ?? "DESCONHECIDO";
            string nome = ObterNomeEmpresaLocal()   ?? "";

            // Verifica se já existe pelo CodigoEmpresa
            string getUrl  = RestBase + "Clientes?CodigoEmpresa=eq." +
                             Uri.EscapeDataString(cod.Trim().ToUpperInvariant()) + "&select=Id";
            using var getReq = new HttpRequestMessage(HttpMethod.Get, getUrl);
            var getResp = await _http.SendAsync(getReq, ct);
            string getBody = await getResp.Content.ReadAsStringAsync(ct);
            if (getResp.IsSuccessStatusCode)
            {
                var arr = JsonConvert.DeserializeObject<ClienteIdRow[]>(getBody);
                if (arr != null && arr.Length > 0)
                {
                    SalvarClienteId(arr[0].Id);
                    _log.LogInformation("Cliente já registrado. ClienteId = {Id}", arr[0].Id);
                    return arr[0].Id;
                }
            }

            // Cria novo cliente
            var payload = JsonConvert.SerializeObject(new
            {
                CodigoEmpresa = cod.Trim().ToUpperInvariant(),
                NomeEmpresa   = nome,
                Nivel         = 2,
                Bloqueado     = false,
                VersaoAtual   = _cfg["VersaoAtual"] ?? "1.0.0",
                DataRegistro  = DateTime.UtcNow
            });
            using var postReq = new HttpRequestMessage(HttpMethod.Post, RestBase + "Clientes");
            postReq.Headers.Add("Prefer", "return=representation");
            postReq.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            var postResp = await _http.SendAsync(postReq, ct);
            string postBody = await postResp.Content.ReadAsStringAsync(ct);
            postResp.EnsureSuccessStatusCode();

            var criados = JsonConvert.DeserializeObject<ClienteIdRow[]>(postBody);
            id = criados != null && criados.Length > 0 ? criados[0].Id : 0;
            SalvarClienteId(id);
            _log.LogInformation("Registrado no Supabase. ClienteId = {Id}", id);
            return id;
        }

        // ── Verificação de atualização ────────────────────────────────────────────

        public async Task<(bool temUpdate, long pacoteId, string versao, string caminhoArquivo)>
            VerificarAsync(long clienteId, string versaoAtual, CancellationToken ct)
        {
            // 1. Tenta buscar nível/bloqueio no Supabase
            int  nivelLocal = ObterNivelLocal();   // fallback se Supabase offline
            int  nivel      = nivelLocal;
            bool bloqueado  = false;

            try
            {
                string clienteUrl = RestBase + "Clientes?Id=eq." + clienteId +
                                    "&select=Nivel,Bloqueado";
                using var cliReq  = new HttpRequestMessage(HttpMethod.Get, clienteUrl);
                var cliResp  = await _http.SendAsync(cliReq, ct);
                string cliBody = await cliResp.Content.ReadAsStringAsync(ct);

                if (cliResp.IsSuccessStatusCode)
                {
                    var clientes = JsonConvert.DeserializeObject<ClienteNivelRow[]>(cliBody);
                    if (clientes != null && clientes.Length > 0)
                    {
                        nivel     = clientes[0].Nivel;
                        bloqueado = clientes[0].Bloqueado;

                        // Sincroniza nível para o banco local (se mudou)
                        if (nivel != nivelLocal)
                        {
                            SalvarNivelLocal(nivel);
                            _log.LogInformation("Nível de atualização sincronizado: {N}", nivel);
                        }

                        // Atualiza UltimaConsulta no Supabase (fire-and-forget)
                        _ = PatchAsync("Clientes", "Id=eq." + clienteId,
                                       new { UltimaConsulta = DateTime.UtcNow }, ct);
                    }
                }
                else
                {
                    _log.LogWarning("Supabase offline — usando nível local ({N}).", nivelLocal);
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning("Erro ao consultar Supabase: {Msg}. Usando nível local ({N}).",
                    ex.Message, nivelLocal);
            }

            if (bloqueado)
            {
                _log.LogWarning("Cliente bloqueado no Supabase. Atualização suspensa.");
                return (false, 0, null, null);
            }

            // 2. Busca pacote disponível para o nível
            string versaoEnc = Uri.EscapeDataString(versaoAtual ?? "");
            string pkgUrl = RestBase + "Pacotes?Ativo=eq.true&Nivel=lte." + nivel +
                            "&Versao=gt." + versaoEnc +
                            "&order=Versao.desc&limit=1";
            try
            {
                using var pkgReq  = new HttpRequestMessage(HttpMethod.Get, pkgUrl);
                var pkgResp  = await _http.SendAsync(pkgReq, ct);
                string pkgBody = await pkgResp.Content.ReadAsStringAsync(ct);
                if (!pkgResp.IsSuccessStatusCode) return (false, 0, null, null);

                var pacotes = JsonConvert.DeserializeObject<PacoteRow[]>(pkgBody);
                if (pacotes == null || pacotes.Length == 0) return (false, 0, null, null);
                var p = pacotes[0];
                return (true, p.Id, p.Versao, p.CaminhoArquivo);
            }
            catch { return (false, 0, null, null); }
        }

        // ── Download do pacote ────────────────────────────────────────────────────

        public async Task<string> BaixarAsync(long pacoteId, string caminhoArquivo, CancellationToken ct)
        {
            string url  = StorageBase + "object/" + BUCKET + "/" +
                          EncodeStoragePath(caminhoArquivo);
            string dest = Path.Combine(Path.GetTempPath(), $"pedeai_update_{pacoteId}.zip");

            using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();
            using var fs = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None);
            await resp.Content.CopyToAsync(fs, ct);

            // Registra o download na tabela AplicacoesUpdate
            await PostAsync("AplicacoesUpdate", new
            {
                ClienteId    = 0L,   // será preenchido pelo chamador se necessário
                PacoteId     = pacoteId,
                DataDownload = DateTime.UtcNow,
                Status       = "baixado"
            }, ct);

            return dest;
        }

        // ── Download com clienteId ────────────────────────────────────────────────

        public async Task<string> BaixarAsync(long clienteId, long pacoteId, string caminhoArquivo, CancellationToken ct)
        {
            string url  = StorageBase + "object/" + BUCKET + "/" +
                          EncodeStoragePath(caminhoArquivo);
            string dest = Path.Combine(Path.GetTempPath(), $"pedeai_update_{pacoteId}.zip");

            using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();
            using var fs = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None);
            await resp.Content.CopyToAsync(fs, ct);

            await PostAsync("AplicacoesUpdate", new
            {
                ClienteId    = clienteId,
                PacoteId     = pacoteId,
                DataDownload = DateTime.UtcNow,
                Status       = "baixado"
            }, ct);

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

                // 3. Copia arquivos (pasta files/ → AppDir; se não houver files/, usa raiz do staging)
                string filesDir = Path.Combine(stagingDir, "files");
                string appDir   = _cfg["AppDir"] ?? AppDomain.CurrentDomain.BaseDirectory;

                // Se não há subpasta 'files/', trata a raiz do staging como origem.
                // Caso especial: se a raiz do staging tiver apenas UMA pasta (e nenhum arquivo),
                // significa que o ZIP foi criado comprimindo a pasta inteira — desempacota um nível.
                string origemCopia;
                if (Directory.Exists(filesDir))
                {
                    origemCopia = filesDir;
                }
                else
                {
                    var rootFolders = Directory.GetDirectories(stagingDir);
                    var rootFiles   = Directory.GetFiles(stagingDir);
                    if (rootFolders.Length == 1 && rootFiles.Length == 0)
                        origemCopia = rootFolders[0];   // desempacota o wrapper
                    else
                        origemCopia = stagingDir;
                }

                // Para subdiretórios de primeiro nível presentes na origem,
                // apaga o correspondente no AppDir antes de copiar — substituição completa
                foreach (string subDir in Directory.GetDirectories(origemCopia))
                {
                    string nomePasta  = Path.GetFileName(subDir);
                    string destinoDir = Path.Combine(appDir, nomePasta);
                    if (Directory.Exists(destinoDir))
                        try { Directory.Delete(destinoDir, true); } catch { }
                }

                // Copia (excluindo update.sql quando a origem é o staging root)
                string[] excluir = origemCopia == stagingDir ? new[] { "update.sql" } : Array.Empty<string>();
                CopiarDiretorio(origemCopia, appDir, excluir);

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
                await IniciarAplicacao();
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
            string status = sucesso ? "aplicado" : "erro";

            // Busca registro de download mais recente para este cliente+pacote
            string getUrl = RestBase + "AplicacoesUpdate?ClienteId=eq." + clienteId +
                            "&PacoteId=eq." + pacoteId + "&order=Id.desc&limit=1";
            using var req = new HttpRequestMessage(HttpMethod.Get, getUrl);
            var resp = await _http.SendAsync(req, ct);
            string body = await resp.Content.ReadAsStringAsync(ct);

            if (resp.IsSuccessStatusCode)
            {
                var rows = JsonConvert.DeserializeObject<AplicacaoIdRow[]>(body);
                if (rows != null && rows.Length > 0)
                {
                    await PatchAsync("AplicacoesUpdate", "Id=eq." + rows[0].Id, new
                    {
                        DataAplicada = DateTime.UtcNow,
                        Status       = status,
                        Detalhe      = detalhe ?? ""
                    }, ct);
                }
            }

            // Atualiza versão do cliente no Supabase
            if (sucesso)
            {
                string versao = _cfg["VersaoAtual"] ?? "";
                await PatchAsync("Clientes", "Id=eq." + clienteId,
                                 new { VersaoAtual = versao }, ct);
            }
        }

        // ── Utilitários ───────────────────────────────────────────────────────────

        private void PararAplicacao()
        {
            string nome = _cfg["AppProcessName"] ?? "RanGoFood";
            foreach (Process p in Process.GetProcessesByName(nome))
                try { p.Kill(); p.WaitForExit(5000); } catch { }
            Thread.Sleep(1000);
        }

        private async Task IniciarAplicacao()
        {
            await Task.Delay(2000);
            string appDir = _cfg["AppDir"] ?? AppDomain.CurrentDomain.BaseDirectory;
            string appExe = Path.Combine(appDir, _cfg["AppExe"] ?? "RanGoFood.exe");
            if (File.Exists(appExe))
                Process.Start(new ProcessStartInfo(appExe) { UseShellExecute = true });
        }

        private static void CopiarDiretorio(string origem, string destino, string[] excluir = null)
        {
            Directory.CreateDirectory(destino);
            foreach (string arq in Directory.GetFiles(origem, "*", SearchOption.AllDirectories))
            {
                string relativo = arq.Substring(origem.Length).TrimStart(Path.DirectorySeparatorChar);
                // Ignora arquivos da lista de excluídos (caminho relativo ou apenas nome na raiz)
                if (excluir != null && Array.Exists(excluir, ex =>
                    string.Equals(relativo, ex, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(Path.GetFileName(arq), ex, StringComparison.OrdinalIgnoreCase) &&
                    !relativo.Contains(Path.DirectorySeparatorChar)))
                    continue;
                string destArq  = Path.Combine(destino, relativo);
                Directory.CreateDirectory(Path.GetDirectoryName(destArq));
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
                using var cmd = new MySqlCommand(
                    "SELECT empCodigo_Empresa FROM empresa WHERE Codigo=1 LIMIT 1", conn);
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
                using var cmd = new MySqlCommand(
                    "SELECT COALESCE(empNome_Fantasia,empNome) FROM empresa WHERE Codigo=1 LIMIT 1", conn);
                return cmd.ExecuteScalar()?.ToString();
            }
            catch { return null; }
        }

        /// <summary>Lê o nível de atualização salvo localmente. Retorna 2 (Standard) se não disponível.</summary>
        private int ObterNivelLocal()
        {
            try
            {
                using var conn = new MySqlConnection(_cfg["ConnectionString"]);
                conn.Open();
                using var cmd = new MySqlCommand(
                    "SELECT empNivel_Atualizacao FROM empresa WHERE Codigo=1 LIMIT 1", conn);
                var v = cmd.ExecuteScalar();
                if (v == null || v == DBNull.Value) return 2;
                return Convert.ToInt32(v);
            }
            catch { return 2; }
        }

        /// <summary>Persiste o nível de atualização recebido do Supabase no banco local.</summary>
        private void SalvarNivelLocal(int nivel)
        {
            try
            {
                using var conn = new MySqlConnection(_cfg["ConnectionString"]);
                conn.Open();
                using var cmd = new MySqlCommand(
                    "UPDATE empresa SET empNivel_Atualizacao=@n WHERE Codigo=1", conn);
                cmd.Parameters.AddWithValue("@n", nivel);
                cmd.ExecuteNonQuery();
            }
            catch { /* não crítico */ }
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
            catch { }
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
            catch { }

            // Persiste também na tabela local para exibição no sistema
            try
            {
                string cs = _cfg["ConnectionString"] ?? "";
                if (string.IsNullOrWhiteSpace(cs)) return;
                using var conn = new MySqlConnection(cs);
                conn.Open();
                using var cmd = new MySqlCommand(
                    "UPDATE empresa SET empVersao_Atual=@v WHERE Codigo=1", conn);
                cmd.Parameters.AddWithValue("@v", versao ?? "");
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        // ── Helpers REST ──────────────────────────────────────────────────────────

        /// <summary>Codifica cada segmento do caminho separadamente, preservando as barras '/'.</summary>
        private static string EncodeStoragePath(string path)
            => string.Join("/", (path ?? "").Split('/').Select(Uri.EscapeDataString));

        private async Task PostAsync(string table, object body, CancellationToken ct)
        {
            var json = JsonConvert.SerializeObject(body);
            var req  = new HttpRequestMessage(HttpMethod.Post, RestBase + table);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            await _http.SendAsync(req, ct);
        }

        private async Task PatchAsync(string table, string filter, object body, CancellationToken ct)
        {
            var json = JsonConvert.SerializeObject(body);
            var req  = new HttpRequestMessage(new HttpMethod("PATCH"),
                RestBase + table + "?" + filter);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            await _http.SendAsync(req, ct);
        }

        // ── DTOs ──────────────────────────────────────────────────────────────────

        private class ClienteIdRow   { public long Id      { get; set; } }
        private class AplicacaoIdRow { public long Id      { get; set; } }
        private class ClienteNivelRow
        {
            public int  Nivel     { get; set; }
            public bool Bloqueado { get; set; }
        }
        private class PacoteRow
        {
            public long   Id             { get; set; }
            public string Versao         { get; set; }
            public string CaminhoArquivo { get; set; }
        }
    }
}
