/// <reference types="vite/client" />
import { createClient } from '@supabase/supabase-js';
import { Database } from '@/types/database';

const supabaseUrl = 'https://uwgcmnmzjjinfmxlskks.supabase.co';
const supabaseAnonKey = 'sb_publishable_EPB0JSyIMXjchdytJxmOnQ_ylLWM9Nv';

export const supabase = createClient<Database>(
  supabaseUrl,
  supabaseAnonKey
);

/**
 * Código único da empresa. Configurado via variável de ambiente VITE_EMPRESA_CODIGO.
 * Quando definido, todas as queries ao Supabase filtram pelo empresa_codigo para
 * garantir que cada instalação (cliente) veja apenas seus próprios dados.
 */
export const empresaCodigo: string = import.meta.env.VITE_EMPRESA_CODIGO ?? '';

/**
 * Retorna true se o erro do Supabase indica que a coluna empresa_codigo ainda
 * não existe no banco (migration não executada). Nesse caso, a query deve ser
 * refeita sem o filtro para que o site continue funcionando normalmente.
 */
export function isEmpresaCodigoMissing(error: unknown): boolean {
  if (!error || typeof error !== 'object') return false;
  const e = error as { code?: string; message?: string };
  return e.code === 'PGRST204' && (e.message?.includes('empresa_codigo') ?? false);
}
