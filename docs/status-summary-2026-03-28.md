# CashFlow Status Summary - 2026-03-28

## Executive Summary

O projeto saiu de uma base com backend .NET e MySQL para um estado mais completo de ambiente local e de evolucao arquitetural.

Hoje, o `CashFlow` esta com:

- infraestrutura local funcional em Docker Compose
- API recolocada no stack de containers
- MySQL, Redis e MinIO operacionais no ambiente local
- migrations executando no startup da API
- Swagger funcionando no endpoint local
- suporte inicial a anexos implementado no backend
- trilha futura de IaC preparada, mas ainda sem provisionamento cloud ativo

## O Que Foi Feito Hoje

### 1. Infraestrutura local consolidada

Foi definida e consolidada a base local do projeto com:

- `api`
- `db` com `mysql:8.0`
- `redis` com `redis:7-alpine`
- `minio` como storage de objetos compativel com `S3`

Arquivos principais envolvidos:

- [`compose.yaml`](/home/lucaslisilva/projetos/CashFlow/compose.yaml)
- [`.env.example`](/home/lucaslisilva/projetos/CashFlow/.env.example)
- [`.env`](/home/lucaslisilva/projetos/CashFlow/.env)
- [`Dockerfile`](/home/lucaslisilva/projetos/CashFlow/Dockerfile)
- [`src/CashFlow.Api/appsettings.json`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Api/appsettings.json)

### 2. Direcao de storage para anexos definida

Foi tomada uma decisao arquitetural importante:

- anexos nao serao tratados como binario dentro de banco
- anexos serao tratados como arquivos em storage de objetos
- metadados dos anexos ficam no banco principal

Decisao atual:

- `MinIO` local hoje
- `S3` como evolucao natural futura
- `MongoDB` adiado por nao ser obrigatorio neste momento

### 3. Estrutura de infra e IaC preparada

Foi criada a base organizacional para evolucao de infraestrutura:

- [`infra/README.md`](/home/lucaslisilva/projetos/CashFlow/infra/README.md)
- [`infra/docker/README.md`](/home/lucaslisilva/projetos/CashFlow/infra/docker/README.md)
- [`infra/terraform/README.md`](/home/lucaslisilva/projetos/CashFlow/infra/terraform/README.md)

Objetivo:

- manter a infraestrutura local simples agora
- deixar a trilha de Terraform pronta para o futuro
- evitar acoplar deploy cloud antes da hora

### 4. Patterns e contexto do projeto formalizados

Foi criado um arquivo de referencia para manter padrao arquitetural e de implementacao:

- [`docs/project-patterns.md`](/home/lucaslisilva/projetos/CashFlow/docs/project-patterns.md)

Esse arquivo registra:

- leitura arquitetural atual
- padroes esperados de use cases, controllers e repositorios
- convencoes de nomenclatura
- interpretacao pratica de `SOLID`, `DDD` e `TDD`
- regras para novas features

### 5. Suporte inicial a anexos implementado

Foi criada a fundacao de anexos e o primeiro fluxo real de upload.

O que entrou:

- entidade `ExpenseAttachment`
- relacao entre `Expense` e `ExpenseAttachment`
- migration para criar a tabela `ExpenseAttachments`
- contratos de response com anexos
- abstracao de storage
- implementacao concreta compativel com `S3/MinIO`
- endpoint de upload de anexo para uma despesa

Arquivos principais:

- [`src/CashFlow.Domain/Entities/ExpenseAttachment.cs`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Domain/Entities/ExpenseAttachment.cs)
- [`src/CashFlow.Domain/Services/Storage/IFileStorageService.cs`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Domain/Services/Storage/IFileStorageService.cs)
- [`src/CashFlow.Infrastructure/Storage/S3CompatibleFileStorageService.cs`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Infrastructure/Storage/S3CompatibleFileStorageService.cs)
- [`src/CashFlow.Application/UseCases/Expenses/Attachments/AddExpenseAttachmentUseCase.cs`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Application/UseCases/Expenses/Attachments/AddExpenseAttachmentUseCase.cs)
- [`src/CashFlow.Api/Controllers/ExpensesController.cs`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Api/Controllers/ExpensesController.cs)
- [`src/CashFlow.Infrastructure/Migrations/20260327233000_AddExpenseAttachments.cs`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Infrastructure/Migrations/20260327233000_AddExpenseAttachments.cs)

## Estado Atual Da Aplicacao

### Stack local

O ambiente local atual esta assim:

```text
Browser / Client
        |
        v
  CashFlow API (.NET 8)
        |
        +--> MySQL 8.0        -> persistencia principal
        +--> Redis 7          -> suporte futuro para cache/operacao
        +--> MinIO            -> storage de anexos
```

### Infra executando

Pelo ultimo estado validado no ambiente:

