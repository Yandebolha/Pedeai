import { useState, useEffect, useMemo } from 'react';
import { Produto, ComplementoGrupoComItens, Adicional, ComplementoItem } from '@/types/database';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Plus, Minus, X, ShoppingBag, Check } from 'lucide-react';
import { useCartStore, SelectedComplemento, SelectedAdicional } from '@/store/useCartStore';
import { motion, AnimatePresence } from 'motion/react';
import { useQuery } from '@tanstack/react-query';
import { supabase } from '@/lib/supabase';

interface ProductFeedProps {
  products: Produto[];
  categoryName: string;
}

export function ProductFeed({ products, categoryName }: ProductFeedProps) {
  const addItem = useCartStore((state) => state.addItem);
  const [selectedProduct, setSelectedProduct] = useState<Produto | null>(null);
  const [selectedComplementos, setSelectedComplementos] = useState<Record<string, string[]>>({});
  const [selectedAdicionais, setSelectedAdicionais] = useState<SelectedAdicional[]>([]);
  const [selectedSabores, setSelectedSabores] = useState<string[]>([]);
  const [modalQuantity, setModalQuantity] = useState(1);

  const { data: complementGrupos } = useQuery<ComplementoGrupoComItens[]>({
    queryKey: ['complemento_grupo', selectedProduct?.id],
    queryFn: async () => {
      const { data, error } = await supabase
        .from('complemento_grupo')
        .select('*, complemento(*)')
        .eq('mercadoria_id', selectedProduct!.id)
        .eq('ativo', true)
        .order('ordem');
      if (error) throw error;
      return data as ComplementoGrupoComItens[];
    },
    enabled: !!selectedProduct,
  });

  const { data: adicionaisList } = useQuery<Adicional[]>({
    queryKey: ['adicional', selectedProduct?.id],
    queryFn: async () => {
      const { data, error } = await supabase
        .from('adicional')
        .select('*')
        .eq('mercadoria_id', selectedProduct!.id)
        .eq('ativo', true)
        .order('ordem');
      if (error) throw error;
      return data as Adicional[];
    },
    enabled: !!selectedProduct,
  });

  // Busca os sabores disponíveis para produtos fracionados (outros produtos da mesma categoria)
  // Produtos sabor são sincronizados com ativo=true pelo backend quando o grupo tem fracionado
  const { data: saboresDisponiveis } = useQuery<Produto[]>({
    queryKey: ['sabores', selectedProduct?.grupo_id],
    queryFn: async () => {
      const { data, error } = await supabase
        .from('mercadoria')
        .select('*')
        .eq('grupo_id', selectedProduct!.grupo_id!)
        .eq('ativo', true)
        .neq('fracionado', true)
        .neq('id', selectedProduct!.id)
        .order('nome');
      if (error) throw error;
      return data as Produto[];
    },
    enabled: !!(selectedProduct?.fracionado && selectedProduct?.grupo_id),
  });

  useEffect(() => {
    if (selectedProduct) {
      setSelectedComplementos({});
      setSelectedAdicionais([]);
      setSelectedSabores([]);
      setModalQuantity(1);
    }
  }, [selectedProduct?.id]);

  const toggleComplemento = (grupo: ComplementoGrupoComItens, itemId: string) => {
    setSelectedComplementos((prev) => {
      const current = prev[grupo.id] || [];
      if (current.includes(itemId)) return { ...prev, [grupo.id]: current.filter((id) => id !== itemId) };
      if (grupo.maximo === 1) return { ...prev, [grupo.id]: [itemId] };
      if (current.length >= grupo.maximo) return prev;
      return { ...prev, [grupo.id]: [...current, itemId] };
    });
  };

  const GROUP_ORDER = ['arroz', 'feijão', 'feijao', 'guarnição', 'guarnicao', 'guarnições', 'guarnicoes', 'salada', 'carne'];
  const sortedGrupos = complementGrupos
    ? [...complementGrupos].sort((a, b) => {
        const ai = GROUP_ORDER.findIndex((k) => a.nome.toLowerCase().startsWith(k));
        const bi = GROUP_ORDER.findIndex((k) => b.nome.toLowerCase().startsWith(k));
        const av = ai === -1 ? 99 : ai;
        const bv = bi === -1 ? 99 : bi;
        return av - bv;
      })
    : [];

  // ── Fracionado (múltiplos sabores) ───────────────────────────────────────
  const isFracionado = selectedProduct?.fracionado === true;
  const qtdSabores = selectedProduct?.qtd_sabores || 2;

  const toggleSabor = (id: string) => {
    setSelectedSabores((prev) => {
      if (prev.includes(id)) return prev.filter((x) => x !== id);
      if (prev.length >= qtdSabores) return prev; // limite atingido
      return [...prev, id];
    });
  };

  // Preço calculado = média dos sabores selecionados
  const precoCalculado = useMemo(() => {
    if (!isFracionado || !saboresDisponiveis || selectedSabores.length !== qtdSabores) return null;
    const soma = selectedSabores.reduce((acc, id) => {
      const prod = saboresDisponiveis.find((p) => p.id === id);
      return acc + (prod ? (prod.preco_promocional ?? prod.preco_venda) : 0);
    }, 0);
    return soma / qtdSabores;
  }, [isFracionado, selectedSabores, qtdSabores, saboresDisponiveis]);

  const canAdd = isFracionado
    ? selectedSabores.length === qtdSabores
    : (!complementGrupos ||
        complementGrupos
          .filter((g) => g.obrigatorio && g.nome.toLowerCase() !== 'geral')
          .every((g) => (selectedComplementos[g.id]?.length || 0) >= g.minimo));

  const adicionaisTotal = selectedAdicionais.reduce((acc, a) => acc + a.preco * a.quantity, 0);
  const itemBasePrice = selectedProduct ? (selectedProduct.preco_promocional || selectedProduct.preco_venda) : 0;
  const finalPrice = isFracionado ? (precoCalculado ?? itemBasePrice) : (itemBasePrice + adicionaisTotal);

  return (
    <div className="px-4 py-6">
      <h2 className="text-lg font-bold text-gray-800 mb-4">{categoryName}</h2>
      
      {products.length === 0 ? (
        <div className="flex flex-col items-center justify-center py-12 text-center bg-gray-50 rounded-2xl border-2 border-dashed border-gray-200">
          <div className="bg-white p-4 rounded-full shadow-sm mb-4">
            <ShoppingBag className="w-8 h-8 text-gray-300" />
          </div>
          <p className="text-gray-500 font-medium">Nenhum produto encontrado</p>
          <p className="text-xs text-gray-400 mt-1">Tente buscar por outro nome ou categoria</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-4">
        {products.map((product) => (
          <motion.div
            key={product.id}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.3 }}
            onClick={() => setSelectedProduct(product)}
          >
            <Card className="flex p-3 gap-3 overflow-hidden hover:shadow-md transition-all active:scale-[0.98] cursor-pointer relative">
              <div className="flex-1 flex flex-col justify-between">
                <div>
                  <h3 className="text-sm font-semibold text-gray-900 line-clamp-1">
                    {product.nome}
                  </h3>
                  <p className="text-xs text-gray-500 line-clamp-2 mt-1">
                    {product.descricao}
                  </p>
                </div>
                
                <div className="flex items-center gap-2 mt-2">
                  <span className="text-sm font-bold text-green-700">
                    R$ {(product.preco_promocional || product.preco_venda).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                  </span>
                  {product.preco_promocional && (
                    <span className="text-[10px] text-gray-400 line-through">
                      R$ {product.preco_venda.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                    </span>
                  )}
                  {product.destaque && (
                    <Badge className="bg-red-100 text-red-600 hover:bg-red-100 border-none text-[10px] px-1.5 py-0">
                      Destaque
                    </Badge>
                  )}
                </div>
              </div>

              <div className="w-24 h-24 rounded-lg overflow-hidden flex-shrink-0 relative">
                <img
                  src={product.imagem_url || 'https://via.placeholder.com/150'}
                  alt={product.nome}
                  className="w-full h-full object-cover"
                  referrerPolicy="no-referrer"
                />
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    if (product.fracionado) {
                      setSelectedProduct(product); // abre o modal para escolher sabores
                    } else {
                      addItem(product, 1);
                    }
                  }}
                  className="absolute bottom-1 right-1 bg-white rounded-full p-1 shadow-lg border border-gray-100 text-red-600 hover:bg-red-50 transition-colors"
                >
                  <Plus className="w-4 h-4" />
                </button>
              </div>
            </Card>
          </motion.div>
        ))}
      </div>
      )}

      {/* Modal de Detalhes do Produto */}
      <AnimatePresence>
        {selectedProduct && (
          <>
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              onClick={() => setSelectedProduct(null)}
              className="fixed inset-0 bg-black/60 z-[60] backdrop-blur-sm"
            />
            <motion.div
              initial={{ y: "100%" }}
              animate={{ y: 0 }}
              exit={{ y: "100%" }}
              transition={{ type: "spring", damping: 25, stiffness: 200 }}
              className="fixed bottom-0 left-0 right-0 bg-white rounded-t-[32px] z-[70] max-h-[90vh] overflow-y-auto"
            >
              <div className="relative">
                <button
                  onClick={() => setSelectedProduct(null)}
                  className="absolute top-4 right-4 bg-black/50 text-white p-2 rounded-full z-10 backdrop-blur-md"
                >
                  <X className="w-5 h-5" />
                </button>
                
                <div className="w-full h-56 overflow-hidden">
                  <img
                    src={selectedProduct.imagem_url || 'https://via.placeholder.com/400'}
                    alt={selectedProduct.nome}
                    className="w-full h-full object-cover"
                  />
                </div>

                <div className="px-6 pt-5 pb-4 space-y-4">
                  <div className="space-y-1">
                    <div className="flex items-center justify-between">
                      <h2 className="text-2xl font-black text-gray-900">{selectedProduct.nome}</h2>
                      {selectedProduct.destaque && (
                        <Badge className="bg-red-100 text-red-600 border-none">Destaque</Badge>
                      )}
                    </div>
                    <p className="text-gray-500 leading-relaxed">
                      {selectedProduct.descricao}
                    </p>
                  </div>

                  {/* ── Seletor de Sabores (produto fracionado) ── */}
                  {isFracionado && (
                    <div className="border-t border-gray-100 -mx-6">
                      {/* Cabeçalho */}
                      <div className="flex items-center justify-between bg-gray-50 px-6 py-3">
                        <div>
                          <h3 className="font-bold text-gray-900 text-[15px] leading-tight">Sabores</h3>
                          <p className="text-xs text-gray-500 mt-0.5">
                            Escolha {qtdSabores} {qtdSabores === 1 ? 'sabor' : 'sabores'}
                          </p>
                        </div>
                        <div className="flex items-center gap-2">
                          {selectedSabores.length > 0 && (
                            <span className="text-xs font-bold text-red-600 bg-red-50 px-2 py-0.5 rounded-full border border-red-100">
                              {selectedSabores.length}/{qtdSabores}
                            </span>
                          )}
                          <span className="text-[10px] bg-red-600 text-white px-2.5 py-1 rounded-full font-bold uppercase tracking-wide">
                            Obrigatório
                          </span>
                        </div>
                      </div>

                      {/* Lista de sabores */}
                      <div className="divide-y divide-gray-100 bg-white">
                        {(saboresDisponiveis || []).map((sabor) => {
                          const isSelected = selectedSabores.includes(sabor.id);
                          const isDisabled = !isSelected && selectedSabores.length >= qtdSabores;
                          const precoPorSabor = sabor.preco_promocional ?? sabor.preco_venda;
                          return (
                            <button
                              key={sabor.id}
                              onClick={() => !isDisabled && toggleSabor(sabor.id)}
                              className={`w-full flex items-center gap-4 px-6 py-4 text-left transition-colors ${
                                isSelected ? 'bg-red-50' : isDisabled ? 'opacity-40 cursor-not-allowed' : 'hover:bg-gray-50 active:bg-gray-100'
                              }`}
                            >
                              {/* Checkbox */}
                              <div
                                className={`w-5 h-5 flex-shrink-0 rounded-md border-2 flex items-center justify-center transition-all ${
                                  isSelected ? 'bg-red-600 border-red-600' : 'border-gray-300 bg-white'
                                }`}
                              >
                                {isSelected && <Check className="w-3 h-3 text-white" strokeWidth={3} />}
                              </div>

                              {/* Imagem miniatura */}
                              {sabor.imagem_url && (
                                <div className="w-10 h-10 rounded-lg overflow-hidden flex-shrink-0">
                                  <img
                                    src={sabor.imagem_url}
                                    alt={sabor.nome}
                                    className="w-full h-full object-cover"
                                    referrerPolicy="no-referrer"
                                  />
                                </div>
                              )}

                              {/* Nome e preço */}
                              <div className="flex-1 min-w-0">
                                <p className="text-sm font-semibold text-gray-800 uppercase leading-tight tracking-wide">
                                  {sabor.nome}
                                </p>
                                {sabor.descricao && (
                                  <p className="text-xs text-gray-400 mt-0.5 line-clamp-1">{sabor.descricao}</p>
                                )}
                              </div>

                              {/* Preço individual */}
                              <span className="text-xs font-bold text-green-700 flex-shrink-0">
                                R$ {precoPorSabor.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                              </span>
                            </button>
                          );
                        })}

                        {(!saboresDisponiveis || saboresDisponiveis.length === 0) && (
                          <div className="px-6 py-8 text-center text-gray-400 text-sm">
                            Nenhum sabor disponível nesta categoria.
                          </div>
                        )}
                      </div>
                    </div>
                  )}

                  {/* Grupos de Complementos */}
                  {sortedGrupos && sortedGrupos.length > 0 && (
                    <div className="space-y-0 pt-2 border-t border-gray-100 -mx-6">
                      {sortedGrupos.map((grupo) => {
                        const selectedCount = selectedComplementos[grupo.id]?.length || 0;
                        const isMulti = grupo.maximo > 1;
                        const subtitle = grupo.minimo === grupo.maximo
                          ? `Escolha ${grupo.maximo === 1 ? '1 item' : `${grupo.maximo} itens`}`
                          : grupo.minimo <= 1
                          ? `Escolha no máximo ${grupo.maximo} ${grupo.maximo === 1 ? 'item' : 'itens'}`
                          : `Escolha entre ${grupo.minimo} e ${grupo.maximo} itens`;
                        return (
                          <div key={grupo.id} className="border-b border-gray-100 last:border-b-0">
                            {/* Cabeçalho do grupo */}
                            <div className="flex items-center justify-between bg-gray-50 px-6 py-3">
                              <div>
                                <h3 className="font-bold text-gray-900 text-[15px] leading-tight">{grupo.nome}</h3>
                                <p className="text-xs text-gray-500 mt-0.5">{subtitle}</p>
                              </div>
                              <div className="flex items-center gap-2">
                                {selectedCount > 0 && (
                                  <span className="text-xs font-bold text-red-600 bg-red-50 px-2 py-0.5 rounded-full border border-red-100">
                                    {selectedCount}/{grupo.maximo}
                                  </span>
                                )}
                                {grupo.obrigatorio && (
                                  <span className="text-[10px] bg-red-600 text-white px-2.5 py-1 rounded-full font-bold uppercase tracking-wide">
                                    Obrigatório
                                  </span>
                                )}
                              </div>
                            </div>

                            {/* Itens */}
                            <div className="divide-y divide-gray-100 bg-white">
                              {(grupo.complemento || [])
                                .filter((c) => c.ativo)
                                .sort((a, b) => a.ordem - b.ordem)
                                .map((item) => {
                                  const isSelected = (selectedComplementos[grupo.id] || []).includes(item.id);
                                  const isDisabled = !isSelected && selectedCount >= grupo.maximo;
                                  return (
                                    <button
                                      key={item.id}
                                      onClick={() => !isDisabled && toggleComplemento(grupo, item.id)}
                                      className={`w-full flex items-center gap-4 px-6 py-4 text-left transition-colors ${
                                        isSelected ? 'bg-red-50' : isDisabled ? 'opacity-40 cursor-not-allowed' : 'hover:bg-gray-50 active:bg-gray-100'
                                      }`}
                                    >
                                      {/* Checkbox / Radio — lado ESQUERDO */}
                                      {isMulti ? (
                                        <div
                                          className={`w-5 h-5 flex-shrink-0 rounded-md border-2 flex items-center justify-center transition-all ${
                                            isSelected ? 'bg-red-600 border-red-600' : 'border-gray-300 bg-white'
                                          }`}
                                        >
                                          {isSelected && <Check className="w-3 h-3 text-white" strokeWidth={3} />}
                                        </div>
                                      ) : (
                                        <div
                                          className={`w-5 h-5 flex-shrink-0 rounded-full border-2 flex items-center justify-center transition-all ${
                                            isSelected ? 'border-red-600' : 'border-gray-300'
                                          }`}
                                        >
                                          {isSelected && <div className="w-2.5 h-2.5 bg-red-600 rounded-full" />}
                                        </div>
                                      )}

                                      {/* Nome e preço */}
                                      <div className="flex-1 min-w-0">
                                        <p className="text-sm font-semibold text-gray-800 uppercase leading-tight tracking-wide">{item.nome}</p>
                                        {item.preco > 0 && (
                                          <p className="text-xs text-green-700 font-bold mt-0.5">
                                            + R$ {item.preco.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                                          </p>
                                        )}
                                      </div>
                                    </button>
                                  );
                                })}
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  )}

                  {/* Adicionais */}
                  {adicionaisList && adicionaisList.length > 0 && (
                    <div className="border-t border-gray-100 -mx-6">
                      {/* Cabeçalho */}
                      <div className="flex items-center justify-between bg-gray-50 px-6 py-3">
                        <div>
                          <h3 className="font-bold text-gray-900 text-[15px] leading-tight">Adicional</h3>
                          <p className="text-xs text-gray-500 mt-0.5">
                            Escolha no máximo {adicionaisList.length} {adicionaisList.length === 1 ? 'item' : 'itens'}
                          </p>
                        </div>
                      </div>
                      <div className="divide-y divide-gray-100 bg-white">
                        {adicionaisList.map((adicional) => {
                          const selectedQty = selectedAdicionais.find((a) => a.id === adicional.id)?.quantity || 0;
                          return (
                            <div key={adicional.id} className="flex items-center gap-4 px-6 py-4">
                              {/* Controles — lado ESQUERDO */}
                              <div className="flex items-center gap-2 flex-shrink-0">
                                <button
                                  onClick={() => {
                                    setSelectedAdicionais((prev) => {
                                      const existing = prev.find((a) => a.id === adicional.id);
                                      if (!existing || existing.quantity === 0) return prev;
                                      if (existing.quantity === 1) return prev.filter((a) => a.id !== adicional.id);
                                      return prev.map((a) => (a.id === adicional.id ? { ...a, quantity: a.quantity - 1 } : a));
                                    });
                                  }}
                                  disabled={selectedQty === 0}
                                  className="w-7 h-7 flex items-center justify-center rounded-full border-2 border-gray-300 text-gray-500 hover:border-red-600 hover:text-red-600 transition-colors disabled:opacity-30"
                                >
                                  <Minus className="w-3.5 h-3.5" />
                                </button>
                                <span className="text-sm font-bold w-5 text-center text-gray-700">{selectedQty}</span>
                                <button
                                  onClick={() => {
                                    setSelectedAdicionais((prev) => {
                                      const existing = prev.find((a) => a.id === adicional.id);
                                      if (!existing)
                                        return [...prev, { id: adicional.id, nome: adicional.nome, preco: adicional.preco, quantity: 1 }];
                                      return prev.map((a) => (a.id === adicional.id ? { ...a, quantity: a.quantity + 1 } : a));
                                    });
                                  }}
                                  className="w-7 h-7 flex items-center justify-center rounded-full border-2 border-red-600 text-red-600 hover:bg-red-50 transition-colors"
                                >
                                  <Plus className="w-3.5 h-3.5" />
                                </button>
                              </div>

                              {/* Nome e preço */}
                              <div className="flex-1 min-w-0">
                                <p className="text-sm font-semibold text-gray-800 uppercase tracking-wide leading-tight">{adicional.nome}</p>
                                <p className="text-xs text-green-700 font-bold mt-0.5">
                                  + R$ {adicional.preco.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                                </p>
                              </div>
                            </div>
                          );
                        })}
                      </div>
                    </div>
                  )}

                  <div className="pt-4 border-t border-gray-100">
                    {isFracionado ? (
                      <>
                        <span className="text-xs font-bold text-gray-400 uppercase tracking-widest">
                          {selectedSabores.length === qtdSabores ? 'Preço calculado' : `A partir de · selecione ${qtdSabores} sabores`}
                        </span>
                        <div className="flex items-baseline gap-2 mt-1">
                          <span className="text-2xl font-black text-green-700">
                            R$ {(precoCalculado ?? itemBasePrice).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                          </span>
                          {precoCalculado !== null && precoCalculado !== itemBasePrice && (
                            <span className="text-xs text-gray-400">
                              ({selectedSabores.length}/{qtdSabores} sabores · média)
                            </span>
                          )}
                        </div>
                      </>
                    ) : (
                      <>
                        <span className="text-xs font-bold text-gray-400 uppercase tracking-widest">Preço base</span>
                        <div className="flex items-baseline gap-2 mt-1">
                          <span className="text-2xl font-black text-green-700">
                            R$ {itemBasePrice.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                          </span>
                          {selectedProduct.preco_promocional && (
                            <span className="text-sm text-gray-400 line-through">
                              R$ {selectedProduct.preco_venda.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                            </span>
                          )}
                        </div>
                      </>
                    )}
                  </div>

                  {/* Seletor de Quantidade */}
                  <div className="flex items-center justify-between p-3 bg-gray-50 rounded-xl border border-gray-100">
                    <span className="font-bold text-gray-700 text-sm">Quantidade</span>
                    <div className="flex items-center gap-3 bg-white rounded-lg p-1 border border-gray-200">
                      <button
                        onClick={() => setModalQuantity((q) => Math.max(1, q - 1))}
                        disabled={modalQuantity <= 1}
                        className="p-1.5 text-red-600 hover:bg-red-50 rounded-md transition-colors disabled:opacity-30"
                      >
                        <Minus className="w-4 h-4" />
                      </button>
                      <span className="text-sm font-bold w-6 text-center">{modalQuantity}</span>
                      <button
                        onClick={() => setModalQuantity((q) => q + 1)}
                        className="p-1.5 text-red-600 hover:bg-red-50 rounded-md transition-colors"
                      >
                        <Plus className="w-4 h-4" />
                      </button>
                    </div>
                  </div>

                  <div className="pt-2">
                    <button
                      disabled={!canAdd}
                      onClick={() => {
                        if (isFracionado && precoCalculado !== null && saboresDisponiveis) {
                          // Produto fracionado: adiciona com sabores como complementos e preço calculado
                          const saboresComp: SelectedComplemento[] = [{
                            grupoId: 'sabores',
                            grupoNome: '🍕 Sabores',
                            itemId: selectedSabores.join(','),
                            itemNome: selectedSabores
                              .map((id) => saboresDisponiveis.find((p) => p.id === id)?.nome || '')
                              .join(', '),
                          }];
                          // Override do preço para o valor calculado (média dos sabores)
                          const produtoOverride: Produto = {
                            ...selectedProduct!,
                            preco_venda: precoCalculado,
                            preco_promocional: null,
                          };
                          addItem(produtoOverride, modalQuantity, saboresComp, []);
                          setSelectedProduct(null);
                          return;
                        }

                        const complementosList: SelectedComplemento[] = (Object.entries(selectedComplementos) as [string, string[]][]).flatMap(
                          ([grupoId, itemIds]) => {
                            const grupo = complementGrupos?.find((g) => g.id === grupoId);
                            const itens = grupo?.complemento as ComplementoItem[] | undefined;
                            return itemIds.map((itemId) => ({
                              grupoId,
                              grupoNome: grupo?.nome || '',
                              itemId,
                              itemNome: itens?.find((c) => c.id === itemId)?.nome || '',
                            }));
                          }
                        );
                        addItem(
                          selectedProduct,
                          modalQuantity,
                          complementosList,
                          selectedAdicionais.filter((a) => a.quantity > 0)
                        );
                        setSelectedProduct(null);
                      }}
                      className="w-full bg-red-600 text-white py-4 rounded-2xl font-bold flex items-center justify-center gap-3 hover:bg-red-700 transition-all shadow-lg shadow-red-200 disabled:opacity-50 disabled:cursor-not-allowed disabled:shadow-none"
                    >
                      <ShoppingBag className="w-5 h-5" />
                      <span>
                        {canAdd
                          ? `Adicionar · R$ ${(finalPrice * modalQuantity).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`
                          : isFracionado
                          ? `Escolha ${qtdSabores - selectedSabores.length} sabor${qtdSabores - selectedSabores.length !== 1 ? 'es' : ''} ainda`
                          : 'Escolha as opções obrigatórias'}
                      </span>
                    </button>
                  </div>
                </div>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  );
}
