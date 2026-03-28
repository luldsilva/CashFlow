# Infra Docker Contexto

## Objetivo
Preparar a infraestrutura do backend CashFlow no WSL Ubuntu usando Docker, de forma incremental, sem perder contexto entre sessoes e deixando a estrutura reaproveitavel como template para projetos futuros.

## Ambiente Ja Preparado
- Projeto `CashFlow` disponivel no WSL Ubuntu.
- Docker Engine instalado no Ubuntu e funcional.
- Codex instalado e com acesso ao projeto.

## O Que Ja Foi Feito Nesta Sessao
- Criado [`compose.yaml`](/home/lucaslisilva/projetos/CashFlow/compose.yaml) na raiz.
- Criado [`.dockerignore`](/home/lucaslisilva/projetos/CashFlow/.dockerignore) na raiz.
- Criado [`.env.example`](/home/lucaslisilva/projetos/CashFlow/.env.example) na raiz.
- Ajustado [`Dockerfile`](/home/lucaslisilva/projetos/CashFlow/Dockerfile) para build/publish da API com porta `8080`.
- Ajustada a aplicacao para aceitar configuracao de banco por variaveis de ambiente:
  - [`src/CashFlow.Infrastructure/DependencyInjectionExtension.cs`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Infrastructure/DependencyInjectionExtension.cs)
  - [`src/CashFlow.Infrastructure/Extensions/ConfigurationExtensions.cs`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Infrastructure/Extensions/ConfigurationExtensions.cs)
  - [`src/CashFlow.Api/Program.cs`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Api/Program.cs)
  - [`src/CashFlow.Api/appsettings.json`](/home/lucaslisilva/projetos/CashFlow/src/CashFlow.Api/appsettings.json)
- Simplificado [`compose.yaml`](/home/lucaslisilva/projetos/CashFlow/compose.yaml) para iniciar apenas o servico `db` com `mysql:8.0`.
- Alinhado [`.dockerignore`](/home/lucaslisilva/projetos/CashFlow/.dockerignore) para um baseline minimo e generico de projeto containerizado.
- Alinhado [`.env.example`](/home/lucaslisilva/projetos/CashFlow/.env.example) para conter apenas as variaveis do MySQL desta etapa.
- Alinhado [`.env`](/home/lucaslisilva/projetos/CashFlow/.env) com as mesmas variaveis para permitir execucao imediata nesta sessao.
- Validado `docker compose config` com sucesso usando a configuracao simplificada.
- Executado `docker compose up -d db`.
- Criado o volume `cashflow_cashflow_mysql_data`.
- Subido o container `cashflow-db`.
- Validado que o healthcheck do MySQL ficou `healthy`.

## Estado Atual

### Stack Atual
- `api` recolocada no `compose.yaml`
- `db` com `mysql:8.0`
- `redis` com `redis:7-alpine`
- `minio` para anexos em storage de objetos compativel com `S3`
- pasta `infra/terraform/` criada como placeholder para futura trilha de IaC

### .dockerignore
- `**/bin`
- `**/obj`
- `**/.vs`
- `**/.vscode`
- `.git`
- `.gitignore`
- `.env`

### .env.example
- `MYSQL_DATABASE=cashflow`
- `MYSQL_USER=cashflow`
- `MYSQL_PASSWORD=cashflow`
- `MYSQL_ROOT_PASSWORD=root`
- `MYSQL_PORT=3306`

### compose.yaml
- Servico `api` com build pelo [`Dockerfile`](/home/lucaslisilva/projetos/CashFlow/Dockerfile)
- API configurada por variaveis de ambiente para MySQL, JWT, Redis e Storage
- Servico `db` com imagem `mysql:8.0`
- Container `cashflow-db`
- `restart: unless-stopped`
- Variaveis `MYSQL_DATABASE`, `MYSQL_USER`, `MYSQL_PASSWORD`, `MYSQL_ROOT_PASSWORD`
- Porta `${MYSQL_PORT}:3306`
- Volume `cashflow_mysql_data:/var/lib/mysql`
- `healthcheck` com `mysqladmin ping`
- Servico `redis` com `redis:7-alpine`
- Volume `cashflow_redis_data:/data`
- `healthcheck` com `redis-cli ping`
- Servico `minio` com volume `cashflow_minio_data:/data`
- Portas `${MINIO_API_PORT}:9000` e `${MINIO_CONSOLE_PORT}:9001`

## Validacoes Executadas
- `docker compose config` resolveu corretamente os valores vindos de `.env`.
- `docker compose up -d db` baixou a imagem `mysql:8.0`, criou o volume nomeado e iniciou o banco.
- `docker compose logs db --tail 80` confirmou:
  - criacao do schema `cashflow`
  - criacao do usuario `cashflow`
  - inicializacao final do servidor na porta `3306`
