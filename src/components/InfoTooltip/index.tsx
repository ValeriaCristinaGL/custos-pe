'use client'

import { Info } from 'lucide-react'
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '../ui/tooltip'
import { Popover, PopoverContent, PopoverTrigger } from '../ui/popover'
import { useIsMobile } from '../../hooks/use-mobile'

interface InfoTooltipProps {
  text: string
}

export function InfoTooltip({ text }: InfoTooltipProps) {
  const isMobile = useIsMobile()

  const InfoButton = (
    <button
      type="button"
      className="ml-1 inline-flex shrink-0 cursor-pointer justify-center rounded-full align-middle text-gray-400 transition-colors hover:bg-blue-50 hover:text-blue-500 focus:outline-none"
      onClick={(e) => {
        e.stopPropagation()
      }}
    >
      <Info size={15} />
      <span className="sr-only">Informação</span>
    </button>
  )

  // --- COMPORTAMENTO MOBILE (POPOVER) ---
  if (isMobile) {
    return (
      <Popover>
        <PopoverTrigger asChild onClick={(e) => e.stopPropagation()}>
          {InfoButton}
        </PopoverTrigger>
        <PopoverContent
          className="z-50 max-w-[250px] border-none bg-gray-900 p-3 text-xs text-white shadow-md"
          side="top"
          onClick={(e) => e.stopPropagation()}
        >
          <p className="whitespace-pre-line">{text}</p>
        </PopoverContent>
      </Popover>
    )
  }

  // --- COMPORTAMENTO DESKTOP (TOOLTIP) ---
  return (
    <TooltipProvider>
      <Tooltip delayDuration={300}>
        <TooltipTrigger asChild onClick={(e) => e.stopPropagation()}>
          {InfoButton}
        </TooltipTrigger>
        <TooltipContent
          className="z-50 max-w-[250px] bg-gray-900 text-xs text-white"
          sideOffset={5}
        >
          <p className="whitespace-pre-line">{text}</p>
        </TooltipContent>
      </Tooltip>
    </TooltipProvider>
  )
}
