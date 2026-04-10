# Core Financial Business Rules

## Objetivo deste documento

Registrar as regras de negocio consolidadas do novo fluxo principal do `CashFlow`, com foco em:

- onboarding em etapas simples
- operacao mensal sem recadastro completo
- virada de mes com reaproveitamento
- preservacao de historico por snapshot

Este documento passa a ser a referencia principal para backend e frontend quando a discussao for sobre experiencia principal do usuario, e nao apenas sobre CRUD tecnico.

## Problema que o fluxo precisa resolver

O usuario nao quer preencher um sistema inteiro antes de enxergar valor.

Ele quer responder rapidamente:

- quanto entra de renda no mes
- quanto ja esta comprometido com contas
- quanto ainda sobra para decidir
- como distribuir o restante sem perder o controle

## Principios de produto

### 1. O onboarding precisa ir direto ao ponto

O fluxo inicial nao deve comecar pedindo todas as configuracoes de buckets, categorias e modelos.

A ordem correta e:

1. entender a estrutura familiar e a renda
2. cadastrar as obrigacoes principais do mes
3. calcular o quanto da renda ja esta comprometido
4. distribuir apenas o valor restante
5. entrar no dashboard

### 2. O sistema deve separar estrutura fixa de operacao mensal

Nem tudo muda todo mes.

Tendem a se repetir:

- renda fixa
- aluguel
- internet
- assinaturas
- configuracao basica de cartoes

Tendem a variar:

- contas variaveis recorrentes
- valor de fatura
- gastos novos
- ajustes pontuais do mes

O produto precisa permitir editar o mes atual sem obrigar o usuario a reconstruir toda a base.

### 3. Editar o novo mes nao pode destruir o historico

Cada mes precisa existir como uma foto consolidada do ciclo.

Isso vale principalmente para:

- renda considerada no periodo
- obrigacoes do periodo
- distribuicao do restante entre buckets
- resumo mensal consolidado
- status de fechamento

Quando o usuario abre ou ajusta um novo mes, o sistema pode reaproveitar dados anteriores, mas nao pode sobrescrever o retrato do mes anterior.

### 4. O sistema deve ajudar na virada de mes

O usuario nao deve depender de memoria ou de recadastro manual completo.

Na virada de ciclo, o produto deve:

- sugerir abertura do novo mes
- carregar como base o que tende a se repetir
- sinalizar o que precisa de revisao
- facilitar ajustes antes e depois do primeiro dia do novo mes

## Fluxo principal consolidado

### Etapa 1. Onboarding inicial

O onboarding inicial deve ser reduzido ao essencial:

- nome da familia
- quantidade de membros
- quantidade de rendas ou fontes de renda
- identificacao de renda fixa ou variavel
- frequencia principal de entrada
- valor das fontes de renda

Em seguida, no mesmo onboarding, o usuario deve ir direto para o cadastro das obrigacoes do mes.

### Etapa 2. Cadastro das obrigacoes do mes

Nesta etapa, o foco e registrar o que compromete o caixa:

- aluguel
- condominio
- luz
- agua
- internet
- assinaturas
- escola
- impostos
- outras contas fixas ou recorrentes

Cada item deve permitir pelo menos:

- titulo
- valor esperado
- vencimento
- competencia
- indicacao se e fixo ou recorrente variavel

O sistema deve priorizar simplicidade.

No fluxo inicial, obrigacao nao deve depender de configuracao previa complexa de bucket.

### Etapa 3. Leitura do comprometimento

Depois de renda e obrigacoes cadastradas, o sistema deve calcular:

- renda total planejada do mes
- valor total comprometido
- percentual da renda ja comprometido
- quanto ainda resta para decidir

Essa leitura e o primeiro momento de valor real do produto.

### Etapa 4. Distribuicao do restante

Somente depois da leitura do comprometimento o usuario deve entrar na distribuicao do restante.

Nesse momento, o sistema pode sugerir poucos buckets iniciais, por exemplo:

- essentials ou base fixa ja comprometida
- livre
- investimentos

Ou uma variacao equivalente mais didatica, desde que:

- a interface nao abra opcoes demais logo no inicio
- o modelo funcione como guia
- o usuario possa ajustar depois

O modelo percentual deve nascer em cima do valor disponivel apos as obrigacoes principais, e nao como barreira anterior ao cadastro das contas.

