# Contexto da Sessao de Hoje

## Objetivo deste documento

Registrar de forma objetiva o que foi ajustado hoje no contrato e no comportamento do `financial-setup`, para facilitar a continuidade do backend e o alinhamento com o frontend.

## O que foi implementado hoje

### 1. Separacao entre criacao e edicao do financial setup

O backend deixou de tratar o setup financeiro como um unico fluxo de `upsert`.

Agora o contrato HTTP ficou separado em:

- `GET /api/financial-setup`
- `POST /api/financial-setup`
- `PUT /api/financial-setup`
- `DELETE /api/financial-setup`

Comportamento consolidado:

- `POST` cria o setup inicial do usuario
- `PUT` atualiza um setup ja existente
- `DELETE` remove o setup inteiro do usuario
- `GET` retorna o setup atual do usuario

Regras de resposta:

- `GET` retorna `404` quando o usuario ainda nao possui setup
- `POST` retorna `201 Created` quando cria com sucesso
- `POST` retorna `409 Conflict` quando o usuario ja possui setup
- `PUT` retorna `404` quando nao existe setup para atualizar
- `DELETE` retorna `204 No Content` quando remove com sucesso
- `DELETE` retorna `404` quando nao existe setup para excluir

### 2. Enums do setup passaram a funcionar como string no JSON

O backend foi ajustado para aceitar e responder enums como string no JSON.

Isso corrige o erro anterior de desserializacao em campos como:

- `primaryIncomeFrequency`
- `planningModel`
- `incomeSources[].type`
- `incomeSources[].frequency`

Exemplos validos agora:

- `"Variable"`
- `"Balanced"`
- `"Business"`
- `"Monthly"`

Ou seja:

- o frontend nao precisa mais enviar enums numericos
- o contrato do setup fica mais legivel e mais estavel para a camada web

### 3. O frontend nao precisa mais enviar code em planning buckets

Foi removida a exigencia de o frontend informar `code` em `planningBuckets`.

Agora cada bucket de planejamento no request precisa enviar apenas:

- `name`
- `percentage`
- `isActive`
- `displayOrder`

O backend passou a gerar automaticamente o identificador interno do bucket.

Essa mudanca reduz acoplamento tecnico no frontend e evita obrigar a UI a inventar ou sincronizar codigos internos.

### 4. Categorias do setup agora referenciam bucket por nome no request

No setup, `expenseCategories` nao precisam mais enviar `bucketCode`.

Agora o request usa:

- `bucketName`

Exemplo:

- categoria `Aluguel` vinculada ao bucket `Essenciais`

O backend resolve internamente o bucket correspondente e gera o `bucketCode` persistido.

### 5. Geracao automatica e padronizada de codigos de bucket

O backend passou a gerar o `code` dos buckets a partir do nome informado.

Para nomes conhecidos, foi mantido um mapeamento canonico para preservar comportamento de negocio ja existente.

Exemplos:

- `Essenciais` -> `essentials`
- `Investimentos` -> `investments`
- `Livre` -> `free`
- `Educacao` -> `education`
- `Aposentadoria` -> `retirement`

Para nomes customizados:

- o backend gera um slug interno automatico

Isso foi importante para nao quebrar calculos e leituras que ainda dependem do `code` no dominio interno, especialmente o dashboard mensal.

### 6. Exclusao completa do setup foi adicionada

Foi implementado:

- `DELETE /api/financial-setup`

Esse endpoint remove o `Household` do usuario autenticado e os dados vinculados em cascade.

Remocoes esperadas junto com o setup:

- `IncomeSources`
- `PlanningBuckets`
- `ExpenseCategories`
- `FinancialObligations`
- `CreditCards`
- `MonthlyClosures`

Observacao importante:

- isso nao apaga `Expenses`, porque hoje despesas nao estao modeladas como filhas de `Household`

### 7. Cobertura de testes foi atualizada

Foram ajustados testes para refletir o contrato novo do `financial-setup`.

Casos cobertos:

- criacao via `POST`
- conflito ao tentar criar setup duplicado
- atualizacao via `PUT`
- `PUT` sem setup existente
- enums como string no payload
- exclusao via `DELETE`
- `DELETE` sem setup existente

