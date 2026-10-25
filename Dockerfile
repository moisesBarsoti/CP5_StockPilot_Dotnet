
# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY global.json ./

COPY StockPilot.Domain/*.csproj StockPilot.Domain/
COPY StockPilot.Application/*.csproj StockPilot.Application/
COPY StockPilot.Infrastructure/*.csproj StockPilot.Infrastructure/
COPY StockPilot.API/*.csproj StockPilot.API/

RUN dotnet restore StockPilot.API/StockPilot.API.csproj

COPY . .

RUN dotnet publish StockPilot.API/StockPilot.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:10000

EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "StockPilot.API.dll"]
