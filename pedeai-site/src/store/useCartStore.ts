import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { Produto } from '@/types/database';

export interface CartItem {
  id: string;
  product: Produto;
  quantity: number;
}

interface CartStore {
  items: CartItem[];
  addItem: (product: Produto, quantity: number) => void;
  removeItem: (id: string) => void;
  updateQuantity: (id: string, quantity: number) => void;
  clearCart: () => void;
  getTotal: () => number;
}

export const useCartStore = create<CartStore>()(
  persist(
    (set, get) => ({
      items: [],
      addItem: (product, quantity) => {
        const currentItems = get().items;
        const existingItem = currentItems.find((item) => item.product.id === product.id);

        if (existingItem) {
          set({
            items: currentItems.map((item) =>
              item.product.id === product.id
                ? { ...item, quantity: item.quantity + quantity }
                : item
            ),
          });
        } else {
          set({ items: [...currentItems, { id: product.id, product, quantity }] });
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
          const price = item.product.preco_promocional || item.product.preco_venda;
          return acc + price * item.quantity;
        }, 0);
      },
    }),
    {
      name: 'rangofood-cart-storage', // Chave usada no LocalStorage
    }
  )
);