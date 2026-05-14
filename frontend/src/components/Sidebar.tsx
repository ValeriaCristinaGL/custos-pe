import { Link, NavLink } from 'react-router-dom'
import {
  LayoutDashboard,
  GitCompareArrows,
  Clock,
  ChevronRight,
} from 'lucide-react'

const navItems = [
  {
    label: 'Painel Geral',
    path: '/dashboard',
    icon: LayoutDashboard,
  },
  {
    label: 'Comparação entre Órgãos',
    path: '/dashboard/comparativo',
    icon: GitCompareArrows,
  },
  {
    label: 'Análise Temporal',
    path: '/dashboard/evolucao',
    icon: Clock,
  },
]

export function Sidebar() {
  return (
    <aside className="fixed left-0 top-0 h-screen w-[260px] bg-[#142F4B] text-white flex flex-col z-50">
      {/* Logo */}
      <Link
        to="/"
        className="px-5 py-5 flex items-center gap-3 border-b border-white/10 hover:bg-white/5 transition-colors"
      >
        <span className="inline-flex h-9 w-9 items-center justify-center rounded-lg bg-[#008C6C]">
          <LayoutDashboard className="h-5 w-5 text-white" />
        </span>
        <div>
          <p className="font-semibold text-sm leading-tight">Transparência PE</p>
          <p className="text-[11px] text-gray-400">Inteligência Fiscal</p>
        </div>
      </Link>

      {/* Navigation */}
      <nav className="flex-1 px-3 py-4">
        <p className="text-[11px] uppercase text-gray-400 tracking-wider font-medium px-3 mb-3">
          Navegação
        </p>
        <div className="rounded-xl border border-white/10 bg-white/5 p-2">
          <ul className="flex flex-col gap-1">
          {navItems.map((item) => (
            <li key={item.path}>
              <NavLink
                to={item.path}
                end={item.path === '/dashboard'}
                className={({ isActive }) =>
                  `flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-all duration-200 group ${
                    isActive
                      ? 'bg-[#008C6C] text-white shadow-lg shadow-[#008C6C]/25'
                      : 'text-gray-300 hover:bg-white/8 hover:text-white'
                  }`
                }
              >
                <item.icon className="h-[18px] w-[18px] shrink-0" />
                <span className="flex-1">{item.label}</span>
                <ChevronRight className="h-4 w-4 opacity-50 group-hover:opacity-100 transition-opacity" />
              </NavLink>
            </li>
          ))}
          </ul>
        </div>
      </nav>

    </aside>
  )
}
