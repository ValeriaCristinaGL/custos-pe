import axios from 'axios'

const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:8080'

const api = axios.create({
  baseURL: `${API_BASE}/api/v1`,
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' },
})

// ===================== TYPES =====================

export interface DashboardResumo {
  totalEmpenhado: number
  totalLiquidado: number
  totalPago: number
  percentualExecutado: number
  totalEmpenhos: number
  totalContratos: number
}

export interface OrgaoComparativoItem {
  codigoOrgao: string
  nomeOrgao: string
  siglaOrgao: string
  totalEmpenhado: number
  totalLiquidado: number
  totalPago: number
}

export interface ComparativoOrgaos {
  orgaos: OrgaoComparativoItem[]
  ano: number
}

export interface DrillDownItem {
  classificacaoMcasp: string
  descricao: string
  totalEmpenhado: number
  quantidadeEmpenhos: number
}

export interface DrillDown {
  codigoOrgao: string
  nomeOrgao: string
  itens: DrillDownItem[]
}

type YearKey = `ano${number}`

const BASE_YEAR = 2026

const BASE_MONTHS = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez']

function clamp(value: number, min: number, max: number): number {
  return Math.max(min, Math.min(max, value))
}

function yearFactor(year: number, base = BASE_YEAR): number {
  return clamp(1 + (year - base) * 0.06, 0.82, 1.2)
}

function scaleNumber(value: number, factor: number): number {
  return Math.round(value * factor)
}

function buildResumoMock(year: number): DashboardResumo {
  const factor = yearFactor(year)
  return {
    totalEmpenhado: scaleNumber(MOCK_RESUMO.totalEmpenhado, factor),
    totalLiquidado: scaleNumber(MOCK_RESUMO.totalLiquidado, factor),
    totalPago: scaleNumber(MOCK_RESUMO.totalPago, factor),
    percentualExecutado: MOCK_RESUMO.percentualExecutado,
    totalEmpenhos: scaleNumber(MOCK_RESUMO.totalEmpenhos, factor),
    totalContratos: scaleNumber(MOCK_RESUMO.totalContratos, factor),
  }
}

function buildComparativoMock(year: number): ComparativoOrgaos {
  const factor = yearFactor(year)
  return {
    ano: year,
    orgaos: MOCK_COMPARATIVO.orgaos.map((orgao, index) => {
      const orgaoFactor = clamp(factor + index * 0.01, 0.82, 1.25)
      return {
        ...orgao,
        totalEmpenhado: scaleNumber(orgao.totalEmpenhado, orgaoFactor),
        totalLiquidado: scaleNumber(orgao.totalLiquidado, orgaoFactor),
        totalPago: scaleNumber(orgao.totalPago, orgaoFactor),
      }
    }),
  }
}

function buildDrillDownMock(codigoOrgao: string, year: number): DrillDown {
  const factor = yearFactor(year)
  return {
    ...MOCK_DRILLDOWN,
    codigoOrgao,
    itens: MOCK_DRILLDOWN.itens.map((item, index) => {
      const itemFactor = clamp(factor + index * 0.03, 0.8, 1.4)
      return {
        ...item,
        totalEmpenhado: scaleNumber(item.totalEmpenhado, itemFactor),
        quantidadeEmpenhos: scaleNumber(item.quantidadeEmpenhos, itemFactor),
      }
    }),
  }
}

export function buildMonthlyEvolution(year: number): MonthlyData[] {
  const currentKey = `ano${year}` as YearKey
  const prevKey = `ano${year - 1}` as YearKey
  const currentFactor = yearFactor(year)
  const prevFactor = yearFactor(year - 1)

  return MOCK_MONTHLY_EVOLUTION.map((item, index) => {
    const month = BASE_MONTHS[index] ?? item.mes.split('/')[0]
    const entry: Record<YearKey, number> = {
      [prevKey]: scaleNumber(item.ano2025, prevFactor),
      [currentKey]: scaleNumber(item.ano2026, currentFactor),
    }
    return {
      mes: `${month}/${String(year).slice(2)}`,
      ...entry,
    }
  })
}

export function buildCategoryDistribution(year: number): CategoryDistribution[] {
  const delta = clamp(year - BASE_YEAR, -2, 2)
  const adjustments = [delta, -delta, Math.round(delta / 2), -Math.round(delta / 2)]
  const adjusted = MOCK_CATEGORY_DISTRIBUTION.map((item, index) => ({
    ...item,
    value: item.value + adjustments[index],
  }))
  const total = adjusted.reduce((sum, item) => sum + item.value, 0)
  return adjusted.map((item) => ({
    ...item,
    value: clamp(Math.round((item.value / total) * 100), 5, 70),
  }))
}

