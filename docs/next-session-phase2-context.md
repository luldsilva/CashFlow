# Contexto da Próxima Sessão - Fase 2

## Objetivo deste documento

Registrar o ponto exato em que o backend foi deixado ao final da Fase 1, para que a próxima sessão possa começar diretamente na Fase 2, sem precisar remontar o contexto do produto e da implementação.

## Estado consolidado ao final da sessão

O backend concluiu o core da Fase 1 do produto, cobrindo:

- `Fase 1A` - setup financeiro inicial
- `Fase 1B` - motor de compromissos do mes
- `Fase 1C` - resumo financeiro do mes e dashboard operacional
- `Fase 1D` - cartao de credito e fatura do ciclo
- `Fase 1E` - fechamento e revisao mensal

Os resumos de negocio de cada fase foram registrados em:

- [`docs/phase-delivery-context.md`](/home/lucaslisilva/projetos/CashFlow/docs/phase-delivery-context.md)

## O que o backend entrega agora

### 1. Setup financeiro

O backend permite:

- salvar configuracao financeira da familia
- cadastrar fontes de renda
- cadastrar buckets percentuais
- cadastrar categorias principais

Endpoint principal:

- `GET /api/financial-setup`
- `PUT /api/financial-setup`

### 2. Operacao mensal

O backend permite:

- cadastrar obrigacoes financeiras por competencia mensal
- classificar por categoria e bucket
- controlar vencimento
- marcar status como `previsto`, `pago` e `ajustado`

Endpoints principais:

- `POST /api/financial-obligations`
- `GET /api/financial-obligations?month=YYYY-MM-DD`
- `GET /api/financial-obligations/{id}`
- `PUT /api/financial-obligations/{id}`
- `DELETE /api/financial-obligations/{id}`

### 3. Dashboard mensal

O backend permite:

- consultar resumo mensal consolidado
- obter entrada planejada
- obter saida paga
- obter saida comprometida
- obter saldo livre para gastar
- obter saldo potencial para investir
- listar proximos vencimentos
- consultar leitura por bucket

Endpoint principal:

- `GET /api/dashboard/monthly-summary?month=YYYY-MM-DD`

### 4. Cartao de credito

O backend permite:

- cadastrar cartoes
- registrar faturas por ciclo
- consultar faturas por competencia mensal

Endpoints principais:

- `POST /api/credit-cards`
- `GET /api/credit-cards`
- `POST /api/credit-cards/statements`
- `GET /api/credit-cards/statements?month=YYYY-MM-DD`

### 5. Fechamento e revisao mensal

O backend permite:

- fechar o mes
- persistir snapshot consolidado
- salvar observacoes do ciclo
- consultar revisao mensal posteriormente

Endpoints principais:

- `POST /api/monthly-review/close`
- `GET /api/monthly-review?month=YYYY-MM-DD`

## Estrutura persistida no banco ao final da Fase 1

Tabelas esperadas:

- `Users`
- `Expenses`
- `ExpenseAttachments`
- `Households`
- `IncomeSources`
- `PlanningBuckets`
- `ExpenseCategories`
- `FinancialObligations`
- `CreditCards`
- `CreditCardStatements`
- `MonthlyClosures`
- `__EFMigrationsHistory`

Migrations esperadas:

- `20250226171340_InitialMigration`
- `20260327233000_AddExpenseAttachments`
- `20260403120000_AddFinancialSetup`
- `20260403133000_AddFinancialObligations`
- `20260403150000_AddCreditCardsAndMonthlyClosures`

## Proxima etapa planejada

O proximo ciclo comeca em `Fase 2 - investimentos`.

Direcao sugerida para a Fase 2:

### Fase 2A - Plano de investimento

- meta mensal de aporte
- plano de investimento
- aporte planejado
- aporte realizado

### Fase 2B - Ativos e posicoes

- cadastro de ativos
- tipo do ativo
- posicao consolidada por ativo

### Fase 2C - Leitura da carteira

- total investido
- distribuicao por tipo
- evolucao patrimonial

## Decisao importante para a proxima sessao

A Fase 2 deve reaproveitar a fundacao da Fase 1, especialmente:

- leitura de `freeToInvest` do dashboard mensal
- buckets percentuais ja configurados
- fechamento mensal como base historica de analise

Ou seja:

- investimentos entram como segunda camada do produto
- nao substituem o core financeiro
- usam a sobra e o planejamento mensal como contexto

## Recomendacao de inicio na proxima sessao

Comecar pela modelagem minima de:

- `InvestmentPlan`
- `InvestmentContribution`
- `InvestmentAsset`
- `InvestmentPosition`

e pelos endpoints iniciais de:

- configuracao da meta mensal
- registro de aporte
- leitura resumida da carteira
