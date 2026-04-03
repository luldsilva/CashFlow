# Phase Delivery Context

## Objetivo deste documento

Registrar, de forma objetiva, o resumo de negocio de cada fase implementada no produto.

A ideia deste arquivo e responder rapidamente:

- o que entrou em cada fase
- o que isso habilita para o usuario
- o que ainda nao entrou naquela etapa

Esse documento serve como referencia de continuidade entre backend e frontend, evitando que as fases virem apenas uma lista tecnica de entidades, endpoints e tabelas.

## Fase 1A - Setup financeiro inicial

### Resumo de negocio

Na Fase 1A, o usuario passa a conseguir fazer o setup financeiro inicial da familia dentro do sistema.

Em termos de produto, essa entrega cria a base do onboarding financeiro:

- informar a composicao familiar
- indicar se a renda e mais estavel ou variavel
- definir a frequencia principal de entrada de dinheiro
- escolher um modelo inicial de planejamento
- configurar a estrutura base que vai orientar a organizacao financeira

Tambem entram, nessa fase:

- cadastro das fontes de renda iniciais
- configuracao dos buckets percentuais
- definicao das categorias principais de gasto

Na pratica, isso faz o produto deixar de ser apenas um cadastro solto de despesas e passar a entender como aquela familia organiza o dinheiro.

### O que essa fase habilita

- iniciar o onboarding financeiro com dados estruturais reais
- salvar a configuracao base da familia
- escolher um modelo inicial de planejamento financeiro
- preparar a classificacao futura de gastos e leitura por buckets
- criar a fundacao necessaria para recorrencia, compromissos mensais e dashboard

### O que ainda nao entra nesta fase

- calculo operacional do mes
- leitura de saldo comprometido
- leitura de saldo livre para gastar
- motor de recorrencia de contas
- dashboard principal
- logica de cartao de credito
- fechamento e revisao mensal

## Fase 1B - Motor de compromissos do mes

### Resumo de negocio

Na Fase 1B, o produto passa a permitir que o usuario cadastre e acompanhe os compromissos financeiros que estruturam o mes.

Em termos de negocio, essa fase tira o sistema do estado de configuracao inicial e coloca o produto mais perto da operacao financeira real do ciclo mensal.

Entram nessa fase:

- obrigacoes financeiras com competencia mensal
- valor previsto da obrigacao
- vencimento
- classificacao por categoria e bucket
- status operacional da obrigacao
- registro de pagamento quando a conta for quitada

Tambem passa a existir a diferenciacao entre tipos de recorrencia, permitindo registrar contas pontuais, contas fixas mensais e contas recorrentes variaveis.

### O que essa fase habilita

- montar o conjunto de contas e obrigacoes do mes
- registrar compromissos antes do pagamento acontecer
- acompanhar o que esta previsto, pago ou ajustado
- filtrar a operacao por competencia mensal
- preparar a base para leitura de vencimentos e comprometimento do mes

### O que ainda nao entra nesta fase

- calculo consolidado de saldo comprometido
- calculo de saldo livre para gastar
- dashboard principal do mes
- leitura automatica de proximos vencimentos em formato de resumo executivo
- reaproveitamento automatico de obrigacoes entre meses
- modelagem de cartao de credito

## Fase 1C - Resumo financeiro do mes e dashboard operacional

### Resumo de negocio

Na Fase 1C, o produto passa a entregar uma leitura operacional do mes, consolidando o setup financeiro e as obrigacoes mensais em um resumo que apoia decisao.

Em termos de negocio, essa fase faz o sistema responder de forma objetiva:

- quanto esta previsto de entrada no mes
- quanto ja saiu
- quanto ainda esta comprometido
- quanto ainda esta livre para gastar
- quanto pode ser direcionado para investimento

Tambem passa a existir uma leitura por bucket, comparando a estrutura planejada com o que ja foi pago ou comprometido no mes, alem da lista de proximos vencimentos para apoiar a operacao cotidiana.

### O que essa fase habilita

- visualizar o resumo financeiro do mes em um unico endpoint
- acompanhar saldo livre e saldo comprometido
- enxergar proximos vencimentos de forma ordenada
- comparar buckets planejados com valores pagos e comprometidos
- preparar o frontend para montar o dashboard principal

### O que ainda nao entra nesta fase

- comparacao com mes anterior
- fechamento mensal
- reaproveitamento automatico para o proximo ciclo
- cartao de credito e fatura
- investimento com posicao por ativo

## Fase 1D - Cartao de credito e fatura do ciclo

### Resumo de negocio

Na Fase 1D, o produto passa a tratar cartao de credito como um dominio proprio, em vez de reduzir tudo a despesa simples.

Em termos de negocio, essa fase habilita:

- cadastro de cartoes
- configuracao de fechamento e vencimento
- registro da fatura do ciclo
- leitura do impacto da fatura por competencia mensal

Isso aproxima o produto da realidade do caixa, porque compras no cartao deixam de ser vistas apenas como um gasto solto e passam a respeitar o ciclo de fechamento e pagamento.

### O que essa fase habilita

- cadastrar cartoes com regras operacionais reais
- registrar faturas por ciclo
- consultar faturas por mes de competencia
- preparar a leitura do impacto no mes atual e no proximo

### O que ainda nao entra nesta fase

- detalhamento de compras individuais do cartao
- parcelamento
- importacao automatica de lancamentos da fatura
- consolidacao da fatura dentro do dashboard mensal

## Fase 1E - Fechamento e revisao mensal

### Resumo de negocio

Na Fase 1E, o produto passa a permitir o fechamento do mes e a consulta de uma revisao consolidada do ciclo.

Em termos de negocio, essa fase transforma o resumo mensal em registro de aprendizagem operacional:

- o usuario pode fechar um mes
- salvar observacoes sobre aquele ciclo
- consultar depois o retrato consolidado do que foi planejado e do que aconteceu

Isso cria a base para disciplina mensal e para ajustes do modelo financeiro ao longo do tempo.

### O que essa fase habilita

- fechar o mes explicitamente
- persistir um snapshot do resumo mensal
- consultar a revisao mensal com dados consolidados e anotacoes
- preparar o terreno para comparativos e reaproveitamento no proximo ciclo

### O que ainda nao entra nesta fase

- comparativo automatico com meses anteriores
- rollover automatico para o mes seguinte
- sugestao automatica de ajuste de buckets
- fechamento assistido com alertas e recomendacoes
