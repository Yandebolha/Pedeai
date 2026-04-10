using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Pedeai.BLL
{
    /// <summary>
    /// Integração com Evolution API (WhatsApp self-hosted).
    /// Docs: https://doc.evolution-api.com
    /// </summary>
    public static class WhatsAppService
    {
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(12) };

        public static string ApiUrl   => ConfigurationManager.AppSettings["WhatsAppApiUrl"]   ?? "";
        public static string ApiKey   => ConfigurationManager.AppSettings["WhatsAppApiKey"]   ?? "";
        public static string Instance => ConfigurationManager.AppSettings["WhatsAppInstance"] ?? "pedeai";

        public static bool Ativo => !string.IsNullOrWhiteSpace(ApiUrl);

        // ── Helpers ──────────────────────────────────────────────────────────

        private static HttpRequestMessage Req(HttpMethod method, string path)
        {
            var r = new HttpRequestMessage(method, ApiUrl.TrimEnd('/') + path);
            if (!string.IsNullOrEmpty(ApiKey))
                r.Headers.Add("apikey", ApiKey);
            return r;
        }

        // ── Conexão ──────────────────────────────────────────────────────────

        public static async Task<string> ObterStatusAsync()
        {
            try
            {
                await CriarSeNecessarioAsync().ConfigureAwait(false);

                var resp = await _http.SendAsync(Req(HttpMethod.Get,
                    $"/instance/connectionState/{Instance}")).ConfigureAwait(false);
                var j = JObject.Parse(await resp.Content.ReadAsStringAsync().ConfigureAwait(false));
                return j["instance"]?["state"]?.ToString()
                    ?? j["state"]?.ToString()
                    ?? "desconhecido";
            }
            catch { return "erro"; }
        }

        public static async Task<(Image qrImage, string status, string erro, string pairingCode)> ObterQRAsync()
        {
            try
            {
                await CriarSeNecessarioAsync().ConfigureAwait(false);

                var resp = await _http.SendAsync(Req(HttpMethod.Get,
                    $"/instance/connect/{Instance}")).ConfigureAwait(false);
                var j      = JObject.Parse(await resp.Content.ReadAsStringAsync().ConfigureAwait(false));
                // v2.3+ returns "base64" (data:image/png;base64,...); older returns "code"/"qr"
                string b64 = j["base64"]?.ToString()
                          ?? j["qr"]?.ToString()
                          ?? "";
                string st  = j["status"]?.ToString() ?? await ObterStatusAsync().ConfigureAwait(false);
                string pc  = j["pairingCode"]?.ToString() ?? "";

                Image img = null;
                if (!string.IsNullOrEmpty(b64))
                    img = Base64ToImage(b64);
                return (img, st, "", pc);
            }
            catch (Exception ex) { return (null, "erro", ex.Message, ""); }
        }

        public static async Task<string> ObterPairingCodeAsync(string telefone)
        {
            try
            {
                string fone = System.Text.RegularExpressions.Regex.Replace(telefone, @"\D", "");
                if (fone.Length < 10) return "";
                if (!fone.StartsWith("55")) fone = "55" + fone;

                // Evolution API v2: to get a pairing code, create the instance with
                // qrcode:true + number together — the create response includes pairingCode.
                try
                {
                    await _http.SendAsync(Req(HttpMethod.Delete, $"/instance/delete/{Instance}"))
                               .ConfigureAwait(false);
                    await Task.Delay(1500).ConfigureAwait(false);
                }
                catch { }

                var createBody = new JObject
                {
                    ["instanceName"] = Instance,
                    ["qrcode"]       = true,
                    ["number"]       = fone,
                    ["integration"]  = "WHATSAPP-BAILEYS",
                };
                var cr = Req(HttpMethod.Post, "/instance/create");
                cr.Content = new StringContent(createBody.ToString(), Encoding.UTF8, "application/json");
                var resp = await _http.SendAsync(cr).ConfigureAwait(false);
                var j = JObject.Parse(await resp.Content.ReadAsStringAsync().ConfigureAwait(false));

                // Pairing code is returned directly in the create response
                string code = j["qrcode"]?["pairingCode"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(code)) return code;

                // Fallback: poll /instance/connect up to 3 times (5s each)
                for (int i = 0; i < 3; i++)
                {
                    await Task.Delay(5000).ConfigureAwait(false);
                    var pollResp = await _http.SendAsync(
                        Req(HttpMethod.Get, $"/instance/connect/{Instance}")).ConfigureAwait(false);
                    var pj = JObject.Parse(await pollResp.Content.ReadAsStringAsync().ConfigureAwait(false));
                    code = pj["pairingCode"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(code)) return code;
                }
                return "";
            }
            catch { return ""; }
        }

        public static async Task<bool> DesconectarAsync()
        {
            try
            {
                var resp = await _http.SendAsync(Req(HttpMethod.Delete,
                    $"/instance/logout/{Instance}")).ConfigureAwait(false);
                return resp.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // ── Envio ─────────────────────────────────────────────────────────────

        public static async Task<bool> EnviarAsync(string telefone, string mensagem)
        {
            if (!Ativo || string.IsNullOrWhiteSpace(telefone) || string.IsNullOrWhiteSpace(mensagem))
                return false;
            try
            {
                string fone = System.Text.RegularExpressions.Regex.Replace(telefone, @"\D", "");
                if (fone.Length < 10) return false;
                if (!fone.StartsWith("55")) fone = "55" + fone;

                var body = new JObject
                {
                    ["number"] = fone + "@s.whatsapp.net",
                    ["text"]   = mensagem,
                    ["options"] = new JObject { ["delay"] = 500 }
                };
                var req = Req(HttpMethod.Post, $"/message/sendText/{Instance}");
                req.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                var resp = await _http.SendAsync(req).ConfigureAwait(false);
                return resp.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        /// <summary>Envia em segundo plano sem bloquear a UI.</summary>
        public static void EnviarBackground(string telefone, string mensagem)
        {
            if (!Ativo) return;
            _ = Task.Run(() => EnviarAsync(telefone, mensagem));
        }

        // ── Mensagens pré-formatadas ──────────────────────────────────────────

        public static void NotificarConfirmacao(string telefone, string textoComanda)
        {
            if (!Ativo || string.IsNullOrWhiteSpace(telefone) || string.IsNullOrWhiteSpace(textoComanda))
                return;
            EnviarBackground(telefone, textoComanda);
        }

        public static void NotificarPreparo(string telefone, string nomeCliente, string numeroPedido)
        {
            if (!Ativo) return;
            string template = ConfigurationManager.AppSettings["WhatsAppMsgPreparo"]
                ?? "Olá {Nome}! 🍕 Seu pedido #{Numero} já está sendo preparado. Em breve ficará pronto!";
            string msg = template
                .Replace("{Nome}",   nomeCliente  ?? "")
                .Replace("{Numero}", numeroPedido ?? "");
            EnviarBackground(telefone, msg);
        }

        public static void NotificarEntrega(string telefone, string nomeCliente, string numeroPedido)
        {
            if (!Ativo) return;
            string template = ConfigurationManager.AppSettings["WhatsAppMsgEntrega"]
                ?? "Olá {Nome}! 🛵 Seu pedido #{Numero} saiu para entrega. Aguarde em breve!";
            string msg = template
                .Replace("{Nome}",   nomeCliente  ?? "")
                .Replace("{Numero}", numeroPedido ?? "");
            EnviarBackground(telefone, msg);
        }

        public static void NotificarCupom(string telefone, string nomeCliente,
            string cupomCodigo, string validade)
        {
            if (!Ativo) return;
            string template = ConfigurationManager.AppSettings["WhatsAppMsgCupom"]
                ?? "Parabéns {Nome}! 🎉 Você ganhou um cupom de desconto: *{CupomCodigo}*\nVálido até {Validade}. Use no seu próximo pedido!";
            string msg = template
                .Replace("{Nome}",       nomeCliente ?? "")
                .Replace("{CupomCodigo}", cupomCodigo ?? "")
                .Replace("{Validade}",   validade    ?? "");
            EnviarBackground(telefone, msg);
        }

        // ── Utilitário ────────────────────────────────────────────────────────

        private static async Task CriarSeNecessarioAsync()
        {
            try
            {
                var resp = await _http.SendAsync(
                    Req(HttpMethod.Get, "/instance/fetchInstances")).ConfigureAwait(false);
                string txt = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (txt.Contains(Instance)) return;

                var body = new JObject
                    { ["instanceName"] = Instance, ["qrcode"] = true, ["integration"] = "WHATSAPP-BAILEYS" };
                var cr = Req(HttpMethod.Post, "/instance/create");
                cr.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                await _http.SendAsync(cr).ConfigureAwait(false);
                await Task.Delay(1500).ConfigureAwait(false);
            }
            catch { }
        }

        public static Image Base64ToImage(string base64)
        {
            try
            {
                int comma = base64.IndexOf(',');
                if (comma >= 0) base64 = base64.Substring(comma + 1);
                byte[] bytes = Convert.FromBase64String(base64);
                // New stream to keep alive while image exists
                return Image.FromStream(new MemoryStream(bytes));
            }
            catch { return null; }
        }

        public static void SalvarConfiguracao(
            string apiUrl, string apiKey, string instanceName,
            string msgPreparo, string msgEntrega, string msgCupom)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                void Set(string key, string val)
                {
                    if (config.AppSettings.Settings[key] == null)
                        config.AppSettings.Settings.Add(key, val ?? "");
                    else
                        config.AppSettings.Settings[key].Value = val ?? "";
                }

                Set("WhatsAppApiUrl",   apiUrl);
                Set("WhatsAppApiKey",   apiKey);
                Set("WhatsAppInstance", instanceName);
                Set("WhatsAppMsgPreparo", msgPreparo);
                Set("WhatsAppMsgEntrega", msgEntrega);
                Set("WhatsAppMsgCupom",   msgCupom);

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao salvar configurações: " + ex.Message, ex);
            }
        }
    }
}
