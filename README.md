# 📚 TomeApp

Diário literário com campeonato mensal para leitores brasileiros.

## Stack

| Camada | Tecnologia |
|---|---|
| App Mobile | .NET MAUI (Android / iOS) |
| Banco local | SQLite + sqlite-net-pcl |
| API | ASP.NET Core 9 |
| Banco nuvem | PostgreSQL via Supabase |
| Fila offline→nuvem | RabbitMQ |
| Painel Admin | Blazor Server |
| Containers | Docker + Docker Compose |

## Estrutura

```
TomeApp/
├── docker-compose.yml
├── TomeApp.sln
├── scripts/
│   └── init-db.sh          # gera + aplica EF migrations
└── src/
    ├── TomeApp.API/         # ASP.NET Core – REST API + worker RabbitMQ
    ├── TomeApp.Admin/       # Blazor Server – painel administrativo
    └── TomeApp.Mobile/      # .NET MAUI – app Android/iOS
```

## Subir o backend com Docker

```bash
# 1. Copie e preencha as variáveis de ambiente
cp .env.example .env

# 2. Suba todos os serviços
docker compose up --build

# Serviços disponíveis:
#   API        → http://localhost:5000  (Swagger: /swagger)
#   Admin      → http://localhost:5001
#   RabbitMQ   → http://localhost:15672 (user: tomeapp / tomeapp_secret)
#   PostgreSQL → localhost:5432
```

> As migrations são aplicadas automaticamente pelo `Program.cs` da API na inicialização.

## Desenvolvimento local (sem Docker)

```bash
# PostgreSQL e RabbitMQ apenas
docker compose up postgres rabbitmq -d

# API
cd src/TomeApp.API
dotnet run

# Admin
cd src/TomeApp.Admin
dotnet run
```

## App Mobile

```bash
# Android (emulador)
cd src/TomeApp.Mobile
dotnet build -t:Run -f net9.0-android

# iOS (requer macOS + Xcode)
dotnet build -t:Run -f net9.0-ios
```

> No emulador Android, o endereço `10.0.2.2:5000` aponta para `localhost` da máquina host.

## Telas do App

| Tela | Descrição |
|---|---|
| **Estante** | Lista todos os livros, filtros por status, FAB para adicionar |
| **Cronômetro** | Inicia/para sessão de leitura; pop-up para registrar página final |
| **Campeonato** | Ranking BR do mês com dias restantes e minha posição |
| **Perfil** | Estatísticas, configurações e logout |

## Painel Admin

- **Dashboard** – usuários totais, lendo hoje, sessões e tempo total
- **Campeonato** – ranking mensal com botão "Marcar enviado" para o prêmio
- **Auditoria** – expande histórico de sessões de cada usuário para detectar fraudes
- **Usuários** – busca e listagem

## Endpoints da API

```
POST /api/v1/auth/register
POST /api/v1/auth/login
GET  /api/v1/books
POST /api/v1/books
PUT  /api/v1/books/{id}
DEL  /api/v1/books/{id}
GET  /api/v1/books/{id}/notes
POST /api/v1/books/{id}/notes
POST /api/v1/sync             ← bulk offline sync via RabbitMQ
GET  /api/v1/ranking
GET  /api/v1/profile
PUT  /api/v1/profile
GET  /health
```

## Fluxo de sincronização offline

```
Usuário lê sem internet
    → LocalReadingSession salva no SQLite (IsSynced = false)
    → Connectivity.Current detecta reconexão
    → SyncService.SyncPendingSessionsAsync()
    → POST /api/v1/sync  (bulk)
    → API publica no RabbitMQ (tome.sync.sessions)
    → SyncProcessorService consome, persiste no PostgreSQL
    → Atualiza ChampionshipEntry do usuário
    → IsSynced = true no SQLite local
```
