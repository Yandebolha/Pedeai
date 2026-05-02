-- Adiciona colunas de controle de uso na tabela cupom do Supabase
-- Execute no Supabase SQL Editor

ALTER TABLE cupom
  ADD COLUMN IF NOT EXISTS limite_usos     INTEGER NOT NULL DEFAULT 1,
  ADD COLUMN IF NOT EXISTS usos_realizados INTEGER NOT NULL DEFAULT 0;

-- Índice para facilitar busca de cupons ainda disponíveis
CREATE INDEX IF NOT EXISTS idx_cupom_uso ON cupom (cliente_id, ativo, usos_realizados, limite_usos);
