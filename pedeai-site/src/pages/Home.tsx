import { useState, useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { Header } from '@/components/Header';
import { CategoryList } from '@/components/CategoryList';
import { ProductFeed } from '@/components/ProductFeed';
import { useCartStore } from '@/store/useCartStore';
import { ShoppingBag, ArrowRight, Loader2 } from 'lucide-react';
import { motion, AnimatePresence } from 'motion/react';
import { useQuery } from '@tanstack/react-query';
import { supabase, empresaCodigo, isEmpresaCodigoMissing } from '@/lib/supabase';
import { Categoria, Produto, Loja } from '@/types/database';

export default function Home() {
  const navigate = useNavigate();
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState('');
  const cartItems = useCartStore((state) => state.items);
  const cartTotal = useCartStore((state) => state.getTotal());

  // Busca as informações da loja (como o banner_url)
  const { data: store, isLoading: isLoadingStore } = useQuery({
    queryKey: ['store', empresaCodigo],
    queryFn: async () => {
      let q = supabase.from('loja').select('*');
      if (empresaCodigo) q = (q as any).eq('empresa_codigo', empresaCodigo);
      const { data, error } = await (q as any).limit(1).maybeSingle();
      if (error) {
        if (isEmpresaCodigoMissing(error)) {
          const { data: d2 } = await supabase.from('loja').select('*').limit(1).maybeSingle();
          return (d2 ?? null) as Loja | null;
        }
        return null;
      }
      return (data ?? null) as Loja | null;
    },
  });

  // Busca as categorias reais do banco de dados
  const { data: categoriesRaw, isLoading: isLoadingCats } = useQuery({
    queryKey: ['categories', empresaCodigo],
    queryFn: async () => {
      let q = supabase.from('grupo_mercadoria').select('*').eq('ativo', true).order('ordem');
      if (empresaCodigo) q = (q as any).eq('empresa_codigo', empresaCodigo);
      const { data, error } = await q;
      if (error) {
        if (isEmpresaCodigoMissing(error)) {
          const { data: d2, error: e2 } = await supabase.from('grupo_mercadoria').select('*').eq('ativo', true).order('ordem');
          if (e2) throw e2;
          return d2 as Categoria[];
        }
        throw error;
      }
      return data as Categoria[];
    },
  });

  // Deduplica categorias pelo nome (evita duplicatas de sync)
  const categories = useMemo(() => {
    if (!categoriesRaw) return [];
    const seen = new Set<string>();
    return categoriesRaw.filter((c) => {
      const key = c.nome.trim().toLowerCase();
      if (seen.has(key)) return false;
      seen.add(key);
      return true;
    });
  }, [categoriesRaw]);

  // Busca os produtos reais do banco de dados
  const { data: products, isLoading: isLoadingProds } = useQuery({
    queryKey: ['products', empresaCodigo],
    queryFn: async () => {
      let q = supabase.from('mercadoria').select('*').eq('ativo', true).not('is_adicional', 'is', true).order('destaque', { ascending: false });
      if (empresaCodigo) q = (q as any).eq('empresa_codigo', empresaCodigo);
      const { data, error } = await q;
      if (error) {
        if (isEmpresaCodigoMissing(error)) {
          const { data: d2, error: e2 } = await supabase.from('mercadoria').select('*').eq('ativo', true).not('is_adicional', 'is', true).order('destaque', { ascending: false });
          if (e2) throw e2;
          return d2 as Produto[];
        }
        throw error;
      }
      return data as Produto[];
    },
  });

  // Define a primeira categoria como selecionada assim que os dados carregarem
  useEffect(() => {
    if (categories && categories.length > 0 && !selectedCategoryId) {
      setSelectedCategoryId(categories[0].id);
    }
  }, [categories, selectedCategoryId]);

  const normalizeText = (text: string | null | undefined) =>
    (text || '').normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase().trim();

  const isSearching = searchQuery.trim().length > 0;

  const filteredProducts = useMemo(() => {
    if (!products) return [];
    
    const term = normalizeText(searchQuery);
    
    if (term.length > 0) {
      return products.filter((p) => 
        normalizeText(p.nome).includes(term) || 
        (p.descricao ? normalizeText(p.descricao).includes(term) : false)
      );
    }
    
    return products.filter(p => p.grupo_id === selectedCategoryId);
  }, [products, searchQuery, selectedCategoryId]);

  const selectedCategoryName = isSearching 
    ? `Resultados para "${searchQuery}"`
    : (categories?.find((c) => c.id === selectedCategoryId)?.nome || 'Produtos');

  if (isLoadingCats || isLoadingProds || isLoadingStore) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-white">
        <div className="flex flex-col items-center gap-2">
          <Loader2 className="w-8 h-8 text-red-600 animate-spin" />
          <p className="text-gray-500 font-medium">Carregando cardápio...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-white pb-24">
      <Header searchQuery={searchQuery} onSearchChange={setSearchQuery} />
      
      <main className="max-w-7xl mx-auto">
        {/* Banners Carousel (Mock) */}
        <div className="px-4 py-4">
          <div className="w-full aspect-[21/9] rounded-xl overflow-hidden shadow-sm">
            <img
              src={store?.banner_url || 'https://picsum.photos/seed/food-banner/800/400'}
              alt={store?.nome || 'Banner da loja'}
              className="w-full h-full object-cover"
              referrerPolicy="no-referrer"
            />
          </div>
        </div>

        {/* Categories */}
        {categories && (
          <CategoryList
            categories={categories}
            selectedId={selectedCategoryId || ''}
            onSelect={(id) => {
              setSelectedCategoryId(id);
              setSearchQuery(''); // Limpa a busca ao filtrar por uma categoria específica
            }}
          />
        )}

        {/* Products */}
        <ProductFeed
          products={filteredProducts}
          categoryName={selectedCategoryName}
        />
      </main>

      {/* Floating Cart Button */}
      <AnimatePresence>
        {cartItems.length > 0 && (
          <motion.div
            initial={{ y: 100, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            exit={{ y: 100, opacity: 0 }}
            className="fixed bottom-6 left-4 right-4 z-50"
          >
            <button 
              onClick={() => navigate('/cart')}
              className="w-full bg-red-600 text-white flex items-center justify-between px-6 py-4 rounded-xl shadow-2xl hover:bg-red-700 transition-all group"
            >
              <div className="flex items-center gap-3">
                <div className="bg-red-500 rounded-lg p-2">
                  <ShoppingBag className="w-5 h-5" />
                </div>
                <div className="text-left">
                  <p className="text-[10px] uppercase font-bold opacity-80">Ver Sacola</p>
                  <p className="text-sm font-bold">
                    {cartItems.length} {cartItems.length === 1 ? 'item' : 'itens'}
                  </p>
                </div>
              </div>
              
              <div className="flex items-center gap-2">
                <span className="text-lg font-bold">
                  R$ {cartTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                </span>
                <ArrowRight className="w-5 h-5 group-hover:translate-x-1 transition-transform" />
              </div>
            </button>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}
