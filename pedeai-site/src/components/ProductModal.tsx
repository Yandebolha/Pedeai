import { useState, useEffect, useMemo } from 'react';
import { Produto, ComplementoGrupoComItens, Adicional, ComplementoItem } from '@/types/database';
import { Badge } from '@/components/ui/badge';
import { Plus, Minus, X, ShoppingBag, Check } from 'lucide-react';
import { SelectedComplemento, SelectedAdicional } from '@/store/useCartStore';
import { motion, AnimatePresence } from 'motion/react';
import { useQuery } from '@tanstack/react-query';
import { supabase } from '@/lib/supabase';

interface ProductModalProps {
  product: Produto | null;
  onClose: () => void;
  onAdd: (product: Produto, quantity: number, complementos: SelectedComplemento[], adicionais: SelectedAdicional[]) => void;
  /** Pre-populate selections when editing a cart item */
  initialComplementos?: Record<string, string[]>;
  initialAdicionais?: SelectedAdicional[];
  initialSabores?: string[];
  initialQuantity?: number;
  /** Changes the button label to "Salvar alterações" */
  editMode?: boolean;
}

const GROUP_ORDER = ['arroz', 'feijão', 'feijao', 'guarnição', 'guarnicao', 'guarnições', 'guarnicoes', 'salada', 'carne'];

