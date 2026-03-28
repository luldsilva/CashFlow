# CashFlow Project Patterns

Este arquivo registra os padroes arquiteturais e convencoes que devem orientar novas implementacoes no projeto.

## Objetivo

Manter consistencia de arquitetura, nomenclatura e organizacao do codigo, evitando que o projeto se degrade com implementacoes pontuais fora do padrao.

## Leitura Rapida

Antes de implementar qualquer feature nova, consultar este arquivo junto com:

- [`docs/infra-decisions.md`](/home/lucaslisilva/projetos/CashFlow/docs/infra-decisions.md)
- [`docs/infra-plan.md`](/home/lucaslisilva/projetos/CashFlow/docs/infra-plan.md)

## Direcao Arquitetural Atual

O projeto segue uma separacao inspirada em `DDD` e `Clean Architecture`, com responsabilidades distribuidas por camadas:

- `CashFlow.Api`: controllers, middleware, configuracao da aplicacao e composicao
- `CashFlow.Application`: casos de uso e orquestracao de regras de aplicacao
- `CashFlow.Domain`: entidades, contratos e abstracoes centrais do dominio
- `CashFlow.Infrastructure`: persistencia, seguranca, implementacoes externas e integracoes tecnicas
- `CashFlow.Communication`: contratos de request e response
- `CashFlow.Exception`: excecoes e mensagens padronizadas
- `tests/*`: testes por camada e tipo

## Padroes Que Ja Estao Claros No Projeto

### 1. Use Cases por acao

Cada acao relevante do sistema deve ficar em um caso de uso explicito, por exemplo:

- `RegisterExpenseUseCase`
- `GetExpenseByIdUseCase`
- `DeleteExpenseUseCase`
- `AddExpenseAttachmentUseCase`

Regra:

- controllers nao devem conter regra de negocio
- controllers apenas recebem a entrada HTTP, chamam o caso de uso e retornam a resposta

### 2. Controllers finos

Os controllers devem continuar leves, sem acesso direto a banco, sem regras de validacao complexa e sem montagem manual de dominio.

Responsabilidade esperada do controller:

- receber request
- delegar para um caso de uso
- transformar o resultado em resposta HTTP

### 3. Contratos via interfaces

Dependencias entre camadas devem continuar orientadas por interfaces, por exemplo:

- repositorios no `Domain`
- servicos como `ILoggedUser`, `IAccessTokenGenerator`, `IFileStorageService`

Regra:

- `Application` depende de abstracoes
- `Infrastructure` implementa essas abstracoes

### 4. Requests e Responses separados do dominio

Os contratos HTTP devem continuar em `CashFlow.Communication`.

Regra:

- nao expor entidade de dominio diretamente pela API
- requests e responses devem ser classes proprias

### 5. AutoMapper apenas entre contratos e dominio

O uso atual do `AutoMapper` e coerente quando limitado a:

- request -> entity
- entity -> response

Regra:

- nao usar AutoMapper para esconder regra de negocio
- transformacoes com decisao de negocio devem continuar explicitas nos casos de uso

### 6. Persistencia atras de repositorio + unit of work

O padrao atual usa:

- repositorios para acesso a dados
- `IUnitOfWork` para commit transacional

Regra:

- casos de uso alteram estado e finalizam com `Commit()`
- consultas e comandos continuam separados por interfaces quando fizer sentido

### 7. Excecoes de dominio/aplicacao padronizadas

O tratamento de erro centralizado por `ExceptionFilter` deve ser preservado.

Regra:

- casos de uso devem lancar excecoes de negocio apropriadas
- controllers nao devem montar manualmente respostas de erro complexas

### 8. Infraestrutura externa atras de abstracao

A direcao de anexos estabeleceu um padrao importante:

- dominio define o contrato
- infraestrutura implementa o provider concreto

Exemplo:

- `IFileStorageService` no dominio
- `S3CompatibleFileStorageService` na infraestrutura

Esse mesmo padrao deve ser seguido para Redis, filas, e-mails e integracoes futuras.

## Como Interpretar SOLID No Projeto

### S de Single Responsibility

O projeto esta no caminho certo:

- controllers focam em HTTP
- use cases focam em fluxo de aplicacao
- repositorios focam em persistencia

Mas isso exige disciplina continua:

- evitar validacoes espalhadas em controller, repository e service ao mesmo tempo
- evitar classes de use case acumulando responsabilidades demais

### O de Open/Closed

O projeto esta razoavelmente preparado para extensao por interface e novas implementacoes.

Exemplo atual:

