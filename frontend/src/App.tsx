import { useRef, useState } from 'react'
import {
  buscarProfessores,
  ErroComunicacao,
  ErroValidacao,
} from './api/buscaProfessores'
import { BuscaForm } from './components/BuscaForm'
import { Alerta, Carregando } from './components/Feedback'
import { Header } from './components/Header'
import { Resultados } from './components/Resultados'
import type {
  BuscaProfessoresRequest,
  BuscaProfessoresResponse,
} from './types/busca'

type Estado =
  | { tipo: 'ocioso' }
  | { tipo: 'carregando' }
  | { tipo: 'ok'; dados: BuscaProfessoresResponse }
  | { tipo: 'validacao'; mensagem: string }
  | { tipo: 'comunicacao'; mensagem: string }

export default function App() {
  const [estado, setEstado] = useState<Estado>({ tipo: 'ocioso' })
  const ultimaBusca = useRef<BuscaProfessoresRequest | null>(null)
  const requisicaoAtual = useRef<AbortController | null>(null)

  async function buscar(payload: BuscaProfessoresRequest) {
    ultimaBusca.current = payload
    requisicaoAtual.current?.abort()
    const controller = new AbortController()
    requisicaoAtual.current = controller

    setEstado({ tipo: 'carregando' })

    try {
      const dados = await buscarProfessores(payload, controller.signal)
      setEstado({ tipo: 'ok', dados })
    } catch (erro) {
      // Busca cancelada por uma nova requisição: nada a exibir.
      if (erro instanceof DOMException && erro.name === 'AbortError') return

      if (erro instanceof ErroValidacao) {
        setEstado({ tipo: 'validacao', mensagem: erro.message })
      } else if (erro instanceof ErroComunicacao) {
        setEstado({ tipo: 'comunicacao', mensagem: erro.message })
      } else {
        setEstado({
          tipo: 'comunicacao',
          mensagem: 'Ocorreu um erro inesperado. Tente novamente.',
        })
      }
    }
  }

  function tentarNovamente() {
    if (ultimaBusca.current) void buscar(ultimaBusca.current)
  }

  return (
    <div className="min-h-svh bg-page">
      <Header />

      <main className="mx-auto max-w-2xl px-4 pt-8 pb-16">
        <div className="mb-6 text-center">
          <h1 className="text-3xl font-black tracking-tight text-ink sm:text-4xl">
            Encontre o professor <span className="text-duo-green">ideal</span>{' '}
            para você
          </h1>
          <p className="mx-auto mt-3 max-w-md font-bold text-ink-soft">
            Diga o idioma, os dias e o horário que funcionam para você. A gente
            mostra quem está livre.
          </p>
        </div>

        <BuscaForm carregando={estado.tipo === 'carregando'} onBuscar={buscar} />

        <section
          className="mt-8"
          aria-live="polite"
          aria-busy={estado.tipo === 'carregando'}
        >
          {estado.tipo === 'carregando' && <Carregando />}

          {estado.tipo === 'ok' && <Resultados dados={estado.dados} />}

          {estado.tipo === 'validacao' && (
            <Alerta
              variante="aviso"
              titulo="Confira os dados da busca"
              mensagem={estado.mensagem}
            />
          )}

          {estado.tipo === 'comunicacao' && (
            <Alerta
              titulo="Não conseguimos buscar agora"
              mensagem={estado.mensagem}
              onTentarNovamente={tentarNovamente}
            />
          )}
        </section>
      </main>
    </div>
  )
}
