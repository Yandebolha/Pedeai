-- ================================================================
-- SCRIPT DE CRIAÇÃO DO BANCO - ORDEM CORRIGIDA
-- Execute diretamente no SQL Editor do Supabase.
-- NÃO use CREATE DATABASE — o banco já existe no Supabase/VPS.
-- ================================================================

-- ---------------------------------------------------------------
-- 1. TABELAS SEM DEPENDÊNCIAS
-- ---------------------------------------------------------------

CREATE TABLE IF NOT EXISTS public."Clientes" (
  "Id"             bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  "CodigoEmpresa"  text NOT NULL,
  "NomeEmpresa"    text NOT NULL DEFAULT '',
  "Nivel"          integer NOT NULL DEFAULT 2,
  "VersaoAtual"    text NOT NULL DEFAULT '',
  "Bloqueado"      boolean NOT NULL DEFAULT false,
  "DataRegistro"   timestamp with time zone NOT NULL DEFAULT now(),
  "UltimaConsulta" timestamp with time zone,
  "ChaveLicenca"   text,
  "MaxMaquinas"    integer NOT NULL DEFAULT 0,
  CONSTRAINT "Clientes_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public."Pacotes" (
  "Id"             bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  "Versao"         text NOT NULL,
  "Nivel"          integer NOT NULL DEFAULT 2,
  "Descricao"      text NOT NULL DEFAULT '',
  "CaminhoArquivo" text NOT NULL,
  "TamanhoBytes"   bigint NOT NULL DEFAULT 0,
  "TemSQL"         boolean NOT NULL DEFAULT false,
  "DataPublicacao" timestamp with time zone NOT NULL DEFAULT now(),
  "Ativo"          boolean NOT NULL DEFAULT true,
  CONSTRAINT "Pacotes_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public."AplicacoesUpdate" (
  "Id"           bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  "ClienteId"    bigint NOT NULL,
  "PacoteId"     bigint NOT NULL,
  "DataDownload" timestamp with time zone,
  "DataAplicada" timestamp with time zone,
  "Status"       text NOT NULL DEFAULT 'baixado',
  "Detalhe"      text,
  CONSTRAINT "AplicacoesUpdate_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public.bairro (
  codigo          integer GENERATED ALWAYS AS IDENTITY NOT NULL,
  auxcodigo       integer NOT NULL DEFAULT 1,
  baicidade       character varying NOT NULL DEFAULT '',
  bainome         character varying NOT NULL DEFAULT '',
  baitaxa_entrega numeric NOT NULL DEFAULT 0.00,
  situacao        character NOT NULL DEFAULT 'A',
  empresa_codigo  text,
  CONSTRAINT bairro_pkey PRIMARY KEY (codigo)
);

