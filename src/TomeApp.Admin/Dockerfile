FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/TomeApp.Admin/TomeApp.Admin.csproj src/TomeApp.Admin/
RUN dotnet restore src/TomeApp.Admin/TomeApp.Admin.csproj
COPY src/TomeApp.Admin/ src/TomeApp.Admin/
RUN dotnet publish src/TomeApp.Admin/TomeApp.Admin.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TomeApp.Admin.dll"]
