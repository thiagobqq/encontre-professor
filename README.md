# Encontre um Professor

API em C#/.NET 8 que encontra professores disponíveis para um aluno (idioma, dias,
faixa de horário e duração) e uma tela em React + TypeScript para usá-la.
Especificação: [`Docs/specs.md`](Docs/specs.md).

## Como rodar

**API** (`.NET 8 SDK`):

```
dotnet run --project Backend
```

Sobe em `http://localhost:5120`; Swagger em `/swagger`.

**Tela** (Node 20+):

```
cd frontend
npm install
npm run dev
```

Abre em `http://localhost:5173` (proxy de `/api` para a API).

**Docker:**

```
docker compose up -d --build
```

Tela em `http://localhost/`; o nginx do frontend faz proxy de `/api` para o backend
(interno, porta 8080).

## Endpoint

`POST /api/professores/busca` — recebe idioma, dias, faixa de horário e duração, e
devolve `professores` elegíveis (com horários e aulas na semana) e `naoElegiveis`
(com o motivo). Validações inválidas retornam **400**.

## Testes

```
dotnet test Backend.sln
```

