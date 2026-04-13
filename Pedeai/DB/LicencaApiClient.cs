using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Pedeai.DB
{
    /// <summary>
    /// Cliente HTTP para comunicação com o PedeaiLicencaServer na VPS.
    /// Chama o endpoint de renovação automática ao iniciar o sistema.
    /// </summary>
    public static class LicencaApiClient
    {
        // HttpClient deve ser instância estática (reusada) para evitar esgotamento de sockets.
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        /// <summary>
        /// Tenta renovar a licença remotamente.
        /// Retorna a nova chave em caso de sucesso, ou null se falhou (sem internet, VPS off, etc.).
        /// Falha silenciosa — o sistema continua com a validação local.
        /// </summary>
        public static string TentarRenovar(string codigoEmpresa, string apiKey, string baseUrl, int diasValidade = 30)
        {
            if (string.IsNullOrWhiteSpace(codigoEmpresa)
                || string.IsNullOrWhiteSpace(apiKey)
                || string.IsNullOrWhiteSpace(baseUrl))
                return null;

            try
            {
                var payload = JsonConvert.SerializeObject(new
                {
                    codigoEmpresa = codigoEmpresa.Trim().ToUpperInvariant(),
                    diasValidade
                });

                var url = baseUrl.TrimEnd('/') + "/api/licenca/renovar";

                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("X-Api-Key", apiKey.Trim());
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = _http.SendAsync(request).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                dynamic result = JsonConvert.DeserializeObject(json);
                return result?.chave?.ToString();
            }
            catch
            {
                // Sem internet, VPS off ou qualquer erro de rede → falha silenciosa.
                return null;
            }
        }

        /// <summary>
        /// Verifica se a VPS está acessível (ping).
        /// </summary>
        public static bool TestarConectividade(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl)) return false;
            try
            {
                var url = baseUrl.TrimEnd('/') + "/api/licenca/ping";
                var response = _http.GetAsync(url).GetAwaiter().GetResult();
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
