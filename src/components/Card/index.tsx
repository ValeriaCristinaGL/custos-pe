'use client'

import { useResponsiveFlex } from '../../hooks/useResponsiveFlex'
import type {
  ReactNode,
  HTMLAttributes,
  CSSProperties,
  KeyboardEvent,
} from 'react'
import { InfoTooltip } from '../InfoTooltip'

type CardProps = HTMLAttributes<HTMLDivElement> & {
  titulo?: string
  icone?: ReactNode
  quantidade?: number | string | ReactNode
  medida?: string
  comparacao?: React.ReactNode
  descricao?: string
  corFundo?: string
  corBorda?: string
  children?: ReactNode
  className?: string
  layout?: 'vertical' | 'horizontal'
  onClick?: () => void
  isSelected?: boolean
  comparacaoColorMode?: 'positivo-verde' | 'positivo-vermelho'
  comparacaoColorDefault?: 'success' | 'danger' | 'neutral'
  heightMode?: 'auto' | 'full'
  infoDescription?: string
}

export default function Card({
  titulo,
  icone,
  quantidade,
  medida,
  comparacao,
  descricao,
  corFundo,
  corBorda,
  children,
  className,
  layout = 'vertical',
  onClick,
  isSelected = false,
  comparacaoColorMode = 'positivo-verde',
  comparacaoColorDefault = 'neutral',
  heightMode = 'full',
  infoDescription,
  ...props
}: CardProps) {
  const { ref, isRow } = useResponsiveFlex()

  const finalBorderColor = isSelected ? '#3B82F6' : corBorda || '#E5E7EB'
  const finalBgColor = isSelected ? '#EFF6FF' : corFundo || 'white'

  const interactiveClass = onClick
    ? 'cursor-pointer transition-all duration-200 hover:shadow-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2'
    : ''

  const cardStyle: CSSProperties = {
    boxShadow: isSelected
      ? '0 0 0 2px rgba(59, 130, 246, 0.5)'
      : '0px 1px 2px rgba(0,0,0,0.1), 0px 1px 3px rgba(0,0,0,0.1)',
    backgroundColor: finalBgColor,
    borderColor: finalBorderColor,
  }

  const layoutClasses =
    layout === 'horizontal' ? 'justify-between' : 'flex-col gap-3'

  const handleKeyDown = (e: KeyboardEvent<HTMLDivElement>) => {
    if (onClick && (e.key === 'Enter' || e.key === ' ')) {
      e.preventDefault()
      onClick()
    }
  }

  const comparacaoStr = String(comparacao)
  const isPositive = comparacaoStr.startsWith('+')
  const isNegative = comparacaoStr.startsWith('-')

  const comparacaoColor =
    comparacaoColorMode === 'positivo-verde'
      ? isPositive
        ? 'text-green-700'
        : isNegative
          ? 'text-red-600'
          : 'text-neutral-gray-600'
      : isPositive
        ? 'text-red-600'
        : isNegative
          ? 'text-green-700'
          : 'text-neutral-gray-600'

  const comparacaoColorClass =
    comparacaoColorDefault === 'danger'
      ? 'text-red-600'
      : comparacaoColorDefault === 'success'
        ? 'text-green-700'
        : 'text-neutral-gray-600'

  const finalComparacao = `text-xs font-medium ${comparacaoColorClass} ${comparacaoColor}`
  const heightClass = heightMode === 'full' ? 'h-full flex-1' : 'h-auto'

  return (
    <div
      {...props}
      onClick={onClick}
      onKeyDown={onClick ? handleKeyDown : undefined}
      role={onClick ? 'button' : undefined}
      tabIndex={onClick ? 0 : undefined}
      className={`flex w-full max-w-full flex-col self-start overflow-hidden rounded-[12px] border p-4 ${heightClass} ${layoutClasses} ${interactiveClass} ${
        className || ''
      }`}
      style={cardStyle}
    >
      {layout === 'horizontal' ? (
        // --- Layout Horizontal ---
        <>
          <div className="flex items-center justify-between gap-3">
            <h2 className="text-neutro-gray-950 text-base leading-snug font-medium">
              {titulo}
              {infoDescription && <InfoTooltip text={infoDescription} />}
            </h2>
          </div>
          <div className="flex gap-2">
            {comparacao && <div className={finalComparacao}>{comparacao}</div>}

            <div className="flex items-end gap-1">
              {quantidade !== undefined && (
                <span className="text-neutro-gray-950 text-2xl font-semibold">
                  {quantidade}
                </span>
              )}
              {medida && (
                <span className="text-neutro-gray-600 text-sm">{medida}</span>
              )}
            </div>
            {icone && <div className="mt-0.75 ml-2">{icone}</div>}
          </div>
          {children && (
            <div className="mt-4 flex w-full flex-1 flex-col">{children}</div>
          )}
        </>
      ) : (
        // --- Layout Vertical ---
        <div className="flex h-full flex-1 flex-col justify-between">
          <div className="flex justify-between items-center gap-3">
            <h2 className="text-neutro-gray-950 text-base leading-snug font-medium">
              {titulo}
              {infoDescription && <InfoTooltip text={infoDescription} />}
            </h2>
            {icone && (
              <div className="text-[#7C7C7C] bg-[#E6ECF2] rounded-md px-1 py-2 items-center justify-center">
                {icone}
              </div>
            )}
          </div>
          {descricao && (
            <span className="text-xs text-neutro-gray-700">{descricao}</span>
          )}
          <div>
            <div className="flex items-baseline gap-1">
              {quantidade !== undefined && (
                <span className="text-neutro-gray-950 text-2xl font-semibold">
                  {quantidade}
                </span>
              )}
              {medida && (
                <span className="text-neutro-gray-600 text-sm">{medida}</span>
              )}
            </div>
            {comparacao && !descricao && (
              <div className={finalComparacao}>{comparacao}</div>
            )}
            {comparacao && descricao && (
              <div
                ref={ref}
                className={`text-neutro-gray-600 flex text-xs font-medium ${
                  isRow ? 'flex-row justify-between' : 'flex-col'
                }`}
              >
                <div className={finalComparacao}>{comparacao}</div>
                <div className="text-neutro-gray-700">{descricao}</div>
              </div>
            )}
          </div>
          {children && <div>{children}</div>}
        </div>
      )}
    </div>
  )
}
