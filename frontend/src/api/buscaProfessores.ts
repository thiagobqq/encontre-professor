import type {
  BuscaProfessoresRequest,
  BuscaProfessoresResponse,
} from '../types/busca'

/**
 * Chamada à API. Em desenvolvimento o Vite faz proxy de `/api` para
 * `http://localhost:5120` (ver `vite.config.ts`), então a base fica vazia.
 * Em produção dá para apontar com `VITE_API_URL`.
 */
const BASE_URL = import.meta.env.VITE_API_URL ?? ''

/** Erro de validação devolvido pela API com 400 (`{ "mensagem": "..." }`). */
export class ErroValidacao extends Error {
  constructor(mensagem: string) {
    super(mensagem)
    this.name = 'ErroValidacao'
  }
}

/** Falha de rede, timeout, 5xx ou resposta ilegível. */
export class ErroComunicacao extends Error {
  constructor(mensagem: string) {
    super(mensagem)
    this.name = 'ErroComunicacao'
  }
}

const MENSAGEM_COMUNICACAO =
  'Não foi possível falar com o servidor. Verifique se a API está no ar e tente novamente.'

type CorpoErro = {
  mensagem?: string
  title?: string
  detail?: string
  errors?: Record<string, string[]>
}

/** Extrai uma mensagem legível de um corpo de erro (inclui o ProblemDetails do ASP.NET). */
function extrairMensagem(corpo: unknown): string | null {
  if (!corpo || typeof corpo !== 'object') return null
  const { mensagem, title, detail, errors } = corpo as CorpoErro

  if (mensagem) return mensagem
  if (errors) {
    const primeiro = Object.values(errors).flat().find(Boolean)
    if (primeiro) return primeiro
  }
  return detail ?? title ?? null
}

export async function buscarProfessores(
  payload: BuscaProfessoresRequest,
  signal?: AbortSignal,
): Promise<BuscaProfessoresResponse> {
  let resposta: Response
  try {
    resposta = await fetch(`${BASE_URL}/api/professores/busca`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
      signal,
    })
  } catch (erro) {
    // AbortError é cancelamento intencional: repassa para o chamador ignorar.
    if (erro instanceof DOMException && erro.name === 'AbortError') throw erro
    throw new ErroComunicacao(MENSAGEM_COMUNICACAO)
  }

  if (resposta.status === 400) {
    const corpo = await resposta.json().catch((erro: unknown) => {
      // AbortError é cancelamento intencional: repassa para o chamador ignorar.
      if (erro instanceof DOMException && erro.name === 'AbortError') throw erro
      return null
    })
    throw new ErroValidacao(
      extrairMensagem(corpo) ?? 'Os dados informados são inválidos.',
    )
  }

  if (!resposta.ok) {
    throw new ErroComunicacao(MENSAGEM_COMUNICACAO)
  }

  try {
    return (await resposta.json()) as BuscaProfessoresResponse
  } catch (erro) {
    // AbortError é cancelamento intencional: repassa para o chamador ignorar.
    if (erro instanceof DOMException && erro.name === 'AbortError') throw erro
    throw new ErroComunicacao(
      'A resposta do servidor veio em um formato inesperado.',
    )
  }
}