function isEmptyResumo(resumo: DashboardResumo): boolean {
  return resumo.totalEmpenhado === 0
    && resumo.totalLiquidado === 0
    && resumo.totalPago === 0
    && resumo.totalEmpenhos === 0
    && resumo.totalContratos === 0
}

function isEmptyComparativo(data: ComparativoOrgaos): boolean {
  return !data.orgaos || data.orgaos.length === 0
}

function isEmptyDrillDown(data: DrillDown): boolean {
  return !data.itens || data.itens.length === 0
}

// ===================== MOCK DATA =====================

const MOCK_RESUMO: DashboardResumo = {
  totalEmpenhado: 48700000000,
  totalLiquidado: 45200000000,
  totalPago: 43100000000,
  percentualExecutado: 88.5,
  totalEmpenhos: 125430,
  totalContratos: 8742,
}

const MOCK_COMPARATIVO: ComparativoOrgaos = {
  ano: 2026,
  orgaos: [
    { codigoOrgao: 'SEE', nomeOrgao: 'Secretaria de Educação', siglaOrgao: 'SEE', totalEmpenhado: 13200000000, totalLiquidado: 12450000000, totalPago: 11800000000 },
    { codigoOrgao: 'SES', nomeOrgao: 'Secretaria de Saúde', siglaOrgao: 'SES', totalEmpenhado: 11500000000, totalLiquidado: 10800000000, totalPago: 10200000000 },
    { codigoOrgao: 'SDS', nomeOrgao: 'Secretaria de Defesa Social', siglaOrgao: 'SDS', totalEmpenhado: 5800000000, totalLiquidado: 5400000000, totalPago: 5100000000 },
    { codigoOrgao: 'SEINFRA', nomeOrgao: 'Secretaria de Infraestrutura', siglaOrgao: 'SEINFRA', totalEmpenhado: 4200000000, totalLiquidado: 3900000000, totalPago: 3600000000 },
    { codigoOrgao: 'SEPLAG', nomeOrgao: 'Secretaria de Planejamento', siglaOrgao: 'SEPLAG', totalEmpenhado: 3100000000, totalLiquidado: 2900000000, totalPago: 2750000000 },
    { codigoOrgao: 'SEFAZ', nomeOrgao: 'Secretaria da Fazenda', siglaOrgao: 'SEFAZ', totalEmpenhado: 2800000000, totalLiquidado: 2650000000, totalPago: 2500000000 },
    { codigoOrgao: 'SAD', nomeOrgao: 'Secretaria de Administração', siglaOrgao: 'SAD', totalEmpenhado: 2400000000, totalLiquidado: 2200000000, totalPago: 2050000000 },
    { codigoOrgao: 'SDSCJ', nomeOrgao: 'Sec. Des. Social, Criança e Juventude', siglaOrgao: 'SDSCJ', totalEmpenhado: 1900000000, totalLiquidado: 1750000000, totalPago: 1600000000 },
    { codigoOrgao: 'SECID', nomeOrgao: 'Secretaria das Cidades', siglaOrgao: 'SECID', totalEmpenhado: 1700000000, totalLiquidado: 1550000000, totalPago: 1400000000 },
    { codigoOrgao: 'SECTMA', nomeOrgao: 'Sec. Ciência, Tecnologia e Meio Ambiente', siglaOrgao: 'SECTMA', totalEmpenhado: 1400000000, totalLiquidado: 1300000000, totalPago: 1200000000 },
  ],
}

const MOCK_DRILLDOWN: DrillDown = {
  codigoOrgao: 'SEE',
  nomeOrgao: 'Secretaria de Educação',
  itens: [
    { classificacaoMcasp: '3.1.90', descricao: 'Pessoal e Encargos Sociais', totalEmpenhado: 8920000000, quantidadeEmpenhos: 42000 },
    { classificacaoMcasp: '3.3.90', descricao: 'Outras Despesas Correntes', totalEmpenhado: 2100000000, quantidadeEmpenhos: 18500 },
    { classificacaoMcasp: '4.4.90', descricao: 'Investimentos', totalEmpenhado: 1350000000, quantidadeEmpenhos: 3200 },
    { classificacaoMcasp: '3.3.50', descricao: 'Transferências a Instituições', totalEmpenhado: 830000000, quantidadeEmpenhos: 850 },
  ],
}

