# 16 — Tratamento de Erros

## Response de erro

```json
{
  "success": false,
  "data": null,
  "message": "Validation failed",
  "errors": ["File type is not supported"]
}
```

## Status HTTP

- 400: request inválida;
- 401: não autenticado;
- 403: sem permissão;
- 404: não encontrado;
- 409: conflito;
- 422: limite atingido ou regra de negócio;
- 500: erro inesperado.

## Erros de storage

Se upload no Supabase falhar:

- não criar job;
- devolver erro controlado;
- não reservar uso, ou reverter a transação se já reservou.

## Erros de conversão

Se LibreOffice falhar:

- status Failed;
- devolver uso;
- logar erro técnico;
- mostrar mensagem amigável.

## Frontend

Mensagens:

- sessão expirada;
- limite atingido;
- arquivo inválido;
- conversão falhou;
- erro temporário.
