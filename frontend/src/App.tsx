import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { LandingPage } from './pages/LandingPage'
import { DashboardLayout } from './pages/Dashboard'
import { PainelGeral } from './pages/PainelGeral'
import { ComparacaoOrgaos } from './pages/ComparacaoOrgaos'
import { AnaliseTemporal } from './pages/AnaliseTemporal'
import './index.css'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/dashboard" element={<DashboardLayout />}>
          <Route index element={<PainelGeral />} />
          <Route path="comparativo" element={<ComparacaoOrgaos />} />
          <Route path="evolucao" element={<AnaliseTemporal />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App
