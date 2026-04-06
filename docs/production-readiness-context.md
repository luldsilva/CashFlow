# Production Readiness Context

## Objetivo deste documento

Registrar pontos do backend que estao corretos para desenvolvimento local e validacao funcional, mas que precisam de implementacao, configuracao ou endurecimento especifico antes de um uso de producao.

Este documento serve para evitar que detalhes de ambiente local sejam confundidos com decisoes finais de produto ou infraestrutura.

## 1. Reset de senha por e-mail

### Estado atual

O fluxo de reset por e-mail foi implementado com:

- geracao de token
- persistencia do token com expiracao
- invalidacao de tokens anteriores
- redefinicao de senha por token
- abstracao de envio de e-mail

Hoje o envio esta acoplado a uma implementacao de desenvolvimento:

- [`DevelopmentEmailSender`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Infrastructure/Email/DevelopmentEmailSender.cs)

Essa implementacao:

- nao envia e-mail real
- grava/loga o link de redefinicao
- atende o fluxo local e os testes automatizados

### O que precisa mudar em producao

Trocar a implementacao de `IEmailSender` por um provider real, por exemplo:

- `Resend`
- `SendGrid`
- `Amazon SES`
- `Postmark`

### O que precisa ser configurado

- credenciais do provider
- remetente oficial
- dominio de envio
- reputacao e autenticacao do dominio, quando aplicavel
- templates de e-mail reais

### Recomendacao

Manter a abstracao atual e criar uma implementacao concreta de producao sem alterar os casos de uso.

## 2. URL de reset de senha

### Estado atual

O link enviado usa configuracao de aplicacao:

- `PasswordReset:BaseUrl`

Hoje o valor padrao esta voltado para ambiente local/frontend local.

### O que precisa mudar em producao

Apontar para a URL real da tela de redefinicao de senha do frontend publicado.

Exemplo:

- `https://app.seudominio.com/reset-password`

## 3. Data Protection

### Estado atual

Os logs da API mostram warning de `DataProtection` persistida apenas no filesystem do container.

### O que precisa mudar em producao

Persistir chaves de `DataProtection` em storage compartilhado ou provider apropriado, dependendo do ambiente.

Sem isso:

- chaves podem se perder ao recriar container
- comportamento de recursos que dependem dessas chaves pode ficar instavel

## 4. Segredos e configuracao sensivel

### Estado atual

O projeto usa configuracao local e variaveis simples de ambiente para:

- JWT
- banco
- storage
- reset de senha

### O que precisa mudar em producao

Usar estrategia segura para segredos, por exemplo:

- secret manager cloud
- cofre centralizado
- variaveis de ambiente gerenciadas por plataforma de deploy

Evitar:

- segredos em arquivo versionado
- valores hardcoded

## 5. Sender de e-mail e observabilidade

### Estado atual

O envio de e-mail de desenvolvimento so registra logs.

### O que precisa mudar em producao

Adicionar:

- tratamento de falha de entrega
- logs estruturados por tentativa
- monitoramento de erro de envio
- politicas de retry, se fizer sentido

## 6. Endurecimento do fluxo de reset

### Estado atual

O fluxo ja tem:

- token aleatorio
- expiracao
- uso unico
- invalidacao de tokens anteriores

### Melhorias recomendadas para producao

- rate limit por e-mail/IP na solicitacao de reset
- auditoria minima do evento
- resposta neutra para evitar enumeracao de usuarios
- monitoramento de volume anormal de solicitacoes

## 7. Estado atual considerado pronto para desenvolvimento

Hoje o backend esta pronto para:

- desenvolvimento local
- integracao com frontend
- validacao funcional do fluxo de reset
- testes automatizados

Nao significa que esta pronto para envio real de e-mails em producao sem a troca do provider.