- storage pode trocar de provider sem mudar o caso de uso

### L de Liskov

Nao ha sinal de violacao grave aqui. As abstracoes usadas sao simples e coerentes.

### I de Interface Segregation

O projeto ja usa segregacao em varios pontos:

- `IExpensesReadOnlyRepository`
- `IExpensesWriteOnlyrepository`
- `IExpensesUpdateOnlyrepository`

Isso faz sentido e deve continuar quando a separacao realmente reduzir acoplamento.

### D de Dependency Inversion

Este e um dos pontos mais fortes da estrutura atual:

- `Application` depende de interfaces
- `Infrastructure` implementa

Esse padrao deve ser mantido.

## Como Interpretar DDD No Projeto

O projeto tem uma estrutura fortemente inspirada em DDD, mas ainda em nivel inicial.

O que ja existe:

- separacao entre dominio e infraestrutura
- entidades de dominio
- repositorios como contratos do dominio
- casos de uso fora da camada de entrega

O que ainda nao esta forte em DDD:

- entidades ainda sao anemicas, com pouca regra encapsulada
- ha pouca expressao de value objects
- regras de negocio ainda vivem mais em use cases do que no dominio

Direcao recomendada:

- continuar com esse desenho
- mover para o dominio regras que realmente pertencem ao comportamento da entidade
- evitar transformar tudo em logica procedural na camada de aplicacao

## Como Interpretar TDD No Projeto

Ha evidencias reais de cultura de testes:

- testes de use case
- testes de validadores
- testes de web api
- builders e doubles em `CommonTestUtilities`

Direcao recomendada:

- toda feature nova relevante deve nascer com teste de caso de uso
- endpoints novos devem ganhar ao menos cobertura de integracao basica
- validacoes de request devem ser cobertas explicitamente

## Convencoes De Nomenclatura

### Manter

- nomes explicitos para casos de uso
- interfaces com prefixo `I`
- requests com prefixo `Request`
- responses com prefixo `Response`

### Ajustar daqui para frente

Preferir nomes consistentes em ingles correto e casing padrao de .NET.

Evitar repetir variacoes como:

- `IExpensesWriteOnlyrepository`
- `IExpensesUpdateOnlyrepository`

Padrao preferivel para novos codigos:

- `IExpensesWriteOnlyRepository`
- `IExpensesUpdateOnlyRepository`

Nao sair renomeando tudo agora sem necessidade. Mas, em codigo novo, seguir o padrao correto.

## Convencoes De Pastas

### Application

Organizar por feature e acao:

- `UseCases/Expenses/Register`
- `UseCases/Expenses/GetById`
- `UseCases/Expenses/Attachments`

### Domain

Concentrar:

- entidades
- enums
- contratos
- servicos abstratos de dominio

### Infrastructure

Concentrar:

- EF Core
- repositorios
- seguranca concreta
- storage concreto
- integracoes externas

### Communication

Separar sempre:

- `Requests`
- `Responses`

## Regras Para Novas Features

Ao adicionar uma feature nova, seguir esta sequencia sempre que possivel:

1. Definir o contrato HTTP em `Communication`
2. Criar ou ajustar o caso de uso em `Application`
3. Criar ou ajustar abstracoes no `Domain`
4. Implementar persistencia ou integracao concreta em `Infrastructure`
5. Expor a feature no controller
6. Adicionar ou atualizar testes

## Regras Especificas Para Integracoes Externas

Nunca acoplar diretamente um provider externo dentro do controller ou do caso de uso.

Exemplos:

- storage
- cache
- mensageria
- e-mail
- OCR

Fluxo esperado:

- contrato no dominio
- implementacao concreta na infraestrutura
- injecao via DI

## Red Flags

Evitar estas decisoes:

- controller acessando `DbContext` diretamente
- response usando entidade de dominio como payload
- caso de uso montando SQL ou detalhes de provider
- regra de negocio escondida em controller
- logica externa acoplada sem interface
- nomes inconsistentes ou com casing fora do padrao

## Decisao Pratica

O projeto esta no caminho certo.

Ele nao e um DDD “puro” nem um exemplo rigoroso de modelagem rica de dominio, mas a base estrutural e boa e suficientemente coerente para evoluir com disciplina.

A direcao correta agora e:

- preservar a separacao atual de camadas
- melhorar consistencia de nomenclatura
- manter controllers finos
- continuar usando casos de uso explicitos
- reforcar testes em cada nova feature
- evitar atalhos arquiteturais que misturem dominio, HTTP e infraestrutura
