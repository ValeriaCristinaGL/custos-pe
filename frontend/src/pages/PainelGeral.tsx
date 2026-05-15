import { useEffect, useState } from 'react'
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend,
  PieChart, Pie, Cell,
  BarChart, Bar,
} from 'recharts'
import { TrendingUp, DollarSign, PiggyBank, Info } from 'lucide-react'
import { DashboardHeader } from './Dashboard'
import { KpiCard } from '../components/KpiCard'
import {
  getResumo,
  getComparativo,
  buildMonthlyEvolution,
  buildCategoryDistribution,
  type DashboardResumo,
  type ComparativoOrgaos,
} from '../api'

function formatCurrency(value: number): string {
  if (value >= 1_000_000_000) return `R$ ${(value / 1_000_000_000).toFixed(1)} bi`
  if (value >= 1_000_000) return `R$ ${(value / 1_000_000).toFixed(1)} mi`
  if (value >= 1_000) return `R$ ${(value / 1_000).toFixed(1)} mil`
  return `R$ ${value.toFixed(2)}`
}

function formatTooltipNumber(value: unknown, suffix: string): [string, string] {
  const numericValue = typeof value === 'number' ? value : Number(value)
  if (Number.isNaN(numericValue)) return ['-', '']
  return [`${numericValue}${suffix}`, '']
}

