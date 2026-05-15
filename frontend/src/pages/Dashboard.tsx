import { Outlet } from 'react-router-dom'
import { Sidebar } from '../components/Sidebar'
import { Calendar } from 'lucide-react'

interface DashboardProps {
  title: string
  subtitle: string
}

export function DashboardLayout() {
  return (
    <div className="min-h-screen bg-[#F8FAFC]">
      <Sidebar />
      <main className="ml-[260px]">
        <Outlet />
      </main>
    </div>
  )
}

export function DashboardHeader({ title, subtitle }: DashboardProps) {
  return (
    <header className="sticky top-0 z-40 bg-white/90 backdrop-blur-md border-b border-gray-200 px-8 py-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <span className="inline-flex h-9 w-9 items-center justify-center rounded-lg bg-[#142F4B]/10 text-[#142F4B]">
            <Calendar className="h-5 w-5" />
          </span>
          <div>
            <h1 className="text-lg font-semibold text-gray-900">{title}</h1>
            <p className="text-sm text-gray-500">{subtitle}</p>
          </div>
        </div>
        <div />
      </div>
    </header>
  )
}
