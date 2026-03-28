# Terraform

Esta pasta e um placeholder intencional para a futura trilha de IaC do projeto.

O objetivo nao e provisionar cloud agora. O objetivo e deixar a estrutura pronta para quando o CashFlow tiver:

- estrategia de deploy definida
- ambientes remotos claros, como `dev`, `staging` e `prod`
- requisitos reais de rede, storage, observabilidade e secrets

Estrutura sugerida:

- `environments/`: composicao por ambiente
- `modules/`: modulos reaproveitaveis

Decisao atual:

- manter a infraestrutura local em Docker Compose
- adiar Terraform ate existir um alvo concreto de cloud
- reaproveitar no futuro os conceitos de naming, variaveis, outputs e modularizacao
