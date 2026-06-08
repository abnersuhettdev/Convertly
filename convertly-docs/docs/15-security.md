# 15 — Segurança

## Segredos

Nunca expor no frontend:

- Supabase Service Role Key;
- connection string;
- JWT secret;
- refresh tokens de outros usuários.

## Supabase

Buckets privados.

Backend faz:

- upload;
- download;
- signed URL, se usado.

## Auth

- JWT em rotas privadas;
- refresh token;
- senha hasheada.

## Ownership

Validar dono em:

- conversion_jobs;
- file_assets;
- download;
- subscription.

## CORS

Development:

```txt
http://localhost:5173
```

Production:

```txt
https://<frontend-vercel-app>.vercel.app
```

## Upload

Validar:

- extensão;
- MIME type;
- tamanho;
- arquivo vazio.

## Logs

Não logar:

- senha;
- token;
- Service Role Key;
- conteúdo do arquivo.
