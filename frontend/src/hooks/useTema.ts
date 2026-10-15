import { useEffect, useState } from 'react'

export type Tema = 'claro' | 'escuro'

const CHAVE = 'tema'
const CLASSE_ESCURO = 'dark'

function lerTemaInicial(): Tema {
  const salvo = window.localStorage.getItem(CHAVE)
  if (salvo === 'claro' || salvo === 'escuro') return salvo

  return window.matchMedia('(prefers-color-scheme: dark)').matches
    ? 'escuro'
    : 'claro'
}

/** Alterna entre claro e escuro, aplicando a classe `dark` no <html>. */
export function useTema() {
  const [tema, setTema] = useState<Tema>(lerTemaInicial)

  useEffect(() => {
    document.documentElement.classList.toggle(
      CLASSE_ESCURO,
      tema === 'escuro',
    )
    window.localStorage.setItem(CHAVE, tema)
  }, [tema])

  return {
    tema,
    alternar: () => setTema((atual) => (atual === 'escuro' ? 'claro' : 'escuro')),
  }
}
