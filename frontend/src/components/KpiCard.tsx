import type { ReactNode } from 'react'

interface KpiCardProps {
  title: string
  value: string
  subtitle?: string
  icon?: ReactNode
  trend?: {
    value: string
    positive: boolean
  }
}

export function KpiCard({ title, value, subtitle, icon, trend }: KpiCardProps) {
  return (
    <div className="bg-white rounded-xl border border-gray-200 p-6 flex flex-col gap-3 hover:shadow-md transition-shadow duration-300">
      <div className="flex items-center justify-between">
        <p className="text-sm text-gray-500 font-medium">{title}</p>
        {icon && (
          <span className="inline-flex h-9 w-9 items-center justify-center rounded-lg bg-gray-100 text-gray-500">
            {icon}
          </span>
        )}
      </div>
      <p className="text-2xl font-bold text-gray-900">{value}</p>
      {(subtitle || trend) && (
        <div className="flex items-center gap-2 text-xs">
          {trend && (
            <span className={`flex items-center gap-1 font-medium ${trend.positive ? 'text-emerald-600' : 'text-red-500'}`}>
              <svg className="w-3 h-3" viewBox="0 0 12 12" fill="none">
                <path
                  d={trend.positive ? 'M6 2L10 7H2L6 2Z' : 'M6 10L2 5H10L6 10Z'}
                  fill="currentColor"
                />
              </svg>
              {trend.value}
            </span>
          )}
          {subtitle && <span className="text-gray-400">{subtitle}</span>}
        </div>
      )}
    </div>
  )
}
