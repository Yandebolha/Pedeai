-- Adiciona empresa_codigo às tabelas complemento e complemento_grupo
-- Executar UMA VEZ no SQL Editor do Supabase antes de sincronizar

ALTER TABLE complemento
  ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;

ALTER TABLE complemento_grupo
  ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;

CREATE INDEX IF NOT EXISTS idx_complemento_empresa
  ON complemento (empresa_codigo);

CREATE INDEX IF NOT EXISTS idx_complemento_grupo_empresa
  ON complemento_grupo (empresa_codigo);

-- Após rodar esta migration, execute uma sincronização completa no sistema
-- para preencher empresa_codigo nos registros existentes via UPDATE abaixo.

-- UPDATE opcional: atribui empresa_codigo aos registros existentes que ainda têm NULL
-- Substitua 'QNTHGRDC' pelo código da sua empresa se diferente.
-- UPDATE complemento SET empresa_codigo = 'QNTHGRDC' WHERE empresa_codigo IS NULL;
-- UPDATE complemento_grupo SET empresa_codigo = 'QNTHGRDC' WHERE empresa_codigo IS NULL;
