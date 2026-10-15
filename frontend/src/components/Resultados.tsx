import { EstadoVazio } from './Feedback'
import { IconeCheckCirculo } from './icons'
import { NaoElegiveis } from './NaoElegiveis'
import { ProfessorCard } from './ProfessorCard'
import type { BuscaProfessoresResponse } from '../types/busca'

interface Props {
  dados: BuscaProfessoresResponse
}

export function Resultados({ dados }: Props) {
  const { professores, naoElegiveis } = dados

  if (professores.length === 0) {
    return (
      <div className="space-y-4">
        <EstadoVazio temNaoElegiveis={naoElegiveis.length > 0} />
        <NaoElegiveis professores={naoElegiveis} />
      </div>
    )
  }

  const total = professores.length
  const textoTotal =
    total === 1 ? '1 professor disponível' : `${total} professores disponíveis`

  return (
    <div className="space-y-4">
      <p className="flex items-center gap-2 px-1 text-sm font-extrabold tracking-wide text-success uppercase">
        <IconeCheckCirculo className="size-5" />
        {textoTotal}
      </p>

      <div className="space-y-3">
        {professores.map((professor, indice) => (
          <ProfessorCard
            key={professor.id}
            professor={professor}
            posicao={indice}
          />
        ))}
      </div>

      <NaoElegiveis professores={naoElegiveis} />
    </div>
  )
}