### Etapa 5. Dashboard principal

Depois disso, o usuario entra no dashboard ja com:

- renda registrada
- obrigacoes do mes registradas
- leitura de comprometimento pronta
- distribuicao inicial do restante configurada

## Regras para recorrencia e reaproveitamento

### 1. Itens fixos devem ser herdados para o novo mes

Ao abrir um novo mes, o sistema deve trazer preenchidos por padrao:

- fontes de renda fixas
- obrigacoes fixas mensais
- assinaturas
- configuracoes estruturais da familia

### 2. Itens variaveis podem vir copiados, mas com revisao obrigatoria

Podem ser reaproveitados como base:

- contas recorrentes variaveis
- impostos com oscilacao
- utilidades com pequenas variacoes
- fatura mensal do cartao

Mas o sistema deve avisar com clareza que esses valores precisam ser confirmados ou ajustados no novo ciclo.

### 3. O usuario deve poder ajustar o mes atual a qualquer momento

O fluxo nao deve existir apenas no primeiro cadastro.

O usuario precisa conseguir:

- revisar o novo mes antes da virada
- ajustar o mes em andamento
- incluir nova conta
- alterar valor de conta variavel
- marcar pagamentos

## Regras para fechamento e snapshot

### 1. Cada mes precisa ter um estado operacional e um estado consolidado

Leitura recomendada:

- um estado editavel do mes atual
- um snapshot consolidado quando o mes for fechado

### 2. Fechar o mes deve congelar a foto consolidada

Ao fechar o mes, o sistema deve preservar:

- renda considerada no fechamento
- obrigacoes do ciclo
- pagos, previstos e ajustados
- buckets e distribuicao considerados naquele momento
- indicadores resumidos do mes
- observacoes do usuario

### 3. Abrir o novo mes deve copiar base, nao mover historico

Virar o mes nao significa atualizar os registros do mes anterior.

Significa:

- criar um novo contexto mensal
- reaproveitar estrutura reaplicavel
- manter o mes anterior intacto para revisao, relatorio e comparacao futura

## Regras para cartao de credito

### 1. Cartao precisa entrar de forma simples no fluxo inicial

No cadastro inicial, o usuario pode nao querer detalhar compras individuais.

O fluxo deve aceitar pelo menos:

- identificacao do cartao
- vencimento
- data de fechamento
- valor atual da fatura do mes

### 2. Fatura mensal e diferente de configuracao estrutural do cartao

Devem existir duas camadas diferentes:

- cadastro estrutural do cartao
- registro mensal da fatura

O valor da fatura muda de mes para mes e deve participar da leitura de comprometimento do ciclo.

## Regras de UX e frontend

### 1. O fluxo precisa ser didatico e confiavel

O frontend deve priorizar:

- passos curtos
- linguagem simples
- feedback claro do que foi salvo
- explicacao visual do que esta comprometido e do que ainda resta

### 2. A virada de mes deve ser assistida

Sugestao de experiencia:

- entre os dias finais do mes, oferecer um popup ou CTA para preparar o proximo ciclo
- permitir fechar o mes anterior quando fizer sentido
- iniciar o novo mes com itens fixos ja preenchidos
- destacar em aviso os itens variaveis que precisam de validacao

### 3. O dashboard deve mostrar o status do ciclo

O usuario precisa entender rapidamente:

- se o mes esta aberto ou fechado
- se ja existe proximo mes preparado
- o que foi herdado do mes anterior
- o que ainda precisa ser revisado

## Direcao tecnica recomendada

### 1. Separar setup estrutural de setup mensal

O dominio nao deve tratar todo o onboarding como um unico pacote fixo.

### 2. Reduzir dependencia precoce de buckets

Obrigacoes iniciais nao devem exigir bucket definido antes do calculo do comprometimento.

### 3. Introduzir uma camada mensal explicita

A evolucao natural do modelo aponta para algo como:

- configuracao estrutural da familia
- instancia operacional do mes
- fechamento com snapshot consolidado

### 4. Preparar notificacoes como evolucao futura

Notificacoes por e-mail e WhatsApp fazem sentido para:

- lembrar revisao de fim de mes
- lembrar fechamento do ciclo
- lembrar validacao de valores variaveis

Mas isso entra depois de consolidar o fluxo principal e a modelagem mensal.
