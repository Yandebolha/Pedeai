import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { supabase, empresaCodigo, isEmpresaCodigoMissing } from '@/lib/supabase';
import { Loja } from '@/types/database';
import { ChevronLeft, MapPin, Phone, Clock, Info } from 'lucide-react';

export default function EnderecoLoja() {
  const navigate = useNavigate();

  const { data: store, isLoading } = useQuery({
    queryKey: ['store'],
    queryFn: async () => {
      let q = supabase.from('loja').select('*');
      if (empresaCodigo) q = q.eq('empresa_codigo', empresaCodigo);
      const { data, error } = await q.limit(1).maybeSingle();
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

  if (isLoading) {
    return <div className="min-h-screen flex items-center justify-center">Carregando informações...</div>;
  }

  // Gerando a URL do mapa baseada no endereço cadastrado
  const mapEmbedUrl = `https://maps.google.com/maps?q=${encodeURIComponent(store?.endereco || '')}&t=&z=15&ie=UTF8&iwloc=&output=embed`;

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header Fixo */}
      <header className="bg-white p-4 border-b flex items-center gap-4 sticky top-0 z-10">
        <button onClick={() => navigate(-1)} className="p-1 hover:bg-gray-100 rounded-full transition-colors">
          <ChevronLeft className="w-6 h-6 text-gray-700" />
        </button>
        <h1 className="font-bold text-lg text-gray-800">Sobre a Loja</h1>
      </header>

      <main className="max-w-2xl mx-auto pb-10">
        {/* Banner e Logo */}
        <div className="relative h-48 w-full bg-gray-200">
          <img 
            src={store?.banner_url || 'https://picsum.photos/seed/food/800/400'} 
            className="w-full h-full object-cover"
            alt="Banner da Loja"
          />
          <div className="absolute -bottom-10 left-6">
            <div className="w-24 h-24 rounded-2xl border-4 border-white overflow-hidden shadow-lg bg-white">
              <img 
                src={store?.logo_url || 'https://picsum.photos/seed/logo/200/200'} 
                className="w-full h-full object-cover"
                alt="Logo"
              />
            </div>
          </div>
        </div>

        <div className="mt-14 px-6 space-y-6">
          {/* Nome e Info Básica */}
          <div>
            <h2 className="text-2xl font-black text-gray-900">{store?.nome}</h2>
            <div className="flex items-center gap-2 mt-2 text-gray-600">
              <Clock className="w-4 h-4 text-red-600" />
              <span className="text-sm font-medium">Aberto agora • {store?.tempo_estimado}</span>
            </div>
          </div>

          {/* Cards de Informação */}
          <div className="grid gap-4">
            <div className="bg-white p-4 rounded-xl shadow-sm border border-gray-100 flex gap-3">
              <MapPin className="w-5 h-5 text-red-600 shrink-0" />
              <div>
                <p className="text-xs font-bold uppercase text-gray-400">Endereço</p>
                <p className="text-sm text-gray-700 font-medium">{store?.endereco}</p>
              </div>
            </div>

            {store?.telefone && (
              <div className="bg-white p-4 rounded-xl shadow-sm border border-gray-100 flex gap-3">
                <Phone className="w-5 h-5 text-red-600 shrink-0" />
                <div>
                  <p className="text-xs font-bold uppercase text-gray-400">Telefone</p>
                  <p className="text-sm text-gray-700 font-medium">{store?.telefone}</p>
                </div>
              </div>
            )}
          </div>

          {/* Visualização do Mapa */}
          <div className="space-y-3">
            <h3 className="font-bold text-gray-800 flex items-center gap-2">
              <Info className="w-5 h-5 text-red-600" />
              Localização no Mapa
            </h3>
            <div className="w-full h-64 rounded-2xl overflow-hidden shadow-inner border border-gray-200">
              <iframe
                width="100%"
                height="100%"
                frameBorder="0"
                scrolling="no"
                marginHeight={0}
                marginWidth={0}
                src={mapEmbedUrl}
                title="Localização da Loja"
              />
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}