/**
 * Bandeiras em SVG. Nada de emoji: cada bandeira é desenhada aqui e recebe
 * tamanho pelo `className` de quem a usa. Todas usam a mesma proporção (3:2)
 * para ficarem alinhadas no formulário.
 */
export interface PropsBandeira {
  className?: string
}

export function BandeiraReinoUnido({ className }: PropsBandeira) {
  return (
    <svg viewBox="0 0 30 20" aria-hidden="true" className={className}>
      <rect width="30" height="20" fill="#012169" />
      <path
        d="M0 0 30 20M30 0 0 20"
        stroke="#ffffff"
        strokeWidth="4"
        fill="none"
      />
      <path
        d="M0 0 30 20M30 0 0 20"
        stroke="#c8102e"
        strokeWidth="1.5"
        fill="none"
      />
      <path d="M15 0V20M0 10H30" stroke="#ffffff" strokeWidth="6" fill="none" />
      <path d="M15 0V20M0 10H30" stroke="#c8102e" strokeWidth="3" fill="none" />
    </svg>
  )
}

export function BandeiraEspanha({ className }: PropsBandeira) {
  return (
    <svg viewBox="0 0 30 20" aria-hidden="true" className={className}>
      <rect width="30" height="20" fill="#aa151b" />
      <rect y="5" width="30" height="10" fill="#f1bf00" />
    </svg>
  )
}

export function BandeiraFranca({ className }: PropsBandeira) {
  return (
    <svg viewBox="0 0 30 20" aria-hidden="true" className={className}>
      <rect width="10" height="20" fill="#002395" />
      <rect x="10" width="10" height="20" fill="#ffffff" />
      <rect x="20" width="10" height="20" fill="#ed2939" />
    </svg>
  )
}
