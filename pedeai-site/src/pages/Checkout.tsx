import { useState, useMemo, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { ChevronLeft, MapPin, Phone, Loader2, Save, ShoppingCart, Check, X, CreditCard, Banknote, QrCode, Tag, User, FileText } from 'lucide-react';
import { supabase, empresaCodigo, isEmpresaCodigoMissing } from '@/lib/supabase';
import { EnderecoSalvo, Loja } from '@/types/database';
import { useCartStore } from '@/store/useCartStore';
import { useQuery } from '@tanstack/react-query';
import { motion, AnimatePresence } from 'motion/react';

export default function Checkout() {
  const navigate = useNavigate();
  const location = useLocation();
  const paymentMethod = location.state?.paymentMethod;
  
  const cartItems = useCartStore((state) => state.items);
  const cartTotal = useCartStore((state) => state.getTotal());
  const clearCart = useCartStore((state) => state.clearCart);

  const [whatsapp, setWhatsapp] = useState('');
  const [troco, setTroco] = useState('');
  const [couponCode, setCouponCode] = useState('');
  const [appliedCoupon, setAppliedCoupon] = useState<{ id: string, valor: number, tipo: string, codigo: string, produto_nome?: string | null } | null>(null);
  const [loading, setLoading] = useState(false);
  const [addressesFound, setAddressesFound] = useState<EnderecoSalvo[]>([]);
  const [selectedAddress, setSelectedAddress] = useState<EnderecoSalvo | null>(null);
  const [showAddressForm, setShowAddressForm] = useState(false);
  const [clientFound, setClientFound] = useState<any | null>(null);
  const [showClientForm, setShowClientForm] = useState(false);
  const [clientFormData, setClientFormData] = useState({
    nome: '',
    cpf_cnpj: ''
  });
  const [showSummary, setShowSummary] = useState(false);
  const [showSuccess, setShowSuccess] = useState(false);
  const [bairroTaxa, setBairroTaxa] = useState<number | null>(null); // taxa pelo bairro cadastrado

  // Busca dados da loja para taxa de entrega e telefone
  const { data: store } = useQuery({
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

  // Estado para novo endereço
  const [formData, setFormData] = useState({
    cep: '',
    endereco: '',
    numero: '',
    bairro: '',
    complemento: '',
    cidade: '',
    uf: ''
  });

  // Máscara CEP
  const formatCep = (value: string) => {
    const nums = value.replace(/\D/g, '').slice(0, 8);
    if (nums.length <= 5) return nums;
    return `${nums.slice(0, 5)}-${nums.slice(5)}`;
  };

  // Sempre que o endereço selecionado mudar, busca a taxa correspondente
  useEffect(() => {
    if (selectedAddress?.bairro) {
      buscarTaxaPorBairro(selectedAddress.bairro, selectedAddress.cidade ?? undefined);
    } else {
      setBairroTaxa(null);
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedAddress?.id]);

  // Busca taxa de entrega pelo bairro e cidade na tabela taxa_entrega
  const buscarTaxaPorBairro = async (nomeBairro: string, nomeCidade?: string) => {
    const bairro = nomeBairro.trim();
    if (!bairro) { setBairroTaxa(null); return; }
    try {
      const cidade = nomeCidade?.trim() ?? '';
      const { data } = cidade
        ? await supabase.from('taxa_entrega').select('valor').ilike('bairro', bairro).ilike('cidade', cidade).limit(1)
        : await supabase.from('taxa_entrega').select('valor').ilike('bairro', bairro).limit(1);
      const raw = data && data.length > 0 ? Number(data[0].valor) : NaN;
      setBairroTaxa(isNaN(raw) ? null : raw);
    } catch { setBairroTaxa(null); }
  };

  // Auto-preenchimento via ViaCEP
  const handleCepBlur = async () => {
    const cep = formData.cep.replace(/\D/g, '');
    if (cep.length !== 8) return;
    try {
      const res = await fetch(`https://viacep.com.br/ws/${cep}/json/`);
      const viaCepData = await res.json();
      if (!viaCepData.erro) {
        const bairroViaCep = viaCepData.bairro || '';
        setFormData(prev => ({
          ...prev,
          endereco: viaCepData.logradouro || prev.endereco,
          bairro: bairroViaCep || prev.bairro,
          cidade: viaCepData.localidade || prev.cidade,
          uf: viaCepData.uf || prev.uf,
        }));
        // Busca taxa pelo bairro+cidade retornados pelo ViaCEP
        const bairroParaBusca = bairroViaCep || formData.bairro;
        const cidadeParaBusca = viaCepData.localidade || formData.cidade;
        if (bairroParaBusca) await buscarTaxaPorBairro(bairroParaBusca, cidadeParaBusca);
      }
    } catch {}
  };

  // Função de Máscara de WhatsApp
  const formatWhatsApp = (value: string) => {
    const nums = value.replace(/\D/g, '');
    if (nums.length <= 2) return nums;
    if (nums.length <= 7) return `(${nums.slice(0, 2)}) ${nums.slice(2)}`;
    return `(${nums.slice(0, 2)}) ${nums.slice(2, 7)}-${nums.slice(7, 11)}`;
  };

  // Função de Máscara de CPF/CNPJ
  const formatCpfCnpj = (value: string) => {
    const nums = value.replace(/\D/g, '').slice(0, 14);
    if (nums.length <= 11) {
      return nums
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d{1,2})/, '$1-$2');
    }
    return nums
      .replace(/(\d{2})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1/$2')
      .replace(/(\d{4})(\d{1,2})/, '$1-$2');
  };

  const handleSearchWhatsapp = async () => {
    const rawWhatsapp = whatsapp.replace(/\D/g, '');
    if (rawWhatsapp.length < 11) return;
    
    setLoading(true);
    try {
      // 1. Busca se o cliente já existe
      const { data: clientData } = await supabase
        .from('cliente')
        .select('id, nome, telefone, cpf_cnpj')
        .eq('telefone', rawWhatsapp)
        .maybeSingle();

      if (clientData) {
        setClientFound(clientData);
        setShowClientForm(false);
        // Busca e aplica automaticamente cupom de fidelização do cliente
        await aplicarCupomFidelizacaoDoCliente(clientData.id);
      } else {
        setClientFound(null);
        setShowClientForm(true);
      }

      // 2. Busca endereços vinculados a este WhatsApp
      const { data: addressData } = await supabase
        .from('enderecos_salvo')
        .select('id, whatsapp, endereco, numero, bairro, complemento, cidade, cep, created_at')
        .eq('whatsapp', rawWhatsapp);

      if (addressData && addressData.length > 0) {
        setAddressesFound(addressData as EnderecoSalvo[]);
        setSelectedAddress(addressData[0] as EnderecoSalvo);
        setShowAddressForm(false);
      } else {
        setAddressesFound([]);
        setSelectedAddress(null);
        setShowAddressForm(true);
      }
    } catch (error) {
      console.error('Erro ao buscar dados:', error);
      setClientFound(null);
      setShowClientForm(true);
      setAddressesFound([]);
      setSelectedAddress(null);
      setShowAddressForm(true);
    } finally {
      setLoading(false);
    }
  };

  const handleRegisterClient = async () => {
    const rawWhatsapp = whatsapp.replace(/\D/g, '');
    const rawCpfCnpj = clientFormData.cpf_cnpj.replace(/\D/g, '');
    
    const isValidCpfCnpj = rawCpfCnpj.length === 11 || rawCpfCnpj.length === 14;
    if (!clientFormData.nome || !isValidCpfCnpj) {
      alert('Por favor, preencha o nome e um CPF/CNPJ válido.');
      return;
    }

    setLoading(true);
    try {
      const { data: newClient, error: clientError } = await supabase
        .from('cliente')
        .insert([{
          nome: clientFormData.nome,
          telefone: rawWhatsapp,
          cpf_cnpj: rawCpfCnpj
        }])
        .select()
        .single();

      if (clientError) throw clientError;
      
      setClientFound(newClient);
      setShowClientForm(false);
    } catch (error: any) {
      alert('Erro ao salvar cadastro: ' + error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleSaveNewAddress = async () => {
    const rawWhatsapp = whatsapp.replace(/\D/g, '');
    if (!formData.endereco || !formData.numero || !formData.bairro || !formData.cidade) {
      alert('Preencha pelo menos rua, número, bairro e cidade.');
      return;
    }
    setLoading(true);
    try {
      const { data: savedAddr, error } = await supabase
        .from('enderecos_salvo')
        .insert([{
          whatsapp: rawWhatsapp,
          cep: formData.cep.replace(/\D/g, '') || null,
          endereco: formData.endereco,
          numero: formData.numero,
          bairro: formData.bairro,
          complemento: formData.complemento || null,
          cidade: formData.cidade
        }])
        .select()
        .single();

      if (error) throw error;

      setAddressesFound(prev => [...prev, savedAddr as EnderecoSalvo]);
      setSelectedAddress(savedAddr as EnderecoSalvo);
      setShowAddressForm(false);
      if ((savedAddr as EnderecoSalvo).bairro) {
        buscarTaxaPorBairro((savedAddr as EnderecoSalvo).bairro!, (savedAddr as EnderecoSalvo).cidade ?? undefined);
      }
    } catch (err: any) {
      alert('Erro ao salvar endereço: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleApplyCoupon = async () => {
    if (!couponCode.trim()) return;

    setLoading(true);
    try {
      let cupomQuery = supabase
        .from('cupom')
        .select('*')
        .eq('codigo', couponCode.trim())
        .eq('ativo', true);
      if (empresaCodigo) cupomQuery = cupomQuery.eq('empresa_codigo', empresaCodigo);
      let { data, error } = await cupomQuery.single();
      if (error && isEmpresaCodigoMissing(error)) {
        // coluna ainda não existe no Supabase — busca sem filtro de empresa
        const fallback = await supabase.from('cupom').select('*')
          .eq('codigo', couponCode.trim()).eq('ativo', true).single();
        data = fallback.data;
        error = fallback.error;
      }

      if (error || !data) {
        alert('Cupom inválido ou não encontrado.');
        setAppliedCoupon(null);
        return;
      }

      const now = new Date();
      const expiryDate = new Date(data.validade);
      
      // Define a expiração para o final do dia (23:59:59) para garantir que 
      // o cupom ainda seja válido durante todo o dia da validade informado.
      expiryDate.setHours(23, 59, 59, 999);

      if (expiryDate < now) {
        alert('Este cupom já expirou.');
        setAppliedCoupon(null);
        return;
      }

      setAppliedCoupon({
        id: data.id,
        valor: Number(data.valor),
        tipo: data.tipo as 'fixo' | 'porcentagem',
        codigo: data.codigo
      });
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const deliveryFee = bairroTaxa !== null ? bairroTaxa : Number(store?.taxa_entrega ?? 0);
  const discountValue = (() => {
    if (!appliedCoupon) return 0;
    if (appliedCoupon.tipo === 'porcentagem') return cartTotal * appliedCoupon.valor / 100;
    if (appliedCoupon.tipo === 'produto') {
      // Desconta o preço real do item do carrinho que corresponde ao produto prêmio
      const nomePremio = appliedCoupon.produto_nome?.toLowerCase().trim();
      if (nomePremio) {
        const itemPremio = cartItems.find(item =>
          item.product.nome.toLowerCase().trim() === nomePremio
        );
        if (itemPremio) {
          // Desconta apenas 1 unidade do produto prêmio, independente da quantidade no carrinho
          const precoItem = (itemPremio.product.preco_promocional || itemPremio.product.preco_venda)
            + itemPremio.adicionais.reduce((a, ad) => a + ad.preco * ad.quantity, 0);
          return precoItem;
        }
      }
      // Fallback: usa o valor armazenado no cupão
      return appliedCoupon.valor || 0;
    }
    return appliedCoupon.valor || 0;
  })();
  const totalOrder = Math.max(0, cartTotal + deliveryFee - discountValue);

  /** Busca o cupom de fidelização vinculado ao cliente e aplica automaticamente,
   *  desde que não haja já um cupom aplicado manualmente. */
  const aplicarCupomFidelizacaoDoCliente = async (clienteId: string) => {
    if (appliedCoupon) return; // já tem cupom aplicado — não sobrescreve
    try {
      const now = new Date().toISOString();
      // Busca até 10 cupons do cliente — filtra client-side por usos disponíveis
      const { data } = await supabase
        .from('cupom')
        .select('id, codigo, valor, tipo, validade, limite_usos, usos_realizados, produto_nome')
        .eq('cliente_id', clienteId)
        .eq('ativo', true)
        .gt('validade', now)
        .order('validade', { ascending: true })
        .limit(10);

      // Aceita cupom que ainda tem usos disponíveis (limite_usos=0 = ilimitado)
      const disponivel = data?.find(c =>
        (c.limite_usos ?? 1) === 0 ||
        (c.usos_realizados ?? 0) < (c.limite_usos ?? 1)
      );

      if (disponivel) {
        setAppliedCoupon({
          id: disponivel.id,
          valor: Number(disponivel.valor),
          tipo: disponivel.tipo as string,
          codigo: disponivel.codigo,
          produto_nome: disponivel.produto_nome ?? null,
        });
      }
    } catch {
      // ignora erro silenciosamente — não bloqueia o fluxo
    }
  };

  const handleConfirmOrder = async () => {
    setLoading(true);
    const rawWhatsapp = whatsapp.replace(/\D/g, '');
    try {
      let clientId = clientFound?.id;

      // 1. Salva o cliente se for um novo cadastro
      if (showClientForm) {
        const { data: newClient, error: clientError } = await supabase
          .from('cliente')
          .insert([{
            nome: clientFormData.nome,
            telefone: rawWhatsapp,
            cpf_cnpj: clientFormData.cpf_cnpj.replace(/\D/g, '')
          }])
          .select()
          .single();

        if (clientError) throw clientError;
        clientId = newClient.id;
      }

      // 1. Salva o endereço se for um novo cadastro
        if (showAddressForm) {
        const { error: addressError } = await supabase
          .from('enderecos_salvo')
          .insert([{
            whatsapp: rawWhatsapp,
            cep: formData.cep.replace(/\D/g, '') || null,
            endereco: formData.endereco,
            numero: formData.numero,
            bairro: formData.bairro,
            complemento: formData.complemento || null,
            cidade: formData.cidade,
            uf: formData.uf || null
          }]);

        if (addressError) throw addressError;
      }

      const addressStr = selectedAddress 
        ? `${selectedAddress.endereco}, ${selectedAddress.numero}${selectedAddress.complemento ? ` - ${selectedAddress.complemento}` : ''} - ${selectedAddress.bairro} - ${selectedAddress.cidade}`
        : `${formData.endereco}, ${formData.numero}${formData.complemento ? ` - ${formData.complemento}` : ''} - ${formData.bairro} - ${formData.cidade}${formData.uf ? ` - ${formData.uf}` : ''}`;

      // 2. Salva o Pedido na tabela pedido_web
      const { data: orderData, error: orderError } = await supabase
        .from('pedido_web')
        .insert([{
          cliente_id: clientId,
          subtotal: cartTotal,
          taxa_entrega: deliveryFee,
          desconto: discountValue,
          total: totalOrder,
          forma_pagamento: paymentMethod?.nome,
          troco: paymentMethod?.tipo === 'dinheiro' ? parseFloat(troco.replace(',', '.')) || 0 : null,
          endereco_entrega: addressStr,
          cupom_id: appliedCoupon?.id || null,
          status: 'pendente',
          ...(empresaCodigo ? { empresa_codigo: empresaCodigo } : {})
        }])
        .select()
        .single();

      if (orderError) throw orderError;

      // 3. Salva os Itens do Pedido na tabela itens_pedido_web
      const orderItems = cartItems.map(item => {
        const adicionaisPrice = item.adicionais?.reduce((acc, a) => acc + a.preco * a.quantity, 0) || 0;
        const unitPrice = (item.product.preco_promocional || item.product.preco_venda) + adicionaisPrice;
        return {
          pedido_id: orderData.id,
          mercadoria_id: item.product.id,
          quantidade: item.quantity,
          preco_unitario: unitPrice,
          preco_adicionais: adicionaisPrice,
          complementos_json: item.complementos?.length > 0 ? item.complementos : null,
          adicionais_json: item.adicionais?.filter(a => a.quantity > 0).length > 0
            ? item.adicionais.filter(a => a.quantity > 0)
            : null,
        };
      });

      const { error: itemsError } = await supabase
        .from('itens_pedido_web')
        .insert(orderItems);

      if (itemsError) throw itemsError;

      // 4. Incrementa usos_realizados do cupom (não bloqueia o fluxo se falhar)
      if (appliedCoupon?.id) {
        try { await (supabase as any).rpc('increment_cupom_uso', { cupom_id: appliedCoupon.id }); } catch { }
      }

      // 5. Fluxo de Sucesso
      setShowSummary(false);
      setShowSuccess(true);
      clearCart();

      // Redireciona após 5 segundos
      setTimeout(() => {
        navigate('/');
      }, 5000);

    } catch (error: any) {
      alert('Erro ao finalizar pedido: ' + error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 pb-20">
      <header className="bg-white p-4 border-b flex items-center gap-4 sticky top-0 z-10">
        <button onClick={() => navigate(-1)} className="p-1">
          <ChevronLeft className="w-6 h-6" />
        </button>
        <h1 className="font-bold text-lg">Finalizar Pedido</h1>
      </header>

      <main className="p-4 max-w-2xl mx-auto space-y-6 mb-10">
        {/* WhatsApp Section */}
        <div className="bg-white p-4 sm:p-6 rounded-2xl shadow-sm space-y-4 border border-gray-100">
          <div className="flex items-center gap-2 mb-2">
            <div className="bg-green-100 p-2 rounded-lg">
              <Phone className="w-5 h-5 text-green-600" />
            </div>
            <h2 className="font-bold text-gray-800">Seu WhatsApp</h2>
          </div>
          
          <div className="flex gap-2 w-full">
            <input
              type="tel"
              placeholder="(00) 00000-0000"
              value={whatsapp}
              onChange={(e) => {
                setWhatsapp(formatWhatsApp(e.target.value));
              }}
              maxLength={15}
              className="flex-1 min-w-0 bg-gray-50 border-gray-200 rounded-xl py-3 px-3 sm:px-4 focus:ring-2 focus:ring-red-500 outline-none text-base sm:text-lg"
            />
            <button 
              onClick={handleSearchWhatsapp}
              disabled={loading || whatsapp.length < 14}
              className="bg-gray-900 text-white px-3 sm:px-6 rounded-xl font-bold disabled:opacity-50 whitespace-nowrap text-sm sm:text-base"
            >
              {loading ? <Loader2 className="w-5 h-5 animate-spin" /> : 'Buscar'}
            </button>
          </div>
        </div>

        {/* Client Status / Registration */}
        {clientFound && (
          <div className="bg-blue-50 p-4 rounded-2xl border border-blue-100 space-y-2 animate-in fade-in slide-in-from-bottom-2">
            <div className="flex items-center gap-3">
              <div className="bg-blue-600 p-2 rounded-full shrink-0">
                <User className="w-4 h-4 text-white" />
              </div>
              <div>
                <p className="text-sm font-bold text-blue-900">Olá, {clientFound.nome}!</p>
                <p className="text-xs text-blue-700">Seu cadastro foi localizado com sucesso.</p>
              </div>
            </div>
            {appliedCoupon && (appliedCoupon.codigo.startsWith('FID') || appliedCoupon.tipo === 'produto') && (
              <div className="flex items-center gap-2 px-3 py-2 bg-amber-50 rounded-xl border border-amber-200 animate-in zoom-in-95">
                <Tag className="w-3.5 h-3.5 text-amber-600 shrink-0" />
                <p className="text-xs text-amber-800 font-medium">
                  {appliedCoupon.tipo === 'produto'
                    ? <>🎁 Prêmio Fidelidade: <span className="font-black">Produto Grátis</span> — R$ {appliedCoupon.valor.toFixed(2)} de desconto aplicado!</>
                    : <>Cupom de fidelidade <span className="font-black">{appliedCoupon.codigo}</span> aplicado automaticamente!{' '}
                      {appliedCoupon.tipo === 'porcentagem'
                        ? `${appliedCoupon.valor}% de desconto`
                        : `R$ ${appliedCoupon.valor.toFixed(2)} de desconto`}</>}
                </p>
              </div>
            )}
          </div>
        )}

        {showClientForm && (
          <div className="bg-white p-6 rounded-2xl shadow-sm space-y-4 border border-gray-100 animate-in fade-in slide-in-from-bottom-4">
            <div className="flex items-center gap-2 mb-2">
              <div className="bg-blue-100 p-2 rounded-lg">
                <User className="w-5 h-5 text-blue-600" />
              </div>
              <h2 className="font-bold text-gray-800">Complete seu Cadastro</h2>
            </div>

            <div className="grid grid-cols-1 gap-3">
              <div className="space-y-1">
                <label className="text-xs font-bold text-gray-400 uppercase ml-1">Nome Completo</label>
                <input
                  placeholder="Como devemos te chamar?"
                  className="w-full bg-gray-50 border-gray-200 rounded-xl py-3 px-4 outline-none focus:ring-2 focus:ring-blue-500"
                  value={clientFormData.nome}
                  onChange={(e) => setClientFormData({...clientFormData, nome: e.target.value.replace(/[^a-zA-ZÀ-ÿ\s]/g, '')})}
                />
              </div>
              <div className="space-y-1">
                <label className="text-xs font-bold text-gray-400 uppercase ml-1">CPF / CNPJ</label>
                <input
                  placeholder="000.000.000-00"
                  maxLength={18}
                  className="w-full bg-gray-50 border-gray-200 rounded-xl py-3 px-4 outline-none focus:ring-2 focus:ring-blue-500"
                  value={clientFormData.cpf_cnpj}
                  onChange={(e) => setClientFormData({...clientFormData, cpf_cnpj: formatCpfCnpj(e.target.value)})}
                />
              </div>
            </div>

            <button
              onClick={handleRegisterClient}
              disabled={
                loading || 
                !clientFormData.nome || 
                !(clientFormData.cpf_cnpj.replace(/\D/g, '').length === 11 || 
                  clientFormData.cpf_cnpj.replace(/\D/g, '').length === 14)
              }
              className="w-full bg-blue-600 text-white py-3 rounded-xl font-bold flex items-center justify-center gap-2 disabled:opacity-50 transition-all active:scale-[0.98]"
            >
              {loading ? <Loader2 className="w-5 h-5 animate-spin" /> : <Save className="w-5 h-5" />}
              Salvar Cadastro
            </button>
          </div>
        )}

        {/* Address Display / Form */}
        {addressesFound.length > 0 && !showAddressForm && (
          <div className="space-y-3 animate-in fade-in slide-in-from-bottom-4">
            <div className="flex items-center justify-between px-2">
              <h2 className="font-bold text-gray-800 flex items-center gap-2">
                <MapPin className="w-5 h-5 text-red-600" />
                Endereços Encontrados
              </h2>
              <button 
                onClick={() => {
                  setShowAddressForm(true);
                  setSelectedAddress(null);
                }}
                className="text-xs font-bold text-red-600 uppercase p-2 hover:bg-red-50 rounded-lg transition-colors"
              >
                Novo Endereço
              </button>
            </div>
            
            {addressesFound.map((addr) => (
              <div 
                key={addr.id}
                onClick={() => {
                  setSelectedAddress(addr);
                  if (addr.bairro) buscarTaxaPorBairro(addr.bairro, addr.cidade ?? undefined);
                  else setBairroTaxa(null);
                }}
                className={`p-4 rounded-2xl cursor-pointer transition-all border-2 ${
                  selectedAddress?.id === addr.id 
                    ? 'border-green-500 bg-green-50 shadow-md' 
                    : 'border-white bg-white shadow-sm'
                }`}
              >
                <p className="text-gray-700 font-medium">
                  {addr.endereco}, {addr.numero}{addr.complemento ? ` - ${addr.complemento}` : ''}<br />
                  <span className="text-sm text-gray-500">{addr.bairro} - {addr.cidade}{addr.cep ? ` · CEP ${addr.cep}` : ''}</span>
                </p>
              </div>
            ))}
          </div>
        )}

        {showAddressForm && (
          <div className="bg-white p-6 rounded-2xl shadow-sm space-y-4 border border-gray-100 animate-in fade-in slide-in-from-bottom-4">
            <div className="flex items-center gap-2 mb-2">
              <div className="bg-red-100 p-2 rounded-lg">
                <MapPin className="w-5 h-5 text-red-600" />
              </div>
              <h2 className="font-bold text-gray-800">Endereço de Entrega</h2>
            </div>

            <div className="grid grid-cols-4 gap-3">
              <div className="col-span-2 space-y-1">
                <label className="text-xs font-bold text-gray-400 uppercase ml-1">CEP</label>
                <input
                  placeholder="00000-000"
                  className="w-full bg-gray-50 border-gray-200 rounded-xl py-3 px-4 outline-none focus:ring-2 focus:ring-red-500"
                  value={formData.cep}
                  onChange={(e) => setFormData({...formData, cep: formatCep(e.target.value)})}
                  onBlur={handleCepBlur}
                  maxLength={9}
                />
              </div>
              <div className="col-span-3 space-y-1">
                <label className="text-xs font-bold text-gray-400 uppercase ml-1">Rua / Logradouro</label>
                <input
                  placeholder="Rua / Logradouro"
                  className="w-full bg-gray-50 border-gray-200 rounded-xl py-3 px-4 outline-none focus:ring-2 focus:ring-red-500"
                  value={formData.endereco}
                  onChange={(e) => setFormData({...formData, endereco: e.target.value})}
                />
              </div>
              <div className="col-span-1 space-y-1">
                <label className="text-xs font-bold text-gray-400 uppercase ml-1">Nº</label>
                <input
                  placeholder="Nº"
                  className="w-full bg-gray-50 border-gray-200 rounded-xl py-3 px-4 outline-none focus:ring-2 focus:ring-red-500"
                  value={formData.numero}
                  onChange={(e) => setFormData({...formData, numero: e.target.value})}
                />
              </div>
              <div className="col-span-2 space-y-1">
                <label className="text-xs font-bold text-gray-400 uppercase ml-1">Bairro</label>
                <input
                  placeholder="Bairro"
                  className="w-full bg-gray-50 border-gray-200 rounded-xl py-3 px-4 outline-none focus:ring-2 focus:ring-red-500"
                  value={formData.bairro}
                  onChange={(e) => setFormData({...formData, bairro: e.target.value})}
                  onBlur={() => buscarTaxaPorBairro(formData.bairro, formData.cidade)}
                />
              </div>
              <div className="col-span-2 space-y-1">
                <label className="text-xs font-bold text-gray-400 uppercase ml-1">Cidade</label>
                <input
                  placeholder="Cidade"
                  className="w-full bg-gray-50 border-gray-200 rounded-xl py-3 px-4 outline-none focus:ring-2 focus:ring-red-500"
                  value={formData.cidade}
                  onChange={(e) => setFormData({...formData, cidade: e.target.value})}
                />
              </div>
              <div className="col-span-4 space-y-1">
                <label className="text-xs font-bold text-gray-400 uppercase ml-1">Complemento</label>
                <input
                  placeholder="Apto, bloco, referência... (opcional)"
                  className="w-full bg-gray-50 border-gray-200 rounded-xl py-3 px-4 outline-none focus:ring-2 focus:ring-red-500"
                  value={formData.complemento}
                  onChange={(e) => setFormData({...formData, complemento: e.target.value})}
                />
              </div>
            </div>

            {bairroTaxa !== null && (
              <div className="flex items-center justify-between px-4 py-3 bg-green-50 rounded-xl border border-green-200">
                <span className="text-sm font-medium text-green-800">Taxa para este bairro:</span>
                <span className="text-sm font-bold text-green-700">R$ {bairroTaxa.toFixed(2)}</span>
              </div>
            )}

            <button
              onClick={handleSaveNewAddress}
              disabled={loading || !formData.endereco || !formData.numero || !formData.bairro || !formData.cidade}
              className="w-full bg-red-600 text-white py-3 rounded-xl font-bold flex items-center justify-center gap-2 disabled:opacity-50 transition-all active:scale-[0.98]"
            >
              {loading ? <Loader2 className="w-5 h-5 animate-spin" /> : <Save className="w-5 h-5" />}
              Salvar Endereço
            </button>
          </div>
        )}

        {/* Coupon Section */}
        <div className="bg-white p-4 sm:p-6 rounded-2xl shadow-sm space-y-4 border border-gray-100">
          <div className="flex items-center gap-2 mb-2">
            <div className="bg-red-100 p-2 rounded-lg">
              <Tag className="w-5 h-5 text-red-600" />
            </div>
            <h2 className="font-bold text-gray-800">Cupom de Desconto</h2>
          </div>
          
          {!appliedCoupon ? (
            <div className="flex gap-2 w-full">
              <input
                placeholder="Digite o código"
                value={couponCode}
                onChange={(e) => setCouponCode(e.target.value.toUpperCase())}
                className="flex-1 min-w-0 bg-gray-50 border-gray-200 rounded-xl py-3 px-3 sm:px-4 focus:ring-2 focus:ring-red-500 outline-none text-base sm:text-lg"
              />
              <button 
                onClick={handleApplyCoupon}
                disabled={loading || !couponCode.trim()}
                className="bg-gray-900 text-white px-3 sm:px-6 rounded-xl font-bold disabled:opacity-50 whitespace-nowrap text-sm sm:text-base"
              >
                Aplicar
              </button>
            </div>
          ) : (
            <div className="flex items-center justify-between p-3 bg-green-50 rounded-xl border border-green-200 animate-in zoom-in-95">
              <div className="flex items-center gap-2">
                <Check className="w-4 h-4 text-green-600" />
                <div>
                  <p className="text-sm font-bold text-green-800">Cupom: {appliedCoupon.codigo}</p>
                  <p className="text-xs text-green-600">
                    Desconto de {appliedCoupon.tipo === 'porcentagem' ? `${appliedCoupon.valor}%` : `R$ ${appliedCoupon.valor.toFixed(2)}`} aplicado!
                  </p>
                </div>
              </div>
              <button 
                onClick={() => {
                  setAppliedCoupon(null);
                  setCouponCode('');
                }}
                className="p-1 hover:bg-green-100 rounded-lg transition-colors"
              >
                <X className="w-5 h-5 text-green-600" />
              </button>
            </div>
          )}
        </div>

        {/* Troco Section - Only for Cash */}
        {paymentMethod?.tipo === 'dinheiro' && (
          <div className="bg-white p-6 rounded-2xl shadow-sm space-y-4 border border-gray-100 animate-in fade-in slide-in-from-bottom-4">
            <div className="flex items-center gap-2 mb-2">
              <div className="bg-amber-100 p-2 rounded-lg">
                <Banknote className="w-5 h-5 text-amber-600" />
              </div>
              <h2 className="font-bold text-gray-800">Troco</h2>
            </div>
            
            <div className="space-y-2">
              <p className="text-sm text-gray-500">Precisa de troco para quanto?</p>
              <div className="relative">
                <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 font-bold">R$</span>
                <input
                  type="number"
                  placeholder="0,00"
                  value={troco}
                  onChange={(e) => setTroco(e.target.value)}
                  className="w-full bg-gray-50 border-gray-200 rounded-xl py-3 pl-10 pr-4 outline-none focus:ring-2 focus:ring-red-500 text-lg font-bold"
                />
              </div>
            </div>
          </div>
        )}

        {/* Summary */}
        <div className="bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
          <h2 className="font-bold text-gray-800 mb-4">Resumo do Pagamento</h2>
          <div className="space-y-2">
            <div className="flex items-center justify-between p-3 bg-gray-50 rounded-xl border border-dashed border-gray-200">
              <span className="text-sm text-gray-600 font-medium">Forma selecionada:</span>
              <span className="text-sm font-bold text-red-600">{paymentMethod?.nome || 'Não selecionada'}</span>
            </div>
            <div className="flex items-center justify-between p-3 bg-gray-50 rounded-xl border border-dashed border-gray-200">
              <span className="text-sm text-gray-600 font-medium">
                Taxa de entrega{bairroTaxa !== null ? ' (pelo bairro)' : ''}:
              </span>
              <span className={`text-sm font-bold ${deliveryFee === 0 ? 'text-green-600' : 'text-gray-800'}`}>
                {!deliveryFee || isNaN(deliveryFee) ? 'Grátis' : `R$ ${deliveryFee.toFixed(2)}`}
              </span>
            </div>
          </div>
        </div>
      </main>

      <div className="fixed bottom-0 left-0 right-0 p-4 bg-white border-t">
        <button
          disabled={
            loading || 
            whatsapp.length < 14 || 
            (!selectedAddress && (!formData.endereco || !formData.cidade)) ||
            (showClientForm && (
              !clientFormData.nome || 
              !(clientFormData.cpf_cnpj.replace(/\D/g, '').length === 11 || clientFormData.cpf_cnpj.replace(/\D/g, '').length === 14)
            ))
          }
          onClick={() => setShowSummary(true)}
          className="w-full bg-red-600 text-white py-4 rounded-xl font-bold flex items-center justify-center gap-2 disabled:opacity-50 shadow-lg shadow-red-200"
        >
          Finalizar Pedido
        </button>
      </div>

      {/* Modal de Resumo do Pedido */}
      <AnimatePresence>
        {showSummary && (
          <>
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              onClick={() => setShowSummary(false)}
              className="fixed inset-0 bg-black/60 z-[60] backdrop-blur-sm"
            />
            <motion.div
              initial={{ y: "100%" }}
              animate={{ y: 0 }}
              exit={{ y: "100%" }}
              className="fixed bottom-0 left-0 right-0 bg-white rounded-t-[32px] z-[70] max-h-[90vh] flex flex-col"
            >
              <div className="p-6 overflow-y-auto flex-1">
                <div className="flex items-center justify-between mb-6">
                  <h2 className="text-xl font-black text-gray-900 flex items-center gap-2">
                    <ShoppingCart className="w-6 h-6 text-red-600" />
                    Resumo do Pedido
                  </h2>
                  <button onClick={() => setShowSummary(false)} className="p-2 bg-gray-100 rounded-full">
                    <X className="w-5 h-5" />
                  </button>
                </div>

                <div className="space-y-6">
                  {/* Itens */}
                  <div className="space-y-3">
                    <p className="text-xs font-bold text-gray-400 uppercase tracking-widest">Itens na Sacola</p>
                    {cartItems.map((item) => {
                      const adicionaisPrice = item.adicionais?.reduce((acc, a) => acc + a.preco * a.quantity, 0) || 0;
                      const unitPrice = (item.product.preco_promocional || item.product.preco_venda) + adicionaisPrice;
                      return (
                        <div key={item.id} className="text-sm space-y-0.5">
                          <div className="flex justify-between">
                            <span className="text-gray-700 font-medium">
                              <span className="font-bold text-red-600">{item.quantity}x</span> {item.product.nome}
                            </span>
                            <span className="font-bold text-gray-900">
                              R$ {(unitPrice * item.quantity).toFixed(2)}
                            </span>
                          </div>
                          {item.complementos?.length > 0 && (
                            <div className="ml-4 space-y-0.5">
                              {item.complementos.map((c) => (
                                <p key={c.grupoId} className="text-[11px] text-gray-400">
                                  {c.grupoNome}: {c.itemNome}
                                </p>
                              ))}
                            </div>
                          )}
                          {item.adicionais?.filter((a) => a.quantity > 0).length > 0 && (
                            <div className="ml-4 space-y-0.5">
                              {item.adicionais.filter((a) => a.quantity > 0).map((a) => (
                                <p key={a.id} className="text-[11px] text-green-600">
                                  +{a.quantity}x {a.nome}
                                </p>
                              ))}
                            </div>
                          )}
                        </div>
                      );
                    })}
                  </div>

                  {/* Endereço */}
                  <div className="space-y-2">
                    <p className="text-xs font-bold text-gray-400 uppercase tracking-widest">Entrega em</p>
                    <div className="p-3 bg-gray-50 rounded-xl flex gap-2">
                      <MapPin className="w-4 h-4 text-red-600 shrink-0 mt-0.5" />
                      <p className="text-sm text-gray-600">
                        {selectedAddress 
                          ? `${selectedAddress.endereco}, ${selectedAddress.numero} - ${selectedAddress.bairro}`
                          : `${formData.endereco}, ${formData.numero} - ${formData.bairro}`}
                      </p>
                    </div>
                  </div>

                  {/* Pagamento */}
                  <div className="space-y-2">
                    <p className="text-xs font-bold text-gray-400 uppercase tracking-widest">Pagamento</p>
                    <div className="p-3 bg-gray-50 rounded-xl flex items-center gap-2">
                      {paymentMethod?.tipo === 'pix' ? <QrCode className="w-4 h-4 text-green-600" /> : 
                       paymentMethod?.tipo === 'dinheiro' ? <Banknote className="w-4 h-4 text-green-600" /> : 
                       <CreditCard className="w-4 h-4 text-blue-600" />}
                      <p className="text-sm font-bold text-gray-700">{paymentMethod?.nome}</p>
                    </div>
                    {paymentMethod?.tipo === 'dinheiro' && troco && (
                      <div className="flex justify-between items-center px-1 mt-2 bg-amber-50 p-2 rounded-lg border border-amber-100">
                        <span className="text-xs text-amber-800 font-medium">Troco para:</span>
                        <span className="text-xs font-bold text-amber-900">
                          R$ {parseFloat(troco.replace(',', '.')).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                        </span>
                      </div>
                    )}
                  </div>

                  {/* Resumo Valores */}
                  <div className="pt-4 border-t border-gray-100 space-y-2">
                    <div className="flex justify-between text-sm text-gray-600">
                      <span>Subtotal</span>
                      <span>R$ {cartTotal.toFixed(2)}</span>
                    </div>
                    <div className="flex justify-between text-sm text-gray-600">
                      <span>Taxa de entrega</span>
                      <span>{!deliveryFee || isNaN(deliveryFee) ? 'Grátis' : `R$ ${deliveryFee.toFixed(2)}`}</span>
                    </div>
                    {appliedCoupon && (
                      <div className="flex justify-between text-sm text-green-600 font-medium">
                        <span>Desconto ({appliedCoupon.codigo})</span>
                        <span>- R$ {discountValue.toFixed(2)}</span>
                      </div>
                    )}
                    <div className="flex justify-between text-lg font-black text-gray-900 pt-2">
                      <span>Total a Pagar</span>
                      <span className="text-red-600">R$ {totalOrder.toFixed(2)}</span>
                    </div>
                  </div>
                </div>
              </div>

              <div className="p-6 bg-gray-50 border-t">
                <button
                  onClick={handleConfirmOrder}
                  disabled={loading}
                  className="w-full bg-green-600 text-white py-4 rounded-2xl font-bold flex items-center justify-center gap-3 shadow-lg shadow-green-100"
                >
                  {loading ? <Loader2 className="w-5 h-5 animate-spin" /> : <Check className="w-5 h-5" />}
                  Confirmar Pedido
                </button>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>

      {/* Modal de Agradecimento (Sucesso) */}
      <AnimatePresence>
        {showSuccess && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            className="fixed inset-0 bg-white z-[100] flex flex-col items-center justify-center p-6 text-center"
          >
            <motion.div
              initial={{ scale: 0 }}
              animate={{ scale: 1 }}
              transition={{ type: "spring", damping: 12, stiffness: 200 }}
              className="w-24 h-24 bg-green-100 rounded-full flex items-center justify-center mb-6"
            >
              <Check className="w-12 h-12 text-green-600" />
            </motion.div>
            
            <h2 className="text-2xl font-black text-gray-900 mb-2">Pedido Confirmado!</h2>
            <p className="text-gray-500 mb-8">
              Obrigado por escolher o RanGoFood. Seu pedido já foi enviado para a cozinha!
            </p>
            
            <div className="w-full max-w-xs bg-gray-100 h-1.5 rounded-full overflow-hidden">
              <motion.div 
                initial={{ width: "100%" }}
                animate={{ width: "0%" }}
                transition={{ duration: 5, ease: "linear" }}
                className="h-full bg-green-600"
              />
            </div>
            <p className="text-[10px] text-gray-400 mt-4 uppercase font-bold tracking-widest">
              Redirecionando em instantes...
            </p>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}