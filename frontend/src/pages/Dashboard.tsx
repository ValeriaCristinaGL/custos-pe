import { Outlet } from 'react-router-dom'
import { Sidebar } from '../components/Sidebar'
import { Calendar } from 'lucide-react'
import type { ReactNode } from 'react'

interface DashboardProps {
  title: string
  subtitle: string
  children?: ReactNode
}

export function DashboardLayout() {
  return (
    <div className="min-h-screen bg-[#F8FAFC]">
      <Sidebar />

      <main className="ml-65 min-h-screen">
        <Outlet />
      </main>
    </div>
  )
}

export function DashboardHeader({ title, subtitle, children }: DashboardProps) {
  return (
    <header className="fixed left-65 right-0 top-0 z-50 border-b border-gray-200 bg-white/95 px-8 py-4 backdrop-blur-md">
      <div className="flex items-center justify-between gap-6">
        <div className="flex items-center gap-3">
          <span className="inline-flex h-9 w-9 items-center justify-center rounded-lg bg-[#142F4B]/10 text-[#142F4B]">
            <Calendar className="h-5 w-5" />
          </span>

          <div>
            <h1 className="text-sm font-semibold text-gray-900">{title}</h1>
            <p className="text-xs text-gray-500">{subtitle}</p>
          </div>
        </div>

        {children && (
          <div className="flex items-center justify-end">{children}</div>
        )}
      </div>
    </header>
  )
}
