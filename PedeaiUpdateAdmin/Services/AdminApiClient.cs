using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PedeaiUpdateAdmin.Services
{
    /// <summary>
    /// Cliente Supabase direto — sem VPS intermediária.
    /// Usa Supabase REST (PostgREST) para dados e Supabase Storage para pacotes ZIP.
    /// Configurado em %APPDATA%\PedeaiUpdateAdmin\config.json.
    /// </summary>
    public class AdminApiClient
    {
        private readonly HttpClient _http;         // REST (anon ou service_role)
        private readonly HttpClient _httpStorage;  // Storage — sempre service_role
        private readonly string     _restBase;
        private readonly string     _storageBase;
        private const    string     BUCKET = "pacotes";

        public AdminApiClient(string supabaseUrl, string supabaseKey, string serviceRoleKey = null)
        {
            string url   = supabaseUrl.TrimEnd('/');
            _restBase    = url + "/rest/v1/";
            _storageBase = url + "/storage/v1/";

            // Cliente REST — usa a chave fornecida (anon ou service_role)
            _http = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            _http.DefaultRequestHeaders.Add("apikey", supabaseKey);
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", supabaseKey);

            // Cliente Storage — usa service_role se disponível (bypassa RLS)
            string storageKey = !string.IsNullOrWhiteSpace(serviceRoleKey) ? serviceRoleKey : supabaseKey;
            _httpStorage = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            _httpStorage.DefaultRequestHeaders.Add("apikey", storageKey);
            _httpStorage.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", storageKey);
        }

        // ── Pacotes ───────────────────────────────────────────────────────────────

        public async Task<long> PublicarArquivosAsync(
            IList<string> arquivos, string versao, int nivel, string descricao)
        {
            if (arquivos == null || arquivos.Count == 0)
                throw new ArgumentException("Nenhum arquivo selecionado.");

            // Detecta a pasta-base em comum (maior caminho que é prefixo de todos os arquivos)
            string baseDir = PastaEmComum(arquivos);

            // Cria ZIP temporário com paths relativos a partir da pasta-base
            string zipTemp = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(), $"pedeai_pub_{versao.Replace(".", "-")}_{Guid.NewGuid():N}.zip");
            try
            {
                using (var zip = System.IO.Compression.ZipFile.Open(
                    zipTemp, System.IO.Compression.ZipArchiveMode.Create))
                {
                    foreach (string arquivo in arquivos)
                    {
                        if (!System.IO.File.Exists(arquivo)) continue;
                        string relPath = System.IO.Path.GetFullPath(arquivo)
                            .Substring(baseDir.Length)
                            .TrimStart(System.IO.Path.DirectorySeparatorChar,
                                       System.IO.Path.AltDirectorySeparatorChar);
                        // Normaliza separador para '/' dentro do ZIP
                        relPath = relPath.Replace('\\', '/');
                        zip.CreateEntryFromFile(arquivo, relPath,
                            System.IO.Compression.CompressionLevel.Optimal);
                    }
                }
                return await PublicarAsync(zipTemp, versao, nivel, descricao);
            }
            finally
            {
                try { System.IO.File.Delete(zipTemp); } catch { }
            }
        }

        /// <summary>Retorna o diretório-base em comum de uma lista de caminhos (com separador final).</summary>
        private static string PastaEmComum(IList<string> arquivos)
        {
            if (arquivos.Count == 1)
                return System.IO.Path.GetDirectoryName(
                    System.IO.Path.GetFullPath(arquivos[0])) + System.IO.Path.DirectorySeparatorChar;

            string[] parts = System.IO.Path.GetFullPath(arquivos[0]).Split(
                System.IO.Path.DirectorySeparatorChar);
            int comum = parts.Length - 1; // exclui o nome do arquivo

            for (int i = 1; i < arquivos.Count; i++)
            {
                string[] cur = System.IO.Path.GetFullPath(arquivos[i]).Split(
                    System.IO.Path.DirectorySeparatorChar);
                int maxCmp = Math.Min(comum, cur.Length - 1);
                int match = 0;
                for (int j = 0; j < maxCmp; j++)
                {
                    if (string.Equals(parts[j], cur[j],
                        StringComparison.OrdinalIgnoreCase)) match++;
                    else break;
                }
                comum = match;
            }

            return string.Join(System.IO.Path.DirectorySeparatorChar.ToString(),
                parts, 0, comum) + System.IO.Path.DirectorySeparatorChar;
        }

        public async Task<long> PublicarAsync(string arquivoZip, string versao, int nivel, string descricao)
        {
            // 0. Garante que o bucket existe (cria se necessário)
            await GarantirBucketAsync();

            // 1. Upload do ZIP para o Supabase Storage (usa _httpStorage com service_role)
            // Armazena na pasta nomeada pela versão: {versao}/files.zip
            // Codifica cada segmento individualmente para preservar o '/' como separador de caminho.
            string fileName   = versao + "/files.zip";
            string uploadPath = string.Join("/",
                fileName.Split('/').Select(s => Uri.EscapeDataString(s)));
            using var fileContent = new StreamContent(File.OpenRead(arquivoZip));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var uploadReq = new HttpRequestMessage(HttpMethod.Post,
                _storageBase + $"object/{BUCKET}/{uploadPath}");
            uploadReq.Headers.Add("x-upsert", "true");
            uploadReq.Content = fileContent;
            HttpResponseMessage uploadResp;
            string uploadBody;
            try
            {
                uploadResp = await _httpStorage.SendAsync(uploadReq);
                uploadBody = await uploadResp.Content.ReadAsStringAsync();
            }
            catch (Exception ex) when (ex.Message.Contains("copying content to a stream") ||
                                        ex.InnerException?.Message.Contains("copying content to a stream") == true)
            {
                throw new Exception(
                    "O servidor encerrou a conexão durante o upload.\n\n" +
                    "Causa mais provável: a chave JWT está incorreta e o servidor rejeita a requisição.\n" +
                    "Use o SUPABASE_SERVICE_KEY do container supabase-kong (execute: echo $SUPABASE_SERVICE_KEY).");
            }
            if (!uploadResp.IsSuccessStatusCode)
                throw new Exception($"Storage upload falhou [{(int)uploadResp.StatusCode}]: {uploadBody}");

            // 2. Verifica se o ZIP contém update.sql
            bool temSql = false;
            try { using var zip = ZipFile.OpenRead(arquivoZip); temSql = zip.GetEntry("update.sql") != null; }
            catch { }

            // 3. Insere linha na tabela Pacotes
            long tamanho = new FileInfo(arquivoZip).Length;
            var row = await PostOneAsync<PacoteRow>("Pacotes", new
            {
                Versao         = versao,
                Nivel          = nivel,
                Descricao      = descricao ?? "",
                CaminhoArquivo = fileName,
                TamanhoBytes   = tamanho,
                TemSQL         = temSql,
                DataPublicacao = DateTime.UtcNow,
                Ativo          = true
            });
            return row?.Id ?? 0;
        }

        public async Task<List<dynamic>> ListarPacotesAsync()
        {
            string json = await _http.GetStringAsync(_restBase + "Pacotes?order=Id.desc");
            return JsonConvert.DeserializeObject<List<dynamic>>(json);
        }

        public async Task ExcluirPacoteAsync(long id)
        {
            // Desativa o pacote (soft-delete) na tabela
            await PatchAsync("Pacotes", "Id=eq." + id, new { Ativo = false });
        }

        // ── Clientes ──────────────────────────────────────────────────────────────

        public async Task<List<dynamic>> ListarClientesAsync()
        {
            string json = await _http.GetStringAsync(_restBase + "Clientes?order=Id.asc");
            return JsonConvert.DeserializeObject<List<dynamic>>(json);
        }

        public async Task AlterarNivelAsync(long clienteId, int nivel)
            => await PatchAsync("Clientes", "Id=eq." + clienteId, new { Nivel = nivel });

        public async Task AlterarMaxMaquinasAsync(long clienteId, int max)
            => await PatchAsync("Clientes", "Id=eq." + clienteId, new { MaxMaquinas = max });

        public async Task BloquearClienteAsync(long clienteId, bool bloquear)
            => await PatchAsync("Clientes", "Id=eq." + clienteId, new { Bloqueado = bloquear });

        // ── Config ────────────────────────────────────────────────────────────────

        public static (string supabaseUrl, string supabaseKey, string serviceRoleKey) CarregarConfig()
        {
            string path = CaminhoConfig();
            if (!File.Exists(path)) return ("", "", "");
            dynamic c = JsonConvert.DeserializeObject(File.ReadAllText(path));
            string url  = (string)c.SupabaseUrl      ?? (string)c.UpdateVpsUrl ?? "";
            string key  = (string)c.SupabaseKey      ?? (string)c.AdminToken   ?? "";
            string srk  = (string)c.ServiceRoleKey   ?? "";
            return (url, key, srk);
        }

        public static void SalvarConfig(string supabaseUrl, string supabaseKey, string serviceRoleKey)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CaminhoConfig()));
            File.WriteAllText(CaminhoConfig(),
                JsonConvert.SerializeObject(
                    new { SupabaseUrl = supabaseUrl, SupabaseKey = supabaseKey, ServiceRoleKey = serviceRoleKey },
                    Formatting.Indented));
        }

        private static string CaminhoConfig()
            => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PedeaiUpdateAdmin", "config.json");

        /// <summary>
        /// Verifica se o bucket existe e, se não existir, tenta criá-lo.
        /// Lança exceção com mensagem detalhada (incluindo resposta HTTP do Supabase) em caso de falha.
        /// </summary>
        public async Task CriarBucketAsync()
        {
            // Testa se o bucket existe (com _httpStorage que tem service_role)
            var checkResp = await _httpStorage.GetAsync(_storageBase + $"bucket/{BUCKET}");
            if (checkResp.IsSuccessStatusCode) return; // bucket existe, tudo ok

            // Tenta criar automaticamente com service_role
            var body = JsonConvert.SerializeObject(new { id = BUCKET, name = BUCKET, @public = true });
            var req  = new HttpRequestMessage(HttpMethod.Post, _storageBase + "bucket");
            req.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var resp = await _httpStorage.SendAsync(req);
            string rb = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                if (!rb.Contains("already exists") && !rb.Contains("Duplicate"))
                {
                    string extra = "";
                    if (rb.Contains("signature verification failed") || rb.Contains("Unauthorized"))
                        extra = "\n\n\u26a0\ufe0f A chave JWT foi rejeitada pelo Supabase.\n" +
                                "Verifique:\n" +
                                "  \u2022 A chave deve ser a service_role (come\u00e7a com \"eyJ\")\n" +
                                "  \u2022 N\u00e3o use sb_publishable_* \u2014 essa chave n\u00e3o \u00e9 um JWT\n" +
                                "  \u2022 Copie a chave em: Supabase Dashboard \u2192 Settings \u2192 API \u2192 service_role";
                    throw new Exception(
                        $"O bucket \"{BUCKET}\" n\u00e3o existe e n\u00e3o foi poss\u00edvel criar automaticamente.\n\n" +
                        $"Crie manualmente: Supabase \u2192 Storage \u2192 New Bucket \u2192 nome: pacotes \u2192 Public.\n\n" +
                        $"Resposta HTTP {(int)resp.StatusCode}: {rb}{extra}");
                }
            }
        }

        /// <summary>
        /// Versão leniente usada antes do upload: se a verificação falhar por auth (JWT),
        /// ignora e tenta o upload mesmo assim — o upload revelará o erro real.
        /// </summary>
        private async Task GarantirBucketAsync()
        {
            try { await CriarBucketAsync(); }
            catch (Exception ex) when (
                ex.Message.Contains("signature verification failed") ||
                ex.Message.Contains("Unauthorized"))
            {
                // JWT inválido para gerenciar buckets — tenta o upload assim mesmo.
                // Se o bucket não existir ou o JWT for inválido para upload, o próximo
                // passo (Storage upload) mostrará o erro correto.
            }
        }

        // ── Helpers REST ──────────────────────────────────────────────────────────

        private async Task<T> PostOneAsync<T>(string table, object body)
        {
            var json = JsonConvert.SerializeObject(body);
            var req  = new HttpRequestMessage(HttpMethod.Post, _restBase + table);
            req.Headers.Add("Prefer", "return=representation");
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp  = await _http.SendAsync(req);
            string rb = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new Exception($"POST {table} [{(int)resp.StatusCode}]: {rb}");
            var arr = JsonConvert.DeserializeObject<T[]>(rb);
            return arr != null && arr.Length > 0 ? arr[0] : default;
        }

        private async Task PatchAsync(string table, string filter, object body)
        {
            var json = JsonConvert.SerializeObject(body);
            var req  = new HttpRequestMessage(new HttpMethod("PATCH"),
                _restBase + table + "?" + filter);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp  = await _http.SendAsync(req);
            string rb = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new Exception($"PATCH {table} [{(int)resp.StatusCode}]: {rb}");
        }

        private class PacoteRow { public long Id { get; set; } }
    }
}
