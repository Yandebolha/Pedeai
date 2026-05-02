-- ============================================================
-- Migração: Desconto por cliente (fixo % e cupom de fidelização)
-- ============================================================

-- 1. Adiciona desconto_porcentagem na tabela cliente
--    Permite atribuir um desconto fixo (%) ao cliente diretamente no cadastro.
ALTER TABLE cliente
  ADD COLUMN IF NOT EXISTS desconto_porcentagem DECIMAL(5,2) DEFAULT 0;

-- 2. Adiciona cliente_id na tabela cupom
--    Permite criar cupons de fidelização vinculados a um cliente específico.
ALTER TABLE cupom
  ADD COLUMN IF NOT EXISTS cliente_id UUID REFERENCES cliente(id) ON DELETE SET NULL;

-- Índice para buscar cupons por cliente com rapidez
CREATE INDEX IF NOT EXISTS idx_cupom_cliente_id ON cupom(cliente_id);

-- ============================================================
-- MySQL (executar no banco local do sistema desktop):
-- ALTER TABLE cliente ADD COLUMN IF NOT EXISTS clieDesconto_Porcentagem DECIMAL(5,2) DEFAULT 0;
-- ============================================================
