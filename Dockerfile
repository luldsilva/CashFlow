FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY CashFlow.sln ./
COPY src/ ./src/

WORKDIR /src/src/CashFlow.Api

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "CashFlow.Api.dll"]
