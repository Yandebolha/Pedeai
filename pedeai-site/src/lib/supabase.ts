/// <reference types="vite/client" />
import { createClient } from '@supabase/supabase-js';
import { Database } from '@/types/database';

const supabaseUrl = 'https://supabase.rangofood.com.br';
const supabaseAnonKey = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzdXBhYmFzZSIsImlhdCI6MTc3ODk0MDk2MCwiZXhwIjo0OTM0NjE0NTYwLCJyb2xlIjoiYW5vbiJ9.HC-Er7LJR5K_EH7sPmRioG4uPC07YtbVm-n3wzYyfEM';

export const supabase = createClient<Database>(
  supabaseUrl,
  supabaseAnonKey
);

/**
 * Código único da empresa.
 * Lido em runtime do arquivo /public/empresa-config.js (window.__EMPRESA_CODIGO__).
 * Isso permite trocar o código apenas editando esse arquivo no dist/ sem precisar
 * de um novo build.
 * Fallback para VITE_EMPRESA_CODIGO (variavel de ambiente em build time).
 */
declare global {
  interface Window { __EMPRESA_CODIGO__?: string; }
}
export const empresaCodigo: string =
  (typeof window !== 'undefined' && window.__EMPRESA_CODIGO__)
    ? window.__EMPRESA_CODIGO__
    : (import.meta.env.VITE_EMPRESA_CODIGO ?? '');

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
