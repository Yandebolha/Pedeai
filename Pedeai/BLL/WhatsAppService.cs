using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Pedeai.Modelo;

namespace Pedeai.BLL
{
    /// <summary>
    /// Integração com Evolution API (WhatsApp self-hosted).
    /// Docs: https://doc.evolution-api.com
    /// </summary>
    public static class WhatsAppService
    {
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };

        // Configuração centralizada no banco de dados (compartilhada entre máquinas em rede)
        private static WhatsAppConfig _cfg;

        private static WhatsAppConfig Cfg
        {
            get
            {
                if (_cfg == null)
                    try { _cfg = new DAL.WhatsAppConfigDAL().Carregar(); }
                    catch { _cfg = new WhatsAppConfig(); }
                return _cfg;
            }
        }

        /// <summary>Invalida o cache local, forçando releitura do banco na próxima chamada.</summary>
        public static void InvalidarCache() => _cfg = null;

        public static string ApiUrl   => Cfg.ApiUrl;
        public static string ApiKey   => Cfg.ApiKey;
        public static string Instance => string.IsNullOrWhiteSpace(Cfg.Instance) ? "pedeai" : Cfg.Instance;
        public static string MsgPreparo => Cfg.MsgPreparo;
        public static string MsgEntrega => Cfg.MsgEntrega;
        public static string MsgCupom   => Cfg.MsgCupom;

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

                // Fallback: poll /instance/connect up to 5 times (8s each)
                for (int i = 0; i < 5; i++)
                {
                    await Task.Delay(8000).ConfigureAwait(false);
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

                // Tenta primeiro sem @s.whatsapp.net (Evolution API v2)
                // depois com sufixo (v1 fallback)
                foreach (string numero in new[] { fone, fone + "@s.whatsapp.net" })
                {
                    var body = new JObject
                    {
                        ["number"] = numero,
                        ["text"]   = mensagem,
                        ["options"] = new JObject { ["delay"] = 500 }
                    };
                    var req = Req(HttpMethod.Post, $"/message/sendText/{Instance}");
                    req.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                    var resp = await _http.SendAsync(req).ConfigureAwait(false);
                    if (resp.IsSuccessStatusCode) return true;

                    // Loga o erro para diagnóstico
                    string errBody = "";
                    try { errBody = await resp.Content.ReadAsStringAsync().ConfigureAwait(false); } catch { }
                    Logger.Log("WhatsAppService", "EnviarAsync",
                        $"Falha [{(int)resp.StatusCode}] para {fone} (fmt={numero}) | {errBody?.Substring(0, Math.Min(200, errBody?.Length ?? 0))}");
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Log("WhatsAppService", "EnviarAsync", $"Exceção para {telefone}", ex);
                return false;
            }
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
            string template = !string.IsNullOrWhiteSpace(MsgPreparo)
                ? MsgPreparo
                : "Olá {Nome}! Seu pedido #{Numero} já está sendo preparado. Em breve ficará pronto!";
            string msg = template
                .Replace("{Nome}",   nomeCliente  ?? "")
                .Replace("{Numero}", numeroPedido ?? "");
            EnviarBackground(telefone, msg);
        }

        public static void NotificarEntrega(string telefone, string nomeCliente, string numeroPedido)
        {
            if (!Ativo) return;
            string template = !string.IsNullOrWhiteSpace(MsgEntrega)
                ? MsgEntrega
                : "Olá {Nome}! 🛵 Seu pedido #{Numero} saiu para entrega. Aguarde em breve!";
            string msg = template
                .Replace("{Nome}",   nomeCliente  ?? "")
                .Replace("{Numero}", numeroPedido ?? "");
            EnviarBackground(telefone, msg);
        }

