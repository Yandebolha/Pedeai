0-- Migração 1: Adiciona suporte a "Adicionais" por categoria para produtos fracionados
-- Migração 2: Adiciona empresa_codigo para isolamento multi-tenant
-- Execute este script no SQL Editor do Supabase
--
-- NOTA: NÃO é necessário preencher empresa_codigo manualmente aqui.
-- O sistema desktop faz isso automaticamente ao sincronizar:
-- ele detecta registros com empresa_codigo NULL e os reivindica para o cliente correto.

-- 1. Adiciona flag is_adicional na tabela mercadoria
ALTER TABLE mercadoria
  ADD COLUMN IF NOT EXISTS is_adicional BOOLEAN DEFAULT FALSE;

-- 2. Adiciona preco_adicional na tabela mercadoria
ALTER TABLE mercadoria
  ADD COLUMN IF NOT EXISTS preco_adicional DECIMAL(10,2) DEFAULT 0;

-- 3. empresa_codigo em todas as tabelas que precisam de isolamento multi-tenant
ALTER TABLE loja                  ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE grupo_mercadoria      ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE mercadoria            ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE complemento_grupo     ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE complemento           ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;

ALTER TABLE adicional             ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE pedido_web            ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE itens_pedido_web      ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE forma_pagamento       ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE cupom                 ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE bairro                ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE taxa_entrega          ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE cliente               ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;
ALTER TABLE enderecos_salvo       ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;

-- 4. Índices de performance
CREATE INDEX IF NOT EXISTS idx_mercadoria_adicional   ON mercadoria(grupo_id, is_adicional, ativo);
CREATE INDEX IF NOT EXISTS idx_mercadoria_empresa     ON mercadoria(empresa_codigo, ativo);
CREATE INDEX IF NOT EXISTS idx_grupo_merc_empresa     ON grupo_mercadoria(empresa_codigo, ativo);
CREATE INDEX IF NOT EXISTS idx_pedido_empresa         ON pedido_web(empresa_codigo, status);
CREATE INDEX IF NOT EXISTS idx_itens_pedido_empresa   ON itens_pedido_web(empresa_codigo);
CREATE INDEX IF NOT EXISTS idx_comp_grupo_empresa     ON complemento_grupo(empresa_codigo);
CREATE INDEX IF NOT EXISTS idx_bairro_empresa         ON bairro(empresa_codigo);
CREATE INDEX IF NOT EXISTS idx_taxa_entrega_empresa   ON taxa_entrega(empresa_codigo);
CREATE INDEX IF NOT EXISTS idx_cliente_empresa        ON cliente(empresa_codigo);
CREATE INDEX IF NOT EXISTS idx_cupom_empresa          ON cupom(empresa_codigo);
