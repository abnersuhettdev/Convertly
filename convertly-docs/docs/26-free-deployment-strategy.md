# 26 — Estratégia de Deploy Gratuito

## Decisão oficial

O deploy gratuito oficial será:

```txt
Frontend: Vercel
Backend/API + Worker: Koyeb via Docker
Database: Supabase PostgreSQL
Storage: Supabase Storage
```

## Por que essa escolha

### Vercel

Boa opção para React/Vite, com deploy simples via GitHub e variável `VITE_API_BASE_URL`.

### Koyeb

Boa opção para backend containerizado, permitindo Dockerfile com LibreOffice instalado.

### Supabase PostgreSQL

Banco PostgreSQL gerenciado, útil para o backend .NET via connection string.

### Supabase Storage

Storage persistente para arquivos originais e convertidos, evitando depender do disco temporário do container.

## Arquitetura de produção

```txt
User
 |
 v
Vercel Frontend
 |
 v
Koyeb Backend/API + Worker
 |          |
 |          v
 |      LibreOffice
 |
 +--> Supabase PostgreSQL
 |
 +--> Supabase Storage
```

## API + Worker juntos

No MVP, a API e o Worker ficam no mesmo serviço Koyeb.

Motivo:

- reduz complexidade;
- reduz custo;
- simplifica deploy;
- suficiente para portfólio.

Futuro:

```txt
convertly-api
convertly-worker
```

## Variáveis no Koyeb

```txt
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<SUPABASE_POSTGRES_CONNECTION_STRING>
Jwt__Secret=<strong-secret>
Jwt__Issuer=Convertly
Jwt__Audience=ConvertlyUsers
Jwt__AccessTokenMinutes=60
Jwt__RefreshTokenDays=7
Supabase__Url=<SUPABASE_URL>
Supabase__ServiceRoleKey=<SUPABASE_SERVICE_ROLE_KEY>
Supabase__OriginalsBucket=convertly-originals
Supabase__ConvertedBucket=convertly-converted
Frontend__BaseUrl=https://<frontend>.vercel.app
```

## Variáveis na Vercel

```txt
VITE_API_BASE_URL=https://<backend>.koyeb.app/api
```

## Supabase setup

Criar:

```txt
Project
Database
Buckets privados:
  convertly-originals
  convertly-converted
```

## Dockerfile backend

Precisa:

- publicar app .NET;
- instalar LibreOffice;
- expor porta usada pelo Koyeb;
- iniciar API e Hangfire Server juntos.

## CORS produção

Permitir apenas:

```txt
https://<frontend>.vercel.app
```

Em desenvolvimento:

```txt
http://localhost:5173
```

## Limitações do free tier

Documentar no README:

- serviços gratuitos podem ter limites;
- cold start pode acontecer;
- conversões grandes podem falhar por limite de recurso;
- projeto é demonstração de portfólio, não ambiente de produção comercial.

## Checklist de deploy

```txt
[ ] Criar projeto Supabase
[ ] Copiar connection string
[ ] Criar buckets privados
[ ] Configurar envs no Koyeb
[ ] Deploy backend via Docker
[ ] Testar /health
[ ] Configurar env Vercel
[ ] Deploy frontend
[ ] Configurar CORS
[ ] Testar cadastro online
[ ] Testar conversão online
[ ] Testar download online
```
