import { Routes, Route } from 'react-router-dom'

import Home from './pages/home/page'
import PainelGeral from './pages/painel-geral/page'
import { AppLayout } from './layouts/AppLayout'
import Comparacao from './pages/comparacao/page'
import AnaliseTemporal from './pages/analise-temporal/page'

function App() {
  return (
    <Routes>
      {/* HOME SEM LAYOUT */}
      <Route path="/" element={<Home />} />

      {/* ROTAS COM LAYOUT */}
      <Route element={<AppLayout />}>
        <Route path="/painel-geral" element={<PainelGeral />} />
        <Route path="/comparacao" element={<Comparacao />} />
        <Route path="/analise-temporal" element={<AnaliseTemporal />} />
      </Route>
    </Routes>
  )
}

export default App
