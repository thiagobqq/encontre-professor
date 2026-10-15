/**
 * Opções do formulário. Os `valor` casam exatamente com o que a API espera
 * (`DiaDaSemanaParser` e a comparação de idioma do backend).
 */

/** Chaves das bandeiras desenhadas em `components/flags.tsx`. */
export type Bandeira = 'reino-unido' | 'espanha' | 'franca'

export interface OpcaoIdioma {
  valor: string
  rotulo: string
  /** Bandeira (SVG) exibida no cartão. */
  bandeira: Bandeira
}

export const IDIOMAS: OpcaoIdioma[] = [
  { valor: 'Inglês', rotulo: 'Inglês', bandeira: 'reino-unido' },
  { valor: 'Espanhol', rotulo: 'Espanhol', bandeira: 'espanha' },
  { valor: 'Francês', rotulo: 'Francês', bandeira: 'franca' },
]

export interface OpcaoDia {
  valor: string
  rotulo: string
  curto: string
}

export const DIAS: OpcaoDia[] = [
  { valor: 'segunda', rotulo: 'Segunda-feira', curto: 'Seg' },
  { valor: 'terca', rotulo: 'Terça-feira', curto: 'Ter' },
  { valor: 'quarta', rotulo: 'Quarta-feira', curto: 'Qua' },
  { valor: 'quinta', rotulo: 'Quinta-feira', curto: 'Qui' },
  { valor: 'sexta', rotulo: 'Sexta-feira', curto: 'Sex' },
  { valor: 'sabado', rotulo: 'Sábado', curto: 'Sáb' },
  { valor: 'domingo', rotulo: 'Domingo', curto: 'Dom' },
]

/** Valores aceitos pela API (a validação recusa qualquer outro). */
export const DURACOES = [30, 60, 90] as const

/** Cores dos avatares, girando pela paleta da marca. */
export const CORES_AVATAR = [
  'bg-duo-green text-white',
  'bg-duo-blue text-white',
  'bg-duo-purple text-white',
  'bg-duo-orange text-white',
  'bg-duo-red text-white',
] as const
