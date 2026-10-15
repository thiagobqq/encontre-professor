# Decisão — Regras da busca de professores

**Status:** Aceita
**Escopo:** regra de negócio de `POST /api/professores/busca` — validação de
entrada (`ValidadorBusca`) e busca (`BuscaProfessoresService`).

## Restrições da especificação

- Sete regras de busca: ensina o idioma; agenda não bloqueada; **horário fixo**
  livre em todos os dias pedidos; aula inteira dentro de um bloco de
  disponibilidade e da faixa; sem sobreposição a aula marcada (**encostar é
  permitido**); início só em `:00`/`:30`; ordenação por menos aulas na semana e,
  no empate, ordem alfabética.
- 400 com mensagem clara em seis casos: idioma vazio; nenhum dia; dia inválido;
  horário fora de `HH:mm`; fim da faixa igual ou anterior ao início; duração
  diferente de 30, 60 ou 90.
- Diferencial: preencher `naoElegiveis` com um dos três textos exatos, o primeiro
  na ordem de prioridade quando houver mais de um motivo.
- Regra de negócio separada da camada HTTP (item avaliado).

## Decisão

O Controller valida e delega; a regra de busca nunca vê string crua. O
`ValidadorBusca` converte o DTO HTTP em `ParametrosBusca` — dias como enum,
faixa como `TimeOnly`, duração como `int` já restrito a uma das três permitidas —
e é essa estrutura tipada que chega ao serviço.

```
Models/
  ParametrosBusca.cs          entrada tipada da regra de busca
  ErroValidacaoResponse.cs    corpo do 400 ({ "mensagem": "..." })
Services/
  ValidadorBusca.cs           DTO HTTP -> ParametrosBusca (ou a mensagem de erro)
  BuscaProfessoresService.cs  as sete regras
Controllers/
  ProfessoresController.cs    valida, delega, traduz em 200 ou 400
```

Fluxo de uma requisição:

```
POST /api/professores/busca
  Controller
    ValidadorBusca.Validar(request)
      falhou -> 400 { "mensagem": "..." }
      ok     -> BuscaProfessoresService.Buscar(parametros) -> 200
```

## Algoritmo da busca

Para cada professor, na ordem de prioridade dos motivos:

1. não ensina o idioma → `Não ensina o idioma`;
2. agenda bloqueada → `Agenda bloqueada`;
3. sem nenhum horário válido → `Sem horário em comum nos dias pedidos`;
4. caso contrário, entra em `professores`.

Os horários válidos saem de uma varredura na grade de 30 minutos:

```
inicio = inicioFaixa
enquanto inicio + duracao <= fimFaixa:
    se a aula cabe em TODOS os dias pedidos -> adiciona inicio
    inicio += 30min
```

E "cabe no dia" é a conjunção de duas checagens:

```
fim = inicio + duracao
cabeEmBloco = existe bloco do dia com bloco.Inicio <= inicio e fim <= bloco.Fim
sobrepoe    = existe aula marcada do dia com inicio < aula.Fim e aula.Inicio < fim
cabe        = cabeEmBloco e não sobrepoe
```

Dois detalhes que caem fora de graça dessa formulação:

- **Encostar é permitido** porque as aulas marcadas são comparadas como
  intervalos meio-abertos (`<`, não `<=`). Uma aula `18:00–19:00` deixa o
  candidato `19:00` passar, e barra `18:30`.
- **Terminar exatamente** no fim do bloco ou no fim da faixa é permitido pelo
  `<=`. É o que faz o exemplo da especificação fechar: Diego, com bloco livre
  `18:00–22:00` e faixa `18:00–20:00`, devolve `18:00`, `18:30` e `19:00` com 60
  minutos.

O `aulasNaSemana` da resposta é simplesmente `AulasMarcadas.Count` — a contagem
da semana inteira, não só dos dias pedidos, como o exemplo da especificação
mostra (Ana, com uma aula marcada, devolve `1`).

## Aritmética em minutos, não em `TimeOnly`

A varredura avança em `TimeOnly`? Não. `TimeOnly.AddMinutes` dá a volta à
meia-noite — `23:30.AddMinutes(30)` é `00:00` —, e uma faixa que termina tarde
(`23:00–23:59`) passaria a comparar `00:00 <= 23:59`, aceitando um horário
inválido e travando o laço para sempre. Toda a comparação de horários é feita em
minutos desde a meia-noite (`hora.Hour * 60 + hora.Minute`), que não tem essa
borda. Os dados não atravessam a meia-noite, mas a validação aceita qualquer
faixa bem-formada até `23:59`, então o caso é alcançável pela API.

## Validação: ordem e mensagens

A especificação lista os casos, mas não diz o que fazer quando mais de um se
aplica. A ordem adotada é a da própria especificação, com o caso extra no lugar
que depende dele:

