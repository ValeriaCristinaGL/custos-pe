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
import * as XLSX from 'xlsx'
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

  function handleExportExcel() {
    if (
      selectedExportOptions.length === 0 ||
      selectedExportYears.length === 0
    ) {
      return
    }

    const rows: Array<Array<string | number>> = []
    const currencyCells: string[] = []
    const currencyFormat = '"R$" #,##0.00'

    const addEmptyLine = () => {
      rows.push([])
    }

    const addTitle = (title: string) => {
      rows.push([title])
    }

    const addCurrencyCell = (rowIndex: number, columnIndex: number) => {
      const cellAddress = XLSX.utils.encode_cell({
        r: rowIndex,
        c: columnIndex,
      })

      currencyCells.push(cellAddress)
    }

    if (selectedExportOptions.includes('indicadores')) {
      addTitle('Indicadores principais')
      rows.push(['Indicador', 'Valor', 'Descrição', 'Ano'])

      selectedExportYears.forEach((year) => {
        rows.push([
          'Crescimento anual médio',
          '47,9%',
          'Média dos últimos 5 anos',
          year,
        ])

        rows.push(['Pico sazonal', 'Janeiro', 'Índice sazonal de 116%', year])

        rows.push(['Tendência 2026', '+3,4%', 'Projeção de crescimento', year])
      })

      addEmptyLine()
    }

    if (selectedExportOptions.includes('despesasTrimestrais')) {
      addTitle('Despesas Trimestrais')
      rows.push(['Trimestre', 'Despesa', 'Ano'])

      selectedExportYears.forEach((year) => {
        const yearKey = `ano${year}`

        MOCK_QUARTERLY.forEach((item) => {
          const value = Number(item[yearKey as keyof typeof item] ?? 0)

          if (value > 0) {
            const rowIndex = rows.length

            rows.push([item.trimestre, value * 1_000_000, year])

            addCurrencyCell(rowIndex, 1)
          }
        })
      })

      addEmptyLine()
    }

    if (selectedExportOptions.includes('crescimentoAnual')) {
      addTitle('Crescimento anual')
      rows.push(['Ano', 'Crescimento'])

      MOCK_ANNUAL_GROWTH.forEach((item) => {
        if (selectedExportYears.includes(item.ano as unknown as ExportYear)) {
          rows.push([item.ano, `${item.valor}%`])
        }
      })

      addEmptyLine()
    }

    if (selectedExportOptions.includes('sazonalidade')) {
      addTitle('Índice de sazonalidade')
      rows.push(['Mês', 'Índice', 'Ano'])

      selectedExportYears.forEach((year) => {
        MOCK_SEASONALITY.forEach((item) => {
          rows.push([item.mes, item.indice, year])
        })
      })

      addEmptyLine()
    }

    if (selectedExportOptions.includes('comparativoMensal')) {
      addTitle('Comparativo mensal por ano')
      rows.push(['Mês', 'Despesa', 'Ano'])

      selectedExportYears.forEach((year) => {
        const yearKey = `ano${year}`

        MOCK_MONTHLY_COMPARISON.forEach((item) => {
          const value = Number(item[yearKey as keyof typeof item] ?? 0)

          if (value > 0) {
            const rowIndex = rows.length

            rows.push([item.mes, value * 1_000_000, year])

            addCurrencyCell(rowIndex, 1)
          }
        })
      })

      addEmptyLine()
    }

    const workbook = XLSX.utils.book_new()
    const worksheet = XLSX.utils.aoa_to_sheet(rows)

    currencyCells.forEach((cellAddress) => {
      if (worksheet[cellAddress]) {
        worksheet[cellAddress].t = 'n'
        worksheet[cellAddress].z = currencyFormat
      }
    })

    worksheet['!cols'] = [{ wch: 28 }, { wch: 22 }, { wch: 36 }, { wch: 12 }]

    XLSX.utils.book_append_sheet(workbook, worksheet, 'Relatório')

    XLSX.writeFile(
      workbook,
      `analise-temporal-${selectedExportYears.join('-')}.xlsx`
    )
  }

  const infoTexts = {
    crescimentoAnualMedio:
      'Mostra quanto as despesas cresceram, em média, nos últimos anos. Esse indicador ajuda a entender se os gastos estão aumentando, diminuindo ou ficando mais estáveis ao longo do tempo.',

    picoSazonal:
      'Mostra o mês em que as despesas costumam ser mais altas. Isso ajuda a identificar períodos do ano em que o governo geralmente gasta mais.',

    tendencia2026:
      'Mostra uma estimativa de como as despesas podem se comportar em 2026. Essa projeção ajuda a perceber se a tendência é de crescimento, queda ou estabilidade nos gastos.',

    despesasTrimestrais:
      'Mostra o total de despesas por trimestre. Cada trimestre representa um período de três meses, facilitando a comparação dos gastos ao longo do ano.',

    crescimentoAnual:
      'Mostra a variação das despesas de um ano para o outro. Esse gráfico ajuda a entender em quais anos os gastos cresceram mais ou tiveram redução.',

    indiceSazonalidade:
      'Mostra em quais meses as despesas costumam ficar acima ou abaixo da média. O índice 100 representa a média mensal: valores acima de 100 indicam gastos maiores que a média, e valores abaixo indicam gastos menores.',

    comparativoMensalPorAno:
      'Compara as despesas mês a mês entre os anos apresentados. Esse gráfico ajuda a ver se, em determinado mês, o gasto foi maior ou menor em relação ao mesmo período de outro ano.',
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

          <button
            type="button"
            onClick={handleExportExcel}
            disabled={
              selectedExportOptions.length === 0 ||
              selectedExportYears.length === 0
            }
            className="inline-flex h-10 items-center gap-2 rounded-lg bg-[#142F4B] px-5 text-sm font-semibold text-white transition hover:bg-[#0f243a] disabled:cursor-not-allowed disabled:bg-gray-400"
          >
            <Download className="h-4 w-4" />
            Exportar
          </button>
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
          <div className="bg-white rounded-xl border border-gray-200 p-6">
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
            <div className="h-60">
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
          <div className="bg-white rounded-xl border border-gray-200 p-6">
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
            <div className="h-60">
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
        <div className="bg-white rounded-xl border border-gray-200 p-6">
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
          <div className="h-50 mt-4">
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
        <div className="bg-white rounded-xl border border-gray-200 p-6">
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
          <div className="h-70 mt-4">
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
