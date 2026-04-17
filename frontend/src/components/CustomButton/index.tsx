/* eslint-disable prettier/prettier */
import { Slot } from '@radix-ui/react-slot'
import React, { type ReactElement, forwardRef, type ButtonHTMLAttributes } from 'react'

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  text?: string
  iconLeft?: ReactElement<{ className?: string }>
  iconRight?: ReactElement<{ className?: string }>
  iconSize?: 'sm' | 'md' | 'lg'
  variant?:
    | 'primario'
    | 'secundario'
    | 'erro'
    | 'erroDelineado'
    | 'atencaoDelineado'
    | 'salvarDelineado'
    | 'delineado'
    | 'fantasma'
    | 'link'
    | 'iconx'
    | 'header'
  size?: 'sm' | 'md' | 'lg'
  isLoading?: boolean
  disabled?: boolean
  asChild?: boolean
}

export const CustomButton = forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      text,
      iconLeft,
      iconRight,
      iconSize = 'sm',
      disabled = false,
      isLoading = false,
      variant,
      size = 'sm',
      asChild = false,
      ...props
    },
    ref
  ) => {
    const isDisabled = disabled || isLoading
    const isIconOnly = !text && (iconLeft || iconRight)
    const paddingClass = isIconOnly ? 'p-3' : 'py-3 px-4'

    const variantClasses = {
      primario:
        'bg-azul-base text-white hover:enabled:bg-azul-marinho-200 focus:enabled:bg-azul-marinho-200',
      secundario:
        'bg-amarelo-50 text-azul-marinho-200 hover:enabled:bg-amarelo-200',
      erro: 'bg-red-500 text-white hover:enabled:bg-red-700',
      erroDelineado:
        'border border-red-600 text-red-600 hover:enabled:bg-red-50 focus:enabled:bg-red-50',
      atencaoDelineado:
        'border border-yellow-600 text-yellow-600 hover:enabled:bg-yellow-50 focus:enabled:bg-yellow-50',
      salvarDelineado:
        'border border-green-600 text-green-600 hover:enabled:bg-green-50 focus:enabled:bg-green-50',
      delineado:
        'bg-transparent border border-azul-marinho-200 text-azul-marinho-200 hover:enabled:bg-blue-50 focus:enabled:bg-blue-50',
      fantasma:
        'bg-transparent text-azul-marinho-100 hover:enabled:bg-gray-100',
      link: 'text-blue-600 hover:enabled:underline',
      iconx: 'bg-transparent text-gray-gray-500',
      header: 'hover:border-b hover:text-azul-base hover:border-azul-base',
    }

    const sizeClasses = {
      sm: 'text-xs',
      md: 'text-sm',
      lg: 'text-base',
    }

    const iconSizeClasses = {
      sm: 'w-[16px] h-[16px]',
      md: 'w-[20px] h-[20px]',
      lg: 'w-[24px] h-[24px]',
    }

    const finalVariant = variant ?? 'primario'

    const finalClass = `
      outline-none w-full inline-flex w-fit items-center justify-center gap-2 rounded-[8px] font-semibold text-nowrap transition-all
      ${variantClasses[finalVariant]}
      ${sizeClasses[size]}
      ${paddingClass}
      ${isDisabled ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer'}
    `

    const renderIcon = (icon?: ReactElement<{ className?: string }>) =>
      icon
        ? React.cloneElement(icon, { className: iconSizeClasses[iconSize] })
        : null

    const Comp = asChild ? Slot : 'button'

    return (
      <Comp
        ref={ref}
        {...props}
        className={`${finalClass} ${props.className ?? ''}`}
        disabled={isDisabled}
      >
        <span className="flex items-center gap-2">
          {renderIcon(iconLeft)}
          {text}
          {renderIcon(iconRight)}
        </span>
      </Comp>
    )
  }
)

CustomButton.displayName = 'CustomButton'

export type { ButtonProps }
