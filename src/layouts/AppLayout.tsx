import { Link, Outlet, useLocation } from 'react-router-dom'
import {
  SidebarProvider,
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarHeader,
  SidebarInset,
  SidebarTrigger,
} from '../components/ui/sidebar'

import {
  BarChart3,
  Building2,
  ChartNoAxesCombined,
  ChevronRight,
} from 'lucide-react'

export function AppLayout() {
  const pageConfig = {
    '/painel-geral': {
      title: 'Visão Geral de Custos',
      description: 'Visão consolidada das despesas do estado de Pernambuco',
    },

    '/comparacao': {
      title: 'Comparação entre Órgãos',
      description: 'Compare indicadores e despesas entre os órgãos estaduais',
    },

    '/analise-temporal': {
      title: 'Análise Temporal',
      description: 'Evolução e tendencias das despesas ao longo do tempo',
    },
  }

  const location = useLocation()

  const currentPage = pageConfig[location.pathname as keyof typeof pageConfig]

  return (
    <SidebarProvider>
      <Sidebar>
        <SidebarHeader>
          <div className="flex flex-col px-2 py-2">
            <p className="font-semibold text-sm text-white">Transparência PE</p>
            <p className="text-sm text-zinc-400">Inteligência Fiscal</p>
          </div>
        </SidebarHeader>

        <SidebarContent>
          <SidebarGroup>
            <SidebarGroupContent>
              <SidebarMenu className="flex flex-col gap-6">
                <SidebarMenuItem>
                  <SidebarMenuButton
                    asChild
                    isActive={location.pathname === '/painel-geral'}
                  >
                    <Link
                      to="/painel-geral"
                      className="flex items-center justify-between"
                    >
                      <div className="flex items-center gap-2">
                        <BarChart3 />
                        <span>Painel Geral</span>
                      </div>
                      <ChevronRight />
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>

                <SidebarMenuItem>
                  <SidebarMenuButton
                    asChild
                    isActive={location.pathname === '/comparacao'}
                  >
                    <Link
                      to="/comparacao"
                      className="flex items-center justify-between"
                    >
                      <div className="flex items-center gap-2">
                        <Building2 />
                        <span>Comparação entre Órgãos</span>
                      </div>
                      <ChevronRight />
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>

                <SidebarMenuItem>
                  <SidebarMenuButton
                    asChild
                    isActive={location.pathname === '/analise-temporal'}
                  >
                    <Link
                      to="/analise-temporal"
                      className="flex items-center justify-between"
                    >
                      <div className="flex items-center gap-2">
                        <ChartNoAxesCombined />
                        <span>Análise Temporal</span>
                      </div>
                      <ChevronRight />
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        </SidebarContent>
      </Sidebar>

      <SidebarInset>
        <header className="h-16 border-b bg-white flex items-center justify-between px-6 border-zinc-100 shadow-xs">
          <div className="flex items-center gap-4">
            <SidebarTrigger />
            <div className="border h-6 border-zinc-300" />
            <div>
              <p className="font-semibold text-[16px]">{currentPage?.title}</p>
              <p className="text-sm text-gray-600">
                {currentPage?.description}
              </p>
            </div>
          </div>
        </header>

        <main className="p-6 bg-[#F3F5F8] h-[calc(100dvh-4rem)]">
          <Outlet />
        </main>
      </SidebarInset>
    </SidebarProvider>
  )
}
