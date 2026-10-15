Uso de IA

Ferramentas: OpenCode, DeepSeek v4.1 e Kimi k3.

Onde usei
Para discutir arquitetura e decisões antes de implementar.
Para organizar em texto a documentação (Docs/) a partir das decisões que tomamos.
Para revisar código pronto, procurando nomes ruins, duplicações e divergências com a documentação.
Para transformar cenários de teste que eu descrevia em testes C#.
Onde a IA errou

Testes redundantes. A IA gerou testes demais, vários cobrindo o mesmo comportamento. Percebi revisando a suíte que o esforço de manutenção não compensava o que esses testes acrescentavam.

Cálculo de horários. A primeira implementação usava TimeOnly.AddMinutes, que dá a volta à meia-noite. Identifiquei o problema ao revisar os limites da faixa de horários. Troquei o cálculo por minutos desde a meia-noite e registrei o caso no Doc e no teste K1.
