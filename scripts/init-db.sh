#!/bin/bash
# Gera e aplica as migrations do EF Core para a API
# Execute: bash scripts/init-db.sh

set -e

echo "==> Instalando dotnet-ef (se necessário)..."
dotnet tool install --global dotnet-ef 2>/dev/null || true
export PATH="$PATH:$HOME/.dotnet/tools"

echo "==> Gerando migrations..."
dotnet ef migrations add InitialCreate \
  --project src/TomeApp.API \
  --startup-project src/TomeApp.API \
  --output-dir Data/Migrations

echo "==> Aplicando migrations..."
dotnet ef database update \
  --project src/TomeApp.API \
  --startup-project src/TomeApp.API

echo "==> Banco de dados pronto!"
