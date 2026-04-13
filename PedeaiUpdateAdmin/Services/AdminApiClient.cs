using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PedeaiUpdateAdmin.Services
{
    /// <summary>
    /// Cliente HTTP para o PedeaiUpdateServer.
    /// Configurado em %APPDATA%\PedeaiUpdateAdmin\config.json.
    /// </summary>
    public class AdminApiClient
    {
        private readonly HttpClient _http;
        private readonly string     _baseUrl;
        private readonly string     _adminToken;

        public AdminApiClient(string baseUrl, string adminToken)
        {
            _baseUrl    = baseUrl.TrimEnd('/');
            _adminToken = adminToken;
            _http = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            _http.DefaultRequestHeaders.Add("X-Admin-Token", adminToken);
        }

        // ── Pacotes ───────────────────────────────────────────────────────────────

        public async Task<long> PublicarAsync(string arquivoZip, string versao, int nivel, string descricao)
        {
            using var form = new MultipartFormDataContent();
            using var file = new StreamContent(File.OpenRead(arquivoZip));
            file.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");
            form.Add(file, "arquivo", Path.GetFileName(arquivoZip));
            form.Add(new StringContent(versao),       "versao");
            form.Add(new StringContent(nivel.ToString()), "nivel");
            form.Add(new StringContent(descricao ?? ""), "descricao");

            var resp = await _http.PostAsync($"{_baseUrl}/api/admin/pacotes/publicar", form);
            resp.EnsureSuccessStatusCode();
            dynamic r = JsonConvert.DeserializeObject(await resp.Content.ReadAsStringAsync());
            return (long)r.pacoteId;
        }

        public async Task<List<dynamic>> ListarPacotesAsync()
        {
            var resp = await _http.GetStringAsync($"{_baseUrl}/api/admin/pacotes");
            return JsonConvert.DeserializeObject<List<dynamic>>(resp);
        }

        public async Task ExcluirPacoteAsync(long id)
        {
            var resp = await _http.DeleteAsync($"{_baseUrl}/api/admin/pacotes/{id}");
            resp.EnsureSuccessStatusCode();
        }

        // ── Clientes ──────────────────────────────────────────────────────────────

        public async Task<List<dynamic>> ListarClientesAsync()
        {
            var resp = await _http.GetStringAsync($"{_baseUrl}/api/admin/clientes");
            return JsonConvert.DeserializeObject<List<dynamic>>(resp);
        }

        public async Task AlterarNivelAsync(long clienteId, int nivel)
        {
            var body = new StringContent(
                JsonConvert.SerializeObject(new { nivel }), Encoding.UTF8, "application/json");
            var resp = await _http.PutAsync($"{_baseUrl}/api/admin/clientes/{clienteId}/nivel", body);
            resp.EnsureSuccessStatusCode();
        }

        public async Task BloquearClienteAsync(long clienteId, bool bloquear)
        {
            string acao = bloquear ? "bloquear" : "desbloquear";
            var resp = await _http.PutAsync(
                $"{_baseUrl}/api/admin/clientes/{clienteId}/{acao}",
                new StringContent("", Encoding.UTF8, "application/json"));
            resp.EnsureSuccessStatusCode();
        }

        // ── Config ────────────────────────────────────────────────────────────────

        public static (string baseUrl, string adminToken) CarregarConfig()
        {
            string path = CaminhoConfig();
            if (!File.Exists(path)) return ("http://localhost:5001", "");
            dynamic c = JsonConvert.DeserializeObject(File.ReadAllText(path));
            return ((string)c.UpdateVpsUrl, (string)c.AdminToken);
        }

        public static void SalvarConfig(string baseUrl, string adminToken)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CaminhoConfig()));
            File.WriteAllText(CaminhoConfig(),
                JsonConvert.SerializeObject(new { UpdateVpsUrl = baseUrl, AdminToken = adminToken },
                    Formatting.Indented));
        }

        private static string CaminhoConfig()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PedeaiUpdateAdmin", "config.json");
        }
    }
}
