# CashFlow Product Roadmap Context

## Objetivo deste documento

Registrar de forma consolidada a direcao de produto alinhada para a proxima fase do `CashFlow`, para que backend e frontend partam da mesma leitura de problema, prioridades, regras de negocio e ordem de implementacao.

Este documento substitui a ambiguidade de um MVP "minimo demais". A decisao atual e construir um `MVP mais encorpado`, com foco direto em resolver o problema real do usuario no proximo ciclo mensal de contas.

O entendimento mais recente do produto refinou essa direcao:

- o onboarding deve ser mais curto e mais util desde o primeiro passo
- renda e obrigacoes entram antes da configuracao completa de buckets
- a distribuicao por buckets acontece depois da leitura do comprometimento
- a virada de mes deve reaproveitar base fixa sem apagar o historico do ciclo anterior

## Problema central que o produto precisa resolver

O problema principal nao e simplesmente registrar despesas.

O produto precisa responder, de forma clara e recorrente, as perguntas:

- quanto entrou no mes
- quanto ja saiu
- quanto ainda esta comprometido
- quanto ainda pode ser gasto sem desorganizar o mes
- quanto pode ser direcionado para investimento

Hoje esse controle acontece manualmente em caderno, exigindo repeticao mensal, disciplina alta, revisao manual e memoria para nao esquecer gastos pequenos ou recorrentes.

## Contexto real de uso que orienta o produto

Contexto atual do usuario:

- familia com 3 pessoas
- renda familiar concentrada em fluxo PJ, com variacao moderada
- aluguel e condominio pagos no dia `10`
- outras contas relevantes concentradas entre os dias `15` e `20`
- existem contas fixas recorrentes
- existem contas recorrentes variaveis
- existem gastos pequenos e assinaturas que podem ser esquecidos
- existe necessidade de saber rapidamente se ainda ha dinheiro disponivel no mes
- existe interesse em separar parte da renda para investimento mensal

Tipos de gasto citados como relevantes:

- aluguel
- condominio
- luz
- gas
- internet
- plataforma de contabilidade
- imposto PJ recorrente com pequena variacao
- plano de celular
- assinaturas diversas
- cartao de credito

## Tese de produto

O `CashFlow` deve nascer como uma ferramenta de `planejamento financeiro operacional familiar`.

Nao e so um app de anotacao.

Ele deve combinar:

- controle financeiro do mes
- previsao de compromissos
- orientacao por modelo financeiro em momento apropriado
- visao de saldo livre
- preparacao para aporte mensal

## Prioridades de produto

Ordem de prioridade alinhada:

1. controle financeiro familiar
2. controle e planejamento de investimentos
3. integracoes externas e ampliacao de inteligencia financeira

Na terceira prioridade entram:

- Open Finance
- leitura mais ampla de situacao financeira do usuario
- possivel consulta de dividas e organizacao de passivos

## Visao de MVP

A decisao atual nao e construir um MVP extremamente reduzido.

O objetivo e entregar um `MVP utilizavel no proximo ciclo de contas real`, com massa funcional suficiente para substituir o caderno e gerar confianca de uso no dia a dia.

Esse MVP deve nascer com:

- fluxo de configuracao inicial simples
- modelagem de renda
- modelagem de contas fixas e recorrentes
- leitura de comprometimento do mes
- classificacao por grupos de planejamento em segunda etapa do onboarding
- visao mensal consolidada
- dashboard principal
- base para investimentos

## Principio funcional mais importante

O sistema precisa deixar explicita a diferenca entre:

- dinheiro aparentemente disponivel
- dinheiro efetivamente livre para gastar

Ou seja:

- uma coisa e o saldo bruto
- outra coisa e o saldo comprometido
- a decisao correta depende do `saldo livre`

Essa e a regra de negocio mais importante do produto.

## Modelo conceitual inicial do dominio

Entidades e agregados sugeridos para a proxima fase:

