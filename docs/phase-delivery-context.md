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
