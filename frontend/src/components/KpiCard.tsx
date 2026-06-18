import type { ReactNode } from 'react'
import { Info } from 'lucide-react'

interface KpiCardProps {
  title: string
  value: string
  subtitle?: string
  icon?: ReactNode
  info?: string
  trend?: {
    value: string
    positive: boolean
  }
}

export function KpiCard({
  title,
  value,
  subtitle,
  icon,
  info,
  trend,
}: KpiCardProps) {
  return (
    <div className="rounded-xl border border-gray-200 bg-white px-6 py-4 flex flex-col gap-3 transition-shadow duration-300 hover:shadow-md">
      <div className="flex items-start justify-between gap-3">
        <div className="flex items-center gap-2">
          <p className="text-sm font-medium text-gray-500">{title}</p>

          {info && (
            <div className="group relative inline-flex">
              <button
                type="button"
                className="text-gray-400 transition-colors hover:text-gray-600"
                aria-label={`Informação sobre ${title}`}
              >
                <Info className="h-4 w-4" />
              </button>

              <div className="pointer-events-none absolute left-1/2 top-6 z-50 w-56 -translate-x-1/2 rounded-lg bg-gray-900 px-3 py-2 text-xs font-normal text-white opacity-0 shadow-lg transition-opacity group-hover:opacity-100">
                {info}

                <div className="absolute -top-1 left-1/2 h-2 w-2 -translate-x-1/2 rotate-45 bg-gray-900" />
              </div>
            </div>
          )}
        </div>

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
            <span
              className={`flex items-center gap-1 font-medium ${
                trend.positive ? 'text-emerald-600' : 'text-red-500'
              }`}
            >
              <svg className="h-3 w-3" viewBox="0 0 12 12" fill="none">
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
