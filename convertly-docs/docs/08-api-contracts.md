# 08 — Contratos da API

Base local:

```txt
http://localhost:5000/api
```

Base produção:

```txt
https://<backend-koyeb-app>.koyeb.app/api
```

## Response padrão

```json
{
  "success": true,
  "data": {},
  "message": "Success",
  "errors": []
}
```

## Auth

### POST /auth/register

```json
{
  "name": "Abner Suhett",
  "email": "abner@email.com",
  "password": "StrongPassword123!"
}
```

### POST /auth/login

```json
{
  "email": "abner@email.com",
  "password": "StrongPassword123!"
}
```

Retorna:

```json
{
  "accessToken": "jwt",
  "refreshToken": "refresh-token",
  "expiresIn": 3600,
  "user": {
    "id": "uuid",
    "name": "Abner Suhett",
    "email": "abner@email.com"
  }
}
```

### POST /auth/refresh

```json
{
  "refreshToken": "refresh-token"
}
```

### GET /auth/me

Privado.

## Plans

### GET /plans

Público.

## Subscription

### GET /subscription/me

Privado.

### POST /subscription/change-plan

Privado.

```json
{
  "planSlug": "pro"
}
```

## Supported Formats

### GET /conversions/supported-formats

Público.

```json
[
  {
    "sourceFormat": "docx",
    "targetFormats": ["pdf"]
  }
]
```

## Conversions

### POST /conversions

Privado.

`multipart/form-data`

Campos:

```txt
file
targetFormat=pdf
```

Retorna:

```json
{
  "conversionId": "uuid",
  "status": "Pending"
}
```

### GET /conversions

Privado.

Query:

```txt
page=1
pageSize=10
status=Completed opcional
```

### GET /conversions/{id}

Privado.

### GET /conversions/{id}/download

Privado.

Estratégia recomendada:

- backend valida dono e status;
- backend gera signed URL curto do Supabase Storage;
- backend redireciona ou retorna URL temporária.

Preferência do MVP:

```txt
Backend retorna stream do arquivo para manter controle total.
```

Alternativa aceita:

```txt
Backend retorna signed URL com expiração curta.
```

## Health

### GET /health

Público.

## Hangfire

Dashboard:

```txt
/hangfire
```

Somente Development ou protegido.
