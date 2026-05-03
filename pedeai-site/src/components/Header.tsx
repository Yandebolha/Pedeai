import { MapPin, ChevronDown, Search, ShoppingBag } from 'lucide-react';
import { useCartStore } from '@/store/useCartStore';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { supabase, empresaCodigo, isEmpresaCodigoMissing } from '@/lib/supabase';
import { Loja } from '@/types/database';

interface HeaderProps {
  searchQuery?: string;
  onSearchChange?: (value: string) => void;
}

export function Header({ searchQuery, onSearchChange }: HeaderProps) {
  const navigate = useNavigate();
  const items = useCartStore((state) => state.items);
  const itemCount = items.reduce((acc, item) => acc + item.quantity, 0);

  // Busca as informações da loja (incluindo o endereço)
  const { data: store } = useQuery({
    queryKey: ['store', empresaCodigo],
    queryFn: async () => {
      const q = supabase.from('loja').select('*');
      const { data, error } = empresaCodigo
        ? await q.eq('empresa_codigo', empresaCodigo).limit(1).maybeSingle()
        : await q.limit(1).maybeSingle();
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

  return (
    <header className="sticky top-0 z-50 bg-white border-b border-gray-100 px-4 py-3">
      <div className="max-w-7xl mx-auto flex flex-col gap-3">
        {/* Top Section: Logo + Address and Cart */}
        <div className="flex items-center justify-between">
          <div 
            className="flex items-center gap-2 cursor-pointer hover:opacity-70 transition-opacity"
            onClick={() => navigate('/address')}
          >
            {store?.logo_url && (
              <img
                src={store.logo_url}
                alt={store.nome || 'Logo'}
                className="w-12 h-12 rounded-lg object-contain flex-shrink-0"
                referrerPolicy="no-referrer"
              />
            )}
            <MapPin className="w-4 h-4 text-red-600" />
            <span className="text-sm font-semibold text-gray-800 truncate max-w-[200px]">
              {store?.endereco || 'Carregando endereço...'}
            </span>
            <ChevronDown className="w-4 h-4 text-red-600" />
          </div>
          
          <div 
            className="relative cursor-pointer p-1 hover:bg-gray-50 rounded-full transition-colors"
            onClick={() => navigate('/cart')}
          >
            <ShoppingBag className="w-6 h-6 text-gray-700" />
            {itemCount > 0 && (
              <span className="absolute -top-0 -right-0 bg-red-600 text-white text-[10px] font-bold w-4 h-4 flex items-center justify-center rounded-full border-2 border-white">
                {itemCount}
              </span>
            )}
          </div>
        </div>

        {/* Search Bar */}
        <form 
          className="relative" 
          onSubmit={(e) => {
            e.preventDefault();
            (document.activeElement as HTMLElement).blur();
          }}
        >
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input
            type="search"
            enterKeyHint="search"
            placeholder="Buscar em RanGoFood"
            value={searchQuery}
            onChange={(e) => onSearchChange?.(e.target.value)}
            className="w-full bg-gray-100 border-none rounded-lg py-2 pl-10 pr-4 text-base focus:ring-2 focus:ring-red-500 transition-all outline-none"
          />
        </form>
      </div>
    </header>
  );
}