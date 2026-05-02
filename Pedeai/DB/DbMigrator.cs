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
                // Conecta sem banco para garantir que ele exista
                var builder = new MySqlConnectionStringBuilder(ConnStr);
                string dbName = builder.Database;
                builder.Database = "";
                using (var connInit = new MySqlConnection(builder.ToString()))
                {
                    connInit.Open();
                    using var cmdCreate = new MySqlCommand(
                        $"CREATE DATABASE IF NOT EXISTS `{dbName}` DEFAULT CHARSET utf8mb4 COLLATE utf8mb4_unicode_ci",
                        connInit);
                    cmdCreate.ExecuteNonQuery();
                }

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

                // ── 4. Tabelas pré-existentes (criadas aqui se banco for novo) ─────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS cliente (
                        auxCodigo            INT           NOT NULL DEFAULT 1,
                        Codigo               INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        clieNome_RazaoSocial VARCHAR(150)  NOT NULL DEFAULT '',
                        clieTelefone         VARCHAR(20)   NOT NULL DEFAULT '',
                        clieCelular          VARCHAR(20)   NOT NULL DEFAULT '',
                        clieEmail            VARCHAR(100)  NOT NULL DEFAULT '',
                        clieCPF_CNPJ_        VARCHAR(18)   NOT NULL DEFAULT '',
                        clieCEP              VARCHAR(10)   NOT NULL DEFAULT '',
                        clieEndereco         VARCHAR(200)  NOT NULL DEFAULT '',
                        clieNumero           VARCHAR(20)   NOT NULL DEFAULT '',
                        clieComplemento      VARCHAR(100)  NOT NULL DEFAULT '',
                        clieBairro           VARCHAR(100)  NOT NULL DEFAULT '',
                        clieCidade           VARCHAR(100)  NOT NULL DEFAULT '',
                        clieEstado           VARCHAR(2)    NOT NULL DEFAULT '',
                        clieTotalPedidos     INT           NOT NULL DEFAULT 0,
                        clieTotalGasto       DECIMAL(12,2) NOT NULL DEFAULT 0.00,
                        clieGasto_Mensal     DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        clieGasto_Mes_Ref    VARCHAR(7)    NOT NULL DEFAULT '',
                        clieData_Cadastro    DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        Situacao             CHAR(1)       NOT NULL DEFAULT 'A',
                        Status_Transmissao   CHAR(1)       NOT NULL DEFAULT 'N',
                        Info                 VARCHAR(255)  NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS grupo_mercadoria (
                        auxCodigo        INT           NOT NULL DEFAULT 1,
                        Codigo           INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        grmeDescricao_   VARCHAR(100)  NOT NULL DEFAULT '',
                        grmeOrdem        INT           NOT NULL DEFAULT 0,
                        Situacao         CHAR(1)       NOT NULL DEFAULT 'A',
                        grmeData_Cadastro DATETIME     NULL DEFAULT NULL
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS mercadoria (
                        auxCodigo            INT           NOT NULL DEFAULT 1,
                        Codigo               INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Codigo_Grupo         INT           NOT NULL DEFAULT 0,
                        mercMercadoria       VARCHAR(150)  NOT NULL DEFAULT '',
                        mercApresentacao     TEXT          NOT NULL,
                        mercPreco_Venda      DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        mercPreco_Custo      DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        mercPreco_Promocional DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        mercEstoque_Atual    DECIMAL(12,4) NOT NULL DEFAULT 0,
                        mercControla_Estoque TINYINT(1)    NOT NULL DEFAULT 0,
                        mercImagem_Url       VARCHAR(300)  NOT NULL DEFAULT '',
                        mercDestaque         TINYINT(1)    NOT NULL DEFAULT 0,
                        mercOrdem            INT           NOT NULL DEFAULT 0,
                        mercHabilitar_Ifood  TINYINT(1)    NOT NULL DEFAULT 0,
                        mercHabilitar_Site   TINYINT(1)    NOT NULL DEFAULT 0,
                        Situacao             CHAR(1)       NOT NULL DEFAULT 'A',
                        Status_Transmissao   CHAR(1)       NOT NULL DEFAULT 'N',
                        Info                 VARCHAR(255)  NOT NULL DEFAULT '',
                        mercData_Cadastro    DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS pedido_web (
                        auxCodigo            INT           NOT NULL DEFAULT 1,
                        Codigo               INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        pediNumero           VARCHAR(20)   NOT NULL DEFAULT '',
                        Codigo_Cliente       INT           NULL DEFAULT NULL,
                        pediNome_Cliente     VARCHAR(150)  NOT NULL DEFAULT '',
                        pediTelefone_Cliente VARCHAR(20)   NOT NULL DEFAULT '',
                        pediSituacao         TINYINT       NOT NULL DEFAULT 0,
                        pediTipo_Entrega     TINYINT       NOT NULL DEFAULT 0,
                        pediForma_Pagamento  TINYINT       NOT NULL DEFAULT 0,
                        pediOrigem           TINYINT       NOT NULL DEFAULT 2,
                        pediSubtotal         DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        pediTaxa_Entrega     DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        pediDesconto         DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        pediValor_Total      DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        pediTroco_Para       DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        pediValor_Pago       DECIMAL(10,2) NULL DEFAULT NULL,
                        pediPago_Dinheiro    DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        pediPago_Cartao      DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        pediPago_Pix         DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        pediCodigo_Transacao VARCHAR(100)  NULL DEFAULT NULL,
                        pediEndereco_Entrega VARCHAR(300)  NOT NULL DEFAULT '',
                        pediObservacoes      TEXT          NOT NULL,
                        pediCancelado_Por    VARCHAR(100)  NULL DEFAULT NULL,
                        pediCodigo_Cupom     VARCHAR(50)   NOT NULL DEFAULT '',
                        pediAutorizador      VARCHAR(150)  NULL DEFAULT NULL,
                        pediData_Lancamento  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        pediData_Atualizacao DATETIME      NULL DEFAULT NULL,
                        Situacao             CHAR(1)       NOT NULL DEFAULT 'A',
                        Status_Transmissao   CHAR(1)       NOT NULL DEFAULT 'N',
                        Info                 VARCHAR(255)  NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS itens_pedido_web (
                        auxCodigo            INT           NOT NULL DEFAULT 1,
                        Codigo               INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Codigo_Pedido        INT           NOT NULL,
                        Codigo_Mercadoria    INT           NOT NULL DEFAULT 0,
                        itpwNome_Mercadoria  VARCHAR(500)  NOT NULL DEFAULT '',
                        itpwQtde             DECIMAL(10,4) NOT NULL DEFAULT 0,
                        itpwPreco_Unitario   DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        itpwSubtotal         DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        itpwObservacoes      VARCHAR(300)  NOT NULL DEFAULT '',
                        itpwDesconto_Pct     DECIMAL(5,2)  NOT NULL DEFAULT 0.00,
                        Situacao             CHAR(1)       NOT NULL DEFAULT 'A',
                        Status_Transmissao   CHAR(1)       NOT NULL DEFAULT 'N',
                        Info                 VARCHAR(255)  NOT NULL DEFAULT ''
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                // ── 4b. Colunas em pedido_web (adicionadas em versões posteriores) ─
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

                // Garante DEFAULT na coluna pediTroco_Para (pode existir sem default em bancos antigos)
                Exec(conn, "UPDATE `pedido_web` SET `pediTroco_Para` = 0.00 WHERE `pediTroco_Para` IS NULL");
                Exec(conn, "ALTER TABLE `pedido_web` MODIFY COLUMN `pediTroco_Para` DECIMAL(10,2) NOT NULL DEFAULT 0.00");

                // ── 4b. Colunas em grupo_mercadoria ─────────────────────────────
                AddColumnIfNotExists(conn, db, "grupo_mercadoria", "grmeData_Cadastro",
                    "DATETIME NULL DEFAULT NULL");
                AddColumnIfNotExists(conn, db, "grupo_mercadoria", "Status_Transmissao",
                    "CHAR(1) NOT NULL DEFAULT 'N'");
                AddColumnIfNotExists(conn, db, "grupo_mercadoria", "Info",
                    "VARCHAR(255) NOT NULL DEFAULT ''");

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
                    "VARCHAR(20) NOT NULL DEFAULT 'VALOR'");
                // Corrige bancos existentes que gravaram a coluna como VARCHAR(10)
                Exec(conn, "ALTER TABLE config_fidelizacao MODIFY COLUMN fidMeta_Tipo VARCHAR(20) NOT NULL DEFAULT 'VALOR'");
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
                    "VARCHAR(7) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "cliente", "cliePedidos_Mensal",
                    "INT NOT NULL DEFAULT 0");
                AddColumnIfNotExists(conn, db, "cliente", "cliePedidos_Mes_Ref",
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

                // ── Licença: colunas empCodigo_Empresa, empChave_Licenca e empData_Graca ──
                AddColumnIfNotExists(conn, db, "empresa", "empCodigo_Empresa",
                    "VARCHAR(20) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "empresa", "empChave_Licenca",
                    "VARCHAR(64) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "empresa", "empData_Graca",
                    "DATE NULL DEFAULT NULL");
                AddColumnIfNotExists(conn, db, "empresa", "empNivel_Atualizacao",
                    "TINYINT NOT NULL DEFAULT 2");
                AddColumnIfNotExists(conn, db, "empresa", "empVersao_Atual",
                    "VARCHAR(20) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "empresa", "empMax_Maquinas",
                    "INT NOT NULL DEFAULT 0");

                // ── Sessões ativas por máquina (controle de máquinas simultâneas) ──
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS sessao_maquina (
                        maq_nome          VARCHAR(100) NOT NULL PRIMARY KEY,
                        ultima_atividade  DATETIME     NOT NULL
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                // Gera o código de empresa se ainda não existir
                {
                    using var chkCod = new MySqlCommand(
                        "SELECT empCodigo_Empresa FROM empresa WHERE Codigo=1 LIMIT 1", conn);
                    var codAtual = chkCod.ExecuteScalar()?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(codAtual))
                    {
                        string novoCod = LicencaService.GerarCodigoEmpresa();
                        using var updCod = new MySqlCommand(
                            "UPDATE empresa SET empCodigo_Empresa=@cod WHERE Codigo=1", conn);
                        updCod.Parameters.AddWithValue("@cod", novoCod);
                        updCod.ExecuteNonQuery();
                    }
                }

                // ── bairro (taxa de entrega por bairro) ──────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS bairro (
                        Codigo           INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        auxCodigo        INT           NOT NULL DEFAULT 1,
                        baiCidade        VARCHAR(100)  NOT NULL DEFAULT '',
                        baiNome          VARCHAR(100)  NOT NULL DEFAULT '',
                        baiTaxa_Entrega  DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        Situacao         CHAR(1)       NOT NULL DEFAULT 'A'
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                // ── marmita ───────────────────────────────────────────────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS marmita (
                        Codigo       INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        auxCodigo    INT           NOT NULL DEFAULT 1,
                        marDescricao VARCHAR(200)  NOT NULL DEFAULT '',
                        marValor     DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        marCusto     DECIMAL(10,2) NOT NULL DEFAULT 0.00,
                        Situacao     CHAR(1)       NOT NULL DEFAULT 'A'
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                AddColumnIfNotExists(conn, db, "marmita", "marCusto",
                    "DECIMAL(10,2) NOT NULL DEFAULT 0.00");
                // Corrige bancos existentes com coluna mais curta
                Exec(conn, "ALTER TABLE itens_pedido_web MODIFY COLUMN itpwNome_Mercadoria VARCHAR(500) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "itens_pedido_web", "itpwCodigo_Marmita",
                    "INT NOT NULL DEFAULT 0");

                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS marmita_item (
                        Codigo             INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        auxCodigo          INT           NOT NULL DEFAULT 1,
                        Codigo_Marmita     INT           NOT NULL,
                        maritmCodigo_Merc  INT           NOT NULL DEFAULT 0,
                        maritmNome         VARCHAR(200)  NOT NULL DEFAULT '',
                        maritmQtde         DECIMAL(10,3) NOT NULL DEFAULT 1.000
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                // ── config_whatsapp (centralizada no banco para funcionar em rede) ─
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS config_whatsapp (
                        Codigo          INT           NOT NULL DEFAULT 1 PRIMARY KEY,
                        whaApiUrl       VARCHAR(300)  NOT NULL DEFAULT '',
                        whaApiKey       VARCHAR(100)  NOT NULL DEFAULT '',
                        whaInstance     VARCHAR(100)  NOT NULL DEFAULT 'pedeai',
                        whaMsgPreparo   TEXT          NOT NULL,
                        whaMsgEntrega   TEXT          NOT NULL,
                        whaMsgCupom     TEXT          NOT NULL
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");

                Exec(conn, @"
                    INSERT IGNORE INTO config_whatsapp
                        (Codigo, whaApiUrl, whaApiKey, whaInstance, whaMsgPreparo, whaMsgEntrega, whaMsgCupom)
                    VALUES (1, '', '', 'pedeai',
                        'Olá {Nome}! 🍕 Seu pedido #{Numero} já está sendo preparado. Em breve ficará pronto!',
                        'Olá {Nome}! 🛵 Seu pedido #{Numero} saiu para entrega. Aguarde em breve!',
                        'Parabéns {Nome}! 🎉 Você ganhou um cupom de desconto: *{CupomCodigo}*\nVálido até {Validade}. Use no seu próximo pedido!')");

                // Migrar configuração WhatsApp do App.config local → banco (somente se banco ainda sem URL)
                {
                    string appUrl = System.Configuration.ConfigurationManager.AppSettings["WhatsAppApiUrl"] ?? "";
                    if (!string.IsNullOrWhiteSpace(appUrl))
                    {
                        using var migCmd = new MySqlCommand(@"
                            UPDATE config_whatsapp SET
                                whaApiUrl=@url, whaApiKey=@key, whaInstance=@inst,
                                whaMsgPreparo=@prep, whaMsgEntrega=@entr, whaMsgCupom=@cup
                            WHERE Codigo=1 AND whaApiUrl=''", conn);
                        migCmd.Parameters.AddWithValue("@url",  appUrl);
                        migCmd.Parameters.AddWithValue("@key",  System.Configuration.ConfigurationManager.AppSettings["WhatsAppApiKey"]      ?? "");
                        migCmd.Parameters.AddWithValue("@inst", System.Configuration.ConfigurationManager.AppSettings["WhatsAppInstance"]     ?? "pedeai");
                        migCmd.Parameters.AddWithValue("@prep", System.Configuration.ConfigurationManager.AppSettings["WhatsAppMsgPreparo"]   ?? "");
                        migCmd.Parameters.AddWithValue("@entr", System.Configuration.ConfigurationManager.AppSettings["WhatsAppMsgEntrega"]   ?? "");
                        migCmd.Parameters.AddWithValue("@cup",  System.Configuration.ConfigurationManager.AppSettings["WhatsAppMsgCupom"]     ?? "");
                        migCmd.ExecuteNonQuery();
                    }
                }

                // ── Integração Supabase: colunas supabase_uuid ───────────────
                AddColumnIfNotExists(conn, db, "grupo_mercadoria", "supabase_uuid",
                    "VARCHAR(50) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "mercadoria", "supabase_uuid",
                    "VARCHAR(50) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "marmita", "supabase_uuid",
                    "VARCHAR(50) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "marmita", "marHabilitar_Site",
                    "TINYINT(1) NOT NULL DEFAULT 0");
                AddColumnIfNotExists(conn, db, "marmita", "marDestaque",
                    "TINYINT(1) NOT NULL DEFAULT 0");
                AddColumnIfNotExists(conn, db, "marmita", "marImagem_Url",
                    "VARCHAR(500) NOT NULL DEFAULT ''");

                // Complement groups linked to a marmita (shown as selectable options on website)
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS marmita_complemento_grupo (
                        Codigo          INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Codigo_Marmita  INT NOT NULL,
                        Codigo_Grupo    INT NOT NULL,
                        grmeDescricao   VARCHAR(200) NOT NULL DEFAULT '',
                        UNIQUE KEY uq_mar_grp (Codigo_Marmita, Codigo_Grupo)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                AddColumnIfNotExists(conn, db, "cupom", "supabase_uuid",
                    "VARCHAR(50) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "bairro", "supabase_uuid",
                    "VARCHAR(50) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "bairro", "baiCEP",
                    "VARCHAR(10) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "pedido_web", "pediSupabase_Id",
                    "VARCHAR(50) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "cliente", "supabase_uuid",
                    "VARCHAR(50) NOT NULL DEFAULT ''");

                // ── Supabase integration: grupo_mercadoria ────────────────────
                AddColumnIfNotExists(conn, db, "grupo_mercadoria", "grmeHabilitar_Site",
                    "TINYINT(1) NOT NULL DEFAULT 0");
                AddColumnIfNotExists(conn, db, "grupo_mercadoria", "grmeImagem_Url",
                    "VARCHAR(500) NOT NULL DEFAULT ''");
                AddColumnIfNotExists(conn, db, "grupo_mercadoria", "supabase_uuid",
                    "VARCHAR(50) NOT NULL DEFAULT ''"  );

                // ── mercadoria_vinculo_grupo: Adicionais/Complementos ─────────
                Exec(conn, @"
                    CREATE TABLE IF NOT EXISTS mercadoria_vinculo_grupo (
                        Codigo        INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Codigo_Mercadoria INT NOT NULL,
                        Codigo_Grupo  INT NOT NULL,
                        tipo          CHAR(1) NOT NULL DEFAULT 'A' COMMENT 'A=Adicional C=Complemento',
                        Situacao      CHAR(1) NOT NULL DEFAULT 'A',
                        UNIQUE KEY uq_merc_grp_tipo (Codigo_Mercadoria, Codigo_Grupo, tipo)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
                // Garante colunas/índice para bancos criados antes desta versão
                 AddColumnIfNotExists(conn, db, "mercadoria_vinculo_grupo", "tipo",
                    "CHAR(1) NOT NULL DEFAULT 'A' COMMENT 'A=Adicional C=Complemento'");
                AddColumnIfNotExists(conn, db, "mercadoria_vinculo_grupo", "Situacao",
                    "CHAR(1) NOT NULL DEFAULT 'A'");
                AddUniqueKeyIfNotExists(conn, db, "mercadoria_vinculo_grupo", "uq_merc_grp_tipo",
                    "(Codigo_Mercadoria, Codigo_Grupo, tipo)");

                // ── mercadoria: campos de precificação de adicional/complemento e fracionado ──
                AddColumnIfNotExists(conn, db, "mercadoria", "mercPreco_Adicional",
                    "DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT 'Preço do produto quando usado como Adicional'");
                AddColumnIfNotExists(conn, db, "mercadoria", "mercFracionado",
                    "TINYINT(1) NOT NULL DEFAULT 0 COMMENT '1=produto com múltiplos sabores (ex: pizza)'");
                AddColumnIfNotExists(conn, db, "mercadoria", "mercQtd_Sabores",
                    "INT NOT NULL DEFAULT 1 COMMENT 'Quantidade máxima de sabores para produto fracionado'");

                // ── empresa: ImgBB API key for image hosting ──────────────
                AddColumnIfNotExists(conn, db, "empresa", "empImgBBKey",
                    "VARCHAR(200) NOT NULL DEFAULT ''");

                // ── empresa: logo URL ─────────────────────────────────────
                AddColumnIfNotExists(conn, db, "empresa", "empLogo_Url",
                    "VARCHAR(500) NOT NULL DEFAULT ''");

                // ── empresa: habilitar conexão com o site (Admin only) ────
                AddColumnIfNotExists(conn, db, "empresa", "empHabilitar_Site",
                    "TINYINT(1) NOT NULL DEFAULT 1");

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

        private static void AddUniqueKeyIfNotExists(
            MySqlConnection conn, string db, string table, string keyName, string cols)
        {
            using var check = new MySqlCommand(@"
                SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
                WHERE TABLE_SCHEMA=@db AND TABLE_NAME=@tbl AND CONSTRAINT_NAME=@key
                  AND CONSTRAINT_TYPE='UNIQUE'", conn);
            check.Parameters.AddWithValue("@db",  db);
            check.Parameters.AddWithValue("@tbl", table);
            check.Parameters.AddWithValue("@key", keyName);
            var exists = Convert.ToInt32(check.ExecuteScalar()) > 0;
            if (!exists)
                Exec(conn, $"ALTER TABLE `{table}` ADD UNIQUE KEY `{keyName}` {cols}");
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
                    "marmita_item", "marmita",
                    "mercadoria", "grupo_mercadoria",
                    "bairro", "taxa_entrega", "forma_pagamento",
                    // WhatsApp / Promoções / Cardápio
                    "promocao_item", "promocao",
                    "cardapio_dia_item", "cardapio_dia",
                    // Fidelização
                    "historico_fidelizacao", "config_fidelizacao",
                    // Empresa e configs (licença incluída — será recriada abaixo)
                    "empresa", "config_impressao"
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

                // Regenera o código de empresa após o reset (licença limpa)
                string novoCodEmp = LicencaService.GerarCodigoEmpresa();
                Exec(conn, $"UPDATE empresa SET empCodigo_Empresa='{novoCodEmp}', empChave_Licenca='', empData_Graca=NULL WHERE Codigo=1");

                Exec(conn, "INSERT IGNORE INTO config_impressao (Codigo) VALUES (1)");
                Exec(conn, @"INSERT IGNORE INTO config_fidelizacao (Codigo, fidMensagem)
                    VALUES (1, 'Parabéns {Nome}! Você atingiu R$ {Meta} em compras e ganhou um cupom {CupomCodigo} válido até {Validade}.')");

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
