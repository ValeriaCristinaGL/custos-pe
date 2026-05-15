import { useEffect, useState } from 'react'
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend,
  RadarChart, PolarGrid, PolarAngleAxis, PolarRadiusAxis, Radar,
} from 'recharts'
import { TrendingUp, Info } from 'lucide-react'
import { DashboardHeader } from './Dashboard'
import {
  getComparativo,
  getEvolucao,
  buildOrgaoEvolution,
  buildRadarData,
  type ComparativoOrgaos,
  type DrillDown,
} from '../api'

function formatCurrency(value: number): string {
  if (value >= 1_000_000_000) return `R$ ${(value / 1_000_000_000).toFixed(1)} bi`
  if (value >= 1_000_000) return `R$ ${(value / 1_000_000).toFixed(1)} mi`
  return `R$ ${value.toFixed(2)}`
}

function formatCurrencyFull(value: number): string {
  return value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL', minimumFractionDigits: 0 })
}

function clampValue(value: number, min: number, max: number): number {
  return Math.max(min, Math.min(max, value))
}

export function ComparacaoOrgaos() {
  const [comparativo, setComparativo] = useState<ComparativoOrgaos | null>(null)
  const [loading, setLoading] = useState(true)
  const [selectedYear, setSelectedYear] = useState(2026)
  const [selectedOrgaoCode, setSelectedOrgaoCode] = useState<string | null>(null)
  const [drillDown, setDrillDown] = useState<DrillDown | null>(null)
  const [drillLoading, setDrillLoading] = useState(false)

  useEffect(() => {
    async function fetchData() {
      setLoading(true)
      const data = await getComparativo(selectedYear)
      setComparativo(data)
      if (!selectedOrgaoCode || !data.orgaos.some((orgao) => orgao.codigoOrgao === selectedOrgaoCode)) {
        setSelectedOrgaoCode(data.orgaos[0]?.codigoOrgao ?? null)
      }
      setLoading(false)
    }
    fetchData()
  }, [selectedYear])

  useEffect(() => {
    async function fetchDrillDown() {
      if (!selectedOrgaoCode) return
      setDrillLoading(true)
      const data = await getEvolucao(selectedOrgaoCode, selectedYear)
      setDrillDown(data)
      setDrillLoading(false)
    }
    fetchDrillDown()
  }, [selectedOrgaoCode, selectedYear])

  if (loading || !comparativo) {
    return (
      <>
        <DashboardHeader
          title="Comparação entre Órgãos"
          subtitle="Análise comparativa de desempenho entre secretarias e órgãos"
        />
        <div className="p-8 flex items-center justify-center min-h-[400px]">
          <div className="loader" />
        </div>
      </>
    )
  }

  const topOrgaos = comparativo.orgaos.slice(0, 3)
  const tableOrgaos = comparativo.orgaos.slice(0, 5)
  const selectedOrgao = comparativo.orgaos.find((orgao) => orgao.codigoOrgao === selectedOrgaoCode)
  const baseEvolution = buildOrgaoEvolution(selectedYear)
  const baseRadar = buildRadarData(selectedYear)
  const primaryOrgao = selectedOrgao ?? comparativo.orgaos[0]
  const orgaosToPlot = primaryOrgao
    ? [primaryOrgao, ...comparativo.orgaos.filter((orgao) => orgao.codigoOrgao !== primaryOrgao.codigoOrgao).slice(0, 2)]
    : []
  const baseKeys = ['SEE', 'SES', 'SEINFRA'] as const
  const colorPalette = ['#008C6C', '#3B82F6', '#C79E41']

  const evolutionData = baseEvolution.map((item) => {
    const row: Record<string, number | string> = { mes: item.mes }
    orgaosToPlot.forEach((orgao, index) => {
      const baseKey = baseKeys[index % baseKeys.length]
      const baseTotal = comparativo.orgaos.find((entry) => entry.codigoOrgao === baseKey)?.totalEmpenhado
        ?? primaryOrgao?.totalEmpenhado
        ?? 1
      const scale = baseTotal > 0 ? orgao.totalEmpenhado / baseTotal : 1
      row[orgao.siglaOrgao] = Math.round(item[baseKey] * scale)
    })
    return row
  })

  const radarData = baseRadar.map((item) => {
    const row: Record<string, number | string> = { subject: item.subject }
    orgaosToPlot.forEach((orgao, index) => {
      const baseKey = baseKeys[index % baseKeys.length]
      const baseTotal = comparativo.orgaos.find((entry) => entry.codigoOrgao === baseKey)?.totalEmpenhado
        ?? primaryOrgao?.totalEmpenhado
        ?? 1
      const scale = baseTotal > 0 ? orgao.totalEmpenhado / baseTotal : 1
      row[orgao.siglaOrgao] = clampValue(Math.round(item[baseKey] * scale), 20, 100)
    })
    return row
  })

  return (
    <>
      <DashboardHeader
        title="Comparação entre Órgãos"
        subtitle="Análise comparativa de desempenho entre secretarias e órgãos"
      />
      <div className="p-8 space-y-6">
        <div className="flex flex-wrap items-center justify-end gap-3">
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
          <label className="text-xs text-gray-500">Órgão</label>
          <select
            value={selectedOrgaoCode ?? ''}
            onChange={(event) => setSelectedOrgaoCode(event.target.value)}
            className="rounded-lg border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700"
          >
            {comparativo.orgaos.map((orgao) => (
              <option key={orgao.codigoOrgao} value={orgao.codigoOrgao}>
                {orgao.siglaOrgao} - {orgao.nomeOrgao}
              </option>
            ))}
          </select>
        </div>
        {/* Charts Row */}
        <div className="grid grid-cols-1 lg:grid-cols-5 gap-5">
          {/* Evolução Comparativa */}
          <div className="lg:col-span-3 bg-white rounded-xl border border-gray-200 p-6">
            <div className="flex items-center gap-2 mb-1">
              <h3 className="font-semibold text-gray-900">Evolução Comparativa</h3>
              <button className="text-gray-400 hover:text-gray-600 transition-colors">
                <Info className="h-4 w-4" />
              </button>
            </div>
            <p className="text-sm text-gray-500 mb-4">Despesas por órgão ao longo dos anos (em milhões R$)</p>
            <div className="h-[280px]">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={evolutionData} margin={{ top: 5, right: 20, left: 0, bottom: 5 }}>
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
                  {orgaosToPlot.map((orgao, index) => {
                    const color = colorPalette[index % colorPalette.length]
                    return (
                      <Line
                        key={orgao.codigoOrgao}
                        type="monotone"
                        dataKey={orgao.siglaOrgao}
                        name={orgao.siglaOrgao}
                        stroke={color}
                        strokeWidth={2}
                        dot={{ r: 3, fill: color, strokeWidth: 2, stroke: 'white' }}
                      />
                    )
                  })}
                </LineChart>
              </ResponsiveContainer>
            </div>
          </div>

          {/* Radar Chart - Perfil de Gastos */}
          <div className="lg:col-span-2 bg-white rounded-xl border border-gray-200 p-6">
            <div>
              <h3 className="font-semibold text-gray-900">Perfil de Gastos</h3>
              <p className="text-sm text-gray-500">Análise multidimensional dos órgãos selecionados</p>
            </div>
            <div className="h-[250px] mt-2">
              <ResponsiveContainer width="100%" height="100%">
                <RadarChart data={radarData} cx="50%" cy="50%" outerRadius="70%">
                  <PolarGrid stroke="#e2e8f0" />
                  <PolarAngleAxis dataKey="subject" tick={{ fontSize: 11, fill: '#64748b' }} />
                  <PolarRadiusAxis tick={{ fontSize: 9, fill: '#94a3b8' }} axisLine={false} />
                  {orgaosToPlot.map((orgao, index) => {
                    const color = colorPalette[index % colorPalette.length]
                    return (
                      <Radar
                        key={orgao.codigoOrgao}
                        name={orgao.siglaOrgao}
                        dataKey={orgao.siglaOrgao}
                        stroke={color}
                        fill={color}
                        fillOpacity={0.12}
                        strokeWidth={2}
                      />
                    )
                  })}
                  <Legend iconType="circle" iconSize={8} wrapperStyle={{ fontSize: '12px' }} />
                  <Tooltip
                    contentStyle={{
                      background: 'white',
                      border: '1px solid #e2e8f0',
                      borderRadius: '8px',
                      fontSize: '12px',
                    }}
                  />
                </RadarChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>

        {/* Órgão Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
          {topOrgaos.map((orgao, i) => {
            const execPct = orgao.totalEmpenhado > 0
              ? ((orgao.totalPago / orgao.totalEmpenhado) * 100).toFixed(1)
              : '0'
            return (
              <button
                key={orgao.codigoOrgao}
                type="button"
                onClick={() => setSelectedOrgaoCode(orgao.codigoOrgao)}
                className={`bg-white rounded-xl border p-6 text-left hover:shadow-md transition-shadow duration-300 ${
                  selectedOrgaoCode === orgao.codigoOrgao
                    ? 'border-emerald-400 ring-1 ring-emerald-200'
                    : 'border-gray-200'
                }`}
              >
                <div className="flex items-center justify-between mb-3">
                  <div className="flex items-center gap-3">
                    <span className="inline-flex h-8 w-8 items-center justify-center rounded-lg bg-[#142F4B] text-white text-sm font-bold">
                      {i + 1}
                    </span>
                    <div>
                      <p className="font-semibold text-gray-900">{orgao.siglaOrgao}</p>
                      <p className="text-xs text-gray-500">{orgao.nomeOrgao.length > 20 ? orgao.nomeOrgao.slice(0, 20) + '...' : orgao.nomeOrgao}</p>
                    </div>
                  </div>
                  <span className="flex items-center gap-1 px-2 py-1 bg-emerald-50 text-emerald-600 text-xs font-semibold rounded-full">
                    <TrendingUp className="h-3 w-3" />
                    +4,2%
                  </span>
                </div>
                <p className="text-2xl font-bold text-gray-900 mb-2">{formatCurrency(orgao.totalEmpenhado)}</p>
                <div className="flex items-center gap-2">
                  <div className="flex-1 bg-gray-100 rounded-full h-1.5">
                    <div
                      className="h-full rounded-full bg-[#008C6C] transition-all duration-500"
                      style={{ width: `${Math.min(parseFloat(execPct), 100)}%` }}
                    />
                  </div>
                  <span className="text-xs text-gray-500 font-medium">{execPct}%</span>
                </div>
              </button>
            )
          })}
        </div>

        {/* Detalhamento por Órgão - Tabela */}
        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <div className="mb-4">
            <h3 className="font-semibold text-gray-900">Detalhamento por Órgão</h3>
            <p className="text-sm text-gray-500">Execução orçamentária detalhada por secretaria</p>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-gray-200">
                  <th className="text-left py-3 px-4 font-semibold text-gray-600">Órgão</th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">Orçamento</th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">Executado</th>
                  <th className="text-center py-3 px-4 font-semibold text-gray-600">Execução</th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">Pessoal</th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">Investimento</th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">Servidores</th>
                </tr>
              </thead>
              <tbody>
                {tableOrgaos.map((orgao) => {
                  const execPct = orgao.totalEmpenhado > 0
                    ? ((orgao.totalPago / orgao.totalEmpenhado) * 100)
                    : 0
                  const barColor = execPct > 80 ? '#22c55e' : execPct > 50 ? '#f59e0b' : '#ef4444'
                  const pessoal = orgao.totalEmpenhado * 0.68
                  const investimento = orgao.totalEmpenhado * 0.1
                  const servidores = Math.round(orgao.totalEmpenhado / 150000)
                  return (
                    <tr key={orgao.codigoOrgao} className="border-b border-gray-100 hover:bg-gray-50/50 transition-colors">
                      <td className="py-3 px-4">
                        <p className="font-medium text-gray-900">{orgao.siglaOrgao}</p>
                        <p className="text-xs text-gray-500">{orgao.nomeOrgao}</p>
                      </td>
                      <td className="text-right py-3 px-4 text-gray-700 font-medium tabular-nums">{formatCurrencyFull(orgao.totalEmpenhado)}</td>
                      <td className="text-right py-3 px-4 text-gray-700 font-medium tabular-nums">{formatCurrencyFull(orgao.totalPago)}</td>
                      <td className="py-3 px-4">
                        <div className="flex items-center gap-2 justify-center">
                          <div className="w-20 bg-gray-100 rounded-full h-2">
                            <div className="h-full rounded-full transition-all duration-500" style={{ width: `${Math.min(execPct, 100)}%`, backgroundColor: barColor }} />
                          </div>
                          <span className="text-xs font-medium text-gray-600 w-10 text-right">{execPct.toFixed(1)}%</span>
                        </div>
                      </td>
                      <td className="text-right py-3 px-4 text-gray-700 tabular-nums">{formatCurrencyFull(pessoal)}</td>
                      <td className="text-right py-3 px-4 text-gray-700 tabular-nums">{formatCurrencyFull(investimento)}</td>
                      <td className="text-right py-3 px-4 text-gray-700 tabular-nums">{servidores.toLocaleString('pt-BR')}</td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>
        </div>

        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <div className="mb-4 flex flex-wrap items-center justify-between gap-2">
            <div>
              <h3 className="font-semibold text-gray-900">Drill-down por órgão</h3>
              <p className="text-sm text-gray-500">
                {selectedOrgao
                  ? `Classificações de custo para ${selectedOrgao.siglaOrgao} (${selectedYear})`
                  : 'Selecione um órgão para ver os itens.'}
              </p>
            </div>
            {drillLoading && <span className="text-xs text-gray-400">Carregando...</span>}
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-gray-200">
                  <th className="text-left py-3 px-4 font-semibold text-gray-600">Classificação</th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-600">Descrição</th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">Total empenhado</th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">Qtd. empenhos</th>
                </tr>
              </thead>
              <tbody>
                {(drillDown?.itens ?? []).map((item) => (
                  <tr key={`${item.classificacaoMcasp}-${item.descricao}`} className="border-b border-gray-100">
                    <td className="py-3 px-4 text-gray-700 font-medium">{item.classificacaoMcasp}</td>
                    <td className="py-3 px-4 text-gray-600">{item.descricao}</td>
                    <td className="py-3 px-4 text-right text-gray-700 font-medium tabular-nums">
                      {formatCurrencyFull(item.totalEmpenhado)}
                    </td>
                    <td className="py-3 px-4 text-right text-gray-600 tabular-nums">
                      {item.quantidadeEmpenhos.toLocaleString('pt-BR')}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {!drillLoading && (drillDown?.itens?.length ?? 0) === 0 && (
              <p className="text-sm text-gray-500 px-4 py-6">Sem dados para o órgão selecionado.</p>
            )}
          </div>
        </div>
      </div>
    </>
  )
}
