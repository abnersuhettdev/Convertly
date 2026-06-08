# 05 — Arquitetura

## Visão geral

```txt
React/Vercel
    |
    v
ASP.NET Core API + Hangfire Worker / Koyeb Docker
    |
    +--> Supabase PostgreSQL
    |
    +--> Supabase Storage
    |
    +--> LibreOffice Headless
```

## Backend

```txt
backend/
  src/
    Convertly.Api/
    Convertly.Application/
    Convertly.Domain/
    Convertly.Infrastructure/
  tests/
    Convertly.Tests/
```

## Camadas

### Convertly.Api

- controllers;
- middleware de erro;
- JWT;
- Swagger;
- CORS;
- health check;
- Hangfire dashboard em Development.

### Convertly.Application

- services;
- DTOs;
- interfaces;
- casos de uso;
- validações.

### Convertly.Domain

- entidades;
- enums;
- constantes;
- regras centrais.

### Convertly.Infrastructure

- EF Core;
- Supabase PostgreSQL;
- Supabase Storage client;
- Hangfire;
- LibreOffice converter;
- JWT service.

## Fluxo de conversão

```txt
1. Frontend envia DOCX
2. API valida usuário/plano/limite
3. API reserva uso mensal
4. API salva original no Supabase Storage
5. API cria ConversionJob Pending
6. API enfileira Hangfire
7. Worker baixa original para diretório temporário
8. Worker converte DOCX -> PDF com LibreOffice
9. Worker envia PDF para Supabase Storage
10. Worker atualiza job para Completed
11. Frontend faz polling
12. Usuário baixa arquivo
```

## Storage

Interface oficial:

```txt
IFileStorageService
```

Implementação MVP:

```txt
SupabaseFileStorageService
```

Implementação opcional local:

```txt
LocalFileStorageService apenas para testes/dev se necessário
```

## API Response

```json
{
  "success": true,
  "data": {},
  "message": "Success",
  "errors": []
}
```

## Listagens

Usar paginação.

## Escalabilidade

A arquitetura deve permitir:

- separar worker futuramente;
- trocar Supabase Storage por S3/R2/Azure;
- adicionar novos conversores;
- adicionar pagamentos reais;
- adicionar admin dashboard.
