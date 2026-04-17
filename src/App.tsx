import {
  ArrowRight,
  Building2,
  Calendar,
  ChartColumnDecreasing,
  Filter,
  Shield,
  TrendingUp,
  Users,
} from 'lucide-react'
import { CustomButton } from './components/CustomButton'
import './index.css'

function App() {
  return (
    <div className="bg-gray-50 min-h-screen">
      <div className="h-full items-center bg-gray-50">
        <div className="fixed top-0 left-0 z-50 w-full backdrop-blur-md bg-white/85 border-b border-gray-200 shadow-sm">
          <div className="flex items-center justify-between py-4 px-8">
            <p className="font-semibold">TransparênciaPE</p>

            <div className="flex gap-14 text-cinza text-sm">
              <a
                href="#funcionalidades"
                className="hover:border-b hover:text-azul-base hover:border-azul-base"
              >
                Funcionalidades
              </a>
              <a
                href="#sobre"
                className="hover:border-b hover:text-azul-base hover:border-azul-base"
              >
                Sobre os dados
              </a>
              <a
                href="#impacto"
                className="hover:border-b hover:text-azul-base hover:border-azul-base"
              >
                Impacto
              </a>
            </div>
            <div>
              <CustomButton
                text="Acessar Painel"
                variant="primario"
                iconRight={<ArrowRight />}
              />
            </div>
          </div>
        </div>
        <div className="min-h-screen items-center pt-20 grid grid-cols-2 gap-5">
          <div className="mx-8 flex flex-col gap-5">
            <div className="text-5xl font-bold flex flex-col gap-4">
              <p>Transparência</p>
              <p>pública ao</p>
              <p>alcance de todos</p>
            </div>
            <div>
              <p className="text-cinza">
                Tranformamos dados financeiros complexos do Estado de Pernambuco
                em informações claras e acessível. Acompanhe despesas, compare
                órgãos e analise tendencias com facildiade.
              </p>
            </div>
            <div className="flex gap-4">
              <div>
                <CustomButton
                  text="Explorar Dados"
                  iconRight={<ArrowRight />}
                />
              </div>
              <div>
                <CustomButton
                  text="Conhecer Funcionalidades"
                  variant="delineado"
                />
              </div>
            </div>
          </div>
          <div></div>
        </div>
        <div className="bg-white border-y border-gray-200 flex justify-between p-15">
          <div className="flex flex-col items-center gap-1">
            <Building2 className="text-verde-base h-8 w-8" />
            <p className="font-semibold text-xl">184</p>
            <p className="text-cinza">Órgãos Monitorados</p>
          </div>
          <div className="flex flex-col items-center gap-1">
            <TrendingUp className="text-verde-base h-8 w-8" />
            <p className="font-semibold text-xl">R$48,7B</p>
            <p className="text-cinza">Em Despesas Analisadas</p>
          </div>
          <div className="flex flex-col items-center gap-1">
            <Calendar className="text-verde-base h-8 w-8" />
            <p className="font-semibold text-xl">5 anos</p>
            <p className="text-cinza">De Dados Históticos</p>
          </div>
          <div className="flex flex-col items-center gap-1">
            <Shield className="text-verde-base h-8 w-8" />
            <p className="font-semibold text-xl">100%</p>
            <p className="text-cinza">Dados Públicos</p>
          </div>
        </div>
        <div id="funcionalidades" className="bg-gray-50">
          <p className="font-semibold text-center pt-25 text-3xl">
            Funcionalidades Prinipais
          </p>
          <p className="text-cinza text-center mt-2">
            Ferramentas poderosas para transformar dados brutos em inteligência
            acionável
          </p>
          <div className="mt-16 grid grid-cols-2">
            <div className="flex flex-col gap-7">
              <div className="bg-white border border-gray-200 rounded-xl p-8 ml-8 flex flex-col gap-4">
                <span className="inline-flex h-10 w-10 items-center justify-center rounded-md bg-azul-base">
                  <ChartColumnDecreasing className="h-5 w-5 text-white" />
                </span>
                <p className="font-semibold">Painel Geral de Custos</p>
                <p className="text-cinza">
                  Visão consolidada de todas as despesas do Estado, com KPIs
                  principais, distribuição por categoria e evolução mensal de
                  gastos.
                </p>
              </div>
              <div className="bg-white border border-gray-200 rounded-xl p-8 ml-8 flex flex-col gap-4">
                <span className="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#31AA40]">
                  <Calendar className="h-5 w-5 text-white" />
                </span>
                <p className="font-semibold">Análise Temporal</p>
                <p className="text-cinza">
                  Acompanhamento da evolução das despesas ao longo do tempo, com
                  detecção de tendências e projeções futuras.
                </p>
              </div>
            </div>
            <div className="flex flex-col gap-7 pb-16">
              <div className="bg-white border border-gray-200 rounded-xl p-8 ml-8 flex flex-col gap-4">
                <span className="inline-flex h-10 w-10 items-center justify-center rounded-md bg-verde-base">
                  <Building2 className="h-5 w-5 text-white" />
                </span>
                <p className="font-semibold">Comparação entre Órgão</p>
                <p className="text-cinza">
                  Análise comparativa detalhada entre secretarias e autarquias,
                  permitindo identificar padrões de eficência e oportunidades de
                  melhoria.
                </p>
              </div>
              <div className="bg-white border border-gray-200 rounded-xl p-8 ml-8 flex flex-col gap-4">
                <span className="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#C79E41]">
                  <Filter className="h-5 w-5 text-white" />
                </span>
                <p className="font-semibold">Filtros Dinâmicos</p>
                <p className="text-cinza">
                  Sistema flexível de filtragem que permite explorar os dados
                  por período, categoria, órgão, tipo de despesa e muito mais.
                </p>
              </div>
            </div>
          </div>
        </div>
        <div
          id="sobre"
          className="bg-azul-base text-white px-8 py-30 grid grid-cols-2 gap-4 justify-center items-center"
        >
          <div className="flex flex-col gap-8">
            <p className="font-semibold text-3xl">
              Dados públicos, acesso democrático
            </p>
            <p className="text-gray-300">
              Todos os dados apresentados nesta plataforma são provenientes de
              fontes oficiais do Governo do Estado de Pernambuco, em
              conformidade com a Lei de Acesso a Informação (Lei 12.527/2011) e
              a Lei de Responsabilidade Fiscal (LC 101/2000)
            </p>
          </div>
          <div>
            <div className="bg-[#2B435C] rounded-md p-4 gap-4 flex flex-col">
              <div className="flex items-center gap-3">
                <Shield className="text-verde-base h-8 w-8" />
                <div>
                  <p>Dados Verificados</p>
                  <p className="text-gray-300">Fontes oficiais e auditáveis</p>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <Users className="text-verde-base h-8 w-8" />
                <div>
                  <p>Para o Cidadão</p>
                  <p className="text-gray-300">
                    Linguagem acessível e intuitiva
                  </p>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <TrendingUp className="text-verde-base h-8 w-8" />
                <div>
                  <p>Atualização Contínua</p>
                  <p className="text-gray-300">
                    Dados sincronizados periodicamente
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div
          id="impacto"
          className="bg-gray-50 flex flex-col items-center gap-4 py-40"
        >
          <p className="text-center text-3xl font-semibold">
            Transparência gera confiança
          </p>

          <p className="text-cinza max-w-2xl text-center">
            Ao facilitar o acesso aos dados públicos, fortalecemos o controle
            social e contribuimos para uma gestão pública eficiente e
            responsável.
          </p>

          <div className="flex justify-center">
            <CustomButton
              text="Começar a Explorar"
              iconRight={<ArrowRight />}
            />
          </div>
        </div>
        <div className="border-t border-gray-200 flex justify-between px-8 py-4 text-cinza">
          <p>TransparênciaPE</p>
          <p>Plataforma de inteligência fiscal para o Estado de Pernambuco</p>
        </div>
      </div>
    </div>
  )
}

export default App