- `cashflow-api` em execucao na porta `8080`
- `cashflow-db` em execucao e `healthy`
- `cashflow-redis` em execucao e `healthy`
- `cashflow-minio` em execucao

Endpoints locais relevantes:

- Swagger: `http://localhost:8080/swagger/index.html`
- API base: `http://localhost:8080`
- MinIO API: `http://localhost:9000`
- MinIO Console: `http://localhost:9001`
- MySQL: `localhost:3306`
- Redis: `localhost:6379`

### Banco de dados atual

O banco em uso agora e o `MySQL` do container Docker, nao o servico antigo do Windows.

Banco criado:

- `cashflow`

Tabelas relevantes aplicadas via migration:

- `Users`
- `Expenses`
- `ExpenseAttachments`
- `__EFMigrationsHistory`

### Swagger

O Swagger inicialmente quebrou por causa da assinatura do endpoint de upload com `IFormFile`.

Isso foi corrigido com um request model de formulario, e o Swagger voltou a funcionar.

## Fluxo Atual De Infra

```text
docker compose up -d --build api
        |
        +--> sobe db
        +--> sobe redis
        +--> sobe minio
        +--> builda e sobe api
        |
        v
API inicia
        |
        v
executa migrations no MySQL
        |
        v
fica disponivel em http://localhost:8080
```

## Fluxo Atual Da API

### Fluxo de despesa

```text
Request HTTP
   |
   v
Controller
   |
   v
UseCase
   |
   v
Repository / UnitOfWork
   |
   v
MySQL
```

### Fluxo de anexo

```text
POST /api/expenses/{id}/attachments
          |
          v
ExpensesController.AddAttachment
          |
          v
AddExpenseAttachmentUseCase
          |
          +--> valida arquivo
          +--> carrega despesa do usuario logado
          +--> gera StorageKey
          +--> envia arquivo para MinIO
          +--> adiciona metadado em Expense.Attachments
          +--> commit no MySQL
          |
          v
ResponseExpenseAttachment
```

## Como A Arquitetura Esta Hoje

### O que esta forte

- separacao por camadas esta boa
- controllers continuam finos
- use cases continuam sendo o centro da orquestracao
- persistencia esta atras de repositorios e `UnitOfWork`
- integracoes externas estao indo para abstracoes, nao para controller
- ha base real para testes e evolucao disciplinada

### O que ainda esta em evolucao

- dominio ainda e mais anemico do que rico
- Redis ainda esta apenas preparado, sem uso funcional real
- fluxo de anexos existe, mas ainda e inicial
- ainda nao ha estrategia ativa de deploy cloud
- Terraform existe apenas como placeholder estrutural

## Situações Legadas Tratadas

Foram encontrados pipelines legados de Azure DevOps:

- [`release-pipeline.yml`](/home/lucaslisilva/projetos/CashFlow/release-pipeline.yml)
- [`sync-build-pipeline.yml`](/home/lucaslisilva/projetos/CashFlow/sync-build-pipeline.yml)

Acao tomada:

- pipelines desativadas com `trigger: none`
- historico preservado
- comentarios adicionados para futura retomada consciente

## Comandos Operacionais Principais

Subir stack principal:

```bash
docker compose up -d --build api
```

Ver logs da API:

```bash
docker compose logs api -f
```

Ver Swagger:

```text
http://localhost:8080/swagger/index.html
```

Entrar no Redis:

```bash
docker compose exec redis redis-cli
```

Parar ambiente:

```bash
docker compose down
```

## Riscos E Observacoes

### 1. Data Protection warning

O container da API avisou que as chaves de `DataProtection` nao estao persistidas fora do container.

Impacto atual:

- aceitavel para desenvolvimento local
- deve ser tratado melhor se isso passar a importar para ambiente persistente

### 2. AutoMapper vulnerability warning

Ainda existe o warning:

- `NU1903` no pacote `AutoMapper`

Isso ja era conhecido antes e segue pendente de revisao.

### 3. API build opaco em alguns cenarios locais

Durante a sessao, houve comportamento inconsistente de `dotnet build` na `API` em alguns cenarios do ambiente, sem diagnostico claro no log.

Apesar disso:

- a imagem Docker buildou
- a API subiu
- o Swagger abriu
- as migrations rodaram

Entao o projeto esta funcional no fluxo principal via container.

## Leitura Final

Hoje o `CashFlow` esta num ponto melhor e mais maduro do que no inicio da sessao.

Resumo direto:

- a infraestrutura local esta pronta para o desenvolvimento diario
- a API esta integrada ao stack local
- o banco e o storage estao definidos de forma coerente
- anexos deixaram de ser apenas ideia e viraram capacidade inicial real
- a direcao arquitetural do projeto foi documentada
- o projeto esta mais preparado para evoluir sem perder padrao
