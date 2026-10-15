import { useState } from 'react'
import type { ProfessorNaoElegivel } from '../types/busca'
import { IconeSetaBaixo } from './icons'

interface Props {
  professores: ProfessorNaoElegivel[]
}

const CORES_MOTIVO: Record<string, string> = {
  'Não ensina o idioma': 'bg-danger-soft text-danger',
  'Agenda bloqueada': 'bg-warning-soft text-warning',
  'Sem horário em comum nos dias pedidos': 'bg-surface-soft text-ink-soft',
}

export function NaoElegiveis({ professores }: Props) {
  const [aberto, setAberto] = useState(false)

  if (professores.length === 0) return null

  return (
    <section className="card-duo overflow-hidden">
      <button
        type="button"
        onClick={() => setAberto((v) => !v)}
        aria-expanded={aberto}
        className="flex w-full cursor-pointer items-center justify-between gap-3 px-5 py-4 text-left hover:bg-surface-soft"
      >
        <span className="font-extrabold text-ink">
          {professores.length}{' '}
          {professores.length === 1
            ? 'professor ficou de fora'
            : 'professores ficaram de fora'}
        </span>
        <IconeSetaBaixo
          className={`size-5 shrink-0 text-ink-faint transition-transform ${
            aberto ? 'rotate-180' : ''
          }`}
        />
      </button>

      {aberto && (
        <ul className="divide-y-2 divide-line border-t-2 border-line">
          {professores.map((professor) => (
            <li
              key={professor.id}
              className="flex items-center justify-between gap-3 px-5 py-3"
            >
              <span className="font-bold text-ink-soft">{professor.nome}</span>
              <span
                className={`rounded-full px-3 py-1 text-xs font-extrabold ${
                  CORES_MOTIVO[professor.motivo] ?? 'bg-surface-soft text-ink-soft'
                }`}
              >
                {professor.motivo}
              </span>
            </li>
          ))}
        </ul>
      )}
    </section>
  )
}
