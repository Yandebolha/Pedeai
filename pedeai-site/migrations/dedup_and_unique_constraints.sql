-- ================================================================
-- Deduplication + Unique Indexes
-- Run ONCE in the Supabase SQL Editor.
-- Keeps the newest row when duplicates exist, then adds unique
-- indexes so the app can safely upsert (ON CONFLICT DO UPDATE).
-- ================================================================

-- ── 1. Remove duplicates ───────────────────────────────────────

-- grupo_mercadoria: keep newest per (nome, empresa_codigo)
DELETE FROM grupo_mercadoria
WHERE id NOT IN (
  SELECT DISTINCT ON (nome, COALESCE(empresa_codigo, ''))
    id
  FROM grupo_mercadoria
  ORDER BY nome, COALESCE(empresa_codigo, ''), created_at DESC
);

-- mercadoria: keep newest per (nome, grupo_id, empresa_codigo)
DELETE FROM mercadoria
WHERE id NOT IN (
  SELECT DISTINCT ON (nome, grupo_id, COALESCE(empresa_codigo, ''))
    id
  FROM mercadoria
  ORDER BY nome, grupo_id, COALESCE(empresa_codigo, ''), created_at DESC
);

-- complemento_grupo: keep newest per (mercadoria_id, nome)
DELETE FROM complemento_grupo
WHERE id NOT IN (
  SELECT DISTINCT ON (mercadoria_id, nome)
    id
  FROM complemento_grupo
  ORDER BY mercadoria_id, nome, created_at DESC
);

-- complemento: keep newest per (grupo_id, nome)
DELETE FROM complemento
WHERE id NOT IN (
  SELECT DISTINCT ON (grupo_id, nome)
    id
  FROM complemento
  ORDER BY grupo_id, nome, created_at DESC
);

-- adicional: keep newest per (mercadoria_id, nome)
DELETE FROM adicional
WHERE id NOT IN (
  SELECT DISTINCT ON (mercadoria_id, nome)
    id
  FROM adicional
  ORDER BY mercadoria_id, nome, created_at DESC
);

-- forma_pagamento: keep newest per (nome, empresa_codigo)
DELETE FROM forma_pagamento
WHERE id NOT IN (
  SELECT DISTINCT ON (nome, COALESCE(empresa_codigo, ''))
    id
  FROM forma_pagamento
  ORDER BY nome, COALESCE(empresa_codigo, ''), created_at DESC
);

-- ── 2. Add unique indexes ──────────────────────────────────────

-- grupo_mercadoria: unique per name per company (NULL empresa treated as '')
CREATE UNIQUE INDEX IF NOT EXISTS uq_grupo_merc_nome_empresa
  ON grupo_mercadoria (nome, (COALESCE(empresa_codigo, '')));

-- mercadoria: unique per name + category + company
CREATE UNIQUE INDEX IF NOT EXISTS uq_mercadoria_nome_grupo_empresa
  ON mercadoria (nome, grupo_id, (COALESCE(empresa_codigo, '')));

-- complemento_grupo: unique per product + group name
CREATE UNIQUE INDEX IF NOT EXISTS uq_comp_grupo_merc_nome
  ON complemento_grupo (mercadoria_id, nome);

-- complemento: unique per group + item name
CREATE UNIQUE INDEX IF NOT EXISTS uq_complemento_grupo_nome
  ON complemento (grupo_id, nome);

-- adicional: unique per product + adicional name
CREATE UNIQUE INDEX IF NOT EXISTS uq_adicional_merc_nome
  ON adicional (mercadoria_id, nome);

-- forma_pagamento: unique per name per company
CREATE UNIQUE INDEX IF NOT EXISTS uq_forma_pgto_nome_empresa
  ON forma_pagamento (nome, (COALESCE(empresa_codigo, '')));