// ===================== MONTHLY EVOLUTION MOCK =====================

export type MonthlyData = { mes: string } & Record<YearKey, number>

export const MOCK_MONTHLY_EVOLUTION: MonthlyData[] = [
  { mes: 'Jan/26', ano2025: 12, ano2026: 52 },
  { mes: 'Fev/26', ano2025: 35, ano2026: 45 },
  { mes: 'Mar/26', ano2025: 28, ano2026: 38 },
  { mes: 'Abr/26', ano2025: 42, ano2026: 48 },
  { mes: 'Mai/26', ano2025: 55, ano2026: 62 },
  { mes: 'Jun/26', ano2025: 48, ano2026: 58 },
  { mes: 'Jul/26', ano2025: 65, ano2026: 78 },
  { mes: 'Ago/26', ano2025: 52, ano2026: 60 },
  { mes: 'Set/26', ano2025: 45, ano2026: 55 },
  { mes: 'Out/26', ano2025: 58, ano2026: 65 },
  { mes: 'Nov/26', ano2025: 42, ano2026: 48 },
  { mes: 'Dez/26', ano2025: 50, ano2026: 55 },
]

export interface CategoryDistribution {
  name: string
  value: number
  color: string
}

export const MOCK_CATEGORY_DISTRIBUTION: CategoryDistribution[] = [
  { name: 'Pessoal', value: 56, color: '#008C6C' },
  { name: 'Custeio', value: 22, color: '#3B82F6' },
  { name: 'Investimentos', value: 14, color: '#22C55E' },
  { name: 'Outros', value: 8, color: '#C79E41' },
]

// ===================== TEMPORAL ANALYSIS MOCK =====================

export interface QuarterlyData {
  trimestre: string
  ano2025: number
  ano2026: number
}

export const MOCK_QUARTERLY: QuarterlyData[] = [
  { trimestre: 'T1', ano2025: 8, ano2026: 22 },
  { trimestre: 'T2', ano2025: 32, ano2026: 65 },
  { trimestre: 'T3', ano2025: 28, ano2026: 45 },
  { trimestre: 'T4', ano2025: 18, ano2026: 30 },
]

export interface AnnualGrowth {
  ano: string
  valor: number
}

export const MOCK_ANNUAL_GROWTH: AnnualGrowth[] = [
  { ano: '2022', valor: 28 },
  { ano: '2023', valor: 35 },
  { ano: '2024', valor: 42 },
  { ano: '2025', valor: 58 },
  { ano: '2026', valor: 72 },
]

export interface SeasonalityData {
  mes: string
  indice: number
}

export const MOCK_SEASONALITY: SeasonalityData[] = [
  { mes: 'Jan', indice: 90 },
  { mes: 'Fev', indice: 75 },
  { mes: 'Mar', indice: 85 },
  { mes: 'Abr', indice: 60 },
  { mes: 'Mai', indice: 0 },
  { mes: 'Jun', indice: 0 },
  { mes: 'Jul', indice: 0 },
  { mes: 'Ago', indice: 0 },
  { mes: 'Set', indice: 0 },
  { mes: 'Out', indice: 0 },
  { mes: 'Nov', indice: 0 },
  { mes: 'Dez', indice: 0 },
]

export interface MonthlyComparison {
  mes: string
  ano2025: number
  ano2026: number
}

export const MOCK_MONTHLY_COMPARISON: MonthlyComparison[] = [
  { mes: 'Jan', ano2025: 15, ano2026: 52 },
  { mes: 'Fev', ano2025: 35, ano2026: 45 },
  { mes: 'Mar', ano2025: 28, ano2026: 38 },
  { mes: 'Abr', ano2025: 42, ano2026: 48 },
  { mes: 'Mai', ano2025: 55, ano2026: 62 },
  { mes: 'Jun', ano2025: 48, ano2026: 58 },
  { mes: 'Jul', ano2025: 65, ano2026: 78 },
  { mes: 'Ago', ano2025: 52, ano2026: 60 },
  { mes: 'Set', ano2025: 45, ano2026: 55 },
  { mes: 'Out', ano2025: 58, ano2026: 65 },
  { mes: 'Nov', ano2025: 42, ano2026: 48 },
  { mes: 'Dez', ano2025: 50, ano2026: 55 },
]

// ===================== COMPARATIVE EVOLUTION MOCK =====================

export interface OrgaoEvolutionMonth {
  mes: string
  SEE: number
  SES: number
  SEINFRA: number
}

