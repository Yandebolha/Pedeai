import { useState } from 'react';
import { Produto } from '@/types/database';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Plus, ShoppingBag } from 'lucide-react';
import { useCartStore, SelectedComplemento, SelectedAdicional } from '@/store/useCartStore';
import { motion } from 'motion/react';
import { ProductModal } from '@/components/ProductModal';

interface ProductFeedProps {
  products: Produto[];
  categoryName: string;
}

export function ProductFeed({ products, categoryName }: ProductFeedProps) {
  const addItem = useCartStore((state) => state.addItem);
  const [selectedProduct, setSelectedProduct] = useState<Produto | null>(null);

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
                    setSelectedProduct(product); // sempre abre o modal para garantir seleção de complementos/sabores
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

      <ProductModal
        product={selectedProduct}
        onClose={() => setSelectedProduct(null)}
        onAdd={(p: Produto, q: number, c: SelectedComplemento[], a: SelectedAdicional[]) => {
          addItem(p, q, c, a);
          setSelectedProduct(null);
        }}
      />
    </div>
  );
}
