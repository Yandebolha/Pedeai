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
        private readonly HttpClient _http;
        private readonly string     _restBase;
        private readonly string     _storageBase;
        private const    string     BUCKET = "pacotes";

        public AdminApiClient(string supabaseUrl, string supabaseKey)
        {
            string url   = supabaseUrl.TrimEnd('/');
            _restBase    = url + "/rest/v1/";
            _storageBase = url + "/storage/v1/";
            _http = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            _http.DefaultRequestHeaders.Add("apikey", supabaseKey);
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", supabaseKey);
        }

        // ── Pacotes ───────────────────────────────────────────────────────────────

        public async Task<long> PublicarAsync(string arquivoZip, string versao, int nivel, string descricao)
        {
            // 1. Upload do ZIP para o Supabase Storage
            string fileName = versao.Replace(".", "-") + "_" + Path.GetFileName(arquivoZip);
            using var fileContent = new StreamContent(File.OpenRead(arquivoZip));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var uploadReq = new HttpRequestMessage(HttpMethod.Post,
                _storageBase + $"object/{BUCKET}/{Uri.EscapeDataString(fileName)}");
            uploadReq.Headers.Add("x-upsert", "true");
            uploadReq.Content = fileContent;
            var uploadResp = await _http.SendAsync(uploadReq);
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

        public static (string supabaseUrl, string supabaseKey) CarregarConfig()
        {
            string path = CaminhoConfig();
            if (!File.Exists(path)) return ("", "");
            dynamic c = JsonConvert.DeserializeObject(File.ReadAllText(path));
            // Compatibilidade retroativa com config antigo (UpdateVpsUrl / AdminToken)
            string url = (string)c.SupabaseUrl ?? (string)c.UpdateVpsUrl ?? "";
            string key = (string)c.SupabaseKey ?? (string)c.AdminToken   ?? "";
            return (url, key);
        }

        public static void SalvarConfig(string supabaseUrl, string supabaseKey)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CaminhoConfig()));
            File.WriteAllText(CaminhoConfig(),
                JsonConvert.SerializeObject(
                    new { SupabaseUrl = supabaseUrl, SupabaseKey = supabaseKey },
                    Formatting.Indented));
        }

        private static string CaminhoConfig()
            => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PedeaiUpdateAdmin", "config.json");

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