export function ProductModal({
  product,
  onClose,
  onAdd,
  initialComplementos,
  initialAdicionais,
  initialSabores,
  initialQuantity,
  editMode = false,
}: ProductModalProps) {
  const [selectedComplementos, setSelectedComplementos] = useState<Record<string, string[]>>(initialComplementos ?? {});
  const [selectedAdicionais, setSelectedAdicionais] = useState<SelectedAdicional[]>(initialAdicionais ?? []);
  const [selectedSabores, setSelectedSabores] = useState<string[]>(initialSabores ?? []);
  const [modalQuantity, setModalQuantity] = useState(initialQuantity ?? 1);

  const { data: complementGrupos, isLoading: isLoadingGroups } = useQuery<ComplementoGrupoComItens[]>({
    queryKey: ['complemento_grupo', product?.id],
    queryFn: async () => {
      // 1. Busca grupos com mercadoria_id direto (modo atual)
      const { data: direct, error } = await supabase
        .from('complemento_grupo')
        .select('*, complemento!inner(*)')
        .eq('mercadoria_id', product!.id)
        .eq('ativo', true)
        .eq('complemento.ativo', true)
        .order('ordem');
      // fallback sem inner join se der erro
      const { data: directFallback } = error ? await supabase
        .from('complemento_grupo')
        .select('*, complemento(*)')
        .eq('mercadoria_id', product!.id)
        .eq('ativo', true)
        .order('ordem') : { data: null };

      const directData = (error ? directFallback : direct) ?? [];

      // 2. Fallback: grupos via link table mercadoria_complemento_grupo (sabores/legado)
      const { data: links } = await supabase
        .from('mercadoria_complemento_grupo')
        .select('grupo_id')
        .eq('mercadoria_id', product!.id);

      let linked: ComplementoGrupoComItens[] = [];
      if (links && links.length > 0) {
        const ids = links.map((l: { grupo_id: string }) => l.grupo_id).filter(Boolean);
        const seen = new Set((directData).map((g) => g.id));
        const missing = ids.filter((id: string) => !seen.has(id));
        if (missing.length > 0) {
          const { data: linkedGroups } = await supabase
            .from('complemento_grupo')
            .select('*, complemento(*)')
            .in('id', missing)
            .eq('ativo', true)
            .order('ordem');
          if (linkedGroups) linked = linkedGroups as ComplementoGrupoComItens[];
        }
      }

      return [...directData, ...linked] as ComplementoGrupoComItens[];
    },
    enabled: !!product,
  });

  const { data: adicionaisList } = useQuery<Adicional[]>({
    queryKey: ['adicional', product?.id],
    queryFn: async () => {
      const { data, error } = await supabase
        .from('adicional')
        .select('*')
        .eq('mercadoria_id', product!.id)
        .not('ativo', 'is', false)
        .order('ordem');
      if (error) throw error;
      return data as Adicional[];
    },
    enabled: !!product,
  });

  // Derive sabores a partir do complemento_grupo "Sabores*" já populado pelo sistema.
  // Isso evita depender de mercadoria.ativo=true para itens que são sabores/complementos.
  const saboresGrupo = complementGrupos?.find((g) => g.nome.toLowerCase().startsWith('sabores'));
  const saboresListFinal: ComplementoItem[] = ((saboresGrupo?.complemento as ComplementoItem[] | undefined) ?? [])
    .filter((c) => c.ativo !== false);

  const { data: saboresDisponiveis } = useQuery<Produto[]>({
    queryKey: ['sabores', product?.grupo_id],
    queryFn: async () => {
      const { data, error } = await supabase
        .from('mercadoria')
        .select('*')
        .eq('grupo_id', product!.grupo_id!)
        .eq('ativo', true)
        .neq('fracionado', true)
        .not('is_adicional', 'is', true)
        .neq('id', product!.id)
        .order('nome');
      if (error) throw error;
      return data as Produto[];
    },
    enabled: !!(product?.fracionado && product?.grupo_id),
  });

  // Reset state when product changes; apply initial values for edit mode
  useEffect(() => {
    if (product) {
      setSelectedComplementos(initialComplementos ?? {});
      setSelectedAdicionais(initialAdicionais ?? []);
      setSelectedSabores(initialSabores ?? []);
      setModalQuantity(initialQuantity ?? 1);
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [product?.id]);

  const toggleComplemento = (grupo: ComplementoGrupoComItens, itemId: string) => {
    setSelectedComplementos((prev) => {
      const current = prev[grupo.id] || [];
      if (current.includes(itemId)) return { ...prev, [grupo.id]: current.filter((id) => id !== itemId) };
      if (grupo.maximo === 1) return { ...prev, [grupo.id]: [itemId] };
      if (current.length >= grupo.maximo) return prev;
      return { ...prev, [grupo.id]: [...current, itemId] };
    });
  };

  const sortedGrupos = complementGrupos
    ? [...complementGrupos]
        .filter((g) => !g.nome.toLowerCase().startsWith('sabores'))
        .sort((a, b) => {
          const ai = GROUP_ORDER.findIndex((k) => a.nome.toLowerCase().startsWith(k));
          const bi = GROUP_ORDER.findIndex((k) => b.nome.toLowerCase().startsWith(k));
          return (ai === -1 ? 99 : ai) - (bi === -1 ? 99 : bi);
        })
    : [];

  const isFracionado = product?.fracionado === true;
  const qtdSabores = product?.qtd_sabores || 2;
  // Preço base do produto (antes de adicionar adicionais)
  const itemBasePrice = product ? (product.preco_promocional || product.preco_venda) : 0;

  // Modo de precificação dos sabores:
  // - preco_fixo=true (Açaí): preço final = preço base + soma dos sabores selecionados
  // - preco_fixo=false/null (Pizza): preço final = média dos sabores selecionados (half/half)
  const saboresModoSoma = isFracionado && product?.preco_fixo === true;

  const toggleSabor = (id: string) => {
    setSelectedSabores((prev) => {
      if (prev.includes(id)) return prev.filter((x) => x !== id);
      if (prev.length >= qtdSabores) return prev;
      return [...prev, id];
    });
  };

  // Todos os produtos fracionados: mínimo 1 sabor, máximo qtdSabores
  const minSabores = 1;

  const precoCalculado = useMemo(() => {
    if (!isFracionado) return null;
    // Preço fixo: o preço é sempre o preço base — sabores não somam nem mudam o valor
    if (saboresModoSoma) return itemBasePrice;
    // Pizza: média dos sabores selecionados (half/half) — calcula com quantos estiverem escolhidos
    if (saboresListFinal.length === 0 || selectedSabores.length === 0) return null;
    const somaPrecos = selectedSabores.reduce((acc, id) => {
      const item = saboresListFinal.find((p) => p.id === id);
      return acc + (item ? item.preco : 0);
    }, 0);
    return somaPrecos / selectedSabores.length;
  }, [isFracionado, selectedSabores, saboresListFinal, saboresModoSoma, itemBasePrice]);

  const canAdd = isFracionado
    ? selectedSabores.length >= minSabores
    : (!isLoadingGroups &&
        (!complementGrupos?.filter((g) => g.obrigatorio && g.nome.toLowerCase() !== 'geral').length ||
          complementGrupos!
            .filter((g) => g.obrigatorio && g.nome.toLowerCase() !== 'geral')
            .every((g) => (selectedComplementos[g.id]?.length || 0) >= Math.max(g.minimo, 1))));

  const adicionaisTotal = selectedAdicionais.reduce((acc, a) => acc + a.preco * a.quantity, 0);
  const finalPrice = isFracionado
    ? (precoCalculado ?? itemBasePrice) + adicionaisTotal
    : itemBasePrice + adicionaisTotal;

  const handleConfirm = () => {
    if (!product || !canAdd) return;

    if (isFracionado && precoCalculado !== null) {
      const saboresComp: SelectedComplemento[] = [{
        grupoId: 'sabores',
        grupoNome: 'Sabores',
        itemId: selectedSabores.join(','),
        itemNome: selectedSabores
          .map((id) => saboresListFinal.find((p) => p.id === id)?.nome || '')
          .join(', '),
      }];
      const produtoOverride: Produto = {
        ...product,
        preco_venda: precoCalculado,
        preco_promocional: null,
      };
      onAdd(produtoOverride, modalQuantity, saboresComp, selectedAdicionais.filter((a) => a.quantity > 0));
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
    onAdd(product, modalQuantity, complementosList, selectedAdicionais.filter((a) => a.quantity > 0));
  };

  return (
    <AnimatePresence>
      {product && (
        <>
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
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
                onClick={onClose}
                className="absolute top-4 right-4 bg-black/50 text-white p-2 rounded-full z-10 backdrop-blur-md"
              >
                <X className="w-5 h-5" />
              </button>

              <div className="w-full h-56 overflow-hidden">
                <img
                  src={product.imagem_url || 'https://via.placeholder.com/400'}
                  alt={product.nome}
                  className="w-full h-full object-cover"
                />
              </div>

              <div className="px-6 pt-5 pb-4 space-y-4">
                <div className="space-y-1">
                  <div className="flex items-center justify-between">
                    <h2 className="text-2xl font-black text-gray-900">{product.nome}</h2>
                    {product.destaque && (
                      <Badge className="bg-red-100 text-red-600 border-none">Destaque</Badge>
                    )}
                  </div>
                  <p className="text-gray-500 leading-relaxed">{product.descricao}</p>
                </div>

                {/* ── Seletor de Sabores (produto fracionado) ── */}
                {isFracionado && (
                  <div className="border-t border-gray-100 -mx-6">
                    <div className="flex items-center justify-between bg-gray-50 px-6 py-3">
                      <div>
                        <h3 className="font-bold text-gray-900 text-[15px] leading-tight">Sabores</h3>
                        <p className="text-xs text-gray-500 mt-0.5">
                          Escolha até {qtdSabores} {qtdSabores === 1 ? 'sabor' : 'sabores'}
                        </p>
                      </div>
                      <div className="flex items-center gap-2">
                        {selectedSabores.length > 0 && (
                          <span className="text-xs font-bold text-red-600 bg-red-50 px-2 py-0.5 rounded-full border border-red-100">
                            {selectedSabores.length}/{qtdSabores}
                          </span>
                        )}
                        <span className="text-[10px] bg-red-600 text-white px-2.5 py-1 rounded-full font-bold uppercase tracking-wide">
                          Mín. 1
                        </span>
                      </div>
                    </div>

                    <div className="divide-y divide-gray-100 bg-white">
                      {saboresListFinal.map((sabor) => {
                        const isSelected = selectedSabores.includes(sabor.id);
                        const isDisabled = !isSelected && selectedSabores.length >= qtdSabores;
                        return (
                          <button
                            key={sabor.id}
                            onClick={() => !isDisabled && toggleSabor(sabor.id)}
                            className={`w-full flex items-center gap-4 px-6 py-4 text-left transition-colors ${
                              isSelected ? 'bg-red-50' : isDisabled ? 'opacity-40 cursor-not-allowed' : 'hover:bg-gray-50 active:bg-gray-100'
                            }`}
                          >
                            <div
                              className={`w-5 h-5 flex-shrink-0 rounded-md border-2 flex items-center justify-center transition-all ${
                                isSelected ? 'bg-red-600 border-red-600' : 'border-gray-300 bg-white'
                              }`}
                            >
                              {isSelected && <Check className="w-3 h-3 text-white" strokeWidth={3} />}
                            </div>
                            {sabor.imagem_url ? (
                              <img
                                src={sabor.imagem_url}
                                alt={sabor.nome}
                                className="w-12 h-12 rounded-lg object-cover flex-shrink-0"
                              />
                            ) : null}
                            <div className="flex-1 min-w-0">
                              <p className="text-sm font-semibold text-gray-800 uppercase leading-tight tracking-wide">{sabor.nome}</p>
                            </div>
                            {/* Preço do sabor: só exibe quando preco_fixo=false (modo pizza/média) */}
                            {!saboresModoSoma && (
                              <span className="text-xs font-bold text-green-700 flex-shrink-0">
                                R$ {sabor.preco.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                              </span>
                            )}
                          </button>
                        );
                      })}
                      {saboresListFinal.length === 0 && (
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
                                    {item.imagem_url ? (
                                      <img
                                        src={item.imagem_url}
                                        alt={item.nome}
                                        className="w-12 h-12 rounded-lg object-cover flex-shrink-0"
                                      />
                                    ) : null}
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
                    <div className="flex items-center justify-between bg-gray-50 px-6 py-3">
                      <div>
                        <h3 className="font-bold text-gray-900 text-[15px] leading-tight">Adicional</h3>
                        <p className="text-xs text-gray-500 mt-0.5">
                          Escolha os adicionais desejados
                        </p>
                      </div>
                    </div>
                    <div className="divide-y divide-gray-100 bg-white">
                      {adicionaisList.map((adicional) => {
                        const selectedQty = selectedAdicionais.find((a) => a.id === adicional.id)?.quantity || 0;
                        const maxQtd = adicional.max_qtde ?? 1;
                        const atMax = maxQtd > 0 && selectedQty >= maxQtd;
                        return (
                          <div key={adicional.id} className="flex items-center gap-4 px-6 py-4">
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
                                  if (atMax) return;
                                  setSelectedAdicionais((prev) => {
                                    const existing = prev.find((a) => a.id === adicional.id);
                                    if (!existing)
                                      return [...prev, { id: adicional.id, nome: adicional.nome, preco: adicional.preco, quantity: 1 }];
                                    return prev.map((a) => (a.id === adicional.id ? { ...a, quantity: a.quantity + 1 } : a));
                                  });
                                }}
                                disabled={atMax}
                                className="w-7 h-7 flex items-center justify-center rounded-full border-2 border-red-600 text-red-600 hover:bg-red-50 transition-colors disabled:opacity-30 disabled:cursor-not-allowed"
                              >
                                <Plus className="w-3.5 h-3.5" />
                              </button>
                            </div>
            {adicional.imagem_url ? (
                              <img
                                src={adicional.imagem_url}
                                alt={adicional.nome}
                                className="w-12 h-12 rounded-lg object-cover flex-shrink-0"
                              />
                            ) : null}
                            <div className="flex-1 min-w-0">
                              <p className="text-sm font-semibold text-gray-800 uppercase tracking-wide leading-tight">{adicional.nome}</p>
                              <p className="text-xs text-green-700 font-bold mt-0.5">
                                + R$ {adicional.preco.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                                {maxQtd > 0 && <span className="text-gray-400 font-normal ml-1">(máx. {maxQtd})</span>}
                              </p>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  </div>
                )}

                {/* Preço */}
                <div className="pt-4 border-t border-gray-100">
                  {isFracionado ? (
                    <>
                      <span className="text-xs font-bold text-gray-400 uppercase tracking-widest">
                        {saboresModoSoma
                          ? 'Preço fixo'
                          : selectedSabores.length === qtdSabores
                            ? 'Preço calculado'
                            : `A partir de · selecione ${qtdSabores} sabores`}
                      </span>
                      <div className="flex items-baseline gap-2 mt-1">
                        <span className="text-2xl font-black text-green-700">
                          R$ {((precoCalculado ?? itemBasePrice) + adicionaisTotal).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                        </span>
                        {precoCalculado !== null && !saboresModoSoma && (
                          <span className="text-xs text-gray-400">
                            ({selectedSabores.length}/{qtdSabores} sabores · média{adicionaisTotal > 0 ? ` + R$ ${adicionaisTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2 })} adicionais` : ''})
                          </span>
                        )}
                        {saboresModoSoma && adicionaisTotal > 0 && (
                          <span className="text-xs text-gray-400">
                            + R$ {adicionaisTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2 })} adicionais
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
                        {product.preco_promocional && (
                          <span className="text-sm text-gray-400 line-through">
                            R$ {product.preco_venda.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                          </span>
                        )}
                      </div>
                    </>
                  )}
                </div>

                {/* Quantidade */}
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

                {/* Botão de confirmação */}
                <div className="pt-2">
                  <button
                    disabled={!canAdd}
                    onClick={handleConfirm}
                    className="w-full bg-red-600 text-white py-4 rounded-2xl font-bold flex items-center justify-center gap-3 hover:bg-red-700 transition-all shadow-lg shadow-red-200 disabled:opacity-50 disabled:cursor-not-allowed disabled:shadow-none"
                  >
                    <ShoppingBag className="w-5 h-5" />
                    <span>
                      {canAdd
                        ? editMode
                          ? `Salvar alterações · R$ ${(finalPrice * modalQuantity).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`
                          : `Adicionar · R$ ${(finalPrice * modalQuantity).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`
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
  );
}
