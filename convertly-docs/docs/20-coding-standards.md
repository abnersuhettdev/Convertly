# 20 — Padrões de Código

## Backend

- Controllers finos.
- Services para regra de negócio.
- DTOs para requests/responses.
- Não retornar entities.
- Usar async/await.
- Usar CancellationToken.
- Usar UTC.
- Usar enums.
- Não logar segredos.
- Não expor stack trace.

## Storage

- Usar interface `IFileStorageService`.
- Não espalhar chamadas Supabase por controllers.
- Não expor Service Role Key.
- Paths sempre gerados no backend.

## Frontend

- Organizar por features.
- Usar React Query para dados de API.
- Usar Zod para validação.
- Usar Axios centralizado.
- Não acessar Supabase direto.
- Não usar `any` sem necessidade.

## Nomes

Bons exemplos:

```txt
CreateConversionRequest
SupabaseFileStorageService
ConversionJobProcessor
MonthlyUsageService
```

Evitar:

```txt
Helper
Utils
Manager
Thing
```
