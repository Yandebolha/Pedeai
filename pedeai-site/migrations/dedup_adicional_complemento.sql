-- Migration: Deduplica tabelas adicional e complemento e adiciona constraints únicas
-- Execute no Supabase SQL Editor (Dashboard > SQL Editor)
-- ATENÇÃO: Faça backup antes de executar.

-- ─── 1. Remove duplicatas da tabela adicional ────────────────────────────────
-- Para cada par (mercadoria_id, nome), mantém apenas o registro mais antigo (menor ctid).
DELETE FROM adicional
WHERE id IN (
  SELECT id FROM (
    SELECT id,
           ROW_NUMBER() OVER (
             PARTITION BY mercadoria_id, nome
             ORDER BY id ASC
           ) AS rn
    FROM adicional
  ) ranked
  WHERE rn > 1
);

-- ─── 2. Remove duplicatas da tabela complemento ──────────────────────────────
DELETE FROM complemento
WHERE id IN (
  SELECT id FROM (
    SELECT id,
           ROW_NUMBER() OVER (
             PARTITION BY grupo_id, nome
             ORDER BY id ASC
           ) AS rn
    FROM complemento
  ) ranked
  WHERE rn > 1
);

-- ─── 3. Adiciona constraints únicas para evitar duplicatas futuras ────────────
-- adicional: um produto não pode ter dois adicionais com o mesmo nome
ALTER TABLE adicional
  ADD CONSTRAINT adicional_mercadoria_id_nome_unique
  UNIQUE (mercadoria_id, nome);

-- complemento: um grupo não pode ter dois itens com o mesmo nome
ALTER TABLE complemento
  ADD CONSTRAINT complemento_grupo_id_nome_unique
  UNIQUE (grupo_id, nome);
