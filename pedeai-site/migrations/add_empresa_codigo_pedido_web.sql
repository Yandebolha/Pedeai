-- Migration: add empresa_codigo to pedido_web and itens_pedido_web
-- Run this in Supabase SQL Editor BEFORE syncing.

ALTER TABLE pedido_web        ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE itens_pedido_web  ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;

CREATE INDEX IF NOT EXISTS idx_pedido_web_empresa     ON pedido_web       (empresa_codigo);
CREATE INDEX IF NOT EXISTS idx_itens_pedido_empresa   ON itens_pedido_web (empresa_codigo);

-- Preenche pedidos existentes sem empresa_codigo (se quiser vincular ao código da sua empresa)
-- Substitua 'QNTHGRDC' pelo seu empresa_codigo se necessário.
-- UPDATE pedido_web       SET empresa_codigo = 'QNTHGRDC' WHERE empresa_codigo IS NULL;
-- UPDATE itens_pedido_web SET empresa_codigo = 'QNTHGRDC' WHERE empresa_codigo IS NULL;
