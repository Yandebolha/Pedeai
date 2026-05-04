-- Adiciona coluna imagem_url nas tabelas complemento e adicional
-- para exibir imagens dos sabores e adicionais no modal do produto no site.

ALTER TABLE complemento
  ADD COLUMN IF NOT EXISTS imagem_url TEXT;

ALTER TABLE adicional
  ADD COLUMN IF NOT EXISTS imagem_url TEXT;
