# 21 — Guia de Implementação Backend

## Estrutura

```txt
backend/
  Convertly.sln
  src/
    Convertly.Api/
    Convertly.Application/
    Convertly.Domain/
    Convertly.Infrastructure/
  tests/
    Convertly.Tests/
```

## Domain

```txt
Entities/
Enums/
Constants/
```

## Application

```txt
Auth/
Subscriptions/
Conversions/
Files/
Common/
```

Interfaces importantes:

```txt
ICurrentUserService
IDateTimeProvider
IFileStorageService
IFileConverter
IConversionJobProcessor
```

## Infrastructure

```txt
Persistence/
Auth/
Storage/SupabaseFileStorageService.cs
Conversions/DocxToPdfConverter.cs
Jobs/ConversionJobProcessor.cs
Time/
```

## Api

```txt
Controllers/
Middlewares/
Extensions/
```

Controllers:

```txt
AuthController
PlansController
SubscriptionController
ConversionsController
HealthController
```

## Supabase Storage

Criar implementação encapsulada.

O restante do app não deve saber detalhes de HTTP/API do Supabase.

## Hangfire

Registrar server no backend.

API e worker rodam juntos no Koyeb.

## Docker

Dockerfile precisa instalar LibreOffice.