## O que o frontend precisa saber desta sessao

### 1. Fluxo correto da tela de setup

O frontend deve operar assim:

1. chamar `GET /api/financial-setup`
2. se vier `404`, entrar em modo de criacao
3. se vier `200`, entrar em modo de edicao
4. usar `POST` para criar
5. usar `PUT` para salvar edicao
6. usar `DELETE` para excluir o setup inteiro

### 2. Buckets nao recebem mais code no request

O frontend nao deve mais enviar:

- `planningBuckets[].code`

Agora deve enviar apenas:

- `planningBuckets[].name`
- `planningBuckets[].percentage`
- `planningBuckets[].isActive`
- `planningBuckets[].displayOrder`

### 3. Categorias do setup nao usam mais bucketCode no request

O frontend nao deve mais enviar:

- `expenseCategories[].bucketCode`

Agora deve enviar:

- `expenseCategories[].bucketName`

### 4. Enums continuam sendo enviados como string

Exemplos:

- `primaryIncomeFrequency: "Variable"`
- `planningModel: "Balanced"`
- `incomeSources[].type: "Business"`
- `incomeSources[].frequency: "Monthly"`

## Contextos relacionados

- resumo das fases: [`docs/phase-delivery-context.md`](/mnt/c/Users/lucas/source/repos/CashFlow/docs/phase-delivery-context.md)
- proxima sessao na Fase 2: [`docs/next-session-phase2-context.md`](/mnt/c/Users/lucas/source/repos/CashFlow/docs/next-session-phase2-context.md)
- pendencias para producao: [`docs/production-readiness-context.md`](/mnt/c/Users/lucas/source/repos/CashFlow/docs/production-readiness-context.md)
- migracao para Windows e debug WSL: [`docs/windows-wsl-debug-context.md`](/mnt/c/Users/lucas/source/repos/CashFlow/docs/windows-wsl-debug-context.md)

---

# Atualizacao da Sessao de 2026-04-10

## Objetivo desta atualizacao

Registrar a rodada em que o backend foi adaptado para o novo fluxo principal do produto:

- onboarding mais direto
- leitura de comprometimento antes dos buckets
- preparo do proximo mes
- isolamento real de dados por usuario
- categorias oficiais com vinculo formal a contas e despesas

## O que foi implementado nesta sessao

### 1. O setup passou a aceitar onboarding minimo sem buckets e categorias

O `financial-setup` foi flexibilizado para permitir criacao inicial com foco em:

- familia
- membros
- renda
- fontes de renda

Sem obrigar o usuario a configurar logo no inicio:

- `planningBuckets`
- `expenseCategories`

Impacto pratico:

- o frontend pode iniciar o onboarding com dados estruturais e renda
- a configuracao de buckets e categorias pode ficar para a etapa seguinte

Tambem passaram a existir indicadores explicitos no retorno do setup:

- `hasPlanningConfigured`
- `hasExpenseCategoriesConfigured`

### 2. Obrigacoes mensais passaram a aceitar criacao sem bucket

O request de obrigacao financeira passou a aceitar:

- `bucketCode` opcional

Isso foi importante para o fluxo novo em que o usuario primeiro informa:

- renda
- contas

e so depois distribui o restante em buckets.

No retorno das obrigacoes foi adicionado:

- `requiresReview`

Esse campo ajuda a UI a destacar:

- contas variaveis
- contas ainda sem bucket definido

### 3. Dashboard mensal foi enriquecido para o novo fluxo

O resumo mensal passou a devolver leitura mais adequada ao onboarding e operacao do mes.

Novos campos principais:

- `commitmentPercentage`
- `remainingToAllocate`
- `creditCardPaidOutflow`
- `creditCardCommittedOutflow`
- `isMonthClosed`
- `suggestedBuckets`
- `creditCardStatements`

Impacto pratico:

- o frontend pode mostrar quanto da renda ja esta comprometido
- pode mostrar quanto ainda resta para decidir
- pode sugerir buckets quando o planejamento ainda nao estiver configurado
- pode tratar cartao como parte do comprometimento do mes

