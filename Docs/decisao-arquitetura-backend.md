# Decisão de arquitetura — Backend

**Status:** Aceita
**Escopo:** API em C#/.NET 8 com `POST /api/professores/busca`.

## Restrições da especificação

- Regra de negócio separada da camada HTTP (item avaliado).
- No mínimo 5 testes automatizados em C#, sem depender de subir a API.
- Dados carregados de `professores.json`, mantidos em memória.

## Decisão

MVC em um único projeto de API (`Backend.csproj`), organizado em pastas, com a
regra de busca isolada em uma classe de serviço POCO que recebe suas dependências
por construtor. Um projeto de testes separado (`Backend.Tests.csproj`) referencia
o projeto da API e testa o serviço diretamente — sem HTTP, sem container e sem
servidor. Fonte de dados: `professores.json` em memória, atrás de
`IProfessorRepository`.

```
Backend/
  Controllers/ProfessoresController.cs
  Services/BuscaProfessoresService.cs
  Services/ValidadorBusca.cs
  Repositories/IProfessorRepository.cs
  Repositories/JsonProfessorRepository.cs
  Models/
  Program.cs

Backend.Tests/
  BuscaProfessoresServiceTests.cs
  ValidadorBuscaTests.cs
```

## Por que um projeto só (e não Clean Architecture)

Não é preciso separar projetos para testar. Um projeto de teste referencia o
projeto da API e exercita a regra de busca diretamente, sem iniciar o servidor.
Basta que a regra viva fora do Controller, numa classe comum.

### Alternativas

- **A. Projeto único + projeto de testes — escolhida.** Simples, atende à
  separação lógica exigida, entrega dentro do prazo e deixa a alteração ao vivo
  fácil de navegar. *Contra:* as fronteiras entre camadas dependem de disciplina,
  não do compilador.
- **B. Domain em class library + Api + Tests.** Fronteiras impostas pelo
  compilador e grafo de dependências explícito. *Contra:* mais cerimônia e mais
  arquivos para um escopo pequeno, sem ganho de testabilidade.
- **C. Clean Architecture completa.** Máxima separação de responsabilidades,
  porém overkill para o tamanho do desafio.

## Fonte de dados: JSON em memória

A especificação pede carregar de `professores.json` e manter em memória. SQLite
adicionaria schema, migrações e seeding sem benefício para os critérios
avaliados. A abstração `IProfessorRepository` preserva a troca futura de
armazenamento sem tocar no serviço nem nos testes.

## Consequências

- Controller fino: desserializa, valida e delega; a regra fica no
  `BuscaProfessoresService`.
- Os testes obrigatórios são de unidade e não sobem a API.
- Alterações tendem a tocar uma classe de serviço e um arquivo de teste.
- Se o escopo crescer, o próximo passo natural é extrair o domínio para uma class
  library (opção B).
