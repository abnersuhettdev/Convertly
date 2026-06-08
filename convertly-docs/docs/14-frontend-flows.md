# 14 — Fluxos do Frontend

## Stack

- React;
- TypeScript;
- Vite;
- TailwindCSS;
- React Hook Form;
- Zod;
- TanStack Query;
- Axios.

## Deploy

Frontend será deployado na Vercel.

Variável:

```txt
VITE_API_BASE_URL=https://<backend-koyeb-app>.koyeb.app/api
```

## Páginas

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

## Nova Conversão

Fluxo:

1. carregar formatos suportados;
2. buscar plano/limite;
3. aceitar apenas `.docx`;
4. validar tamanho;
5. enviar para backend;
6. redirecionar para detalhes;
7. fazer polling.

## Polling

Enquanto status for:

```txt
Pending
Processing
```

Chamar:

```txt
GET /conversions/{id}
```

A cada 2 ou 3 segundos.

Parar quando:

```txt
Completed
Failed
Expired
```

## Download

Frontend chama backend:

```txt
GET /conversions/{id}/download
```

O backend decide se retorna stream ou signed URL.

## Segurança frontend

Não usar Supabase client com Service Role Key.

Frontend só conversa com a API.