- `Household`
- `HouseholdMember`
- `IncomeSource`
- `IncomeEntry`
- `Expense`
- `ExpenseCategory`
- `RecurringRule`
- `FinancialObligation`
- `CreditCard`
- `CreditCardStatement`
- `MonthlyPlan`
- `BudgetModel`
- `BudgetBucket`
- `MonthlyAllocation`
- `InvestmentPlan`
- `InvestmentContribution`
- `InvestmentAsset`
- `InvestmentPosition`

Observacao:

- nao significa implementar tudo de uma vez em profundidade total
- significa ter essa leitura como mapa coerente do dominio

## Regras de negocio alinhadas

### 1. Planejado, previsto e realizado nao sao a mesma coisa

O sistema deve distinguir:

- valor planejado
- compromisso previsto
- lancamento efetivamente realizado

Isso vale tanto para entradas quanto para saidas.

### 2. Toda movimentacao precisa de contexto temporal

Cada entrada ou saida deve carregar pelo menos:

- competencia
- data esperada
- data efetiva, quando existir

### 3. Recorrencia precisa ser nativa

O produto nao pode depender de o usuario lembrar manualmente das mesmas contas todos os meses.

Recorrencia deve existir para:

- renda
- contas fixas
- contas recorrentes variaveis
- assinaturas
- aportes planejados

Isso implica que a virada de mes deve reaproveitar automaticamente a base reaplicavel do ciclo anterior.

### 4. Despesas variaveis recorrentes devem permitir previsao

Exemplo:

- imposto PJ
- energia
- gas

Essas despesas nao sao perfeitamente fixas, mas tambem nao sao totalmente imprevisiveis.

O sistema deve permitir valor esperado com ajuste posterior.

### 5. Cartao de credito nao deve ser tratado como despesa simples

O cartao gera:

- compras ao longo do tempo
- fechamento
- vencimento
- impacto futuro no caixa

O dominio precisa refletir isso.

### 6. O produto deve mostrar comprometimento do mes

A conta de visao deve considerar:

- entradas confirmadas
- entradas provaveis
- saidas pagas
- obrigacoes futuras do mesmo ciclo

Essa leitura de comprometimento deve aparecer antes da configuracao completa do modelo percentual.

### 7. O modelo percentual e um guia, nao uma prisao

O sistema deve orientar o usuario por faixas e metas, sem bloquear ajustes quando a realidade do mes mudar.

### 8. O produto deve orientar educacao financeira sem virar curso

O valor esta em:

- sugerir estrutura
- acompanhar aderencia
- dar leitura visual clara

Nao em transformar a aplicacao num portal de conteudo.

## Planejamento por percentuais

Foi alinhado que o produto deve suportar um modelo de distribuicao de renda por grupos percentuais, inspirado em abordagens conhecidas de organizacao financeira, mas com personalizacao livre.

Exemplo de modelo citado:

- `50%` gastos fixos e essenciais
- `10%` educacao
- `20%` investimentos
- `10%` aposentadoria
- `10%` livre

Esse modelo deve poder ser:

- sugerido no onboarding
- ajustado pelo usuario
- redefinido quando a realidade da familia mudar

## Modelos de planejamento sugeridos

O produto deve permitir ao menos estas opcoes iniciais:

- modelo equilibrado padrao
- modelo inspirado em divisao percentual classica
- modelo com foco em investimento
- modelo totalmente personalizado

O importante nao e impor uma metodologia unica, mas oferecer uma estrutura inicial util.

## Buckets de planejamento sugeridos

Buckets iniciais mais provaveis:

- essenciais
- educacao
- investimentos
- aposentadoria
- livre
- lazer
- reserva
- doacao

Nem todos precisam estar ativos por padrao.

Na experiencia inicial, o produto nao deve expor todos esses buckets de uma vez.

O recomendado e começar com poucos buckets sugeridos, em linguagem simples, e expandir a customizacao depois.

## Comportamento esperado do modelo percentual

