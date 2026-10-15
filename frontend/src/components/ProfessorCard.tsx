import { CORES_AVATAR } from '../constants/opcoes'
import type { ProfessorDisponivel } from '../types/busca'
import { IconeEstrela } from './icons'

interface Props {
  professor: ProfessorDisponivel
  posicao: number
}

export function ProfessorCard({ professor, posicao }: Props) {
  const cor = CORES_AVATAR[posicao % CORES_AVATAR.length]
  const inicial = professor.nome.charAt(0).toUpperCase()
  const aulas = professor.aulasNaSemana
  const textoAulas =
    aulas === 0
      ? 'Sem aulas marcadas'
      : `${aulas} ${aulas === 1 ? 'aula' : 'aulas'} na semana`

  return (
    <article className="card-duo animate-pop flex gap-4 border-b-4 p-5">
      <span
        aria-hidden
        className={`grid size-14 shrink-0 place-items-center rounded-2xl text-2xl font-black ${cor}`}
      >
        {inicial}
      </span>

      <div className="min-w-0 flex-1">
        <div className="flex flex-wrap items-center gap-x-3 gap-y-1">
          <h3 className="text-xl font-extrabold text-ink">{professor.nome}</h3>
          {posicao === 0 && (
            <span className="inline-flex items-center gap-1 rounded-full bg-warning-soft px-2.5 py-0.5 text-xs font-extrabold text-warning">
              <IconeEstrela className="size-3.5" />
              Melhor opção
            </span>
          )}
        </div>

        <p className="mt-0.5 text-sm font-bold text-ink-faint">{textoAulas}</p>

        <div className="mt-3">
          <p className="mb-1.5 text-xs font-extrabold tracking-wide text-ink-soft uppercase">
            Horários possíveis
          </p>
          <ul className="flex flex-wrap gap-2">
            {professor.horarios.map((horario) => (
              <li
                key={horario}
                className="rounded-xl bg-success-soft px-3 py-1 font-extrabold text-success"
              >
                {horario}
              </li>
            ))}
          </ul>
        </div>
      </div>
    </article>
  )
}
