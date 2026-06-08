# 22 — Guia de Implementação Frontend

## Estrutura

```txt
frontend/
  src/
    app/
    components/
    features/
      auth/
      dashboard/
      conversions/
      billing/
      account/
    lib/
    routes/
    types/
```

## API

`src/lib/api.ts`

Responsável por:

- baseURL;
- token;
- 401;
- erros padronizados.

## Vercel

Variável:

```txt
VITE_API_BASE_URL=https://<backend-koyeb-app>.koyeb.app/api
```

## Rotas

```txt
/
 /login
 /register
 /dashboard
 /conversions
 /conversions/new
 /conversions/:id
 /billing
 /account
```

## Upload

Frontend envia arquivo para backend.

Não envia direto para Supabase no MVP.

## Download

Frontend chama backend.

Backend valida e retorna arquivo ou URL assinada.
