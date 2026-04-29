-- RanGoFood - Supabase Schema (PostgreSQL)

-- 1. Loja (Configurações)
CREATE TABLE loja (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  nome TEXT NOT NULL DEFAULT 'RanGoFood',
  logo_url TEXT,
  banner_url TEXT,
  cor_primaria TEXT DEFAULT '#EA1D2C',
  taxa_entrega DECIMAL(10,2) DEFAULT 5.00,
  tempo_estimado TEXT DEFAULT '30-45 min',
  endereco TEXT,
  telefone TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 2. Grupo de Mercadoria (Categorias)
CREATE TABLE grupo_mercadoria (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  nome TEXT NOT NULL,
  imagem_url TEXT,
  ordem INTEGER DEFAULT 0,
  ativo BOOLEAN DEFAULT TRUE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 3. Mercadoria (Produtos)
CREATE TABLE mercadoria (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  grupo_id UUID REFERENCES grupo_mercadoria(id) ON DELETE CASCADE,
  nome TEXT NOT NULL,
  descricao TEXT,
  preco_venda DECIMAL(10,2) NOT NULL,
  preco_promocional DECIMAL(10,2),
  imagem_url TEXT,
  ativo BOOLEAN DEFAULT TRUE,
  destaque BOOLEAN DEFAULT FALSE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 4. Complemento Grupo (Ex: Escolha o ponto da carne)
CREATE TABLE complemento_grupo (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  nome TEXT NOT NULL,
  minimo INTEGER DEFAULT 0,
  maximo INTEGER DEFAULT 1,
  obrigatorio BOOLEAN DEFAULT FALSE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 5. Complemento (Ex: Bem passado, Mal passado)
CREATE TABLE complemento (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  grupo_id UUID REFERENCES complemento_grupo(id) ON DELETE CASCADE,
  nome TEXT NOT NULL,
  preco DECIMAL(10,2) DEFAULT 0.00,
  ativo BOOLEAN DEFAULT TRUE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Tabela de ligação Produto <-> Grupos de Complementos
CREATE TABLE mercadoria_complemento_grupo (
  mercadoria_id UUID REFERENCES mercadoria(id) ON DELETE CASCADE,
  grupo_id UUID REFERENCES complemento_grupo(id) ON DELETE CASCADE,
  PRIMARY KEY (mercadoria_id, grupo_id)
);

-- 6. Cliente (Perfil estendido do Auth)
-- 1. Remover a restrição de chave estrangeira se o ID for gerenciado pelo app e não pelo Auth
ALTER TABLE cliente ALTER COLUMN id SET DEFAULT gen_random_uuid();
ALTER TABLE cliente DROP CONSTRAINT IF EXISTS cliente_id_fkey;

-- 2. Garantir que a coluna cpf_cnpj existe
DO $$ 
BEGIN 
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'cliente' AND COLUMN_NAME = 'cpf_cnpj') THEN
        ALTER TABLE cliente ADD COLUMN cpf_cnpj TEXT;
    END IF;
END $$;

-- 3. Atualizar Políticas de RLS para permitir o fluxo de checkout
DROP POLICY IF EXISTS "Usuários podem inserir seu próprio perfil" ON cliente;
DROP POLICY IF EXISTS "Usuários podem ver seu próprio perfil" ON cliente;

-- Permite que qualquer pessoa insira um novo cliente (necessário para o checkout)
CREATE POLICY "Permitir inserção pública de clientes" ON cliente FOR INSERT WITH CHECK (true);

-- Permite que o sistema busque o cliente pelo telefone (para o seu handleSearchWhatsapp)
CREATE POLICY "Permitir busca pública de clientes" ON cliente FOR SELECT USING (true);

-- 7. Cupom
CREATE TABLE cupom (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  codigo TEXT UNIQUE NOT NULL,
  valor DECIMAL(10,2),
  tipo TEXT CHECK (tipo IN ('fixo', 'porcentagem')),
  validade TIMESTAMP WITH TIME ZONE,
  ativo BOOLEAN DEFAULT TRUE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 8. Pedido Web
CREATE TABLE pedido_web (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  cliente_id UUID REFERENCES cliente(id),
  subtotal DECIMAL(10,2) NOT NULL,
  taxa_entrega DECIMAL(10,2) NOT NULL,
  desconto DECIMAL(10,2) DEFAULT 0.00,
  total DECIMAL(10,2) NOT NULL,
  status TEXT DEFAULT 'pendente' CHECK (status IN ('pendente', 'confirmado', 'preparando', 'saiu_para_entrega', 'entregue', 'cancelado')),
  forma_pagamento TEXT,
  troco DECIMAL(10,2),
  endereco_entrega TEXT NOT NULL,
  cupom_id UUID REFERENCES cupom(id),
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- 9. Itens do Pedido
CREATE TABLE itens_pedido_web (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  pedido_id UUID REFERENCES pedido_web(id) ON DELETE CASCADE,
  mercadoria_id UUID REFERENCES mercadoria(id),
  quantidade INTEGER NOT NULL DEFAULT 1,
  preco_unitario DECIMAL(10,2) NOT NULL,
  observacao TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Índices para performance
CREATE INDEX idx_mercadoria_grupo ON mercadoria(grupo_id);
CREATE INDEX idx_pedido_cliente ON pedido_web(cliente_id);
CREATE INDEX idx_itens_pedido ON itens_pedido_web(pedido_id);

-- Configuração de Storage (Bucket público)
-- Nota: Isso deve ser feito via Dashboard do Supabase ou via SQL se as permissões permitirem
-- INSERT INTO storage.buckets (id, name, public) VALUES ('rangofood-images', 'rangofood-images', true);

-- ==========================================
-- 10. ROW LEVEL SECURITY (RLS)
-- ==========================================

-- Ativar RLS em todas as tabelas
ALTER TABLE loja ENABLE ROW LEVEL SECURITY;
ALTER TABLE grupo_mercadoria ENABLE ROW LEVEL SECURITY;
ALTER TABLE mercadoria ENABLE ROW LEVEL SECURITY;
ALTER TABLE complemento_grupo ENABLE ROW LEVEL SECURITY;
ALTER TABLE complemento ENABLE ROW LEVEL SECURITY;
ALTER TABLE mercadoria_complemento_grupo ENABLE ROW LEVEL SECURITY;
ALTER TABLE cliente ENABLE ROW LEVEL SECURITY;
ALTER TABLE cupom ENABLE ROW LEVEL SECURITY;
ALTER TABLE pedido_web ENABLE ROW LEVEL SECURITY;
ALTER TABLE itens_pedido_web ENABLE ROW LEVEL SECURITY;

-- POLÍTICAS DE ACESSO

-- Leitura pública para dados do cardápio e loja
CREATE POLICY "Leitura pública para loja" ON loja FOR SELECT USING (true);
CREATE POLICY "Leitura pública para categorias" ON grupo_mercadoria FOR SELECT USING (true);
CREATE POLICY "Leitura pública para produtos" ON mercadoria FOR SELECT USING (true);
CREATE POLICY "Leitura pública para grupos de complementos" ON complemento_grupo FOR SELECT USING (true);
CREATE POLICY "Leitura pública para complementos" ON complemento FOR SELECT USING (true);
CREATE POLICY "Leitura pública para ligação produto-complemento" ON mercadoria_complemento_grupo FOR SELECT USING (true);

-- Pedidos: Apenas o próprio usuário pode ver seus pedidos e itens
CREATE POLICY "Usuários podem ver seus próprios pedidos" ON pedido_web FOR SELECT USING (auth.uid() = cliente_id);
CREATE POLICY "Qualquer pessoa pode criar pedidos" ON pedido_web FOR INSERT WITH CHECK (true);

CREATE POLICY "Usuários podem ver itens de seus próprios pedidos" ON itens_pedido_web FOR SELECT 
USING (EXISTS (SELECT 1 FROM pedido_web WHERE id = itens_pedido_web.pedido_id AND cliente_id = auth.uid()));

CREATE POLICY "Qualquer pessoa pode inserir itens de pedido" ON itens_pedido_web FOR INSERT WITH CHECK (true);

-- Cupons: Leitura pública (para validação no checkout)
CREATE POLICY "Leitura pública para cupons" ON cupom FOR SELECT USING (true);

-- Tabela de forma de pagamento

-- Criar a tabela de Formas de Pagamento
CREATE TABLE forma_pagamento (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  nome TEXT NOT NULL,
  tipo TEXT CHECK (tipo IN ('cartao', 'pix', 'dinheiro', 'vale')),
  ativo BOOLEAN DEFAULT TRUE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Ativar RLS
ALTER TABLE forma_pagamento ENABLE ROW LEVEL SECURITY;

-- Política de leitura pública
CREATE POLICY "Leitura pública para formas de pagamento" ON forma_pagamento FOR SELECT USING (true);

-- Inserir dados iniciais
INSERT INTO forma_pagamento (nome, tipo) VALUES
('Cartão de Crédito (Maquininha)', 'cartao'),
('Cartão de Débito (Maquininha)', 'cartao'),
('Pix', 'pix'),
('Dinheiro', 'dinheiro'),
('Vale Refeição', 'vale');

-- ==========================================
-- MIGRAÇÃO: Complementos e Adicionais
-- ==========================================

-- Adicionar mercadoria_id direto ao complemento_grupo (ligação simplificada, sem junction table)
ALTER TABLE complemento_grupo
  ADD COLUMN IF NOT EXISTS mercadoria_id UUID REFERENCES mercadoria(id) ON DELETE CASCADE,
  ADD COLUMN IF NOT EXISTS ordem INTEGER DEFAULT 0,
  ADD COLUMN IF NOT EXISTS ativo BOOLEAN DEFAULT TRUE;

-- Adicionar ordem ao complemento
ALTER TABLE complemento
  ADD COLUMN IF NOT EXISTS ordem INTEGER DEFAULT 0;

-- Criar tabela de adicionais (itens pagos que somam ao preço do produto)
CREATE TABLE IF NOT EXISTS adicional (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  mercadoria_id UUID NOT NULL REFERENCES mercadoria(id) ON DELETE CASCADE,
  nome TEXT NOT NULL,
  preco DECIMAL(10,2) NOT NULL DEFAULT 0,
  ordem INTEGER DEFAULT 0,
  ativo BOOLEAN DEFAULT TRUE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Adicionar colunas de detalhe nos itens do pedido
ALTER TABLE itens_pedido_web
  ADD COLUMN IF NOT EXISTS complementos_json JSONB,
  ADD COLUMN IF NOT EXISTS adicionais_json JSONB,
  ADD COLUMN IF NOT EXISTS preco_adicionais DECIMAL(10,2) DEFAULT 0;

-- Índices para performance
CREATE INDEX IF NOT EXISTS idx_complemento_grupo_mercadoria ON complemento_grupo(mercadoria_id);
CREATE INDEX IF NOT EXISTS idx_adicional_mercadoria ON adicional(mercadoria_id);

-- RLS para adicional
ALTER TABLE adicional ENABLE ROW LEVEL SECURITY;
CREATE POLICY "Leitura pública para adicionais" ON adicional FOR SELECT USING (true);