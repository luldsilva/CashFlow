## Sobre o projeto

Esta **API** desenvolvida utilizando **.NET 8**, adota os princípios do **Domain-Driven Design (DDD)** para oferecer uma solução estruturada e eficaz no gerenciamento de despesas pessoais. O principal objetivo é permitir que os usuários registrem suas despesas, detalhando informações como título, data e hora, desccrição, valor e tipo de pagamento, com os dados sendo armazenados em um banco de dados **MySQL**.

A arquitetura da **API** baseia-se em **REST**, utilizando métodos **HTTP** padrão para uma comunicação eficiente e simplificada. Além disso, é complementada por uma documentação **Swagger**, que proporciona uma interface fráfica interativa para que os desenvolvedores possam explorar e testar os endpoints de maneira fácil.

Dentre os pacotes NuGet utilizados, o **AutoMapper** é o responsável pelo mapeamento entre objetos de domínio e requisição/resposta, reduzindo a necessidade de código repetitivo e manual. O **FluentAssertions** é utilizado nos testes de unidade para tornar as verificações mais legíveis, ajudando a escrever testes claros e compreensíveis. Para as validações o **FluentValidation** é usado para implementar regras de validação de forma simples e intuitiva nas classes de requisições, mantendo o código limpo e fácil de manter. Por fim, o **EntityFramework** atua como um **ORM (Object-Relational Mapper)** que simplifica as interações com o banco de dados, permitindo o uso de objetos .NET para manipular dados diretamente, sem a necessidade de lidar com consultas SQL.

### Features
- **Domain-Driven Design (DDD):** Estrutura modular que facilita o entendimento e a manutenção do domínio da aplicação.
- **Testes de unidade e de integração:** Testes abrangentes com FluentAssertions para garantir a funcionalidade e a qualidade.
- **Geração de Relatório:** Capacidade de exportar relatórios detalhados para **PDF** e **Excel**, oferecendo uma análise visual e eficaz das despesas.
- **RESTful API com Documentaçao Swagger:** Interface documentada que facilita a integração e o teste por parte dos desenvolvedores.

### Construído com
![.NET Badge](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=fff&style=for-the-badge)
![Windows](https://img.shields.io/badge/Windows-0078D4?logo=windows&logoColor=fff&style=for-the-badge)
![Visual Studio Code](https://img.shields.io/badge/Visual%20Studio-5C2D91?logo=visualstudio&logoColor=fff&style=for-the-badge)
![MySQL Badge](https://img.shields.io/badge/MySQL-4479A1?logo=mysql&logoColor=fff&style=for-the-badge)
![Swagger Badge](https://img.shields.io/badge/Swagger-85EA2D?logo=swagger&logoColor=000&style=for-the-badge)

## Getting Started

Para obter uma cópia local funcionando, siga estes passos simples.

### Requisitos
- Visual Studio versão 2022+ ou Visual Studio Code
- Windows 10+ ou Linux/MacOS com [.NET SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0) instalado
- Docker e Docker Compose

### Instalação

1. Clone o repositório:
   
    ```sh
    git clone https://github.com/luldsilva/CashFlow.git
    ```
3. Copie `.env.example` para `.env`.
4. Suba a infraestrutura local com `docker compose up -d`.
5. Execute a API.

### Infraestrutura local

O projeto adota neste momento a seguinte base local:

- `MySQL` para persistencia principal
- `Redis` para apoio operacional e cache
- `MinIO` para anexos em storage de objetos

A trilha futura de IaC foi preparada na pasta [`infra/terraform`](/home/lucaslisilva/projetos/CashFlow/infra/terraform/README.md), mas o provisionamento cloud permanece adiado ate existir um alvo real de deploy.

### Compose

O `compose.yaml` reintroduz a API junto da infraestrutura base. A aplicacao recebe via variaveis de ambiente:

- conexao com MySQL
- configuracao de JWT
- configuracao inicial de storage compativel com `MinIO` e futura migracao para `S3`
- configuracao basica de Redis


