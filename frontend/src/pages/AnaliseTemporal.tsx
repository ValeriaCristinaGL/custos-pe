import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend,
  BarChart,
  Bar,
} from 'recharts'
import { useEffect, useRef, useState } from 'react'
import { TrendingUp, Calendar, Info, Download } from 'lucide-react'
import {
  exportToExcel,
  exportToPdf,
  type ExportTable,
} from '../services/ExportService'
import { toPng } from 'html-to-image'
import { DashboardHeader } from './Dashboard'
import { KpiCard } from '../components/KpiCard'
import {
  MOCK_QUARTERLY,
  MOCK_ANNUAL_GROWTH,
  MOCK_SEASONALITY,
  MOCK_MONTHLY_COMPARISON,
} from '../api'

type ExportYear = 2026 | 2025 | 2024

const exportYears: ExportYear[] = [2026, 2025, 2024]

type ExportOption =
  | 'indicadores'
  | 'despesasTrimestrais'
  | 'crescimentoAnual'
  | 'sazonalidade'
  | 'comparativoMensal'

const exportOptions: { label: string; value: ExportOption }[] = [
  { label: 'Indicadores principais', value: 'indicadores' },
  { label: 'Despesas Trimestrais', value: 'despesasTrimestrais' },
  { label: 'Crescimento anual', value: 'crescimentoAnual' },
  { label: 'Índice de sazonalidade', value: 'sazonalidade' },
  { label: 'Comparativo mensal por ano', value: 'comparativoMensal' },
]

function InfoTooltip({ text, label }: { text: string; label: string }) {
  return (
    <span
      className="group relative inline-flex"
      onClick={(event) => event.stopPropagation()}
      onMouseDown={(event) => event.stopPropagation()}
    >
      <span
        role="img"
        aria-label={label}
        className="inline-flex h-5 w-5 items-center justify-center rounded-full text-gray-400 transition-colors hover:bg-gray-100 hover:text-gray-600"
      >
        <Info className="h-4 w-4" />
      </span>

      <span className="pointer-events-none absolute left-1/2 top-6 z-9999 w-72 -translate-x-1/2 rounded-lg bg-gray-900 px-3 py-2 text-xs font-normal leading-relaxed text-white opacity-0 shadow-lg transition-opacity group-hover:opacity-100">
        {text}

        <span className="absolute -top-1 left-1/2 h-2 w-2 -translate-x-1/2 rotate-45 bg-gray-900" />
      </span>
    </span>
  )
}

