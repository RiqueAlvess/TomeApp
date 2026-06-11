FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/TomeApp.API/TomeApp.API.csproj src/TomeApp.API/
RUN dotnet restore src/TomeApp.API/TomeApp.API.csproj

COPY src/TomeApp.API/ src/TomeApp.API/

# Instala o dotnet-ef e gera a migration inicial se ainda não existir
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN ls src/TomeApp.API/Data/Migrations/*.cs 2>/dev/null \
    || dotnet ef migrations add InitialCreate \
        --project src/TomeApp.API \
        --startup-project src/TomeApp.API \
        --output-dir Data/Migrations

RUN dotnet publish src/TomeApp.API/TomeApp.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TomeApp.API.dll"]
