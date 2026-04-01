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

                // ── 3b. Colunas em usuario (retrocompatibilidade) ─────────────────
                AddColumnIfNotExists(conn, db, "usuario", "Info",
                    "VARCHAR(255) NOT NULL DEFAULT ''");

                // ── 4. Colunas em pedido_web ─────────────────────────────────────────
                AddColumnIfNotExists(conn, db, "pedido_web", "pediCancelado_Por",
                    "VARCHAR(100) NULL DEFAULT NULL");
                AddColumnIfNotExists(conn, db, "pedido_web", "pediValor_Pago",
                    "DECIMAL(10,2) NULL DEFAULT NULL");
                AddColumnIfNotExists(conn, db, "pedido_web", "pediCodigo_Transacao",
                    "VARCHAR(100) NULL DEFAULT NULL");
                AddColumnIfNotExists(conn, db, "pedido_web", "pediData_Atualizacao",
                    "DATETIME NULL DEFAULT NULL");
                AddColumnIfNotExists(conn, db, "pedido_web", "Codigo_Cliente",
                    "INT NULL DEFAULT NULL");

                // Colunas de pagamento fracionado (multiplas formas no mesmo pedido)
                AddColumnIfNotExists(conn, db, "pedido_web", "pediPago_Dinheiro",
                    "DECIMAL(10,2) NOT NULL DEFAULT 0");
                AddColumnIfNotExists(conn, db, "pedido_web", "pediPago_Cartao",
                    "DECIMAL(10,2) NOT NULL DEFAULT 0");
                AddColumnIfNotExists(conn, db, "pedido_web", "pediPago_Pix",
                    "DECIMAL(10,2) NOT NULL DEFAULT 0");

                // ── 4b. Colunas em grupo_mercadoria ─────────────────────────────
                AddColumnIfNotExists(conn, db, "grupo_mercadoria", "grmeData_Cadastro",
                    "DATETIME NULL DEFAULT NULL");

                // ── 4c. Colunas em mercadoria ────────────────────────────────
                AddColumnIfNotExists(conn, db, "mercadoria", "mercHabilitar_Site",
                    "TINYINT(1) NOT NULL DEFAULT 0");

                // ── 5. Dado inicial: empresa ──────────────────────────────────
                //     INSERT IGNORE: silencioso se já existir Codigo=1
                Exec(conn, @"
                    INSERT IGNORE INTO empresa
                        (Codigo, empNome, empNome_Fantasia, empCNPJ,
                         empTelefone, empEmail, empEndereco, Info)
                    VALUES (1, 'Minha Empresa', '', '', '', '', '', '')");

                // ── 6. Dado inicial: admin (hash SHA-256 de '$up0rte') ────────
                //     ON DUPLICATE KEY: atualiza hash/nivel se o admin já existir
                string hashAdmin = HashSenha("$up0rte");
                Exec(conn, $@"
                    INSERT INTO usuario
                        (auxCodigo, usuNome, usuLogin, usuSenha, usuNivel, Situacao)
                    VALUES (1, 'Administrador', 'admin', '{hashAdmin}', 9, 'A')
                    ON DUPLICATE KEY UPDATE
                        usuSenha = '{hashAdmin}',
                        usuNivel = 9,
                        Situacao = 'A'");

                // ── 7. Tabela: config_impressao ───────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS config_impressao (
                        Codigo              INT           NOT NULL DEFAULT 1 PRIMARY KEY,
                        cabNomeEmpresa      VARCHAR(100)  NOT NULL DEFAULT '',
                        cabEndereco         VARCHAR(200)  NOT NULL DEFAULT '',
                        cabTelefone         VARCHAR(50)   NOT NULL DEFAULT '',
                        cabCNPJ             VARCHAR(30)   NOT NULL DEFAULT '',
                        separador           VARCHAR(5)    NOT NULL DEFAULT '-',
                        rodapeAvisoFiscal   VARCHAR(100)  NOT NULL DEFAULT '*** NAO E DOCUMENTO FISCAL ***',
                        rodapeTextoLivre    TEXT          NOT NULL,
                        lblNumeroPedido     VARCHAR(50)   NOT NULL DEFAULT 'Pedido N.:',
                        lblColunaItem       VARCHAR(50)   NOT NULL DEFAULT 'ITEM (V.Unit)',
                        lblColunaTotal      VARCHAR(30)   NOT NULL DEFAULT 'Total',
                        lblSubtotal         VARCHAR(50)   NOT NULL DEFAULT 'TOTAL:',
                        lblTaxaEntrega      VARCHAR(50)   NOT NULL DEFAULT '+ ENTREGA:',
                        lblTotalPagar       VARCHAR(50)   NOT NULL DEFAULT '= TOTAL A PAGAR:',
                        lblAtendente        VARCHAR(50)   NOT NULL DEFAULT 'Atendente:',
                        larguraCaracteres   INT           NOT NULL DEFAULT 42,
                        impressoraNome      VARCHAR(200)  NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                Exec(conn, @"
                    INSERT IGNORE INTO config_impressao (Codigo)
                    VALUES (1)");

                // ── 8. Tabela: entrada_mercadoria ─────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS entrada_mercadoria (
                        auxCodigo           INT           NOT NULL DEFAULT 1,
                        Codigo              INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Codigo_Fornecedor   INT           NULL,
                        entNome_Fornecedor  VARCHAR(150)  NOT NULL DEFAULT '',
                        entData             DATE          NOT NULL,
                        entNumeroDoc        VARCHAR(50)   NOT NULL DEFAULT '',
                        entObservacoes      VARCHAR(500)  NOT NULL DEFAULT '',
                        entValorTotal       DECIMAL(12,2) NOT NULL DEFAULT 0.00,
                        Situacao            CHAR(1)       NOT NULL DEFAULT 'A',
                        Info                VARCHAR(255)  NOT NULL DEFAULT '',
                        entData_Lancamento  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS item_entrada_mercadoria (
                        auxCodigo           INT           NOT NULL DEFAULT 1,
                        Codigo              INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Codigo_Entrada      INT           NOT NULL,
                        Codigo_Mercadoria   INT           NOT NULL DEFAULT 0,
                        itmNome_Mercadoria  VARCHAR(150)  NOT NULL DEFAULT '',
                        itmQtde             DECIMAL(12,4) NOT NULL DEFAULT 0,
                        itmPreco_Custo      DECIMAL(12,4) NOT NULL DEFAULT 0,
                        itmSubtotal         DECIMAL(12,2) NOT NULL DEFAULT 0,
                        itmAtualizar_Custo  TINYINT(1)    NOT NULL DEFAULT 1,
                        Situacao            CHAR(1)       NOT NULL DEFAULT 'A'
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS parcela_entrada_mercadoria (
                        auxCodigo       INT           NOT NULL DEFAULT 1,
                        Codigo          INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Codigo_Entrada  INT           NOT NULL,
                        parNumero       TINYINT       NOT NULL DEFAULT 1,
                        parVencimento   DATE          NOT NULL,
                        parValor        DECIMAL(12,2) NOT NULL DEFAULT 0,
                        parObservacao   VARCHAR(250)  NOT NULL DEFAULT '',
                        Situacao        CHAR(1)       NOT NULL DEFAULT 'A',
                        parData_Pagamento DATE        NULL
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                // ── 9. Tabela: turno (abertura / fechamento de caixa) ──────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS turno (
                        auxCodigo        INT           NOT NULL DEFAULT 1,
                        Codigo           INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        turAbertura      DATETIME      NOT NULL,
                        turFechamento    DATETIME      NULL,
                        turUsuario       VARCHAR(100)  NOT NULL DEFAULT '',
                        turCaixa_Inicial DECIMAL(10,2) NOT NULL DEFAULT 0,
                        turCaixa_Final   DECIMAL(10,2) NULL,
                        turObservacao    VARCHAR(500)  NOT NULL DEFAULT '',
                        turSituacao      CHAR(1)       NOT NULL DEFAULT 'A'
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

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