| # | Regra | Mensagem |
|---|---|---|
| 1 | `idioma` vazio, nulo ou só espaços | `Informe o idioma.` |
| 2 | nenhum dia informado | `Informe ao menos um dia da semana.` |
| 3 | dia fora do enum | `Dia da semana inválido: '{valor}'.` |
| 4 | horário fora de `HH:mm` | `Horário inválido: '{valor}'. Use o formato HH:mm.` |
| 5 | `inicioFaixa` fora da grade `:00`/`:30` | `O início da faixa deve ser em hora cheia ou meia hora (:00 ou :30).` |
| 6 | `fimFaixa` ≤ `inicioFaixa` | `O fim da faixa deve ser posterior ao início.` |
| 7 | duração fora de 30, 60 ou 90 | `A duração deve ser 30, 60 ou 90 minutos.` |

O caso 5 depende do 4 (não dá para checar a grade de um horário que não
*parseou*), e o 6 depende do 4 também (não dá para comparar). Por isso a ordem
não é a da tabela ao acaso: cada linha só roda sobre valores já garantidos.

O horário é lido com `TimeOnly.TryParseExact(..., "HH:mm", InvariantCulture)`, que
rejeita `7:00`, `18:60` e `18h` — o `TryParse` permissivo aceitaria os três.

## Casos que a especificação não cobre

O enunciado pede para decidir e registrar. Estas são as decisões:

| Caso | Decisão | Por quê |
|---|---|---|
| `inicioFaixa` fora da grade (ex.: `18:15`) | **400** | A regra 6 restringe a aula a `:00`/`:30`. Aceitar `18:15` em silêncio faria o resultado nunca conter o horário que o aluno digitou, o que parece erro do sistema. Falhar explícito é mais honesto. `fimFaixa` não tem a restrição: é só um limite superior e nunca produz horário errado. |
| Dias repetidos (`["segunda","segunda"]`) | deduplicados | Repetir um dia não muda o resultado — a aula precisa caber em todos. |
| Idioma com maiúsculas/minúsculas | comparação `OrdinalIgnoreCase` | `"espanhol"` acha `"Espanhol"`. Acento continua significativo: `"Ingles"` não acha `"Inglês"`, porque os dados usam acento. |
| Dia com acento (`"terça"`, `"sábado"`) | aceito | Superset do que a especificação lista; o `DiaDaSemanaParser` já fazia isso. |
| Ordem da lista `naoElegiveis` | alfabética por nome | A especificação define a prioridade dos *motivos*, não a ordem da *lista*. Alfabética é determinística e fácil de conferir. |
| Bloco que começa fora da grade (`18:15`, caso do Fábio) | primeiro candidato é `18:30` | Consequência natural da regra 6: a grade não é do bloco, é dos inícios. `18:15` nunca é candidato. |
| Professor que ensina e não está bloqueado, mas não tem horário | `Sem horário em comum nos dias pedidos` | É o terceiro motivo e o que sobra para "não achou vaga". |
| Corpo do 400 | `{ "mensagem": "..." }` | Simples de exibir na tela e de testar. JSON malformado ou body ausente continuam caindo no `ProblemDetails` automático do `[ApiController]`, que é outro caso. |

## Alternativas consideradas

- **A. Validar no Controller, regra tipada — escolhida.** O `ValidadorBusca` é
  uma classe separada, então a validação tem teste próprio e o
  `BuscaProfessoresService` trabalha só com tipos de domínio. *Contra:* o
  Controller conhece o validador, são duas dependências em vez de uma.
- **B. Validar dentro do serviço.** Uma peça menos e o Controller só traduz
  resultado em status. *Contra:* o serviço passaria a receber o DTO HTTP e
  misturaria validação de entrada com regra de busca — justamente o que o item
  "regra de negócio separada da camada HTTP" cobra.
- **C. Arredondar a faixa fora da grade em vez de 400.** Mais permissivo e nunca
  devolve erro para uma faixa `HH:mm` válida. *Contra:* o aluno digita `18:15` e
  não entende por que o sistema responde `18:30`. Escolhemos o 400 e resolvemos a
  origem na tela, com `step` de 30 minutos no campo de horário.

## Consequências

- A regra de busca é testável sem HTTP: recebe `ParametrosBusca` e devolve a
  resposta.
- Os três textos de motivo e a ordem de prioridade ficam em constantes privadas
  do serviço, num só lugar.
- Adicionar um idioma, um dia ou um valor de duração é mudar uma coleção; a
  estrutura do algoritmo não muda.
- Um sétimo caso de 400 (`inicioFaixa` fora da grade) foi adicionado além da
  lista da especificação. Está registrado acima e na tela, mas é a decisão mais
  discutível deste documento — um avaliador pode esperar que a faixa seja aceita
  como veio.
