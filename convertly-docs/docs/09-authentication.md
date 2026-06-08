# 09 — Autenticação

## Estratégia

Auth customizada no backend.

Não usar Supabase Auth.

## Componentes

- JWT access token;
- refresh token persistido;
- senha hasheada.

## Cadastro

Regras:

- e-mail lowercase;
- senha mínima 8 caracteres;
- e-mail único;
- criar assinatura Free.

## Access token

Expiração:

```txt
60 minutos
```

Claims:

```txt
sub
email
name
```

## Refresh token

Expiração:

```txt
7 dias
```

Regras:

- token aleatório;
- preferir salvar hash;
- rotacionar no refresh;
- revogar anterior.

## Rotas públicas

```txt
POST /auth/register
POST /auth/login
POST /auth/refresh
GET /plans
GET /conversions/supported-formats
GET /health
```

## Rotas privadas

```txt
GET /auth/me
GET /subscription/me
POST /subscription/change-plan
POST /conversions
GET /conversions
GET /conversions/{id}
GET /conversions/{id}/download
```

## Frontend

O frontend deve:

- armazenar tokens;
- enviar `Authorization: Bearer`;
- redirecionar para login em 401;
- nunca acessar Supabase diretamente com Service Role Key.
