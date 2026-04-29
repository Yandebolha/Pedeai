import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { Produto } from '@/types/database';

export interface SelectedComplemento {
  grupoId: string;
  grupoNome: string;
  itemId: string;
  itemNome: string;
}

export interface SelectedAdicional {
  id: string;
  nome: string;
  preco: number;
  quantity: number;
}

export interface CartItem {
  id: string;
  product: Produto;
  quantity: number;
  complementos: SelectedComplemento[];
  adicionais: SelectedAdicional[];
}

interface CartStore {
  items: CartItem[];
  addItem: (product: Produto, quantity: number, complementos?: SelectedComplemento[], adicionais?: SelectedAdicional[]) => void;
  removeItem: (id: string) => void;
  updateQuantity: (id: string, quantity: number) => void;
  clearCart: () => void;
  getTotal: () => number;
}

export const useCartStore = create<CartStore>()(
  persist(
    (set, get) => ({
      items: [],
      addItem: (product, quantity, complementos = [], adicionais = []) => {
        const currentItems = get().items;

        // Se tem personalizações, sempre cria item novo com ID único
        if (complementos.length > 0 || adicionais.length > 0) {
          const newId = `${product.id}-${Date.now()}`;
          set({ items: [...currentItems, { id: newId, product, quantity, complementos, adicionais }] });
          return;
        }

        // Quick-add sem personalização: agrupa por produto
        const existingItem = currentItems.find(
          (item) => item.product.id === product.id && item.complementos.length === 0 && item.adicionais.length === 0
        );

        if (existingItem) {
          set({
            items: currentItems.map((item) =>
              item.id === existingItem.id
                ? { ...item, quantity: item.quantity + quantity }
                : item
            ),
          });
        } else {
          set({ items: [...currentItems, { id: product.id, product, quantity, complementos: [], adicionais: [] }] });
        }
      },
      removeItem: (id) => set({ items: get().items.filter((item) => item.id !== id) }),
      updateQuantity: (id, quantity) =>
        set({
          items: get().items.map((item) => (item.id === id ? { ...item, quantity } : item)),
        }),
      clearCart: () => set({ items: [] }),
      getTotal: () => {
        return get().items.reduce((acc, item) => {
          const basePrice = item.product.preco_promocional || item.product.preco_venda;
          const adicionaisPrice = item.adicionais.reduce((a, add) => a + add.preco * add.quantity, 0);
          return acc + (basePrice + adicionaisPrice) * item.quantity;
        }, 0);
      },
    }),
    {
      name: 'rangofood-cart-storage',
    }
  )
);