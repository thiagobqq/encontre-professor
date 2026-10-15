import { useTema } from '../hooks/useTema'
import { IconeLua, IconeSol } from './icons'

export function TemaToggle() {
  const { tema, alternar } = useTema()
  const escuro = tema === 'escuro'

  return (
    <button
      type="button"
      onClick={alternar}
      aria-pressed={escuro}
      aria-label={escuro ? 'Ativar modo claro' : 'Ativar modo escuro'}
      title={escuro ? 'Modo claro' : 'Modo escuro'}
      className="grid size-10 cursor-pointer place-items-center rounded-2xl border-2 border-line bg-surface text-ink-soft transition-colors hover:bg-surface-soft hover:text-ink"
    >
      {escuro ? (
        <IconeSol className="size-5" />
      ) : (
        <IconeLua className="size-5" />
      )}
    </button>
  )
}
