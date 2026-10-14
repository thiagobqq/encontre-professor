# Desafio técnico — Encontre um Professor

API em C#/.NET 8 que encontra professores disponíveis para um aluno, a partir do
idioma, dos dias da semana, da faixa de horário e da duração da aula pedidos.

> **Status:** backend em fase inicial. A busca ainda não foi implementada e a tela
> em React não existe. Ver [O que falta](#o-que-falta).

## Stack

- **Backend:** ASP.NET Core 8 (MVC), Swagger/Swashbuckle
- **Testes:** xUnit
- **Dados:** `Backend/professores.json`, carregados em memória

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

> No estado atual só existe `GET /`, um endpoint temporário de verificação.
> O endpoint da busca ainda será implementado.

## Como rodar a tela

A tela em React ainda não foi criada. (Pendente.)

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

## O que falta

- `POST /api/professores/busca` — serviço com as regras da busca, validação e
  Controller.
- No mínimo 5 testes automatizados sobre as regras da busca (idioma, agenda
  bloqueada, horário fixo em todos os dias, caber no bloco, encostar permitido,
  `:00`/`:30`, ordenação).
- Tela em React (formulário, resultado e estados: carregando, vazio, erro de
  comunicação e erro de validação).
- Diferencial: preencher `naoElegiveis` com o motivo de cada professor.
- `USO_DE_IA.md`.