export function PainelGeral() {
  const [resumo, setResumo] = useState<DashboardResumo | null>(null)
  const [comparativo, setComparativo] = useState<ComparativoOrgaos | null>(null)
  const [loading, setLoading] = useState(true)
  const [selectedYear, setSelectedYear] = useState(2026)

  useEffect(() => {
    async function fetchData() {
      setLoading(true)
      const [r, c] = await Promise.all([getResumo(selectedYear), getComparativo(selectedYear)])
      setResumo(r)
      setComparativo(c)
      setLoading(false)
    }
    fetchData()
  }, [selectedYear])

  if (loading || !resumo) {
    return (
      <>
        <DashboardHeader
          title="Visão Geral de Custos"
          subtitle="Visão consolidada das despesas do estado de Pernambuco"
        />
        <div className="p-8 flex items-center justify-center min-h-[400px]">
          <div className="loader" />
        </div>
      </>
    )
  }

  const topOrgaos = comparativo?.orgaos.slice(0, 10) ?? []
  const orgaoBars = topOrgaos.map((o) => ({
    sigla: o.siglaOrgao,
    valor: o.totalEmpenhado / 1_000_000_000,
  }))
  const monthlyData = buildMonthlyEvolution(selectedYear)
  const categoryData = buildCategoryDistribution(selectedYear)
  const currentKey = `ano${selectedYear}`
  const previousKey = `ano${selectedYear - 1}`

  return (
    <>
      <DashboardHeader
        title="Visão Geral de Custos"
        subtitle="Visão consolidada das despesas do estado de Pernambuco"
      />
      <div className="p-8 space-y-6">
        <div className="flex items-center justify-end gap-3">
          <label className="text-xs text-gray-500">Ano</label>
          <select
            value={selectedYear}
            onChange={(event) => setSelectedYear(Number(event.target.value))}
            className="rounded-lg border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700"
          >
            {[2026, 2025, 2024].map((year) => (
              <option key={year} value={year}>{year}</option>
            ))}
          </select>
        </div>
        {/* KPI Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
          <KpiCard
            title="Despesa Total"
            value={formatCurrency(resumo.totalEmpenhado)}
            icon={<TrendingUp className="h-5 w-5" />}
            trend={{ value: '+ 7,5%', positive: true }}
            subtitle="vs ano anterior"
          />
          <KpiCard
            title="Receita Total"
            value={formatCurrency(resumo.totalLiquidado)}
            icon={<DollarSign className="h-5 w-5" />}
            trend={{ value: '+ 7,5%', positive: true }}
            subtitle="vs ano anterior"
          />
          <KpiCard
            title="Investimentos"
            value={formatCurrency(resumo.totalPago)}
            icon={<PiggyBank className="h-5 w-5" />}
            trend={{ value: '+ 7,5%', positive: true }}
            subtitle="vs ano anterior"
          />
        </div>

        {/* Charts Row */}
        <div className="grid grid-cols-1 lg:grid-cols-5 gap-5">
          {/* Evolução Mensal */}
          <div className="lg:col-span-3 bg-white rounded-xl border border-gray-200 p-6">
            <div className="flex items-center justify-between mb-1">
              <div>
                <h3 className="font-semibold text-gray-900">Evolução mensal de despesas</h3>
                <p className="text-sm text-gray-500">Comparativo do ano atual (em milhões R$)</p>
              </div>
              <button className="text-gray-400 hover:text-gray-600 transition-colors">
                <Info className="h-4 w-4" />
              </button>
            </div>
            <div className="h-[280px] mt-4">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={monthlyData} margin={{ top: 5, right: 20, left: 0, bottom: 5 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                  <XAxis dataKey="mes" tick={{ fontSize: 11, fill: '#94a3b8' }} axisLine={false} tickLine={false} />
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
                  <Line type="monotone" dataKey={currentKey} name={String(selectedYear)} stroke="#008C6C" strokeWidth={2} dot={{ r: 4, fill: '#008C6C', strokeWidth: 2, stroke: 'white' }} activeDot={{ r: 6 }} />
                  <Line type="monotone" dataKey={previousKey} name={String(selectedYear - 1)} stroke="#3B82F6" strokeWidth={2} dot={{ r: 4, fill: '#3B82F6', strokeWidth: 2, stroke: 'white' }} activeDot={{ r: 6 }} strokeDasharray="6 3" />
                </LineChart>
              </ResponsiveContainer>
            </div>
          </div>

          {/* Distribuição por Categorias */}
          <div className="lg:col-span-2 bg-white rounded-xl border border-gray-200 p-6">
            <div className="flex items-center justify-between mb-1">
              <div>
                <h3 className="font-semibold text-gray-900">Distribuição por categorias</h3>
                <p className="text-sm text-gray-500">Composição das despesas no ano atual</p>
              </div>
            </div>
            <div className="h-[200px] mt-4">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={categoryData}
                    cx="50%"
                    cy="50%"
                    innerRadius={55}
                    outerRadius={80}
                    paddingAngle={3}
                    dataKey="value"
                    stroke="none"
                  >
                    {categoryData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} />
                    ))}
                  </Pie>
                  <Tooltip
                    formatter={(value) => formatTooltipNumber(value, '%')}
                    contentStyle={{
                      background: 'white',
                      border: '1px solid #e2e8f0',
                      borderRadius: '8px',
                      fontSize: '12px',
                    }}
                  />
                </PieChart>
              </ResponsiveContainer>
            </div>
            <div className="space-y-2 mt-2">
              {categoryData.map((cat) => (
                <div key={cat.name} className="flex items-center justify-between text-sm">
                  <div className="flex items-center gap-2">
                    <span className="w-2.5 h-2.5 rounded-full" style={{ backgroundColor: cat.color }} />
                    <span className="text-gray-600">{cat.name}</span>
                  </div>
                  <span className="text-gray-500 font-medium">{cat.value}%</span>
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* Maiores Órgãos por Despesa */}
        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <div className="mb-1">
            <h3 className="font-semibold text-gray-900">Maiores Órgãos por Despesa</h3>
            <p className="text-sm text-gray-500">Top 10 órgão com maior volume de despesas no ano atual</p>
          </div>
          <div className="h-[380px] mt-4">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={orgaoBars} layout="vertical" margin={{ top: 5, right: 30, left: 10, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" horizontal={false} />
                <XAxis type="number" tick={{ fontSize: 11, fill: '#94a3b8' }} axisLine={false} tickLine={false} />
                <YAxis type="category" dataKey="sigla" tick={{ fontSize: 12, fill: '#475569', fontWeight: 500 }} axisLine={false} tickLine={false} width={60} />
                <Tooltip
                  formatter={(value) => {
                    const numericValue = typeof value === 'number' ? value : Number(value)
                    const formatted = Number.isNaN(numericValue)
                      ? '-'
                      : `R$ ${numericValue.toFixed(1)} bi`
                    return [formatted, 'Órgãos']
                  }}
                  contentStyle={{
                    background: 'white',
                    border: '1px solid #e2e8f0',
                    borderRadius: '8px',
                    fontSize: '12px',
                    boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)',
                  }}
                />
                <Legend iconType="square" iconSize={10} wrapperStyle={{ fontSize: '12px' }} />
                <Bar dataKey="valor" name="Órgãos" fill="#2d4a63" radius={[0, 4, 4, 0]} barSize={18} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>
    </>
  )
}
