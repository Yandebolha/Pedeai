-- Adiciona empresa_codigo à tabela forma_pagamento (multi-tenant)
-- Executar UMA VEZ no SQL Editor do Supabase

ALTER TABLE forma_pagamento
  ADD COLUMN IF NOT EXISTS empresa_codigo TEXT;

-- Índice para performance nas queries por empresa
CREATE INDEX IF NOT EXISTS idx_forma_pagamento_empresa
  ON forma_pagamento (empresa_codigo);

-- Remover registros genéricos/duplicados do schema inicial
-- "Cartão" genérico é redundante pois já existem "Cartão Crédito" e "Cartão Débito"
DELETE FROM forma_pagamento
  WHERE nome IN (
    'Cartão',
    'Cartão de Crédito (Maquininha)',
    'Cartão de Débito (Maquininha)',
    'Vale Refeição'
  );

-- Atualiza RLS policy para leitura pública continuar funcionando
-- (a política existente FOR SELECT USING (true) já cobre a nova coluna)
