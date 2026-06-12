FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/TomeApp.Web/TomeApp.Web.csproj src/TomeApp.Web/
RUN dotnet restore src/TomeApp.Web/TomeApp.Web.csproj

COPY src/TomeApp.Web/ src/TomeApp.Web/
RUN dotnet publish src/TomeApp.Web/TomeApp.Web.csproj -c Release -o /app/publish

FROM nginx:alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY src/TomeApp.Web/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
