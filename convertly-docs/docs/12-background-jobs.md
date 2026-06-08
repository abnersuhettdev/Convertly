# 12 — Background Jobs

## Decisão

Usar Hangfire.

No MVP, API e Worker rodam juntos no mesmo serviço Koyeb.

## Fluxo

```txt
POST /conversions
  -> valida
  -> reserva uso
  -> salva original no Supabase Storage
  -> cria ConversionJob Pending
  -> enfileira Hangfire
  -> retorna conversionId
```

Worker:

```txt
ProcessAsync(conversionJobId)
  -> muda para Processing
  -> baixa original do Supabase Storage
  -> converte com LibreOffice
  -> envia PDF para Supabase Storage
  -> muda para Completed
```

## Hangfire storage

Pode usar o mesmo Supabase PostgreSQL para armazenar dados do Hangfire.

## Dashboard

```txt
/hangfire
```

Somente Development ou protegido.

## Idempotência

Antes de processar:

- se job já Completed, encerrar;
- se job não existir, encerrar;
- não consumir uso novamente;
- não devolver uso duas vezes.

## Falha

Em falha técnica:

- status Failed;
- devolver uso;
- registrar erro simplificado;
- logar detalhe técnico.
