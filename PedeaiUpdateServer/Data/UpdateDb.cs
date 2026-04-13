using PedeaiUpdateServer.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PedeaiUpdateServer.Data
{
    /// <summary>
    /// Repositório Supabase via REST API (PostgREST).
    /// As tabelas devem ser criadas previamente rodando Data/init_supabase.sql
    /// no editor SQL do projeto Supabase.
    /// </summary>
    public class UpdateDb
    {
        private readonly HttpClient _http;
        private readonly string     _base;

        private static readonly JsonSerializerOptions _jsOpts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public UpdateDb(string supabaseUrl, string supabaseKey)
        {
            _base = supabaseUrl.TrimEnd('/') + "/rest/v1/";
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("apikey",        supabaseKey);
            _http.DefaultRequestHeaders.Add("Authorization", "Bearer " + supabaseKey);
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private T[] Get<T>(string table, string query = "")
        {
            string url  = _base + table + (string.IsNullOrEmpty(query) ? "" : "?" + query);
            var    resp = _http.GetAsync(url).GetAwaiter().GetResult();
            string body = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Supabase GET {table} [{(int)resp.StatusCode}]: {body}");
            return JsonSerializer.Deserialize<T[]>(body, _jsOpts) ?? Array.Empty<T>();
        }

        private T PostOne<T>(string table, object body)
        {
            var json = JsonSerializer.Serialize(body, _jsOpts);
            var req  = new HttpRequestMessage(HttpMethod.Post, _base + table);
            req.Headers.Add("Prefer", "return=representation");
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp     = _http.SendAsync(req).GetAwaiter().GetResult();
            string rBody = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Supabase POST {table} [{(int)resp.StatusCode}]: {rBody}");
            var arr = JsonSerializer.Deserialize<T[]>(rBody, _jsOpts);
            return arr != null && arr.Length > 0 ? arr[0] : default;
        }

        private void Patch(string table, string filter, object body)
        {
            var json = JsonSerializer.Serialize(body, _jsOpts);
            var req  = new HttpRequestMessage(new HttpMethod("PATCH"), _base + table + "?" + filter);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var    resp  = _http.SendAsync(req).GetAwaiter().GetResult();
            string rBody = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Supabase PATCH {table} [{(int)resp.StatusCode}]: {rBody}");
        }

        // ── Clientes ──────────────────────────────────────────────────────────────

        public long RegistrarCliente(string codigoEmpresa, string nomeEmpresa)
        {
            string cod      = codigoEmpresa.Trim().ToUpperInvariant();
            var    existing = Get<ClienteRow>("Clientes",
                                "CodigoEmpresa=eq." + Uri.EscapeDataString(cod) + "&select=Id");
            if (existing.Length > 0) return existing[0].Id;

            var row = PostOne<ClienteRow>("Clientes", new
            {
                CodigoEmpresa = cod,
                NomeEmpresa   = nomeEmpresa ?? "",
                Nivel         = 2,
                Bloqueado     = false,
                DataRegistro  = DateTime.UtcNow
            });
            return row?.Id ?? 0;
        }

        public Cliente ObterCliente(long id)
        {
            var rows = Get<ClienteRow>("Clientes", "Id=eq." + id);
            return rows.Length > 0 ? MapCliente(rows[0]) : null;
        }

        public List<Cliente> ListarClientes()
        {
            var rows = Get<ClienteRow>("Clientes", "order=Id.asc");
            var list = new List<Cliente>();
            foreach (var r in rows) list.Add(MapCliente(r));
            return list;
        }

        public void AtualizarUltimaConsulta(long clienteId)
            => Patch("Clientes", "Id=eq." + clienteId, new { UltimaConsulta = DateTime.UtcNow });

        public void AtualizarVersaoCliente(long clienteId, string versao)
            => Patch("Clientes", "Id=eq." + clienteId, new { VersaoAtual = versao ?? "" });

        public void AlterarNivelCliente(long clienteId, int nivel)
            => Patch("Clientes", "Id=eq." + clienteId, new { Nivel = nivel });

        public void AlterarBloqueioCliente(long clienteId, bool bloqueado)
            => Patch("Clientes", "Id=eq." + clienteId, new { Bloqueado = bloqueado });

        // ── Pacotes ───────────────────────────────────────────────────────────────

        public long InserirPacote(Pacote p)
        {
            var row = PostOne<PacoteRow>("Pacotes", new
            {
                Versao         = p.Versao,
                Nivel          = p.Nivel,
                Descricao      = p.Descricao ?? "",
                CaminhoArquivo = p.CaminhoArquivo,
                TamanhoBytes   = p.TamanhoBytes,
                TemSQL         = p.TemSQL,
                DataPublicacao = DateTime.UtcNow,
                Ativo          = true
            });
            return row?.Id ?? 0;
        }

        public Pacote ObterPacoteParaCliente(long clienteId, string versaoAtual)
        {
            var cliente = ObterCliente(clienteId);
            if (cliente == null || cliente.Bloqueado) return null;

            string versaoEnc = Uri.EscapeDataString(versaoAtual ?? "");
            var rows = Get<PacoteRow>("Pacotes",
                "Ativo=eq.true&Nivel=lte." + cliente.Nivel +
                "&Versao=gt." + versaoEnc + "&order=Versao.desc&limit=1");
            return rows.Length > 0 ? MapPacote(rows[0]) : null;
        }

        public Pacote ObterPacote(long id)
        {
            var rows = Get<PacoteRow>("Pacotes", "Id=eq." + id + "&Ativo=eq.true");
            return rows.Length > 0 ? MapPacote(rows[0]) : null;
        }

        public List<Pacote> ListarPacotes()
        {
            var rows = Get<PacoteRow>("Pacotes", "order=Id.desc");
            var list = new List<Pacote>();
            foreach (var r in rows) list.Add(MapPacote(r));
            return list;
        }

        public void DesativarPacote(long id)
            => Patch("Pacotes", "Id=eq." + id, new { Ativo = false });

        // ── Aplicações ────────────────────────────────────────────────────────────

        public long RegistrarDownload(long clienteId, long pacoteId)
        {
            var row = PostOne<AplicacaoRow>("AplicacoesUpdate", new
            {
                ClienteId    = clienteId,
                PacoteId     = pacoteId,
                DataDownload = DateTime.UtcNow,
                Status       = "baixado"
            });
            return row?.Id ?? 0;
        }

        public void ConfirmarAplicacao(long clienteId, long pacoteId, string status, string detalhe)
        {
            var rows = Get<AplicacaoRow>("AplicacoesUpdate",
                "ClienteId=eq." + clienteId + "&PacoteId=eq." + pacoteId + "&order=Id.desc&limit=1");

            if (rows.Length > 0)
                Patch("AplicacoesUpdate", "Id=eq." + rows[0].Id, new
                {
                    DataAplicada = DateTime.UtcNow,
                    Status       = status ?? "aplicado",
                    Detalhe      = detalhe ?? ""
                });

            if (status == "aplicado")
            {
                var pacote = ObterPacote(pacoteId);
                if (pacote != null)
                    AtualizarVersaoCliente(clienteId, pacote.Versao);
            }
        }

        // ── Maps ──────────────────────────────────────────────────────────────────

        private static Cliente MapCliente(ClienteRow r) => new Cliente
        {
            Id             = r.Id,
            CodigoEmpresa  = r.CodigoEmpresa,
            NomeEmpresa    = r.NomeEmpresa,
            Nivel          = r.Nivel,
            VersaoAtual    = r.VersaoAtual,
            Bloqueado      = r.Bloqueado,
            DataRegistro   = r.DataRegistro,
            UltimaConsulta = r.UltimaConsulta
        };

        private static Pacote MapPacote(PacoteRow r) => new Pacote
        {
            Id             = r.Id,
            Versao         = r.Versao,
            Nivel          = r.Nivel,
            Descricao      = r.Descricao,
            CaminhoArquivo = r.CaminhoArquivo,
            TamanhoBytes   = r.TamanhoBytes,
            TemSQL         = r.TemSQL,
            DataPublicacao = r.DataPublicacao,
            Ativo          = r.Ativo
        };

        // ── Row DTOs ──────────────────────────────────────────────────────────────

        private class ClienteRow
        {
            public long   Id             { get; set; }
            public string CodigoEmpresa  { get; set; }
            public string NomeEmpresa    { get; set; }
            public int    Nivel          { get; set; }
            public string VersaoAtual    { get; set; }
            public bool   Bloqueado      { get; set; }
            public string DataRegistro   { get; set; }
            public string UltimaConsulta { get; set; }
        }

        private class PacoteRow
        {
            public long   Id             { get; set; }
            public string Versao         { get; set; }
            public int    Nivel          { get; set; }
            public string Descricao      { get; set; }
            public string CaminhoArquivo { get; set; }
            public long   TamanhoBytes   { get; set; }
            public bool   TemSQL         { get; set; }
            public string DataPublicacao { get; set; }
            public bool   Ativo          { get; set; }
        }

        private class AplicacaoRow
        {
            public long   Id           { get; set; }
            public long   ClienteId    { get; set; }
            public long   PacoteId     { get; set; }
            public string DataDownload { get; set; }
            public string DataAplicada { get; set; }
            public string Status       { get; set; }
            public string Detalhe      { get; set; }
        }
    }
}


