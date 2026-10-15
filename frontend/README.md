# Frontend — Encontre um Professor

Tela em **React 19 + TypeScript + Vite + Tailwind CSS 4** que consome
`POST /api/professores/busca`.

## Rodar

Com a API no ar em `http://localhost:5120`:

```
npm install
npm run dev
```

Abre em `http://localhost:5173`. O Vite faz proxy de `/api` para a API
(ver `vite.config.ts`), então não há CORS no desenvolvimento.

## Tema

Claro/escuro pelo botão de sol/lua no cabeçalho, persistido em `localStorage`
(na primeira visita segue o `prefers-color-scheme` do sistema). As cores são
tokens semânticos definidos em `src/index.css`: o bloco `.dark` troca os valores
e cada componente usa classes como `bg-surface` e `text-ink`.

## Scripts

| Script            | O que faz                                  |
| ----------------- | ------------------------------------------ |
| `npm run dev`     | Servidor de desenvolvimento com HMR        |
| `npm run build`   | Type-check (`tsc -b`) + build de produção  |
| `npm run preview` | Serve o build de produção localmente       |
| `npm run lint`    | oxlint                                     |

## Estrutura

```
src/
  api/buscaProfessores.ts   cliente HTTP (ErroValidacao × ErroComunicacao)
  components/
    Header.tsx              cabeçalho
    TemaToggle.tsx          alterna claro/escuro
    icons.tsx               ícones SVG (sem emojis)
    flags.tsx               bandeiras dos idiomas em SVG
    BuscaForm.tsx           formulário (idioma, dias, faixa, duração)
    Resultados.tsx          lista de resultados + estado vazio
    ProfessorCard.tsx       cartão de cada professor
    NaoElegiveis.tsx        quem ficou de fora e o motivo
    Feedback.tsx            carregando, vazio e alertas de erro
  constants/opcoes.ts       idiomas, dias e durações
  hooks/useTema.ts          tema claro/escuro com localStorage
  types/busca.ts            contratos da API
  App.tsx                   máquina de estados da tela
  index.css                 tokens de design e tema (Tailwind `@theme`)
```

Mais detalhes de decisões no [`../README.MD`](../README.MD).
