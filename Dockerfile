FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR usr/src/app

COPY src/ .

WORKDIR usr/src/app/CashFlow.Api

RUN dotnet restore

RUN dotnet publish -c Release -o usr/src/app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR usr/src/app

COPY --from=build-env usr/src/app/out .

ENTRYPOINT [ "dotnet", "CashFlow.Api.dll" ]