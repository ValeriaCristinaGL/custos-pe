import { useEffect, useRef, useState } from 'react'
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend,
  PieChart,
  Pie,
  Cell,
  BarChart,
  Bar,
} from 'recharts'
import {
  TrendingUp,
  DollarSign,
  PiggyBank,
  Info,
  Download,
  ChevronDown,
} from 'lucide-react'
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
import {
  exportToExcel,
  exportToPdf,
  type ExportTable,
} from '../services/ExportService'
import { toPng } from 'html-to-image'
type ExportYear = 2026 | 2025 | 2024

const exportYears: ExportYear[] = [2026, 2025, 2024]

type ExportOption = 'resumo' | 'evolucaoMensal' | 'categorias' | 'orgaos'

const exportOptions: { label: string; value: ExportOption }[] = [
  { label: 'Resumo geral', value: 'resumo' },
  { label: 'Evolução mensal', value: 'evolucaoMensal' },
  { label: 'Categorias', value: 'categorias' },
  { label: 'Órgãos', value: 'orgaos' },
]

function formatCurrency(value: number): string {
  if (value >= 1_000_000_000)
    return `R$ ${(value / 1_000_000_000).toFixed(1)} bi`
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
  const exportDropdownRef = useRef<HTMLDivElement | null>(null)
  const [resumo, setResumo] = useState<DashboardResumo | null>(null)
  const [comparativo, setComparativo] = useState<ComparativoOrgaos | null>(null)
  const [loading, setLoading] = useState(true)
  const [selectedYear, setSelectedYear] = useState(2026)
  const [selectedExportYears, setSelectedExportYears] = useState<ExportYear[]>(
    []
  )
  const [selectedExportOptions, setSelectedExportOptions] = useState<
    ExportOption[]
  >([])
  const [isExportDropdownOpen, setIsExportDropdownOpen] = useState(false)
  const [isExporting, setIsExporting] = useState(false)

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (
        exportDropdownRef.current &&
        !exportDropdownRef.current.contains(event.target as Node)
      ) {
        setIsExportDropdownOpen(false)
      }
    }

    document.addEventListener('mousedown', handleClickOutside)

    return () => {
      document.removeEventListener('mousedown', handleClickOutside)
    }
  }, [])

  function toggleExportOption(option: ExportOption) {
    setSelectedExportOptions((prev) => {
      if (prev.includes(option)) {
        return prev.filter((item) => item !== option)
      }

      return [...prev, option]
    })
  }

  function toggleExportYear(year: ExportYear) {
    setSelectedExportYears((prev) => {
      if (prev.includes(year)) {
        return prev.filter((item) => item !== year)
      }

      return [...prev, year]
    })
  }

  function selectAllExportYears() {
    setSelectedExportYears(exportYears)
  }

  function clearExportYears() {
    setSelectedExportYears([])
  }

  function selectAllExportOptions() {
    setSelectedExportOptions(exportOptions.map((option) => option.value))
  }

  function clearExportOptions() {
    setSelectedExportOptions([])
  }

  useEffect(() => {
    async function fetchData() {
      setLoading(true)
      const [r, c] = await Promise.all([
        getResumo(selectedYear),
        getComparativo(selectedYear),
      ])
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
        <div className="p-8 flex items-center justify-center min-h-100">
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

  async function handleExport(format: 'excel' | 'pdf') {
    if (
      !resumo ||
      selectedExportOptions.length === 0 ||
      selectedExportYears.length === 0
    ) {
      return
    }

    setIsExporting(true)
    try {
      const tables: ExportTable[] = []

      if (selectedExportOptions.includes('resumo')) {
        const data: Record<string, any>[] = []
        selectedExportYears.forEach((year) => {
          data.push({ indicador: 'Despesa Total', valor: resumo.totalEmpenhado, ano: year })
          data.push({ indicador: 'Receita Total', valor: resumo.totalLiquidado, ano: year })
          data.push({ indicador: 'Investimentos', valor: resumo.totalPago, ano: year })
        })

        tables.push({
          title: 'Resumo Geral',
          columns: [
            { header: 'Indicador', key: 'indicador', width: 30 },
            { header: 'Valor', key: 'valor', width: 30, isCurrency: true },
            { header: 'Ano', key: 'ano', width: 15 },
          ],
          data,
        })
      }

      if (selectedExportOptions.includes('evolucaoMensal')) {
        const data: Record<string, any>[] = []
        selectedExportYears.forEach((year) => {
          const currentKeyByYear = `ano${year}`
          monthlyData.forEach((item) => {
            const value = Number(item[currentKeyByYear as keyof typeof item] ?? 0)
            data.push({ mes: item.mes, valor: value * 1_000_000, ano: year })
          })
        })

        let imageBase64: string | undefined
        const element = document.getElementById('chart-evolucao-mensal')
        if (element) {
          imageBase64 = await toPng(element, { pixelRatio: 2, backgroundColor: '#ffffff' })
        }

        tables.push({
          title: 'Evolução Mensal',
          columns: [
            { header: 'Mês', key: 'mes', width: 20 },
            { header: 'Despesa', key: 'valor', width: 30, isCurrency: true },
            { header: 'Ano', key: 'ano', width: 15 },
          ],
          data,
          imageBase64,
        })
      }

      if (selectedExportOptions.includes('categorias')) {
        const data: Record<string, any>[] = []
        selectedExportYears.forEach((year) => {
          categoryData.forEach((item) => {
            data.push({ categoria: item.name, percentual: item.value, ano: year })
          })
        })

        let imageBase64: string | undefined
        const element = document.getElementById('chart-categorias')
        if (element) {
          imageBase64 = await toPng(element, { pixelRatio: 2, backgroundColor: '#ffffff' })
        }

        tables.push({
          title: 'Categorias',
          columns: [
            { header: 'Categoria', key: 'categoria', width: 30 },
            { header: 'Percentual', key: 'percentual', width: 20, isPercentage: true },
            { header: 'Ano', key: 'ano', width: 15 },
          ],
          data,
          imageBase64,
        })
      }

      if (selectedExportOptions.includes('orgaos')) {
        const data: Record<string, any>[] = []
        selectedExportYears.forEach((year) => {
          topOrgaos.forEach((orgao, index) => {
            data.push({
              posicao: index + 1,
              sigla: orgao.siglaOrgao,
              nome: orgao.nomeOrgao,
              codigo: orgao.codigoOrgao,
              empenhado: orgao.totalEmpenhado,
              liquidado: orgao.totalLiquidado,
              pago: orgao.totalPago,
              ano: year,
            })
          })
        })

        let imageBase64: string | undefined
        const element = document.getElementById('chart-orgaos')
        if (element) {
          imageBase64 = await toPng(element, { pixelRatio: 2, backgroundColor: '#ffffff' })
        }

        tables.push({
          title: 'Órgãos',
          columns: [
            { header: 'Posição', key: 'posicao', width: 10 },
            { header: 'Sigla', key: 'sigla', width: 15 },
            { header: 'Órgão', key: 'nome', width: 45 },
            { header: 'Código', key: 'codigo', width: 15 },
            { header: 'Total Empenhado', key: 'empenhado', width: 25, isCurrency: true },
            { header: 'Total Liquidado', key: 'liquidado', width: 25, isCurrency: true },
            { header: 'Total Pago', key: 'pago', width: 25, isCurrency: true },
            { header: 'Ano', key: 'ano', width: 10 },
          ],
          data,
          imageBase64,
        })
      }

      const filename = `custos-pe-${selectedExportYears.join('-')}`
      if (format === 'excel') {
        await exportToExcel(filename, tables)
      } else {
        await exportToPdf(filename, tables)
      }
    } finally {
      setIsExporting(false)
    }
  }

  return (
    <div className="pt-15 space-y-6">
      <DashboardHeader
        title="Visão Geral de Custos"
        subtitle="Visão consolidada das despesas do estado de Pernambuco"
      />
      <div className="p-8 space-y-6">
        <div className="flex justify-between">
          <div className="flex flex-col gap-1">
            <label className="text-xs font-medium text-gray-500">Ano</label>
            <select
              value={selectedYear}
              onChange={(event) => setSelectedYear(Number(event.target.value))}
              className="h-9 w-30 rounded-lg border border-gray-200 bg-white px-2 text-sm text-gray-700 outline-none transition focus:border-[#142F4B] focus:ring-2 focus:ring-[#142F4B]/10"
            >
              {[2026, 2025, 2024].map((year) => (
                <option key={year} value={year}>
                  {year}
                </option>
              ))}
            </select>
          </div>
          <div className="flex items-end gap-2">
            {/* Exportação */}
            <div
              ref={exportDropdownRef}
              className="relative flex flex-col gap-1"
            >
              <label className="text-xs font-medium text-gray-500">
                Exportação
              </label>

              <button
                type="button"
                onClick={() => setIsExportDropdownOpen((prev) => !prev)}
                className="flex h-9 min-w-50 items-center justify-between rounded-lg border border-gray-200 bg-white px-3 text-sm text-gray-700 outline-none transition hover:bg-gray-50 focus:border-[#142F4B] focus:ring-2 focus:ring-[#142F4B]/10"
              >
                <span className="truncate">
                  {selectedExportOptions.length === 0 ||
                  selectedExportYears.length === 0
                    ? 'Selecionar exportação'
                    : `${selectedExportOptions.length} info(s) • ${selectedExportYears.length} ano(s)`}
                </span>

                <span className="ml-2">
                  <ChevronDown className="w-4 text-gray-700" />
                </span>
              </button>

              {isExportDropdownOpen && (
                <div
                  className="absolute right-0 top-full z-9999 mt-2 w-[320px] rounded-xl border border-gray-200 bg-white p-4 shadow-lg"
                  onClick={(event) => event.stopPropagation()}
                >
                  <div className="mb-3 flex items-center justify-between border-b border-gray-100 pb-2">
                    <p className="text-sm font-semibold text-gray-900">
                      Exportar informações
                    </p>

                    <button
                      type="button"
                      onClick={() => {
                        clearExportOptions()
                        clearExportYears()
                      }}
                      className="text-xs font-medium text-red-500 hover:text-red-600"
                    >
                      Limpar
                    </button>
                  </div>

                  <div className="mb-4">
                    <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-gray-400">
                      Anos do relatório
                    </p>

                    <div className="space-y-1">
                      {exportYears.map((year) => (
                        <button
                          key={year}
                          type="button"
                          onClick={() => toggleExportYear(year)}
                          className="flex w-full items-center justify-between rounded-lg px-3 py-2 text-left text-sm text-gray-700 hover:bg-gray-50"
                        >
                          <span>{year}</span>

                          <span
                            className={`flex h-4 w-4 items-center justify-center rounded border text-[10px] ${
                              selectedExportYears.includes(year)
                                ? 'border-[#142F4B] bg-[#142F4B] text-white'
                                : 'border-gray-300 bg-white'
                            }`}
                          >
                            {selectedExportYears.includes(year) ? '✓' : ''}
                          </span>
                        </button>
                      ))}
                    </div>

                    <button
                      type="button"
                      onClick={selectAllExportYears}
                      className="mt-2 text-xs font-medium text-[#142F4B] hover:underline"
                    >
                      Selecionar todos os anos
                    </button>
                  </div>

                  <div className="border-t border-gray-100 pt-4">
                    <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-gray-400">
                      Informações
                    </p>

                    <div className="space-y-1">
                      {exportOptions.map((option) => (
                        <button
                          key={option.value}
                          type="button"
                          onClick={() => toggleExportOption(option.value)}
                          className="flex w-full items-center justify-between rounded-lg px-3 py-2 text-left text-sm text-gray-700 hover:bg-gray-50"
                        >
                          <span>{option.label}</span>

                          <span
                            className={`flex h-4 w-4 items-center justify-center rounded border text-[10px] ${
                              selectedExportOptions.includes(option.value)
                                ? 'border-[#142F4B] bg-[#142F4B] text-white'
                                : 'border-gray-300 bg-white'
                            }`}
                          >
                            {selectedExportOptions.includes(option.value)
                              ? '✓'
                              : ''}
                          </span>
                        </button>
                      ))}
                    </div>

                    <button
                      type="button"
                      onClick={selectAllExportOptions}
                      className="mt-2 text-xs font-medium text-[#142F4B] hover:underline"
                    >
                      Selecionar todas as informações
                    </button>
                  </div>
                </div>
              )}
            </div>

            {/* Botões Exportar */}
            <div className="flex items-center gap-2">
              <button
                type="button"
                onClick={() => handleExport('excel')}
                disabled={
                  isExporting ||
                  selectedExportOptions.length === 0 ||
                  selectedExportYears.length === 0
                }
                className="cursor-pointer inline-flex h-9 items-center gap-2 rounded-lg bg-[#142F4B] px-4 text-sm font-semibold text-white transition hover:bg-[#0f243a] disabled:cursor-not-allowed disabled:bg-gray-400"
              >
                <Download className="h-4 w-4" />
                {isExporting ? 'Gerando...' : 'Excel'}
              </button>
              <button
                type="button"
                onClick={() => handleExport('pdf')}
                disabled={
                  isExporting ||
                  selectedExportOptions.length === 0 ||
                  selectedExportYears.length === 0
                }
                className="cursor-pointer inline-flex h-9 items-center gap-2 rounded-lg bg-[#008C6C] px-4 text-sm font-semibold text-white transition hover:bg-[#007258] disabled:cursor-not-allowed disabled:bg-gray-400"
              >
                <Download className="h-4 w-4" />
                {isExporting ? 'Gerando...' : 'PDF'}
              </button>
            </div>
          </div>
        </div>
        {/* KPI Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
          <KpiCard
            title="Despesa Total"
            value={formatCurrency(resumo.totalEmpenhado)}
            icon={<TrendingUp className="h-5 w-5" />}
            trend={{ value: '+ 7,5%', positive: true }}
            info="Total de despesas empenhadas (comprometidas) no período selecionado."
            subtitle="vs ano anterior"
          />
          <KpiCard
            title="Receita Total"
            value={formatCurrency(resumo.totalLiquidado)}
            icon={<DollarSign className="h-5 w-5" />}
            trend={{ value: '+ 7,5%', positive: true }}
            info="Total arrecadado pelo governo no período selecionado."
            subtitle="vs ano anterior"
          />
          <KpiCard
            title="Investimentos"
            value={formatCurrency(resumo.totalPago)}
            icon={<PiggyBank className="h-5 w-5" />}
            trend={{ value: '+ 7,5%', positive: true }}
            info="Total destinado a obras, infraestrutura, saúde, educação e outros projetos públicos."
            subtitle="vs ano anterior"
          />
        </div>

        {/* Charts Row */}
        <div className="grid grid-cols-1 lg:grid-cols-5 gap-5">
          {/* Evolução Mensal */}
          <div id="chart-evolucao-mensal" className="lg:col-span-3 bg-white rounded-xl border border-gray-200 p-6">
            <div className="flex items-center justify-between mb-1">
              <div>
                <div className="flex items-center gap-2">
                  <h3 className="font-semibold text-gray-900">
                    Evolução mensal de despesas
                  </h3>

                  <div className="group relative inline-flex">
                    <button
                      type="button"
                      className="text-gray-400 transition-colors hover:text-gray-600"
                      aria-label="Informação sobre evolução mensal de despesas"
                    >
                      <Info className="h-4 w-4" />
                    </button>

                    <div className="pointer-events-none absolute left-1/2 top-6 z-50 w-64 -translate-x-1/2 rounded-lg bg-gray-900 px-3 py-2 text-xs font-normal text-white opacity-0 shadow-lg transition-opacity group-hover:opacity-100">
                      Acompanhamento da evolução mensal dos gastos e comparação direta com o ano anterior.
                      <div className="absolute -top-1 left-1/2 h-2 w-2 -translate-x-1/2 rotate-45 bg-gray-900" />
                    </div>
                  </div>
                </div>
                <p className="text-sm text-gray-500">
                  Comparativo do ano atual (em milhões R$)
                </p>
              </div>
            </div>
            <div className="h-70 mt-4 bg-white">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart
                  data={monthlyData}
                  margin={{ top: 5, right: 20, left: 0, bottom: 5 }}
                >
                  <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                  <XAxis
                    dataKey="mes"
                    tick={{ fontSize: 11, fill: '#94a3b8' }}
                    axisLine={false}
                    tickLine={false}
                  />
                  <YAxis
                    tick={{ fontSize: 11, fill: '#94a3b8' }}
                    axisLine={false}
                    tickLine={false}
                  />
                  <Tooltip
                    contentStyle={{
                      background: 'white',
                      border: '1px solid #e2e8f0',
                      borderRadius: '8px',
                      fontSize: '12px',
                      boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)',
                    }}
                  />
                  <Legend
                    iconType="circle"
                    iconSize={8}
                    wrapperStyle={{ fontSize: '12px' }}
                  />
                  <Line
                    type="monotone"
                    dataKey={currentKey}
                    name={String(selectedYear)}
                    stroke="#008C6C"
                    strokeWidth={2}
                    dot={{
                      r: 4,
                      fill: '#008C6C',
                      strokeWidth: 2,
                      stroke: 'white',
                    }}
                    activeDot={{ r: 6 }}
                  />
                  <Line
                    type="monotone"
                    dataKey={previousKey}
                    name={String(selectedYear - 1)}
                    stroke="#3B82F6"
                    strokeWidth={2}
                    dot={{
                      r: 4,
                      fill: '#3B82F6',
                      strokeWidth: 2,
                      stroke: 'white',
                    }}
                    activeDot={{ r: 6 }}
                    strokeDasharray="6 3"
                  />
                </LineChart>
              </ResponsiveContainer>
            </div>
          </div>

          {/* Distribuição por Categorias */}
          <div id="chart-categorias" className="lg:col-span-2 bg-white rounded-xl border border-gray-200 p-6">
            <div className="flex items-center justify-between mb-1">
              <div>
                <div className="flex items-center gap-2">
                  <h3 className="font-semibold text-gray-900">
                    Distribuição por categorias
                  </h3>

                  <div className="group relative inline-flex">
                    <button
                      type="button"
                      className="text-gray-400 transition-colors hover:text-gray-600"
                      aria-label="Informação sobre distribuição por categorias"
                    >
                      <Info className="h-4 w-4" />
                    </button>

                    <div className="pointer-events-none absolute left-1/2 top-6 z-50 w-64 -translate-x-1/2 rounded-lg bg-gray-900 px-3 py-2 text-xs font-normal text-white opacity-0 shadow-lg transition-opacity group-hover:opacity-100">
                      Composição das despesas dividida por categoria, evidenciando a concentração dos recursos.
                      <div className="absolute -top-1 left-1/2 h-2 w-2 -translate-x-1/2 rotate-45 bg-gray-900" />
                    </div>
                  </div>
                </div>
                <p className="text-sm text-gray-500">
                  Composição das despesas no ano atual
                </p>
              </div>
            </div>
            <div className="h-50 mt-4 bg-white">
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
                <div
                  key={cat.name}
                  className="flex items-center justify-between text-sm"
                >
                  <div className="flex items-center gap-2">
                    <span
                      className="w-2.5 h-2.5 rounded-full"
                      style={{ backgroundColor: cat.color }}
                    />
                    <span className="text-gray-600">{cat.name}</span>
                  </div>
                  <span className="text-gray-500 font-medium">
                    {cat.value}%
                  </span>
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* Maiores Órgãos por Despesa */}
        <div id="chart-orgaos" className="bg-white rounded-xl border border-gray-200 p-6">
          <div className="mb-1">
            <div className="flex items-center gap-2">
              <h3 className="font-semibold text-gray-900">
                Maiores Órgãos por Despesa
              </h3>

              <div className="group relative inline-flex">
                <button
                  type="button"
                  className="text-gray-400 transition-colors hover:text-gray-600"
                  aria-label="Informação sobre maiores órgãos por despesa"
                >
                  <Info className="h-4 w-4" />
                </button>

                <div className="pointer-events-none absolute left-1/2 top-6 z-50 w-72 -translate-x-1/2 rounded-lg bg-gray-900 px-3 py-2 text-xs font-normal text-white opacity-0 shadow-lg transition-opacity group-hover:opacity-100">
                  Ranking dos órgãos com maior volume de despesas dentro do orçamento.
                  <div className="absolute -top-1 left-1/2 h-2 w-2 -translate-x-1/2 rotate-45 bg-gray-900" />
                </div>
              </div>
            </div>
            <p className="text-sm text-gray-500">
              Top 10 órgão com maior volume de despesas no ano atual
            </p>
          </div>
          <div className="h-95 mt-4 bg-white">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart
                data={orgaoBars}
                layout="vertical"
                margin={{ top: 5, right: 30, left: 10, bottom: 5 }}
              >
                <CartesianGrid
                  strokeDasharray="3 3"
                  stroke="#f0f0f0"
                  horizontal={false}
                />
                <XAxis
                  type="number"
                  tick={{ fontSize: 11, fill: '#94a3b8' }}
                  axisLine={false}
                  tickLine={false}
                />
                <YAxis
                  type="category"
                  dataKey="sigla"
                  tick={{ fontSize: 12, fill: '#475569', fontWeight: 500 }}
                  axisLine={false}
                  tickLine={false}
                  width={60}
                />
                <Tooltip
                  formatter={(value) => {
                    const numericValue =
                      typeof value === 'number' ? value : Number(value)
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
                <Legend
                  iconType="square"
                  iconSize={10}
                  wrapperStyle={{ fontSize: '12px' }}
                />
                <Bar
                  dataKey="valor"
                  name="Órgãos"
                  fill="#2d4a63"
                  radius={[0, 4, 4, 0]}
                  barSize={18}
                />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>
    </div>
  )
}
