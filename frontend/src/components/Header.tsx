import { TemaToggle } from './TemaToggle'
import { IconeGraduacao } from './icons'

export function Header() {
  return (
    <header className="sticky top-0 z-20 border-b-2 border-line bg-page-glass backdrop-blur">
      <div className="mx-auto flex max-w-4xl items-center justify-between gap-4 px-4 py-3">
        <div className="flex items-center gap-3">
          <span className="grid size-10 place-items-center rounded-2xl bg-duo-green text-white shadow-[0_3px_0_0_#46a302]">
            <IconeGraduacao className="size-6" />
          </span>
          <span className="text-lg font-extrabold tracking-tight text-ink">
            Encontre um Professor
          </span>
        </div>
        <div className="flex items-center gap-3">
          <span className="hidden rounded-full bg-surface-soft px-3 py-1 text-xs font-extrabold tracking-wide text-ink-soft uppercase sm:block">
            Escola de Idiomas
          </span>
          <TemaToggle />
        </div>
      </div>
    </header>
  )
}