O sistema deve:

- calcular a meta em valor para cada bucket a partir da renda planejada ou confirmada do mes
- permitir mapear despesas e aportes para buckets
- mostrar `planejado x realizado`
- alertar desvios relevantes
- permitir reconfiguracao do modelo sem quebrar o historico

## Fluxo inicial de onboarding

O onboarding sugerido para a nova fase deve seguir esta linha:

1. identificar composicao familiar
2. identificar quantas rendas existem e se elas sao fixas ou variaveis
3. cadastrar fontes de renda e valores esperados
4. cadastrar contas fixas e recorrentes do mes
5. registrar cartao e/ou fatura atual quando fizer sentido
6. calcular o percentual da renda ja comprometido
7. distribuir apenas o valor restante em poucos buckets sugeridos
8. revisar o resumo inicial do mes
9. entrar no dashboard principal

Observacao importante:

- buckets, percentuais e modelo de planejamento deixam de ser a porta de entrada do fluxo
- eles passam a ser uma etapa posterior, guiada pelo valor que restou apos o comprometimento inicial

## Virada de mes e continuidade operacional

Foi alinhado que o produto deve funcionar bem nao apenas no onboarding, mas tambem no inicio de cada novo ciclo.

Na virada de mes, o sistema deve:

- sugerir ao usuario a abertura do novo mes
- reaproveitar renda fixa e contas fixas
- copiar contas variaveis recorrentes com aviso para revisao
- permitir ajuste do novo mes antes ou depois do fechamento do mes anterior
- preservar uma foto consolidada do mes fechado para historico, relatorio e comparacao

O comportamento esperado nao e recadastrar tudo, mas revisar uma base herdada com seguranca.

## Dashboard principal

Foi alinhado que deve existir uma tela principal de dashboard desde a fase inicial.

Ela deve ser operacional, nao apenas estetica.

Blocos sugeridos:

- entrou no mes
- saiu no mes
- comprometido ate o fim do mes
- livre para gastar
- livre para investir

Blocos secundarios:

- proximos vencimentos
- contas atrasadas
- cartao atual e proximo vencimento
- assinaturas e pequenos recorrentes
- aderencia ao modelo percentual
- comparacao com mes anterior

## Funcionalidades essenciais da fase 1

### Fase 1A. Estrutura de planejamento financeiro

- onboarding de configuracao financeira simples
- household e composicao familiar
- fontes de renda
- renda recorrente e renda eventual
- abertura do mes operacional inicial

### Fase 1B. Motor de compromissos do mes

- despesas fixas recorrentes
- despesas recorrentes variaveis
- obrigacoes com vencimento
- competencia mensal
- status de previsto, pago e ajustado
- leitura de comprometimento inicial da renda

### Fase 1C. Dashboard e operacao do mes

- resumo financeiro do mes
- saldo livre
- saldo comprometido
- lista de vencimentos
- leitura por bucket
- distribuicao guiada do restante entre buckets

### Fase 1D. Cartao de credito

- cadastro de cartao
- fechamento
- vencimento
- fatura do ciclo
- impacto no mes atual e no proximo

### Fase 1E. Revisao mensal

- fechamento do mes
- comparativo planejado x realizado
- ajustes de modelo
- reaproveitamento de configuracao para o mes seguinte
- virada assistida de ciclo com snapshot preservado

## Funcionalidades da fase 2: investimentos

Objetivo da fase:

- transformar sobra livre em decisao de investimento

Capacidades desejadas:

- definir meta mensal de aporte
- registrar aporte planejado
- registrar aporte realizado
- manter posicao por ativo
- classificar ativos por tipo
- acompanhar valor investido total
- acompanhar distribuicao da carteira

Tipos iniciais de ativo:

- renda fixa
- fii
- acao
- etf
- caixa

Leituras desejadas:

- quanto foi investido no mes
- quanto falta para bater a meta
- distribuicao por tipo
- evolucao patrimonial

