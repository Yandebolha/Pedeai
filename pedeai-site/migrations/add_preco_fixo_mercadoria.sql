-- Adiciona coluna preco_fixo à tabela mercadoria no Supabase
-- Quando TRUE: preço final = preço base do produto + soma dos sabores selecionados (ex: Açaí R$15 + sabores)
-- Quando FALSE/NULL: preço final = média dos sabores selecionados (ex: Pizza half/half)
ALTER TABLE mercadoria
  ADD COLUMN IF NOT EXISTS preco_fixo BOOLEAN DEFAULT FALSE;
