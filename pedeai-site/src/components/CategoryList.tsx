import { Categoria } from '@/types/database';
import { motion } from 'motion/react';

interface CategoryListProps {
  categories: Categoria[];
  selectedId?: string;
  onSelect: (id: string) => void;
}

export function CategoryList({ categories, selectedId, onSelect }: CategoryListProps) {
  return (
    <div className="flex overflow-x-auto gap-4 px-4 py-4 no-scrollbar">
      {categories.map((category) => (
        <motion.button
          key={category.id}
          whileTap={{ scale: 0.95 }}
          onClick={() => onSelect(category.id)}
          className="flex flex-col items-center gap-2 min-w-[80px]"
        >
          <div 
            className={`w-16 h-16 rounded-full overflow-hidden border-2 transition-all ${
              selectedId === category.id ? 'border-red-600 scale-110' : 'border-transparent'
            }`}
          >
            <img
              src={category.imagem_url || 'https://via.placeholder.com/100'}
              alt={category.nome}
              className="w-full h-full object-cover"
              referrerPolicy="no-referrer"
            />
          </div>
          <span className={`text-xs font-medium ${
            selectedId === category.id ? 'text-red-600' : 'text-gray-600'
          }`}>
            {category.nome}
          </span>
        </motion.button>
      ))}
    </div>
  );
}