### 4. Foi criado o endpoint de preparo do proximo mes

Novo endpoint:

- `POST /api/monthly-review/prepare`

Objetivo:

- preparar o proximo mes usando o ciclo anterior como base

O preparo atual:

- copia obrigacoes recorrentes
- copia faturas de cartao como base quando fizer sentido
- informa o que foi criado
- informa o que foi pulado
- informa se o mes alvo ja esta fechado

Isso cria a fundacao da virada assistida de mes sem recadastro completo.

### 5. Dados de relatorios de despesas passaram a respeitar o usuario logado

Foi corrigido um vazamento importante:

- os relatorios por mes de despesas buscavam dados apenas por periodo
- nao filtravam pelo usuario autenticado

Correcao aplicada:

- `PDF` e `Excel` agora filtram por mes e por usuario logado

Impacto pratico:

- um usuario nao recebe mais despesas de outro no relatorio

### 6. Categorias oficiais ganharam CRUD proprio

Foi implementado um dominio proprio de categorias oficiais do usuario.

Novos endpoints:

- `GET /api/expense-categories`
- `POST /api/expense-categories`
- `PUT /api/expense-categories/{id}`
- `DELETE /api/expense-categories/{id}`

As categorias continuam pertencendo ao `Household` do usuario autenticado.

### 7. Despesas e obrigacoes passaram a se vincular formalmente a categorias oficiais

Foi introduzido vinculo formal por `categoryId` em:

- despesas
- obrigacoes financeiras

Novos campos principais de request:

- `RequestExpense.categoryId`
- `RequestExpense.categoryName`
- `RequestFinancialObligation.categoryId`

Novos campos principais de response:

- `categoryId`
- `categoryName`

Regra atual:

- o frontend deve preferir enviar `categoryId`
- `categoryName` segue existindo por compatibilidade e exibicao

Com isso, categorias oficiais passam a servir para:

- organizacao da UI
- filtros
- consistencia entre contas e despesas
- relatorios

### 8. Exclusao de categoria agora respeita uso real

Ao excluir categoria oficial:

- se estiver em uso por despesa ou obrigacao, o backend responde `409 Conflict`

Impacto pratico:

- o frontend precisa tratar esse erro com mensagem adequada

### 9. Relatorios passaram a exibir categoria oficial da despesa

Foi ajustado o conteudo dos relatorios:

- `Excel` passou a incluir coluna de categoria
- `PDF` passou a exibir a categoria da despesa

## O que o frontend precisa saber desta sessao

### 1. Novo fluxo recomendado

Fluxo principal recomendado:

1. criar ou atualizar `financial-setup` minimo
2. cadastrar obrigacoes do mes
3. consultar `dashboard/monthly-summary`
4. mostrar comprometimento e valor restante
5. configurar buckets se ainda nao existirem
6. usar `monthly-review/prepare` para preparar o proximo mes

### 2. Categoria oficial agora deve ser tratada como entidade

O frontend nao deve mais depender apenas de texto livre para categoria.

Fluxo recomendado:

1. carregar `GET /api/expense-categories`
2. deixar o usuario selecionar categoria oficial
3. se nao existir, criar via `POST /api/expense-categories`
4. salvar despesa ou obrigacao com `categoryId`

### 3. Despesas e obrigacoes devem preferir `categoryId`

Para criar ou editar:

- preferir enviar `categoryId`
- manter `categoryName` por compatibilidade visual

### 4. O front deve tratar erros de conflito de categoria

Casos principais:

- tentativa de criar categoria duplicada
- tentativa de excluir categoria em uso

Resposta esperada:

- `409 Conflict`

### 5. O dashboard agora suporta o onboarding em duas etapas

O front deve usar:

- `commitmentPercentage`
- `remainingToAllocate`
- `suggestedBuckets`
- `requiresReview`

Para construir:

- tela de comprometimento inicial
- sugestao de distribuicao do restante
- destaque visual para itens que exigem revisao

## Validacao executada nesta sessao

Suites executadas com sucesso:

- `dotnet test tests/UseCases.Test/UseCases.Tests.csproj`
- `dotnet test tests/WebApi.Test/WebApi.Test.csproj`

## Arquivos de referencia desta sessao

- regras de negocio do core: [`docs/core-financial-business-rules.md`](/mnt/c/Users/lucas/source/repos/CashFlow/docs/core-financial-business-rules.md)
- roadmap de produto: [`docs/product-roadmap-context.md`](/mnt/c/Users/lucas/source/repos/CashFlow/docs/product-roadmap-context.md)
- fases do produto: [`docs/phase-delivery-context.md`](/mnt/c/Users/lucas/source/repos/CashFlow/docs/phase-delivery-context.md)
- contexto de continuidade: [`docs/next-session-context.md`](/mnt/c/Users/lucas/source/repos/CashFlow/docs/next-session-context.md)

## Hotfix de incidente identificado depois da entrega

Incidente observado:

- respostas `500` em rotas do onboarding e consultas relacionadas
- payload devolvido ao front como `{"errorMessages":["Unknown error"]}`

Causa raiz:

- o backend passou a consultar `FinancialObligations.ExpenseCategoryId`
- a migration `20260410093000_AddOfficialExpenseCategoriesLink` existia no codigo, mas nao estava anotada com `[Migration(...)]` e `[DbContext(...)]`
- por isso o EF nao descobria a migration no startup
- o banco seguia sem a coluna `ExpenseCategoryId` em `FinancialObligations` e `Expenses`

Correcao aplicada:

- adicionados os atributos de migration em [`src/CashFlow.Infrastructure/Migrations/20260410093000_AddOfficialExpenseCategoriesLink.cs`](/mnt/c/Users/lucas/source/repos/CashFlow/src/CashFlow.Infrastructure/Migrations/20260410093000_AddOfficialExpenseCategoriesLink.cs)
- adicionado log de excecao nao tratada em [`src/CashFlow.Api/Filters/ExceptionFilter.cs`](/mnt/c/Users/lucas/source/repos/CashFlow/src/CashFlow.Api/Filters/ExceptionFilter.cs)

Acao operacional necessaria:

- reiniciar a API para o startup aplicar a migration pendente no MySQL do ambiente
- se o ambiente nao executar migration automatica, aplicar manualmente a migration/DDL correspondente antes de subir a API nova

## Ajuste posterior de contrato para categorias sem bucket

Regra consolidada:

- categoria oficial pode ser criada antes da configuracao de buckets
- o vinculo com bucket passou a ser realmente opcional

Alteracoes aplicadas:

- `ExpenseCategory.BucketCode` passou a aceitar `null`
- `ResponseExpenseCategory.bucketCode` passou a retornar `null` quando nao houver bucket
- `ResponseExpenseCategory.bucketName` passou a retornar `null` quando nao houver bucket
- o fluxo de `financial-setup` tambem passou a aceitar `expenseCategories[].bucketName = null`

Migration adicionada:

- [`src/CashFlow.Infrastructure/Migrations/20260410113000_MakeExpenseCategoryBucketOptional.cs`](/mnt/c/Users/lucas/source/repos/CashFlow/src/CashFlow.Infrastructure/Migrations/20260410113000_MakeExpenseCategoryBucketOptional.cs)

## Encerramento desta sessao

Estado combinado ao final:

- backend ajustado para suportar o fluxo novo e os hotfixes identificados durante os testes
- categorias oficiais agora podem existir sem bucket
- erro de migration nao descoberta foi corrigido
- contratos principais usados pelo frontend nesta rodada ficaram estabilizados

Foco imediato decidido:

- neste momento o trabalho principal segue no frontend
- prioridade atual e testar fluxos, validar contratos e corrigir bugs encontrados na integracao
- por enquanto nao ha decisao de abrir novas frentes de evolucao no backend alem de correcoes necessarias

Diretriz para a proxima sessao:

- retomar backend apenas se surgirem bugs, inconsistencias contratuais ou bloqueios reais vindos dos testes do frontend
- se nao houver bloqueio, deixar backlog de melhorias de backend para uma rodada futura mais consciente
