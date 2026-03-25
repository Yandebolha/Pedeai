using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MySqlConnector;

namespace Pedeai.DB
{
    /// <summary>
    /// Executa todas as migrações de banco de dados ao iniciar o sistema.
    /// Cria tabelas, colunas e dados iniciais que ainda não existam.
    /// Seguro para rodar múltiplas vezes (idempotente).
    /// </summary>
    public static class DbMigrator
    {
        private static string ConnStr =>
            ConfigurationManager.AppSettings["ConnectionString"]
            ?? "Server=localhost;Database=pedeai;User=root;Password=;Port=3306;CharSet=utf8mb4;SslMode=None;";

        public static bool Executar()
        {
            try
            {
                using var conn = new MySqlConnection(ConnStr);
                conn.Open();
                string db = conn.Database;

                // ── 1. Tabela: usuario ────────────────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS usuario (
                        auxCodigo        INT           NOT NULL DEFAULT 1,
                        Codigo           INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        usuNome          VARCHAR(100)  NOT NULL,
                        usuLogin         VARCHAR(50)   NOT NULL UNIQUE,
                        usuSenha         VARCHAR(64)   NOT NULL,
                        usuNivel         TINYINT       NOT NULL DEFAULT 1
                            COMMENT '1=Operador 2=Gerente 9=Admin',
                        Situacao         CHAR(1)       NOT NULL DEFAULT 'A',
                        usuData_Cadastro DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        Info             VARCHAR(255)  NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                // ── 2. Tabela: empresa ────────────────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS empresa (
                        Codigo           INT          NOT NULL DEFAULT 1 PRIMARY KEY,
                        empNome          VARCHAR(100) NOT NULL DEFAULT '',
                        empNome_Fantasia  VARCHAR(100) NOT NULL DEFAULT '',
                        empCNPJ          VARCHAR(18)  NOT NULL DEFAULT '',
                        empTelefone      VARCHAR(20)  NOT NULL DEFAULT '',
                        empEmail         VARCHAR(100) NOT NULL DEFAULT '',
                        empEndereco      VARCHAR(255) NOT NULL DEFAULT '',
                        Info             VARCHAR(255) NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                // ── 3. Tabela: gasto_material ─────────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS gasto_material (
                        auxCodigo        INT            NOT NULL DEFAULT 1,
                        Codigo           INT            NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        gmatData         DATE           NOT NULL,
                        gmatDescricao    VARCHAR(255)   NOT NULL,
                        gmatValor        DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
                        gmatObservacoes  VARCHAR(500)   NOT NULL DEFAULT '',
                        Situacao         CHAR(1)        NOT NULL DEFAULT 'A',
                        Info             VARCHAR(255)   NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                // ── 4. Coluna pediCancelado_Por em pedido_web ─────────────────
                AddColumnIfNotExists(conn, db, "pedido_web", "pediCancelado_Por",
                    "VARCHAR(100) NULL DEFAULT NULL AFTER pediCodigo_Transacao");

                // ── 5. Dado inicial: empresa ──────────────────────────────────
                Exec(conn, @"
                    INSERT INTO empresa (Codigo, empNome)
                    SELECT 1, 'Minha Empresa'
                    FROM DUAL
                    WHERE NOT EXISTS (SELECT 1 FROM empresa WHERE Codigo=1)");

                // ── 6. Dado inicial: admin (hash SHA-256 de '$up0rte') ────────
                //     Insere se não existir; se existir com senha em texto puro, corrige.
                string hashAdmin = HashSenha("$up0rte");
                Exec(conn, $@"
                    INSERT INTO usuario
                        (auxCodigo, Codigo, usuNome, usuLogin, usuSenha, usuNivel, Situacao)
                    SELECT 1, 1, 'Administrador', 'admin', '{hashAdmin}', 9, 'A'
                    FROM DUAL
                    WHERE NOT EXISTS (SELECT 1 FROM usuario WHERE usuLogin='admin')");

                // Corrige senha do admin caso esteja em texto puro ou hash antigo
                Exec(conn, $@"
                    UPDATE usuario
                    SET usuSenha = '{hashAdmin}'
                    WHERE usuLogin = 'admin'
                      AND usuSenha <> '{hashAdmin}'");

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao inicializar banco de dados:\n\n" + ex.Message +
                    "\n\nVerifique a connection string no App.config.",
                    "Erro de Conexao",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private static void Exec(MySqlConnection conn, string sql)
        {
            using var cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        private static void AddColumnIfNotExists(
            MySqlConnection conn, string db, string table, string column, string definition)
        {
            using var check = new MySqlCommand(@"
                SELECT COUNT(*) FROM information_schema.COLUMNS
                WHERE TABLE_SCHEMA=@db AND TABLE_NAME=@tbl AND COLUMN_NAME=@col", conn);
            check.Parameters.AddWithValue("@db",  db);
            check.Parameters.AddWithValue("@tbl", table);
            check.Parameters.AddWithValue("@col", column);
            var exists = Convert.ToInt32(check.ExecuteScalar()) > 0;
            if (!exists)
                Exec(conn, $"ALTER TABLE `{table}` ADD COLUMN `{column}` {definition}");
        }

        private static string HashSenha(string senha)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha ?? ""));
            var sb = new StringBuilder(64);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
