using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

        public async Task<long> PublicarAsync(string arquivoZip, string versao, int nivel, string descricao)
        {
            // 0. Garante que o bucket existe (cria se necessário)
            await GarantirBucketAsync();

            // 1. Upload do ZIP para o Supabase Storage (usa _httpStorage com service_role)
            string fileName = versao.Replace(".", "-") + "_" + Path.GetFileName(arquivoZip);
            using var fileContent = new StreamContent(File.OpenRead(arquivoZip));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var uploadReq = new HttpRequestMessage(HttpMethod.Post,
                _storageBase + $"object/{BUCKET}/{Uri.EscapeDataString(fileName)}");
            uploadReq.Headers.Add("x-upsert", "true");
            uploadReq.Content = fileContent;
            var uploadResp = await _httpStorage.SendAsync(uploadReq);
            string uploadBody = await uploadResp.Content.ReadAsStringAsync();
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

        private async Task GarantirBucketAsync()
        {
            // Testa se o bucket existe (com _httpStorage que tem service_role)
            var checkResp = await _httpStorage.GetAsync(_storageBase + $"bucket/{BUCKET}");
            if (checkResp.IsSuccessStatusCode) return; // bucket existe, tudo ok

            // Tenta criar automaticamente com service_role
            var body = JsonConvert.SerializeObject(new { id = BUCKET, name = BUCKET, @public = true });
            var req  = new HttpRequestMessage(HttpMethod.Post, _storageBase + "bucket");
            req.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var resp = await _httpStorage.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                string rb = await resp.Content.ReadAsStringAsync();
                if (!rb.Contains("already exists") && !rb.Contains("Duplicate"))
                    throw new Exception(
                        $"O bucket \"{BUCKET}\" não existe e não foi possível criar automaticamente.\n\n" +
                        $"Crie manualmente: Supabase → Storage → New Bucket → nome: pacotes → Public.\n\nDetalhe: {rb}");
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
