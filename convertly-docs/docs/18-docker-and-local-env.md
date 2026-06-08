# 18 — Docker e Ambiente Local

## Objetivo

Rodar localmente de forma parecida com produção.

## Serviços locais

```txt
backend
frontend
```

PostgreSQL será Supabase remoto.

Opcionalmente, pode haver Postgres local para desenvolvimento rápido, mas o deploy oficial usa Supabase PostgreSQL.

## Backend Docker

O Dockerfile do backend deve instalar:

- .NET runtime/SDK conforme necessário;
- LibreOffice;
- dependências de fontes se necessário.

## Variáveis backend

```txt
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=<SUPABASE_POSTGRES_CONNECTION_STRING>
Jwt__Secret=development-secret-change-me
Jwt__Issuer=Convertly
Jwt__Audience=ConvertlyUsers
Jwt__AccessTokenMinutes=60
Jwt__RefreshTokenDays=7
Supabase__Url=<SUPABASE_URL>
Supabase__ServiceRoleKey=<SUPABASE_SERVICE_ROLE_KEY>
Supabase__OriginalsBucket=convertly-originals
Supabase__ConvertedBucket=convertly-converted
Frontend__BaseUrl=http://localhost:5173
```

## Variáveis frontend

```txt
VITE_API_BASE_URL=http://localhost:5000/api
```

## Docker Compose local

Pode subir:

- backend;
- frontend.

PostgreSQL local é opcional.

## Comandos

Backend:

```bash
dotnet restore
dotnet build
dotnet run --project backend/src/Convertly.Api
```

Frontend:

```bash
cd frontend
npm install
npm run dev
```

Migrations:

```bash
dotnet ef database update --project backend/src/Convertly.Infrastructure --startup-project backend/src/Convertly.Api
```
