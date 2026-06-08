# 03 — Decisões Técnicas Oficiais

## Backend

```txt
C#
.NET 8 LTS
ASP.NET Core 8 Web API
```

## Banco

```txt
Supabase PostgreSQL
Entity Framework Core 8
```

O Supabase será usado como banco PostgreSQL gerenciado.

Não usar Supabase Auth.

## Storage

```txt
Supabase Storage
```

O backend será responsável por enviar e baixar arquivos do Supabase Storage.

O frontend não deve receber Service Role Key.

## Jobs

```txt
Hangfire
```

No MVP, API e Worker rodam no mesmo serviço Koyeb.

## Conversão

```txt
LibreOffice headless
```

O container Docker do backend precisa instalar LibreOffice.

## Frontend

```txt
React + TypeScript + Vite
TailwindCSS
React Hook Form
Zod
TanStack Query
Axios
```

## Deploy

```txt
Frontend: Vercel
Backend/API + Worker: Koyeb via Docker
Database: Supabase PostgreSQL
Storage: Supabase Storage
```

## Autenticação

Auth customizada no backend:

- JWT;
- refresh token persistido;
- hash de senha.

Não usar ASP.NET Identity completo.

## Datas

Usar UTC.

Não usar `DateTime.Now`.

## IDs

Usar `Guid`.

## API

REST API.

## Pagamento

Não implementar pagamento real no MVP.

## Variáveis sensíveis

Segredos ficam apenas no backend/Koyeb:

- Supabase Service Role Key;
- JWT Secret;
- Connection String;
- Supabase Storage config.

Frontend/Vercel só recebe:

```txt
VITE_API_BASE_URL
```
