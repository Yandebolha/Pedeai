-- =============================================================
-- Pedeai — Migração: Usuários, Empresa, Gastos de Material
-- Execute no MySQL antes de iniciar o sistema.
-- =============================================================

-- ── Tabela: usuario ─────────────────────────────────────────
CREATE TABLE IF NOT EXISTS usuario (
    auxCodigo        INT           NOT NULL DEFAULT 1,
    Codigo           INT           NOT NULL AUTO_INCREMENT PRIMARY KEY,
    usuNome          VARCHAR(100)  NOT NULL,
    usuLogin         VARCHAR(50)   NOT NULL UNIQUE,
    usuSenha         VARCHAR(64)   NOT NULL,   -- SHA-256 hex
    usuNivel         TINYINT       NOT NULL DEFAULT 1  COMMENT '1=Operador 2=Gerente 9=Admin',
    Situacao         CHAR(1)       NOT NULL DEFAULT 'A',
    usuData_Cadastro DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Info             VARCHAR(255)  NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Usuario administrador padrão (senha: $up0rte)
-- SHA-256 de '$up0rte' = 4871a52c4788770a274fec042f22cab797112c35492511eaf472df827fbdbfc2
INSERT INTO usuario (auxCodigo,Codigo,usuNome,usuLogin,usuSenha,usuNivel,Situacao)
VALUES (1,1,'Administrador','admin','4871a52c4788770a274fec042f22cab797112c35492511eaf472df827fbdbfc2',9,'A')
ON DUPLICATE KEY UPDATE usuSenha='4871a52c4788770a274fec042f22cab797112c35492511eaf472df827fbdbfc2', usuNivel=9, Situacao='A';

-- ── Tabela: empresa ──────────────────────────────────────────
CREATE TABLE IF NOT EXISTS empresa (
    Codigo          INT          NOT NULL DEFAULT 1 PRIMARY KEY,
    empNome         VARCHAR(100) NOT NULL DEFAULT '',
    empNome_Fantasia VARCHAR(100) NOT NULL DEFAULT '',
    empCNPJ         VARCHAR(18)  NOT NULL DEFAULT '',
    empTelefone     VARCHAR(20)  NOT NULL DEFAULT '',
    empEmail        VARCHAR(100) NOT NULL DEFAULT '',
    empEndereco     VARCHAR(255) NOT NULL DEFAULT '',
    Info            VARCHAR(255) NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO empresa (Codigo,empNome) VALUES (1,'Minha Empresa')
ON DUPLICATE KEY UPDATE Codigo=Codigo;

-- ── Tabela: gasto_material ───────────────────────────────────
CREATE TABLE IF NOT EXISTS gasto_material (
    auxCodigo        INT            NOT NULL DEFAULT 1,
    Codigo           INT            NOT NULL AUTO_INCREMENT PRIMARY KEY,
    gmatData         DATE           NOT NULL,
    gmatDescricao    VARCHAR(255)   NOT NULL,
    gmatValor        DECIMAL(10,2)  NOT NULL DEFAULT 0.00,
    gmatObservacoes  VARCHAR(500)   NOT NULL DEFAULT '',
    Situacao         CHAR(1)        NOT NULL DEFAULT 'A',
    Info             VARCHAR(255)   NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ── Coluna pediCancelado_Por em pedido_web ────────────────────
-- Compatível com MySQL 5.7+
SET @dbname = DATABASE();
SET @colname = 'pediCancelado_Por';
SET @tblname = 'pedido_web';
SET @sql = IF(
    (SELECT COUNT(*) FROM information_schema.COLUMNS
     WHERE TABLE_SCHEMA = @dbname
       AND TABLE_NAME   = @tblname
       AND COLUMN_NAME  = @colname) = 0,
    CONCAT('ALTER TABLE `', @tblname, '` ADD COLUMN `', @colname, '` VARCHAR(100) NULL DEFAULT NULL AFTER pediCodigo_Transacao'),
    'SELECT 1'
);
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