        public static void NotificarCupom(string telefone, string nomeCliente,
            string cupomCodigo, string validade)
        {
            if (!Ativo) return;
            string template = !string.IsNullOrWhiteSpace(MsgCupom)
                ? MsgCupom
                : "Parabéns {Nome}! 🎉 Você ganhou um cupom de desconto: *{CupomCodigo}*\nVálido até {Validade}. Use no seu próximo pedido!";
            string msg = template
                .Replace("{Nome}",       nomeCliente ?? "")
                .Replace("{CupomCodigo}", cupomCodigo ?? "")
                .Replace("{Validade}",   validade    ?? "");
            EnviarBackground(telefone, msg);
        }

        /// <summary>Notifica cupom de fidelidade com desconto e pedido mínimo detalhados.</summary>
        public static void NotificarCupomDetalhado(string telefone, string nomeCliente,
            string cupomCodigo, string validade, string tipoDesc, decimal valorDesc, decimal pedidoMinimo)
        {
            if (!Ativo) return;
            string descontoStr = tipoDesc == "PERCENTUAL"
                ? $"{valorDesc:0.#}%"
                : $"R$ {valorDesc:N2}";
            string minimoStr = pedidoMinimo > 0 ? $"\nPedido mínimo: R$ {pedidoMinimo:N2}" : "";
            string msg = $"Parabéns {nomeCliente}! 🎉\n" +
                         $"Você ganhou um cupom de desconto: *{cupomCodigo}*\n" +
                         $"Desconto: *{descontoStr}*{minimoStr}\n" +
                         $"Válido até {validade}. Use no seu próximo pedido!";
            EnviarBackground(telefone, msg);
        }

        /// <summary>Notifica prêmio PRODUTO de fidelidade.</summary>
        public static void NotificarPremioProduto(string telefone, string nomeCliente,
            string produtoNome, int qtde)
        {
            if (!Ativo) return;
            string qtdeStr = qtde > 1 ? $"{qtde}x " : "";
            string msg = $"Parabéns {nomeCliente}! 🎁\n" +
                         $"Você atingiu sua meta de fidelidade!\n" +
                         $"Seu prêmio: *{qtdeStr}{produtoNome}* GRÁTIS no próximo pedido. 🥳\n" +
                         $"Informe ao atendente ao fazer seu pedido. Obrigado pela fidelidade!";
            EnviarBackground(telefone, msg);
        }

        public static void NotificarEntregue(string telefone, string nomeCliente, string numeroPedido)
        {
            if (!Ativo) return;
            string msg = $"Olá {nomeCliente}! ✅ Seu pedido #{numeroPedido} foi entregue. Obrigado pela preferência! Esperamos vê-lo novamente em breve.";
            EnviarBackground(telefone, msg);
        }

        public static void NotificarCancelado(string telefone, string nomeCliente, string numeroPedido)
        {
            if (!Ativo) return;
            string msg = $"Olá {nomeCliente}. Infelizmente seu pedido #{numeroPedido} foi cancelado. Em caso de dúvidas, entre em contato conosco.";
            EnviarBackground(telefone, msg);
        }

        /// <summary>Envia mensagem de promoção com cupom de desconto para um cliente.</summary>
        public static async Task<bool> NotificarPromocao(string telefone, string nomeCliente,
            string promNome, string dataFim, string tipoDesc, decimal valorDesc,
            System.Collections.Generic.List<(string nome, decimal preco)> produtos,
            string cupomCodigo)
        {
            if (!Ativo) return false;
            string descontoStr = tipoDesc == "PERCENTUAL"
                ? $"{valorDesc:0.#}% OFF"
                : $"R$ {valorDesc:N2} de desconto";
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Olá, {nomeCliente}! 🏷️");
            sb.AppendLine($"*{promNome}*");
            sb.AppendLine($"Desconto: *{descontoStr}*  |  Válido até: {dataFim}");
            if (produtos.Count > 0)
            {
                sb.AppendLine("\nProdutos em promoção:");
                foreach (var (nome, preco) in produtos)
                {
                    string precoStr = preco > 0 ? $" — R$ {preco:N2}" : "";
                    sb.AppendLine($"  ▪ *{nome}*{precoStr}");
                }
            }
            if (!string.IsNullOrWhiteSpace(cupomCodigo))
            {
                sb.AppendLine();
                sb.AppendLine($"🎟️ Use o cupom *{cupomCodigo}* no seu próximo pedido e ganhe {descontoStr}!");
                sb.AppendLine("Informe ao atendente ao fazer seu pedido.");
            }
            sb.AppendLine("\nNão perca essa oportunidade! 🛍️");
            return await EnviarAsync(telefone, sb.ToString().Trim()).ConfigureAwait(false);
        }

