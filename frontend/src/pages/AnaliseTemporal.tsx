import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend,
  BarChart, Bar,
} from 'recharts'
import { TrendingUp, Calendar, Info } from 'lucide-react'
import { DashboardHeader } from './Dashboard'
import { KpiCard } from '../components/KpiCard'
import {
  MOCK_QUARTERLY,
  MOCK_ANNUAL_GROWTH,
  MOCK_SEASONALITY,
  MOCK_MONTHLY_COMPARISON,
} from '../api'

export function AnaliseTemporal() {
  return (
    <>
      <DashboardHeader
        title="Análise Temporal"
        subtitle="Evolução e tendências das despesas ao longo do tempo"
      />
      <div className="p-8 space-y-6">
        {/* KPI Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
          <KpiCard
            title="Crescimento anual médio"
            value="47,9%"
            icon={<TrendingUp className="h-5 w-5" />}
            subtitle="Média dos últimos 5 anos"
          />
          <KpiCard
            title="Pico sazonal"
            value="Janeiro"
            icon={<Calendar className="h-5 w-5" />}
            subtitle="Índice sazonal de 116%"
          />
          <div className="bg-white rounded-xl border border-gray-200 p-6 flex flex-col gap-3 hover:shadow-md transition-shadow duration-300">
            <div className="flex items-center justify-between">
              <p className="text-sm text-gray-500 font-medium">Tendência 2026</p>
              <span className="px-3 py-1 bg-emerald-50 text-emerald-600 text-xs font-semibold rounded-full">
                Estável
              </span>
            </div>
            <p className="text-2xl font-bold text-gray-900">+3,4%</p>
            <p className="text-xs text-gray-400">Projeção de crescimento</p>
          </div>
        </div>

        {/* Charts Row 1 */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
          {/* Despesas Trimestrais */}
          <div className="bg-white rounded-xl border border-gray-200 p-6">
            <div className="flex items-center gap-2 mb-1">
              <h3 className="font-semibold text-gray-900">Despesas Trimestrais</h3>
              <button className="text-gray-400 hover:text-gray-600 transition-colors">
                <Info className="h-4 w-4" />
              </button>
            </div>
            <p className="text-sm text-gray-500 mb-4">Evolução trimestral dos últimos 2 anos (em milhões R$)</p>
            <div className="h-[240px]">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={MOCK_QUARTERLY} margin={{ top: 5, right: 20, left: 0, bottom: 5 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" vertical={false} />
                  <XAxis dataKey="trimestre" tick={{ fontSize: 12, fill: '#64748b' }} axisLine={false} tickLine={false} />
                  <YAxis tick={{ fontSize: 11, fill: '#94a3b8' }} axisLine={false} tickLine={false} />
                  <Tooltip
                    contentStyle={{
                      background: 'white',
                      border: '1px solid #e2e8f0',
                      borderRadius: '8px',
                      fontSize: '12px',
                      boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)',
                    }}
                  />
                  <Legend iconType="circle" iconSize={8} wrapperStyle={{ fontSize: '12px' }} />
                  <Bar dataKey="ano2025" name="2025" fill="#008C6C" radius={[4, 4, 0, 0]} barSize={24} />
                  <Bar dataKey="ano2026" name="2026" fill="#142F4B" radius={[4, 4, 0, 0]} barSize={24} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>

          {/* Crescimento Anual */}
          <div className="bg-white rounded-xl border border-gray-200 p-6">
            <div>
              <h3 className="font-semibold text-gray-900">Crescimento anual</h3>
              <p className="text-sm text-gray-500 mb-4">Taxa de crescimento da despesa total por ano</p>
            </div>
            <div className="h-[240px]">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={MOCK_ANNUAL_GROWTH} margin={{ top: 5, right: 20, left: 0, bottom: 5 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                  <XAxis dataKey="ano" tick={{ fontSize: 12, fill: '#64748b' }} axisLine={false} tickLine={false} />
                  <YAxis tick={{ fontSize: 11, fill: '#94a3b8' }} axisLine={false} tickLine={false} />
                  <Tooltip
                    contentStyle={{
                      background: 'white',
                      border: '1px solid #e2e8f0',
                      borderRadius: '8px',
                      fontSize: '12px',
                      boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)',
                    }}
                  />
                  <Line type="monotone" dataKey="valor" stroke="#008C6C" strokeWidth={2} dot={{ r: 4, fill: '#008C6C', strokeWidth: 2, stroke: 'white' }} activeDot={{ r: 6 }} />
                </LineChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>

        {/* Índice de Sazonalidade */}
        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <div className="mb-1">
            <h3 className="font-semibold text-gray-900">Índice de sazonalidade</h3>
            <p className="text-sm text-gray-500">Padrão de gastos ao longo do ao (índice 100 = média mensal)</p>
          </div>
          <div className="h-[200px] mt-4">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={MOCK_SEASONALITY} margin={{ top: 5, right: 20, left: 0, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" vertical={false} />
                <XAxis dataKey="mes" tick={{ fontSize: 12, fill: '#64748b' }} axisLine={false} tickLine={false} />
                <YAxis tick={{ fontSize: 11, fill: '#94a3b8' }} axisLine={false} tickLine={false} />
                <Tooltip
                  contentStyle={{
                    background: 'white',
                    border: '1px solid #e2e8f0',
                    borderRadius: '8px',
                    fontSize: '12px',
                    boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)',
                  }}
                />
                <Bar dataKey="indice" fill="#142F4B" radius={[4, 4, 0, 0]} barSize={32} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Comparativo Mensal por Ano */}
        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <div className="mb-1">
            <h3 className="font-semibold text-gray-900">Comparativo mensal por ano</h3>
            <p className="text-sm text-gray-500">Despesas mensais dos últimos 2 anos (em milhões R$)</p>
          </div>
          <div className="h-[280px] mt-4">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={MOCK_MONTHLY_COMPARISON} margin={{ top: 5, right: 20, left: 0, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                <XAxis dataKey="mes" tick={{ fontSize: 12, fill: '#64748b' }} axisLine={false} tickLine={false} />
                <YAxis tick={{ fontSize: 11, fill: '#94a3b8' }} axisLine={false} tickLine={false} />
                <Tooltip
                  contentStyle={{
                    background: 'white',
                    border: '1px solid #e2e8f0',
                    borderRadius: '8px',
                    fontSize: '12px',
                    boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)',
                  }}
                />
                <Legend iconType="circle" iconSize={8} wrapperStyle={{ fontSize: '12px' }} />
                <Line type="monotone" dataKey="ano2025" name="2025" stroke="#008C6C" strokeWidth={2} dot={{ r: 4, fill: '#008C6C', strokeWidth: 2, stroke: 'white' }} />
                <Line type="monotone" dataKey="ano2026" name="2026" stroke="#94a3b8" strokeWidth={2} dot={{ r: 4, fill: '#94a3b8', strokeWidth: 2, stroke: 'white' }} strokeDasharray="6 3" />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>
    </>
  )
}
