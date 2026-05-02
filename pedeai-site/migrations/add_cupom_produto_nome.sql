-- Migration: add produto_nome to cupom table
-- Execute in Supabase SQL Editor

ALTER TABLE cupom
  ADD COLUMN IF NOT EXISTS produto_nome TEXT NULL;

COMMENT ON COLUMN cupom.produto_nome IS 'Nome do produto gratuito (somente para cupons tipo=produto)';
