import { useState } from 'react'
import type { ComponentType, FormEvent, ReactNode } from 'react'
import { DIAS, DURACOES, IDIOMAS } from '../constants/opcoes'
import type { Bandeira as BandeiraIdioma } from '../constants/opcoes'
import type { PropsBandeira } from './flags'
import { BandeiraEspanha, BandeiraFranca, BandeiraReinoUnido } from './flags'
import type { BuscaProfessoresRequest } from '../types/busca'

const BANDEIRAS: Record<BandeiraIdioma, ComponentType<PropsBandeira>> = {
  'reino-unido': BandeiraReinoUnido,
  espanha: BandeiraEspanha,
  franca: BandeiraFranca,
}

interface Props {
  carregando: boolean
  onBuscar: (payload: BuscaProfessoresRequest) => void
}

function TituloPasso({
  numero,
  children,
}: {
  numero: number
  children: ReactNode
}) {
  return (
    <legend className="mb-3 flex items-center gap-2 text-lg font-extrabold text-ink">
      <span className="grid size-6 place-items-center rounded-full bg-duo-blue text-sm text-white">
        {numero}
      </span>
      {children}
    </legend>
  )
}

const BASE_CARTAO =
  'cursor-pointer rounded-2xl border-2 border-b-4 font-extrabold transition-colors'
const CARTAO_ATIVO = 'border-duo-blue bg-accent-soft text-accent'
const CARTAO_INATIVO =
  'border-line bg-surface text-ink-soft hover:bg-surface-soft'

const CAMPO =
  'w-full rounded-2xl border-2 border-line bg-surface-soft px-4 py-3 text-lg font-bold text-ink outline-none transition-colors focus:border-duo-blue'

export function BuscaForm({ carregando, onBuscar }: Props) {
  const [idioma, setIdioma] = useState(IDIOMAS[0].valor)
  const [dias, setDias] = useState<string[]>(['segunda'])
  const [inicio, setInicio] = useState('18:00')
  const [fim, setFim] = useState('20:00')
  const [duracao, setDuracao] = useState<number>(60)

  function alternarDia(valor: string) {
    setDias((atual) =>
      atual.includes(valor)
        ? atual.filter((d) => d !== valor)
        : [...atual, valor],
    )
  }

  function enviar(evento: FormEvent) {
    evento.preventDefault()
    onBuscar({
      idioma,
      dias,
      inicioFaixa: inicio,
      fimFaixa: fim,
      duracaoMinutos: duracao,
    })
  }

  return (
    <form onSubmit={enviar} className="card-duo space-y-8 p-5 sm:p-8">
      {/* Idioma */}
      <fieldset>
        <TituloPasso numero={1}>Qual idioma você quer aprender?</TituloPasso>
        <div className="grid grid-cols-3 gap-3">
          {IDIOMAS.map((opcao) => {
            const selecionado = opcao.valor === idioma
            const Bandeira = BANDEIRAS[opcao.bandeira]
            return (
              <button
                key={opcao.valor}
                type="button"
                aria-pressed={selecionado}
                onClick={() => setIdioma(opcao.valor)}
                className={`${BASE_CARTAO} flex flex-col items-center gap-2 px-2 py-4 ${
                  selecionado ? CARTAO_ATIVO : CARTAO_INATIVO
                }`}
              >
                <span className="overflow-hidden rounded-md ring-1 ring-black/10 dark:ring-white/20">
                  <Bandeira className="block h-8 w-12" />
                </span>
                <span className="text-sm sm:text-base">{opcao.rotulo}</span>
              </button>
            )
          })}
        </div>
      </fieldset>

      {/* Dias */}
      <fieldset>
        <TituloPasso numero={2}>Em quais dias você pode?</TituloPasso>
        <div className="flex flex-wrap gap-2">
          {DIAS.map((dia) => {
            const selecionado = dias.includes(dia.valor)
            return (
              <button
                key={dia.valor}
                type="button"
                aria-pressed={selecionado}
                aria-label={dia.rotulo}
                onClick={() => alternarDia(dia.valor)}
                className={`${BASE_CARTAO} min-w-16 px-4 py-2.5 ${
                  selecionado
                    ? 'border-duo-blue bg-duo-blue text-white'
                    : CARTAO_INATIVO
                }`}
              >
                {dia.curto}
              </button>
            )
          })}
        </div>
        {dias.length === 0 && (
          <p className="mt-2 text-sm font-bold text-danger">
            Escolha ao menos um dia.
          </p>
        )}
      </fieldset>

      {/* Horário e duração */}
      <fieldset>
        <TituloPasso numero={3}>Em que horário e por quanto tempo?</TituloPasso>
        <div className="grid gap-4 sm:grid-cols-2">
          <label className="block">
            <span className="mb-1.5 block text-sm font-extrabold text-ink-soft">
              Início da faixa
            </span>
            <input
              type="time"
              step={1800}
              required
              value={inicio}
              onChange={(e) => setInicio(e.target.value)}
              className={CAMPO}
            />
          </label>
          <label className="block">
            <span className="mb-1.5 block text-sm font-extrabold text-ink-soft">
              Fim da faixa
            </span>
            <input
              type="time"
              step={1800}
              required
              value={fim}
              onChange={(e) => setFim(e.target.value)}
              className={CAMPO}
            />
          </label>
        </div>

        <div className="mt-4">
          <span className="mb-1.5 block text-sm font-extrabold text-ink-soft">
            Duração da aula
          </span>
          <div className="grid grid-cols-3 gap-3">
            {DURACOES.map((minutos) => {
              const selecionado = minutos === duracao
              return (
                <button
                  key={minutos}
                  type="button"
                  aria-pressed={selecionado}
                  onClick={() => setDuracao(minutos)}
                  className={`${BASE_CARTAO} px-3 py-3 ${
                    selecionado
                      ? 'border-duo-blue bg-duo-blue text-white'
                      : CARTAO_INATIVO
                  }`}
                >
                  {minutos} min
                </button>
              )
            })}
          </div>
        </div>
      </fieldset>

      <button
        type="submit"
        disabled={carregando}
        className="btn-3d w-full border-duo-green-dark bg-duo-green text-white hover:brightness-105 sm:text-lg"
      >
        {carregando ? 'Buscando…' : 'Buscar professores'}
      </button>
    </form>
  )
}
