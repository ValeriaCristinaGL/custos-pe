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
  RadarChart,
  PolarGrid,
  PolarAngleAxis,
  PolarRadiusAxis,
  Radar,
} from 'recharts'
import { TrendingUp, Info, Download, ChevronDown } from 'lucide-react'
import * as XLSX from 'xlsx'
import { DashboardHeader } from './Dashboard'
import {
  getComparativo,
  getEvolucao,
  buildOrgaoEvolution,
  buildRadarData,
  type ComparativoOrgaos,
  type DrillDown,
} from '../api'

type ExportYear = 2026 | 2025 | 2024

const exportYears: ExportYear[] = [2026, 2025, 2024]

type ExportOption =
  | 'evolucaoComparativa'
  | 'perfilGastos'
  | 'detalhamentoOrgao'
  | 'drillDownOrgao'

const exportOptions: { label: string; value: ExportOption }[] = [
  { label: 'Evolução Comparativa', value: 'evolucaoComparativa' },
  { label: 'Perfil de Gastos', value: 'perfilGastos' },
  { label: 'Detalhamento por Órgão', value: 'detalhamentoOrgao' },
  { label: 'Drill-down por órgão', value: 'drillDownOrgao' },
]

function formatCurrency(value: number): string {
  if (value >= 1_000_000_000)
    return `R$ ${(value / 1_000_000_000).toFixed(1)} bi`
  if (value >= 1_000_000) return `R$ ${(value / 1_000_000).toFixed(1)} mi`
  return `R$ ${value.toFixed(2)}`
}

function formatCurrencyFull(value: number): string {
  return value.toLocaleString('pt-BR', {
    style: 'currency',
    currency: 'BRL',
    minimumFractionDigits: 0,
  })
}

function clampValue(value: number, min: number, max: number): number {
  return Math.max(min, Math.min(max, value))
}

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

