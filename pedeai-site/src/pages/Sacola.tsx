import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCartStore, CartItem, SelectedComplemento, SelectedAdicional } from '@/store/useCartStore';
import { ChevronLeft, Minus, Plus, Trash2, ShoppingBag, Loader2, X, CreditCard, Banknote, QrCode, Pencil } from 'lucide-react';
import { useQuery } from '@tanstack/react-query';
import { supabase, empresaCodigo, isEmpresaCodigoMissing } from '@/lib/supabase';
import { Loja, FormaPagamento, Produto } from '@/types/database';
import { motion, AnimatePresence } from 'motion/react';
import { ProductModal } from '@/components/ProductModal';

export default function Sacola() {
  const navigate = useNavigate();
  const { items, removeItem, updateQuantity, addItem, getTotal } = useCartStore();
  const subtotal = getTotal();
  const [showPaymentModal, setShowPaymentModal] = useState(false);
  const [selectedPayment, setSelectedPayment] = useState<FormaPagamento | null>(null);
  const [editingItem, setEditingItem] = useState<CartItem | null>(null);

  /** Reconstruct initialComplementos Record from CartItem.complementos */
  const getInitialComplementos = (item: CartItem): Record<string, string[]> => {
    const map: Record<string, string[]> = {};
    item.complementos.filter(c => c.grupoId !== 'sabores').forEach(c => {
      if (!map[c.grupoId]) map[c.grupoId] = [];
      map[c.grupoId].push(c.itemId);
    });
    return map;
  };

  /** Reconstruct initialSabores from CartItem.complementos */
  const getInitialSabores = (item: CartItem): string[] => {
    const saboresComp = item.complementos.find(c => c.grupoId === 'sabores');
    return saboresComp ? saboresComp.itemId.split(',').filter(Boolean) : [];
  };

  // Busca dados da loja para pegar a taxa de entrega real
  const { data: store, isLoading: isLoadingStore } = useQuery({
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

  // Busca as formas de pagamento do banco
  const { data: paymentMethods } = useQuery({
    queryKey: ['payment-methods', empresaCodigo],
    queryFn: async () => {
      const base = supabase.from('forma_pagamento').select('*').eq('ativo', true);
      const { data, error } = empresaCodigo
        ? await base.eq('empresa_codigo', empresaCodigo)
        : await base;
      if (error) {
        if (isEmpresaCodigoMissing(error)) {
          const { data: d2, error: e2 } = await supabase.from('forma_pagamento').select('*').eq('ativo', true);
          if (e2) throw e2;
          return d2 as FormaPagamento[];
        }
        throw error;
      }
      return data as FormaPagamento[];
    },
  });

  if (isLoadingStore) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-white">
        <div className="flex flex-col items-center gap-2">
          <Loader2 className="w-8 h-8 text-red-600 animate-spin" />
          <p className="text-gray-500 font-medium">Carregando sacola...</p>
        </div>
      </div>
    );
  }

  const deliveryFee = store?.taxa_entrega || 0;
  const total = subtotal + deliveryFee;

  const getPaymentIcon = (tipo: string) => {
    if (tipo === 'pix') return <QrCode className="w-5 h-5 text-green-600" />;
    if (tipo === 'dinheiro') return <Banknote className="w-5 h-5 text-green-600" />;
    return <CreditCard className="w-5 h-5 text-blue-600" />;
  };

  if (items.length === 0) {
    return (
      <div className="min-h-screen bg-white flex flex-col">
        <header className="p-4 border-b flex items-center gap-4">
          <button onClick={() => navigate(-1)} className="p-1">
            <ChevronLeft className="w-6 h-6" />
          </button>
          <h1 className="font-bold text-lg">Sacola</h1>
        </header>
        <div className="flex-1 flex flex-col items-center justify-center p-8 text-center">
          <div className="bg-gray-100 p-6 rounded-full mb-4 text-gray-400">
            <ShoppingBag className="w-12 h-12" />
          </div>
          <h2 className="text-xl font-bold text-gray-800">Sua sacola está vazia</h2>
          <p className="text-gray-500 mt-2">Adicione itens do cardápio para começar a pedir.</p>
          <button 
            onClick={() => navigate('/')}
            className="mt-6 bg-red-600 text-white px-8 py-3 rounded-xl font-bold hover:bg-red-700 transition-colors"
          >
            Ver cardápio
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 pb-32">
      <header className="bg-white p-4 border-b flex items-center gap-4 sticky top-0 z-10">
        <button onClick={() => navigate(-1)} className="p-1">
          <ChevronLeft className="w-6 h-6" />
        </button>
        <h1 className="font-bold text-lg">Sacola</h1>
      </header>

      <main className="p-4 max-w-2xl mx-auto space-y-4">
        <div className="bg-white rounded-xl shadow-sm overflow-hidden">
          <div className="p-4 border-b">
            <h2 className="text-sm font-bold text-gray-800 uppercase tracking-wider">Itens</h2>
          </div>
          <div className="divide-y divide-gray-100">
            {items.map((item) => (
              <div key={item.id} className="p-4 flex gap-4">
                <img 
                  src={item.product.imagem_url || 'https://via.placeholder.com/150'} 
                  className="w-16 h-16 rounded-lg object-cover flex-shrink-0"
                  alt={item.product.nome}
                />
                <div className="flex-1 flex flex-col justify-between min-w-0">
                  <div>
                    <h3 className="text-sm font-bold text-gray-900 truncate">{item.product.nome}</h3>
                    <p className="text-xs text-gray-500 line-clamp-1">{item.product.descricao}</p>
                    {item.complementos?.length > 0 && (
                      <div className="mt-1 space-y-0.5">
                        {item.complementos.map((c) => (
                          <p key={c.grupoId} className="text-[11px] text-gray-400 leading-tight">
                            {c.grupoNome}: <span className="text-gray-600 font-medium">{c.itemNome}</span>
                          </p>
                        ))}
                      </div>
                    )}
                    {item.adicionais?.filter((a) => a.quantity > 0).length > 0 && (
                      <div className="mt-1 space-y-0.5">
                        {item.adicionais.filter((a) => a.quantity > 0).map((a) => (
                          <p key={a.id} className="text-[11px] text-green-600 leading-tight">
                            +{a.quantity}x {a.nome} (+ R$ {(a.preco * a.quantity).toLocaleString('pt-BR', { minimumFractionDigits: 2 })})
                          </p>
                        ))}
                      </div>
                    )}
                  </div>
                  
                  <div className="flex items-center justify-between mt-2">
                    <div className="flex items-center gap-2">
                      <span className="text-sm font-bold text-red-600">
                        R$ {(
                          (item.product.preco_promocional || item.product.preco_venda) +
                          (item.adicionais?.reduce((acc, a) => acc + a.preco * a.quantity, 0) || 0)
                        ).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                      </span>
                      <button
                        onClick={() => setEditingItem(item)}
                        className="p-1 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-md transition-colors"
                        title="Editar item"
                      >
                        <Pencil className="w-3.5 h-3.5" />
                      </button>
                    </div>

                    <div className="flex items-center gap-3 bg-gray-50 rounded-lg p-1 border border-gray-100">
                      <button 
                        onClick={() => {
                          if (item.quantity > 1) {
                            updateQuantity(item.id, item.quantity - 1);
                          } else {
                            removeItem(item.id);
                          }
                        }}
                        className="p-1 text-red-600 hover:bg-red-50 rounded-md transition-colors"
                      >
                        {item.quantity === 1 ? <Trash2 className="w-4 h-4" /> : <Minus className="w-4 h-4" />}
                      </button>
                      <span className="text-sm font-bold w-4 text-center">{item.quantity}</span>
                      <button 
                        onClick={() => updateQuantity(item.id, item.quantity + 1)}
                        className="p-1 text-red-600 hover:bg-red-50 rounded-md transition-colors"
                      >
                        <Plus className="w-4 h-4" />
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm p-4 space-y-3">
          <div className="flex justify-between text-sm text-gray-600">
            <span>Subtotal</span>
            <span>R$ {subtotal.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</span>
          </div>
          <div className="flex justify-between text-sm text-gray-600">
            <span>Taxa de entrega</span>
            <span>R$ {deliveryFee.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</span>
          </div>
          <div className="flex justify-between font-bold text-lg pt-2 border-t border-gray-100 text-gray-900">
            <span>Total</span>
            <span>R$ {total.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</span>
          </div>
        </div>
      </main>

      <div className="fixed bottom-0 left-0 right-0 bg-white border-t p-4 pb-8 shadow-[0_-4px_10px_rgba(0,0,0,0.05)]">
        <div className="max-w-2xl mx-auto">
          <button 
            onClick={() => setShowPaymentModal(true)}
            className="w-full bg-red-600 text-white py-4 rounded-xl font-bold flex items-center justify-center gap-2 hover:bg-red-700 transition-all active:scale-[0.98]"
          >
            {selectedPayment ? `Pagar com ${selectedPayment.nome}` : 'Escolher forma de pagamento'}
          </button>
        </div>
      </div>

      {/* Modal de Edição de Item */}
      <ProductModal
        product={editingItem?.product ?? null}
        onClose={() => setEditingItem(null)}
        initialComplementos={editingItem ? getInitialComplementos(editingItem) : undefined}
        initialAdicionais={editingItem?.adicionais}
        initialSabores={editingItem ? getInitialSabores(editingItem) : undefined}
        initialQuantity={editingItem?.quantity}
        editMode
        onAdd={(p: Produto, q: number, c: SelectedComplemento[], a: SelectedAdicional[]) => {
          if (editingItem) removeItem(editingItem.id);
          addItem(p, q, c, a);
          setEditingItem(null);
        }}
      />

      {/* Modal de Formas de Pagamento */}
      <AnimatePresence>
        {showPaymentModal && (
          <>
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              onClick={() => setShowPaymentModal(false)}
              className="fixed inset-0 bg-black/60 z-[60] backdrop-blur-sm"
            />
            <motion.div
              initial={{ y: "100%" }}
              animate={{ y: 0 }}
              exit={{ y: "100%" }}
              transition={{ type: "spring", damping: 25, stiffness: 200 }}
              className="fixed bottom-0 left-0 right-0 bg-white rounded-t-[32px] z-[70] max-h-[80vh] overflow-y-auto"
            >
              <div className="p-6">
                <div className="flex items-center justify-between mb-6">
                  <h2 className="text-xl font-black text-gray-900">Forma de Pagamento</h2>
                  <button onClick={() => setShowPaymentModal(false)} className="p-2 bg-gray-100 rounded-full">
                    <X className="w-5 h-5 text-gray-500" />
                  </button>
                </div>

                <div className="space-y-3">
                  {paymentMethods?.map((method) => (
                    <button
                      key={method.id}
                      onClick={() => {
                        setSelectedPayment(method);
                        setShowPaymentModal(false);
                        // Redireciona para o checkout passando a forma de pagamento selecionada
                        navigate('/checkout', { state: { paymentMethod: method } });
                      }}
                      className={`w-full flex items-center justify-between p-4 rounded-xl border-2 transition-all ${
                        selectedPayment?.id === method.id 
                          ? 'border-red-600 bg-red-50' 
                          : 'border-gray-100 hover:border-gray-200'
                      }`}
                    >
                      <div className="flex items-center gap-3">
                        <div className="p-2 bg-white rounded-lg shadow-sm">
                          {getPaymentIcon(method.tipo)}
                        </div>
                        <span className="font-bold text-gray-800">{method.nome}</span>
                      </div>
                      <div className={`w-5 h-5 rounded-full border-2 flex items-center justify-center ${
                        selectedPayment?.id === method.id ? 'border-red-600' : 'border-gray-300'
                      }`}>
                        {selectedPayment?.id === method.id && (
                          <div className="w-2.5 h-2.5 bg-red-600 rounded-full" />
                        )}
                      </div>
                    </button>
                  ))}
                </div>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  );
}