export function AnaliseTemporal() {
  const exportDropdownRef = useRef<HTMLDivElement | null>(null)

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

  async function handleExport(format: 'excel' | 'pdf') {
    if (
      selectedExportOptions.length === 0 ||
      selectedExportYears.length === 0
    ) {
      return
    }

    setIsExporting(true)
    try {
      const tables: ExportTable[] = []

      if (selectedExportOptions.includes('indicadores')) {
        const data: Record<string, unknown>[] = []
        selectedExportYears.forEach((year) => {
          data.push({
            indicador: 'Crescimento anual médio',
            valor: '47,9%',
            descricao: 'Média dos últimos 5 anos',
            ano: year,
          })
          data.push({
            indicador: 'Pico sazonal',
            valor: 'Janeiro',
            descricao: 'Índice sazonal de 116%',
            ano: year,
          })
          data.push({
            indicador: 'Tendência 2026',
            valor: '+3,4%',
            descricao: 'Projeção de crescimento',
            ano: year,
          })
        })

        tables.push({
          title: 'Indicadores Principais',
          columns: [
            { header: 'Indicador', key: 'indicador', width: 25 },
            { header: 'Valor', key: 'valor', width: 15 },
            { header: 'Descrição', key: 'descricao', width: 35 },
            { header: 'Ano', key: 'ano', width: 10 },
          ],
          data,
        })
      }

      if (selectedExportOptions.includes('despesasTrimestrais')) {
        const data: Record<string, unknown>[] = []
        selectedExportYears.forEach((year) => {
          const yearKey = `ano${year}`
          MOCK_QUARTERLY.forEach((item) => {
            const value = Number(item[yearKey as keyof typeof item] ?? 0)
            if (value > 0) {
              data.push({
                trimestre: item.trimestre,
                despesa: value * 1_000_000,
                ano: year,
              })
            }
          })
        })

        let imageBase64: string | undefined
        const element = document.getElementById('chart-despesas-trimestrais')
        if (element) {
          imageBase64 = await toPng(element, {
            pixelRatio: 2,
            backgroundColor: '#ffffff',
          })
        }

        tables.push({
          title: 'Despesas Trimestrais',
          columns: [
            { header: 'Trimestre', key: 'trimestre', width: 20 },
            { header: 'Despesa', key: 'despesa', width: 30, isCurrency: true },
            { header: 'Ano', key: 'ano', width: 15 },
          ],
          data,
          imageBase64,
        })
      }

      if (selectedExportOptions.includes('crescimentoAnual')) {
        const data: Record<string, unknown>[] = []
        MOCK_ANNUAL_GROWTH.forEach((item) => {
          if (selectedExportYears.includes(item.ano as unknown as ExportYear)) {
            data.push({ ano: item.ano, crescimento: item.valor })
          }
        })

        let imageBase64: string | undefined
        const element = document.getElementById('chart-crescimento-anual')
        if (element) {
          imageBase64 = await toPng(element, {
            pixelRatio: 2,
            backgroundColor: '#ffffff',
          })
        }

        tables.push({
          title: 'Crescimento Anual',
          columns: [
            { header: 'Ano', key: 'ano', width: 20 },
            {
              header: 'Crescimento',
              key: 'crescimento',
              width: 20,
              isPercentage: true,
            },
          ],
          data,
          imageBase64,
        })
      }

      if (selectedExportOptions.includes('sazonalidade')) {
        const data: Record<string, unknown>[] = []
        selectedExportYears.forEach((year) => {
          MOCK_SEASONALITY.forEach((item) => {
            data.push({ mes: item.mes, indice: item.indice, ano: year })
          })
        })

        let imageBase64: string | undefined
        const element = document.getElementById('chart-sazonalidade')
        if (element) {
          imageBase64 = await toPng(element, {
            pixelRatio: 2,
            backgroundColor: '#ffffff',
          })
        }

        tables.push({
          title: 'Índice de Sazonalidade',
          columns: [
            { header: 'Mês', key: 'mes', width: 20 },
            { header: 'Índice', key: 'indice', width: 15 },
            { header: 'Ano', key: 'ano', width: 15 },
          ],
          data,
          imageBase64,
        })
      }

      if (selectedExportOptions.includes('comparativoMensal')) {
        const data: Record<string, unknown>[] = []
        selectedExportYears.forEach((year) => {
          const yearKey = `ano${year}`
          MOCK_MONTHLY_COMPARISON.forEach((item) => {
            const value = Number(item[yearKey as keyof typeof item] ?? 0)
            if (value > 0) {
              data.push({
                mes: item.mes,
                despesa: value * 1_000_000,
                ano: year,
              })
            }
          })
        })

        let imageBase64: string | undefined
        const element = document.getElementById('chart-comparativo-mensal')
        if (element) {
          imageBase64 = await toPng(element, {
            pixelRatio: 2,
            backgroundColor: '#ffffff',
          })
        }

        tables.push({
          title: 'Comparativo Mensal',
          columns: [
            { header: 'Mês', key: 'mes', width: 20 },
            { header: 'Despesa', key: 'despesa', width: 30, isCurrency: true },
            { header: 'Ano', key: 'ano', width: 15 },
          ],
          data,
          imageBase64,
        })
      }

      const filename = `analise-temporal-${selectedExportYears.join('-')}`
      if (format === 'excel') {
        await exportToExcel(filename, tables)
      } else {
        await exportToPdf(filename, tables)
      }
    } finally {
      setIsExporting(false)
    }
  }

  const infoTexts = {
    crescimentoAnualMedio:
      'Média de crescimento das despesas nos últimos anos.',

    picoSazonal: 'Mês com histórico de maiores despesas.',

    tendencia2026: 'Projeção do comportamento das despesas para 2026.',

    despesasTrimestrais: 'Total de despesas acumuladas por trimestre.',

    crescimentoAnual: 'Variação percentual das despesas entre os anos.',

    indiceSazonalidade:
      'Comportamento mensal em relação à média do ano (Base 100).',

    comparativoMensalPorAno:
      'Comparação das despesas mensais entre diferentes anos.',
  }

  return (
    <div className="pt-15 space-y-6">
      <DashboardHeader
        title="Análise Temporal"
        subtitle="Evolução e tendências das despesas ao longo do tempo"
      />
      <div className="p-8 space-y-6">
        <div className="flex items-end justify-end gap-4">
          <div ref={exportDropdownRef} className="relative flex flex-col gap-1">
            <label className="text-xs font-medium text-gray-500">
              Exportação
            </label>

            <button
              type="button"
              onClick={() => setIsExportDropdownOpen((prev) => !prev)}
              className="flex h-10 min-w-50 items-center justify-between rounded-lg border border-gray-200 bg-white px-3 text-sm text-gray-700 outline-none transition hover:bg-gray-50 focus:border-[#142F4B] focus:ring-2 focus:ring-[#142F4B]/10"
            >
              <span className="truncate">
                {selectedExportOptions.length === 0 ||
                selectedExportYears.length === 0
                  ? 'Selecionar exportação'
                  : `${selectedExportOptions.length} info(s) • ${selectedExportYears.length} ano(s)`}
              </span>

              <span className="ml-2 text-gray-400">▾</span>
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
        {/* KPI Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
          <KpiCard
            title="Crescimento anual médio"
            value="47,9%"
            info={infoTexts.crescimentoAnualMedio}
            icon={<TrendingUp className="h-5 w-5" />}
            subtitle="Média dos últimos 5 anos"
          />
          <KpiCard
            title="Pico sazonal"
            value="Janeiro"
            info={infoTexts.picoSazonal}
            icon={<Calendar className="h-5 w-5" />}
            subtitle="Índice sazonal de 116%"
          />
          <div className="bg-white rounded-xl border border-gray-200 p-6 flex flex-col gap-3 hover:shadow-md transition-shadow duration-300">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <p className="text-sm font-medium text-gray-500">
                  Tendência 2026
                </p>

                <InfoTooltip
                  label="Informação sobre tendência 2026"
                  text={infoTexts.tendencia2026}
                />
              </div>
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
          <div
            id="chart-despesas-trimestrais"
            className="bg-white rounded-xl border border-gray-200 p-6"
          >
            <div className="flex items-center gap-2 mb-1">
              <div className="mb-1 flex items-center gap-2">
                <h3 className="font-semibold text-gray-900">
                  Despesas Trimestrais
                </h3>

                <InfoTooltip
                  label="Informação sobre despesas trimestrais"
                  text={infoTexts.despesasTrimestrais}
                />
              </div>
            </div>
            <p className="text-sm text-gray-500 mb-4">
              Evolução trimestral dos últimos 2 anos (em milhões R$)
            </p>
            <div className="h-60 bg-white">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart
                  data={MOCK_QUARTERLY}
                  margin={{ top: 5, right: 20, left: 0, bottom: 5 }}
                >
                  <CartesianGrid
                    strokeDasharray="3 3"
                    stroke="#f0f0f0"
                    vertical={false}
                  />
                  <XAxis
                    dataKey="trimestre"
                    tick={{ fontSize: 12, fill: '#64748b' }}
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
                  <Bar
                    dataKey="ano2025"
                    name="2025"
                    fill="#008C6C"
                    radius={[4, 4, 0, 0]}
                    barSize={24}
                  />
                  <Bar
                    dataKey="ano2026"
                    name="2026"
                    fill="#142F4B"
                    radius={[4, 4, 0, 0]}
                    barSize={24}
                  />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>

          {/* Crescimento Anual */}
          <div
            id="chart-crescimento-anual"
            className="bg-white rounded-xl border border-gray-200 p-6"
          >
            <div>
              <div className="flex items-center gap-2">
                <h3 className="font-semibold text-gray-900">
                  Crescimento anual
                </h3>

                <InfoTooltip
                  label="Informação sobre crescimento anual"
                  text={infoTexts.crescimentoAnual}
                />
              </div>
              <p className="text-sm text-gray-500 mb-4">
                Taxa de crescimento da despesa total por ano
              </p>
            </div>
            <div className="h-60 bg-white">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart
                  data={MOCK_ANNUAL_GROWTH}
                  margin={{ top: 5, right: 20, left: 0, bottom: 5 }}
                >
                  <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                  <XAxis
                    dataKey="ano"
                    tick={{ fontSize: 12, fill: '#64748b' }}
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
                  <Line
                    type="monotone"
                    dataKey="valor"
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
                </LineChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>

        {/* Índice de Sazonalidade */}
        <div
          id="chart-sazonalidade"
          className="bg-white rounded-xl border border-gray-200 p-6"
        >
          <div className="mb-1">
            <div className="flex items-center gap-2">
              <h3 className="font-semibold text-gray-900">
                Índice de sazonalidade
              </h3>

              <InfoTooltip
                label="Informação sobre índice de sazonalidade"
                text={infoTexts.indiceSazonalidade}
              />
            </div>
            <p className="text-sm text-gray-500">
              {' '}
              Padrão de gastos ao longo do ano (índice 100 = média mensal)
            </p>
          </div>
          <div className="h-50 mt-4 bg-white">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart
                data={MOCK_SEASONALITY}
                margin={{ top: 5, right: 20, left: 0, bottom: 5 }}
              >
                <CartesianGrid
                  strokeDasharray="3 3"
                  stroke="#f0f0f0"
                  vertical={false}
                />
                <XAxis
                  dataKey="mes"
                  tick={{ fontSize: 12, fill: '#64748b' }}
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
                <Bar
                  dataKey="indice"
                  fill="#142F4B"
                  radius={[4, 4, 0, 0]}
                  barSize={32}
                />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Comparativo Mensal por Ano */}
        <div
          id="chart-comparativo-mensal"
          className="bg-white rounded-xl border border-gray-200 p-6"
        >
          <div className="mb-1">
            <div className="flex items-center gap-2">
              <h3 className="font-semibold text-gray-900">
                Comparativo mensal por ano
              </h3>

              <InfoTooltip
                label="Informação sobre comparativo mensal por ano"
                text={infoTexts.comparativoMensalPorAno}
              />
            </div>
            <p className="text-sm text-gray-500">
              Despesas mensais dos últimos 2 anos (em milhões R$)
            </p>
          </div>
          <div className="h-70 mt-4 bg-white">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart
                data={MOCK_MONTHLY_COMPARISON}
                margin={{ top: 5, right: 20, left: 0, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                <XAxis
                  dataKey="mes"
                  tick={{ fontSize: 12, fill: '#64748b' }}
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
                  dataKey="ano2025"
                  name="2025"
                  stroke="#008C6C"
                  strokeWidth={2}
                  dot={{
                    r: 4,
                    fill: '#008C6C',
                    strokeWidth: 2,
                    stroke: 'white',
                  }}
                />
                <Line
                  type="monotone"
                  dataKey="ano2026"
                  name="2026"
                  stroke="#94a3b8"
                  strokeWidth={2}
                  dot={{
                    r: 4,
                    fill: '#94a3b8',
                    strokeWidth: 2,
                    stroke: 'white',
                  }}
                  strokeDasharray="6 3"
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>
    </div>
  )
}
