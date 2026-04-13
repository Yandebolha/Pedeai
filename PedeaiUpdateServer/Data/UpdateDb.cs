using Npgsql;
using PedeaiUpdateServer.Models;
using System;
using System.Collections.Generic;

namespace PedeaiUpdateServer.Data
{
    /// <summary>
    /// Repositório PostgreSQL (Supabase) para o servidor de atualizações.
    /// As tabelas são criadas automaticamente na primeira execução.
    /// </summary>
    public class UpdateDb
    {
        private readonly string _connStr;

        public UpdateDb(string connectionString)
        {
            _connStr = connectionString;
            EnsureCreated();
        }

        // ── Inicialização ─────────────────────────────────────────────────────────

        private void EnsureCreated()
        {
            using var conn = Open();
            Exec(conn, @"
                CREATE TABLE IF NOT EXISTS ""Clientes"" (
                    ""Id""             BIGSERIAL    PRIMARY KEY,
                    ""CodigoEmpresa""  TEXT         NOT NULL,
                    ""NomeEmpresa""    TEXT         NOT NULL DEFAULT '',
                    ""Nivel""          INTEGER      NOT NULL DEFAULT 2,
                    ""VersaoAtual""    TEXT         NOT NULL DEFAULT '',
                    ""Bloqueado""      BOOLEAN      NOT NULL DEFAULT FALSE,
                    ""DataRegistro""   TIMESTAMPTZ  NOT NULL,
                    ""UltimaConsulta"" TIMESTAMPTZ
                )");

            Exec(conn, @"
                CREATE TABLE IF NOT EXISTS ""Pacotes"" (
                    ""Id""              BIGSERIAL   PRIMARY KEY,
                    ""Versao""          TEXT        NOT NULL,
                    ""Nivel""           INTEGER     NOT NULL DEFAULT 2,
                    ""Descricao""       TEXT        NOT NULL DEFAULT '',
                    ""CaminhoArquivo""  TEXT        NOT NULL,
                    ""TamanhoBytes""    BIGINT      NOT NULL DEFAULT 0,
                    ""TemSQL""          BOOLEAN     NOT NULL DEFAULT FALSE,
                    ""DataPublicacao""  TIMESTAMPTZ NOT NULL,
                    ""Ativo""           BOOLEAN     NOT NULL DEFAULT TRUE
                )");

            Exec(conn, @"
                CREATE TABLE IF NOT EXISTS ""AplicacoesUpdate"" (
                    ""Id""           BIGSERIAL   PRIMARY KEY,
                    ""ClienteId""    BIGINT      NOT NULL,
                    ""PacoteId""     BIGINT      NOT NULL,
                    ""DataDownload"" TIMESTAMPTZ,
                    ""DataAplicada"" TIMESTAMPTZ,
                    ""Status""       TEXT        NOT NULL DEFAULT 'baixado',
                    ""Detalhe""      TEXT
                )");
        }

        // ── Clientes ──────────────────────────────────────────────────────────────

        public long RegistrarCliente(string codigoEmpresa, string nomeEmpresa)
        {
            string cod = codigoEmpresa.Trim().ToUpperInvariant();
            using var conn = Open();

            using var check = new NpgsqlCommand(
                @"SELECT ""Id"" FROM ""Clientes"" WHERE ""CodigoEmpresa""=@cod LIMIT 1", conn);
            check.Parameters.AddWithValue("@cod", cod);
            object existente = check.ExecuteScalar();
            if (existente != null) return Convert.ToInt64(existente);

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO ""Clientes"" (""CodigoEmpresa"", ""NomeEmpresa"", ""Nivel"", ""Bloqueado"", ""DataRegistro"")
                VALUES (@cod, @nome, 2, FALSE, @data)
                RETURNING ""Id""", conn);
            cmd.Parameters.AddWithValue("@cod",  cod);
            cmd.Parameters.AddWithValue("@nome", nomeEmpresa ?? "");
            cmd.Parameters.AddWithValue("@data", DateTime.UtcNow);
            return Convert.ToInt64(cmd.ExecuteScalar());
        }

        public Cliente ObterCliente(long id)
        {
            using var conn = Open();
            using var cmd  = new NpgsqlCommand(@"SELECT * FROM ""Clientes"" WHERE ""Id""=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? MapCliente(r) : null;
        }

        public List<Cliente> ListarClientes()
        {
            var list = new List<Cliente>();
            using var conn = Open();
            using var cmd  = new NpgsqlCommand(@"SELECT * FROM ""Clientes"" ORDER BY ""Id""", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapCliente(r));
            return list;
        }

        public void AtualizarUltimaConsulta(long clienteId)
        {
            using var conn = Open();
            using var cmd  = new NpgsqlCommand(
                @"UPDATE ""Clientes"" SET ""UltimaConsulta""=@d WHERE ""Id""=@id", conn);
            cmd.Parameters.AddWithValue("@d",  DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@id", clienteId);
            cmd.ExecuteNonQuery();
        }

        public void AtualizarVersaoCliente(long clienteId, string versao)
        {
            using var conn = Open();
            using var cmd  = new NpgsqlCommand(
                @"UPDATE ""Clientes"" SET ""VersaoAtual""=@v WHERE ""Id""=@id", conn);
            cmd.Parameters.AddWithValue("@v",  versao ?? "");
            cmd.Parameters.AddWithValue("@id", clienteId);
            cmd.ExecuteNonQuery();
        }

        public void AlterarNivelCliente(long clienteId, int nivel)
        {
            using var conn = Open();
            using var cmd  = new NpgsqlCommand(
                @"UPDATE ""Clientes"" SET ""Nivel""=@n WHERE ""Id""=@id", conn);
            cmd.Parameters.AddWithValue("@n",  nivel);
            cmd.Parameters.AddWithValue("@id", clienteId);
            cmd.ExecuteNonQuery();
        }

        public void AlterarBloqueioCliente(long clienteId, bool bloqueado)
        {
            using var conn = Open();
            using var cmd  = new NpgsqlCommand(
                @"UPDATE ""Clientes"" SET ""Bloqueado""=@b WHERE ""Id""=@id", conn);
            cmd.Parameters.AddWithValue("@b",  bloqueado);
            cmd.Parameters.AddWithValue("@id", clienteId);
            cmd.ExecuteNonQuery();
        }

        // ── Pacotes ───────────────────────────────────────────────────────────────

        public long InserirPacote(Pacote p)
        {
            using var conn = Open();
            using var cmd  = new NpgsqlCommand(@"
                INSERT INTO ""Pacotes"" (""Versao"",""Nivel"",""Descricao"",""CaminhoArquivo"",""TamanhoBytes"",""TemSQL"",""DataPublicacao"",""Ativo"")
                VALUES (@v,@n,@d,@c,@t,@s,@pub,TRUE)
                RETURNING ""Id""", conn);
            cmd.Parameters.AddWithValue("@v",   p.Versao);
            cmd.Parameters.AddWithValue("@n",   p.Nivel);
            cmd.Parameters.AddWithValue("@d",   p.Descricao ?? "");
            cmd.Parameters.AddWithValue("@c",   p.CaminhoArquivo);
            cmd.Parameters.AddWithValue("@t",   p.TamanhoBytes);
            cmd.Parameters.AddWithValue("@s",   p.TemSQL);
            cmd.Parameters.AddWithValue("@pub", DateTime.UtcNow);
            return Convert.ToInt64(cmd.ExecuteScalar());
        }

        public Pacote ObterPacoteParaCliente(long clienteId, string versaoAtual)
        {
            var cliente = ObterCliente(clienteId);
            if (cliente == null || cliente.Bloqueado) return null;

            using var conn = Open();
            using var cmd  = new NpgsqlCommand(@"
                SELECT * FROM ""Pacotes""
                WHERE ""Ativo""=TRUE
                  AND ""Nivel"" <= @nivel
                  AND ""Versao"" > @versao
                ORDER BY ""Versao"" DESC
                LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@nivel",  cliente.Nivel);
            cmd.Parameters.AddWithValue("@versao", versaoAtual ?? "");
            using var r = cmd.ExecuteReader();
            return r.Read() ? MapPacote(r) : null;
        }

        public Pacote ObterPacote(long id)
        {
            using var conn = Open();
            using var cmd  = new NpgsqlCommand(@"SELECT * FROM ""Pacotes"" WHERE ""Id""=@id AND ""Ativo""=TRUE", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? MapPacote(r) : null;
        }

        public List<Pacote> ListarPacotes()
        {
            var list = new List<Pacote>();
            using var conn = Open();
            using var cmd  = new NpgsqlCommand(@"SELECT * FROM ""Pacotes"" ORDER BY ""Id"" DESC", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapPacote(r));
            return list;
        }

        public void DesativarPacote(long id)
        {
            using var conn = Open();
            using var cmd  = new NpgsqlCommand(@"UPDATE ""Pacotes"" SET ""Ativo""=FALSE WHERE ""Id""=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // ── Aplicações ────────────────────────────────────────────────────────────

        public long RegistrarDownload(long clienteId, long pacoteId)
        {
            using var conn = Open();
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO ""AplicacoesUpdate"" (""ClienteId"",""PacoteId"",""DataDownload"",""Status"")
                VALUES (@c,@p,@d,'baixado')
                RETURNING ""Id""", conn);
            cmd.Parameters.AddWithValue("@c", clienteId);
            cmd.Parameters.AddWithValue("@p", pacoteId);
            cmd.Parameters.AddWithValue("@d", DateTime.UtcNow);
            return Convert.ToInt64(cmd.ExecuteScalar());
        }

        public void ConfirmarAplicacao(long clienteId, long pacoteId, string status, string detalhe)
        {
            using var conn = Open();
            // PostgreSQL não suporta ORDER BY em UPDATE diretamente — usa subquery
            using var cmd  = new NpgsqlCommand(@"
                UPDATE ""AplicacoesUpdate""
                SET ""DataAplicada""=@d, ""Status""=@s, ""Detalhe""=@det
                WHERE ""Id"" = (
                    SELECT ""Id"" FROM ""AplicacoesUpdate""
                    WHERE ""ClienteId""=@c AND ""PacoteId""=@p
                    ORDER BY ""Id"" DESC
                    LIMIT 1
                )", conn);
            cmd.Parameters.AddWithValue("@d",   DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@s",   status ?? "aplicado");
            cmd.Parameters.AddWithValue("@det", detalhe ?? "");
            cmd.Parameters.AddWithValue("@c",   clienteId);
            cmd.Parameters.AddWithValue("@p",   pacoteId);
            cmd.ExecuteNonQuery();

            if (status == "aplicado")
            {
                var pacote = ObterPacote(pacoteId);
                if (pacote != null)
                    AtualizarVersaoCliente(clienteId, pacote.Versao);
            }
        }

        // ── Interno ───────────────────────────────────────────────────────────────

        private NpgsqlConnection Open()
        {
            var conn = new NpgsqlConnection(_connStr);
            conn.Open();
            return conn;
        }

        private static void Exec(NpgsqlConnection conn, string sql)
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        private static Cliente MapCliente(NpgsqlDataReader r) => new Cliente
        {
            Id             = r.GetInt64(r.GetOrdinal("Id")),
            CodigoEmpresa  = r["CodigoEmpresa"]?.ToString(),
            NomeEmpresa    = r["NomeEmpresa"]?.ToString(),
            Nivel          = Convert.ToInt32(r["Nivel"]),
            VersaoAtual    = r["VersaoAtual"]?.ToString(),
            Bloqueado      = r.GetBoolean(r.GetOrdinal("Bloqueado")),
            DataRegistro   = r["DataRegistro"]?.ToString(),
            UltimaConsulta = r["UltimaConsulta"] is DBNull ? null : r["UltimaConsulta"]?.ToString()
        };

        private static Pacote MapPacote(NpgsqlDataReader r) => new Pacote
        {
            Id             = r.GetInt64(r.GetOrdinal("Id")),
            Versao         = r["Versao"]?.ToString(),
            Nivel          = Convert.ToInt32(r["Nivel"]),
            Descricao      = r["Descricao"]?.ToString(),
            CaminhoArquivo = r["CaminhoArquivo"]?.ToString(),
            TamanhoBytes   = Convert.ToInt64(r["TamanhoBytes"]),
            TemSQL         = r.GetBoolean(r.GetOrdinal("TemSQL")),
            DataPublicacao = r["DataPublicacao"]?.ToString(),
            Ativo          = r.GetBoolean(r.GetOrdinal("Ativo"))
        };
    }
}

