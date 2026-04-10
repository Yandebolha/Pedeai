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

                AddColumnIfNotExists(conn, db, "pedido_web", "pediCodigo_Cupom",
                    "VARCHAR(50) NOT NULL DEFAULT ''");

                // Colunas de pagamento fracionado (multiplas formas no mesmo pedido)
                AddColumnIfNotExists(conn, db, "pedido_web", "pediPago_Dinheiro",
                    "DECIMAL(10,2) NOT NULL DEFAULT 0");
                AddColumnIfNotExists(conn, db, "pedido_web", "pediPago_Cartao",
                    "DECIMAL(10,2) NOT NULL DEFAULT 0");
                AddColumnIfNotExists(conn, db, "pedido_web", "pediPago_Pix",
                    "DECIMAL(10,2) NOT NULL DEFAULT 0");
                AddColumnIfNotExists(conn, db, "pedido_web", "pediAutorizador",
                    "VARCHAR(150) NULL DEFAULT NULL");

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

                AddColumnIfNotExists(conn, db, "config_impressao", "lblDesconto",
                    "VARCHAR(50) NOT NULL DEFAULT '- DESCONTO:'");
                AddColumnIfNotExists(conn, db, "config_impressao", "lblCupom",
                    "VARCHAR(50) NOT NULL DEFAULT '- CUPOM:'");

                // ── 8. Tabela: entrada_mercadoria ─────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS entrada_mercadoria (
                        auxCodigo           INT           NOT NULL DEFAULT 1,
                        Codigo              INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Codigo_Fornecedor   INT           NULL,
                        entNome_Fornecedor  VARCHAR(150)  NOT NULL DEFAULT '',
                        entData             DATE          NOT NULL,
                        entNumeroDoc        VARCHAR(50)   NOT NULL DEFAULT '',
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

                // ── 10. Tabela: cupom ─────────────────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS cupom (
                        auxCodigo              INT            NOT NULL DEFAULT 1,
                        Codigo                 INT            NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        cupomCodigo            VARCHAR(50)    NOT NULL,
                        cupomDescricao         VARCHAR(200)   NOT NULL DEFAULT '',
                        cupomTipo              VARCHAR(20)    NOT NULL DEFAULT 'PERCENTUAL',
                        cupomValor             DECIMAL(10,2)  NOT NULL DEFAULT 0,
                        cupomPedido_Minimo     DECIMAL(10,2)  NOT NULL DEFAULT 0,
                        cupomLimite_Usos       INT            NOT NULL DEFAULT 0,
                        cupomUsos_Realizados   INT            NOT NULL DEFAULT 0,
                        cupomValido_Ate        DATE           NOT NULL,
                        cupomData_Cadastro     DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        Situacao               CHAR(1)        NOT NULL DEFAULT 'A',
                        Status_Transmissao     CHAR(1)        NOT NULL DEFAULT 'N',
                        Info                   VARCHAR(255)   NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                // Garante colunas que podem faltar em instalações antigas
                AddColumnIfNotExists(conn, db, "cupom", "Status_Transmissao",
                    "CHAR(1) NOT NULL DEFAULT 'N'");
                AddColumnIfNotExists(conn, db, "cupom", "Info",
                    "VARCHAR(255) NOT NULL DEFAULT ''");

                // Corrige cupomTipo se estiver como INT em instalações antigas
                EnsureColumnType(conn, db, "cupom", "cupomTipo",
                    "int", "MODIFY COLUMN `cupomTipo` VARCHAR(20) NOT NULL DEFAULT 'PERCENTUAL'");

                // ── 11. Tabela: estoque_item ──────────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS estoque_item (
                        Codigo                      INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        estoNome                    VARCHAR(150)  NOT NULL DEFAULT '',
                        estoUnidade                 VARCHAR(20)   NOT NULL DEFAULT 'un',
                        estoQtde_Atual              DECIMAL(12,4) NOT NULL DEFAULT 0,
                        estoPreco_Custo             DECIMAL(12,4) NOT NULL DEFAULT 0,
                        estoEh_Produto              TINYINT(1)    NOT NULL DEFAULT 0,
                        estoFracao_Entrada          DECIMAL(12,4) NOT NULL DEFAULT 1,
                        estoFracao_Entrada_Unidade  VARCHAR(20)   NOT NULL DEFAULT '',
                        estoFracao_Saida            DECIMAL(12,4) NOT NULL DEFAULT 1,
                        estoFracao_Saida_Unidade    VARCHAR(20)   NOT NULL DEFAULT '',
                        Codigo_Grupo                INT           NULL DEFAULT NULL,
                        Codigo_Mercadoria           INT           NULL DEFAULT NULL,
                        Situacao                    CHAR(1)       NOT NULL DEFAULT 'A',
                        estoData_Cadastro           DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                // Colunas de fração (instalações existentes)
                // Remove coluna descontinuada
                DropColumnIfExists(conn, db, "estoque_item", "estoEstoque_Min");

                AddColumnIfNotExists(conn, db, "estoque_item", "estoFracao_Entrada",
                    "DECIMAL(12,4) NOT NULL DEFAULT 1");
                AddColumnIfNotExists(conn, db, "estoque_item", "estoFracao_Saida",
                    "DECIMAL(12,4) NOT NULL DEFAULT 1");
                AddColumnIfNotExists(conn, db, "estoque_item", "estoFracao_Entrada_Unidade",
                    "VARCHAR(20) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "estoque_item", "estoFracao_Saida_Unidade",
                    "VARCHAR(20) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "estoque_item", "Codigo_Grupo",
                    "INT NULL DEFAULT NULL");

                // Remove coluna descontinuada de entrada_mercadoria
                DropColumnIfExists(conn, db, "entrada_mercadoria", "entObservacoes");

                // Frações de entrada em item_entrada_mercadoria
                AddColumnIfNotExists(conn, db, "item_entrada_mercadoria", "itmFracao",
                    "DECIMAL(12,4) NOT NULL DEFAULT 1.0000");
                AddColumnIfNotExists(conn, db, "item_entrada_mercadoria", "itmUnid_Entrada",
                    "VARCHAR(20) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "item_entrada_mercadoria", "itmUnid_Saida",
                    "VARCHAR(20) NOT NULL DEFAULT ''");

                // ── 12. Tabela: fornecedor ────────────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS fornecedor (
                        auxCodigo                INT           NOT NULL DEFAULT 1,
                        Codigo                   INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        fornNome_RazaoSocial     VARCHAR(150)  NOT NULL DEFAULT '',
                        fornApelido_Fantasia     VARCHAR(100)  NOT NULL DEFAULT '',
                        fornCPF_CNPJ_            VARCHAR(20)   NOT NULL DEFAULT '',
                        fornRG_InscricaoEstadual VARCHAR(30)   NOT NULL DEFAULT '',
                        fornTelefone             VARCHAR(20)   NOT NULL DEFAULT '',
                        fornEmail                VARCHAR(100)  NOT NULL DEFAULT '',
                        fornContato              VARCHAR(100)  NOT NULL DEFAULT '',
                        fornCEP                  VARCHAR(10)   NOT NULL DEFAULT '',
                        fornEndereco             VARCHAR(200)  NOT NULL DEFAULT '',
                        fornNumero               VARCHAR(10)   NOT NULL DEFAULT '',
                        fornBairro               VARCHAR(100)  NOT NULL DEFAULT '',
                        fornCidade               VARCHAR(100)  NOT NULL DEFAULT '',
                        fornEstado               VARCHAR(5)    NOT NULL DEFAULT '',
                        fornObservacoes          VARCHAR(500)  NOT NULL DEFAULT '',
                        fornData_Cadastro        DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        Situacao                 CHAR(1)       NOT NULL DEFAULT 'A',
                        Status_Transmissao       CHAR(1)       NOT NULL DEFAULT 'N',
                        Info                     VARCHAR(255)  NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                // Colunas adicionadas ao fornecedor (instalações existentes sem essas colunas)
                AddColumnIfNotExists(conn, db, "fornecedor", "fornContato",
                    "VARCHAR(100) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "fornecedor", "fornRG_InscricaoEstadual",
                    "VARCHAR(30) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "fornecedor", "fornCEP",
                    "VARCHAR(10) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "fornecedor", "fornEndereco",
                    "VARCHAR(200) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "fornecedor", "fornNumero",
                    "VARCHAR(10) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "fornecedor", "fornBairro",
                    "VARCHAR(100) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "fornecedor", "fornCidade",
                    "VARCHAR(100) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "fornecedor", "fornEstado",
                    "VARCHAR(5) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "fornecedor", "fornObservacoes",
                    "VARCHAR(500) NOT NULL DEFAULT ''");

                // ── config_fidelizacao ────────────────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS config_fidelizacao (
                        Codigo            INT           NOT NULL DEFAULT 1 PRIMARY KEY,
                        fidAtivo          TINYINT(1)    NOT NULL DEFAULT 0,
                        fidMeta_Gasto     DECIMAL(10,2) NOT NULL DEFAULT 500.00,
                        fidPremio_Tipo    VARCHAR(20)   NOT NULL DEFAULT 'CUPOM',
                        fidCupom_Tipo     VARCHAR(20)   NOT NULL DEFAULT 'PERCENTUAL',
                        fidCupom_Valor    DECIMAL(10,2) NOT NULL DEFAULT 10.00,
                        fidCupom_Minimo   DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        fidCupom_Validade INT           NOT NULL DEFAULT 30,
                        fidProduto_Codigo INT           NULL DEFAULT NULL,
                        fidProduto_Nome   VARCHAR(150)  NOT NULL DEFAULT '',
                        fidMensagem       TEXT          NOT NULL,
                        Info              VARCHAR(255)  NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                Exec(conn, @"INSERT IGNORE INTO config_fidelizacao
                    (Codigo, fidMensagem)
                    VALUES (1, 'Parabéns {Nome}! Você atingiu R$ {Meta} em compras e ganhou um cupom {CupomCodigo} válido até {Validade}.')");

                // Promote Codigo to AUTO_INCREMENT if not yet
                {
                    using var chk = new MySqlCommand(@"
                        SELECT COLUMN_NAME FROM information_schema.COLUMNS
                        WHERE TABLE_SCHEMA=@db AND TABLE_NAME='config_fidelizacao'
                          AND COLUMN_NAME='Codigo' AND EXTRA LIKE '%auto_increment%' LIMIT 1", conn);
                    chk.Parameters.AddWithValue("@db", db);
                    if (chk.ExecuteScalar() == null)
                        Exec(conn, "ALTER TABLE config_fidelizacao MODIFY Codigo INT NOT NULL AUTO_INCREMENT");
                }
                AddColumnIfNotExists(conn, db, "config_fidelizacao", "fidNome",
                    "VARCHAR(100) NOT NULL DEFAULT 'Regra Padrão'");                AddColumnIfNotExists(conn, db, "config_fidelizacao", "fidProduto_Qtde",
                    "INT NOT NULL DEFAULT 1");
                AddColumnIfNotExists(conn, db, "config_fidelizacao", "fidMeta_Tipo",
                    "VARCHAR(10) NOT NULL DEFAULT 'VALOR'");
                // ── historico_fidelizacao ─────────────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS historico_fidelizacao (
                        Codigo         INT          NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Codigo_Cliente INT          NOT NULL,
                        Codigo_Pedido  INT          NOT NULL,
                        fidData        DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        fidCupomCodigo VARCHAR(50)  NOT NULL DEFAULT '',
                        fidDescricao   VARCHAR(200) NOT NULL DEFAULT '',
                        fidTelefone    VARCHAR(20)  NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                AddColumnIfNotExists(conn, db, "historico_fidelizacao", "Codigo_Config",
                    "INT NULL DEFAULT NULL");

                // ── colunas gasto mensal cliente ──────────────────────────────
                AddColumnIfNotExists(conn, db, "cliente", "clieGasto_Mensal",
                    "DECIMAL(10,2) NOT NULL DEFAULT 0.00");
                AddColumnIfNotExists(conn, db, "cliente", "clieGasto_Mes_Ref",
                    "VARCHAR(7) NOT NULL DEFAULT ''");                AddColumnIfNotExists(conn, db, "itens_pedido_web", "itpwDesconto_Pct",
                    "DECIMAL(5,2) NOT NULL DEFAULT 0.00");

                // ── Promoções e Cardápio do Dia ────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS promocao (
                        Codigo            INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        prom_Nome         VARCHAR(100)  NOT NULL DEFAULT '',
                        prom_DataInicio   DATE          NOT NULL,
                        prom_DataFim      DATE          NOT NULL,
                        prom_Desconto_Tipo  VARCHAR(20) NOT NULL DEFAULT 'PERCENTUAL',
                        prom_Desconto_Valor DECIMAL(10,2) NOT NULL DEFAULT 10.00,
                        prom_Ativo        TINYINT(1)    NOT NULL DEFAULT 1
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS promocao_item (
                        Codigo              INT          NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Codigo_Promocao     INT          NOT NULL,
                        Codigo_Mercadoria   INT          NOT NULL DEFAULT 0,
                        prom_Produto_Nome   VARCHAR(150) NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS cardapio_dia (
                        Codigo        INT          NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        card_Data     DATE         NOT NULL,
                        card_Titulo   VARCHAR(200) NOT NULL DEFAULT '',
                        card_Observacao TEXT        NOT NULL
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS cardapio_dia_item (
                        Codigo                INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Codigo_Cardapio       INT           NOT NULL,
                        Codigo_Mercadoria     INT           NOT NULL DEFAULT 0,
                        card_Produto_Nome     VARCHAR(150)  NOT NULL DEFAULT '',
                        card_Produto_Preco    DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        card_Produto_Descricao VARCHAR(300) NOT NULL DEFAULT ''
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

        private static void DropColumnIfExists(
            MySqlConnection conn, string db, string table, string column)
        {
            using var check = new MySqlCommand(@"
                SELECT COUNT(*) FROM information_schema.COLUMNS
                WHERE TABLE_SCHEMA=@db AND TABLE_NAME=@tbl AND COLUMN_NAME=@col", conn);
            check.Parameters.AddWithValue("@db",  db);
            check.Parameters.AddWithValue("@tbl", table);
            check.Parameters.AddWithValue("@col", column);
            var exists = Convert.ToInt32(check.ExecuteScalar()) > 0;
            if (exists)
                Exec(conn, $"ALTER TABLE `{table}` DROP COLUMN `{column}`");
        }

        /// <summary>
        /// Executa um ALTER TABLE se o tipo atual da coluna começar com <paramref name="typeContains"/>.
        /// </summary>
        private static void EnsureColumnType(
            MySqlConnection conn, string db, string table, string column,
            string typeContains, string alterClause)
        {
            using var check = new MySqlCommand(@"
                SELECT DATA_TYPE FROM information_schema.COLUMNS
                WHERE TABLE_SCHEMA=@db AND TABLE_NAME=@tbl AND COLUMN_NAME=@col
                LIMIT 1", conn);
            check.Parameters.AddWithValue("@db",  db);
            check.Parameters.AddWithValue("@tbl", table);
            check.Parameters.AddWithValue("@col", column);
            var dataType = check.ExecuteScalar()?.ToString() ?? "";
            if (dataType.IndexOf(typeContains, StringComparison.OrdinalIgnoreCase) >= 0)
                Exec(conn, $"ALTER TABLE `{table}` {alterClause}");
        }

        private static string HashSenha(string senha)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha ?? ""));
            var sb = new StringBuilder(64);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private static void TruncateIfExists(MySqlConnection conn, string table)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @t";
            cmd.Parameters.AddWithValue("@t", table);
            if (Convert.ToInt64(cmd.ExecuteScalar()) > 0)
                Exec(conn, $"TRUNCATE TABLE `{table}`");
        }

        /// <summary>
        /// Apaga TODOS os dados do banco, reinsere apenas o usuário Admin (Codigo=1)
        /// e o registro mínimo de empresa/config_impressao.
        /// Retorna string vazia em caso de sucesso ou a mensagem de erro.
        /// </summary>
        public static string ResetarBanco()
        {
            try
            {
                using var conn = new MySqlConnection(ConnStr);
                conn.Open();

                Exec(conn, "SET FOREIGN_KEY_CHECKS = 0");

                string[] truncar = {
                    "usuario",
                    "itens_pedido_web", "pedido_web",
                    "item_entrada_mercadoria", "parcela_entrada_mercadoria",
                    "entrada_mercadoria", "gasto_material", "necessidade_empresa",
                    "turno", "estoque_item", "cliente", "cupom", "fornecedor",
                    "mercadoria", "grupo_mercadoria", "empresa", "config_impressao"
                };
                foreach (var t in truncar)
                    TruncateIfExists(conn, t);

                string[] dropar = {
                    "produto", "categoria", "pedido", "item_pedido",
                    "pagamento", "notificacao", "configuracao", "taxa_entrega",
                    "banner", "avaliacao", "token_dispositivo", "endereco_cliente",
                    "horario_funcionamento"
                };
                foreach (var t in dropar)
                    Exec(conn, $"DROP TABLE IF EXISTS `{t}`");

                Exec(conn, "SET FOREIGN_KEY_CHECKS = 1");

                // Re-seed: admin com Codigo=1 e senha $up0rte
                string hashAdmin = HashSenha("$up0rte");
                Exec(conn, $@"INSERT INTO usuario
                    (Codigo, auxCodigo, usuNome, usuLogin, usuSenha, usuNivel, Situacao)
                    VALUES (1, 1, 'Administrador', 'admin', '{hashAdmin}', 9, 'A')");

                Exec(conn, @"INSERT IGNORE INTO empresa
                    (Codigo, empNome, empNome_Fantasia, empCNPJ, empTelefone, empEmail, empEndereco, Info)
                    VALUES (1, 'Minha Empresa', '', '', '', '', '', '')");

                Exec(conn, "INSERT IGNORE INTO config_impressao (Codigo) VALUES (1)");

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
