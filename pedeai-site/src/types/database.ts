export type Json =
  | string
  | number
  | boolean
  | null
  | { [key: string]: Json | undefined }
  | Json[]

export interface Database {
  public: {
    Tables: {
      loja: {
        Row: {
          id: string
          nome: string
          logo_url: string | null
          banner_url: string | null
          cor_primaria: string
          taxa_entrega: number
          tempo_estimado: string
          endereco: string | null
          telefone: string | null
          created_at: string
        }
      }
      grupo_mercadoria: {
        Row: {
          id: string
          nome: string
          imagem_url: string | null
          ordem: number
          ativo: boolean
          created_at: string
        }
      }
      mercadoria: {
        Row: {
          id: string
          grupo_id: string | null
          nome: string
          descricao: string | null
          preco_venda: number
          preco_promocional: number | null
          imagem_url: string | null
          ativo: boolean
          destaque: boolean
          created_at: string
        }
      }
      cliente: {
        Row: {
          id: string
          nome: string
          telefone: string | null
          cpf_cnpj: string | null
          endereco_padrao: string | null
          created_at: string
        }
      }
      pedido_web: {
        Row: {
          id: string
          cliente_id: string | null
          subtotal: number
          taxa_entrega: number
          desconto: number
          total: number
          status: string
          forma_pagamento: string | null
          endereco_entrega: string
          cupom_id: string | null
          created_at: string
        }
      }
      forma_pagamento: {
        Row: {
          id: string
          nome: string
          tipo: string
          ativo: boolean
          created_at: string
        }
      }
      enderecos_salvo: {
        Row: {
          id: string
          whatsapp: string
          endereco: string
          bairro: string | null
          numero: string | null
          complemento: string | null
          cidade: string | null
          cep: string | null
          created_at: string
        }
      }
      complemento_grupo: {
        Row: {
          id: string
          mercadoria_id: string | null
          nome: string
          minimo: number
          maximo: number
          obrigatorio: boolean
          ordem: number
          ativo: boolean
          created_at: string
        }
      }
      complemento: {
        Row: {
          id: string
          grupo_id: string | null
          nome: string
          preco: number
          ordem: number
          ativo: boolean
          created_at: string
        }
      }
      adicional: {
        Row: {
          id: string
          mercadoria_id: string | null
          nome: string
          preco: number
          ordem: number
          ativo: boolean
          created_at: string
        }
      }
      itens_pedido_web: {
        Row: {
          id: string
          pedido_id: string | null
          mercadoria_id: string | null
          quantidade: number
          preco_unitario: number
          preco_adicionais: number
          complementos_json: unknown | null
          adicionais_json: unknown | null
          observacao: string | null
          created_at: string
        }
      }
    }
  }
}

export type Loja = Database['public']['Tables']['loja']['Row'];
export type Categoria = Database['public']['Tables']['grupo_mercadoria']['Row'];
export type Produto = Database['public']['Tables']['mercadoria']['Row'];
export type Cliente = Database['public']['Tables']['cliente']['Row'];
export type Pedido = Database['public']['Tables']['pedido_web']['Row'];
export type FormaPagamento = Database['public']['Tables']['forma_pagamento']['Row'];
export type EnderecoSalvo = Database['public']['Tables']['enderecos_salvo']['Row'];
export type ComplementoGrupo = Database['public']['Tables']['complemento_grupo']['Row'];
export type ComplementoItem = Database['public']['Tables']['complemento']['Row'];
export type Adicional = Database['public']['Tables']['adicional']['Row'];

export interface ComplementoGrupoComItens extends ComplementoGrupo {
  complemento: ComplementoItem[];
}
