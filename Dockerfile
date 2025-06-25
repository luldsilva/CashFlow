FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /usr/src/app

# Copia o conteúdo da pasta src
COPY src/ ./src/

# Entra no diretório correto
WORKDIR /usr/src/app/src/CashFlow.Api

# Restaura os pacotes
RUN dotnet restore

# Publica a aplicação
RUN dotnet publish -c Release -o /usr/src/app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /usr/src/app

COPY --from=build-env /usr/src/app/out .

ENTRYPOINT ["dotnet", "CashFlow.Api.dll"]