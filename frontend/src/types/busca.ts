/**
 * Contratos de entrada e saída de `POST /api/professores/busca`.
 * Espelham `Backend/Models/BuscaProfessores{Request,Response}.cs`.
 */

export interface BuscaProfessoresRequest {
  idioma: string
  dias: string[]
  inicioFaixa: string
  fimFaixa: string
  duracaoMinutos: number
}

export interface ProfessorDisponivel {
  id: number
  nome: string
  aulasNaSemana: number
  horarios: string[]
}

export interface ProfessorNaoElegivel {
  id: number
  nome: string
  motivo: string
}

export interface BuscaProfessoresResponse {
  professores: ProfessorDisponivel[]
  naoElegiveis: ProfessorNaoElegivel[]
}
