using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pedeai.DB
{
    /// <summary>
    /// Serviço de integração com o Supabase (plataforma web RanGoFood).
    /// Sincroniza produtos, grupos, marmitas, cupons, bairros e importa pedidos web.
    /// </summary>
    public static class SupabaseService
    {
        private const string BASE = "https://uwgcmnmzjjinfmxlskks.supabase.co/rest/v1";
        private const string KEY  = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InV3Z2Ntbm16amppbmZteGxza2tzIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc3NjEyOTQwNCwiZXhwIjoyMDkxNzA1NDA0fQ.ZKkr27mHvknWpOh1C9PQePJEAHQYQvnnUjvMEFvj5Gg";

        private static readonly HttpClient _http;
        private static string _marmitaGrupoUuid; // cached UUID of "Marmitas" group

        static SupabaseService()
        {
            _http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _http.DefaultRequestHeaders.Add("apikey", KEY);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", KEY);
        }

        private static string ConnStr =>
            ConfigurationManager.AppSettings["ConnectionString"]
            ?? "Server=localhost;Database=pedeai;User=root;Password=;Port=3306;CharSet=utf8mb4;SslMode=None;";

        // ── HTTP Helpers ─────────────────────────────────────────────────────

        private static async Task<JArray> GetAsync(string endpoint)
        {
            var resp = await _http.GetAsync($"{BASE}/{endpoint}");
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Supabase GET {resp.StatusCode}: {body}");
            var token = JToken.Parse(body);
            return token is JArray arr ? arr : new JArray(token);
        }

        private static async Task<JObject> PostAsync(string endpoint, object body)
        {
            var json    = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var req = new HttpRequestMessage(HttpMethod.Post, $"{BASE}/{endpoint}") { Content = content };
            req.Headers.Add("Prefer", "return=representation");
            var resp     = await _http.SendAsync(req);
            var respBody = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Supabase POST {resp.StatusCode}: {respBody}");
            var token = JToken.Parse(respBody);
            if (token is JArray arr && arr.Count > 0) return (JObject)arr[0];
            if (token is JObject obj) return obj;
            return null;
        }

        private static async Task PatchAsync(string endpoint, string filter, object body)
        {
            var json    = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"{BASE}/{endpoint}?{filter}") { Content = content };
            req.Headers.Add("Prefer", "return=minimal");
            var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                var rb = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Supabase PATCH {resp.StatusCode}: {rb}");
            }
        }

        private static async Task DeleteAsync(string endpoint, string filter)
        {
            using var req = new HttpRequestMessage(HttpMethod.Delete, $"{BASE}/{endpoint}?{filter}");
            req.Headers.Add("Prefer", "return=minimal");
            await _http.SendAsync(req);
        }

        // ── MySQL Helpers ─────────────────────────────────────────────────────

        private static MySqlConnection AbrirMysql()
        {
            var conn = new MySqlConnection(ConnStr);
            conn.Open();
            return conn;
        }

        private static string GetSupabaseUuid(string tabela, string pkCol, int pkVal)
        {
            try
            {
                using var conn = AbrirMysql();
                using var cmd  = new MySqlCommand(
                    $"SELECT supabase_uuid FROM `{tabela}` WHERE `{pkCol}`=@pk LIMIT 1", conn);
                cmd.Parameters.AddWithValue("@pk", pkVal);
                return cmd.ExecuteScalar()?.ToString() ?? "";
            }
            catch { return ""; }
        }

        private static void SaveSupabaseUuid(string tabela, string pkCol, int pkVal, string uuid)
        {
            try
            {
                using var conn = AbrirMysql();
                using var cmd  = new MySqlCommand(
                    $"UPDATE `{tabela}` SET supabase_uuid=@uuid WHERE `{pkCol}`=@pk", conn);
                cmd.Parameters.AddWithValue("@uuid", uuid);
                cmd.Parameters.AddWithValue("@pk",   pkVal);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SaveSupabaseUuid", $"Erro ao salvar UUID em {tabela}", ex);
            }
        }

        private static int ProximoCodigo(string tabela, MySqlConnection conn, MySqlTransaction trans = null)
        {
            using var cmd = new MySqlCommand(
                $"SELECT COALESCE(MAX(Codigo),0)+1 FROM `{tabela}`", conn);
            if (trans != null) cmd.Transaction = trans;
            var r = cmd.ExecuteScalar();
            return r == null || r == DBNull.Value ? 1 : Convert.ToInt32(r);
        }

        private static int ProximoAuxCodigo(string tabela, MySqlConnection conn, MySqlTransaction trans = null)
        {
            using var cmd = new MySqlCommand(
                $"SELECT COALESCE(MAX(auxCodigo),0)+1 FROM `{tabela}`", conn);
            if (trans != null) cmd.Transaction = trans;
            var r = cmd.ExecuteScalar();
            return r == null || r == DBNull.Value ? 1 : Convert.ToInt32(r);
        }

        // ── Grupo Mercadoria ─────────────────────────────────────────────────

        public static async Task<string> SincronizarGrupoAsync(int codigoGrupo)
        {
            try
            {
                string nome, imagemUrl; int ordem; bool ativo, habSite;
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT grmeDescricao_, grmeOrdem, Situacao, " +
                    "COALESCE(grmeHabilitar_Site,0) AS grmeHabilitar_Site, " +
                    "COALESCE(grmeImagem_Url,'')   AS grmeImagem_Url " +
                    "FROM grupo_mercadoria WHERE Codigo=@c LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@c", codigoGrupo);
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return "";
                    nome     = r["grmeDescricao_"]?.ToString() ?? "";
                    ordem    = r["grmeOrdem"] == DBNull.Value ? 0 : Convert.ToInt32(r["grmeOrdem"]);
                    ativo    = r["Situacao"]?.ToString() == "A";
                    habSite  = Convert.ToBoolean(r["grmeHabilitar_Site"]);
                    imagemUrl = r["grmeImagem_Url"]?.ToString() ?? "";
                }

                // Only sync to Supabase if explicitly enabled for site
                if (!habSite) return "";

                string uuid = GetSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo);

                // Check Supabase by nome to avoid duplicates if UUID not stored
                if (string.IsNullOrWhiteSpace(uuid))
                {
                    var existing = await GetAsync($"grupo_mercadoria?nome=eq.{Uri.EscapeDataString(nome)}&limit=1");
                    if (existing.Count > 0)
                    {
                        uuid = existing[0]["id"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(uuid))
                            SaveSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo, uuid);
                    }
                }

                var payload = new { nome, ordem, ativo, imagem_url = imagemUrl };

                if (!string.IsNullOrWhiteSpace(uuid))
                    await PatchAsync("grupo_mercadoria", $"id=eq.{uuid}", payload);
                else
                {
                    var result = await PostAsync("grupo_mercadoria", payload);
                    uuid = result?["id"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(uuid))
                        SaveSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo, uuid);
                }
                return "";
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarGrupoAsync", $"Erro grupo {codigoGrupo}", ex);
                return ex.Message;
            }
        }

        // ── Mercadoria ────────────────────────────────────────────────────────

        public static async Task<string> SincronizarProdutoAsync(int codigoMercadoria)
        {
            try
            {
                string nome, descricao, imagemUrl, situacao;
                decimal precoVenda, precoPromo;
                bool destaque, habSite;
                int codigoGrupo;

                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT mercMercadoria, mercApresentacao, mercPreco_Venda, mercPreco_Promocional, " +
                    "mercImagem_Url, mercDestaque, mercHabilitar_Site, Codigo_Grupo, Situacao " +
                    "FROM mercadoria WHERE Codigo=@c LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@c", codigoMercadoria);
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return "";
                    nome        = r["mercMercadoria"]?.ToString() ?? "";
                    descricao   = r["mercApresentacao"]?.ToString() ?? "";
                    precoVenda  = r["mercPreco_Venda"] == DBNull.Value ? 0m : Convert.ToDecimal(r["mercPreco_Venda"]);
                    precoPromo  = r["mercPreco_Promocional"] == DBNull.Value ? 0m : Convert.ToDecimal(r["mercPreco_Promocional"]);
                    imagemUrl   = r["mercImagem_Url"]?.ToString() ?? "";
                    destaque    = r["mercDestaque"]      != DBNull.Value && Convert.ToBoolean(r["mercDestaque"]);
                    habSite     = r["mercHabilitar_Site"] != DBNull.Value && Convert.ToBoolean(r["mercHabilitar_Site"]);
                    codigoGrupo = r["Codigo_Grupo"] == DBNull.Value ? 0 : Convert.ToInt32(r["Codigo_Grupo"]);
                    situacao    = r["Situacao"]?.ToString() ?? "A";
                }

                string uuid = GetSupabaseUuid("mercadoria", "Codigo", codigoMercadoria);

                if (!habSite || situacao != "A")
                {
                    // Deactivate in Supabase if already synced
                    if (!string.IsNullOrWhiteSpace(uuid))
                        await PatchAsync("mercadoria", $"id=eq.{uuid}", new { ativo = false });
                    return "";
                }

                // Ensure group is synced first
                string grupoUuid = "";
                if (codigoGrupo > 0)
                {
                    await SincronizarGrupoAsync(codigoGrupo);
                    grupoUuid = GetSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo);
                }

                var payload = string.IsNullOrWhiteSpace(grupoUuid)
                    ? (object)new { nome, descricao, preco_venda = precoVenda, preco_promocional = precoPromo,
                                    imagem_url = imagemUrl, ativo = true, destaque }
                    : (object)new { grupo_id = grupoUuid, nome, descricao,
                                    preco_venda = precoVenda, preco_promocional = precoPromo,
                                    imagem_url = imagemUrl, ativo = true, destaque };

                if (!string.IsNullOrWhiteSpace(uuid))
                    await PatchAsync("mercadoria", $"id=eq.{uuid}", payload);
                else
                {
                    var result = await PostAsync("mercadoria", payload);
                    uuid = result?["id"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(uuid))
                        SaveSupabaseUuid("mercadoria", "Codigo", codigoMercadoria, uuid);
                }
                return "";
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarProdutoAsync", $"Erro produto {codigoMercadoria}", ex);
                return ex.Message;
            }
        }

        public static async Task SincronizarTodosProdutosAsync()
        {
            try
            {
                var codigos = new List<int>();
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT Codigo FROM mercadoria WHERE mercHabilitar_Site=1 AND Situacao='A'", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) codigos.Add(Convert.ToInt32(r["Codigo"]));
                }
                foreach (var cod in codigos)
                    await SincronizarProdutoAsync(cod);
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodosProdutosAsync", "Erro", ex);
            }
        }

        // ── Marmita ──────────────────────────────────────────────────────────

        private static async Task<string> ObterOuCriarGrupoMarmitaAsync()
        {
            if (!string.IsNullOrEmpty(_marmitaGrupoUuid)) return _marmitaGrupoUuid;

            var arr = await GetAsync("grupo_mercadoria?nome=eq.Marmitas&limit=1");
            if (arr.Count > 0)
            {
                _marmitaGrupoUuid = arr[0]["id"]?.ToString() ?? "";
                return _marmitaGrupoUuid;
            }
            var result = await PostAsync("grupo_mercadoria",
                new { nome = "Marmitas", ordem = 99, ativo = true });
            _marmitaGrupoUuid = result?["id"]?.ToString() ?? "";
            return _marmitaGrupoUuid;
        }

        public static async Task<string> SincronizarMarmitaAsync(int codigoMarmita)
        {
            try
            {
                string descricao, situacao;
                decimal valor;
                bool habSite, destaque;

                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT marDescricao, marValor, Situacao, " +
                    "COALESCE(marHabilitar_Site,0) AS marHabilitar_Site, " +
                    "COALESCE(marDestaque,0) AS marDestaque " +
                    "FROM marmita WHERE Codigo=@c LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@c", codigoMarmita);
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return "";
                    descricao = r["marDescricao"]?.ToString() ?? "";
                    valor     = r["marValor"] == DBNull.Value ? 0m : Convert.ToDecimal(r["marValor"]);
                    situacao  = r["Situacao"]?.ToString() ?? "A";
                    habSite   = Convert.ToBoolean(r["marHabilitar_Site"]);
                    destaque  = Convert.ToBoolean(r["marDestaque"]);
                }

                string uuid = GetSupabaseUuid("marmita", "Codigo", codigoMarmita);

                if (!habSite || situacao != "A")
                {
                    if (!string.IsNullOrWhiteSpace(uuid))
                        await PatchAsync("mercadoria", $"id=eq.{uuid}", new { ativo = false });
                    return "";
                }

                string grupoUuid = await ObterOuCriarGrupoMarmitaAsync();
                var payload = new
                {
                    grupo_id          = grupoUuid,
                    nome              = descricao,
                    descricao         = "Marmita",
                    preco_venda       = valor,
                    preco_promocional = 0m,
                    imagem_url        = "",
                    ativo             = true,
                    destaque
                };

                if (!string.IsNullOrWhiteSpace(uuid))
                    await PatchAsync("mercadoria", $"id=eq.{uuid}", payload);
                else
                {
                    var result = await PostAsync("mercadoria", payload);
                    uuid = result?["id"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(uuid))
                        SaveSupabaseUuid("marmita", "Codigo", codigoMarmita, uuid);
                }

                // Sync the items that can be chosen for this marmita
                await SincronizarItensMarmitaAsync(codigoMarmita);
                return "";
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarMarmitaAsync", $"Erro marmita {codigoMarmita}", ex);
                return ex.Message;
            }
        }

        public static async Task SincronizarTodasMarmitasAsync()
        {
            try
            {
                var codigos = new List<int>();
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT Codigo FROM marmita WHERE COALESCE(marHabilitar_Site,0)=1 AND Situacao='A'", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) codigos.Add(Convert.ToInt32(r["Codigo"]));
                }
                foreach (var cod in codigos)
                    await SincronizarMarmitaAsync(cod);
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodasMarmitasAsync", "Erro", ex);
            }
        }

        // ── Cupom ─────────────────────────────────────────────────────────────

        public static async Task<string> SincronizarCupomAsync(int codigoCupom)
        {
            try
            {
                string codigo, tipo, situacao;
                decimal valor;
                DateTime? validade;

                int limiteUsos = 0, usosRealizados = 0;
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT cupomCodigo, cupomTipo, cupomValor, cupomValido_Ate, Situacao, " +
                    "COALESCE(cupomLimite_Usos,0) AS lim, COALESCE(cupomUsos_Realizados,0) AS usos " +
                    "FROM cupom WHERE Codigo=@c AND cupomCodigo NOT LIKE 'FID%' LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@c", codigoCupom);
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return "";
                    codigo        = r["cupomCodigo"]?.ToString() ?? "";
                    tipo          = r["cupomTipo"]?.ToString() ?? "PERCENTUAL";
                    valor         = r["cupomValor"] == DBNull.Value ? 0m : Convert.ToDecimal(r["cupomValor"]);
                    situacao      = r["Situacao"]?.ToString() ?? "A";
                    limiteUsos    = Convert.ToInt32(r["lim"]);
                    usosRealizados = Convert.ToInt32(r["usos"]);
                    var v         = r["cupomValido_Ate"];
                    validade      = v == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(v);
                }

                // Map local tipo to Supabase tipo
                string supaTipo = tipo.ToUpper() == "VALOR" ? "fixo" : "porcentagem";
                bool   ativo    = situacao == "A";
                string uuid     = GetSupabaseUuid("cupom", "Codigo", codigoCupom);
                // If no stored UUID, check Supabase by codigo to avoid duplicates
                if (string.IsNullOrWhiteSpace(uuid))
                {
                    var existing = await GetAsync($"cupom?codigo=eq.{Uri.EscapeDataString(codigo)}&limit=1");
                    if (existing.Count > 0)
                    {
                        uuid = existing[0]["id"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(uuid))
                            SaveSupabaseUuid("cupom", "Codigo", codigoCupom, uuid);
                    }
                }
                var payload = new
                {
                    codigo,
                    valor,
                    tipo           = supaTipo,
                    validade       = validade.HasValue ? validade.Value.ToString("yyyy-MM-dd") : (string)null,
                    ativo,
                    limite_usos    = limiteUsos,
                    usos_realizados = usosRealizados,
                };

                if (!string.IsNullOrWhiteSpace(uuid))
                    await PatchAsync("cupom", $"id=eq.{uuid}", payload);
                else
                {
                    var result = await PostAsync("cupom", payload);
                    uuid = result?["id"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(uuid))
                        SaveSupabaseUuid("cupom", "Codigo", codigoCupom, uuid);
                }
                return "";
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarCupomAsync", $"Erro cupom {codigoCupom}", ex);
                return ex.Message;
            }
        }

        public static async Task SincronizarTodosCuponsAsync()
        {
            try
            {
                var codigos = new List<int>();
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT Codigo FROM cupom WHERE Situacao='A' AND cupomCodigo NOT LIKE 'FID%'", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) codigos.Add(Convert.ToInt32(r["Codigo"]));
                }
                foreach (var cod in codigos)
                    await SincronizarCupomAsync(cod);
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodosCuponsAsync", "Erro", ex);
            }
        }

        // ── Bairro / Taxa Entrega ─────────────────────────────────────────────

        public static async Task<string> SincronizarBairroAsync(int codigoBairro)
        {
            try
            {
                string cep, situacao;
                decimal taxa;

                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT COALESCE(baiCEP,'') AS baiCEP, baiTaxa_Entrega, Situacao " +
                    "FROM bairro WHERE Codigo=@c LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@c", codigoBairro);
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return "";
                    cep      = r["baiCEP"]?.ToString()?.Replace("-", "").Trim() ?? "";
                    taxa     = r["baiTaxa_Entrega"] == DBNull.Value ? 0m : Convert.ToDecimal(r["baiTaxa_Entrega"]);
                    situacao = r["Situacao"]?.ToString() ?? "A";
                }

                if (string.IsNullOrWhiteSpace(cep)) return ""; // no CEP, skip

                string uuid = GetSupabaseUuid("bairro", "Codigo", codigoBairro);

                if (situacao != "A")
                {
                    if (!string.IsNullOrWhiteSpace(uuid))
                        await DeleteAsync("taxa_entrega", $"id=eq.{uuid}");
                    return "";
                }

                var payload = new { cep, valor = taxa };

                if (!string.IsNullOrWhiteSpace(uuid))
                    await PatchAsync("taxa_entrega", $"id=eq.{uuid}", payload);
                else
                {
                    var result = await PostAsync("taxa_entrega", payload);
                    uuid = result?["id"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(uuid))
                        SaveSupabaseUuid("bairro", "Codigo", codigoBairro, uuid);
                }
                return "";
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarBairroAsync", $"Erro bairro {codigoBairro}", ex);
                return ex.Message;
            }
        }

        public static async Task SincronizarTodosBairrosAsync()
        {
            try
            {
                var codigos = new List<int>();
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT Codigo FROM bairro WHERE Situacao='A' AND baiCEP IS NOT NULL AND baiCEP <> ''", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) codigos.Add(Convert.ToInt32(r["Codigo"]));
                }
                foreach (var cod in codigos)
                    await SincronizarBairroAsync(cod);
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodosBairrosAsync", "Erro", ex);
            }
        }

        // ── Loja ─────────────────────────────────────────────────────────────

        public static async Task SincronizarLojaAsync()
        {
            try
            {
                string nome, endereco, telefone;
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT COALESCE(NULLIF(empNome_Fantasia,''), empNome) AS nome, " +
                    "empEndereco, empTelefone FROM empresa ORDER BY Codigo LIMIT 1", conn))
                {
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return;
                    nome     = r["nome"]?.ToString() ?? "";
                    endereco = r["empEndereco"]?.ToString() ?? "";
                    telefone = r["empTelefone"]?.ToString() ?? "";
                }

                var lojas = await GetAsync("loja?limit=1");
                if (lojas.Count == 0) return;
                string lojaId = lojas[0]["id"]?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(lojaId)) return;

                object payload = string.IsNullOrWhiteSpace(nome)
                    ? (object)new { endereco, telefone }
                    : new { nome, endereco, telefone };

                await PatchAsync("loja", $"id=eq.{lojaId}", payload);
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarLojaAsync", "Erro", ex);
            }
        }

        // ── Web Orders Import ─────────────────────────────────────────────────

        public static async Task<List<PedidoWebSupabase>> BuscarPedidosPendentesAsync()
        {
            var result = new List<PedidoWebSupabase>();
            try
            {
                // Accept any order that hasn't been processed yet (website may use "pendente" or "banda")
                var arr = await GetAsync("pedido_web?status=in.(pendente,banda,novo,aguardando)&order=created_at.asc&limit=50");
                foreach (JObject item in arr)
                {
                    result.Add(new PedidoWebSupabase
                    {
                        Id              = item["id"]?.ToString()             ?? "",
                        ClienteId       = item["cliente_id"]?.ToString()     ?? "",
                        Subtotal        = item["subtotal"]?.ToObject<decimal>()       ?? 0m,
                        TaxaEntrega     = item["taxa_entrega"]?.ToObject<decimal>()   ?? 0m,
                        Desconto        = item["desconto"]?.ToObject<decimal>()       ?? 0m,
                        Total           = item["total"]?.ToObject<decimal>()          ?? 0m,
                        Status          = item["status"]?.ToString()         ?? "",
                        FormaPagamento  = item["forma_pagamento"]?.ToString() ?? "",
                        EnderecoEntrega = item["endereco_entrega"]?.ToString() ?? "",
                        CupomId         = item["cupom_id"]?.ToString()       ?? "",
                        CreatedAt       = item["created_at"]?.ToObject<DateTime?>()  ?? DateTime.Now,
                        Troco           = item["troco"]?.ToObject<decimal?>(),
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "BuscarPedidosPendentesAsync", "Erro", ex);
            }
            return result;
        }

        private static async Task<List<ItemPedidoWebSupabase>> BuscarItensPedidoAsync(string pedidoId)
        {
            var result = new List<ItemPedidoWebSupabase>();
            try
            {
                var arr = await GetAsync($"itens_pedido_web?pedido_id=eq.{pedidoId}");
                foreach (JObject item in arr)
                {
                    result.Add(new ItemPedidoWebSupabase
                    {
                        Id            = item["id"]?.ToString()              ?? "",
                        PedidoId      = item["pedido_id"]?.ToString()       ?? "",
                        MercadoriaId  = item["mercadoria_id"]?.ToString()   ?? "",
                        Quantidade    = item["quantidade"]?.ToObject<decimal>()    ?? 1m,
                        PrecoUnitario = item["preco_unitario"]?.ToObject<decimal>() ?? 0m,
                        Observacao    = item["observacao"]?.ToString()      ?? "",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "BuscarItensPedidoAsync", "Erro", ex);
            }
            return result;
        }

        private static async Task<ClienteSupabase> BuscarClienteSupabaseAsync(string clienteId)
        {
            if (string.IsNullOrWhiteSpace(clienteId)) return null;
            try
            {
                var arr = await GetAsync($"cliente?id=eq.{clienteId}&limit=1");
                if (arr.Count == 0) return null;
                var item = (JObject)arr[0];
                return new ClienteSupabase
                {
                    Id       = item["id"]?.ToString()        ?? "",
                    Nome     = item["nome"]?.ToString()      ?? "",
                    Telefone = item["telefone"]?.ToString()  ?? "",
                    CpfCnpj  = item["cpf_cnpj"]?.ToString()  ?? "",
                    Endereco = item["endereco"]?.ToString()  ?? "",
                    Numero   = item["numero"]?.ToString()    ?? "",
                    Bairro   = item["bairro"]?.ToString()    ?? "",
                    Cidade   = item["cidade"]?.ToString()    ?? "",
                    Cep      = item["cep"]?.ToString()       ?? "",
                };
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "BuscarClienteSupabaseAsync", "Erro", ex);
                return null;
            }
        }

        public static async Task AtualizarStatusPedidoWebAsync(string supabaseId, string status)
        {
            try
            {
                await PatchAsync("pedido_web", $"id=eq.{supabaseId}", new { status });
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "AtualizarStatusPedidoWebAsync", $"Erro {supabaseId}", ex);
            }
        }

        /// <summary>Importa um pedido web do Supabase para o MySQL local.</summary>
        public static async Task<string> ImportarPedidoAsync(PedidoWebSupabase supaPedido)
        {
            try
            {
                // Check if already imported
                using (var conn = AbrirMysql())
                using (var chk  = new MySqlCommand(
                    "SELECT COUNT(*) FROM pedido_web WHERE pediSupabase_Id=@sid", conn))
                {
                    chk.Parameters.AddWithValue("@sid", supaPedido.Id);
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        return ""; // already imported
                }

                // Fetch items and client
                var itens       = await BuscarItensPedidoAsync(supaPedido.Id);
                ClienteSupabase supaCliente = null;
                if (!string.IsNullOrWhiteSpace(supaPedido.ClienteId))
                    supaCliente = await BuscarClienteSupabaseAsync(supaPedido.ClienteId);

                // Find or create local client by CPF
                int codigoClienteLocal = 0;
                if (supaCliente != null)
                    codigoClienteLocal = BuscarOuCriarClienteLocal(supaCliente, supaPedido.EnderecoEntrega);

                int  formaPagamento = MapearFormaPagamento(supaPedido.FormaPagamento);
                int  tipoEntrega    = string.IsNullOrWhiteSpace(supaPedido.EnderecoEntrega) ? 0 : 1;

                using var connM = AbrirMysql();
                using var trans = connM.BeginTransaction();

                int nextCod = ProximoCodigo("pedido_web", connM, trans);
                int nextAux = ProximoAuxCodigo("pedido_web", connM, trans);

                // Daily sequence number
                int seqDia;
                using (var cmdSeq = new MySqlCommand(
                    "SELECT COALESCE(MAX(CAST(SUBSTR(pediNumero,9) AS UNSIGNED)),0)+1 " +
                    "FROM pedido_web WHERE DATE(pediData_Lancamento)=DATE(@dt)", connM, trans))
                {
                    cmdSeq.Parameters.AddWithValue("@dt", supaPedido.CreatedAt);
                    var res = cmdSeq.ExecuteScalar();
                    seqDia = res == null || res == DBNull.Value ? 1 : Convert.ToInt32(res);
                }
                string numero      = supaPedido.CreatedAt.ToString("yyyyMMdd") + seqDia.ToString("D3");
                string nomeCliente = supaCliente?.Nome ?? "Cliente Web";
                string telefone    = SanitizarTelefone(supaCliente?.Telefone ?? "");

                const string sqlP = @"INSERT INTO pedido_web
                    (auxCodigo, Codigo, pediNumero, pediNome_Cliente, pediTelefone_Cliente,
                     pediSituacao, pediTipo_Entrega, pediForma_Pagamento, pediOrigem,
                     pediSubtotal, pediTaxa_Entrega, pediDesconto, pediValor_Total,
                     pediTroco_Para, pediEndereco_Entrega, pediObservacoes,
                     pediData_Lancamento, Codigo_Cliente, pediSupabase_Id, Situacao, Info)
                    VALUES(@aux, @cod, @num, @nomeCli, @tel, 0, @tipoEnt, @formaPag, 0,
                           @sub, @taxa, @desc, @total, @troco, @end, '',
                           @dtLanc, @codCli, @supId, 'A', '')";

                using (var cmdP = new MySqlCommand(sqlP, connM, trans))
                {
                    cmdP.Parameters.AddWithValue("@aux",    nextAux);
                    cmdP.Parameters.AddWithValue("@cod",    nextCod);
                    cmdP.Parameters.AddWithValue("@num",    numero);
                    cmdP.Parameters.AddWithValue("@nomeCli", nomeCliente);
                    cmdP.Parameters.AddWithValue("@tel",    telefone);
                    cmdP.Parameters.AddWithValue("@tipoEnt", tipoEntrega);
                    cmdP.Parameters.AddWithValue("@formaPag", formaPagamento);
                    cmdP.Parameters.AddWithValue("@sub",    supaPedido.Subtotal);
                    cmdP.Parameters.AddWithValue("@taxa",   supaPedido.TaxaEntrega);
                    cmdP.Parameters.AddWithValue("@desc",   supaPedido.Desconto);
                    cmdP.Parameters.AddWithValue("@total",  supaPedido.Total);
                    cmdP.Parameters.AddWithValue("@troco",  supaPedido.Troco.HasValue
                        ? (object)supaPedido.Troco.Value : DBNull.Value);
                    cmdP.Parameters.AddWithValue("@end",    supaPedido.EnderecoEntrega ?? "");
                    cmdP.Parameters.AddWithValue("@dtLanc", supaPedido.CreatedAt);
                    cmdP.Parameters.AddWithValue("@codCli", codigoClienteLocal > 0
                        ? (object)codigoClienteLocal : DBNull.Value);
                    cmdP.Parameters.AddWithValue("@supId",  supaPedido.Id);
                    cmdP.ExecuteNonQuery();
                }

                // Insert items
                foreach (var it in itens)
                {
                    string nomeProd = BuscarNomeProdutoPorSupabaseUuid(it.MercadoriaId, connM, trans);
                    int    codMerc  = BuscarCodigoProdutoPorSupabaseUuid(it.MercadoriaId, connM, trans);
                    decimal subtotalItem = it.Quantidade * it.PrecoUnitario;

                    int iAux = ProximoAuxCodigo("itens_pedido_web", connM, trans);
                    int iCod = ProximoCodigo("itens_pedido_web", connM, trans);

                    using var cmdI = new MySqlCommand(@"
                        INSERT INTO itens_pedido_web
                        (auxCodigo, Codigo, Codigo_Pedido, Codigo_Mercadoria, itpwNome_Mercadoria,
                         itpwQtde, itpwPreco_Unitario, itpwSubtotal, itpwObservacoes,
                         itpwDesconto_Pct, Situacao, Info)
                        VALUES(@aux, @cod, @pedCod, @merc, @nome, @qtde, @pu, @sub, @obs, 0, 'A', '')",
                        connM, trans);
                    cmdI.Parameters.AddWithValue("@aux",    iAux);
                    cmdI.Parameters.AddWithValue("@cod",    iCod);
                    cmdI.Parameters.AddWithValue("@pedCod", nextCod);
                    cmdI.Parameters.AddWithValue("@merc",   codMerc);
                    cmdI.Parameters.AddWithValue("@nome",   nomeProd);
                    cmdI.Parameters.AddWithValue("@qtde",   it.Quantidade);
                    cmdI.Parameters.AddWithValue("@pu",     it.PrecoUnitario);
                    cmdI.Parameters.AddWithValue("@sub",    subtotalItem);
                    cmdI.Parameters.AddWithValue("@obs",    it.Observacao ?? "");
                    cmdI.ExecuteNonQuery();
                }

                trans.Commit();

                // Register coupon usage if order had a coupon
                if (!string.IsNullOrWhiteSpace(supaPedido.CupomId))
                    RegistrarUsoCupomLocal(supaPedido.CupomId);

                return "";
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "ImportarPedidoAsync", $"Erro pedido {supaPedido.Id}", ex);
                return ex.Message;
            }
        }

        // ── Client Find/Create ───────────────────────────────────────────────

        private static int BuscarOuCriarClienteLocal(ClienteSupabase supaCliente, string enderecoEntrega = "")
        {
            try
            {
                string cpf = SanitizarDigitos(supaCliente.CpfCnpj?.ToString() ?? "");
                string tel = SanitizarTelefone(supaCliente.Telefone?.ToString() ?? "");

                using var conn = AbrirMysql();

                // Search by CPF (compare digits-only to handle masks in DB)
                if (!string.IsNullOrWhiteSpace(cpf) && cpf.Length >= 11)
                {
                    using var chk = new MySqlCommand(
                        "SELECT Codigo FROM cliente WHERE " +
                        "REPLACE(REPLACE(REPLACE(REPLACE(clieCPF_CNPJ_,'.',''),'-',''),'/',''),' ','')=@cpf " +
                        "AND Situacao='A' LIMIT 1", conn);
                    chk.Parameters.AddWithValue("@cpf", cpf);
                    var existing = chk.ExecuteScalar();
                    if (existing != null && existing != DBNull.Value)
                    {
                        int cod = Convert.ToInt32(existing);
                        TentarAtualizarEnderecoCliente(conn, cod, supaCliente, enderecoEntrega);
                        return cod;
                    }
                }

                // Search by phone (digits-only)
                if (!string.IsNullOrWhiteSpace(tel))
                {
                    using var chk2 = new MySqlCommand(
                        "SELECT Codigo FROM cliente WHERE (" +
                        "REPLACE(REPLACE(clieCelular,'(',''),')','') LIKE @telPct OR " +
                        "REPLACE(REPLACE(clieTelefone,'(',''),')','') LIKE @telPct) " +
                        "AND Situacao='A' LIMIT 1", conn);
                    chk2.Parameters.AddWithValue("@telPct", "%" + tel.Substring(Math.Max(0, tel.Length - 8)));
                    var existing2 = chk2.ExecuteScalar();
                    if (existing2 != null && existing2 != DBNull.Value)
                    {
                        int cod2 = Convert.ToInt32(existing2);
                        TentarAtualizarEnderecoCliente(conn, cod2, supaCliente, enderecoEntrega);
                        return cod2;
                    }
                }

                // Create new client — store with masks + address
                int nextCod = ProximoCodigo("cliente", conn);
                int nextAux = ProximoAuxCodigo("cliente", conn);
                string cpfMasked = FormatarCpf(cpf);
                string telMasked = FormatarTelefone(tel);

                // Determine address to save: prefer supaCliente fields, fallback parse enderecoEntrega
                string rua = supaCliente.Endereco, numero = supaCliente.Numero,
                       bairro = supaCliente.Bairro, cidade = supaCliente.Cidade,
                       cep = SanitizarDigitos(supaCliente.Cep ?? "");
                if (string.IsNullOrWhiteSpace(rua) && !string.IsNullOrWhiteSpace(enderecoEntrega))
                    (rua, numero, bairro, cidade) = ParseEndereco(enderecoEntrega);

                using var ins = new MySqlCommand(@"
                    INSERT INTO cliente
                    (auxCodigo, Codigo, clieNome_RazaoSocial, clieCelular, clieTelefone,
                     clieCPF_CNPJ_, clieCEP, clieEndereco, clieNumero, clieBairro, clieCidade,
                     clieData_Cadastro, Situacao, Status_Transmissao, Info)
                    VALUES(@aux, @cod, @nome, @cel, @tel, @cpf, @cep, @end, @num, @bairro, @cidade, NOW(), 'A', 'N', '')", conn);
                ins.Parameters.AddWithValue("@aux",    nextAux);
                ins.Parameters.AddWithValue("@cod",    nextCod);
                ins.Parameters.AddWithValue("@nome",   supaCliente.Nome ?? "Cliente Web");
                ins.Parameters.AddWithValue("@cel",    telMasked);
                ins.Parameters.AddWithValue("@tel",    telMasked);
                ins.Parameters.AddWithValue("@cpf",    cpfMasked);
                ins.Parameters.AddWithValue("@cep",    cep);
                ins.Parameters.AddWithValue("@end",    rua ?? "");
                ins.Parameters.AddWithValue("@num",    numero ?? "");
                ins.Parameters.AddWithValue("@bairro", bairro ?? "");
                ins.Parameters.AddWithValue("@cidade", cidade ?? "");
                ins.ExecuteNonQuery();
                return nextCod;
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "BuscarOuCriarClienteLocal", "Erro", ex);
                return 0;
            }
        }

        private static void TentarAtualizarEnderecoCliente(
            MySqlConnection conn, int codigoCli, ClienteSupabase supaCliente, string enderecoEntrega)
        {
            try
            {
                // Only update if client has no address stored yet
                using var chk = new MySqlCommand(
                    "SELECT clieEndereco FROM cliente WHERE Codigo=@c LIMIT 1", conn);
                chk.Parameters.AddWithValue("@c", codigoCli);
                var existingEnd = chk.ExecuteScalar()?.ToString() ?? "";
                if (!string.IsNullOrWhiteSpace(existingEnd)) return; // already has address

                string rua = supaCliente.Endereco, numero = supaCliente.Numero,
                       bairro = supaCliente.Bairro, cidade = supaCliente.Cidade,
                       cep = SanitizarDigitos(supaCliente.Cep ?? "");
                if (string.IsNullOrWhiteSpace(rua) && !string.IsNullOrWhiteSpace(enderecoEntrega))
                    (rua, numero, bairro, cidade) = ParseEndereco(enderecoEntrega);
                if (string.IsNullOrWhiteSpace(rua)) return;

                using var upd = new MySqlCommand(@"
                    UPDATE cliente SET clieEndereco=@end, clieNumero=@num,
                        clieBairro=@bairro, clieCidade=@cidade, clieCEP=@cep
                    WHERE Codigo=@c", conn);
                upd.Parameters.AddWithValue("@end",    rua);
                upd.Parameters.AddWithValue("@num",    numero ?? "");
                upd.Parameters.AddWithValue("@bairro", bairro ?? "");
                upd.Parameters.AddWithValue("@cidade", cidade ?? "");
                upd.Parameters.AddWithValue("@cep",    cep);
                upd.Parameters.AddWithValue("@c",      codigoCli);
                upd.ExecuteNonQuery();
            }
            catch { /* non-critical */ }
        }

        private static (string rua, string numero, string bairro, string cidade) ParseEndereco(string end)
        {
            // Expected format: "Rua X, 123 - Bairro - Cidade"
            string rua = "", numero = "", bairro = "", cidade = "";
            if (string.IsNullOrWhiteSpace(end)) return (rua, numero, bairro, cidade);
            var parts = end.Split('-');
            var ruaPart = parts[0].Trim();
            var comma = ruaPart.LastIndexOf(',');
            if (comma >= 0) { rua = ruaPart.Substring(0, comma).Trim(); numero = ruaPart.Substring(comma + 1).Trim(); }
            else rua = ruaPart;
            if (parts.Length > 1) bairro = parts[1].Trim();
            if (parts.Length > 2) cidade = parts[2].Trim();
            return (rua, numero, bairro, cidade);
        }

        // ── Internal Helpers ─────────────────────────────────────────────────

        private static string BuscarNomeProdutoPorSupabaseUuid(
            string uuid, MySqlConnection conn, MySqlTransaction trans)
        {
            if (string.IsNullOrWhiteSpace(uuid)) return "Produto";
            try
            {
                using var cmd = new MySqlCommand(
                    "SELECT mercMercadoria FROM mercadoria WHERE supabase_uuid=@u LIMIT 1", conn);
                if (trans != null) cmd.Transaction = trans;
                cmd.Parameters.AddWithValue("@u", uuid);
                var r = cmd.ExecuteScalar();
                if (r != null && r != DBNull.Value) return r.ToString();

                using var cmd2 = new MySqlCommand(
                    "SELECT marDescricao FROM marmita WHERE supabase_uuid=@u LIMIT 1", conn);
                if (trans != null) cmd2.Transaction = trans;
                cmd2.Parameters.AddWithValue("@u", uuid);
                var r2 = cmd2.ExecuteScalar();
                if (r2 != null && r2 != DBNull.Value) return r2.ToString();
            }
            catch { }
            return "Produto Web";
        }

        private static int BuscarCodigoProdutoPorSupabaseUuid(
            string uuid, MySqlConnection conn, MySqlTransaction trans)
        {
            if (string.IsNullOrWhiteSpace(uuid)) return 0;
            try
            {
                using var cmd = new MySqlCommand(
                    "SELECT Codigo FROM mercadoria WHERE supabase_uuid=@u LIMIT 1", conn);
                if (trans != null) cmd.Transaction = trans;
                cmd.Parameters.AddWithValue("@u", uuid);
                var r = cmd.ExecuteScalar();
                if (r != null && r != DBNull.Value) return Convert.ToInt32(r);
            }
            catch { }
            return 0;
        }

        private static int MapearFormaPagamento(string forma)
        {
            if (string.IsNullOrWhiteSpace(forma)) return 0;
            var f = forma.ToLowerInvariant();
            if (f.Contains("pix"))                return 2;
            if (f.Contains("cart") || f.Contains("cred") || f.Contains("deb")) return 1;
            return 0; // dinheiro
        }

        private static string SanitizarDigitos(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            var sb = new StringBuilder();
            foreach (var c in value) if (char.IsDigit(c)) sb.Append(c);
            return sb.ToString();
        }

        private static string SanitizarTelefone(string tel)
        {
            if (string.IsNullOrWhiteSpace(tel)) return "";
            return SanitizarDigitos(tel);
        }

        private static string FormatarCpf(string digits)
        {
            if (digits.Length == 11)
                return $"{digits.Substring(0,3)}.{digits.Substring(3,3)}.{digits.Substring(6,3)}-{digits.Substring(9)}";
            if (digits.Length == 14)
                return $"{digits.Substring(0,2)}.{digits.Substring(2,3)}.{digits.Substring(5,3)}/{digits.Substring(8,4)}-{digits.Substring(12)}";
            return digits;
        }

        private static string FormatarTelefone(string digits)
        {
            if (digits.Length == 11)
                return $"({digits.Substring(0,2)}) {digits.Substring(2,5)}-{digits.Substring(7)}";
            if (digits.Length == 10)
                return $"({digits.Substring(0,2)}) {digits.Substring(2,4)}-{digits.Substring(6)}";
            return digits;
        }

        // ── Full Startup Sync ─────────────────────────────────────────────────

        // ── Cliente Sync ─────────────────────────────────────────────────────

        public static async Task<string> SincronizarClienteAsync(int codigoCliente)
        {
            try
            {
                string nome, celular, cpf, cep, endereco, numero, bairro, cidade;
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT clieNome_RazaoSocial, clieCelular, clieTelefone, clieCPF_CNPJ_, " +
                    "clieCEP, clieEndereco, clieNumero, clieBairro, clieCidade, Situacao " +
                    "FROM cliente WHERE Codigo=@c LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@c", codigoCliente);
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return "";
                    if (r["Situacao"]?.ToString() != "A") return ""; // inactive
                    nome     = r["clieNome_RazaoSocial"]?.ToString() ?? "";
                    string cel = r["clieCelular"]?.ToString() ?? "";
                    string tel = r["clieTelefone"]?.ToString() ?? "";
                    celular  = SanitizarDigitos(string.IsNullOrWhiteSpace(cel) ? tel : cel);
                    cpf      = SanitizarDigitos(r["clieCPF_CNPJ_"]?.ToString() ?? "");
                    cep      = SanitizarDigitos(r["clieCEP"]?.ToString() ?? "");
                    endereco = r["clieEndereco"]?.ToString() ?? "";
                    numero   = r["clieNumero"]?.ToString() ?? "";
                    bairro   = r["clieBairro"]?.ToString() ?? "";
                    cidade   = r["clieCidade"]?.ToString() ?? "";
                }

                if (string.IsNullOrWhiteSpace(nome)) return "";

                string uuid = GetSupabaseUuid("cliente", "Codigo", codigoCliente);

                // Search Supabase by phone to avoid duplicates
                if (string.IsNullOrWhiteSpace(uuid) && !string.IsNullOrWhiteSpace(celular))
                {
                    if (long.TryParse(celular, out long telNum))
                    {
                        var existing = await GetAsync($"cliente?telefone=eq.{telNum}&limit=1");
                        if (existing.Count > 0)
                        {
                            uuid = existing[0]["id"]?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(uuid))
                                SaveSupabaseUuid("cliente", "Codigo", codigoCliente, uuid);
                        }
                    }
                }

                long telLong = long.TryParse(celular, out var tl) ? tl : 0;
                long cpfLong = long.TryParse(cpf,     out var cl) ? cl : 0;

                var payload = new
                {
                    nome,
                    telefone = telLong,
                    cpf_cnpj = cpfLong > 0 ? (object)cpfLong : null,
                    cep,
                    endereco,
                    numero,
                    bairro,
                    cidade,
                };

                if (!string.IsNullOrWhiteSpace(uuid))
                    await PatchAsync("cliente", $"id=eq.{uuid}", payload);
                else
                {
                    var result = await PostAsync("cliente", payload);
                    uuid = result?["id"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(uuid))
                        SaveSupabaseUuid("cliente", "Codigo", codigoCliente, uuid);
                }
                return "";
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarClienteAsync", $"Erro cliente {codigoCliente}", ex);
                return ex.Message;
            }
        }

        public static async Task SincronizarTodosClientesAsync()
        {
            try
            {
                var codigos = new List<int>();
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT Codigo FROM cliente WHERE Situacao='A' ORDER BY clieData_Cadastro DESC LIMIT 500", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) codigos.Add(Convert.ToInt32(r["Codigo"]));
                }
                foreach (var cod in codigos)
                    await SincronizarClienteAsync(cod);
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodosClientesAsync", "Erro", ex);
            }
        }

        // ── Marmita Items Sync ────────────────────────────────────────────────

        public static async Task SincronizarItensMarmitaAsync(int codigoMarmita)
        {
            try
            {
                string marmitaUuid = GetSupabaseUuid("marmita", "Codigo", codigoMarmita);
                if (string.IsNullOrWhiteSpace(marmitaUuid)) return; // marmita not synced yet

                // Load marmita items with their product's supabase_uuid
                var itens = new List<(string nome, string mercUuid, int ordem)>();
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(@"
                    SELECT mi.maritmNome,
                           COALESCE(m.supabase_uuid,'') AS mercUuid
                    FROM marmita_item mi
                    LEFT JOIN mercadoria m ON m.Codigo = mi.maritmCodigo_Merc
                    WHERE mi.Codigo_Marmita = @c
                    ORDER BY mi.maritmNome", conn))
                {
                    cmd.Parameters.AddWithValue("@c", codigoMarmita);
                    using var r = cmd.ExecuteReader();
                    int ordem = 0;
                    while (r.Read())
                        itens.Add((r["maritmNome"]?.ToString() ?? "", r["mercUuid"]?.ToString() ?? "", ordem++));
                }

                // Delete existing items for this marmita in Supabase
                try { await DeleteAsync("marmita_item", $"marmita_id=eq.{marmitaUuid}"); } catch { }

                // Insert new items
                foreach (var (nome, mercUuid, ordem) in itens)
                {
                    var payload = new
                    {
                        marmita_id    = marmitaUuid,
                        mercadoria_id = string.IsNullOrWhiteSpace(mercUuid) ? null : (string)mercUuid,
                        nome,
                        ordem,
                        ativo = true
                    };
                    try { await PostAsync("marmita_item", payload); } catch { }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarItensMarmitaAsync", $"Erro marmita {codigoMarmita}", ex);
            }
        }

        // ── Coupon Usage ──────────────────────────────────────────────────────

        private static void RegistrarUsoCupomLocal(string cupomSupabaseId)
        {
            if (string.IsNullOrWhiteSpace(cupomSupabaseId)) return;
            try
            {
                using var conn = AbrirMysql();
                int codCupom = 0, limite = 0, usos = 0;
                using (var cmdFind = new MySqlCommand(
                    "SELECT Codigo, COALESCE(cupomLimite_Usos,0) AS lim, " +
                    "COALESCE(cupomUsos_Realizados,0) AS usos " +
                    "FROM cupom WHERE supabase_uuid=@u LIMIT 1", conn))
                {
                    cmdFind.Parameters.AddWithValue("@u", cupomSupabaseId);
                    using var r = cmdFind.ExecuteReader();
                    if (!r.Read()) return;
                    codCupom = Convert.ToInt32(r["Codigo"]);
                    limite   = Convert.ToInt32(r["lim"]);
                    usos     = Convert.ToInt32(r["usos"]);
                }

                usos++;
                bool desativar = limite > 0 && usos >= limite;
                string sql = "UPDATE cupom SET cupomUsos_Realizados=@u" +
                             (desativar ? ", Situacao='I'" : "") +
                             " WHERE Codigo=@c";
                using var cmdUpd = new MySqlCommand(sql, conn);
                cmdUpd.Parameters.AddWithValue("@u", usos);
                cmdUpd.Parameters.AddWithValue("@c", codCupom);
                cmdUpd.ExecuteNonQuery();

                // Re-sync to Supabase to reflect updated usage / deactivation
                Task.Run(async () => await SincronizarCupomAsync(codCupom));
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "RegistrarUsoCupomLocal", "Erro", ex);
            }
        }

        // ── (end of new methods) ──────────────────────────────────────────────

        public static async Task SincronizarTodosGruposAsync()
        {
            try
            {
                var codigos = new List<int>();
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT Codigo FROM grupo_mercadoria WHERE Situacao='A' AND COALESCE(grmeHabilitar_Site,0)=1", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) codigos.Add(Convert.ToInt32(r["Codigo"]));
                }
                foreach (var cod in codigos)
                    await SincronizarGrupoAsync(cod);
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodosGruposAsync", "Erro", ex);
            }
        }

        public static async Task SincronizarTudoAsync()
        {
            try
            {
                await SincronizarLojaAsync();
                await SincronizarTodosGruposAsync();
                await SincronizarTodosProdutosAsync();
                await SincronizarTodasMarmitasAsync();
                await SincronizarTodosCuponsAsync();
                await SincronizarTodosBairrosAsync();
                await SincronizarTodosClientesAsync();
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTudoAsync", "Erro", ex);
            }
        }

        /// <summary>
        /// Uploads a local image file to Supabase Storage (bucket: categorias)
        /// and saves the public URL back to grupo_mercadoria.grmeImagem_Url.
        /// </summary>
        public static async Task UploadCategoriaImagemAsync(int codigoGrupo, string localPath)
        {
            try
            {
                if (!System.IO.File.Exists(localPath)) return;

                var ext      = System.IO.Path.GetExtension(localPath).ToLower();
                var fileName = $"categoria_{codigoGrupo}{ext}";
                var mimeType = ext == ".png" ? "image/png"
                             : ext == ".gif" ? "image/gif"
                             : "image/jpeg";

                // Upload to Supabase Storage bucket "categorias"
                var storageBase = "https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object";
                var uploadUrl   = $"{storageBase}/categorias/{fileName}";

                using var content = new ByteArrayContent(System.IO.File.ReadAllBytes(localPath));
                content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                using var req = new HttpRequestMessage(HttpMethod.Post, uploadUrl) { Content = content };
                req.Headers.Add("apikey", KEY);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", KEY);
                req.Headers.Add("x-upsert", "true");   // overwrite if exists

                var resp = await _http.SendAsync(req);
                var body = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                    throw new Exception($"Storage upload {resp.StatusCode}: {body}");

                // Public URL pattern for Supabase Storage
                var publicUrl = $"https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object/public/categorias/{fileName}";

                // Persist public URL to local DB
                using var conn = AbrirMysql();
                using var cmd  = new MySqlCommand(
                    "UPDATE grupo_mercadoria SET grmeImagem_Url=@url WHERE Codigo=@c", conn);
                cmd.Parameters.AddWithValue("@url", publicUrl);
                cmd.Parameters.AddWithValue("@c",   codigoGrupo);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "UploadCategoriaImagemAsync", $"Erro categoria {codigoGrupo}", ex);
            }
        }
    }

    // ── DTOs ─────────────────────────────────────────────────────────────────

    public class PedidoWebSupabase
    {
        public string   Id              { get; set; }
        public string   ClienteId       { get; set; }
        public decimal  Subtotal        { get; set; }
        public decimal  TaxaEntrega     { get; set; }
        public decimal  Desconto        { get; set; }
        public decimal  Total           { get; set; }
        public string   Status          { get; set; }
        public string   FormaPagamento  { get; set; }
        public string   EnderecoEntrega { get; set; }
        public string   CupomId         { get; set; }
        public DateTime CreatedAt       { get; set; }
        public decimal? Troco           { get; set; }
    }

    public class ItemPedidoWebSupabase
    {
        public string  Id            { get; set; }
        public string  PedidoId      { get; set; }
        public string  MercadoriaId  { get; set; }
        public decimal Quantidade    { get; set; }
        public decimal PrecoUnitario { get; set; }
        public string  Observacao    { get; set; }
    }

    public class ClienteSupabase
    {
        public string Id        { get; set; }
        public string Nome      { get; set; }
        public string Telefone  { get; set; }
        public string CpfCnpj   { get; set; }
        public string Endereco  { get; set; } = "";
        public string Numero    { get; set; } = "";
        public string Bairro    { get; set; } = "";
        public string Cidade    { get; set; } = "";
        public string Cep       { get; set; } = "";
    }
}
