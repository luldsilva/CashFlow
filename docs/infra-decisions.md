# Infra Decisions

## Decisao Atual

O `CashFlow` seguira primeiro com infraestrutura local orientada ao MVP e ao desenvolvimento diario.

Stack de infraestrutura local:

- `MySQL` como banco principal do dominio
- `Redis` como apoio para cache e futuras necessidades operacionais
- `MinIO` como storage de objetos para anexos

## Racional

### MySQL

O dominio principal do sistema de controle de gastos e relacional. Usuarios, gastos, categorias, contas e relatorios se encaixam melhor em banco relacional.

### Redis

O Redis nao e requisito funcional do MVP, mas faz sentido como apoio de curto e medio prazo para:

- cache de consultas e dashboards
- rate limiting
- chaves temporarias e coordenacao simples

### MinIO

Os anexos do sistema devem ser tratados como arquivos, nao como documentos armazenados diretamente em banco.

Uso esperado:

- foto de produto
- nota fiscal
- comprovante
- print relacionado a gasto

Direcao adotada:

- arquivo armazenado em storage de objeto
- metadados e vinculo com o gasto armazenados no banco principal

O uso de `MinIO` local permite um fluxo compativel com futura migracao para `S3`, reduzindo retrabalho.

## MongoDB

O `MongoDB` foi adiado.

Ele pode voltar a ser considerado caso surja um caso real de:

- documentos semiestruturados ricos
- pipeline de OCR ou processamento documental
- metadados variaveis que nao caibam bem no modelo relacional

Neste momento, ele nao e necessario para o MVP.

## Trilha De IaC

O projeto passa a reservar a pasta `infra/terraform/` como placeholder de evolucao.

O uso de Terraform sera retomado quando houver:

- alvo claro de deploy
- ambientes remotos definidos
- necessidade concreta de provisionar cloud

## Principios

- evitar complexidade prematura
- manter compatibilidade com evolucao futura
- priorizar um ambiente local reproduzivel
- separar claramente runtime local de futura infraestrutura remota
