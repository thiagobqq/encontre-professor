import { IconeAlerta, IconeBusca } from './icons'

interface AlertaProps {
  titulo: string
  mensagem: string
  variante?: 'erro' | 'aviso'
  onTentarNovamente?: () => void
}

export function Alerta({
  titulo,
  mensagem,
  variante = 'erro',
  onTentarNovamente,
}: AlertaProps) {
  const estilo =
    variante === 'erro'
      ? 'border-danger bg-danger-soft text-danger'
      : 'border-warning bg-warning-soft text-warning'

  return (
    <div
      role="alert"
      className={`animate-pop rounded-3xl border-2 border-b-4 p-5 ${estilo}`}
    >
      <div className="flex items-start gap-3">
        <IconeAlerta className="mt-0.5 size-6 shrink-0" />
        <div className="flex-1">
          <h3 className="font-extrabold">{titulo}</h3>
          <p className="mt-0.5 font-bold opacity-90">{mensagem}</p>
          {onTentarNovamente && (
            <button
              type="button"
              onClick={onTentarNovamente}
              className="mt-3 cursor-pointer rounded-xl border-2 border-current px-4 py-1.5 text-sm font-extrabold uppercase transition-colors hover:bg-white/30"
            >
              Tentar novamente
            </button>
          )}
        </div>
      </div>
    </div>
  )
}

export function Carregando() {
  return (
    <div className="space-y-3" aria-hidden>
      {[0, 1, 2].map((i) => (
        <div
          key={i}
          className="card-duo flex animate-pulse gap-4 p-5"
          style={{ animationDelay: `${i * 120}ms` }}
        >
          <div className="size-14 shrink-0 rounded-2xl bg-surface-soft" />
          <div className="flex-1 space-y-3 py-1">
            <div className="h-4 w-32 rounded-full bg-surface-soft" />
            <div className="h-3 w-40 rounded-full bg-surface-soft" />
            <div className="flex gap-2 pt-1">
              <div className="h-6 w-16 rounded-lg bg-surface-soft" />
              <div className="h-6 w-16 rounded-lg bg-surface-soft" />
              <div className="h-6 w-16 rounded-lg bg-surface-soft" />
            </div>
          </div>
        </div>
      ))}
    </div>
  )
}

interface VazioProps {
  temNaoElegiveis: boolean
}

export function EstadoVazio({ temNaoElegiveis }: VazioProps) {
  return (
    <div className="card-duo animate-pop px-6 py-12 text-center">
      <span className="mx-auto grid size-16 place-items-center rounded-full bg-surface-soft text-ink-faint">
        <IconeBusca className="size-8" />
      </span>
      <h3 className="mt-4 text-xl font-extrabold text-ink">
        Nenhum professor encontrado
      </h3>
      <p className="mx-auto mt-2 max-w-md font-bold text-ink-soft">
        Ninguém ficou livre para esse combinado de idioma, dias e horário.
        {temNaoElegiveis
          ? ' Veja abaixo o motivo de cada professor que ficou de fora.'
          : ' Tente ampliar a faixa de horário ou incluir mais dias.'}
      </p>
    </div>
  )
}
