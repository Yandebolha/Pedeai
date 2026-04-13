using Microsoft.Data.Sqlite;
using PedeaiUpdateServer.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PedeaiUpdateServer.Data
{
    /// <summary>
    /// Repositório SQLite para o servidor de atualizações.
    /// Banco de dados criado automaticamente em Data/update.db.
    /// </summary>
    public class UpdateDb
    {
        private readonly string _connStr;

        public UpdateDb(string dbPath)
        {
            _connStr = $"Data Source={dbPath}";
            EnsureCreated(dbPath);
        }

        // ── Inicialização ─────────────────────────────────────────────────────────

        private void EnsureCreated(string dbPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? ".");
            using var conn = Open();
            Exec(conn, @"
                CREATE TABLE IF NOT EXISTS Clientes (
                    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
                    CodigoEmpresa TEXT    NOT NULL,
                    NomeEmpresa   TEXT    NOT NULL DEFAULT '',
                    Nivel         INTEGER NOT NULL DEFAULT 2,
                    VersaoAtual   TEXT    NOT NULL DEFAULT '',
                    Bloqueado     INTEGER NOT NULL DEFAULT 0,
                    DataRegistro  TEXT    NOT NULL,
                    UltimaConsulta TEXT
                )");

            Exec(conn, @"
                CREATE TABLE IF NOT EXISTS Pacotes (
                    Id             INTEGER PRIMARY KEY AUTOINCREMENT,
                    Versao         TEXT    NOT NULL,
                    Nivel          INTEGER NOT NULL DEFAULT 2,
                    Descricao      TEXT    NOT NULL DEFAULT '',
                    CaminhoArquivo TEXT    NOT NULL,
                    TamanhoBytes   INTEGER NOT NULL DEFAULT 0,
                    TemSQL         INTEGER NOT NULL DEFAULT 0,
                    DataPublicacao TEXT    NOT NULL,
                    Ativo          INTEGER NOT NULL DEFAULT 1
                )");

            Exec(conn, @"
                CREATE TABLE IF NOT EXISTS AplicacoesUpdate (
                    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClienteId    INTEGER NOT NULL,
                    PacoteId     INTEGER NOT NULL,
                    DataDownload TEXT,
                    DataAplicada TEXT,
                    Status       TEXT    NOT NULL DEFAULT 'baixado',
                    Detalhe      TEXT
                )");
        }

        // ── Clientes ──────────────────────────────────────────────────────────────

        public long RegistrarCliente(string codigoEmpresa, string nomeEmpresa)
        {
            using var conn = Open();
            // Retorna existente se já registrado
            using var check = new SqliteCommand(
                "SELECT Id FROM Clientes WHERE CodigoEmpresa=@cod LIMIT 1", conn);
            check.Parameters.AddWithValue("@cod", codigoEmpresa.Trim().ToUpperInvariant());
            long? existente = check.ExecuteScalar() as long?;
            if (existente.HasValue) return existente.Value;

            using var cmd = new SqliteCommand(@"
                INSERT INTO Clientes (CodigoEmpresa, NomeEmpresa, Nivel, Bloqueado, DataRegistro)
                VALUES (@cod, @nome, 2, 0, @data);
                SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@cod",  codigoEmpresa.Trim().ToUpperInvariant());
            cmd.Parameters.AddWithValue("@nome", nomeEmpresa ?? "");
            cmd.Parameters.AddWithValue("@data", DateTime.UtcNow.ToString("o"));
            return (long)cmd.ExecuteScalar();
        }

        public Cliente ObterCliente(long id)
        {
            using var conn = Open();
            using var cmd  = new SqliteCommand("SELECT * FROM Clientes WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? MapCliente(r) : null;
        }

        public List<Cliente> ListarClientes()
        {
            var list = new List<Cliente>();
            using var conn = Open();
            using var cmd  = new SqliteCommand("SELECT * FROM Clientes ORDER BY Id", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapCliente(r));
            return list;
        }

        public void AtualizarUltimaConsulta(long clienteId)
        {
            using var conn = Open();
            using var cmd  = new SqliteCommand(
                "UPDATE Clientes SET UltimaConsulta=@d WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@d",  DateTime.UtcNow.ToString("o"));
            cmd.Parameters.AddWithValue("@id", clienteId);
            cmd.ExecuteNonQuery();
        }

        public void AtualizarVersaoCliente(long clienteId, string versao)
        {
            using var conn = Open();
            using var cmd  = new SqliteCommand(
                "UPDATE Clientes SET VersaoAtual=@v WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@v",  versao ?? "");
            cmd.Parameters.AddWithValue("@id", clienteId);
            cmd.ExecuteNonQuery();
        }

        public void AlterarNivelCliente(long clienteId, int nivel)
        {
            using var conn = Open();
            using var cmd  = new SqliteCommand(
                "UPDATE Clientes SET Nivel=@n WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@n",  nivel);
            cmd.Parameters.AddWithValue("@id", clienteId);
            cmd.ExecuteNonQuery();
        }

        public void AlterarBloqueioCliente(long clienteId, bool bloqueado)
        {
            using var conn = Open();
            using var cmd  = new SqliteCommand(
                "UPDATE Clientes SET Bloqueado=@b WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@b",  bloqueado ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", clienteId);
            cmd.ExecuteNonQuery();
        }

        // ── Pacotes ───────────────────────────────────────────────────────────────

        public long InserirPacote(Pacote p)
        {
            using var conn = Open();
            using var cmd  = new SqliteCommand(@"
                INSERT INTO Pacotes (Versao,Nivel,Descricao,CaminhoArquivo,TamanhoBytes,TemSQL,DataPublicacao,Ativo)
                VALUES (@v,@n,@d,@c,@t,@s,@pub,1);
                SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@v",   p.Versao);
            cmd.Parameters.AddWithValue("@n",   p.Nivel);
            cmd.Parameters.AddWithValue("@d",   p.Descricao ?? "");
            cmd.Parameters.AddWithValue("@c",   p.CaminhoArquivo);
            cmd.Parameters.AddWithValue("@t",   p.TamanhoBytes);
            cmd.Parameters.AddWithValue("@s",   p.TemSQL ? 1 : 0);
            cmd.Parameters.AddWithValue("@pub", DateTime.UtcNow.ToString("o"));
            return (long)cmd.ExecuteScalar();
        }

        public Pacote ObterPacoteParaCliente(long clienteId, string versaoAtual)
        {
            var cliente = ObterCliente(clienteId);
            if (cliente == null || cliente.Bloqueado) return null;

            using var conn = Open();
            // Busca o pacote mais recente compatível com o nível do cliente
            using var cmd  = new SqliteCommand(@"
                SELECT * FROM Pacotes
                WHERE Ativo=1
                  AND Nivel <= @nivel
                  AND Versao > @versao
                ORDER BY Versao DESC
                LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@nivel",  cliente.Nivel);
            cmd.Parameters.AddWithValue("@versao", versaoAtual ?? "");
            using var r = cmd.ExecuteReader();
            return r.Read() ? MapPacote(r) : null;
        }

        public Pacote ObterPacote(long id)
        {
            using var conn = Open();
            using var cmd  = new SqliteCommand("SELECT * FROM Pacotes WHERE Id=@id AND Ativo=1", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? MapPacote(r) : null;
        }

        public List<Pacote> ListarPacotes()
        {
            var list = new List<Pacote>();
            using var conn = Open();
            using var cmd  = new SqliteCommand("SELECT * FROM Pacotes ORDER BY Id DESC", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(MapPacote(r));
            return list;
        }

        public void DesativarPacote(long id)
        {
            using var conn = Open();
            using var cmd  = new SqliteCommand("UPDATE Pacotes SET Ativo=0 WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // ── Aplicações ────────────────────────────────────────────────────────────

        public long RegistrarDownload(long clienteId, long pacoteId)
        {
            using var conn = Open();
            using var cmd = new SqliteCommand(@"
                INSERT INTO AplicacoesUpdate (ClienteId,PacoteId,DataDownload,Status)
                VALUES (@c,@p,@d,'baixado');
                SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@c", clienteId);
            cmd.Parameters.AddWithValue("@p", pacoteId);
            cmd.Parameters.AddWithValue("@d", DateTime.UtcNow.ToString("o"));
            return (long)cmd.ExecuteScalar();
        }

        public void ConfirmarAplicacao(long clienteId, long pacoteId, string status, string detalhe)
        {
            using var conn = Open();
            using var cmd  = new SqliteCommand(@"
                UPDATE AplicacoesUpdate
                SET DataAplicada=@d, Status=@s, Detalhe=@det
                WHERE ClienteId=@c AND PacoteId=@p
                ORDER BY Id DESC
                LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@d",   DateTime.UtcNow.ToString("o"));
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

        private SqliteConnection Open()
        {
            var conn = new SqliteConnection(_connStr);
            conn.Open();
            return conn;
        }

        private static void Exec(SqliteConnection conn, string sql)
        {
            using var cmd = new SqliteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        private static Cliente MapCliente(SqliteDataReader r) => new Cliente
        {
            Id             = r.GetInt64(r.GetOrdinal("Id")),
            CodigoEmpresa  = r["CodigoEmpresa"]?.ToString(),
            NomeEmpresa    = r["NomeEmpresa"]?.ToString(),
            Nivel          = Convert.ToInt32(r["Nivel"]),
            VersaoAtual    = r["VersaoAtual"]?.ToString(),
            Bloqueado      = Convert.ToInt32(r["Bloqueado"]) == 1,
            DataRegistro   = r["DataRegistro"]?.ToString(),
            UltimaConsulta = r["UltimaConsulta"]?.ToString()
        };

        private static Pacote MapPacote(SqliteDataReader r) => new Pacote
        {
            Id             = r.GetInt64(r.GetOrdinal("Id")),
            Versao         = r["Versao"]?.ToString(),
            Nivel          = Convert.ToInt32(r["Nivel"]),
            Descricao      = r["Descricao"]?.ToString(),
            CaminhoArquivo = r["CaminhoArquivo"]?.ToString(),
            TamanhoBytes   = Convert.ToInt64(r["TamanhoBytes"]),
            TemSQL         = Convert.ToInt32(r["TemSQL"]) == 1,
            DataPublicacao = r["DataPublicacao"]?.ToString(),
            Ativo          = Convert.ToInt32(r["Ativo"]) == 1
        };
    }
}