const BASE_ORGAO_EVOLUTION: OrgaoEvolutionMonth[] = [
  { mes: 'Jan', SEE: 5, SES: 3, SEINFRA: 2 },
  { mes: 'Feb', SEE: 12, SES: 8, SEINFRA: 4 },
  { mes: 'Mar', SEE: 18, SES: 15, SEINFRA: 8 },
  { mes: 'Abr', SEE: 25, SES: 22, SEINFRA: 12 },
  { mes: 'Mai', SEE: 45, SES: 38, SEINFRA: 18 },
  { mes: 'Jun', SEE: 55, SES: 48, SEINFRA: 22 },
  { mes: 'Jul', SEE: 65, SES: 55, SEINFRA: 28 },
  { mes: 'Ago', SEE: 58, SES: 50, SEINFRA: 25 },
  { mes: 'Set', SEE: 52, SES: 45, SEINFRA: 22 },
  { mes: 'Out', SEE: 60, SES: 52, SEINFRA: 28 },
  { mes: 'Nov', SEE: 55, SES: 48, SEINFRA: 25 },
  { mes: 'Dez', SEE: 50, SES: 42, SEINFRA: 22 },
]

export interface RadarDataPoint {
  subject: string
  SEE: number
  SES: number
  SEINFRA: number
}

const BASE_RADAR_DATA: RadarDataPoint[] = [
  { subject: 'Pessoal', SEE: 95, SES: 80, SEINFRA: 40 },
  { subject: 'Eficiência', SEE: 78, SES: 85, SEINFRA: 72 },
  { subject: 'Custeio', SEE: 65, SES: 70, SEINFRA: 88 },
  { subject: 'Execução', SEE: 82, SES: 75, SEINFRA: 68 },
  { subject: 'Investimento', SEE: 55, SES: 45, SEINFRA: 92 },
]

export function buildOrgaoEvolution(year: number): OrgaoEvolutionMonth[] {
  const factor = yearFactor(year)
  return BASE_ORGAO_EVOLUTION.map((item) => ({
    mes: item.mes,
    SEE: scaleNumber(item.SEE, factor),
    SES: scaleNumber(item.SES, factor),
    SEINFRA: scaleNumber(item.SEINFRA, factor),
  }))
}

export function buildRadarData(year: number): RadarDataPoint[] {
  const factor = yearFactor(year)
  return BASE_RADAR_DATA.map((item, index) => {
    const itemFactor = clamp(factor + index * 0.01, 0.7, 1.2)
    return {
      subject: item.subject,
      SEE: clamp(scaleNumber(item.SEE, itemFactor), 20, 100),
      SES: clamp(scaleNumber(item.SES, itemFactor), 20, 100),
      SEINFRA: clamp(scaleNumber(item.SEINFRA, itemFactor), 20, 100),
    }
  })
}

// ===================== API FUNCTIONS =====================

export async function getResumo(ano?: number): Promise<DashboardResumo> {
  try {
    const params = ano ? { ano } : {}
    const { data } = await api.get<DashboardResumo>('/dashboard/resumo', { params })
    if (isEmptyResumo(data)) {
      console.warn('Backend retornou resumo vazio, usando dados mock')
      return buildResumoMock(ano ?? BASE_YEAR)
    }
    return data
  } catch {
    console.warn('Backend indisponível, usando dados mock para resumo')
    return buildResumoMock(ano ?? BASE_YEAR)
  }
}

export async function getComparativo(ano: number): Promise<ComparativoOrgaos> {
  try {
    const { data } = await api.get<ComparativoOrgaos>('/dashboard/comparativo', { params: { ano } })
    if (isEmptyComparativo(data)) {
      console.warn('Backend retornou comparativo vazio, usando dados mock')
      return buildComparativoMock(ano)
    }
    return data
  } catch {
    console.warn('Backend indisponível, usando dados mock para comparativo')
    return buildComparativoMock(ano)
  }
}

export async function getEvolucao(codigoOrgao: string, ano?: number): Promise<DrillDown> {
  try {
    const params: Record<string, string | number> = { codigoOrgao }
    if (ano) params.ano = ano
    const { data } = await api.get<DrillDown>('/dashboard/evolucao', { params })
    if (isEmptyDrillDown(data)) {
      console.warn('Backend retornou drill-down vazio, usando dados mock')
      return buildDrillDownMock(codigoOrgao, ano ?? BASE_YEAR)
    }
    return data
  } catch {
    console.warn('Backend indisponível, usando dados mock para drill-down')
    return buildDrillDownMock(codigoOrgao, ano ?? BASE_YEAR)
  }
}

export default api
