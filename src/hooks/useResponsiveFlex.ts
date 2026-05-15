import { useState, useEffect, useRef } from 'react'

export function useResponsiveFlex(threshold: number = 260) {
  const ref = useRef<HTMLDivElement | null>(null)
  const [isRow, setIsRow] = useState(false)

  useEffect(() => {
    if (!ref.current) return

    const observer = new ResizeObserver(([entry]) => {
      const width = entry.contentRect.width
      setIsRow(width >= threshold)
    })

    observer.observe(ref.current)

    return () => observer.disconnect()
  }, [threshold])

  return { ref, isRow }
}
