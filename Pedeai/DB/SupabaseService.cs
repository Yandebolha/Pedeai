using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        private const string BASE = "https://supabase.rangofood.com.br/rest/v1";
        private const string KEY  = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzdXBhYmFzZSIsImlhdCI6MTc3ODk0MDk2MCwiZXhwIjo0OTM0NjE0NTYwLCJyb2xlIjoic2VydmljZV9yb2xlIn0.tjMNViPE7WqMoOB9UH_CBKSuMCwU7fE9voX5lrAUvjI";

        // Supabase table names (must match exactly what's in the Supabase project)
        private const string TBL_MERCADORIAS    = "mercadoria";
        private const string TBL_GRUPO_MERC     = "grupo_mercadoria";
        private const string TBL_BAIRRO         = "bairro";
        private const string TBL_CUPOM          = "cupom";
        private const string TBL_TAXA_ENTREGA   = "taxa_entrega";
        private const string TBL_CLIENTE        = "cliente";
        private const string TBL_COMP_GRUPO     = "complemento_grupo";
        private const string TBL_COMPLEMENTO    = "complemento";          // items within a group
        private const string TBL_MERC_COMP_GRP  = "mercadoria_complemento_grupo"; // link table
        private const string TBL_LOJA           = "loja";

        private static readonly HttpClient _http;
        private static string _marmitaGrupoUuid; // cached UUID of "Marmitas" group

        // Session cache: groups already verified/synced this run — avoids re-syncing same group N times
        private static readonly HashSet<int> _gruposSincronizados = new HashSet<int>();

        // Cache: sabe se o Supabase tem colunas fracionado/qtd_sabores na tabela mercadoria
        // null = não testado ainda, true = tem, false = não tem
        private static bool? _supabaseTemColFracionado = null;
        private static bool? _supabaseTemColIsAdicional   = null;
        /// <summary>
        /// Indica se as colunas empresa_codigo já foram criadas no Supabase (migration executada).
        /// null = não testado; true = existem; false = não existem (PGRST204 detectado).
        /// Quando false, o campo empresa_codigo é omitido dos payloads e filtros automaticamente.
        /// </summary>
        private static bool? _supabaseTemColEmpresaCodigo = null;
        /// <summary>
        /// Indica se a coluna max_qtde já existe na tabela adicional (migration executada).
        /// null = não testado; true = existe; false = não existe — campo omitido automaticamente.
        /// </summary>
        private static bool? _supabaseTemColAdicionalMaxQtde = null;
        /// <summary>
        /// Indica se a coluna produto_nome já existe na tabela cupom (migration executada).
        /// null = não testado; true = existe; false = não existe — campo omitido automaticamente.
        /// </summary>
        private static bool? _supabaseTemColCupomProdutoNome = null;

        /// <summary>
        /// Quando false, todas as operações com o Supabase são bloqueadas.
        /// Definido pelo Administrador na aba Sistema de Empresa/Usuários.
        /// </summary>
        public static bool SiteConectado { get; set; } = true;

        /// <summary>Código único da empresa local. Usado para isolar dados no Supabase multi-tenant.</summary>
        private static string _empresaCodigo = "";

        /// <summary>Retorna o código de empresa configurado (somente leitura).</summary>
        public static string EmpresaCodigoAtual => _empresaCodigo;

        /// <summary>Carrega o código da empresa do MySQL. Deve ser chamado na inicialização do sistema.</summary>
        public static void CarregarEmpresaCodigo()
        {
            try
            {
                using var conn = AbrirMysql();
                using var cmd  = new MySqlCommand(
                    "SELECT COALESCE(empCodigo_Empresa,'') AS cod, COALESCE(empHabilitar_Site,1) AS hab FROM empresa ORDER BY Codigo LIMIT 1", conn);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    _empresaCodigo = r["cod"]?.ToString()?.Trim() ?? "";
                    SiteConectado  = r["hab"]?.ToString() != "0";
                }
            }
            catch { /* fallback: código vazio → não filtra */ }
        }

        /// <summary>
        /// Salva o estado de SiteConectado no banco local.
        /// Chamado pelo frmEmpresa quando o Admin altera o checkbox.
        /// </summary>
        public static void SalvarSiteConectado(bool habilitado)
        {
            SiteConectado = habilitado;
            try
            {
                using var conn = AbrirMysql();
                using var cmd  = new MySqlCommand(
                    "UPDATE empresa SET empHabilitar_Site=@v ORDER BY Codigo LIMIT 1", conn);
                cmd.Parameters.AddWithValue("@v", habilitado ? 1 : 0);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        /// <summary>
        /// Helper: retorna true somente quando empresa_codigo está configurado E a coluna
        /// já foi confirmada no Supabase (ou ainda não foi testada = assume que existe).
        /// Retorna false quando PGRST204 já foi detectado anteriormente nesta sessão.
        /// </summary>
        private static bool UsarEmpresaCodigo
            => !string.IsNullOrWhiteSpace(_empresaCodigo) && _supabaseTemColEmpresaCodigo != false;

        /// <summary>Helper: retorna filtro URL para empresa_codigo, vazio se não configurado.</summary>
        private static string EmpresaFilter(bool prefixAmpersand = true)
            => UsarEmpresaCodigo
               ? (prefixAmpersand ? "&" : "") + $"empresa_codigo=eq.{Uri.EscapeDataString(_empresaCodigo)}"
               : "";

        /// <summary>
        /// Reivindica automaticamente os registros do Supabase que ainda não têm empresa_codigo
        /// preenchido, atribuindo o código desta instalação.
        /// Isso substitui qualquer UPDATE manual no SQL — cada cliente "marca" seus dados
        /// na primeira sincronização após a migração, sem intervenção manual.
        /// </summary>
        /// <summary>
        /// Reivindica todos os registros sem empresa_codigo (NULL ou string vazia) para a empresa
        /// atual. Deve ser chamado manualmente pelo usuário UMA VEZ, a partir do cliente da empresa
        /// correta. Trata tanto empresa_codigo IS NULL quanto empresa_codigo = '' (string vazia).
        /// </summary>
        public static async Task CorrigirEmpresaCodigoNuloAsync()
        {
            if (!UsarEmpresaCodigo) return;
            var payload = new { empresa_codigo = _empresaCodigo };
            var tabelas = new[]
            {
                TBL_LOJA, TBL_GRUPO_MERC, TBL_MERCADORIAS,
                TBL_COMP_GRUPO, TBL_COMPLEMENTO,
                "cupom", "bairro", "forma_pagamento", "taxa_entrega",
                "cliente", "enderecos_salvo", "pedido_web", "itens_pedido_web"
            };
            foreach (var tbl in tabelas)
            {
                // Corrige registros com NULL
                try { await PatchAsync(tbl, "empresa_codigo=is.null", payload); }
                catch { /* ignora se a coluna não existir */ }
                // Corrige registros com string vazia
                try { await PatchAsync(tbl, "empresa_codigo=eq.", payload); }
                catch { /* ignora se a coluna não existir */ }
            }
        }
        // Lock por produto: evita condição de corrida quando dois threads tentam criar o mesmo produto
        // ao mesmo tempo (ex: salvar produto + sync periódico rodando simultaneamente)
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, System.Threading.SemaphoreSlim>
            _produtoSyncLocks = new();

        // Lock global: garante que apenas UMA sincronização de catálogo rode por vez
        // (evita duplicatas causadas por sync manual + auto-sync executando ao mesmo tempo)
        private static readonly System.Threading.SemaphoreSlim _syncLock = new(1, 1);

        /// <summary>Clears the per-run group cache. Call at the start of a full sync.</summary>
        public static void ResetSyncCache() { _gruposSincronizados.Clear(); _supabaseTemColFracionado = null; _supabaseTemColIsAdicional = null; _supabaseTemColEmpresaCodigo = null; _supabaseTemColAdicionalMaxQtde = null; _supabaseTemColCupomProdutoNome = null; }
        static SupabaseService()
        {
            _http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _http.DefaultRequestHeaders.Add("apikey", KEY);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", KEY);
        }

        // ── Drive URL helper ─────────────────────────────────────────────────
        /// <summary>
        /// Returns the drive base URL configured in the empresa table, or empty string if not set.
        /// </summary>
        private static string GetImgBBKey()
        {
            try
            {
                using var conn = AbrirMysql();
                using var cmd  = new MySqlCommand(
                    "SELECT COALESCE(empImgBBKey,'') FROM empresa ORDER BY Codigo LIMIT 1", conn);
                return cmd.ExecuteScalar()?.ToString() ?? "";
            }
            catch { return ""; }
        }

        /// <summary>
        /// Uploads an image to ImgBB and returns the direct image URL.
        /// API docs: https://api.imgbb.com/
        /// </summary>
        private static async Task<string> UploadImgBBAsync(string localPath, string apiKey)
        {
            // ImgBB API: chave na query string, imagem como base64 em campo multipart.
            // Não usar FormUrlEncodedContent (corrompe base64) nem ByteArrayContent (API rejeita).
            var base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(localPath));
            var name   = System.IO.Path.GetFileNameWithoutExtension(localPath);

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(base64), "image");
            form.Add(new StringContent(name),   "name");

            var url  = $"https://api.imgbb.com/1/upload?key={Uri.EscapeDataString(apiKey)}";
            var resp = await _http.PostAsync(url, form);
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new Exception($"ImgBB upload {resp.StatusCode}: {body}");
            var j = JObject.Parse(body);
            return j["data"]?["display_url"]?.ToString()
                ?? j["data"]?["url"]?.ToString()
                ?? throw new Exception("ImgBB: URL não encontrada na resposta.");
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
            // Omite max_qtde se coluna ainda não existe no Supabase
            if (_supabaseTemColAdicionalMaxQtde == false && endpoint.StartsWith("adicional"))
                body = RemoveField(body, "max_qtde");
            // Omite produto_nome se coluna ainda não existe no Supabase
            if (_supabaseTemColCupomProdutoNome == false && endpoint.StartsWith("cupom"))
                body = RemoveField(body, "produto_nome");
            object safeBody = _supabaseTemColEmpresaCodigo == false ? RemoveEmpresaCodigo(body) : body;
            var json    = JsonConvert.SerializeObject(safeBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var req = new HttpRequestMessage(HttpMethod.Post, $"{BASE}/{endpoint}") { Content = content };
            req.Headers.Add("Prefer", "return=representation");
            var resp     = await _http.SendAsync(req);
            var respBody = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                // Detecta coluna ausente — desabilita empresa_codigo e tenta novamente
                if (respBody.Contains("PGRST204") && respBody.Contains("empresa_codigo"))
                {
                    _supabaseTemColEmpresaCodigo = false;
                    return await PostAsync(endpoint, body); // retry sem empresa_codigo
                }
                // Detecta max_qtde ausente na tabela adicional — desabilita e tenta novamente
                if (endpoint.StartsWith("adicional") && (respBody.Contains("max_qtde") || respBody.Contains("PGRST204") && respBody.Contains("max_qtde")))
                {
                    _supabaseTemColAdicionalMaxQtde = false;
                    return await PostAsync(endpoint, body);
                }
                // Detecta produto_nome ausente na tabela cupom — desabilita e tenta novamente
                if (endpoint.StartsWith("cupom") && respBody.Contains("produto_nome"))
                {
                    _supabaseTemColCupomProdutoNome = false;
                    return await PostAsync(endpoint, body);
                }
                throw new Exception($"Supabase POST {resp.StatusCode}: {respBody}");
            }
            if (_supabaseTemColEmpresaCodigo == null)
                _supabaseTemColEmpresaCodigo = true; // confirmado que existe
            if (_supabaseTemColAdicionalMaxQtde == null && endpoint.StartsWith("adicional"))
                _supabaseTemColAdicionalMaxQtde = true;
            var token = JToken.Parse(respBody);
            if (token is JArray arr && arr.Count > 0) return (JObject)arr[0];
            if (token is JObject obj) return obj;
            return null;
        }

        /// <summary>
        /// Upsert (INSERT ... ON CONFLICT DO UPDATE) via Supabase resolution=merge-duplicates.
        /// Requires unique indexes to exist on the target table (see migration dedup_and_unique_constraints.sql).
        /// Falls back to a regular POST if the server rejects the merge hint.
        /// </summary>
        private static async Task<JObject> UpsertAsync(string endpoint, object body, string onConflict = null)
        {
            // Apply same column-presence guards as PostAsync
            if (_supabaseTemColAdicionalMaxQtde == false && endpoint.StartsWith("adicional"))
                body = RemoveField(body, "max_qtde");
            if (_supabaseTemColCupomProdutoNome == false && endpoint.StartsWith("cupom"))
                body = RemoveField(body, "produto_nome");
            object safeBody = _supabaseTemColEmpresaCodigo == false ? RemoveEmpresaCodigo(body) : body;
            var json    = JsonConvert.SerializeObject(safeBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            string url  = string.IsNullOrWhiteSpace(onConflict)
                ? $"{BASE}/{endpoint}"
                : $"{BASE}/{endpoint}?on_conflict={Uri.EscapeDataString(onConflict)}";
            using var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            req.Headers.Add("Prefer", "resolution=merge-duplicates,return=representation");
            var resp     = await _http.SendAsync(req);
            var respBody = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                // Detecta coluna ausente — desabilita empresa_codigo e tenta novamente
                if (respBody.Contains("PGRST204") && respBody.Contains("empresa_codigo"))
                {
                    _supabaseTemColEmpresaCodigo = false;
                    return await UpsertAsync(endpoint, body, onConflict);
                }
                if (endpoint.StartsWith("adicional") && respBody.Contains("max_qtde"))
                {
                    _supabaseTemColAdicionalMaxQtde = false;
                    return await UpsertAsync(endpoint, body, onConflict);
                }
                // Se o upsert falhou (ex: índice ainda não existe), cai no POST normal
                return await PostAsync(endpoint, body);
            }
            if (_supabaseTemColEmpresaCodigo == null) _supabaseTemColEmpresaCodigo = true;
            if (_supabaseTemColAdicionalMaxQtde == null && endpoint.StartsWith("adicional"))
                _supabaseTemColAdicionalMaxQtde = true;
            var token = JToken.Parse(respBody);
            if (token is JArray arr && arr.Count > 0) return (JObject)arr[0];
            if (token is JObject obj2) return obj2;
            return null;
        }

        /// <summary>
        /// Remove o campo empresa_codigo de um payload anônimo, convertendo para JObject.
        /// Usado quando a migration ainda não foi executada no Supabase.
        /// </summary>
        private static object RemoveEmpresaCodigo(object body)
        {
            try
            {
                var j = JObject.Parse(JsonConvert.SerializeObject(body));
                j.Remove("empresa_codigo");
                return j;
            }
            catch { return body; }
        }

        private static object RemoveField(object body, string field)
        {
            try
            {
                var j = JObject.Parse(JsonConvert.SerializeObject(body));
                j.Remove(field);
                return j;
            }
            catch { return body; }
        }

        private static async Task PatchAsync(string endpoint, string filter, object body)
        {
            // Omite max_qtde se coluna ainda não existe no Supabase
            if (_supabaseTemColAdicionalMaxQtde == false && endpoint.StartsWith("adicional"))
                body = RemoveField(body, "max_qtde");
            // Omite produto_nome se coluna ainda não existe no Supabase
            if (_supabaseTemColCupomProdutoNome == false && endpoint.StartsWith("cupom"))
                body = RemoveField(body, "produto_nome");

            // Remove filtro de empresa_codigo do filter quando coluna não existe
            string safeFilter = filter;
            if (_supabaseTemColEmpresaCodigo == false && safeFilter.Contains("empresa_codigo"))
            {
                safeFilter = System.Text.RegularExpressions.Regex.Replace(
                    safeFilter, @"&?empresa_codigo=[^&]*", "").TrimStart('&');
            }

            // Remove campo empresa_codigo do payload quando coluna não existe
            object safeBody = body;
            if (_supabaseTemColEmpresaCodigo == false)
                safeBody = RemoveEmpresaCodigo(body);

            var json    = JsonConvert.SerializeObject(safeBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"{BASE}/{endpoint}?{safeFilter}") { Content = content };
            req.Headers.Add("Prefer", "return=minimal");
            var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                var rb = await resp.Content.ReadAsStringAsync();
                // Detecta coluna ausente — desabilita empresa_codigo e tenta novamente
                if (rb.Contains("PGRST204") && rb.Contains("empresa_codigo"))
                {
                    _supabaseTemColEmpresaCodigo = false;
                    await PatchAsync(endpoint, filter, body); // retry sem empresa_codigo
                    return;
                }
                // Detecta max_qtde ausente na tabela adicional — desabilita e tenta novamente
                if (endpoint.StartsWith("adicional") && rb.Contains("max_qtde"))
                {
                    _supabaseTemColAdicionalMaxQtde = false;
                    await PatchAsync(endpoint, filter, body);
                    return;
                }
                // Detecta produto_nome ausente na tabela cupom — desabilita e tenta novamente
                if (endpoint.StartsWith("cupom") && rb.Contains("produto_nome"))
                {
                    _supabaseTemColCupomProdutoNome = false;
                    await PatchAsync(endpoint, filter, body);
                    return;
                }
                throw new Exception($"Supabase PATCH {resp.StatusCode}: {rb}");
            }
            else if (_supabaseTemColEmpresaCodigo == null)
            {
                _supabaseTemColEmpresaCodigo = true; // confirmado que existe
            }
            if (_supabaseTemColAdicionalMaxQtde == null && endpoint.StartsWith("adicional"))
                _supabaseTemColAdicionalMaxQtde = true;
            if (_supabaseTemColCupomProdutoNome == null && endpoint.StartsWith("cupom"))
                _supabaseTemColCupomProdutoNome = true;
        }

        private static async Task DeleteAsync(string endpoint, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return; // safety: never delete all rows
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
            if (!SiteConectado) return "";
            try
            {
                string nome, imagemUrl; int ordem; bool ativo, habSite;
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT grmeDescricao_, grmeOrdem, Situacao, " +
                    "COALESCE(grmeHabilitar_Site,1) AS grmeHabilitar_Site, " +
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

                // Never send local file paths to Supabase
                if (!string.IsNullOrWhiteSpace(imagemUrl) &&
                    !imagemUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    imagemUrl = "";

                string uuid = GetSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo);

                // Skip completely deleted records
                if (!ativo)
                {
                    if (!string.IsNullOrWhiteSpace(uuid))
                        await PatchAsync("grupo_mercadoria", $"id=eq.{uuid}", new { ativo = false });
                    return "";
                }

                // Verify cached UUID still exists in Supabase (may have been deleted)
                // Only clear if we get a confirmed 200-OK with 0 results (not on network error)
                // Verifica apenas por id — sem filtrar empresa_codigo, pois registros criados
                // antes da migração podem ter empresa_codigo=NULL e seriam incorretamente descartados.
                if (!string.IsNullOrWhiteSpace(uuid) && !_gruposSincronizados.Contains(codigoGrupo))
                {
                    try
                    {
                        var check = await GetAsync($"grupo_mercadoria?id=eq.{uuid}&limit=1");
                        if (check.Count == 0) { SaveSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo, ""); uuid = ""; }
                    }
                    catch { /* network error — keep existing UUID, don't clear */ }
                }

                // Check Supabase by nome to avoid duplicates if UUID not stored
                if (string.IsNullOrWhiteSpace(uuid))
                {
                    // Busca sem filtro de empresa_codigo para encontrar registros legados com NULL
                    var existing = await GetAsync($"grupo_mercadoria?nome=eq.{Uri.EscapeDataString(nome)}&limit=1");
                    if (existing.Count > 0)
                    {
                        uuid = existing[0]["id"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(uuid))
                            SaveSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo, uuid);
                    }
                }

                // habSite controls visibility on the site (ativo flag), but we always sync the record
                bool ativoSite = habSite && ativo;
                var payload = UsarEmpresaCodigo
                    ? (object)new { nome, ordem, ativo = ativoSite, imagem_url = imagemUrl, empresa_codigo = _empresaCodigo }
                    : (object)new { nome, ordem, ativo = ativoSite, imagem_url = imagemUrl };

                if (!string.IsNullOrWhiteSpace(uuid))
                {
                    // Filtra somente por id — empresa_codigo vem no payload e é atualizado.
                    // Filtrar por empresa_codigo causava falha silenciosa em registros com empresa_codigo=NULL.
                    object groupPatchPayload = payload;
                    if (string.IsNullOrWhiteSpace(imagemUrl))
                    {
                        // Omit imagem_url from PATCH — never clear an existing Supabase category image.
                        var jo = JObject.FromObject(payload);
                        jo.Remove("imagem_url");
                        groupPatchPayload = jo;
                    }
                    await PatchAsync("grupo_mercadoria", $"id=eq.{uuid}", groupPatchPayload);
                }
                else
                {
                    var result = await UpsertAsync("grupo_mercadoria", payload, "nome");
                    uuid = result?["id"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(uuid))
                        SaveSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo, uuid);
                }
                _gruposSincronizados.Add(codigoGrupo); // mark as synced this session
                return "";
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarGrupoAsync", $"Erro grupo {codigoGrupo}", ex);
                return ex.Message;
            }
        }

        // ── Mercadoria ────────────────────────────────────────────────────────

        public static async Task<string> SincronizarProdutoAsync(int codigoMercadoria, bool forcarAtivoFalse = false)
        {
            if (!SiteConectado) return "";
            // Garante que somente uma thread por vez sincroniza o mesmo produto,
            // evitando criação de duplicatas por chamadas concorrentes.
            var prodLock = _produtoSyncLocks.GetOrAdd(codigoMercadoria, _ => new System.Threading.SemaphoreSlim(1, 1));
            await prodLock.WaitAsync();
            try
            {
                string nome, descricao, imagemUrl, situacao;
                decimal precoVenda, precoPromo, precoAdicional;
                bool destaque, habSite, fracionado, precoFixo;
                int codigoGrupo, qtdSabores, qtdSaboresManual;

                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT mercMercadoria, mercApresentacao, mercPreco_Venda, mercPreco_Promocional, " +
                    "mercImagem_Url, mercDestaque, COALESCE(mercHabilitar_Site,1) AS mercHabilitar_Site, Codigo_Grupo, Situacao, " +
                    "COALESCE(mercFracionado,0) AS mercFracionado, COALESCE(mercQtd_Sabores,1) AS mercQtd_Sabores, " +
                    "COALESCE(mercPreco_Fixo,0) AS mercPreco_Fixo, " +
                    "COALESCE(mercPreco_Adicional,0) AS mercPreco_Adicional, " +
                    "COALESCE(mercQtd_Sabores_Manual,0) AS mercQtd_Sabores_Manual " +
                    "FROM mercadoria WHERE Codigo=@c LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@c", codigoMercadoria);
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return "";
                    nome           = r["mercMercadoria"]?.ToString() ?? "";
                    descricao      = r["mercApresentacao"]?.ToString() ?? "";
                    precoVenda     = r["mercPreco_Venda"] == DBNull.Value ? 0m : Convert.ToDecimal(r["mercPreco_Venda"]);
                    precoPromo     = r["mercPreco_Promocional"] == DBNull.Value ? 0m : Convert.ToDecimal(r["mercPreco_Promocional"]);
                    imagemUrl      = r["mercImagem_Url"]?.ToString() ?? "";
                    destaque       = r["mercDestaque"]      != DBNull.Value && Convert.ToBoolean(r["mercDestaque"]);
                    habSite        = r["mercHabilitar_Site"] != DBNull.Value && Convert.ToBoolean(r["mercHabilitar_Site"]);
                    codigoGrupo    = r["Codigo_Grupo"] == DBNull.Value ? 0 : Convert.ToInt32(r["Codigo_Grupo"]);
                    situacao       = r["Situacao"]?.ToString() ?? "A";
                    fracionado      = r["mercFracionado"]?.ToString() == "1";
                    qtdSabores      = r["mercQtd_Sabores"] == DBNull.Value ? 1 : Convert.ToInt32(r["mercQtd_Sabores"]);
                    precoFixo       = r["mercPreco_Fixo"]?.ToString() == "1";
                    precoAdicional  = r["mercPreco_Adicional"] == DBNull.Value ? 0m : Convert.ToDecimal(r["mercPreco_Adicional"]);
                    qtdSaboresManual = r["mercQtd_Sabores_Manual"] == DBNull.Value ? 0 : Convert.ToInt32(r["mercQtd_Sabores_Manual"]);
                }

                // Never send local file paths to Supabase
                if (!string.IsNullOrWhiteSpace(imagemUrl) &&
                    !imagemUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    imagemUrl = "";

                string uuid = GetSupabaseUuid("mercadoria", "Codigo", codigoMercadoria);

                // Skip completely deleted records
                if (situacao != "A")
                {
                    if (!string.IsNullOrWhiteSpace(uuid))
                        await PatchAsync(TBL_MERCADORIAS, $"id=eq.{uuid}", new { ativo = false });
                    return "";
                }

                // Verify cached UUID still exists in Supabase
                // Only clear on confirmed 200-OK with 0 results (not on network error)
                // Verifica apenas por id — sem empresa_codigo, pois registros legados podem ter empresa_codigo=NULL
                // e seriam incorretamente descartados, gerando duplicatas a cada sync.
                if (!string.IsNullOrWhiteSpace(uuid))
                {
                    try
                    {
                        var check = await GetAsync($"{TBL_MERCADORIAS}?id=eq.{uuid}&limit=1");
                        if (check.Count == 0) { SaveSupabaseUuid("mercadoria", "Codigo", codigoMercadoria, ""); uuid = ""; }
                    }
                    catch { /* network error — keep existing UUID */ }
                }

                // forcarAtivoFalse: produto é complemento puro (não deve aparecer como produto standalone)
                // Produto fracionado SEMPRE fica ativo no site (é o container de tamanho — ex: Pizza Grande)
                // Produto não-fracionado em grupo que tem fracionado fica ativo como sabor disponível,
                // mesmo que não esteja habilitado como produto standalone (habSite=false).
                bool isGrupoComFracionado = false;
                if (!fracionado && !forcarAtivoFalse && codigoGrupo > 0)
                {
                    try
                    {
                        using var connF = AbrirMysql();
                        using var cmdF = new MySqlCommand(
                            "SELECT COUNT(*) FROM mercadoria " +
                            "WHERE Codigo_Grupo=@g AND COALESCE(mercFracionado,0)=1 AND Situacao='A' AND Codigo<>@c LIMIT 1",
                            connF);
                        cmdF.Parameters.AddWithValue("@g", codigoGrupo);
                        cmdF.Parameters.AddWithValue("@c", codigoMercadoria);
                        isGrupoComFracionado = Convert.ToInt32(cmdF.ExecuteScalar()) > 0;
                    }
                    catch { /* ignora — fallback para habSite */ }
                }
                // Visibilidade controlada EXCLUSIVAMENTE pelo checkbox "No site" (habSite).
                // Produtos fracionados, sabores e adicionais só aparecem no site se o
                // usuário marcou explicitamente "No site" — sem sobreposição automática.
                bool ativoSite = forcarAtivoFalse ? false : habSite;

                // Produtos com sabores manuais configurados (Sabores checkbox) são tratados
                // como fracionados no Supabase para que o seletor de sabores apareça no site.
                bool fracionadoSite = fracionado || qtdSaboresManual > 0;
                int  qtdSaboresSite = fracionado ? qtdSabores
                                     : (qtdSaboresManual > 0 ? qtdSaboresManual : qtdSabores);

                // Ensure group UUID is available — only do full sync if not already done this session
                string grupoUuid = "";
                if (codigoGrupo > 0)
                {
                    grupoUuid = GetSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo);
                    if (string.IsNullOrWhiteSpace(grupoUuid))
                    {
                        // Group not yet synced — sync it now and get UUID
                        await SincronizarGrupoAsync(codigoGrupo);
                        grupoUuid = GetSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo);
                    }
                }

                // Detecta (uma vez por sessão) se as colunas fracionado/qtd_sabores existem no Supabase
                if (_supabaseTemColFracionado == null)
                {
                    try
                    {
                        var sample = await GetAsync($"{TBL_MERCADORIAS}?limit=1");
                        _supabaseTemColFracionado = sample.Count > 0 && sample[0]["fracionado"] != null;
                        _supabaseTemColIsAdicional = sample.Count > 0 && sample[0]["is_adicional"] != null;
                    }
                    catch { _supabaseTemColFracionado = false; _supabaseTemColIsAdicional = false; }
                }
                bool usarFracionado  = _supabaseTemColFracionado  == true;
                bool usarIsAdicional = _supabaseTemColIsAdicional  == true;

                // Determina se produto é is_adicional = true no Supabase.
                // REGRA: is_adicional só é true quando o produto é APENAS adicional (sem vínculo tipo='C').
                // Se tem ambos (adicional + complemento), is_adicional = false para que o produto
                // apareça como sabor fracionado E também como entrada na tabela `adicional`.
                bool ehAdicional = false;
                if (usarIsAdicional)
                {
                    try
                    {
                        using var connA = AbrirMysql();
                        using var cmdA  = new MySqlCommand(
                            "SELECT " +
                            "  SUM(CASE WHEN tipo='A' THEN 1 ELSE 0 END) AS qtdAd, " +
                            "  SUM(CASE WHEN tipo='C' THEN 1 ELSE 0 END) AS qtdComp, " +
                            "  SUM(CASE WHEN tipo='S' THEN 1 ELSE 0 END) AS qtdSab " +
                            "FROM mercadoria_vinculo_grupo " +
                            "WHERE Codigo_Mercadoria=@c AND Situacao='A'", connA);
                        cmdA.Parameters.AddWithValue("@c", codigoMercadoria);
                        using var rA = cmdA.ExecuteReader();
                        if (rA.Read())
                        {
                            int qtdAd   = rA["qtdAd"]   == DBNull.Value ? 0 : Convert.ToInt32(rA["qtdAd"]);
                            int qtdComp = rA["qtdComp"] == DBNull.Value ? 0 : Convert.ToInt32(rA["qtdComp"]);
                            int qtdSab  = rA["qtdSab"]  == DBNull.Value ? 0 : Convert.ToInt32(rA["qtdSab"]);
                            // Adicional PURO = tem tipo='A', sem tipo='C'.
                            // Se tem sabores (tipo='S') mas TODOS os alvos são produtos fracionados,
                            // significa que ESTE produto É um sabor/componente desses produtos — deve
                            // ser ocultado do feed principal (is_adicional=true).
                            // Se algum alvo de tipo='S' NÃO é fracionado, o produto tem sub-sabores
                            // próprios (ex: "Banana com mel") e deve aparecer no feed (is_adicional=false).
                            if (qtdAd > 0 && qtdComp == 0)
                            {
                                if (qtdSab == 0)
                                {
                                    ehAdicional = true;
                                }
                                else
                                {
                                    // Verifica se todos os alvos tipo='S' são produtos fracionados
                                    int qtdSabFrac = 0;
                                    try
                                    {
                                        using var connF2 = AbrirMysql();
                                        using var cmdF2  = new MySqlCommand(
                                            "SELECT COUNT(*) FROM mercadoria m " +
                                            "JOIN mercadoria_vinculo_grupo mvg ON mvg.Codigo_Grupo = m.Codigo " +
                                            "WHERE mvg.Codigo_Mercadoria=@c AND mvg.tipo='S' AND mvg.Situacao='A' " +
                                            "AND COALESCE(m.mercFracionado,0)=1", connF2);
                                        cmdF2.Parameters.AddWithValue("@c", codigoMercadoria);
                                        qtdSabFrac = Convert.ToInt32(cmdF2.ExecuteScalar());
                                    }
                                    catch { }
                                    // Se todos os alvos são fracionados → é componente → ocultar do feed
                                    ehAdicional = qtdSabFrac == qtdSab;
                                }
                            }
                        }
                    }
                    catch { }
                }

                // Resolve UUID by name+fracionado lookup in Supabase if not stored in MySQL
                // Filtra por fracionado para não pegar registro antigo com flag diferente
                if (string.IsNullOrWhiteSpace(uuid))
                {
                    try
                    {
                        // Busca por nome+grupo — sem filtrar por fracionado.
                        // Se o produto mudou de fracionado=false para true (ou vice-versa),
                        // devemos ATUALIZAR o registro existente, não criar um novo.
                        var qGrupo = string.IsNullOrWhiteSpace(grupoUuid) ? "" : $"&grupo_id=eq.{grupoUuid}";
                        var existing = await GetAsync(
                            $"{TBL_MERCADORIAS}?nome=eq.{Uri.EscapeDataString(nome)}{qGrupo}{EmpresaFilter()}&limit=1");
                        if (existing.Count > 0)
                        {
                            uuid = existing[0]["id"]?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(uuid))
                                SaveSupabaseUuid("mercadoria", "Codigo", codigoMercadoria, uuid);
                        }
                    }
                    catch { }
                }

                // Monta payload base — inclui fracionado/qtd_sabores se colunas existirem no Supabase
                object BuildPayload(bool comImagem)
                {
                    string emp = _empresaCodigo;
                    bool temEmp = UsarEmpresaCodigo;
                    if (usarFracionado && usarIsAdicional)
                        return string.IsNullOrWhiteSpace(grupoUuid)
                            ? temEmp ? (object)new { nome, descricao, preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque, fracionado = fracionadoSite, qtd_sabores = qtdSaboresSite,
                                            preco_fixo = precoFixo,
                                            is_adicional = ehAdicional, preco_adicional = precoAdicional, empresa_codigo = emp }
                                     : (object)new { nome, descricao, preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque, fracionado = fracionadoSite, qtd_sabores = qtdSaboresSite,
                                            preco_fixo = precoFixo,
                                            is_adicional = ehAdicional, preco_adicional = precoAdicional }
                            : temEmp ? (object)new { grupo_id = grupoUuid, nome, descricao,
                                            preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque, fracionado = fracionadoSite, qtd_sabores = qtdSaboresSite,
                                            preco_fixo = precoFixo,
                                            is_adicional = ehAdicional, preco_adicional = precoAdicional, empresa_codigo = emp }
                                     : (object)new { grupo_id = grupoUuid, nome, descricao,
                                            preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque, fracionado = fracionadoSite, qtd_sabores = qtdSaboresSite,
                                            preco_fixo = precoFixo,
                                            is_adicional = ehAdicional, preco_adicional = precoAdicional };
                    else if (usarFracionado)
                        return string.IsNullOrWhiteSpace(grupoUuid)
                            ? temEmp ? (object)new { nome, descricao, preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque, fracionado = fracionadoSite, qtd_sabores = qtdSaboresSite,
                                            preco_fixo = precoFixo, empresa_codigo = emp }
                                     : (object)new { nome, descricao, preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque, fracionado = fracionadoSite, qtd_sabores = qtdSaboresSite,
                                            preco_fixo = precoFixo }
                            : temEmp ? (object)new { grupo_id = grupoUuid, nome, descricao,
                                            preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque, fracionado = fracionadoSite, qtd_sabores = qtdSaboresSite,
                                            preco_fixo = precoFixo, empresa_codigo = emp }
                                     : (object)new { grupo_id = grupoUuid, nome, descricao,
                                            preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque, fracionado = fracionadoSite, qtd_sabores = qtdSaboresSite,
                                            preco_fixo = precoFixo };
                    else
                        return string.IsNullOrWhiteSpace(grupoUuid)
                            ? temEmp ? (object)new { nome, descricao, preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque, empresa_codigo = emp }
                                     : (object)new { nome, descricao, preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque }
                            : temEmp ? (object)new { grupo_id = grupoUuid, nome, descricao,
                                            preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque, empresa_codigo = emp }
                                     : (object)new { grupo_id = grupoUuid, nome, descricao,
                                            preco_venda = precoVenda,
                                            imagem_url = comImagem ? imagemUrl : null,
                                            ativo = ativoSite, destaque };
                }

                var postPayload = BuildPayload(true);

                if (!string.IsNullOrWhiteSpace(uuid))
                {
                    object patchPayload;
                    if (string.IsNullOrWhiteSpace(imagemUrl))
                    {
                        // Omit imagem_url from PATCH — never clear an existing Supabase image
                        // just because MySQL still has a local path or no URL yet.
                        var jo = JObject.FromObject(BuildPayload(false));
                        jo.Remove("imagem_url");
                        patchPayload = jo;
                    }
                    else
                    {
                        patchPayload = postPayload;
                    }
                    // PATCH pelo id apenas — empresa_codigo vem no payload e é atualizado.
                    // Filtrar por empresa_codigo causava falha silenciosa quando o registro
                    // ainda tinha empresa_codigo=NULL ("Corrigir Dados" não executado).
                    await PatchAsync(TBL_MERCADORIAS, $"id=eq.{uuid}", patchPayload);
                }
                else
                {
                    // Antes de POST: garante que o grupo existe no Supabase.
                    // O verify check em SincronizarGrupoAsync pode ter mantido UUID inválido
                    // se houve exceção durante o GET (ex: timeout, RLS). Isso causaria
                    // FK constraint 23503 no POST do produto.
                    if (!string.IsNullOrWhiteSpace(grupoUuid) && codigoGrupo > 0)
                    {
                        try
                        {
                            var grpCheck = await GetAsync($"grupo_mercadoria?id=eq.{grupoUuid}&limit=1");
                            if (grpCheck.Count == 0)
                            {
                                // UUID do grupo é inválido — limpa e re-sincroniza o grupo
                                SaveSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo, "");
                                _gruposSincronizados.Remove(codigoGrupo);
                                await SincronizarGrupoAsync(codigoGrupo);
                                grupoUuid = GetSupabaseUuid("grupo_mercadoria", "Codigo", codigoGrupo);
                                // Reconstrói payload com novo grupoUuid
                                postPayload = BuildPayload(true);
                            }
                        }
                        catch { /* mantém grupoUuid atual em caso de erro de rede */ }
                    }

                    var result = await UpsertAsync(TBL_MERCADORIAS, postPayload, "nome,grupo_id");
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
            finally
            {
                prodLock.Release();
            }
        }

        public static async Task<List<string>> SincronizarTodosProdutosAsync()
        {
            var erros = new List<string>();
            try
            {
                var codigos = new List<int>();
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT Codigo FROM mercadoria", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) codigos.Add(Convert.ToInt32(r["Codigo"]));
                }
                foreach (var cod in codigos)
                {
                    var err = await SincronizarProdutoAsync(cod);
                    if (!string.IsNullOrWhiteSpace(err)) erros.Add($"Produto {cod}: {err}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodosProdutosAsync", "Erro", ex);
                erros.Add($"Produtos: {ex.Message}");
            }
            return erros;
        }

        // ── Marmita ──────────────────────────────────────────────────────────

        private static async Task<string> ObterOuCriarGrupoMarmitaAsync()
        {
            // Verify cached UUID still exists in Supabase
            if (!string.IsNullOrEmpty(_marmitaGrupoUuid))
            {
                try
                {
                    var check = await GetAsync($"grupo_mercadoria?id=eq.{_marmitaGrupoUuid}&limit=1");
                    if (check.Count == 0) _marmitaGrupoUuid = null; // stale — re-fetch below
                }
                catch { _marmitaGrupoUuid = null; }
            }

            if (!string.IsNullOrEmpty(_marmitaGrupoUuid)) return _marmitaGrupoUuid;

            var arr = await GetAsync($"grupo_mercadoria?nome=eq.Marmitas{EmpresaFilter()}&limit=1");
            if (arr.Count > 0)
            {
                _marmitaGrupoUuid = arr[0]["id"]?.ToString() ?? "";
                return _marmitaGrupoUuid;
            }
            var result = await UpsertAsync("grupo_mercadoria",
                UsarEmpresaCodigo
                    ? (object)new { nome = "Marmitas", ordem = 99, ativo = true, empresa_codigo = _empresaCodigo }
                    : (object)new { nome = "Marmitas", ordem = 99, ativo = true },
                "nome");
            _marmitaGrupoUuid = result?["id"]?.ToString() ?? "";
            return _marmitaGrupoUuid;
        }

        public static async Task<string> SincronizarMarmitaAsync(int codigoMarmita)
        {
            if (!SiteConectado) return "";
            try
            {
                string descricao, situacao, imagemUrl;
                decimal valor;
                bool habSite, destaque;

                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT marDescricao, marValor, Situacao, " +
                    "COALESCE(marHabilitar_Site,0) AS marHabilitar_Site, " +
                    "COALESCE(marDestaque,0) AS marDestaque, " +
                    "COALESCE(marImagem_Url,'') AS marImagem_Url " +
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
                    imagemUrl = r["marImagem_Url"]?.ToString() ?? "";
                }

                // Never send local file paths to Supabase
                if (!string.IsNullOrWhiteSpace(imagemUrl) &&
                    !imagemUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    imagemUrl = "";

                string uuid = GetSupabaseUuid("marmita", "Codigo", codigoMarmita);

                // Skip completely deleted records
                if (situacao != "A")
                {
                    if (!string.IsNullOrWhiteSpace(uuid))
                        await PatchAsync(TBL_MERCADORIAS, $"id=eq.{uuid}", new { ativo = false });
                    return "";
                }

                // Verify cached UUID still exists in Supabase
                // Only clear on confirmed 200-OK with 0 results
                // Verifica apenas por id — sem empresa_codigo para não perder registros com empresa_codigo=NULL.
                if (!string.IsNullOrWhiteSpace(uuid))
                {
                    try
                    {
                        var check = await GetAsync($"{TBL_MERCADORIAS}?id=eq.{uuid}&limit=1");
                        if (check.Count == 0) { SaveSupabaseUuid("marmita", "Codigo", codigoMarmita, ""); uuid = ""; }
                    }
                    catch { /* keep UUID on network error */ }
                }

                // habSite controls Supabase visibility — always sync
                bool ativoSite = habSite;

                string grupoUuid = await ObterOuCriarGrupoMarmitaAsync();

                // Resolve UUID by name lookup in Supabase if not stored in MySQL (avoids duplicate POST)
                if (string.IsNullOrWhiteSpace(uuid))
                {
                    try
                    {
                        var existing = await GetAsync(
                            $"{TBL_MERCADORIAS}?nome=eq.{Uri.EscapeDataString(descricao)}{EmpresaFilter()}&limit=1");
                        if (existing.Count > 0)
                        {
                            uuid = existing[0]["id"]?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(uuid))
                                SaveSupabaseUuid("marmita", "Codigo", codigoMarmita, uuid);
                        }
                    }
                    catch { }
                }

                bool temEmpMar = UsarEmpresaCodigo;
                var postPayload = temEmpMar
                    ? (object)new { grupo_id = grupoUuid, nome = descricao, descricao = "Marmita",
                                    preco_venda = valor, imagem_url = imagemUrl, ativo = ativoSite, destaque,
                                    empresa_codigo = _empresaCodigo }
                    : (object)new { grupo_id = grupoUuid, nome = descricao, descricao = "Marmita",
                                    preco_venda = valor, imagem_url = imagemUrl, ativo = ativoSite, destaque };

                if (!string.IsNullOrWhiteSpace(uuid))
                {
                    // PATCH: only update imagem_url if we actually have one (don't clear existing)
                    object patchPayload = string.IsNullOrWhiteSpace(imagemUrl)
                        ? temEmpMar
                            ? (object)new { grupo_id = grupoUuid, nome = descricao, descricao = "Marmita",
                                            preco_venda = valor, ativo = ativoSite, destaque, empresa_codigo = _empresaCodigo }
                            : (object)new { grupo_id = grupoUuid, nome = descricao, descricao = "Marmita",
                                            preco_venda = valor, ativo = ativoSite, destaque }
                        : postPayload;
                    // PATCH apenas pelo id — empresa_codigo vem no payload, não no filtro.
                    await PatchAsync(TBL_MERCADORIAS, $"id=eq.{uuid}", patchPayload);
                }
                else
                {
                    var result = await UpsertAsync(TBL_MERCADORIAS, postPayload, "nome,grupo_id");
                    uuid = result?["id"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(uuid))
                        SaveSupabaseUuid("marmita", "Codigo", codigoMarmita, uuid);
                }

                // Items/complement groups are synced separately via SincronizarComplementosMarmitaAsync
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
                    "SELECT Codigo FROM marmita", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) codigos.Add(Convert.ToInt32(r["Codigo"]));
                }
                foreach (var cod in codigos)
                {
                    await SincronizarMarmitaAsync(cod);
                    await SincronizarItensMarmitaAsync(cod);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodasMarmitasAsync", "Erro", ex);
            }
        }

        // ── Cupom ─────────────────────────────────────────────────────────────

        // ── Formas de Pagamento ───────────────────────────────────────────────

        /// <summary>
        /// Seeds the Supabase forma_pagamento table with the default payment methods
        /// (Dinheiro, Cartão, Pix). Only inserts rows that are not already present.
        /// </summary>
        public static async Task SincronizarFormasPagamentoAsync()
        {
            try
            {
                // Filtra por empresa_codigo para não detectar registros de outras empresas
                string epFilter = UsarEmpresaCodigo ? "&empresa_codigo=eq." + Uri.EscapeDataString(_empresaCodigo) : "";
                var existentes = await GetAsync("forma_pagamento?select=nome" + epFilter);
                var nomesExistentes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (JObject fp in existentes)
                {
                    var n = fp["nome"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(n)) nomesExistentes.Add(n);
                }

                var defaults = new (string nome, string tipo)[]
                {
                    ("Dinheiro",       "dinheiro"),
                    ("Cartão Crédito", "cartao"),
                    ("Cartão Débito",  "cartao"),
                    ("Pix",            "pix"),
                };

                foreach (var (nome, tipo) in defaults)
                {
                    if (nomesExistentes.Contains(nome)) continue;
                    object payload = UsarEmpresaCodigo
                        ? (object)new { nome, tipo, ativo = true, empresa_codigo = _empresaCodigo }
                        : new { nome, tipo, ativo = true };
                    try { await UpsertAsync("forma_pagamento", payload, "nome"); } catch { }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarFormasPagamentoAsync", "Erro", ex);
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
                    string searchFilter = $"cupom?codigo=eq.{Uri.EscapeDataString(codigo)}&limit=1";
                    if (UsarEmpresaCodigo)
                        searchFilter += $"&empresa_codigo=eq.{Uri.EscapeDataString(_empresaCodigo)}";
                    var existing = await GetAsync(searchFilter);
                    if (existing.Count > 0)
                    {
                        uuid = existing[0]["id"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(uuid))
                            SaveSupabaseUuid("cupom", "Codigo", codigoCupom, uuid);
                    }
                }

                string validadeStr = validade.HasValue
                    ? validade.Value.Date.Add(new TimeSpan(23, 59, 59)).ToString("yyyy-MM-ddTHH:mm:ss")
                    : (string)null;

                var payload = UsarEmpresaCodigo
                    ? (object)new
                    {
                        codigo,
                        valor,
                        tipo            = supaTipo,
                        validade        = validadeStr,
                        ativo,
                        limite_usos     = limiteUsos,
                        usos_realizados = usosRealizados,
                        empresa_codigo  = _empresaCodigo,
                    }
                    : (object)new
                    {
                        codigo,
                        valor,
                        tipo            = supaTipo,
                        validade        = validadeStr,
                        ativo,
                        limite_usos     = limiteUsos,
                        usos_realizados = usosRealizados,
                    };

                if (!string.IsNullOrWhiteSpace(uuid))
                {
                    string patchFilter = UsarEmpresaCodigo
                        ? $"id=eq.{uuid}&empresa_codigo=eq.{Uri.EscapeDataString(_empresaCodigo)}"
                        : $"id=eq.{uuid}";
                    await PatchAsync("cupom", patchFilter, payload);
                }
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

        // ── Cupons de Fidelização (vinculados a cliente_id) ─────────────────────

        /// <summary>
        /// Sincroniza um único cupão de fidelização ao Supabase, vinculando-o ao
        /// cliente através do campo cliente_id. Chamado ao criar o cupão no BLL.
        /// </summary>
        public static async Task SincronizarCupomFidelizacaoAsync(string codigoCupom, int codigoCliente)
        {
            if (!SiteConectado || string.IsNullOrWhiteSpace(codigoCupom)) return;
            try
            {
                string tipo, situacao, descricao;
                decimal valor;
                DateTime? validade;
                int limiteUsos, usosRealizados, codigoCupomMysql;
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT Codigo, cupomTipo, cupomValor, cupomValido_Ate, Situacao, " +
                    "COALESCE(cupomLimite_Usos,1) AS lim, COALESCE(cupomUsos_Realizados,0) AS usos, " +
                    "COALESCE(cupomDescricao,'') AS descricao " +
                    "FROM cupom WHERE cupomCodigo=@cod AND cupomCodigo LIKE 'FID%' LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@cod", codigoCupom);
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return;
                    codigoCupomMysql = Convert.ToInt32(r["Codigo"]);
                    tipo            = r["cupomTipo"]?.ToString() ?? "PERCENTUAL";
                    valor           = r["cupomValor"] == DBNull.Value ? 0m : Convert.ToDecimal(r["cupomValor"]);
                    situacao        = r["Situacao"]?.ToString() ?? "A";
                    limiteUsos      = Convert.ToInt32(r["lim"]);
                    usosRealizados  = Convert.ToInt32(r["usos"]);
                    descricao       = r["descricao"]?.ToString() ?? "";
                    var v           = r["cupomValido_Ate"];
                    validade        = v == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(v);
                }

                // Extrai nome do produto prêmio da descrição: "Prêmio produto: 1x Média" → "Média"
                string produtoNome = null;
                if (tipo.ToUpper() == "PRODUTO" && !string.IsNullOrWhiteSpace(descricao))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(descricao, @"\d+x\s+(.+)$");
                    if (match.Success) produtoNome = match.Groups[1].Value.Trim();
                }

                string clienteUuid = GetSupabaseUuid("cliente", "Codigo", codigoCliente);
                if (string.IsNullOrWhiteSpace(clienteUuid))
                {
                    // UUID não está em cache local — tenta buscar direto no Supabase pelo codigo MySQL
                    try
                    {
                        var clienteRows = await GetAsync($"cliente?codigo=eq.{codigoCliente}&limit=1");
                        if (clienteRows.Count > 0)
                        {
                            clienteUuid = clienteRows[0]["id"]?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(clienteUuid))
                                SaveSupabaseUuid("cliente", "Codigo", codigoCliente, clienteUuid);
                        }
                    }
                    catch { }
                }
                if (string.IsNullOrWhiteSpace(clienteUuid))
                {
                    Logger.Log("SupabaseService", "SincronizarCupomFidelizacaoAsync", $"Cliente {codigoCliente} não encontrado no Supabase — cupom {codigoCupom} não sincronizado");
                    return;
                }

                string supaTipo = tipo.ToUpper() switch {
                    "VALOR"      => "fixo",
                    "PERCENTUAL" => "porcentagem",
                    "PRODUTO"    => "produto",
                    _            => "fixo"
                };
                var payload = new
                {
                    codigo          = codigoCupom,
                    valor,
                    tipo            = supaTipo,
                    validade        = validade.HasValue
                        ? validade.Value.Date.Add(new TimeSpan(23, 59, 59)).ToString("yyyy-MM-ddTHH:mm:ss")
                        : (string)null,
                    ativo           = situacao == "A",
                    cliente_id      = clienteUuid,
                    limite_usos     = limiteUsos,
                    usos_realizados = usosRealizados,
                    produto_nome    = produtoNome,
                    empresa_codigo  = UsarEmpresaCodigo ? _empresaCodigo : (string)null,
                };

                // Verifica se já existe no Supabase pelo código
                var existing = await GetAsync($"cupom?codigo=eq.{Uri.EscapeDataString(codigoCupom)}&limit=1");
                if (existing.Count > 0)
                {
                    string uuid = existing[0]["id"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(uuid))
                    {
                        await PatchAsync("cupom", $"id=eq.{uuid}", payload);
                        // Garante que o UUID do Supabase está salvo no MySQL para rastrear usos
                        SaveSupabaseUuid("cupom", "Codigo", codigoCupomMysql, uuid);
                    }
                }
                else
                {
                    var result = await PostAsync("cupom", payload);
                    string newUuid = result?["id"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(newUuid))
                        SaveSupabaseUuid("cupom", "Codigo", codigoCupomMysql, newUuid);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarCupomFidelizacaoAsync", $"Erro {codigoCupom}", ex);
            }
        }

        /// <summary>
        /// Sincroniza TODOS os cupôns de fidelização (FID%) ativos do MySQL para o Supabase,
        /// vinculando cada um ao cliente correto via cliente_id.
        /// </summary>
        public static async Task SincronizarTodosCuponsFidelizacaoAsync()
        {
            if (!SiteConectado) return;
            try
            {
                // Lê todos os cupôns FID com o código do cliente via historico_fidelizacao.
                // Parte 1: cupons PERCENTUAL/VALOR (fidCupomCodigo = cupomCodigo direto).
                // Parte 2: cupons PRODUTO (FIDP%) — historico armazena "PROD:X"; emparelha por
                //          proximidade de data (CriarCupomPremioProduto e RegistrarHistorico
                //          são chamados em sequência no mesmo request, sempre dentro de 60s).
                var registros = new List<(string codigo, int codigoCliente)>();
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT c.cupomCodigo, h.Codigo_Cliente " +
                    "FROM cupom c " +
                    "JOIN historico_fidelizacao h ON h.fidCupomCodigo = c.cupomCodigo " +
                    "WHERE c.Situacao='A' AND c.cupomCodigo LIKE 'FID%' " +
                    "AND c.cupomTipo <> 'PRODUTO' " +
                    "AND (c.cupomLimite_Usos = 0 OR c.cupomUsos_Realizados < c.cupomLimite_Usos) " +
                    "AND c.cupomValido_Ate >= CURDATE() " +
                    "UNION " +
                    "SELECT c.cupomCodigo, h.Codigo_Cliente " +
                    "FROM cupom c " +
                    "JOIN historico_fidelizacao h ON (" +
                    "    h.fidCupomCodigo LIKE 'PROD:%' " +
                    "    AND h.fidCupomCodigo <> 'PROD:OK' " +
                    "    AND ABS(TIMESTAMPDIFF(SECOND, c.cupomData_Cadastro, h.fidData)) <= 60) " +
                    "WHERE c.cupomTipo='PRODUTO' AND c.cupomCodigo LIKE 'FIDP%' " +
                    "AND c.Situacao='A' " +
                    "AND (c.cupomLimite_Usos = 0 OR c.cupomUsos_Realizados < c.cupomLimite_Usos) " +
                    "AND c.cupomValido_Ate >= CURDATE()", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                        registros.Add((r["cupomCodigo"].ToString(), Convert.ToInt32(r["Codigo_Cliente"])));
                }

                Logger.Log("SupabaseService", "SincronizarTodosCuponsFidelizacaoAsync", $"Cupons encontrados: {registros.Count}");
                foreach (var (codigo, codigoCliente) in registros)
                    await SincronizarCupomFidelizacaoAsync(codigo, codigoCliente);
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodosCuponsFidelizacaoAsync", "Erro", ex);
            }
        }

        // ── Bairro / Taxa Entrega ─────────────────────────────────────────────

        public static async Task<string> SincronizarBairroAsync(int codigoBairro)
        {
            try
            {
                string cep, situacao, nome, cidade;
                decimal taxa;
                int auxCodigo;

                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT COALESCE(baiCEP,'') AS baiCEP, baiTaxa_Entrega, Situacao, " +
                    "COALESCE(baiNome,'') AS baiNome, COALESCE(baiCidade,'') AS baiCidade, " +
                    "COALESCE(auxCodigo,1) AS auxCodigo " +
                    "FROM bairro WHERE Codigo=@c LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@c", codigoBairro);
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return "";
                    cep       = r["baiCEP"]?.ToString()?.Replace("-","").Trim() ?? "";
                    taxa      = r["baiTaxa_Entrega"] == DBNull.Value ? 0m : Convert.ToDecimal(r["baiTaxa_Entrega"]);
                    situacao  = r["Situacao"]?.ToString() ?? "A";
                    nome      = r["baiNome"]?.ToString() ?? "";
                    cidade    = r["baiCidade"]?.ToString() ?? "";
                    auxCodigo = r["auxCodigo"] == DBNull.Value ? 1 : Convert.ToInt32(r["auxCodigo"]);
                }

                // ── Sync to Supabase `bairro` table (mirrors MySQL structure) ──
                try
                {
                    object bairroPayload = UsarEmpresaCodigo
                        ? (object)new
                        {
                            codigo          = codigoBairro,
                            auxcodigo       = auxCodigo,
                            baicidade       = cidade,
                            bainome         = nome,
                            baitaxa_entrega = taxa,
                            situacao        = situacao == "A" ? "A" : "I",
                            empresa_codigo  = _empresaCodigo,
                        }
                        : (object)new
                        {
                            codigo          = codigoBairro,
                            auxcodigo       = auxCodigo,
                            baicidade       = cidade,
                            bainome         = nome,
                            baitaxa_entrega = taxa,
                            situacao        = situacao == "A" ? "A" : "I",
                        };
                    // Check if row exists by codigo (integer PK) + empresa_codigo
                    string bairroFilter = UsarEmpresaCodigo
                        ? $"bairro?codigo=eq.{codigoBairro}&empresa_codigo=eq.{Uri.EscapeDataString(_empresaCodigo)}&limit=1"
                        : $"bairro?codigo=eq.{codigoBairro}&limit=1";
                    var existentes = await GetAsync(bairroFilter);
                    if (existentes.Count > 0)
                        await PatchAsync("bairro", $"codigo=eq.{codigoBairro}{EmpresaFilter()}", bairroPayload);
                    else
                        await PostAsync("bairro", bairroPayload);
                }
                catch { } // non-critical — taxa_entrega sync below is the primary delivery fee store

                // ── Sync to `taxa_entrega` (CEP + bairro + cidade + valor) ──────
                {
                    object payload = UsarEmpresaCodigo
                        ? (object)new
                        {
                            cep    = string.IsNullOrWhiteSpace(cep) ? (object)null : cep,
                            bairro = nome,
                            cidade = cidade,
                            valor  = taxa,
                            empresa_codigo = _empresaCodigo,
                        }
                        : (object)new
                        {
                            cep    = string.IsNullOrWhiteSpace(cep) ? (object)null : cep,
                            bairro = nome,
                            cidade = cidade,
                            valor  = taxa,
                        };

                    // Always search by bairro+cidade (+ empresa_codigo) to avoid stale-UUID issue
                    var nomeEnc   = Uri.EscapeDataString(nome);
                    var cidadeEnc = Uri.EscapeDataString(cidade);
                    string taxaFilter = UsarEmpresaCodigo
                        ? $"taxa_entrega?bairro=eq.{nomeEnc}&cidade=eq.{cidadeEnc}&empresa_codigo=eq.{Uri.EscapeDataString(_empresaCodigo)}&limit=1"
                        : $"taxa_entrega?bairro=eq.{nomeEnc}&cidade=eq.{cidadeEnc}&limit=1";
                    var existing  = await GetAsync(taxaFilter);

                    if (situacao != "A")
                    {
                        if (existing.Count > 0)
                        {
                            var delUuid = existing[0]["id"]?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(delUuid))
                                await DeleteAsync("taxa_entrega", $"id=eq.{delUuid}");
                        }
                        SaveSupabaseUuid("bairro", "Codigo", codigoBairro, "");
                        return "";
                    }

                    if (existing.Count > 0)
                    {
                        var uuid = existing[0]["id"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(uuid))
                        {
                            await PatchAsync("taxa_entrega", $"id=eq.{uuid}", payload);
                            SaveSupabaseUuid("bairro", "Codigo", codigoBairro, uuid);
                        }
                    }
                    else
                    {
                        var result = await PostAsync("taxa_entrega", payload);
                        var newUuid = result?["id"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(newUuid))
                            SaveSupabaseUuid("bairro", "Codigo", codigoBairro, newUuid);
                    }
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
                    "SELECT Codigo FROM bairro WHERE Situacao='A'", conn))
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
                string nome, endereco, telefone, logoUrl, bannerUrl;
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT COALESCE(NULLIF(empNome_Fantasia,''), empNome) AS nome, " +
                    "empEndereco, empTelefone, COALESCE(empLogo_Url,'') AS empLogo_Url, " +
                    "COALESCE(empBanner_Url,'') AS empBanner_Url FROM empresa ORDER BY Codigo LIMIT 1", conn))
                {
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return;
                    nome      = r["nome"]?.ToString() ?? "";
                    endereco  = r["empEndereco"]?.ToString() ?? "";
                    telefone  = r["empTelefone"]?.ToString() ?? "";
                    logoUrl   = r["empLogo_Url"]?.ToString() ?? "";
                    bannerUrl = r["empBanner_Url"]?.ToString() ?? "";
                }

                var lojas = UsarEmpresaCodigo
                    ? await GetAsync($"loja?empresa_codigo=eq.{Uri.EscapeDataString(_empresaCodigo)}&limit=1")
                    : await GetAsync("loja?limit=1");

                if (lojas.Count == 0)
                {
                    // Linha não existe para esta empresa — cria
                    var insert = UsarEmpresaCodigo
                        ? (object)new { nome = string.IsNullOrWhiteSpace(nome) ? "Minha Loja" : nome, endereco, telefone, logo_url = logoUrl, banner_url = bannerUrl, empresa_codigo = _empresaCodigo }
                        : (object)new { nome = string.IsNullOrWhiteSpace(nome) ? "Minha Loja" : nome, endereco, telefone, logo_url = logoUrl, banner_url = bannerUrl };
                    try { await PostAsync("loja", insert); } catch { }
                    // Relê para pegar o id gerado
                    lojas = UsarEmpresaCodigo
                        ? await GetAsync($"loja?empresa_codigo=eq.{Uri.EscapeDataString(_empresaCodigo)}&limit=1")
                        : await GetAsync("loja?limit=1");
                    if (lojas.Count == 0) return;
                }

                string lojaId = lojas[0]["id"]?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(lojaId)) return;

                // 1. Envia nome/endereco/telefone + empresa_codigo
                object basePayload = UsarEmpresaCodigo
                    ? (object)new { nome = string.IsNullOrWhiteSpace(nome) ? (string)null : nome, endereco, telefone, empresa_codigo = _empresaCodigo }
                    : (object)new { nome = string.IsNullOrWhiteSpace(nome) ? (string)null : nome, endereco, telefone };
                await PatchAsync("loja", $"id=eq.{lojaId}", basePayload);

                // 2. Envia logo_url separadamente — se a coluna não existir, não quebra o sync principal
                //    Só envia URLs válidas (não envia caminhos de arquivo local)
                if (!string.IsNullOrWhiteSpace(logoUrl) && logoUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        await PatchAsync("loja", $"id=eq.{lojaId}", new { logo_url = logoUrl });
                    }
                    catch (Exception exLogo)
                    {
                        Logger.Log("SupabaseService", "SincronizarLojaAsync", $"Erro ao enviar logo_url: {exLogo.Message}");
                    }
                }

                // 3. Envia banner_url separadamente
                if (!string.IsNullOrWhiteSpace(bannerUrl) && bannerUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        await PatchAsync("loja", $"id=eq.{lojaId}", new { banner_url = bannerUrl });
                    }
                    catch (Exception exBanner)
                    {
                        Logger.Log("SupabaseService", "SincronizarLojaAsync", $"Erro ao enviar banner_url: {exBanner.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarLojaAsync", "Erro", ex);
            }
        }

        // ── Reset Supabase ────────────────────────────────────────────────────

        /// <summary>
        /// Remove todos os dados desta empresa no Supabase.
        /// Usa o empresa_codigo como filtro para não afetar outros clientes.
        /// Se empresa_codigo não estiver configurado, apaga tudo (TRUNCATE via DELETE sem filtro).
        /// </summary>
        public static async Task LimparTudoSupabaseAsync()
        {
            if (!SiteConectado) return;

            string empFilter = UsarEmpresaCodigo
                ? $"empresa_codigo=eq.{Uri.EscapeDataString(_empresaCodigo)}"
                : "id=neq.00000000-0000-0000-0000-000000000000"; // sem filtro de empresa → apaga tudo

            // Ordem importa: dependentes antes dos pais
            string[] tabelas = {
                "itens_pedido_web",
                "pedido_web",
                "enderecos_salvo",
                "cliente",
                "cupom",
                "complemento",
                "complemento_grupo",
                "adicional",
                "mercadoria",
                "grupo_mercadoria",
                "forma_pagamento",
                "taxa_entrega",
                "bairro",
                "loja",
            };

            foreach (var tabela in tabelas)
            {
                try { await DeleteAsync(tabela, empFilter); }
                catch { /* ignora tabelas que não existem ou falhas pontuais */ }
            }

            // Reseta o cache de UUID após limpar
            ResetSyncCache();
        }

        // ── Web Orders Import ─────────────────────────────────────────────────

        public static async Task<List<PedidoWebSupabase>> BuscarPedidosPendentesAsync()
        {
            var result = new List<PedidoWebSupabase>();
            if (!SiteConectado) return result;
            try
            {
                // Accept any order that hasn't been processed yet (website may use "pendente" or "banda")
                // Usa OR para capturar tanto pedidos com empresa_codigo correto quanto os com NULL (legados)
                string empOr = UsarEmpresaCodigo
                    ? $"&or=(empresa_codigo.eq.{Uri.EscapeDataString(_empresaCodigo)},empresa_codigo.is.null)"
                    : "";
                JArray arr;
                try
                {
                    arr = await GetAsync($"pedido_web?status=in.(pendente,banda,novo,aguardando){empOr}&order=created_at.asc&limit=50");
                }
                catch
                {
                    // Coluna empresa_codigo ainda não existe no banco — busca sem filtro
                    arr = await GetAsync("pedido_web?status=in.(pendente,banda,novo,aguardando)&order=created_at.asc&limit=50");
                }
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
                // Try both common table names used by the website
                JArray arr = null;
                foreach (var tbl in new[] { "itens_pedido_web", "itens_pedido", "pedido_itens", "itens" })
                {
                    try
                    {
                        arr = await GetAsync($"{tbl}?pedido_id=eq.{pedidoId}");
                        if (arr.Count > 0) break;
                    }
                    catch { }
                }
                if (arr == null) return result;

                foreach (JObject item in arr)
                {
                    // Build readable obs from complementos_json + adicionais_json
                    var obsBuilder = new System.Text.StringBuilder();
                    var obsBase = (item["observacao"] ?? item["obs"])?.ToString() ?? "";
                    try
                    {
                        var compJson = item["complementos_json"];
                        if (compJson != null && compJson.Type != Newtonsoft.Json.Linq.JTokenType.Null)
                        {
                            var compArr = compJson as Newtonsoft.Json.Linq.JArray
                                ?? Newtonsoft.Json.Linq.JArray.Parse(compJson.ToString());
                            foreach (JObject c in compArr)
                            {
                                var grpNome  = c["grupoNome"]?.ToString() ?? c["grupo_nome"]?.ToString() ?? "";
                                var itemNome = c["itemNome"]?.ToString()  ?? c["item_nome"]?.ToString()  ?? "";
                                if (!string.IsNullOrWhiteSpace(itemNome))
                                    obsBuilder.AppendLine($"{grpNome}: {itemNome}");
                            }
                        }
                    }
                    catch { }
                    try
                    {
                        var adicJson = item["adicionais_json"];
                        if (adicJson != null && adicJson.Type != Newtonsoft.Json.Linq.JTokenType.Null)
                        {
                            var adicArr = adicJson as Newtonsoft.Json.Linq.JArray
                                ?? Newtonsoft.Json.Linq.JArray.Parse(adicJson.ToString());
                            foreach (JObject a in adicArr)
                            {
                                var nome = a["nome"]?.ToString() ?? "";
                                var qty  = a["quantity"]?.ToObject<int>() ?? 1;
                                if (!string.IsNullOrWhiteSpace(nome))
                                    obsBuilder.AppendLine($"+ {(qty > 1 ? qty + "x " : "")}{nome}");
                            }
                        }
                    }
                    catch { }
                    string obsComposta = obsBuilder.ToString().TrimEnd();
                    if (!string.IsNullOrWhiteSpace(obsBase))
                        obsComposta = string.IsNullOrWhiteSpace(obsComposta) ? obsBase : obsBase + "\n" + obsComposta;

                    result.Add(new ItemPedidoWebSupabase
                    {
                        Id            = item["id"]?.ToString()              ?? "",
                        PedidoId      = item["pedido_id"]?.ToString()       ?? "",
                        MercadoriaId  = (item["mercadoria_id"] ?? item["produto_id"])?.ToString() ?? "",
                        Quantidade    = (item["quantidade"] ?? item["qtde"])?.ToObject<decimal>()    ?? 1m,
                        PrecoUnitario = (item["preco_unitario"] ?? item["preco"])?.ToObject<decimal>() ?? 0m,
                        Observacao    = obsComposta,
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
                return MapearClienteSupabase(item);
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "BuscarClienteSupabaseAsync", "Erro", ex);
                return null;
            }
        }

        private static ClienteSupabase MapearClienteSupabase(JObject item) => new ClienteSupabase
        {
            Id          = item["id"]?.ToString()           ?? "",
            Nome        = item["nome"]?.ToString()         ?? "",
            Telefone    = item["telefone"]?.ToString()     ?? "",
            CpfCnpj     = item["cpf_cnpj"]?.ToString()    ?? "",
            Endereco    = item["endereco"]?.ToString()     ?? "",
            Numero      = item["numero"]?.ToString()       ?? "",
            Complemento = item["complemento"]?.ToString() ?? "",
            Bairro      = item["bairro"]?.ToString()       ?? "",
            Cidade      = item["cidade"]?.ToString()       ?? "",
            Uf          = (item["uf"] ?? item["estado"])?.ToString() ?? "",
            Cep         = item["cep"]?.ToString()          ?? "",
            Email       = item["email"]?.ToString()        ?? "",
        };

        /// <summary>Fetches all clients from Supabase and upserts them into local MySQL. Returns a diagnostic string.</summary>
        public static async Task<string> ImportarClientesSupabaseAsync()
        {
            var sb = new System.Text.StringBuilder();
            try
            {
                // Fetch only clients of this empresa — paginate in batches of 1000
                var todos = new List<JObject>();
                int offset = 0;
                string empQ = UsarEmpresaCodigo
                    ? $"empresa_codigo=eq.{Uri.EscapeDataString(_empresaCodigo)}&"
                    : "";
                while (true)
                {
                    var lote = await GetAsync($"cliente?{empQ}order=nome.asc&limit=1000&offset={offset}");
                    if (lote.Count == 0) break;
                    foreach (JObject j in lote) todos.Add(j);
                    if (lote.Count < 1000) break;
                    offset += 1000;
                }
                sb.AppendLine($"Encontrados no Supabase: {todos.Count} cliente(s).");
                int criados = 0, atualizados = 0, erros = 0;

                foreach (JObject item in todos)
                {
                    try
                    {
                        var sc = MapearClienteSupabase(item);
                        if (string.IsNullOrWhiteSpace(sc.Nome)) continue;

                        string cpf = SanitizarDigitos(sc.CpfCnpj ?? "");
                        string tel = SanitizarTelefone(sc.Telefone ?? "");
                        string telMasked = FormatarTelefone(tel);
                        string cpfMasked = FormatarCpf(cpf);
                        string cep  = SanitizarDigitos(sc.Cep ?? "");

                        using var conn = AbrirMysql();

                        // Find existing by UUID, then CPF, then phone
                        int existCod = 0;
                        if (!string.IsNullOrWhiteSpace(sc.Id))
                        {
                            using var q = new MySqlCommand("SELECT Codigo FROM cliente WHERE supabase_uuid=@u LIMIT 1", conn);
                            q.Parameters.AddWithValue("@u", sc.Id);
                            var r0 = q.ExecuteScalar();
                            if (r0 != null && r0 != DBNull.Value) existCod = Convert.ToInt32(r0);
                        }
                        if (existCod == 0 && !string.IsNullOrWhiteSpace(cpf) && cpf.Length >= 11)
                        {
                            using var q = new MySqlCommand(
                                "SELECT Codigo FROM cliente WHERE " +
                                "REPLACE(REPLACE(REPLACE(REPLACE(clieCPF_CNPJ_,'.',''),'-',''),'/',''),' ','')=@c AND Situacao='A' LIMIT 1", conn);
                            q.Parameters.AddWithValue("@c", cpf);
                            var r1 = q.ExecuteScalar();
                            if (r1 != null && r1 != DBNull.Value) existCod = Convert.ToInt32(r1);
                        }
                        if (existCod == 0 && !string.IsNullOrWhiteSpace(tel) && tel.Length >= 8)
                        {
                            string tail = tel.Substring(tel.Length - 8);
                            using var q = new MySqlCommand(
                                "SELECT Codigo FROM cliente WHERE (" +
                                "REPLACE(REPLACE(REPLACE(REPLACE(clieCelular,'(',''),')',''),'-',''),' ','') LIKE @t OR " +
                                "REPLACE(REPLACE(REPLACE(REPLACE(clieTelefone,'(',''),')',''),'-',''),' ','') LIKE @t) " +
                                "AND Situacao='A' LIMIT 1", conn);
                            q.Parameters.AddWithValue("@t", "%" + tail);
                            var r2 = q.ExecuteScalar();
                            if (r2 != null && r2 != DBNull.Value) existCod = Convert.ToInt32(r2);
                        }

                        if (existCod > 0)
                        {
                            // Update existing record with Supabase data, filling any empty fields
                            using var upd = new MySqlCommand(@"
                                UPDATE cliente SET
                                    clieNome_RazaoSocial = @nome,
                                    clieCelular  = CASE WHEN COALESCE(clieCelular,'')='' THEN @cel ELSE clieCelular END,
                                    clieTelefone = CASE WHEN COALESCE(clieTelefone,'')='' THEN @cel ELSE clieTelefone END,
                                    clieCPF_CNPJ_= CASE WHEN COALESCE(clieCPF_CNPJ_,'')='' THEN @cpf ELSE clieCPF_CNPJ_ END,
                                    clieEmail    = CASE WHEN COALESCE(clieEmail,'')='' THEN @email ELSE clieEmail END,
                                    clieCEP      = CASE WHEN COALESCE(clieCEP,'')='' THEN @cep ELSE clieCEP END,
                                    clieEndereco = CASE WHEN COALESCE(clieEndereco,'')='' THEN @end ELSE clieEndereco END,
                                    clieNumero   = CASE WHEN COALESCE(clieNumero,'')='' THEN @num ELSE clieNumero END,
                                    clieComplemento = CASE WHEN COALESCE(clieComplemento,'')='' THEN @comp ELSE clieComplemento END,
                                    clieBairro   = CASE WHEN COALESCE(clieBairro,'')='' THEN @bairro ELSE clieBairro END,
                                    clieCidade   = CASE WHEN COALESCE(clieCidade,'')='' THEN @cidade ELSE clieCidade END,
                                    clieEstado   = CASE WHEN COALESCE(clieEstado,'')='' THEN @uf ELSE clieEstado END,
                                    supabase_uuid= CASE WHEN COALESCE(supabase_uuid,'')='' THEN @uuid ELSE supabase_uuid END
                                WHERE Codigo=@cod", conn);
                            upd.Parameters.AddWithValue("@nome",   sc.Nome);
                            upd.Parameters.AddWithValue("@cel",    telMasked);
                            upd.Parameters.AddWithValue("@cpf",    cpfMasked);
                            upd.Parameters.AddWithValue("@email",  sc.Email ?? "");
                            upd.Parameters.AddWithValue("@cep",    cep);
                            upd.Parameters.AddWithValue("@end",    sc.Endereco ?? "");
                            upd.Parameters.AddWithValue("@num",    sc.Numero ?? "");
                            upd.Parameters.AddWithValue("@comp",   sc.Complemento ?? "");
                            upd.Parameters.AddWithValue("@bairro", sc.Bairro ?? "");
                            upd.Parameters.AddWithValue("@cidade", sc.Cidade ?? "");
                            upd.Parameters.AddWithValue("@uf",     sc.Uf ?? "");
                            upd.Parameters.AddWithValue("@uuid",   sc.Id);
                            upd.Parameters.AddWithValue("@cod",    existCod);
                            upd.ExecuteNonQuery();
                            atualizados++;
                        }
                        else
                        {
                            // Create new
                            int nextCod = ProximoCodigo("cliente", conn);
                            int nextAux = ProximoAuxCodigo("cliente", conn);
                            using var ins = new MySqlCommand(@"
                                INSERT INTO cliente
                                (auxCodigo, Codigo, clieNome_RazaoSocial, clieCelular, clieTelefone,
                                 clieCPF_CNPJ_, clieEmail, clieCEP, clieEndereco, clieNumero,
                                 clieComplemento, clieBairro, clieCidade, clieEstado,
                                 clieData_Cadastro, Situacao, Status_Transmissao, supabase_uuid)
                                VALUES(@aux,@cod,@nome,@cel,@cel,@cpf,@email,@cep,@end,@num,
                                       @comp,@bairro,@cidade,@uf,NOW(),'A','N',@uuid)", conn);
                            ins.Parameters.AddWithValue("@aux",    nextAux);
                            ins.Parameters.AddWithValue("@cod",    nextCod);
                            ins.Parameters.AddWithValue("@nome",   sc.Nome);
                            ins.Parameters.AddWithValue("@cel",    telMasked);
                            ins.Parameters.AddWithValue("@cpf",    cpfMasked);
                            ins.Parameters.AddWithValue("@email",  sc.Email ?? "");
                            ins.Parameters.AddWithValue("@cep",    cep);
                            ins.Parameters.AddWithValue("@end",    sc.Endereco ?? "");
                            ins.Parameters.AddWithValue("@num",    sc.Numero ?? "");
                            ins.Parameters.AddWithValue("@comp",   sc.Complemento ?? "");
                            ins.Parameters.AddWithValue("@bairro", sc.Bairro ?? "");
                            ins.Parameters.AddWithValue("@cidade", sc.Cidade ?? "");
                            ins.Parameters.AddWithValue("@uf",     sc.Uf ?? "");
                            ins.Parameters.AddWithValue("@uuid",   sc.Id);
                            ins.ExecuteNonQuery();
                            criados++;
                        }
                    }
                    catch (Exception ex2) { erros++; Logger.Log("SupabaseService","ImportarClientesSupabaseAsync","Erro cliente",ex2); }
                }
                sb.AppendLine($"Criados: {criados} | Atualizados: {atualizados} | Erros: {erros}");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"ERRO GERAL: {ex.Message}");
            }
            return sb.ToString();
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

        /// <summary>Manual pull: returns diagnostic message showing how many were imported and any errors.</summary>
        public static async Task<string> ImportarPedidosManuaisAsync()
        {
            var sb = new StringBuilder();
            try
            {
                var pedidos = await BuscarPedidosPendentesAsync();
                sb.AppendLine($"Encontrados no Supabase: {pedidos.Count} pedido(s) pendente(s).");
                int ok = 0, erros = 0;
                foreach (var p in pedidos)
                {
                    var itens = await BuscarItensPedidoAsync(p.Id);
                    sb.AppendLine($"  Pedido {p.Id[..8]}... | status={p.Status} | itens={itens.Count}");
                    var erro = await ImportarPedidoAsync(p);
                    if (string.IsNullOrEmpty(erro))
                    {
                        ok++;
                        await AtualizarStatusPedidoWebAsync(p.Id, "recebido");
                    }
                    else
                    {
                        erros++;
                        sb.AppendLine($"    ERRO: {erro}");
                    }
                }
                sb.AppendLine($"\nImportados: {ok} | Erros: {erros}");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"ERRO GERAL: {ex.Message}");
            }
            return sb.ToString();
        }
        public static async Task<string> ImportarPedidoAsync(PedidoWebSupabase supaPedido)
        {
            try
            {
                // Check if already imported — if so, ensure Supabase status is updated and return
                using (var conn = AbrirMysql())
                using (var chk  = new MySqlCommand(
                    "SELECT COUNT(*) FROM pedido_web WHERE pediSupabase_Id=@sid", conn))
                {
                    chk.Parameters.AddWithValue("@sid", supaPedido.Id);
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                    {
                        // Ensure Supabase status is "recebido" so it stops appearing in polls
                        try { await AtualizarStatusPedidoWebAsync(supaPedido.Id, "recebido"); } catch { }
                        return "JA_IMPORTADO"; // already imported — caller must not count as new
                    }
                }

                // Fetch items and client
                var itens       = await BuscarItensPedidoAsync(supaPedido.Id);
                ClienteSupabase supaCliente = null;
                if (!string.IsNullOrWhiteSpace(supaPedido.ClienteId))
                    supaCliente = await BuscarClienteSupabaseAsync(supaPedido.ClienteId);

                // Enrich client address from enderecos_salvo (structured fields: cep, bairro, cidade, uf, complemento)
                if (supaCliente != null && string.IsNullOrWhiteSpace(supaCliente.Endereco))
                {
                    try
                    {
                        string telBusca = SanitizarTelefone(supaCliente.Telefone ?? "");
                        if (!string.IsNullOrWhiteSpace(telBusca))
                        {
                            var endArr = await GetAsync(
                                $"enderecos_salvo?whatsapp=eq.{Uri.EscapeDataString(telBusca)}&order=created_at.desc&limit=1");
                            if (endArr.Count > 0)
                            {
                                var e = (JObject)endArr[0];
                                supaCliente.Endereco    = e["endereco"]?.ToString() ?? "";
                                supaCliente.Numero      = e["numero"]?.ToString() ?? "";
                                supaCliente.Complemento = e["complemento"]?.ToString() ?? "";
                                supaCliente.Bairro      = e["bairro"]?.ToString() ?? "";
                                supaCliente.Cidade      = e["cidade"]?.ToString() ?? "";
                                supaCliente.Uf          = e["uf"]?.ToString() ?? "";
                                supaCliente.Cep         = SanitizarDigitos(e["cep"]?.ToString() ?? "");
                            }
                        }
                    }
                    catch { }
                }

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
                     pediData_Lancamento, Codigo_Cliente, pediSupabase_Id, Situacao)
                    VALUES(@aux, @cod, @num, @nomeCli, @tel, 0, @tipoEnt, @formaPag, 0,
                           @sub, @taxa, @desc, @total, @troco, @end, '',
                           @dtLanc, @codCli, @supId, 'A')";

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
                    cmdP.Parameters.AddWithValue("@troco",  (object)(supaPedido.Troco ?? 0m));
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
                         itpwDesconto_Pct, Situacao)
                        VALUES(@aux, @cod, @pedCod, @merc, @nome, @qtde, @pu, @sub, @obs, 0, 'A')",
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

                    // Decrement stock if product controls stock
                    if (codMerc > 0)
                    {
                        using var cmdEst = new MySqlCommand(
                            "UPDATE mercadoria SET mercEstoque_Atual = mercEstoque_Atual - @qtde " +
                            "WHERE Codigo=@c AND mercControla_Estoque=1 AND mercEstoque_Atual > 0",
                            connM, trans);
                        cmdEst.Parameters.AddWithValue("@qtde", it.Quantidade);
                        cmdEst.Parameters.AddWithValue("@c",    codMerc);
                        cmdEst.ExecuteNonQuery();
                    }
                }

                trans.Commit();

                // Incrementa totais do cliente (gasto mensal, pedidos mensais, gasto total)
                // Igual ao que acontece com pedidos locais ao serem pagos
                if (codigoClienteLocal > 0)
                {
                    try { new DAL.ClienteDAL().IncrementarTotais(codigoClienteLocal, supaPedido.Total); }
                    catch { }

                    // Verificar fidelização — mesmo comportamento dos pedidos locais
                    try
                    {
                        int pedCod = nextCod; int cliCod = codigoClienteLocal;
                        var premioMsg = new BLL.FidelizacaoBLL().VerificarEDispararPremio(cliCod, pedCod);
                        if (!string.IsNullOrEmpty(premioMsg))
                            Logger.Log("SupabaseService", "ImportarPedidoAsync", "Fidelizacao: " + premioMsg);
                    }
                    catch { }
                }

                // Register coupon usage if order had a coupon
                if (!string.IsNullOrWhiteSpace(supaPedido.CupomId))
                    RegistrarUsoCupomLocal(supaPedido.CupomId);

                // Send WhatsApp confirmation to client
                // Priority: 1) client phone from Supabase cliente table
                //           2) enderecos_salvo.whatsapp filtered by cliente_id
                //           3) local MySQL client phone
                string telWpp = SanitizarTelefone(supaCliente?.Telefone ?? "");
                if (string.IsNullOrWhiteSpace(telWpp) && !string.IsNullOrWhiteSpace(supaPedido.ClienteId))
                {
                    // enderecos_salvo stores whatsapp = customer phone, linked by same clienteId
                    try
                    {
                        // enderecos_salvo has whatsapp col = phone; no direct cliente_id FK,
                        // but the phone used on the site is the same as whatsapp field
                        // Try fetching from cliente table directly with telefone field
                        var cliArr = await GetAsync(
                            $"cliente?id=eq.{Uri.EscapeDataString(supaPedido.ClienteId)}&select=telefone&limit=1");
                        if (cliArr.Count > 0)
                            telWpp = SanitizarTelefone(((JObject)cliArr[0])["telefone"]?.ToString() ?? "");
                    }
                    catch { }
                }
                if (string.IsNullOrWhiteSpace(telWpp) && codigoClienteLocal > 0)
                {
                    // Try to load from local MySQL
                    try
                    {
                        using var cW = AbrirMysql();
                        using var cCmd = new MySqlCommand(
                            "SELECT COALESCE(clieCelular, clieTelefone, '') FROM cliente WHERE Codigo=@c LIMIT 1", cW);
                        cCmd.Parameters.AddWithValue("@c", codigoClienteLocal);
                        telWpp = SanitizarTelefone(cCmd.ExecuteScalar()?.ToString() ?? "");
                    }
                    catch { }
                }
                string nomeWpp   = supaCliente?.Nome ?? "Cliente";
                string numeroWpp = numero;
                decimal totalWpp = supaPedido.Total;
                BLL.WhatsAppService.NotificarPedidoWebRecebido(telWpp, nomeWpp, numeroWpp, totalWpp);

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
                string cpf      = SanitizarDigitos(supaCliente.CpfCnpj?.ToString() ?? "");
                string tel      = SanitizarTelefone(supaCliente.Telefone?.ToString() ?? "");
                string supaUuid = supaCliente.Id ?? "";
                string nome     = supaCliente.Nome?.Trim() ?? "";

                using var conn = AbrirMysql();

                // ── 1. Search by Supabase UUID (fastest — exact match) ─────
                if (!string.IsNullOrWhiteSpace(supaUuid))
                {
                    using var chkU = new MySqlCommand(
                        "SELECT Codigo FROM cliente WHERE supabase_uuid=@u AND Situacao='A' LIMIT 1", conn);
                    chkU.Parameters.AddWithValue("@u", supaUuid);
                    var existU = chkU.ExecuteScalar();
                    if (existU != null && existU != DBNull.Value)
                    {
                        int cod = Convert.ToInt32(existU);
                        TentarAtualizarEnderecoCliente(conn, cod, supaCliente, enderecoEntrega);
                        return cod;
                    }
                }

                // ── 2. Search by CPF (digits-only) ────────────────────────
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
                        // Save UUID so future lookups use path 1
                        if (!string.IsNullOrWhiteSpace(supaUuid))
                            SalvarUuidCliente(conn, cod, supaUuid);
                        TentarAtualizarEnderecoCliente(conn, cod, supaCliente, enderecoEntrega);
                        return cod;
                    }
                }

                // ── 3. Search by phone (last 8 digits) ────────────────────
                if (!string.IsNullOrWhiteSpace(tel) && tel.Length >= 8)
                {
                    string tail = tel.Substring(tel.Length - 8);
                    using var chk2 = new MySqlCommand(
                        "SELECT Codigo FROM cliente WHERE (" +
                        "REPLACE(REPLACE(REPLACE(REPLACE(clieCelular,'(',''),')',''),'-',''),' ','') LIKE @t OR " +
                        "REPLACE(REPLACE(REPLACE(REPLACE(clieTelefone,'(',''),')',''),'-',''),' ','') LIKE @t) " +
                        "AND Situacao='A' LIMIT 1", conn);
                    chk2.Parameters.AddWithValue("@t", "%" + tail);
                    var existing2 = chk2.ExecuteScalar();
                    if (existing2 != null && existing2 != DBNull.Value)
                    {
                        int cod2 = Convert.ToInt32(existing2);
                        if (!string.IsNullOrWhiteSpace(supaUuid))
                            SalvarUuidCliente(conn, cod2, supaUuid);
                        TentarAtualizarEnderecoCliente(conn, cod2, supaCliente, enderecoEntrega);
                        return cod2;
                    }
                }

                // ── 4. Search by name + phone (no-CPF fallback) ───────────
                if (!string.IsNullOrWhiteSpace(nome) && !string.IsNullOrWhiteSpace(tel) && tel.Length >= 8)
                {
                    string tail4 = tel.Substring(tel.Length - 8);
                    using var chk3 = new MySqlCommand(
                        "SELECT Codigo FROM cliente WHERE " +
                        "clieNome_RazaoSocial=@nome AND (" +
                        "REPLACE(REPLACE(REPLACE(REPLACE(clieCelular,'(',''),')',''),'-',''),' ','') LIKE @t OR " +
                        "REPLACE(REPLACE(REPLACE(REPLACE(clieTelefone,'(',''),')',''),'-',''),' ','') LIKE @t) " +
                        "AND Situacao='A' LIMIT 1", conn);
                    chk3.Parameters.AddWithValue("@nome", nome);
                    chk3.Parameters.AddWithValue("@t",    "%" + tail4);
                    var existing3 = chk3.ExecuteScalar();
                    if (existing3 != null && existing3 != DBNull.Value)
                    {
                        int cod3 = Convert.ToInt32(existing3);
                        if (!string.IsNullOrWhiteSpace(supaUuid))
                            SalvarUuidCliente(conn, cod3, supaUuid);
                        TentarAtualizarEnderecoCliente(conn, cod3, supaCliente, enderecoEntrega);
                        return cod3;
                    }
                }

                // ── 5. Create new client ──────────────────────────────────
                int nextCod = ProximoCodigo("cliente", conn);
                int nextAux = ProximoAuxCodigo("cliente", conn);
                string cpfMasked = FormatarCpf(cpf);
                string telMasked = FormatarTelefone(tel);

                string rua = supaCliente.Endereco, numero = supaCliente.Numero,
                       complemento = supaCliente.Complemento,
                       bairro = supaCliente.Bairro, cidade = supaCliente.Cidade,
                       uf = supaCliente.Uf,
                       cep = SanitizarDigitos(supaCliente.Cep ?? "");
                if (string.IsNullOrWhiteSpace(rua) && !string.IsNullOrWhiteSpace(enderecoEntrega))
                    (rua, numero, bairro, cidade) = ParseEndereco(enderecoEntrega);

                using var ins = new MySqlCommand(@"
                    INSERT INTO cliente
                    (auxCodigo, Codigo, clieNome_RazaoSocial, clieCelular, clieTelefone,
                     clieCPF_CNPJ_, clieEmail, clieCEP, clieEndereco, clieNumero,
                     clieComplemento, clieBairro, clieCidade, clieEstado,
                     clieData_Cadastro, Situacao, Status_Transmissao, supabase_uuid)
                    VALUES(@aux, @cod, @nome, @cel, @tel, @cpf, @email, @cep, @end, @num,
                           @comp, @bairro, @cidade, @uf, NOW(), 'A', 'N', @uuid)", conn);
                ins.Parameters.AddWithValue("@aux",    nextAux);
                ins.Parameters.AddWithValue("@cod",    nextCod);
                ins.Parameters.AddWithValue("@nome",   nome.Length > 0 ? nome : "Cliente Web");
                ins.Parameters.AddWithValue("@cel",    telMasked);
                ins.Parameters.AddWithValue("@tel",    telMasked);
                ins.Parameters.AddWithValue("@cpf",    cpfMasked);
                ins.Parameters.AddWithValue("@email",  supaCliente.Email ?? "");
                ins.Parameters.AddWithValue("@cep",    cep);
                ins.Parameters.AddWithValue("@end",    rua ?? "");
                ins.Parameters.AddWithValue("@num",    numero ?? "");
                ins.Parameters.AddWithValue("@comp",   complemento ?? "");
                ins.Parameters.AddWithValue("@bairro", bairro ?? "");
                ins.Parameters.AddWithValue("@cidade", cidade ?? "");
                ins.Parameters.AddWithValue("@uf",     uf ?? "");
                ins.Parameters.AddWithValue("@uuid",   supaUuid);
                ins.ExecuteNonQuery();

                // Send WhatsApp welcome message for new web client
                BLL.WhatsAppService.NotificarNovoCadastroWeb(tel, nome.Length > 0 ? nome : "Cliente");

                return nextCod;
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "BuscarOuCriarClienteLocal", "Erro", ex);
                return 0;
            }
        }

        private static void SalvarUuidCliente(MySqlConnection conn, int codigo, string uuid)
        {
            try
            {
                using var cmd = new MySqlCommand(
                    "UPDATE cliente SET supabase_uuid=@u WHERE Codigo=@c AND (supabase_uuid IS NULL OR supabase_uuid='')", conn);
                cmd.Parameters.AddWithValue("@u", uuid);
                cmd.Parameters.AddWithValue("@c", codigo);
                cmd.ExecuteNonQuery();
            }
            catch { }
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
                       complemento = supaCliente.Complemento,
                       bairro = supaCliente.Bairro, cidade = supaCliente.Cidade,
                       uf = supaCliente.Uf,
                       cep = SanitizarDigitos(supaCliente.Cep ?? "");
                if (string.IsNullOrWhiteSpace(rua) && !string.IsNullOrWhiteSpace(enderecoEntrega))
                    (rua, numero, bairro, cidade) = ParseEndereco(enderecoEntrega);
                if (string.IsNullOrWhiteSpace(rua)) return;

                using var upd = new MySqlCommand(@"
                    UPDATE cliente SET clieEndereco=@end, clieNumero=@num,
                        clieComplemento=@comp, clieBairro=@bairro, clieCidade=@cidade,
                        clieEstado=@uf, clieCEP=@cep
                    WHERE Codigo=@c", conn);
                upd.Parameters.AddWithValue("@end",    rua);
                upd.Parameters.AddWithValue("@num",    numero ?? "");
                upd.Parameters.AddWithValue("@comp",   complemento ?? "");
                upd.Parameters.AddWithValue("@bairro", bairro ?? "");
                upd.Parameters.AddWithValue("@cidade", cidade ?? "");
                upd.Parameters.AddWithValue("@uf",     uf ?? "");
                upd.Parameters.AddWithValue("@cep",    cep);
                upd.Parameters.AddWithValue("@c",      codigoCli);
                upd.ExecuteNonQuery();
            }
            catch { /* non-critical */ }
        }

        private static (string rua, string numero, string bairro, string cidade) ParseEndereco(string end)
        {
            // Formats produced by the site:
            // New:  "Rua X, 123 - Complemento - Bairro - Cidade - UF"
            // New (no compl): "Rua X, 123 - Bairro - Cidade - UF"
            // Old:  "Rua X, 123 - Bairro, Cidade"
            string rua = "", numero = "", bairro = "", cidade = "";
            if (string.IsNullOrWhiteSpace(end)) return (rua, numero, bairro, cidade);

            var parts = end.Split(new[] { " - " }, StringSplitOptions.None);

            // Part 0: "Rua X, 123"
            var ruaPart = parts[0].Trim();
            var comma = ruaPart.LastIndexOf(',');
            if (comma >= 0) { rua = ruaPart.Substring(0, comma).Trim(); numero = ruaPart.Substring(comma + 1).Trim(); }
            else rua = ruaPart;

            if (parts.Length == 2)
            {
                // "Rua X, 123 - Bairro, Cidade" (old format)
                var seg = parts[1].Trim();
                var ci = seg.IndexOf(',');
                if (ci >= 0) { bairro = seg.Substring(0, ci).Trim(); cidade = seg.Substring(ci + 1).Trim(); }
                else bairro = seg;
            }
            else if (parts.Length == 3)
            {
                // "Rua X, 123 - Bairro - Cidade" or "Rua X, 123 - Complemento - Bairro"
                bairro = parts[1].Trim();
                cidade = parts[2].Trim();
            }
            else if (parts.Length >= 4)
            {
                // "Rua X, 123 - Complemento - Bairro - Cidade" or "- Cidade - UF"
                // Last part may be UF (2 chars), second-to-last is Cidade, third-to-last is Bairro
                int last = parts.Length - 1;
                string possibleUf = parts[last].Trim();
                if (possibleUf.Length <= 2)
                {
                    cidade = parts[last - 1].Trim();
                    bairro = parts[last - 2].Trim();
                }
                else
                {
                    cidade = parts[last].Trim();
                    bairro = parts[last - 1].Trim();
                }
            }
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
                string nome, celular, cpf, cep, endereco, numero, complemento, bairro, cidade, uf, email;
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT clieNome_RazaoSocial, clieCelular, clieTelefone, clieCPF_CNPJ_, " +
                    "clieCEP, clieEndereco, clieNumero, clieComplemento, clieBairro, clieCidade, " +
                    "COALESCE(clieEstado,'') AS clieEstado, COALESCE(clieEmail,'') AS clieEmail, Situacao " +
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
                    endereco    = r["clieEndereco"]?.ToString() ?? "";
                    numero      = r["clieNumero"]?.ToString() ?? "";
                    complemento = r["clieComplemento"]?.ToString() ?? "";
                    bairro   = r["clieBairro"]?.ToString() ?? "";
                    cidade   = r["clieCidade"]?.ToString() ?? "";
                    uf       = r["clieEstado"]?.ToString() ?? "";
                    email    = r["clieEmail"]?.ToString() ?? "";
                }

                if (string.IsNullOrWhiteSpace(nome)) return "";

                string uuid = GetSupabaseUuid("cliente", "Codigo", codigoCliente);

                // Verify cached UUID still exists in Supabase
                if (!string.IsNullOrWhiteSpace(uuid))
                {
                    try
                    {
                        var check = await GetAsync($"cliente?id=eq.{uuid}&limit=1");
                        if (check.Count == 0) { SaveSupabaseUuid("cliente", "Codigo", codigoCliente, ""); uuid = ""; }
                    }
                    catch { /* keep UUID on network error */ }
                }

                // Search Supabase for existing record to avoid duplicates
                if (string.IsNullOrWhiteSpace(uuid))
                {
                    // 1. By phone (most reliable — phone is stored as numeric)
                    if (!string.IsNullOrWhiteSpace(celular) && long.TryParse(celular, out long telLookup) && telLookup > 0)
                    {
                        var byTel = await GetAsync($"cliente?telefone=eq.{telLookup}&limit=1");
                        if (byTel.Count > 0)
                        {
                            uuid = byTel[0]["id"]?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(uuid))
                                SaveSupabaseUuid("cliente", "Codigo", codigoCliente, uuid);
                        }
                    }
                    // 2. By nome (last resort)
                    if (string.IsNullOrWhiteSpace(uuid) && !string.IsNullOrWhiteSpace(nome))
                    {
                        var byNome = await GetAsync($"cliente?nome=eq.{Uri.EscapeDataString(nome)}{EmpresaFilter()}&limit=1");
                        if (byNome.Count > 0)
                        {
                            uuid = byNome[0]["id"]?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(uuid))
                                SaveSupabaseUuid("cliente", "Codigo", codigoCliente, uuid);
                        }
                    }
                }

                // telefone: send null when empty to avoid storing 0
                long? telLong = (!string.IsNullOrWhiteSpace(celular) && long.TryParse(celular, out var tl) && tl > 0)
                    ? (long?)tl : null;

                // cpf_cnpj: numeric — only send when parseable and > 0
                long? cpfLong = (!string.IsNullOrWhiteSpace(cpf) && long.TryParse(cpf, out var cl) && cl > 0)
                    ? (long?)cl : null;

                // Build formatted address string for endereco_padrao
                string enderecoFormatado = endereco;
                if (!string.IsNullOrWhiteSpace(numero))      enderecoFormatado += $", {numero}";
                if (!string.IsNullOrWhiteSpace(complemento)) enderecoFormatado += $" - {complemento}";
                if (!string.IsNullOrWhiteSpace(bairro))      enderecoFormatado += $", {bairro}";
                if (!string.IsNullOrWhiteSpace(cidade))      enderecoFormatado += $" - {cidade}";

                var payload = new
                {
                    nome,
                    telefone        = (object)telLong ?? DBNull.Value,
                    cpf_cnpj        = (object)cpfLong ?? DBNull.Value,
                    endereco_padrao = string.IsNullOrWhiteSpace(enderecoFormatado) ? null : enderecoFormatado,
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

                // Sync address to enderecos_salvo — lookup by whatsapp+endereco only (bairro can be null)
                if (!string.IsNullOrWhiteSpace(celular) && !string.IsNullOrWhiteSpace(endereco))
                {
                    try
                    {
                        var byAddr = await GetAsync(
                            $"enderecos_salvo?whatsapp=eq.{Uri.EscapeDataString(celular)}&endereco=eq.{Uri.EscapeDataString(endereco)}&limit=1");
                        var addrPayload = new
                        {
                            whatsapp    = celular,
                            endereco,
                            bairro      = string.IsNullOrWhiteSpace(bairro)    ? null : bairro,
                            numero      = string.IsNullOrWhiteSpace(numero)     ? null : numero,
                            complemento = string.IsNullOrWhiteSpace(complemento) ? null : complemento,
                            cidade      = string.IsNullOrWhiteSpace(cidade)    ? null : cidade,
                        };
                        if (byAddr.Count > 0)
                        {
                            string addrId = byAddr[0]["id"]?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(addrId))
                                await PatchAsync("enderecos_salvo", $"id=eq.{addrId}", addrPayload);
                        }
                        else
                        {
                            await PostAsync("enderecos_salvo", addrPayload);
                        }
                    }
                    catch { } // non-critical
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

        /// <summary>
        /// Sincroniza os itens da tabela marmita_item como complementos no Supabase.
        /// Cada marmita ganha um complemento_grupo próprio vinculado via mercadoria_complemento_grupo.
        /// Os itens aparecem apenas na categoria Marmitas — não são produtos independentes.
        /// </summary>
        public static async Task SincronizarItensMarmitaAsync(int codigoMarmita)
        {
            if (!SiteConectado) return;
            try
            {
                // 1. UUID da marmita no Supabase
                string marmitaUuid = GetSupabaseUuid("marmita", "Codigo", codigoMarmita);
                if (string.IsNullOrWhiteSpace(marmitaUuid)) return;

                // 2. Busca itens agrupados: nome, preco, grupo, maxGrupo, maxAd, imagemUrl
                var itensRaw = new List<(string nome, decimal preco, string grupo, int maxGrupo, int maxAd, string imagemUrl)>();
                using (var conn = AbrirMysql())
                using (var cmd = new MySqlCommand(@"
                    SELECT mi.maritmNome,
                           CASE WHEN COALESCE(mi.maritmGrupo,'') = 'Adicionais'
                                     AND COALESCE(m.mercPreco_Adicional,0) > 0
                                THEN m.mercPreco_Adicional
                                ELSE 0  -- complementos da marmita não têm cobrança extra (incluídos no preço da marmita)
                           END AS preco,
                           COALESCE(mi.maritmGrupo, 'Geral')    AS grupo,
                           COALESCE(mi.maritmGrupoMax, 1)        AS maxGrupo,
                           COALESCE(m.mercAdicional_Qtd_Max, 1)  AS maxAd,
                           COALESCE(m.mercImagem_Url, '')         AS imagemUrl
                    FROM marmita_item mi
                    LEFT JOIN mercadoria m ON m.Codigo = mi.maritmCodigo_Merc
                    WHERE mi.Codigo_Marmita = @c
                    ORDER BY mi.maritmGrupo, mi.maritmNome", conn))
                {
                    cmd.Parameters.AddWithValue("@c", codigoMarmita);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        string rawImg = r["imagemUrl"]?.ToString() ?? "";
                        // Nunca enviar caminhos locais para o Supabase
                        if (!string.IsNullOrWhiteSpace(rawImg) &&
                            !rawImg.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                            rawImg = "";
                        itensRaw.Add((
                            r["maritmNome"]?.ToString() ?? "",
                            Convert.ToDecimal(r["preco"]),
                            r["grupo"]?.ToString()?.Trim() is string g && g.Length > 0 ? g : "Geral",
                            r["maxGrupo"] == DBNull.Value ? 1 : Convert.ToInt32(r["maxGrupo"]),
                            r["maxAd"]    == DBNull.Value ? 1 : Convert.ToInt32(r["maxAd"]),
                            rawImg
                        ));
                    }
                }

                if (!itensRaw.Any()) return;

                // 3. Agrupa por grupo — separa "Adicionais" dos complementos normais
                var porGrupo = itensRaw
                    .Where(i => !i.grupo.Equals("Adicionais", StringComparison.OrdinalIgnoreCase))
                    .GroupBy(i => i.grupo, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        g => g.Key,
                        g => (itens: g.ToList(), maxGrupo: g.First().maxGrupo),
                        StringComparer.OrdinalIgnoreCase);

                var adicionaisItens = itensRaw
                    .Where(i => i.grupo.Equals("Adicionais", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // 4. Busca todos os grupos de complemento existentes desta marmita no Supabase
                var gruposExistentes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // nome → id
                try
                {
                    var grpArr = await GetAsync($"{TBL_COMP_GRUPO}?mercadoria_id=eq.{marmitaUuid}&select=id,nome");
                    foreach (JObject g in grpArr)
                    {
                        var n  = g["nome"]?.ToString();
                        var id = g["id"]?.ToString();
                        if (!string.IsNullOrEmpty(n) && !string.IsNullOrEmpty(id))
                            gruposExistentes[n] = id;
                    }
                }
                catch { }

                // 5. Para cada grupo local, upsert no Supabase
                var gruposUsados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var kvGrupo in porGrupo)
                {
                    string grupoNome = kvGrupo.Key;
                    var    grupoItens = kvGrupo.Value.itens;
                    int    grupoMax   = kvGrupo.Value.maxGrupo < 1 ? 1 : kvGrupo.Value.maxGrupo;
                    gruposUsados.Add(grupoNome);

                    string grupoCompId;
                    if (gruposExistentes.TryGetValue(grupoNome, out string existId))
                    {
                        grupoCompId = existId;
                        try
                        {
                            await PatchAsync(TBL_COMP_GRUPO, $"id=eq.{grupoCompId}", new
                            {
                                mercadoria_id = marmitaUuid,
                                maximo        = grupoMax,
                                minimo        = 1,
                                obrigatorio   = !string.Equals(grupoNome, "Geral", StringComparison.OrdinalIgnoreCase),
                                ativo         = true,
                            });
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            var grpResult = await UpsertAsync(TBL_COMP_GRUPO, new
                            {
                                mercadoria_id = marmitaUuid,
                                nome          = grupoNome,
                                obrigatorio   = !string.Equals(grupoNome, "Geral", StringComparison.OrdinalIgnoreCase),
                                minimo        = 1,
                                maximo        = grupoMax,
                                ativo         = true,
                            }, "mercadoria_id,nome");
                            grupoCompId = grpResult?["id"]?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(grupoCompId))
                                gruposExistentes[grupoNome] = grupoCompId;
                        }
                        catch { continue; }
                    }

                    if (string.IsNullOrWhiteSpace(grupoCompId)) continue;

                    // 5b. Busca complementos já existentes neste grupo
                    var compExistentes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        var compArr = await GetAsync($"{TBL_COMPLEMENTO}?grupo_id=eq.{grupoCompId}&select=id,nome");
                        foreach (JObject c in compArr)
                        {
                            var n  = c["nome"]?.ToString();
                            var id = c["id"]?.ToString();
                            if (!string.IsNullOrEmpty(n) && !string.IsNullOrEmpty(id))
                                compExistentes[n] = id;
                        }
                    }
                    catch { }

                    var nomesNoGrupo = new HashSet<string>(grupoItens.Select(i => i.nome), StringComparer.OrdinalIgnoreCase);

                    // Upsert itens do grupo (inclui imagem_url do produto no Supabase)
                    foreach (var (nome, preco, _, __, ___, itemImagemUrl) in grupoItens)
                    {
                        try
                        {
                            if (compExistentes.TryGetValue(nome, out string cId))
                            {
                                object patchComp = string.IsNullOrWhiteSpace(itemImagemUrl)
                                    ? (object)new { nome, preco, ativo = true }
                                    : (object)new { nome, preco, ativo = true, imagem_url = itemImagemUrl };
                                await PatchAsync(TBL_COMPLEMENTO, $"id=eq.{cId}", patchComp);
                            }
                            else
                            {
                                object postComp = string.IsNullOrWhiteSpace(itemImagemUrl)
                                    ? (object)new { grupo_id = grupoCompId, nome, preco, ativo = true }
                                    : (object)new { grupo_id = grupoCompId, nome, preco, ativo = true, imagem_url = itemImagemUrl };
                                await UpsertAsync(TBL_COMPLEMENTO, postComp, "grupo_id,nome");
                            }
                        }
                        catch { }
                    }

                    // Remove itens que não existem mais no grupo
                    foreach (var kvComp in compExistentes)
                        if (!nomesNoGrupo.Contains(kvComp.Key))
                            try { await DeleteAsync(TBL_COMPLEMENTO, $"id=eq.{kvComp.Value}"); } catch { }
                }

                // 6. Remove grupos que não existem mais localmente (deleta complementos + grupo)
                foreach (var kvGrupoOld in gruposExistentes)
                {
                    if (gruposUsados.Contains(kvGrupoOld.Key)) continue;
                    try
                    {
                        // Deleta complementos do grupo antes de deletar o grupo
                        await DeleteAsync(TBL_COMPLEMENTO, $"grupo_id=eq.{kvGrupoOld.Value}");
                        await DeleteAsync(TBL_COMP_GRUPO, $"id=eq.{kvGrupoOld.Value}");
                    }
                    catch { }
                }

                // 7. Sincroniza itens do grupo "Adicionais" para a tabela `adicional` do Supabase
                //    Cada item aparece com +/- seletor de quantidade e tem seu preço somado ao total.
                if (adicionaisItens.Any())
                {
                    // Garante que o grupo "Adicionais" não exista como complemento_grupo (limpeza)
                    if (gruposExistentes.TryGetValue("Adicionais", out string adGrupoId))
                    {
                        try { await DeleteAsync(TBL_COMPLEMENTO, $"grupo_id=eq.{adGrupoId}"); } catch { }
                        try { await DeleteAsync(TBL_COMP_GRUPO, $"id=eq.{adGrupoId}"); } catch { }
                    }

                    // Busca adicionais existentes desta marmita no Supabase (filtrado por empresa)
                    var adExistentes = new Dictionary<string, (string id, decimal preco, int maxQtde)>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        // Busca SEM empresa_codigo para encontrar registros legados com empresa_codigo=NULL
                        var adArr = await GetAsync(
                            $"adicional?mercadoria_id=eq.{marmitaUuid}&select=id,nome,preco,max_qtde");
                        foreach (JObject a in adArr)
                        {
                            string n = a["nome"]?.ToString(); string id = a["id"]?.ToString();
                            if (!string.IsNullOrEmpty(n) && !string.IsNullOrEmpty(id))
                                adExistentes[n] = (id, a["preco"] == null ? 0m : Convert.ToDecimal(a["preco"]),
                                                       a["max_qtde"] == null ? 1 : Convert.ToInt32(a["max_qtde"]));
                        }
                    }
                    catch { }

                    int maxGrupoAd = adicionaisItens.FirstOrDefault().maxGrupo < 1 ? 1 : adicionaisItens.First().maxGrupo;
                    var nomesAd = new HashSet<string>(adicionaisItens.Select(i => i.nome), StringComparer.OrdinalIgnoreCase);

                    foreach (var (nome, preco, _, maxG, maxAd, adImagemUrl) in adicionaisItens)
                    {
                        int qtdeMax = maxAd > 0 ? maxAd : 1;
                        bool temEmp = !string.IsNullOrWhiteSpace(_empresaCodigo);
                        bool temImg = !string.IsNullOrWhiteSpace(adImagemUrl);
                        try
                        {
                            if (adExistentes.TryGetValue(nome, out var existing))
                            {
                                object patchAd;
                                if (temImg && temEmp)       patchAd = new { nome, preco, max_qtde = qtdeMax, ativo = true, imagem_url = adImagemUrl, empresa_codigo = _empresaCodigo };
                                else if (temImg)            patchAd = new { nome, preco, max_qtde = qtdeMax, ativo = true, imagem_url = adImagemUrl };
                                else if (temEmp)            patchAd = new { nome, preco, max_qtde = qtdeMax, ativo = true, empresa_codigo = _empresaCodigo };
                                else                        patchAd = new { nome, preco, max_qtde = qtdeMax, ativo = true };
                                await PatchAsync("adicional", $"id=eq.{existing.id}", patchAd);
                            }
                            else
                            {
                                object postAd;
                                if (temImg && temEmp)       postAd = new { mercadoria_id = marmitaUuid, nome, preco, max_qtde = qtdeMax, ativo = true, imagem_url = adImagemUrl, empresa_codigo = _empresaCodigo };
                                else if (temImg)            postAd = new { mercadoria_id = marmitaUuid, nome, preco, max_qtde = qtdeMax, ativo = true, imagem_url = adImagemUrl };
                                else if (temEmp)            postAd = new { mercadoria_id = marmitaUuid, nome, preco, max_qtde = qtdeMax, ativo = true, empresa_codigo = _empresaCodigo };
                                else                        postAd = new { mercadoria_id = marmitaUuid, nome, preco, max_qtde = qtdeMax, ativo = true };
                                await UpsertAsync("adicional", postAd, "mercadoria_id,nome");
                            }
                        }
                        catch { }
                    }

                    // Remove adicionais que não existem mais no grupo
                    foreach (var kvAd in adExistentes)
                        if (!nomesAd.Contains(kvAd.Key))
                            try { await DeleteAsync("adicional", $"id=eq.{kvAd.Value.id}"); } catch { }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarItensMarmitaAsync", $"Erro marmita {codigoMarmita}", ex);
            }
        }

        // Kept for backwards compatibility — now just delegates to SincronizarItensMarmitaAsync
        public static async Task SincronizarComplementosMarmitaAsync(int codigoMarmita)
            => await SincronizarItensMarmitaAsync(codigoMarmita);

        /// <summary>
        /// Lê todos os vínculos ativos de mercadoria_vinculo_grupo e chama SincronizarVinculosGrupoAsync
        /// para cada produto que tenha ao menos um vínculo ativo.
        /// </summary>
        public static async Task SincronizarTodosVinculosAsync()
        {
            try
            {
                // Busca todos os vínculos agrupados por produto
                var vinculos = new Dictionary<int, (List<(int, string)> ads, List<(int, string)> comps, List<(int, string)> sabs, int qtdSaboresMan)>();
                using (var conn = AbrirMysql())
                using (var cmd = new MySqlCommand(@"
                    SELECT mvg.Codigo_Mercadoria, mvg.Codigo_Grupo, mvg.tipo,
                           COALESCE(gm.grmeDescricao_,'') AS Nome,
                           COALESCE(m2.mercMercadoria,'') AS NomeProd,
                           COALESCE(m.mercQtd_Sabores_Manual,1) AS QtdSabMan
                    FROM mercadoria_vinculo_grupo mvg
                    LEFT JOIN grupo_mercadoria gm ON gm.Codigo = mvg.Codigo_Grupo AND mvg.tipo<>'S'
                    LEFT JOIN mercadoria m2 ON m2.Codigo = mvg.Codigo_Grupo AND mvg.tipo='S'
                    LEFT JOIN mercadoria m ON m.Codigo = mvg.Codigo_Mercadoria
                    WHERE mvg.Situacao = 'A'
                    ORDER BY mvg.Codigo_Mercadoria", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        int    codMerc  = r.GetInt32(0);
                        int    codGrupo = r.GetInt32(1);
                        string tipo     = r.GetString(2);
                        string nome     = r.GetString(3);
                        string nomeProd = r.GetString(4);
                        int    qtdSMan  = r["QtdSabMan"] == DBNull.Value ? 1 : Convert.ToInt32(r["QtdSabMan"]);
                        if (!vinculos.ContainsKey(codMerc))
                            vinculos[codMerc] = (new List<(int, string)>(), new List<(int, string)>(), new List<(int, string)>(), qtdSMan);
                        if (tipo == "A")      vinculos[codMerc].ads.Add((codGrupo, nome));
                        else if (tipo == "C") vinculos[codMerc].comps.Add((codGrupo, nome));
                        else if (tipo == "S") vinculos[codMerc].sabs.Add((codGrupo, nomeProd));
                    }
                }
                foreach (var kv in vinculos)
                    await SincronizarVinculosGrupoAsync(kv.Key, kv.Value.ads, kv.Value.comps, kv.Value.sabs, kv.Value.qtdSaboresMan);

                // Produtos fracionados sem vínculos também precisam ser sincronizados
                var codsFrac = new List<int>();
                using (var conn = AbrirMysql())
                using (var cmd = new MySqlCommand(
                    "SELECT Codigo FROM mercadoria WHERE COALESCE(mercFracionado,0)=1 AND Situacao='A'", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) codsFrac.Add(r.GetInt32(0));
                }
                foreach (var cod in codsFrac)
                {
                    if (vinculos.ContainsKey(cod)) continue; // já foi chamado acima
                    string uuid = GetSupabaseUuid("mercadoria", "Codigo", cod);
                    if (string.IsNullOrEmpty(uuid)) { await SincronizarProdutoAsync(cod); uuid = GetSupabaseUuid("mercadoria", "Codigo", cod); }
                    await SincronizarFracionadoAsync(cod, uuid);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodosVinculosAsync", "Erro", ex);
            }
        }

        /// <summary>
        /// Syncs Adicionais and Complementos group links to Supabase.
        /// adicionais (opcional=true) and complementos (opcional=false) are posted to the 'adicionais' table
        /// keyed by the product's mercadoria_id, using the grupo_mercadoria name.
        /// </summary>
        /// <summary>
        /// Sincroniza vínculos de adicionais e complementos de um produto.
        /// - Complementos com categoria "Marmitas": são adicionados ao complemento_grupo da marmita
        ///   e o produto é marcado como ativo=false (não aparece como produto independente).
        /// - Complementos de outras categorias: criam/atualizam um complemento_grupo próprio
        ///   vinculado ao produto pai, e inserem o produto como complemento.
        /// - Adicionais: sincronizados normalmente como mercadoria.
        /// </summary>
        public static async Task SincronizarVinculosGrupoAsync(
            int codigoMercadoria,
            IEnumerable<(int CodigoGrupo, string NomeGrupo)> adicionais,
            IEnumerable<(int CodigoGrupo, string NomeGrupo)> complementos,
            IEnumerable<(int CodigoProduto, string NomeProduto)> sabores = null,
            int qtdMaxSabores = 1)
        {
            try
            {
                var compList = complementos?.ToList() ?? new List<(int, string)>();
                var adList   = adicionais?.ToList()   ?? new List<(int, string)>();
                var sabList0 = sabores?.ToList()      ?? new List<(int, string)>();

                // Somente ingredientes de Marmitas ficam ocultos como produto independente.
                // Produtos com vínculos complemento de outras categorias (ou vínculos obsoletos)
                // respeitam o flag habSite para decidir visibilidade standalone.
                bool isPuroIngredienteMarmita =
                    compList.Any(c => c.NomeGrupo.Equals("Marmitas", StringComparison.OrdinalIgnoreCase))
                    && !adList.Any()
                    && !sabList0.Any();

                // 1. Sincroniza o produto principal
                if (isPuroIngredienteMarmita)
                    await SincronizarProdutoAsync(codigoMercadoria, forcarAtivoFalse: true);
                else
                    await SincronizarProdutoAsync(codigoMercadoria);

                string produtoUuid = GetSupabaseUuid("mercadoria", "Codigo", codigoMercadoria);

                // 2. Pré-sincroniza todos os produtos de CADA grupo de adicional para garantir
                //    que os UUIDs no cache sejam da empresa corrente (não registros orphan).
                foreach (var (codGrupo, _) in adList)
                {
                    if (codGrupo <= 0) continue;
                    var codsDoGrupo = new List<int>();
                    try
                    {
                        using var connG = AbrirMysql();
                        using var cmdG  = new MySqlCommand(
                            "SELECT Codigo FROM mercadoria WHERE Codigo_Grupo=@g AND Situacao='A'", connG);
                        cmdG.Parameters.AddWithValue("@g", codGrupo);
                        using var rG = cmdG.ExecuteReader();
                        while (rG.Read()) codsDoGrupo.Add(rG.GetInt32(0));
                    }
                    catch { }
                    foreach (var cod in codsDoGrupo)
                        await SincronizarProdutoAsync(cod);
                }

                if (string.IsNullOrWhiteSpace(produtoUuid)) return;

                // 3. Complementos: upsert na tabela complemento do Supabase
                foreach (var (codGrupo, nomeGrupo) in complementos)
                {
                    if (codGrupo <= 0) continue;

                    // Verifica se é o grupo Marmitas
                    bool ehMarmitas = nomeGrupo.Equals("Marmitas", StringComparison.OrdinalIgnoreCase);

                    // Nome/preço/imagem do produto que será inserido como complemento
                    string nomeProduto = "";
                    decimal precoProduto = 0m;
                    string imagemProduto = "";
                    using (var conn = AbrirMysql())
                    using (var cmd = new MySqlCommand(
                        "SELECT mercMercadoria, COALESCE(mercPreco_Venda,0) AS preco, " +
                        "COALESCE(mercImagem_Url,'') AS mercImagem_Url " +
                        "FROM mercadoria WHERE Codigo=@c LIMIT 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@c", codigoMercadoria);
                        using var r = cmd.ExecuteReader();
                        if (r.Read())
                        {
                            nomeProduto   = r["mercMercadoria"]?.ToString() ?? "";
                            precoProduto  = Convert.ToDecimal(r["preco"]);
                            imagemProduto = r["mercImagem_Url"]?.ToString() ?? "";
                        }
                    }
                    if (string.IsNullOrWhiteSpace(nomeProduto)) continue;
                    // Só envia URLs válidas para o Supabase
                    string imgUrlMarmita = imagemProduto.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                                          ? imagemProduto : null;

                    // Se for Marmitas: este produto serve de complemento para CADA marmita existente
                    if (ehMarmitas)
                    {
                        // Marca produto como não independente (ativo=false na tabela mercadoria)
                        try { await PatchAsync(TBL_MERCADORIAS, $"id=eq.{produtoUuid}", new { ativo = false }); }
                        catch (Exception ex) { Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync", $"Erro ao marcar ativo=false produto {codigoMercadoria}", ex); }

                        // Busca marmitas reais no MySQL (tabela `marmita`, não `mercadoria`)
                        // e sincroniza cada uma com o Supabase se ainda não tiver UUID
                        var marmitaUuids = new List<(string uuid, string nome)>();
                        try
                        {
                            var codsMarmita = new List<(int cod, string nome)>();
                            using (var conn2 = AbrirMysql())
                            using (var cmd2 = new MySqlCommand(
                                "SELECT Codigo, marDescricao FROM marmita WHERE Situacao='A' ORDER BY Codigo", conn2))
                            {
                                using var r2 = cmd2.ExecuteReader();
                                while (r2.Read())
                                    codsMarmita.Add((r2.GetInt32(0), r2.GetString(1)));
                            }
                            foreach (var (cod, mNome) in codsMarmita)
                            {
                                string muuid = GetSupabaseUuid("marmita", "Codigo", cod);
                                if (string.IsNullOrEmpty(muuid))
                                {
                                    await SincronizarMarmitaAsync(cod);
                                    muuid = GetSupabaseUuid("marmita", "Codigo", cod);
                                }
                                if (!string.IsNullOrEmpty(muuid))
                                    marmitaUuids.Add((muuid, mNome));
                            }
                        }
                        catch (Exception ex) { Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync", $"Erro ao buscar marmitas para produto {codigoMercadoria}", ex); }

                        // Para cada marmita: garante que existe um complemento_grupo e insere o ingrediente
                        var gruposUuids = new List<string>();
                        foreach (var (muuid, mNome) in marmitaUuids)
                        {
                            try
                            {
                                string gid = "";
                                // Busca grupo existente via link table
                                var lnks = await GetAsync($"{TBL_MERC_COMP_GRP}?mercadoria_id=eq.{muuid}&select=grupo_id&limit=1");
                                if (lnks.Count > 0)
                                {
                                    gid = lnks[0]["grupo_id"]?.ToString() ?? "";
                                }
                                else
                                {
                                    object mGrpPost = UsarEmpresaCodigo
                                        ? (object)new { nome = mNome, obrigatorio = true, minimo = 1, maximo = 1, empresa_codigo = _empresaCodigo }
                                        : (object)new { nome = mNome, obrigatorio = true, minimo = 1, maximo = 1 };
                                    var grpResult = await UpsertAsync(TBL_COMP_GRUPO, mGrpPost, "mercadoria_id,nome");
                                    gid = grpResult?["id"]?.ToString() ?? "";
                                    if (!string.IsNullOrEmpty(gid))
                                        try { await PostAsync(TBL_MERC_COMP_GRP, new { mercadoria_id = muuid, grupo_id = gid }); } catch { }
                                }
                                if (!string.IsNullOrEmpty(gid)) gruposUuids.Add(gid);
                            }
                            catch (Exception ex) { Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync", $"Erro ao criar complemento_grupo para marmita {muuid}", ex); }
                        }

                        // Upsert do ingrediente em cada grupo de marmita
                        foreach (var grupoId in gruposUuids)
                        {
                            try
                            {
                                var existentes = await GetAsync(
                                    $"{TBL_COMPLEMENTO}?grupo_id=eq.{grupoId}&nome=eq.{Uri.EscapeDataString(nomeProduto)}&limit=1");
                                bool temEmpMar = UsarEmpresaCodigo; string empMarCod = _empresaCodigo;
                                if (existentes.Count > 0)
                                {
                                    // PATCH — inclui imagem_url apenas se disponível (evita limpar imagem existente)
                                    object patchPayload;
                                    if (imgUrlMarmita != null)
                                        patchPayload = temEmpMar ? (object)new { nome = nomeProduto, preco = precoProduto, ativo = true, imagem_url = imgUrlMarmita, empresa_codigo = empMarCod } : (object)new { nome = nomeProduto, preco = precoProduto, ativo = true, imagem_url = imgUrlMarmita };
                                    else
                                    {
                                        var jo = JObject.FromObject(temEmpMar ? (object)new { nome = nomeProduto, preco = precoProduto, ativo = true, imagem_url = (string)null, empresa_codigo = empMarCod } : (object)new { nome = nomeProduto, preco = precoProduto, ativo = true, imagem_url = (string)null });
                                        jo.Remove("imagem_url");
                                        patchPayload = jo;
                                    }
                                    await PatchAsync(TBL_COMPLEMENTO, $"id=eq.{existentes[0]["id"]}", patchPayload);
                                }
                                else
                                    await UpsertAsync(TBL_COMPLEMENTO,
                                        temEmpMar
                                            ? (object)new { grupo_id = grupoId, nome = nomeProduto, preco = precoProduto, ativo = true, imagem_url = imgUrlMarmita, empresa_codigo = empMarCod }
                                            : (object)new { grupo_id = grupoId, nome = nomeProduto, preco = precoProduto, ativo = true, imagem_url = imgUrlMarmita },
                                        "grupo_id,nome");
                                Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync", $"Complemento '{nomeProduto}' inserido no grupo {grupoId}");
                            }
                            catch (Exception ex) { Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync", $"Erro ao upsert complemento '{nomeProduto}' no grupo {grupoId}", ex); }
                        }

                        if (gruposUuids.Count == 0)
                            Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync", $"Nenhum complemento_grupo encontrado/criado para produto {codigoMercadoria} ({nomeProduto}) — nenhuma marmita ativa?");
                    }
                    else
                    {
                        // Complemento de categoria não-Marmitas: NÃO cria grupo separado.
                        // O produto aparecerá como sabor no grupo "Sabores" do produto fracionado
                        // via SincronizarFracionadoAsync. Grupos órfãos de sincronizações antigas
                        // são desativados para não aparecerem no site.
                        var codsAlvo = new List<int>();
                        try
                        {
                            using var connAlvo = AbrirMysql();
                            using var cmdAlvo = new MySqlCommand(
                                "SELECT Codigo FROM mercadoria WHERE Codigo_Grupo=@g AND Situacao='A' AND Codigo<>@c", connAlvo);
                            cmdAlvo.Parameters.AddWithValue("@g", codGrupo);
                            cmdAlvo.Parameters.AddWithValue("@c", codigoMercadoria);
                            using var rAlvo = cmdAlvo.ExecuteReader();
                            while (rAlvo.Read()) codsAlvo.Add(rAlvo.GetInt32(0));
                        }
                        catch { }

                        foreach (var codAlvo in codsAlvo)
                        {
                            string alvoUuid = GetSupabaseUuid("mercadoria", "Codigo", codAlvo);
                            if (string.IsNullOrWhiteSpace(alvoUuid)) continue;
                            try
                            {
                                // Desativa qualquer grupo órfão com este nome que ainda esteja ativo
                                var grpArr = await GetAsync(
                                    $"{TBL_COMP_GRUPO}?mercadoria_id=eq.{alvoUuid}&nome=eq.{Uri.EscapeDataString(nomeGrupo)}&ativo=eq.true");
                                foreach (JObject grp in grpArr)
                                {
                                    var gId = grp["id"]?.ToString() ?? "";
                                    if (!string.IsNullOrWhiteSpace(gId))
                                        await PatchAsync(TBL_COMP_GRUPO, $"id=eq.{gId}", new { ativo = false });
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync",
                                    $"Erro ao desativar grupo complemento órfão '{nomeGrupo}' em produto {codAlvo}", ex);
                            }
                        }
                    }
                }

                // 4. Adicionais: sincroniza na tabela `adicional` vinculado a cada produto fracionado do grupo
                foreach (var (codGrupo, nomeGrupo) in adList)
                {
                    if (codGrupo <= 0) continue;

                    // Lê nome, preço_adicional, qtd_max e imagem do produto no MySQL
                    string nomeAd = ""; decimal precoAd = 0m; int qtdMaxAd = 1; string imgAd = "";
                    using (var conn = AbrirMysql())
                    using (var cmd = new MySqlCommand(
                        "SELECT mercMercadoria, COALESCE(mercPreco_Adicional,0) AS precoad, COALESCE(mercAdicional_Qtd_Max,1) AS maxad, " +
                        "COALESCE(mercImagem_Url,'') AS img " +
                        "FROM mercadoria WHERE Codigo=@c LIMIT 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@c", codigoMercadoria);
                        using var r = cmd.ExecuteReader();
                        if (r.Read())
                        {
                            nomeAd   = r["mercMercadoria"]?.ToString() ?? "";
                            precoAd  = Convert.ToDecimal(r["precoad"]);
                            qtdMaxAd = r["maxad"] == DBNull.Value ? 1 : Convert.ToInt32(r["maxad"]);
                            imgAd    = r["img"]?.ToString() ?? "";
                            if (!imgAd.StartsWith("http", StringComparison.OrdinalIgnoreCase)) imgAd = "";
                        }
                    }
                    if (string.IsNullOrWhiteSpace(nomeAd)) continue;

                    // Busca todos os produtos ATIVOS do grupo no MySQL (fracionados ou não)
                    var codsFracionados = new List<int>();
                    using (var conn = AbrirMysql())
                    using (var cmd = new MySqlCommand(
                        "SELECT Codigo FROM mercadoria WHERE Codigo_Grupo=@g AND Situacao='A'", conn))
                    {
                        cmd.Parameters.AddWithValue("@g", codGrupo);
                        using var r = cmd.ExecuteReader();
                        while (r.Read()) codsFracionados.Add(r.GetInt32(0));
                    }

                    // Upsert um registro na tabela `adicional` por produto do grupo
                    foreach (var codFrac in codsFracionados)
                    {
                        string fracUuid = GetSupabaseUuid("mercadoria", "Codigo", codFrac);
                        if (string.IsNullOrWhiteSpace(fracUuid))
                        {
                            await SincronizarProdutoAsync(codFrac);
                            fracUuid = GetSupabaseUuid("mercadoria", "Codigo", codFrac);
                        }
                        if (string.IsNullOrWhiteSpace(fracUuid)) continue;

                        try
                        {
                            bool temEmpAd = !string.IsNullOrWhiteSpace(_empresaCodigo);
                            // Lookup SEM empresa_codigo para capturar registros legados com NULL
                            var existAd = await GetAsync(
                                $"adicional?mercadoria_id=eq.{fracUuid}&nome=eq.{Uri.EscapeDataString(nomeAd)}&limit=1");
                            if (existAd.Count > 0)
                            {
                                object patchAd = string.IsNullOrEmpty(imgAd)
                                    ? temEmpAd ? (object)new { nome = nomeAd, preco = precoAd, max_qtde = qtdMaxAd, ativo = true, empresa_codigo = _empresaCodigo }
                                               : (object)new { nome = nomeAd, preco = precoAd, max_qtde = qtdMaxAd, ativo = true }
                                    : temEmpAd ? (object)new { nome = nomeAd, preco = precoAd, max_qtde = qtdMaxAd, ativo = true, imagem_url = imgAd, empresa_codigo = _empresaCodigo }
                                               : (object)new { nome = nomeAd, preco = precoAd, max_qtde = qtdMaxAd, ativo = true, imagem_url = imgAd };
                                await PatchAsync("adicional", $"id=eq.{existAd[0]["id"]}", patchAd);
                            }
                            else
                            {
                                object postAd = string.IsNullOrEmpty(imgAd)
                                    ? temEmpAd ? (object)new { mercadoria_id = fracUuid, nome = nomeAd, preco = precoAd, max_qtde = qtdMaxAd, ativo = true, empresa_codigo = _empresaCodigo }
                                               : (object)new { mercadoria_id = fracUuid, nome = nomeAd, preco = precoAd, max_qtde = qtdMaxAd, ativo = true }
                                    : temEmpAd ? (object)new { mercadoria_id = fracUuid, nome = nomeAd, preco = precoAd, max_qtde = qtdMaxAd, ativo = true, imagem_url = imgAd, empresa_codigo = _empresaCodigo }
                                               : (object)new { mercadoria_id = fracUuid, nome = nomeAd, preco = precoAd, max_qtde = qtdMaxAd, ativo = true, imagem_url = imgAd };
                                await UpsertAsync("adicional", postAd, "mercadoria_id,nome");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync", $"Erro ao upsert adicional '{nomeAd}' para produto {codFrac}", ex);
                        }
                    }
                }

                // 5. Produto fracionado: cria/atualiza complemento_grupo com todos os produtos da mesma categoria
                await SincronizarFracionadoAsync(codigoMercadoria, produtoUuid);

                // 6. Sabores manuais (sentido invertido): o produto atual (ex: Bis) É um sabor
                //    disponível DENTRO dos produtos listados em sabList (ex: Açaí Grande).
                //    Criamos/atualizamos o grupo "Sabores" em cada produto-alvo adicionando
                //    este produto como uma opção. O grupo "Sabores" nunca fica no produto atual.
                var sabList = sabores?.ToList() ?? new List<(int, string)>();
                if (sabList.Any() && !string.IsNullOrWhiteSpace(produtoUuid))
                {
                    try
                    {
                        // Lê nome/preço/imagem do produto atual (ex: Bis) uma única vez
                        string nomeProdAtual = ""; decimal precoProdAtual = 0m; string imgProdAtual = "";
                        try
                        {
                            using var connSelf = AbrirMysql();
                            using var cmdSelf  = new MySqlCommand(
                                "SELECT mercMercadoria, COALESCE(mercPreco_Venda,0) AS preco, " +
                                "COALESCE(mercImagem_Url,'') AS img FROM mercadoria WHERE Codigo=@c LIMIT 1",
                                connSelf);
                            cmdSelf.Parameters.AddWithValue("@c", codigoMercadoria);
                            using var rSelf = cmdSelf.ExecuteReader();
                            if (rSelf.Read())
                            {
                                nomeProdAtual  = rSelf["mercMercadoria"]?.ToString() ?? "";
                                precoProdAtual = Convert.ToDecimal(rSelf["preco"]);
                                imgProdAtual   = rSelf["img"]?.ToString() ?? "";
                                if (!string.IsNullOrWhiteSpace(imgProdAtual) &&
                                    !imgProdAtual.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                                    imgProdAtual = "";
                            }
                        }
                        catch { }

                        // Para cada produto-alvo (ex: Açaí Grande), garante que existe no Supabase
                        // e cria/atualiza seu grupo "Sabores", adicionando o produto atual como opção.
                        foreach (var (codTarget, _) in sabList)
                        {
                            await SincronizarProdutoAsync(codTarget);
                            string targetUuid = GetSupabaseUuid("mercadoria", "Codigo", codTarget);
                            if (string.IsNullOrWhiteSpace(targetUuid)) continue;

                            // Lê qtd_sabores do produto-alvo para definir o máximo do grupo
                            int targetQtd = 1;
                            try
                            {
                                using var connQ = AbrirMysql();
                                using var cmdQ  = new MySqlCommand(
                                    "SELECT COALESCE(mercQtd_Sabores,1) AS qtd FROM mercadoria WHERE Codigo=@c LIMIT 1",
                                    connQ);
                                cmdQ.Parameters.AddWithValue("@c", codTarget);
                                var valQ = cmdQ.ExecuteScalar();
                                targetQtd = (valQ == null || valQ == DBNull.Value) ? 1 : Math.Max(1, Convert.ToInt32(valQ));
                            }
                            catch { }

                            string nomeGrpTarget = targetQtd == 1
                                ? "Sabores (escolha 1)"
                                : $"Sabores (escolha até {targetQtd})";

                            // Busca ou cria o grupo "Sabores*" no produto-alvo
                            string grpTargetId = "";
                            var grpTargetArr = await GetAsync(
                                $"{TBL_COMP_GRUPO}?mercadoria_id=eq.{targetUuid}&nome=ilike.Sabores*&limit=1");
                            if (grpTargetArr.Count > 0)
                            {
                                grpTargetId = grpTargetArr[0]["id"]?.ToString() ?? "";
                                object grpTargetPatch = UsarEmpresaCodigo
                                    ? (object)new { nome = nomeGrpTarget, obrigatorio = true, minimo = 1, maximo = targetQtd, ativo = true, empresa_codigo = _empresaCodigo }
                                    : (object)new { nome = nomeGrpTarget, obrigatorio = true, minimo = 1, maximo = targetQtd, ativo = true };
                                await PatchAsync(TBL_COMP_GRUPO, $"id=eq.{grpTargetId}", grpTargetPatch);
                            }
                            else
                            {
                                object grpTargetPost = UsarEmpresaCodigo
                                    ? (object)new { mercadoria_id = targetUuid, nome = nomeGrpTarget, obrigatorio = true, minimo = 1, maximo = targetQtd, ativo = true, empresa_codigo = _empresaCodigo }
                                    : (object)new { mercadoria_id = targetUuid, nome = nomeGrpTarget, obrigatorio = true, minimo = 1, maximo = targetQtd, ativo = true };
                                var resGrp = await UpsertAsync(TBL_COMP_GRUPO, grpTargetPost, "mercadoria_id,nome");
                                grpTargetId = resGrp?["id"]?.ToString() ?? "";
                            }
                            if (string.IsNullOrWhiteSpace(grpTargetId)) continue;

                            // Adiciona produto atual (Bis) como complemento no grupo "Sabores" do alvo
                            var existComp = await GetAsync(
                                $"{TBL_COMPLEMENTO}?grupo_id=eq.{grpTargetId}&nome=eq.{Uri.EscapeDataString(nomeProdAtual)}&limit=1");
                            try
                            {
                                bool temEmpS = UsarEmpresaCodigo; string empS = _empresaCodigo;
                                if (existComp.Count > 0)
                                {
                                    string eid = existComp[0]["id"]?.ToString() ?? "";
                                    await PatchAsync(TBL_COMPLEMENTO, $"id=eq.{eid}",
                                        string.IsNullOrEmpty(imgProdAtual)
                                            ? temEmpS ? (object)new { nome = nomeProdAtual, preco = precoProdAtual, ativo = true, empresa_codigo = empS }
                                                      : (object)new { nome = nomeProdAtual, preco = precoProdAtual, ativo = true }
                                            : temEmpS ? (object)new { nome = nomeProdAtual, preco = precoProdAtual, imagem_url = imgProdAtual, ativo = true, empresa_codigo = empS }
                                                      : (object)new { nome = nomeProdAtual, preco = precoProdAtual, imagem_url = imgProdAtual, ativo = true });
                                }
                                else
                                {
                                    await UpsertAsync(TBL_COMPLEMENTO,
                                        string.IsNullOrEmpty(imgProdAtual)
                                            ? temEmpS ? (object)new { grupo_id = grpTargetId, nome = nomeProdAtual, preco = precoProdAtual, ativo = true, empresa_codigo = empS }
                                                      : (object)new { grupo_id = grpTargetId, nome = nomeProdAtual, preco = precoProdAtual, ativo = true }
                                            : temEmpS ? (object)new { grupo_id = grpTargetId, nome = nomeProdAtual, preco = precoProdAtual, imagem_url = imgProdAtual, ativo = true, empresa_codigo = empS }
                                                      : (object)new { grupo_id = grpTargetId, nome = nomeProdAtual, preco = precoProdAtual, imagem_url = imgProdAtual, ativo = true },
                                        "grupo_id,nome");
                                }
                            }
                            catch (Exception exS) { Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync", $"Erro ao inserir sabor '{nomeProdAtual}' no alvo {codTarget}", exS); }
                        }

                        // Remove grupo "Sabores" que possa ter sido criado incorretamente NESTE produto
                        // (versões anteriores criavam o grupo no sentido errado — Bis como container)
                        var oldGrpArr = await GetAsync(
                            $"{TBL_COMP_GRUPO}?mercadoria_id=eq.{produtoUuid}&nome=ilike.Sabores*&select=id");
                        foreach (JObject oldGrp in oldGrpArr)
                        {
                            string oid = oldGrp["id"]?.ToString() ?? "";
                            if (string.IsNullOrWhiteSpace(oid)) continue;
                            var orphanComps = await GetAsync($"{TBL_COMPLEMENTO}?grupo_id=eq.{oid}&select=id");
                            foreach (JObject oc in orphanComps)
                                try { await PatchAsync(TBL_COMPLEMENTO, $"id=eq.{oc["id"]}", new { ativo = false }); } catch { }
                            try { await PatchAsync(TBL_COMP_GRUPO, $"id=eq.{oid}", new { ativo = false }); } catch { }
                        }

                        Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync",
                            $"Produto {codigoMercadoria}: {sabList.Count} alvos de sabor sincronizados (sentido invertido)");
                    }
                    catch (Exception exSab)
                    {
                        Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync", $"Erro sabores produto {codigoMercadoria}", exSab);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarVinculosGrupoAsync", $"Erro produto {codigoMercadoria}", ex);
            }
        }

        /// <summary>
        /// Se o produto estiver marcado como fracionado (múltiplos sabores), cria um complemento_grupo
        /// chamado "Sabores" vinculado ao produto, e insere todos os produtos da mesma categoria como opções.
        /// </summary>
        private static async Task SincronizarFracionadoAsync(int codigoMercadoria, string produtoUuid)
        {
            if (string.IsNullOrWhiteSpace(produtoUuid)) return;
            try
            {
                // Verifica se produto é fracionado
                bool fracionado = false; int qtdSabores = 1; int codigoGrupo = 0;
                using (var conn = AbrirMysql())
                using (var cmd = new MySqlCommand(
                    "SELECT COALESCE(mercFracionado,0) AS frac, COALESCE(mercQtd_Sabores,1) AS qtd, Codigo_Grupo " +
                    "FROM mercadoria WHERE Codigo=@c LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@c", codigoMercadoria);
                    using var r = cmd.ExecuteReader();
                    if (!r.Read()) return;
                    fracionado   = r["frac"]?.ToString() == "1";
                    qtdSabores   = r["qtd"] == DBNull.Value ? 1 : Convert.ToInt32(r["qtd"]);
                    codigoGrupo  = r["Codigo_Grupo"] == DBNull.Value ? 0 : Convert.ToInt32(r["Codigo_Grupo"]);
                }
                if (codigoGrupo <= 0) return;

                // Se o produto tem sabores manuais configurados (tipo='S'), o grupo "Sabores"
                // é gerenciado por SincronizarVinculosGrupoAsync — não criamos o automático aqui.
                bool temSaboresManuais = false;
                try
                {
                    using var connSM = AbrirMysql();
                    using var cmdSM  = new MySqlCommand(
                        "SELECT COUNT(*) FROM mercadoria_vinculo_grupo WHERE Codigo_Mercadoria=@c AND tipo='S' AND Situacao='A'",
                        connSM);
                    cmdSM.Parameters.AddWithValue("@c", codigoMercadoria);
                    temSaboresManuais = Convert.ToInt32(cmdSM.ExecuteScalar()) > 0;
                }
                catch { }

                // Adicionais e sabores só fazem sentido para produtos fracionados
                // (ex: Pizza Grande com adicionais e sabores).
                // Produtos não-fracionados (ex: próprios adicionais como "Batata Frita") não devem
                // ter registros na tabela adicional do Supabase vinculados a eles mesmos.
                if (!fracionado) return;

                // Se tem sabores manuais, não criar grupo automático — evita duplicata
                if (temSaboresManuais) return;

                // Sincroniza adicionais do grupo para este produto fracionado
                try
                {
                    var adicionaisMysql = new List<(string nome, decimal preco, int maxad, string img)>();
                    using (var conn = AbrirMysql())
                    using (var cmd = new MySqlCommand(
                        @"SELECT m.mercMercadoria, COALESCE(m.mercPreco_Adicional,0) AS precoad, COALESCE(m.mercAdicional_Qtd_Max,1) AS maxad,
                                 COALESCE(m.mercImagem_Url,'') AS img
                          FROM mercadoria_vinculo_grupo mvg
                          JOIN mercadoria m ON m.Codigo = mvg.Codigo_Mercadoria
                          WHERE mvg.Codigo_Grupo=@g AND mvg.tipo='A' AND mvg.Situacao='A' AND m.Situacao='A'", conn))
                    {
                        cmd.Parameters.AddWithValue("@g", codigoGrupo);
                        using var r = cmd.ExecuteReader();
                        while (r.Read())
                        {
                            string imgAd2 = r["img"]?.ToString() ?? "";
                            if (!imgAd2.StartsWith("http", StringComparison.OrdinalIgnoreCase)) imgAd2 = "";
                            adicionaisMysql.Add((r.GetString(0), Convert.ToDecimal(r["precoad"]), r["maxad"] == DBNull.Value ? 1 : Convert.ToInt32(r["maxad"]), imgAd2));
                        }
                    }

                    foreach (var (nomeAd, precoAd, maxAd, imgAd2) in adicionaisMysql)
                    {
                        try
                        {
                            bool temEmpAd2 = !string.IsNullOrWhiteSpace(_empresaCodigo);
                            // Lookup SEM empresa_codigo para capturar registros legados com NULL
                            var existAd = await GetAsync(
                                $"adicional?mercadoria_id=eq.{produtoUuid}&nome=eq.{Uri.EscapeDataString(nomeAd)}&limit=1");
                            if (existAd.Count > 0)
                            {
                                object pAd = string.IsNullOrEmpty(imgAd2)
                                    ? temEmpAd2 ? (object)new { nome = nomeAd, preco = precoAd, max_qtde = maxAd, ativo = true, empresa_codigo = _empresaCodigo }
                                               : (object)new { nome = nomeAd, preco = precoAd, max_qtde = maxAd, ativo = true }
                                    : temEmpAd2 ? (object)new { nome = nomeAd, preco = precoAd, max_qtde = maxAd, ativo = true, imagem_url = imgAd2, empresa_codigo = _empresaCodigo }
                                               : (object)new { nome = nomeAd, preco = precoAd, max_qtde = maxAd, ativo = true, imagem_url = imgAd2 };
                                await PatchAsync("adicional", $"id=eq.{existAd[0]["id"]}", pAd);
                            }
                            else
                            {
                                object pAd = string.IsNullOrEmpty(imgAd2)
                                    ? temEmpAd2 ? (object)new { mercadoria_id = produtoUuid, nome = nomeAd, preco = precoAd, max_qtde = maxAd, ativo = true, empresa_codigo = _empresaCodigo }
                                               : (object)new { mercadoria_id = produtoUuid, nome = nomeAd, preco = precoAd, max_qtde = maxAd, ativo = true }
                                    : temEmpAd2 ? (object)new { mercadoria_id = produtoUuid, nome = nomeAd, preco = precoAd, max_qtde = maxAd, ativo = true, imagem_url = imgAd2, empresa_codigo = _empresaCodigo }
                                               : (object)new { mercadoria_id = produtoUuid, nome = nomeAd, preco = precoAd, max_qtde = maxAd, ativo = true, imagem_url = imgAd2 };
                                await UpsertAsync("adicional", pAd, "mercadoria_id,nome");
                            }
                        }
                        catch (Exception exAd) { Logger.Log("SupabaseService", "SincronizarFracionadoAsync", $"Erro adicional '{nomeAd}'", exAd); }
                    }
                    if (adicionaisMysql.Count > 0)
                        Logger.Log("SupabaseService", "SincronizarFracionadoAsync", $"Produto {codigoMercadoria}: {adicionaisMysql.Count} adicionais sincronizados");
                }
                catch (Exception ex)
                {
                    Logger.Log("SupabaseService", "SincronizarFracionadoAsync", $"Erro ao sincronizar adicionais do produto {codigoMercadoria}", ex);
                }

                // Busca todos os produtos ATIVOS da mesma categoria que NÃO são fracionados
                // Detecção automática de sabores removida.
                // Somente vínculos explícitos (tipo='S') configurados pelo usuário criam o grupo
                // Sabores — gerenciado por SincronizarVinculosGrupoAsync.
                // Aqui apenas limpamos grupos órfãos que possam ter ficado de sincronizações antigas.

                // Limpa complemento_grupos órfãos: desativa grupos que existem no Supabase
                // mas não possuem mais vínculo ativo no MySQL (ex: quando o usuário desmarcou
                // "Complementos" em um produto e fez nova sincronização).
                try
                {
                    // 1. Nomes de grupos válidos no MySQL para este produto (tipo='C' e tipo='S')
                    var nomesValidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    using (var connV = AbrirMysql())
                    using (var cmdV = new MySqlCommand(@"
                        SELECT COALESCE(gm.grmeDescricao_,'') AS nome
                        FROM mercadoria_vinculo_grupo mvg
                        LEFT JOIN grupo_mercadoria gm ON gm.Codigo = mvg.Codigo_Grupo
                        WHERE mvg.Codigo_Mercadoria=@c AND mvg.Situacao='A'", connV))
                    {
                        cmdV.Parameters.AddWithValue("@c", codigoMercadoria);
                        using var rv = cmdV.ExecuteReader();
                        while (rv.Read()) { var n = rv.GetString(0); if (!string.IsNullOrWhiteSpace(n)) nomesValidos.Add(n); }
                    }

                    // 2. Busca todos os complemento_grupo ativos deste produto no Supabase
                    var grpsSupabase = await GetAsync($"{TBL_COMP_GRUPO}?mercadoria_id=eq.{produtoUuid}&ativo=eq.true&select=id,nome");
                    foreach (JObject g in grpsSupabase)
                    {
                        var gNome = g["nome"]?.ToString() ?? "";
                        var gId   = g["id"]?.ToString()   ?? "";
                        if (string.IsNullOrWhiteSpace(gId)) continue;
                        if (!nomesValidos.Contains(gNome))
                        {
                            await PatchAsync(TBL_COMP_GRUPO, $"id=eq.{gId}", new { ativo = false });
                            Logger.Log("SupabaseService", "SincronizarFracionadoAsync",
                                $"Produto {codigoMercadoria}: complemento_grupo '{gNome}' desativado (órfão)");
                        }
                    }

                    // 3. Faz o mesmo via link table (mercadoria_complemento_grupo) para grupos
                    //    que podem não ter mercadoria_id direto mas estão vinculados via link table
                    var lnkArr = await GetAsync($"{TBL_MERC_COMP_GRP}?mercadoria_id=eq.{produtoUuid}&select=grupo_id");
                    foreach (JObject lnk in lnkArr)
                    {
                        var gid = lnk["grupo_id"]?.ToString() ?? "";
                        if (string.IsNullOrWhiteSpace(gid)) continue;
                        var grpChk = await GetAsync($"{TBL_COMP_GRUPO}?id=eq.{gid}&ativo=eq.true&limit=1");
                        if (grpChk.Count == 0) continue;
                        var gNome2 = grpChk[0]["nome"]?.ToString() ?? "";
                        if (!nomesValidos.Contains(gNome2))
                        {
                            await PatchAsync(TBL_COMP_GRUPO, $"id=eq.{gid}", new { ativo = false });
                            Logger.Log("SupabaseService", "SincronizarFracionadoAsync",
                                $"Produto {codigoMercadoria}: complemento_grupo '{gNome2}' (via link) desativado (órfão)");
                        }
                    }
                }
                catch (Exception exClean)
                {
                    Logger.Log("SupabaseService", "SincronizarFracionadoAsync", $"Erro limpeza complementos órfãos produto {codigoMercadoria}", exClean);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarFracionadoAsync", $"Erro produto {codigoMercadoria}", ex);
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
                string codigoCupomStr = "";
                using (var cmdFind = new MySqlCommand(
                    "SELECT Codigo, cupomCodigo, COALESCE(cupomLimite_Usos,0) AS lim, " +
                    "COALESCE(cupomUsos_Realizados,0) AS usos " +
                    "FROM cupom WHERE supabase_uuid=@u LIMIT 1", conn))
                {
                    cmdFind.Parameters.AddWithValue("@u", cupomSupabaseId);
                    using var r = cmdFind.ExecuteReader();
                    if (!r.Read()) return;
                    codCupom        = Convert.ToInt32(r["Codigo"]);
                    codigoCupomStr  = r["cupomCodigo"]?.ToString() ?? "";
                    limite          = Convert.ToInt32(r["lim"]);
                    usos            = Convert.ToInt32(r["usos"]);
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
                if (codigoCupomStr.StartsWith("FID", StringComparison.OrdinalIgnoreCase))
                {
                    // FID coupons need special sync that preserves cliente_id
                    int codigoCliente = 0;
                    using (var cmdH = new MySqlCommand(
                        "SELECT Codigo_Cliente FROM historico_fidelizacao WHERE fidCupomCodigo=@c LIMIT 1", conn))
                    {
                        cmdH.Parameters.AddWithValue("@c", codigoCupomStr);
                        var obj = cmdH.ExecuteScalar();
                        if (obj != null && obj != DBNull.Value) codigoCliente = Convert.ToInt32(obj);
                    }
                    if (codigoCliente > 0)
                    {
                        string cod = codigoCupomStr; int cli = codigoCliente;
                        Task.Run(async () => await SincronizarCupomFidelizacaoAsync(cod, cli));
                    }
                }
                else
                {
                    Task.Run(async () => await SincronizarCupomAsync(codCupom));
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "RegistrarUsoCupomLocal", "Erro", ex);
            }
        }

        // ── (end of new methods) ──────────────────────────────────────────────

        public static async Task<List<string>> SincronizarTodosGruposAsync()
        {
            var erros = new List<string>();
            try
            {
                var codigos = new List<int>();
                using (var conn = AbrirMysql())
                using (var cmd  = new MySqlCommand(
                    "SELECT Codigo FROM grupo_mercadoria", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) codigos.Add(Convert.ToInt32(r["Codigo"]));
                }
                foreach (var cod in codigos)
                {
                    var err = await SincronizarGrupoAsync(cod);
                    if (!string.IsNullOrWhiteSpace(err)) erros.Add($"Categoria {cod}: {err}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodosGruposAsync", "Erro", ex);
                erros.Add($"Categorias: {ex.Message}");
            }
            return erros;
        }

        /// <summary>
        /// Syncs only catalog-related tables (categories, products, marmitas, images).
        /// Called every few minutes as a background catch-all — faster than SincronizarTudoAsync.
        /// </summary>
        public static async Task SincronizarCatalogoAsync()
        {
            if (!SiteConectado) return;
            // Se já há uma sincronização em curso, pula — evita duplicatas
            if (!await _syncLock.WaitAsync(0)) return;
            try
            {
                // Limpa apenas o cache de grupos já sincronizados nesta sessão para forçar
                // re-verificação dos registros. NÃO reinicia as flags de detecção de colunas
                // (_supabaseTemColEmpresaCodigo etc.) pois isso causa falsos "not found" e duplicatas.
                _gruposSincronizados.Clear();
                await SincronizarTodosGruposAsync();
                await SincronizarTodosProdutosAsync();
                await SincronizarTodasMarmitasAsync();
                await SincronizarTodosVinculosAsync();
                await SincronizarTodasImagensAsync();
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarCatalogoAsync", "Erro", ex);
            }
            finally
            {
                _syncLock.Release();
            }
        }

        /// <summary>
        /// Runs a live connectivity and permissions diagnostic against Supabase.
        /// Tests GET, INSERT and DELETE for each relevant table and returns the raw
        /// HTTP status codes and response bodies so any misconfiguration is visible.
        /// </summary>
        public static async Task<string> DiagnosticaAsync()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Diagnóstico Supabase — {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"URL: {BASE}");
            sb.AppendLine(new string('═', 70));

            // ── MySQL ─────────────────────────────────────────────────────────
            sb.AppendLine("\n[MySQL]");
            try
            {
                using var conn = AbrirMysql();
                using var cmd  = new MySqlCommand("SELECT COUNT(*) FROM mercadoria", conn);
                sb.AppendLine($"  ✓ Conectado — {cmd.ExecuteScalar()} produto(s) no banco local");
            }
            catch (Exception ex) { sb.AppendLine($"  ✗ ERRO: {ex.Message}"); }

            // ── GET /loja  (basic auth test) ──────────────────────────────────
            sb.AppendLine("\n[GET /loja]");
            try
            {
                var r    = await _http.GetAsync($"{BASE}/loja?limit=1");
                var body = await r.Content.ReadAsStringAsync();
                sb.AppendLine($"  Status: {(int)r.StatusCode} {r.ReasonPhrase}");
                if (body.Length > 0) sb.AppendLine("  " + body[..Math.Min(180, body.Length)]);
            }
            catch (Exception ex) { sb.AppendLine($"  ✗ ERRO: {ex.Message}"); }

            // ── GET /mercadoria ──────────────────────────────────────────────
            sb.AppendLine("\n[GET /mercadoria]");
            try
            {
                var r    = await _http.GetAsync($"{BASE}/mercadoria?limit=2");
                var body = await r.Content.ReadAsStringAsync();
                sb.AppendLine($"  Status: {(int)r.StatusCode} {r.ReasonPhrase}");
                if (body.Length > 0) sb.AppendLine("  " + body[..Math.Min(300, body.Length)]);
            }
            catch (Exception ex) { sb.AppendLine($"  ✗ ERRO: {ex.Message}"); }

            // ── POST /mercadoria (campos mínimos) ─────────────────────────────
            sb.AppendLine("\n[POST /mercadoria — campos mínimos]");
            string testId1 = null;
            try
            {
                var json = JsonConvert.SerializeObject(new
                    { nome = "_DIAG_MIN_", preco_venda = 0.01m, ativo = false });
                using var req = new HttpRequestMessage(HttpMethod.Post, $"{BASE}/mercadoria")
                    { Content = new StringContent(json, Encoding.UTF8, "application/json") };
                req.Headers.Add("Prefer", "return=representation");
                var r    = await _http.SendAsync(req);
                var body = await r.Content.ReadAsStringAsync();
                sb.AppendLine($"  Status: {(int)r.StatusCode} {r.ReasonPhrase}");
                sb.AppendLine("  " + body[..Math.Min(400, body.Length)]);
                if (r.IsSuccessStatusCode)
                    try { testId1 = (JToken.Parse(body) is JArray a ? a[0] : JToken.Parse(body))?["id"]?.ToString(); } catch { }
            }
            catch (Exception ex) { sb.AppendLine($"  ✗ ERRO: {ex.Message}"); }
            if (!string.IsNullOrEmpty(testId1))
            {
                try
                {
                    using var d = new HttpRequestMessage(HttpMethod.Delete, $"{BASE}/mercadoria?id=eq.{testId1}");
                    d.Headers.Add("Prefer", "return=minimal");
                    await _http.SendAsync(d);
                    sb.AppendLine("  (registro de teste removido ✓)");
                }
                catch { }
            }

            // ── POST /mercadoria (payload completo) ───────────────────────────
            sb.AppendLine("\n[POST /mercadoria — payload completo]");
            string testId2 = null;
            try
            {
                var json = JsonConvert.SerializeObject(new
                {
                    nome        = "_DIAG_FULL_",
                    descricao   = "",
                    preco_venda = 0.01m,
                    imagem_url  = "",
                    ativo       = false,
                    destaque    = false
                });
                using var req = new HttpRequestMessage(HttpMethod.Post, $"{BASE}/mercadoria")
                    { Content = new StringContent(json, Encoding.UTF8, "application/json") };
                req.Headers.Add("Prefer", "return=representation");
                var r    = await _http.SendAsync(req);
                var body = await r.Content.ReadAsStringAsync();
                sb.AppendLine($"  Status: {(int)r.StatusCode} {r.ReasonPhrase}");
                sb.AppendLine("  " + body[..Math.Min(400, body.Length)]);
                if (r.IsSuccessStatusCode)
                    try { testId2 = (JToken.Parse(body) is JArray a ? a[0] : JToken.Parse(body))?["id"]?.ToString(); } catch { }
            }
            catch (Exception ex) { sb.AppendLine($"  ✗ ERRO: {ex.Message}"); }
            if (!string.IsNullOrEmpty(testId2))
            {
                try
                {
                    using var d = new HttpRequestMessage(HttpMethod.Delete, $"{BASE}/mercadoria?id=eq.{testId2}");
                    d.Headers.Add("Prefer", "return=minimal");
                    await _http.SendAsync(d);
                    sb.AppendLine("  (registro de teste removido ✓)");
                }
                catch { }
            }

            // ── GET /grupo_mercadoria ─────────────────────────────────────────
            sb.AppendLine("\n[GET /grupo_mercadoria]");
            try
            {
                var r    = await _http.GetAsync($"{BASE}/grupo_mercadoria?limit=2");
                var body = await r.Content.ReadAsStringAsync();
                sb.AppendLine($"  Status: {(int)r.StatusCode} {r.ReasonPhrase}");
                if (body.Length > 0) sb.AppendLine("  " + body[..Math.Min(200, body.Length)]);
            }
            catch (Exception ex) { sb.AppendLine($"  ✗ ERRO: {ex.Message}"); }

            // ── GET /bairro ───────────────────────────────────────────────────
            sb.AppendLine("\n[GET /bairro]");
            try
            {
                var r    = await _http.GetAsync($"{BASE}/bairro?limit=1");
                var body = await r.Content.ReadAsStringAsync();
                sb.AppendLine($"  Status: {(int)r.StatusCode} {r.ReasonPhrase}");
                if (body.Length > 0) sb.AppendLine("  " + body[..Math.Min(200, body.Length)]);
            }
            catch (Exception ex) { sb.AppendLine($"  ✗ ERRO: {ex.Message}"); }

            // ── GET /cupom ────────────────────────────────────────────────────
            sb.AppendLine("\n[GET /cupom]");
            try
            {
                var r    = await _http.GetAsync($"{BASE}/cupom?limit=1");
                var body = await r.Content.ReadAsStringAsync();
                sb.AppendLine($"  Status: {(int)r.StatusCode} {r.ReasonPhrase}");
                if (body.Length > 0) sb.AppendLine("  " + body[..Math.Min(200, body.Length)]);
            }
            catch (Exception ex) { sb.AppendLine($"  ✗ ERRO: {ex.Message}"); }

            // ── SYNC REAL: 1ª categoria do MySQL → grupo_mercadoria ──────────
            sb.AppendLine("\n[SYNC REAL — 1ª categoria MySQL → grupo_mercadoria]");
            try
            {
                int codGrupo = 0;
                string nomeGrupo = "", imagemGrupo = ""; int ordemGrupo = 0;
                using (var conn = AbrirMysql())
                using (var cmd = new MySqlCommand(
                    "SELECT Codigo, grmeDescricao_, COALESCE(grmeOrdem,0) AS grmeOrdem, " +
                    "COALESCE(grmeImagem_Url,'') AS grmeImagem_Url FROM grupo_mercadoria LIMIT 1", conn))
                {
                    using var r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        codGrupo   = Convert.ToInt32(r["Codigo"]);
                        nomeGrupo  = r["grmeDescricao_"]?.ToString() ?? "";
                        ordemGrupo = Convert.ToInt32(r["grmeOrdem"]);
                        imagemGrupo = r["grmeImagem_Url"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(imagemGrupo) &&
                            !imagemGrupo.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                            imagemGrupo = "";
                    }
                }
                if (codGrupo > 0)
                {
                    sb.AppendLine($"  MySQL Codigo={codGrupo} nome=\"{nomeGrupo}\"");
                    var payload = new { nome = nomeGrupo, ordem = ordemGrupo, ativo = true, imagem_url = imagemGrupo };
                    var payloadJson = JsonConvert.SerializeObject(payload);
                    sb.AppendLine($"  Payload: {payloadJson}");

                    // Check if already exists
                    var existentes = await GetAsync($"grupo_mercadoria?nome=eq.{Uri.EscapeDataString(nomeGrupo)}{EmpresaFilter()}&limit=1");
                    if (existentes.Count > 0)
                    {
                        var existId = existentes[0]["id"]?.ToString();
                        sb.AppendLine($"  Já existe no Supabase id={existId} — fazendo PATCH");
                        using var req = new HttpRequestMessage(new HttpMethod("PATCH"),
                            $"{BASE}/grupo_mercadoria?id=eq.{existId}")
                        { Content = new StringContent(payloadJson, Encoding.UTF8, "application/json") };
                        req.Headers.Add("Prefer", "return=representation");
                        var r = await _http.SendAsync(req);
                        var body = await r.Content.ReadAsStringAsync();
                        sb.AppendLine($"  PATCH Status: {(int)r.StatusCode} {r.ReasonPhrase}");
                        sb.AppendLine("  " + body[..Math.Min(300, body.Length)]);
                    }
                    else
                    {
                        sb.AppendLine("  Não existe — fazendo POST");
                        using var req = new HttpRequestMessage(HttpMethod.Post, $"{BASE}/grupo_mercadoria")
                        { Content = new StringContent(payloadJson, Encoding.UTF8, "application/json") };
                        req.Headers.Add("Prefer", "return=representation");
                        var r = await _http.SendAsync(req);
                        var body = await r.Content.ReadAsStringAsync();
                        sb.AppendLine($"  POST Status: {(int)r.StatusCode} {r.ReasonPhrase}");
                        sb.AppendLine("  " + body[..Math.Min(300, body.Length)]);
                    }
                }
                else sb.AppendLine("  Nenhuma categoria encontrada no MySQL.");
            }
            catch (Exception ex) { sb.AppendLine($"  ✗ ERRO: {ex.Message}"); }

            // ── SYNC REAL: 1º produto do MySQL → mercadoria ──────────────────
            sb.AppendLine("\n[SYNC REAL — 1º produto MySQL → mercadoria]");
            try
            {
                int codMerc = 0;
                string nomeMerc = "", descMerc = "", imgMerc = "";
                decimal precoMerc = 0m;
                using (var conn = AbrirMysql())
                using (var cmd = new MySqlCommand(
                    "SELECT Codigo, mercMercadoria, COALESCE(mercApresentacao,'') AS mercApresentacao, " +
                    "COALESCE(mercPreco_Venda,0) AS mercPreco_Venda, " +
                    "COALESCE(mercImagem_Url,'') AS mercImagem_Url FROM mercadoria WHERE Situacao='A' LIMIT 1", conn))
                {
                    using var r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        codMerc   = Convert.ToInt32(r["Codigo"]);
                        nomeMerc  = r["mercMercadoria"]?.ToString() ?? "";
                        descMerc  = r["mercApresentacao"]?.ToString() ?? "";
                        precoMerc = Convert.ToDecimal(r["mercPreco_Venda"]);
                        imgMerc   = r["mercImagem_Url"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(imgMerc) &&
                            !imgMerc.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                            imgMerc = "";
                    }
                }
                if (codMerc > 0)
                {
                    sb.AppendLine($"  MySQL Codigo={codMerc} nome=\"{nomeMerc}\" preco={precoMerc}");
                    var payload = new { nome = nomeMerc, descricao = descMerc, preco_venda = precoMerc,
                                        imagem_url = imgMerc, ativo = true, destaque = false };
                    var payloadJson = JsonConvert.SerializeObject(payload);
                    sb.AppendLine($"  Payload: {payloadJson}");

                    // Check if already exists
                    var existentes = await GetAsync($"mercadoria?nome=eq.{Uri.EscapeDataString(nomeMerc)}{EmpresaFilter()}&limit=1");
                    if (existentes.Count > 0)
                    {
                        var existId = existentes[0]["id"]?.ToString();
                        sb.AppendLine($"  Já existe no Supabase id={existId} — fazendo PATCH");
                        using var req = new HttpRequestMessage(new HttpMethod("PATCH"),
                            $"{BASE}/mercadoria?id=eq.{existId}")
                        { Content = new StringContent(payloadJson, Encoding.UTF8, "application/json") };
                        req.Headers.Add("Prefer", "return=representation");
                        var r = await _http.SendAsync(req);
                        var body = await r.Content.ReadAsStringAsync();
                        sb.AppendLine($"  PATCH Status: {(int)r.StatusCode} {r.ReasonPhrase}");
                        sb.AppendLine("  " + body[..Math.Min(300, body.Length)]);
                    }
                    else
                    {
                        sb.AppendLine("  Não existe — fazendo POST");
                        using var req = new HttpRequestMessage(HttpMethod.Post, $"{BASE}/mercadoria")
                        { Content = new StringContent(payloadJson, Encoding.UTF8, "application/json") };
                        req.Headers.Add("Prefer", "return=representation");
                        var r = await _http.SendAsync(req);
                        var body = await r.Content.ReadAsStringAsync();
                        sb.AppendLine($"  POST Status: {(int)r.StatusCode} {r.ReasonPhrase}");
                        sb.AppendLine("  " + body[..Math.Min(300, body.Length)]);
                        // Clean up test if it worked
                        if (r.IsSuccessStatusCode)
                        {
                            try
                            {
                                var inserted = JToken.Parse(body);
                                var insId = (inserted is JArray aa ? aa[0] : inserted)?["id"]?.ToString();
                                if (!string.IsNullOrWhiteSpace(insId))
                                {
                                    using var d = new HttpRequestMessage(HttpMethod.Delete, $"{BASE}/mercadoria?id=eq.{insId}");
                                    d.Headers.Add("Prefer", "return=minimal");
                                    await _http.SendAsync(d);
                                    sb.AppendLine("  (registro de teste removido ✓ — rode o Enviar Tudo ao Site para enviar de verdade)");
                                }
                            }
                            catch { }
                        }
                    }
                }
                else sb.AppendLine("  Nenhum produto ativo encontrado no MySQL.");
            }
            catch (Exception ex) { sb.AppendLine($"  ✗ ERRO: {ex.Message}"); }

            sb.AppendLine("\n" + new string('═', 70));
            return sb.ToString();
        }

        public static async Task SincronizarTudoAsync()
        {
            if (!SiteConectado) return;
            if (!await _syncLock.WaitAsync(0)) return; // skip se já há sync em curso
            try
            {
                await SincronizarLojaAsync();
                await SincronizarFormasPagamentoAsync();
                await SincronizarTodosGruposAsync();
                await SincronizarTodosProdutosAsync();
                await SincronizarTodasMarmitasAsync();
                await SincronizarTodosVinculosAsync();
                await SincronizarTodasImagensAsync();
                await SincronizarTodosCuponsAsync();
                await SincronizarTodosCuponsFidelizacaoAsync();
                await SincronizarTodosBairrosAsync();
                await SincronizarTodosClientesAsync();
                try { await CorrigirEmpresaCodigoNuloAsync(); } catch { }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTudoAsync", "Erro", ex);
            }
            finally
            {
                _syncLock.Release();
            }
        }

        /// <summary>
        /// Full sync with step-by-step progress reporting. Returns accumulated errors.
        /// Uses IProgress&lt;string&gt; so callbacks are marshalled back to the UI thread automatically.
        /// </summary>
        public static async Task<string> SincronizarTudoComProgressoAsync(IProgress<string> progress)
        {
            if (!SiteConectado) return "Conexão com o site está desabilitada.";
            // Aguarda lock: sincronização manual sempre executa, mas espera a automática terminar
            progress?.Report("Aguardando sincronização automática em curso...");
            await _syncLock.WaitAsync();
            var todosErros = new List<string>();
            _gruposSincronizados.Clear(); // limpa cache de grupos desta sessão (não reseta flags de colunas)
            try
            {
            progress?.Report("Sincronizando dados da loja...");
            await SincronizarLojaAsync();

            progress?.Report("Sincronizando formas de pagamento...");
            await SincronizarFormasPagamentoAsync();

            progress?.Report("Sincronizando categorias...");
            todosErros.AddRange(await SincronizarTodosGruposAsync());

            progress?.Report("Sincronizando produtos...");
            todosErros.AddRange(await SincronizarTodosProdutosAsync());

            progress?.Report("Sincronizando marmitas e itens...");
            await SincronizarTodasMarmitasAsync();

            progress?.Report("Sincronizando complementos e adicionais...");
            await SincronizarTodosVinculosAsync();

            progress?.Report("Sincronizando imagens...");
            await SincronizarTodasImagensAsync();

            progress?.Report("Sincronizando cupons...");
            await SincronizarTodosCuponsAsync();

            progress?.Report("Sincronizando cupôns de fidelização...");
            await SincronizarTodosCuponsFidelizacaoAsync();

            progress?.Report("Sincronizando bairros e taxas de entrega...");
            await SincronizarTodosBairrosAsync();

            progress?.Report("Sincronizando clientes...");
            await SincronizarTodosClientesAsync();

            // Garante que todos os registros ainda sem empresa_codigo (legados) sejam corrigidos.
            // Executar ao final para não afetar o _supabaseTemColEmpresaCodigo das etapas anteriores.
            progress?.Report("Corrigindo empresa_codigo em registros legados...");
            try { await CorrigirEmpresaCodigoNuloAsync(); } catch { }

            progress?.Report(todosErros.Count == 0 ? "Concluido!" : $"Concluido com {todosErros.Count} erro(s).");
            return todosErros.Count == 0 ? "" : string.Join("\n", todosErros);
            }
            finally
            {
                _syncLock.Release();
            }
        }

        /// <summary>
        /// Force-patches url_da_imagem in Supabase for every local record that already has
        /// a valid HTTP URL in MySQL. Uses UUID when available; falls back to name lookup in Supabase
        /// for records that were synced before UUID tracking was added.
        /// Safe to call repeatedly — only updates the image column.
        /// </summary>
        public static async Task SincronizarTodasImagensAsync()
        {
            try
            {
                // ── Produtos (mercadoria MySQL → mercadoria Supabase) ──────────────
                var prodRows = new List<(int codigo, string nome, string uuid, string url)>();
                using (var conn = AbrirMysql())
                using (var cmd = new MySqlCommand(
                    "SELECT Codigo, mercMercadoria, COALESCE(supabase_uuid,''), mercImagem_Url " +
                    "FROM mercadoria WHERE mercImagem_Url LIKE 'http%'", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                        prodRows.Add((r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3)));
                }
                foreach (var (codigo, nome, uuid, url) in prodRows)
                    try { await ForcarUrlImagemAsync(TBL_MERCADORIAS, "mercadoria", "Codigo", codigo, nome, url); } catch { }

                // ── Categorias (grupo_mercadoria MySQL → grupo_mercadoria Supabase) ─
                var grpRows = new List<(int codigo, string nome, string uuid, string url)>();
                using (var conn = AbrirMysql())
                using (var cmd = new MySqlCommand(
                    "SELECT Codigo, grmeDescricao_, COALESCE(supabase_uuid,''), grmeImagem_Url " +
                    "FROM grupo_mercadoria WHERE grmeImagem_Url LIKE 'http%'", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                        grpRows.Add((r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3)));
                }
                foreach (var (codigo, nome, uuid, url) in grpRows)
                    try { await ForcarUrlImagemAsync("grupo_mercadoria", "grupo_mercadoria", "Codigo", codigo, nome, url, "imagem_url"); } catch { }

                // ── Marmitas (marmita MySQL → mercadoria Supabase, same table as products) ─
                var marRows = new List<(int codigo, string nome, string uuid, string url)>();
                using (var conn = AbrirMysql())
                using (var cmd = new MySqlCommand(
                    "SELECT Codigo, marDescricao, COALESCE(supabase_uuid,''), marImagem_Url " +
                    "FROM marmita WHERE marImagem_Url LIKE 'http%'", conn))
                {
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                        marRows.Add((r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3)));
                }
                foreach (var (codigo, nome, uuid, url) in marRows)
                    try { await ForcarUrlImagemAsync(TBL_MERCADORIAS, "marmita", "Codigo", codigo, nome, url); } catch { }
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "SincronizarTodasImagensAsync", "Erro", ex);
            }
        }

        /// <summary>
        /// Finds the Supabase UUID for a row using MySQL first, then a name lookup in Supabase as
        /// fallback (handles records that exist in Supabase but whose UUID was never stored locally).
        /// Saves UUID back to MySQL if found via fallback. Then PATCHes url_da_imagem.
        /// </summary>
        private static async Task ForcarUrlImagemAsync(
            string supabaseTable, string mysqlTable, string pkCol, int pkVal,
            string nome, string publicUrl, string imageColName = "imagem_url")
        {
            // 1. Try local UUID cache
            string uuid = GetSupabaseUuid(mysqlTable, pkCol, pkVal);

            // 2. Fallback: search Supabase by nome (handles missing UUID in MySQL)
            if (string.IsNullOrWhiteSpace(uuid))
            {
                try
                {
                    var arr = await GetAsync(
                        $"{supabaseTable}?nome=eq.{Uri.EscapeDataString(nome)}{EmpresaFilter()}&limit=1");
                    if (arr.Count > 0)
                    {
                        uuid = arr[0]["id"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(uuid))
                            SaveSupabaseUuid(mysqlTable, pkCol, pkVal, uuid);
                    }
                }
                catch { }
            }

            // 3. PATCH if we have the UUID
            if (!string.IsNullOrWhiteSpace(uuid))
            {
                var patch = new Dictionary<string, object> { [imageColName] = publicUrl };
                await PatchAsync(supabaseTable, $"id=eq.{uuid}",
                    JsonConvert.DeserializeObject(JsonConvert.SerializeObject(patch)));
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
                string publicUrl;

                string imgbbKey = GetImgBBKey();

                if (!string.IsNullOrWhiteSpace(imgbbKey))
                {
                    publicUrl = await UploadImgBBAsync(localPath, imgbbKey);
                }
                else
                {
                    // Upload to Supabase Storage bucket "categorias"
                    var mimeType    = ext == ".png" ? "image/png"
                                    : ext == ".gif" ? "image/gif"
                                    : "image/jpeg";
                    var storageBase = "https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object";
                    var uploadUrl   = $"{storageBase}/categorias/{fileName}";

                    using var content = new ByteArrayContent(System.IO.File.ReadAllBytes(localPath));
                    content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                    using var req = new HttpRequestMessage(HttpMethod.Post, uploadUrl) { Content = content };
                    req.Headers.Add("apikey", KEY);
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", KEY);
                    req.Headers.Add("x-upsert", "true");

                    var resp = await _http.SendAsync(req);
                    var body = await resp.Content.ReadAsStringAsync();
                    if (!resp.IsSuccessStatusCode)
                        throw new Exception($"Storage upload {resp.StatusCode}: {body}");

                    publicUrl = $"https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object/public/categorias/{fileName}";
                }

                // Persist public URL to local DB
                using var conn = AbrirMysql();
                using var cmd  = new MySqlCommand(
                    "UPDATE grupo_mercadoria SET grmeImagem_Url=@url WHERE Codigo=@c", conn);
                cmd.Parameters.AddWithValue("@url", publicUrl);
                cmd.Parameters.AddWithValue("@c",   codigoGrupo);
                cmd.ExecuteNonQuery();

                // Read category name for Supabase lookup fallback
                string grpNome = "";
                using (var cmd2 = new MySqlCommand(
                    "SELECT grmeDescricao_ FROM grupo_mercadoria WHERE Codigo=@c LIMIT 1", conn))
                {
                    cmd2.Parameters.AddWithValue("@c", codigoGrupo);
                    grpNome = cmd2.ExecuteScalar()?.ToString() ?? "";
                }

                // Push URL to Supabase — bypasses habSite gate, finds UUID by name if not in MySQL
                await ForcarUrlImagemAsync("grupo_mercadoria", "grupo_mercadoria",
                    "Codigo", codigoGrupo, grpNome, publicUrl, "imagem_url");
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "UploadCategoriaImagemAsync", $"Erro categoria {codigoGrupo}", ex);
            }
        }

        public static async Task UploadMarmitaImagemAsync(int codigoMarmita, string localPath)
        {
            try
            {
                if (!System.IO.File.Exists(localPath)) return;

                var ext      = System.IO.Path.GetExtension(localPath).ToLower();
                var fileName = $"marmita_{codigoMarmita}{ext}";
                string publicUrl;

                string imgbbKey = GetImgBBKey();

                if (!string.IsNullOrWhiteSpace(imgbbKey))
                {
                    publicUrl = await UploadImgBBAsync(localPath, imgbbKey);
                }
                else
                {
                    var mimeType    = ext == ".png" ? "image/png"
                                    : ext == ".gif" ? "image/gif"
                                    : "image/jpeg";
                    var storageBase = "https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object";
                    var uploadUrl   = $"{storageBase}/marmitas/{fileName}";

                    using var content = new ByteArrayContent(System.IO.File.ReadAllBytes(localPath));
                    content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                    using var req = new HttpRequestMessage(HttpMethod.Post, uploadUrl) { Content = content };
                    req.Headers.Add("apikey", KEY);
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", KEY);
                    req.Headers.Add("x-upsert", "true");

                    var resp = await _http.SendAsync(req);
                    var body = await resp.Content.ReadAsStringAsync();
                    if (!resp.IsSuccessStatusCode)
                        throw new Exception($"Storage upload marmita {resp.StatusCode}: {body}");

                    publicUrl = $"https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object/public/marmitas/{fileName}";
                }

                // Persist public URL to local DB
                using var conn = AbrirMysql();
                using var cmd  = new MySqlCommand(
                    "UPDATE marmita SET marImagem_Url=@url WHERE Codigo=@c", conn);
                cmd.Parameters.AddWithValue("@url", publicUrl);
                cmd.Parameters.AddWithValue("@c",   codigoMarmita);
                cmd.ExecuteNonQuery();

                // Read marmita name for Supabase lookup fallback
                string marNome = "";
                using (var cmd2 = new MySqlCommand(
                    "SELECT marDescricao FROM marmita WHERE Codigo=@c LIMIT 1", conn))
                {
                    cmd2.Parameters.AddWithValue("@c", codigoMarmita);
                    marNome = cmd2.ExecuteScalar()?.ToString() ?? "";
                }

                // Push URL to Supabase — bypasses habSite gate, finds UUID by name if not in MySQL
                await ForcarUrlImagemAsync(TBL_MERCADORIAS, "marmita",
                    "Codigo", codigoMarmita, marNome, publicUrl);
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "UploadMarmitaImagemAsync", $"Erro marmita {codigoMarmita}", ex);
            }
        }

        /// <summary>
        /// Uploads a product image to Supabase Storage (bucket: produtos)
        /// and saves the public URL back to mercadoria.mercImagem_Url.
        /// Returns empty string on success, or the error message on failure.
        /// </summary>
        public static async Task<string> UploadProdutoImagemAsync(int codigoProduto, string localPath)
        {
            try
            {
                if (!System.IO.File.Exists(localPath)) return "Arquivo n\u00e3o encontrado: " + localPath;

                var ext      = System.IO.Path.GetExtension(localPath).ToLower();
                var fileName = $"produto_{codigoProduto}{ext}";
                string publicUrl;

                string imgbbKey = GetImgBBKey();

                if (!string.IsNullOrWhiteSpace(imgbbKey))
                {
                    publicUrl = await UploadImgBBAsync(localPath, imgbbKey);
                }
                else
                {
                    var mimeType    = ext == ".png" ? "image/png"
                                    : ext == ".gif" ? "image/gif"
                                    : "image/jpeg";
                    var storageBase = "https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object";
                    var uploadUrl   = $"{storageBase}/produtos/{fileName}";

                    using var content = new ByteArrayContent(System.IO.File.ReadAllBytes(localPath));
                    content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                    using var req = new HttpRequestMessage(HttpMethod.Post, uploadUrl) { Content = content };
                    req.Headers.Add("apikey", KEY);
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", KEY);
                    req.Headers.Add("x-upsert", "true");

                    var resp = await _http.SendAsync(req);
                    var body = await resp.Content.ReadAsStringAsync();
                    if (!resp.IsSuccessStatusCode)
                        throw new Exception($"Storage upload produto {resp.StatusCode}: {body}");

                    publicUrl = $"https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object/public/produtos/{fileName}";
                }

                // Persist public URL to local DB
                using var conn = AbrirMysql();
                using var cmd  = new MySqlCommand(
                    "UPDATE mercadoria SET mercImagem_Url=@url WHERE Codigo=@c", conn);
                cmd.Parameters.AddWithValue("@url", publicUrl);
                cmd.Parameters.AddWithValue("@c",   codigoProduto);
                cmd.ExecuteNonQuery();

                // Read product name for Supabase lookup fallback
                string prodNome = "";
                using (var cmd2 = new MySqlCommand(
                    "SELECT mercMercadoria FROM mercadoria WHERE Codigo=@c LIMIT 1", conn))
                {
                    cmd2.Parameters.AddWithValue("@c", codigoProduto);
                    prodNome = cmd2.ExecuteScalar()?.ToString() ?? "";
                }

                // Push URL to Supabase — bypasses habSite gate, finds UUID by name if not in MySQL
                await ForcarUrlImagemAsync(TBL_MERCADORIAS, "mercadoria",
                    "Codigo", codigoProduto, prodNome, publicUrl);
                return "";
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "UploadProdutoImagemAsync", $"Erro produto {codigoProduto}", ex);
                return ex.Message;
            }
        }

        /// <summary>Faz upload da logo da empresa para ImgBB e retorna a URL pública.</summary>
        public static async Task<string> UploadLogoEmpresaAsync(string localPath)
        {
            try
            {
                if (!System.IO.File.Exists(localPath)) return "";

                // Tenta ImgBB se a chave estiver configurada
                string imgbbKey = GetImgBBKey();
                if (!string.IsNullOrWhiteSpace(imgbbKey))
                {
                    string imgbbUrl = await UploadImgBBAsync(localPath, imgbbKey);
                    if (!string.IsNullOrWhiteSpace(imgbbUrl)) return imgbbUrl;
                }

                // Fallback: Supabase Storage (bucket "produtos", pasta "logos/")
                var ext      = System.IO.Path.GetExtension(localPath).ToLower();
                if (string.IsNullOrEmpty(ext)) ext = ".jpg";
                var fileName    = $"logos/logo_empresa{ext}";
                var mimeType    = ext == ".png" ? "image/png"
                                : ext == ".gif" ? "image/gif"
                                : ext == ".webp" ? "image/webp"
                                : "image/jpeg";
                var storageBase = "https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object";
                var uploadUrl   = $"{storageBase}/produtos/{fileName}";

                using var content = new ByteArrayContent(System.IO.File.ReadAllBytes(localPath));
                content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                using var req = new HttpRequestMessage(HttpMethod.Post, uploadUrl) { Content = content };
                req.Headers.Add("apikey", KEY);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", KEY);
                req.Headers.Add("x-upsert", "true");

                var resp = await _http.SendAsync(req);
                if (resp.IsSuccessStatusCode)
                    return $"https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object/public/produtos/{fileName}";

                var errBody = await resp.Content.ReadAsStringAsync();
                Logger.Log("SupabaseService", "UploadLogoEmpresaAsync", $"Storage upload falhou {resp.StatusCode}: {errBody}");
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "UploadLogoEmpresaAsync", "Erro upload logo", ex);
            }
            return "";
        }

        public static async Task<string> UploadBannerEmpresaAsync(string localPath)
        {
            try
            {
                if (!System.IO.File.Exists(localPath)) return "";

                // Tenta ImgBB se a chave estiver configurada
                string imgbbKey = GetImgBBKey();
                if (!string.IsNullOrWhiteSpace(imgbbKey))
                {
                    string imgbbUrl = await UploadImgBBAsync(localPath, imgbbKey);
                    if (!string.IsNullOrWhiteSpace(imgbbUrl)) return imgbbUrl;
                }

                // Fallback: Supabase Storage (bucket "produtos", pasta "banners/")
                var ext      = System.IO.Path.GetExtension(localPath).ToLower();
                if (string.IsNullOrEmpty(ext)) ext = ".jpg";
                var fileName    = $"banners/banner_empresa{ext}";
                var mimeType    = ext == ".png" ? "image/png"
                                : ext == ".gif" ? "image/gif"
                                : ext == ".webp" ? "image/webp"
                                : "image/jpeg";
                var storageBase = "https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object";
                var uploadUrl   = $"{storageBase}/produtos/{fileName}";

                using var content = new ByteArrayContent(System.IO.File.ReadAllBytes(localPath));
                content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                using var req = new HttpRequestMessage(HttpMethod.Post, uploadUrl) { Content = content };
                req.Headers.Add("apikey", KEY);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", KEY);
                req.Headers.Add("x-upsert", "true");

                var resp = await _http.SendAsync(req);
                if (resp.IsSuccessStatusCode)
                    return $"https://uwgcmnmzjjinfmxlskks.supabase.co/storage/v1/object/public/produtos/{fileName}";

                var errBody = await resp.Content.ReadAsStringAsync();
                Logger.Log("SupabaseService", "UploadBannerEmpresaAsync", $"Storage upload falhou {resp.StatusCode}: {errBody}");
            }
            catch (Exception ex)
            {
                Logger.Log("SupabaseService", "UploadBannerEmpresaAsync", "Erro upload banner", ex);
            }
            return "";
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
        public string Id          { get; set; }
        public string Nome        { get; set; }
        public string Telefone    { get; set; }
        public string CpfCnpj     { get; set; }
        public string Endereco    { get; set; } = "";
        public string Numero      { get; set; } = "";
        public string Complemento { get; set; } = "";
        public string Bairro      { get; set; } = "";
        public string Cidade      { get; set; } = "";
        public string Uf          { get; set; } = "";
        public string Cep         { get; set; } = "";
        public string Email       { get; set; } = "";
    }
}
