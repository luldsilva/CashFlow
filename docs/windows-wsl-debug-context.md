# Contexto de Migracao para Windows e Debug WSL

## Objetivo deste documento

Registrar a migracao do codigo para o caminho Windows e o fluxo que funcionou para depurar a API no Visual Studio enquanto o frontend continua rodando no WSL.

## Migracao do codigo

O codigo do projeto foi sincronizado para o caminho Windows:

- `C:\Users\lucas\source\repos\CashFlow`

No WSL, o mesmo projeto deve ser acessado por:

- `/mnt/c/Users/lucas/source/repos/CashFlow`

Esse passou a ser o caminho de trabalho compartilhado entre:

- `Visual Studio` no Windows
- terminal e `Codex` no WSL

## O que causava o breakpoint nao bater

O problema nao estava no breakpoint do controller de login.

O ponto principal era o roteamento da chamada do frontend:

- o frontend em `Vite` fazia `POST /api/Login`
- esse caminho estava sendo proxyado para `http://localhost:8080`
- `localhost:8080` era a API em Docker
- o breakpoint estava no processo da API iniciado pelo `Visual Studio`

Resultado:

- o login funcionava em outra instancia
- o breakpoint no `Visual Studio` nao era atingido

## Fluxo correto para depurar com frontend no WSL

### Infraestrutura

Manter no Docker apenas:

- `db`
- `redis`
- `minio`

Comando usado:

```bash
docker compose up -d db redis minio
docker compose stop api
```

Observacao:

- o serviço `api` no `compose.yaml` usa profile `full-stack`
- por isso `docker compose up -d` pode subir apenas `db`, `redis` e `minio`
- isso estava correto

### API

Para esse cenário, a API deve ser executada no `Visual Studio` com o profile:

- `WSL`

Com esse profile, o Swagger ficou acessivel em:

- `https://localhost:7058/swagger/index.html`

O teste que confirmou conectividade a partir do WSL foi:

```bash
curl -k -i https://localhost:7058/swagger/index.html
```

Esse teste respondeu `HTTP/2 200`.

### Frontend

O frontend permaneceu no WSL com `Vite`.

O proxy que funcionou foi:

```ts
server: {
  port: 5173,
  proxy: {
    '/api': {
      target: 'https://localhost:7058',
      changeOrigin: true,
      secure: false,
    },
  },
},
```

O campo `secure: false` foi necessario porque a API exposta nesse fluxo usa certificado local self-signed.

Sem isso, o erro no `Vite` era:

- `self-signed certificate`

## Resultado final

Com esse arranjo:

- frontend no `WSL`
- API no `Visual Studio` com profile `WSL`
- `db`, `redis` e `minio` no Docker
- proxy do `Vite` para `https://localhost:7058` com `secure: false`

o login voltou a funcionar e o breakpoint passou a bater normalmente.

## Observacao sobre tentativas que nao resolveram

As tentativas abaixo nao fecharam o fluxo:

- proxy do `Vite` para `http://localhost:8080`
- proxy para IP do host Windows a partir do WSL
- uso da API no `Visual Studio` com profile `http` apontando para `localhost:5198`

Para este projeto e neste ambiente, o fluxo confiavel foi o profile `WSL` da API.

## Impacto em GitHub e deploy

O arranjo local atual:

- backend no Windows
- frontend no WSL
- API depurada pelo `Visual Studio`
- infraestrutura de apoio em Docker

nao deve causar problema por si so ao publicar no GitHub ou ao preparar fluxo de deploy.

Esse setup e apenas uma forma de desenvolvimento local.

### O que nao deve impactar GitHub

O GitHub recebe os arquivos versionados, nao o fato de o desenvolvimento ter sido feito em:

- Windows
- WSL
- ou uma combinacao dos dois

Logo, esse fluxo local nao deve afetar:

- branch
- commit
- pull request
- merge
- GitHub Actions

### O que precisa ficar separado do deploy

Configuracoes locais nao devem ser tratadas como configuracoes de producao.

Pontos de atencao:

- nao commitar caminhos absolutos de maquina como `C:\Users\...` ou `/mnt/c/...`
- nao usar `localhost:7058`, `localhost:5198` ou `localhost:8080` como URL de ambiente real
- manter proxy do `Vite` restrito ao desenvolvimento local
- usar variaveis de ambiente para URLs reais de homologacao e producao
- nao depender do profile `WSL` do `Visual Studio` para publicacao

### Leitura pratica

O fluxo local hibrido pode continuar existindo sem problema, desde que:

- o projeto mantenha configuracao de ambiente bem separada
- o deploy use configuracoes proprias do ambiente alvo
- a documentacao deixe claro o que e apenas debug local e o que e fluxo oficial de execucao/publicacao

### Riscos reais para revisar depois

Os riscos mais provaveis nao sao do Windows + WSL em si, mas sim de acoplamento indevido de configuracoes locais:

- `vite.config` ficar preso a um proxy muito especifico para um unico ambiente
- `launchSettings.json` sofrer alteracoes que atrapalhem outros perfis locais
- configuracoes de deploy dependerem acidentalmente de `localhost`
- documentacao nao separar bem desenvolvimento local de execucao em deploy

Isso deve ser revisitado antes de consolidar pipeline ou estrategia de publicacao.