## Integracoes futuras para investimentos

Foi alinhado que ha interesse em consultar APIs de mercado para acompanhar ganhos e atualizacao de valores.

Direcao sugerida:

- iniciar com entrada manual de aporte e posicao
- depois enriquecer com cotacoes externas

Referencias verificadas:

- a `B3` possui portal de APIs para clientes e parceiros, mas o uso nao parece ser um caminho simples de MVP para app de pessoa fisica
- a `brapi` aparece como alternativa pratica para cotacoes e dados de ativos brasileiros

Essas integracoes devem ficar em fase posterior ao controle financeiro principal.

## Open Finance

Open Finance faz sentido estrategicamente, mas nao deve entrar no core imediato da proxima rodada de implementacao.

Motivos:

- maior complexidade regulatoria e de integracao
- dependencia de consentimento e fluxo institucional
- menor relacao custo-beneficio para resolver a dor principal imediata

Open Finance entra como direcao futura para:

- consolidar contas
- importar transacoes
- reduzir digitacao manual
- enriquecer diagnostico financeiro

## Dividas e situacao financeira ampliada

Tambem foi mencionado interesse em entender melhor:

- dividas
- situacao financeira junto a orgaos e bureaus

Direcao sugerida:

- primeiro resolver `divida interna` dentro do proprio produto
- depois avaliar integracoes externas

Divida interna inclui:

- faturas de cartao
- contas atrasadas
- parcelas
- emprestimos
- juros previstos

## Roadmap executivo

### Etapa 1. Consolidacao do core financeiro familiar

- modelar renda, obrigacoes, recorrencia, competencia e buckets
- criar onboarding inicial
- criar dashboard principal
- permitir operacao do mes sem depender de caderno
- permitir virada de mes sem recadastro completo

### Etapa 2. Profundidade operacional

- melhorar fechamento mensal
- adicionar revisao de desvio por bucket
- fortalecer cartao de credito
- melhorar acompanhamento de assinaturas e pequenos gastos recorrentes

### Etapa 3. Investimentos

- aporte mensal
- posicao por ativo
- meta de investimento
- leitura de patrimonio

### Etapa 4. Dados externos

- cotacoes via API
- enriquecimento de rentabilidade
- primeiros conectores externos de consolidacao

### Etapa 5. Open Finance e inteligencia financeira ampliada

- importacao mais automatica de dados
- leitura consolidada de vida financeira
- visoes mais sofisticadas de risco, divida e patrimonio

## Ordem recomendada de implementacao a partir de amanha

1. consolidar regras de negocio do core financeiro
2. separar setup estrutural de operacao mensal
3. redefinir o onboarding inicial em duas etapas
4. implementar o motor de comprometimento mensal
5. implementar recorrencia e reaproveitamento de mes
6. implementar buckets percentuais sobre o restante
7. fortalecer cartao de credito dentro do fluxo mensal
8. consolidar dashboard e fechamento
9. preparar base de investimentos
10. deixar integracoes externas para depois

## Decisao pratica para a proxima sessao

A proxima sessao deve comecar por refinamento e implementacao do `core de planejamento financeiro familiar`.

Objetivo de curto prazo:

- sair do modelo de anotacao em caderno
- passar a operar o proximo ciclo mensal dentro do produto

## Alinhamento com frontend

O frontend precisa ter ciencia de que a aplicacao vai evoluir de um fluxo simples de CRUD de despesas para uma experiencia orientada por:

- onboarding financeiro
- onboarding em passos curtos e didaticos
- dashboard principal forte
- planejamento por percentuais em etapa posterior ao cadastro das contas
- visao de saldo livre
- compromissos mensais
- virada de mes assistida
- investimento como segunda camada do produto

## Observacao final

O produto nao deve se posicionar apenas como "controle de gastos".

A direcao alinhada agora e:

`organizar a vida financeira da familia, mostrar o que esta comprometido, o que esta livre e o que pode virar investimento`.