CREATE TABLE IF NOT EXISTS public.cliente (
  id                    uuid NOT NULL DEFAULT gen_random_uuid(),
  nome                  text NOT NULL,
  telefone              numeric,
  endereco_padrao       text,
  created_at            timestamp with time zone DEFAULT now(),
  cpf_cnpj              numeric,
  empresa_codigo        text,
  desconto_porcentagem  numeric DEFAULT 0,
  CONSTRAINT cliente_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.enderecos_salvo (
  id             uuid NOT NULL DEFAULT gen_random_uuid(),
  whatsapp       text NOT NULL,
  endereco       text NOT NULL,
  bairro         text,
  numero         text,
  complemento    text,
  cidade         text DEFAULT 'Cidade Exemplo',
  created_at     timestamp with time zone DEFAULT now(),
  cep            character varying,
  uf             character varying,
  empresa_codigo text,
  CONSTRAINT enderecos_salvo_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.forma_pagamento (
  id             uuid NOT NULL DEFAULT gen_random_uuid(),
  nome           text NOT NULL,
  tipo           text CHECK (tipo = ANY (ARRAY['cartao'::text, 'pix'::text, 'dinheiro'::text, 'vale'::text])),
  ativo          boolean DEFAULT true,
  created_at     timestamp with time zone DEFAULT now(),
  empresa_codigo text,
  CONSTRAINT forma_pagamento_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.grupo_mercadoria (
  id             uuid NOT NULL DEFAULT gen_random_uuid(),
  nome           text NOT NULL,
  imagem_url     text,
  ordem          integer DEFAULT 0,
  ativo          boolean DEFAULT true,
  created_at     timestamp with time zone DEFAULT now(),
  empresa_codigo text,
  CONSTRAINT grupo_mercadoria_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.loja (
  id             uuid NOT NULL DEFAULT gen_random_uuid(),
  nome           text NOT NULL DEFAULT 'RanGoFood',
  logo_url       text,
  banner_url     text,
  cor_primaria   text DEFAULT '#EA1D2C',
  taxa_entrega   numeric DEFAULT 5.00,
  tempo_estimado text DEFAULT '30-45 min',
  endereco       text,
  telefone       text,
  created_at     timestamp with time zone DEFAULT now(),
  empresa_codigo text,
  CONSTRAINT loja_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.taxa_entrega (
  id             uuid NOT NULL DEFAULT gen_random_uuid(),
  cep            text NOT NULL UNIQUE,
  valor          numeric NOT NULL,
  created_at     timestamp with time zone DEFAULT now(),
  bairro         text,
  cidade         text,
  empresa_codigo text,
  CONSTRAINT taxa_entrega_pkey PRIMARY KEY (id)
);

-- ---------------------------------------------------------------
-- 2. TABELAS QUE DEPENDEM DE grupo_mercadoria e cliente
-- ---------------------------------------------------------------

CREATE TABLE IF NOT EXISTS public.mercadoria (
  id             uuid NOT NULL DEFAULT gen_random_uuid(),
  grupo_id       uuid,
  nome           text NOT NULL,
  descricao      text,
  preco_venda    numeric NOT NULL,
  imagem_url     text,
  ativo          boolean DEFAULT true,
  destaque       boolean DEFAULT false,
  created_at     timestamp with time zone DEFAULT now(),
  fracionado     boolean DEFAULT false,
  qtd_sabores    integer DEFAULT 1,
  is_adicional   boolean DEFAULT false,
  preco_adicional numeric DEFAULT 0,
  empresa_codigo text,
  preco_fixo     boolean DEFAULT false,
  CONSTRAINT mercadoria_pkey PRIMARY KEY (id),
  CONSTRAINT mercadoria_grupo_id_fkey FOREIGN KEY (grupo_id) REFERENCES public.grupo_mercadoria(id)
);

CREATE TABLE IF NOT EXISTS public.cupom (
  id              uuid NOT NULL DEFAULT gen_random_uuid(),
  codigo          text NOT NULL UNIQUE,
  valor           numeric,
  tipo            text CHECK (tipo = ANY (ARRAY['fixo'::text, 'porcentagem'::text, 'produto'::text])),
  validade        timestamp with time zone,
  ativo           boolean DEFAULT true,
  created_at      timestamp with time zone DEFAULT now(),
  empresa_codigo  text,
  cliente_id      uuid,
  limite_usos     integer NOT NULL DEFAULT 1,
  usos_realizados integer NOT NULL DEFAULT 0,
  produto_nome    text,
  CONSTRAINT cupom_pkey PRIMARY KEY (id),
  CONSTRAINT cupom_cliente_id_fkey FOREIGN KEY (cliente_id) REFERENCES public.cliente(id)
);

-- ---------------------------------------------------------------
-- 3. TABELAS QUE DEPENDEM DE mercadoria e cupom
-- ---------------------------------------------------------------

CREATE TABLE IF NOT EXISTS public.complemento_grupo (
  id             uuid NOT NULL DEFAULT gen_random_uuid(),
  nome           text NOT NULL,
  minimo         integer DEFAULT 0,
  maximo         integer DEFAULT 1,
  obrigatorio    boolean DEFAULT false,
  created_at     timestamp with time zone DEFAULT now(),
  calculo_preco  text DEFAULT 'soma',
  mercadoria_id  uuid,
  ordem          integer DEFAULT 0,
  ativo          boolean DEFAULT true,
  empresa_codigo text,
  CONSTRAINT complemento_grupo_pkey PRIMARY KEY (id),
  CONSTRAINT complemento_grupo_mercadoria_id_fkey FOREIGN KEY (mercadoria_id) REFERENCES public.mercadoria(id)
);

CREATE TABLE IF NOT EXISTS public.adicional (
  id             uuid NOT NULL DEFAULT gen_random_uuid(),
  mercadoria_id  uuid NOT NULL,
  nome           text NOT NULL,
  preco          numeric NOT NULL DEFAULT 0,
  ordem          integer DEFAULT 0,
  ativo          boolean DEFAULT true,
  created_at     timestamp with time zone DEFAULT now(),
  empresa_codigo text,
  max_qtde       integer NOT NULL DEFAULT 1,
  imagem_url     text,
  CONSTRAINT adicional_pkey PRIMARY KEY (id),
  CONSTRAINT adicional_mercadoria_id_fkey FOREIGN KEY (mercadoria_id) REFERENCES public.mercadoria(id)
);

CREATE TABLE IF NOT EXISTS public.pedido_web (
  id               uuid NOT NULL DEFAULT gen_random_uuid(),
  cliente_id       uuid,
  subtotal         numeric NOT NULL,
  taxa_entrega     numeric NOT NULL,
  desconto         numeric DEFAULT 0.00,
  total            numeric NOT NULL,
  status           text DEFAULT 'pendente'
                   CHECK (status = ANY (ARRAY[
                     'pendente'::text, 'confirmado'::text, 'preparando'::text,
                     'saiu_para_entrega'::text, 'entregue'::text, 'cancelado'::text
                   ])),
  forma_pagamento  text,
  endereco_entrega text NOT NULL,
  cupom_id         uuid,
  created_at       timestamp with time zone DEFAULT now(),
  troco            numeric,
  empresa_codigo   text,
  CONSTRAINT pedido_web_pkey PRIMARY KEY (id),
  CONSTRAINT pedido_web_cliente_id_fkey FOREIGN KEY (cliente_id) REFERENCES public.cliente(id),
  CONSTRAINT pedido_web_cupom_id_fkey  FOREIGN KEY (cupom_id)   REFERENCES public.cupom(id)
);

-- ---------------------------------------------------------------
-- 4. TABELAS QUE DEPENDEM DE complemento_grupo, pedido_web e mercadoria
-- ---------------------------------------------------------------

CREATE TABLE IF NOT EXISTS public.complemento (
  id             uuid NOT NULL DEFAULT gen_random_uuid(),
  grupo_id       uuid,
  nome           text NOT NULL,
  preco          numeric DEFAULT 0.00,
  ativo          boolean DEFAULT true,
  created_at     timestamp with time zone DEFAULT now(),
  descricao      text,
  imagem_url     text,
  ordem          integer DEFAULT 0,
  empresa_codigo text,
  CONSTRAINT complemento_pkey PRIMARY KEY (id),
  CONSTRAINT complemento_grupo_id_fkey FOREIGN KEY (grupo_id) REFERENCES public.complemento_grupo(id)
);

CREATE TABLE IF NOT EXISTS public.itens_pedido_web (
  id               uuid NOT NULL DEFAULT gen_random_uuid(),
  pedido_id        uuid,
  mercadoria_id    uuid,
  quantidade       integer NOT NULL DEFAULT 1,
  preco_unitario   numeric NOT NULL,
  observacao       text,
  created_at       timestamp with time zone DEFAULT now(),
  complementos_json jsonb,
  adicionais_json  jsonb,
  preco_adicionais numeric DEFAULT 0,
  empresa_codigo   text,
  CONSTRAINT itens_pedido_web_pkey PRIMARY KEY (id),
  CONSTRAINT itens_pedido_web_pedido_id_fkey    FOREIGN KEY (pedido_id)     REFERENCES public.pedido_web(id),
  CONSTRAINT itens_pedido_web_mercadoria_id_fkey FOREIGN KEY (mercadoria_id) REFERENCES public.mercadoria(id)
);

CREATE TABLE IF NOT EXISTS public.mercadoria_complemento_grupo (
  mercadoria_id uuid NOT NULL,
  grupo_id      uuid NOT NULL,
  CONSTRAINT mercadoria_complemento_grupo_pkey           PRIMARY KEY (mercadoria_id, grupo_id),
  CONSTRAINT mercadoria_complemento_grupo_mercadoria_id_fkey FOREIGN KEY (mercadoria_id) REFERENCES public.mercadoria(id),
  CONSTRAINT mercadoria_complemento_grupo_grupo_id_fkey      FOREIGN KEY (grupo_id)      REFERENCES public.complemento_grupo(id)
);