- `docker inspect --format '{{json .State.Health}}' cashflow-db` retornou `Status=healthy`.

## Warnings E Observacoes Tecnicas
- O comando `docker compose up -d db` exibiu warning sobre container orfao `cashflow-api`.
- Isso aconteceu porque antes existia um `compose.yaml` com servico `api`, e a etapa atual simplificou o compose para `db` apenas.
- O warning nao bloqueia a etapa atual.
- Se for desejado limpar esse legado depois, usar `docker compose up -d --remove-orphans` ou remover o container explicitamente, mas isso deve ser feito de forma consciente porque pode desligar a API que estava rodando.
- Os logs do MySQL mostraram warnings padrao da imagem oficial:
  - `--skip-host-cache` deprecated
  - certificado `ca.pem` self-signed
  - aviso sobre `pid-file`
  - aviso do `mysqladmin` sobre senha na linha de comando no healthcheck
- Nenhum desses warnings bloqueou a inicializacao.

## Direcao De Template
- Manter o `compose.yaml` orientado a infraestrutura, subindo servicos por etapas pequenas e verificaveis.
- Manter `.env.example` enxuto, com apenas as variaveis necessarias para a etapa atual ou para o stack publicado.
- Preferir nomes de servico e volume semanticamente neutros e reaproveitaveis:
  - servico `db`
  - volume `cashflow_mysql_data`
- Ao adicionar novos servicos, seguir o mesmo padrao:
  - variaveis em `.env.example`
  - `healthcheck` quando houver suporte
  - volume nomeado quando houver persistencia
  - validacao por `docker compose config`, `up -d`, `ps` e `logs`

## Proximos Passos Recomendados
1. Validar a subida completa de `api + db + redis + minio`.
2. Definir o modelo de metadados de anexos no banco principal.
3. Criar a primeira abstracao de storage na aplicacao apontando para `MinIO`, de forma compativel com futura migracao para `S3`.
4. Decidir se a criacao de bucket sera feita pela aplicacao, por bootstrap local ou por etapa operacional separada.
5. Opcionalmente separar o compose em arquivos mais genericos no futuro, por exemplo:
   - `compose.yaml` para infraestrutura base
   - `compose.override.yaml` para desenvolvimento local
   - `compose.app.yaml` para subir API junto da infra

## Fluxo Ideal Definido Nesta Sessao
- Fase 1: estabilizar `api + db + redis + minio` no compose.
- Fase 2: modelar anexos como arquivos em storage de objetos com metadados no banco principal.
- Fase 3: implementar integracao de storage na aplicacao.
- Fase 4: reavaliar MongoDB somente se surgir um caso real de documento semiestruturado rico.
- Fase 5: retomar Terraform quando houver alvo concreto de deploy.

## Passos Combinados Para Amanhã
1. Manter a infra base como esta.
2. Adicionar Redis e MinIO.
3. Depois recolocar a API no `compose` de forma limpa e coerente.

## Proximos Passos Operacionais
- Confirmar o desenho de anexos usando storage de objetos com `MinIO` local e futura compatibilidade com `S3`.
- Definir as variaveis de ambiente padrao para `Redis` e `MinIO` em formato reutilizavel.
- Modelar depois os metadados de anexos no banco principal.
- Decidir se o nome do volume do MySQL deve continuar com prefixo do projeto (`cashflow_mysql_data`) ou ser generalizado quando for criado um template externo.

## Warnings Observados
- `NU1903` relacionado ao pacote `AutoMapper`.
- `CS8618` em `tests/WebApi.Test/CustomWebApplicationFactory.cs`.
- O usuario confirmou que `dotnet build` executado localmente no VS Code terminou com sucesso.

## Regra De Trabalho Combinada
Nesta sessao, o usuario autorizou executar todas as configuracoes necessarias sem validacao manual intermediaria.
- Em sessoes futuras, confirmar novamente esse modo de trabalho antes de assumir autonomia total.

## Observacoes
- O arquivo `.env` existente foi alterado nesta sessao para refletir a etapa atual focada apenas no MySQL.
- O container `cashflow-db` ficou em execucao ao final desta etapa.
- O container `cashflow-api` ja existia anteriormente e permaneceu em execucao fora do compose atual, aparecendo como orfao nas verificacoes.
- Este arquivo de contexto pode ser apagado depois, quando nao for mais necessario.

## Como Retomar Amanhã
Pedir para o Codex:
- "continue a partir de `docs/infra-plan.md`"
- ou "leia `docs/infra-plan.md` e siga da etapa do Redis e MinIO"
- ou "leia `docs/infra-plan.md` e reconecte a API depois da infra base"
- ou "leia `docs/infra-plan.md` e siga os tres passos combinados para amanhã"
