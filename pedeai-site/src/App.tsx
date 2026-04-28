import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import Home from '@/pages/Home';
import Sacola from '@/pages/Sacola';
import EnderecoLoja from '@/pages/EnderecoLoja';
import Checkout from '@/pages/Checkout';

const queryClient = new QueryClient();

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/cart" element={<Sacola />} />
          <Route path="/address" element={<EnderecoLoja />} />
          <Route path="/checkout" element={<Checkout />} />
        </Routes>
      </Router>
    </QueryClientProvider>
  );
}