export function ComparacaoOrgaos() {
  const [comparativo, setComparativo] = useState<ComparativoOrgaos | null>(null)
  const [loading, setLoading] = useState(true)
  const [selectedYear, setSelectedYear] = useState(2026)
  const [selectedOrgaoCode, setSelectedOrgaoCode] = useState<string | null>(
    null
  )
  const [drillDown, setDrillDown] = useState<DrillDown | null>(null)
  const [drillLoading, setDrillLoading] = useState(false)
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

  useEffect(() => {
    async function fetchData() {
      setLoading(true)
      const data = await getComparativo(selectedYear)
      setComparativo(data)
      if (
        !selectedOrgaoCode ||
        !data.orgaos.some((orgao) => orgao.codigoOrgao === selectedOrgaoCode)
      ) {
        setSelectedOrgaoCode(data.orgaos[0]?.codigoOrgao ?? null)
      }
      setLoading(false)
    }
    fetchData()
  }, [selectedOrgaoCode, selectedYear])

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
        <div className="p-8 flex items-center justify-center min-h-100">
          <div className="loader" />
        </div>
      </>
    )
  }

  const topOrgaos = comparativo.orgaos.slice(0, 3)
  const tableOrgaos = comparativo.orgaos.slice(0, 5)
  const selectedOrgao = comparativo.orgaos.find(
    (orgao) => orgao.codigoOrgao === selectedOrgaoCode
  )
  const baseEvolution = buildOrgaoEvolution(selectedYear)
  const baseRadar = buildRadarData(selectedYear)
  const primaryOrgao = selectedOrgao ?? comparativo.orgaos[0]
  const orgaosToPlot = primaryOrgao
    ? [
        primaryOrgao,
        ...comparativo.orgaos
          .filter((orgao) => orgao.codigoOrgao !== primaryOrgao.codigoOrgao)
          .slice(0, 2),
      ]
    : []
  const baseKeys = ['SEE', 'SES', 'SEINFRA'] as const
  const colorPalette = ['#008C6C', '#3B82F6', '#C79E41']

  const evolutionData = baseEvolution.map((item) => {
    const row: Record<string, number | string> = { mes: item.mes }
    orgaosToPlot.forEach((orgao, index) => {
      const baseKey = baseKeys[index % baseKeys.length]
      const baseTotal =
        comparativo.orgaos.find((entry) => entry.codigoOrgao === baseKey)
          ?.totalEmpenhado ??
        primaryOrgao?.totalEmpenhado ??
        1
      const scale = baseTotal > 0 ? orgao.totalEmpenhado / baseTotal : 1
      row[orgao.siglaOrgao] = Math.round(item[baseKey] * scale)
    })
    return row
  })

  const radarData = baseRadar.map((item) => {
    const row: Record<string, number | string> = { subject: item.subject }
    orgaosToPlot.forEach((orgao, index) => {
      const baseKey = baseKeys[index % baseKeys.length]
      const baseTotal =
        comparativo.orgaos.find((entry) => entry.codigoOrgao === baseKey)
          ?.totalEmpenhado ??
        primaryOrgao?.totalEmpenhado ??
        1
      const scale = baseTotal > 0 ? orgao.totalEmpenhado / baseTotal : 1
      row[orgao.siglaOrgao] = clampValue(
        Math.round(item[baseKey] * scale),
        20,
        100
      )
    })
    return row
  })

  async function handleExportExcel() {
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

    const comparativosByYear = await Promise.all(
      selectedExportYears.map(async (year) => {
        const data = await getComparativo(year)
        return { year, data }
      })
    )

    if (selectedExportOptions.includes('evolucaoComparativa')) {
      addTitle('Evolução Comparativa')
      rows.push(['Mês', 'Órgão', 'Despesa', 'Ano'])

      comparativosByYear.forEach(({ year, data }) => {
        const selectedOrgaoByYear =
          data.orgaos.find(
            (orgao) => orgao.codigoOrgao === selectedOrgaoCode
          ) ?? data.orgaos[0]

        const orgaosToExport = selectedOrgaoByYear
          ? [
              selectedOrgaoByYear,
              ...data.orgaos
                .filter(
                  (orgao) =>
                    orgao.codigoOrgao !== selectedOrgaoByYear.codigoOrgao
                )
                .slice(0, 2),
            ]
          : []

        const baseEvolutionByYear = buildOrgaoEvolution(year)
        const baseKeysByExport = ['SEE', 'SES', 'SEINFRA'] as const

        const exportEvolutionData = baseEvolutionByYear.map((item) => {
          const row: Record<string, number | string> = { mes: item.mes }

          orgaosToExport.forEach((orgao, index) => {
            const baseKey = baseKeysByExport[index % baseKeysByExport.length]

            const baseTotal =
              data.orgaos.find((entry) => entry.codigoOrgao === baseKey)
                ?.totalEmpenhado ??
              selectedOrgaoByYear?.totalEmpenhado ??
              1

            const scale = baseTotal > 0 ? orgao.totalEmpenhado / baseTotal : 1

            row[orgao.siglaOrgao] = Math.round(item[baseKey] * scale)
          })

          return row
        })

        exportEvolutionData.forEach((item) => {
          orgaosToExport.forEach((orgao) => {
            const value = Number(item[orgao.siglaOrgao] ?? 0)
            const rowIndex = rows.length

            rows.push([item.mes, orgao.siglaOrgao, value * 1_000_000, year])
            addCurrencyCell(rowIndex, 2)
          })
        })
      })

      addEmptyLine()
    }

    if (selectedExportOptions.includes('perfilGastos')) {
      addTitle('Perfil de Gastos')
      rows.push(['Critério', 'Órgão', 'Valor', 'Ano'])

      comparativosByYear.forEach(({ year, data }) => {
        const selectedOrgaoByYear =
          data.orgaos.find(
            (orgao) => orgao.codigoOrgao === selectedOrgaoCode
          ) ?? data.orgaos[0]

        const orgaosToExport = selectedOrgaoByYear
          ? [
              selectedOrgaoByYear,
              ...data.orgaos
                .filter(
                  (orgao) =>
                    orgao.codigoOrgao !== selectedOrgaoByYear.codigoOrgao
                )
                .slice(0, 2),
            ]
          : []

        const baseRadarByYear = buildRadarData(year)
        const baseKeysByExport = ['SEE', 'SES', 'SEINFRA'] as const

        const exportRadarData = baseRadarByYear.map((item) => {
          const row: Record<string, number | string> = { subject: item.subject }

          orgaosToExport.forEach((orgao, index) => {
            const baseKey = baseKeysByExport[index % baseKeysByExport.length]

            const baseTotal =
              data.orgaos.find((entry) => entry.codigoOrgao === baseKey)
                ?.totalEmpenhado ??
              selectedOrgaoByYear?.totalEmpenhado ??
              1

            const scale = baseTotal > 0 ? orgao.totalEmpenhado / baseTotal : 1

            row[orgao.siglaOrgao] = clampValue(
              Math.round(item[baseKey] * scale),
              20,
              100
            )
          })

          return row
        })

        exportRadarData.forEach((item) => {
          orgaosToExport.forEach((orgao) => {
            rows.push([
              item.subject,
              orgao.siglaOrgao,
              Number(item[orgao.siglaOrgao] ?? 0),
              year,
            ])
          })
        })
      })

      addEmptyLine()
    }

    if (selectedExportOptions.includes('detalhamentoOrgao')) {
      addTitle('Detalhamento por Órgão')
      rows.push([
        'Órgão',
        'Nome do Órgão',
        'Orçamento',
        'Executado',
        'Execução',
        'Pessoal',
        'Investimento',
        'Servidores',
        'Ano',
      ])

      comparativosByYear.forEach(({ year, data }) => {
        data.orgaos.slice(0, 5).forEach((orgao) => {
          const execPct =
            orgao.totalEmpenhado > 0
              ? (orgao.totalPago / orgao.totalEmpenhado) * 100
              : 0

          const pessoal = orgao.totalEmpenhado * 0.68
          const investimento = orgao.totalEmpenhado * 0.1
          const servidores = Math.round(orgao.totalEmpenhado / 150000)

          const rowIndex = rows.length

          rows.push([
            orgao.siglaOrgao,
            orgao.nomeOrgao,
            orgao.totalEmpenhado,
            orgao.totalPago,
            `${execPct.toFixed(1)}%`,
            pessoal,
            investimento,
            servidores,
            year,
          ])

          addCurrencyCell(rowIndex, 2)
          addCurrencyCell(rowIndex, 3)
          addCurrencyCell(rowIndex, 5)
          addCurrencyCell(rowIndex, 6)
        })
      })

      addEmptyLine()
    }

    if (selectedExportOptions.includes('drillDownOrgao')) {
      addTitle('Drill-down por órgão')
      rows.push([
        'Órgão',
        'Classificação',
        'Descrição',
        'Total empenhado',
        'Qtd. empenhos',
        'Ano',
      ])

      const drillDownByYear = selectedOrgaoCode
        ? await Promise.all(
            selectedExportYears.map(async (year) => {
              const data = await getEvolucao(selectedOrgaoCode, year)
              return { year, data }
            })
          )
        : []

      drillDownByYear.forEach(({ year, data }) => {
        const orgaoLabel =
          comparativo?.orgaos.find(
            (orgao) => orgao.codigoOrgao === selectedOrgaoCode
          )?.siglaOrgao ??
          selectedOrgaoCode ??
          '-'

        data.itens.forEach((item) => {
          const rowIndex = rows.length

          rows.push([
            orgaoLabel,
            item.classificacaoMcasp,
            item.descricao,
            item.totalEmpenhado,
            item.quantidadeEmpenhos,
            year,
          ])

          addCurrencyCell(rowIndex, 3)
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

    worksheet['!cols'] = [
      { wch: 24 },
      { wch: 36 },
      { wch: 44 },
      { wch: 22 },
      { wch: 18 },
      { wch: 22 },
      { wch: 22 },
      { wch: 16 },
      { wch: 12 },
    ]

    XLSX.utils.book_append_sheet(workbook, worksheet, 'Relatório')

    XLSX.writeFile(
      workbook,
      `comparacao-orgaos-${selectedExportYears.join('-')}.xlsx`
    )
  }

  return (
    <div className="pt-15 space-y-6">
      <DashboardHeader
        title="Comparação entre Órgãos"
        subtitle="Análise comparativa de desempenho entre secretarias e órgãos"
      />
      <div className="p-8 space-y-6">
        <div className="flex justify-between">
          <div className="flex flex-wrap items-center justify-end gap-3">
            <div className="flex flex-col">
              <label className="text-xs text-gray-500">Ano</label>
              <select
                value={selectedYear}
                onChange={(event) =>
                  setSelectedYear(Number(event.target.value))
                }
                className="rounded-lg border border-gray-200 bg-white px-2 h-9 text-sm text-gray-700"
              >
                {[2026, 2025, 2024].map((year) => (
                  <option key={year} value={year}>
                    {year}
                  </option>
                ))}
              </select>
            </div>
            <div className="flex flex-col">
              <label className="text-xs text-gray-500">Órgão</label>
              <select
                value={selectedOrgaoCode ?? ''}
                onChange={(event) => setSelectedOrgaoCode(event.target.value)}
                className="w-55 max-w-full truncate rounded-lg border border-gray-200 bg-white px-2 h-9 text-sm text-gray-700"
              >
                {comparativo.orgaos.map((orgao) => (
                  <option key={orgao.codigoOrgao} value={orgao.codigoOrgao}>
                    {orgao.siglaOrgao} - {orgao.nomeOrgao}
                  </option>
                ))}
              </select>
            </div>
          </div>
          <div className="flex items-end gap-2">
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

            <button
              type="button"
              onClick={handleExportExcel}
              disabled={
                selectedExportOptions.length === 0 ||
                selectedExportYears.length === 0
              }
              className="cursor-pointer inline-flex h-9 items-center gap-2 rounded-lg bg-[#142F4B] px-5 text-sm font-semibold text-white transition hover:bg-[#0f243a] disabled:cursor-not-allowed disabled:bg-gray-400"
            >
              <Download className="h-4 w-4" />
              Exportar
            </button>
          </div>
        </div>
        {/* Órgão Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
          {topOrgaos.map((orgao, i) => {
            const execPct =
              orgao.totalEmpenhado > 0
                ? ((orgao.totalPago / orgao.totalEmpenhado) * 100).toFixed(1)
                : '0'
            return (
              <button
                key={orgao.codigoOrgao}
                type="button"
                onClick={() => setSelectedOrgaoCode(orgao.codigoOrgao)}
                className={`bg-white rounded-xl border p-6 text-left hover:shadow-md transition-shadow duration-300 cursor-pointer ${
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
                      <div className="flex items-center gap-2">
                        <p className="font-semibold text-gray-900">
                          {orgao.siglaOrgao}
                        </p>

                        <InfoTooltip
                          label={`Informação sobre ${orgao.siglaOrgao}`}
                          text={`Mostra um resumo das despesas de ${orgao.siglaOrgao}. Aqui você vê o valor total registrado para esse órgão e quanto desse valor já foi pago. Clique no card para analisar esse órgão com mais detalhes nos gráficos e tabelas abaixo.`}
                        />
                      </div>
                      <p className="text-xs text-gray-500">
                        {orgao.nomeOrgao.length > 20
                          ? orgao.nomeOrgao.slice(0, 20) + '...'
                          : orgao.nomeOrgao}
                      </p>
                    </div>
                  </div>
                  <span className="flex items-center gap-1 px-2 py-1 bg-emerald-50 text-emerald-600 text-xs font-semibold rounded-full">
                    <TrendingUp className="h-3 w-3" />
                    +4,2%
                  </span>
                </div>
                <p className="text-2xl font-bold text-gray-900 mb-2">
                  {formatCurrency(orgao.totalEmpenhado)}
                </p>
                <div className="flex items-center gap-2">
                  <div className="flex-1 bg-gray-100 rounded-full h-1.5">
                    <div
                      className="h-full rounded-full bg-[#008C6C] transition-all duration-500"
                      style={{
                        width: `${Math.min(parseFloat(execPct), 100)}%`,
                      }}
                    />
                  </div>
                  <span className="text-xs text-gray-500 font-medium">
                    {execPct}%
                  </span>
                </div>
              </button>
            )
          })}
        </div>
        {/* Charts Row */}
        <div className="grid grid-cols-1 lg:grid-cols-5 gap-5">
          {/* Evolução Comparativa */}
          <div className="lg:col-span-3 bg-white rounded-xl border border-gray-200 p-6">
            <div className="flex items-center gap-2 mb-1">
              <div className="flex items-center gap-2 mb-1">
                <h3 className="font-semibold text-gray-900">
                  Evolução Comparativa
                </h3>

                <InfoTooltip
                  label="Informação sobre evolução comparativa"
                  text="Mostra como as despesas dos órgãos mudaram ao longo dos meses. Esse gráfico ajuda a comparar se um órgão está gastando mais, menos ou seguindo um comportamento parecido com os outros."
                />
              </div>
            </div>
            <p className="text-sm text-gray-500 mb-4">
              Despesas por órgão ao longo dos anos (em milhões R$)
            </p>
            <div className="h-70">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart
                  data={evolutionData}
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
                        dot={{
                          r: 3,
                          fill: color,
                          strokeWidth: 2,
                          stroke: 'white',
                        }}
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
              <div className="flex items-center gap-2">
                <h3 className="font-semibold text-gray-900">
                  Perfil de Gastos
                </h3>

                <InfoTooltip
                  label="Informação sobre perfil de gastos"
                  text="Mostra uma visão comparativa do comportamento de gastos dos órgãos selecionados. Quanto maior a área preenchida no gráfico, maior é a participação daquele órgão nos critérios analisados."
                />
              </div>
              <p className="text-sm text-gray-500">
                Análise multidimensional dos órgãos selecionados
              </p>
            </div>
            <div className="h-62.5 mt-2">
              <ResponsiveContainer width="100%" height="100%">
                <RadarChart
                  data={radarData}
                  cx="50%"
                  cy="50%"
                  outerRadius="70%"
                >
                  <PolarGrid stroke="#e2e8f0" />
                  <PolarAngleAxis
                    dataKey="subject"
                    tick={{ fontSize: 11, fill: '#64748b' }}
                  />
                  <PolarRadiusAxis
                    tick={{ fontSize: 9, fill: '#94a3b8' }}
                    axisLine={false}
                  />
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
                  <Legend
                    iconType="circle"
                    iconSize={8}
                    wrapperStyle={{ fontSize: '12px' }}
                  />
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

        {/* Detalhamento por Órgão - Tabela */}
        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <div className="mb-4">
            <div className="flex items-center gap-2">
              <h3 className="font-semibold text-gray-900">
                Detalhamento por Órgão
              </h3>

              <InfoTooltip
                label="Informação sobre detalhamento por órgão"
                text="Mostra os principais dados de execução orçamentária por órgão, como orçamento registrado, valor executado, percentual de execução, gastos com pessoal, investimentos e quantidade estimada de servidores."
              />
            </div>
            <p className="text-sm text-gray-500">
              Execução orçamentária detalhada por secretaria
            </p>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-gray-200">
                  <th className="text-left py-3 px-4 font-semibold text-gray-600">
                    Órgão
                  </th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">
                    Orçamento
                  </th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">
                    Executado
                  </th>
                  <th className="text-center py-3 px-4 font-semibold text-gray-600">
                    Execução
                  </th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">
                    Pessoal
                  </th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">
                    Investimento
                  </th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">
                    Servidores
                  </th>
                </tr>
              </thead>
              <tbody>
                {tableOrgaos.map((orgao) => {
                  const execPct =
                    orgao.totalEmpenhado > 0
                      ? (orgao.totalPago / orgao.totalEmpenhado) * 100
                      : 0
                  const barColor =
                    execPct > 80
                      ? '#22c55e'
                      : execPct > 50
                        ? '#f59e0b'
                        : '#ef4444'
                  const pessoal = orgao.totalEmpenhado * 0.68
                  const investimento = orgao.totalEmpenhado * 0.1
                  const servidores = Math.round(orgao.totalEmpenhado / 150000)
                  return (
                    <tr
                      key={orgao.codigoOrgao}
                      className="border-b border-gray-100 hover:bg-gray-50/50 transition-colors"
                    >
                      <td className="py-3 px-4">
                        <p className="font-medium text-gray-900">
                          {orgao.siglaOrgao}
                        </p>
                        <p className="text-xs text-gray-500">
                          {orgao.nomeOrgao}
                        </p>
                      </td>
                      <td className="text-right py-3 px-4 text-gray-700 font-medium tabular-nums">
                        {formatCurrencyFull(orgao.totalEmpenhado)}
                      </td>
                      <td className="text-right py-3 px-4 text-gray-700 font-medium tabular-nums">
                        {formatCurrencyFull(orgao.totalPago)}
                      </td>
                      <td className="py-3 px-4">
                        <div className="flex items-center gap-2 justify-center">
                          <div className="w-20 bg-gray-100 rounded-full h-2">
                            <div
                              className="h-full rounded-full transition-all duration-500"
                              style={{
                                width: `${Math.min(execPct, 100)}%`,
                                backgroundColor: barColor,
                              }}
                            />
                          </div>
                          <span className="text-xs font-medium text-gray-600 w-10 text-right">
                            {execPct.toFixed(1)}%
                          </span>
                        </div>
                      </td>
                      <td className="text-right py-3 px-4 text-gray-700 tabular-nums">
                        {formatCurrencyFull(pessoal)}
                      </td>
                      <td className="text-right py-3 px-4 text-gray-700 tabular-nums">
                        {formatCurrencyFull(investimento)}
                      </td>
                      <td className="text-right py-3 px-4 text-gray-700 tabular-nums">
                        {servidores.toLocaleString('pt-BR')}
                      </td>
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
              <div className="flex items-center gap-2">
                <h3 className="font-semibold text-gray-900">
                  Drill-down por órgão
                </h3>

                <InfoTooltip
                  label="Informação sobre drill-down por órgão"
                  text="Mostra uma abertura mais detalhada das despesas do órgão selecionado. Aqui é possível entender melhor quais classificações de custo concentram os maiores valores."
                />
              </div>
              <p className="text-sm text-gray-500">
                {selectedOrgao
                  ? `Classificações de custo para ${selectedOrgao.siglaOrgao} (${selectedYear})`
                  : 'Selecione um órgão para ver os itens.'}
              </p>
            </div>
            {drillLoading && (
              <span className="text-xs text-gray-400">Carregando...</span>
            )}
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-gray-200">
                  <th className="text-left py-3 px-4 font-semibold text-gray-600">
                    Classificação
                  </th>
                  <th className="text-left py-3 px-4 font-semibold text-gray-600">
                    Descrição
                  </th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">
                    Total empenhado
                  </th>
                  <th className="text-right py-3 px-4 font-semibold text-gray-600">
                    Qtd. empenhos
                  </th>
                </tr>
              </thead>
              <tbody>
                {(drillDown?.itens ?? []).map((item) => (
                  <tr
                    key={`${item.classificacaoMcasp}-${item.descricao}`}
                    className="border-b border-gray-100"
                  >
                    <td className="py-3 px-4 text-gray-700 font-medium">
                      {item.classificacaoMcasp}
                    </td>
                    <td className="py-3 px-4 text-gray-600">
                      {item.descricao}
                    </td>
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
              <p className="text-sm text-gray-500 px-4 py-6">
                Sem dados para o órgão selecionado.
              </p>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
