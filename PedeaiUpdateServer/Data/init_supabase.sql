-- ============================================================
-- Pedeai Update Server — Inicialização das tabelas no Supabase
-- Execute este script uma única vez no SQL Editor do Supabase:
--   Dashboard → SQL Editor → New Query → colar e executar
-- ============================================================

-- Tabela de clientes (instalações)
CREATE TABLE IF NOT EXISTS "Clientes" (
    "Id"             BIGSERIAL    PRIMARY KEY,
    "CodigoEmpresa"  TEXT         NOT NULL,
    "NomeEmpresa"    TEXT         NOT NULL DEFAULT '',
    "Nivel"          INTEGER      NOT NULL DEFAULT 2,
    "VersaoAtual"    TEXT         NOT NULL DEFAULT '',
    "Bloqueado"      BOOLEAN      NOT NULL DEFAULT FALSE,
    "DataRegistro"   TIMESTAMPTZ  NOT NULL DEFAULT now(),
    "UltimaConsulta" TIMESTAMPTZ
);

-- Tabela de pacotes de atualização
CREATE TABLE IF NOT EXISTS "Pacotes" (
    "Id"              BIGSERIAL   PRIMARY KEY,
    "Versao"          TEXT        NOT NULL,
    "Nivel"           INTEGER     NOT NULL DEFAULT 2,
    "Descricao"       TEXT        NOT NULL DEFAULT '',
    "CaminhoArquivo"  TEXT        NOT NULL,
    "TamanhoBytes"    BIGINT      NOT NULL DEFAULT 0,
    "TemSQL"          BOOLEAN     NOT NULL DEFAULT FALSE,
    "DataPublicacao"  TIMESTAMPTZ NOT NULL DEFAULT now(),
    "Ativo"           BOOLEAN     NOT NULL DEFAULT TRUE
);

-- Tabela de histórico de aplicações
CREATE TABLE IF NOT EXISTS "AplicacoesUpdate" (
    "Id"           BIGSERIAL   PRIMARY KEY,
    "ClienteId"    BIGINT      NOT NULL,
    "PacoteId"     BIGINT      NOT NULL,
    "DataDownload" TIMESTAMPTZ,
    "DataAplicada" TIMESTAMPTZ,
    "Status"       TEXT        NOT NULL DEFAULT 'baixado',
    "Detalhe"      TEXT
);

-- Desabilitar Row Level Security (backend server-side — segurança via ApiKey)
ALTER TABLE "Clientes"        DISABLE ROW LEVEL SECURITY;
ALTER TABLE "Pacotes"         DISABLE ROW LEVEL SECURITY;
ALTER TABLE "AplicacoesUpdate" DISABLE ROW LEVEL SECURITY;

-- Conceder permissões ao role anon (usado pela publishable key)
GRANT ALL ON TABLE "Clientes", "Pacotes", "AplicacoesUpdate" TO anon;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO anon;
