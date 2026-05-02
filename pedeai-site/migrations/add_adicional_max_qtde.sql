-- Migration: add max_qtde to adicional table
-- Execute in Supabase SQL Editor

ALTER TABLE adicional
  ADD COLUMN IF NOT EXISTS max_qtde INTEGER NOT NULL DEFAULT 1;

COMMENT ON COLUMN adicional.max_qtde IS 'Quantidade máxima que pode ser adicionada por pedido (1 = padrão)';
