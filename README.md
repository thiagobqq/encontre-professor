# Desafio técnico — Encontre um Professor

API em C#/.NET 8 que encontra professores disponíveis para um aluno, a partir do
idioma, dos dias da semana, da faixa de horário e da duração da aula pedidos, e
uma tela em React + TypeScript para usar a API.

> **Status:** backend com a busca implementada e tela em React criada. Ver
> [O que falta](#o-que-falta).

## Stack

- **Backend:** ASP.NET Core 8 (MVC), Swagger/Swashbuckle
- **Testes:** xUnit
- **Dados:** `Backend/professores.json`, carregados em memória
- **Frontend:** React 19 + TypeScript + Vite + Tailwind CSS 4

## Estrutura

```
Backend/
  Models/                       entidades e value objects do domínio
    Professor.cs
    Disponibilidade.cs
    AulaMarcada.cs
    DiaDaSemana.cs
  Repositories/
    IProfessorRepository.cs
    JsonProfessorRepository.cs  lê o professores.json para a memória
  Program.cs
  professores.json
  Backend.csproj

Backend.Tests/
  JsonProfessorRepositoryTests.cs

frontend/
  src/
    api/buscaProfessores.ts       chama POST /api/professores/busca
    components/                   Header, formulário, resultados e estados
    constants/opcoes.ts           idiomas, dias e durações do formulário
    types/busca.ts                contratos da requisição e da resposta
    App.tsx                       máquina de estados da tela
  vite.config.ts                  proxy de /api para a API

Docs/
  decisao-arquitetura-backend.md
  specs.md

Backend.sln
```

## Como rodar a API

Pré-requisitos:

- **.NET 8 SDK** (ou o SDK 9 + o **runtime ASP.NET Core 8**).

```
dotnet run --project Backend
```

A API sobe em `http://localhost:5120` (e `https://localhost:7227`) e o Swagger
fica em `http://localhost:5120/swagger` no ambiente de desenvolvimento.

> O endpoint da busca é `POST /api/professores/busca`, com a validação e as
> regras descritas em [`Docs/decisao-regras-busca.md`](Docs/decisao-regras-busca.md).

## Como rodar a tela

Pré-requisitos:

- **Node.js 20+** (testado com o Node 24).
- A **API no ar** em `http://localhost:5120` (veja acima).

```
cd frontend
npm install
npm run dev
```

A tela abre em `http://localhost:5173`. Em desenvolvimento o Vite faz **proxy**
de `/api` para `http://localhost:5120` (configurado em `frontend/vite.config.ts`),
então não há CORS e não é preciso configurar a URL da API.

Para apontar para outra API, defina `VITE_API_URL` antes do build:

```
$env:VITE_API_URL="https://minha-api"; npm run build
```

## Testes

```
dotnet test Backend.sln
```

Os testes são de unidade e **não dependem de subir a API**.

## Decisões

- **Arquitetura:** MVC em um único projeto de API, com um projeto de testes
  separado. O raciocínio e as alternativas estão em
  [`Docs/decisao-arquitetura-backend.md`](Docs/decisao-arquitetura-backend.md).
- **Domínio tipado:** horários como `TimeOnly` e dias como enum `DiaDaSemana`,
  em vez de strings — as regras de horário/sobreposição dependem disso.
- **Fonte de dados:** `professores.json` foi movido para dentro do projeto
  (`Backend/`) para simplificar o carregamento, e é lido uma única vez para a
  memória atrás de `IProfessorRepository`. Trocar por outro armazenamento no
  futuro é implementar essa interface.
- **Testes sem HTTP:** a regra de negócio vive fora do Controller, então os testes
  instanciam o serviço diretamente, sem subir servidor.

### Decisões — tela

- **Proxy em vez de CORS.** O `vite.config.ts` encaminha `/api` para
  `http://localhost:5120`. A tela não precisa saber a porta da API nem o backend
  precisa liberar CORS só para o dev.
- **Sem biblioteca de UI.** Tailwind com um pequeno conjunto de tokens no
  `index.css` reproduz a linguagem "chunky" do Duolingo: verde `#58CC02`, cantos
  arredondados, bordas grossas e o botão com borda inferior que "afunda" ao
  clicar.
- **Modo claro/escuro.** O tema é uma classe `dark` no `<html>`, alternada pelo
  botão no cabeçalho e persistida em `localStorage` (o inicial segue o sistema).
  As cores não são fixas por componente: `index.css` define tokens semânticos
  (`--page`, `--surface`, `--ink`, `--line`, …) publicados como utilitários via
  `@theme inline`, e o bloco `.dark` só troca os valores. Assim cada componente
  escreve `bg-surface`/`text-ink` uma vez e funciona nos dois temas. Um script
  inline no `index.html` aplica o tema antes do primeiro paint, sem *flash*.
- **Sem emojis.** Todos os ícones são SVG em `components/icons.tsx` (traço
  arredondado, `currentColor`) e as bandeiras dos idiomas são SVG desenhadas em
  `components/flags.tsx` (Reino Unido, Espanha e França na mesma proporção 3:2) —
  inclusive o favicon. Nada disso depende de emoji.
- **Máquina de estados explícita.** O `App.tsx` guarda um único estado
  discriminado (`ocioso`, `carregando`, `ok`, `validacao`, `comunicacao`), então
  cada estado da tela é uma tela só — nada de flags soltas.
- **Erro de validação ≠ erro de comunicação.** O `400` da API é mostrado como
  aviso junto ao formulário; falha de rede/5xx vira um alerta com **Tentar
  novamente**. Ambos vêm do mesmo módulo `api/buscaProfessores.ts`, que separa
  `ErroValidacao` de `ErroComunicacao`.
- **Requisições canceladas.** Uma busca nova cancela a anterior com
  `AbortController`, evitando que uma resposta antiga sobrescreva a mais recente.
- **Horário em meia hora.** Os campos usam `step="1800"`, alinhados com a regra
  de que a aula só começa em `:00` ou `:30` (ver `decisao-regras-busca.md`).

## O que falta

- Registrar no `USO_DE_IA.md` um caso em que a IA errou e como foi percebido.
