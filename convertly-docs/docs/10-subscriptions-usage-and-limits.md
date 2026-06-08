# 10 — Assinaturas, Uso Mensal e Limites

## Planos

| Slug | Nome | Conversões/mês | Tamanho máximo | Retenção |
|---|---|---:|---:|---:|
| free | Free | 5 | 10 MB | 24h |
| pro | Pro | 100 | 50 MB | 168h |
| business | Business | 500 | 200 MB | 720h |

## Uso mensal

Chave:

```txt
user_id + year + month
```

## Reserva

Ao criar uma conversão:

1. abrir transação;
2. buscar plano;
3. buscar/criar monthly_usage;
4. validar limite;
5. incrementar `conversions_used`;
6. criar job com `usage_reserved = true`.

## Falha

Se o job falhar tecnicamente:

- decrementar uso;
- setar `usage_reserved = false`.

## Sucesso

Se concluir:

- manter uso consumido.

## Concorrência

A reserva deve ser transacional para não permitir ultrapassar limite.

## Upgrade simulado

Muda assinatura ativa imediatamente.

## Downgrade

Se uso atual for maior que limite novo, usuário fica bloqueado até próximo mês ou upgrade.