        /// <summary>Envia cardápio do dia para um cliente.</summary>
        public static async Task<bool> NotificarCardapio(string telefone, string nomeCliente,
            string titulo, string data,
            System.Collections.Generic.List<(string nome, string desc)> itens,
            string observacao)
        {
            if (!Ativo) return false;
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Olá, {nomeCliente}! 🍽️");
            sb.AppendLine($"*Cardápio do Dia — {data}*");
            if (!string.IsNullOrWhiteSpace(titulo)) sb.AppendLine($"_{titulo}_");
            sb.AppendLine();
            foreach (var (nome, desc) in itens)
            {
                sb.AppendLine($"  ▪ *{nome}*");
                if (!string.IsNullOrWhiteSpace(desc)) sb.AppendLine($"    {desc}");
            }
            if (!string.IsNullOrWhiteSpace(observacao))
            {
                sb.AppendLine();
                sb.AppendLine(observacao);
            }
            sb.AppendLine("\nFaça seu pedido agora! 📱");
            return await EnviarAsync(telefone, sb.ToString().Trim()).ConfigureAwait(false);
        }

        /// <summary>Lista todos os clientes ativos com telefone/celular disponível.</summary>
        public static System.Collections.Generic.List<(string fone, string nome)> ListarClientesComFone()
        {
            try
            {
                return new DAL.ClienteDAL().ListarComCelular();
            }
            catch { return new System.Collections.Generic.List<(string, string)>(); }
        }

        // ── Notificações Web ──────────────────────────────────────────────────

        /// <summary>Envia confirmação de recebimento de pedido feito pelo site.</summary>
        public static void NotificarPedidoWebRecebido(string telefone, string nomeCliente,
            string numeroPedido, decimal total)
        {
            if (!Ativo || string.IsNullOrWhiteSpace(telefone)) return;
            string msg = $"Olá *{nomeCliente}*! 🎉\n" +
                         $"Seu pedido *#{numeroPedido}* foi recebido com sucesso!\n" +
                         $"Total: *R$ {total:N2}*\n" +
                         $"Em breve entraremos em contato. Obrigado pela preferência! 🍽️";
            EnviarBackground(telefone, msg);
        }

        /// <summary>Envia boas-vindas quando um cliente se cadastra pelo site.</summary>
        public static void NotificarNovoCadastroWeb(string telefone, string nomeCliente)
        {
            if (!Ativo || string.IsNullOrWhiteSpace(telefone)) return;
            string msg = $"Olá *{nomeCliente}*! 😊\n" +
                         $"Seu cadastro foi realizado com sucesso em nosso sistema!\n" +
                         $"Agora você pode acompanhar seus pedidos e aproveitar nossas promoções. Bem-vindo(a)! 🎊";
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
                var cfg = new WhatsAppConfig
                {
                    ApiUrl     = apiUrl,
                    ApiKey     = apiKey,
                    Instance   = string.IsNullOrWhiteSpace(instanceName) ? "pedeai" : instanceName,
                    MsgPreparo = msgPreparo,
                    MsgEntrega = msgEntrega,
                    MsgCupom   = msgCupom,
                };
                new DAL.WhatsAppConfigDAL().Salvar(cfg);
                // Invalida o cache para que todos os processos releiam do banco
                InvalidarCache();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao salvar configurações: " + ex.Message, ex);
            }
        }
    }
}
