# 04 — Regras de Negócio

## Usuários

### BR-001 — Cadastro

Usuário cadastra com:

- nome;
- e-mail;
- senha.

### BR-002 — E-mail único

E-mail deve ser único e salvo em lowercase.

### BR-003 — Plano inicial

Todo usuário novo recebe plano Free.

### BR-004 — Autenticação

Somente usuários autenticados podem criar conversões, acessar histórico e baixar arquivos.

## Planos

| Plano | Slug | Conversões/mês | Tamanho máximo | Retenção |
|---|---|---:|---:|---:|
| Free | free | 5 | 10 MB | 24h |
| Pro | pro | 100 | 50 MB | 7 dias |
| Business | business | 500 | 200 MB | 30 dias |

## Uso mensal

### BR-005 — Uso por mês

Uso é controlado por usuário, ano e mês.

### BR-006 — Reserva

Ao criar conversão, reservar 1 uso mensal.

### BR-007 — Sucesso

Se a conversão concluir, a reserva vira uso consumido.

### BR-008 — Falha técnica

Se a conversão falhar tecnicamente, devolver a reserva.

### BR-009 — Concorrência

Validação e reserva devem ser transacionais para não ultrapassar limite.

## Arquivos

### BR-010 — Storage oficial

Arquivos originais e convertidos devem ser salvos no Supabase Storage.

### BR-011 — Bucket privado

Buckets devem ser privados.

Download deve ser feito por endpoint autenticado no backend ou por signed URL gerada pelo backend.

### BR-012 — Dono

Usuário só acessa arquivos dele.

### BR-013 — Nome físico

Usar nomes/path com UUID.

Não usar nome original como caminho final.

## Conversões

Status:

```txt
Pending
Processing
Completed
Failed
Expired
```

## Download

Download só é permitido se:

- usuário autenticado;
- usuário é dono;
- conversão está Completed;
- arquivo convertido existe;
- arquivo não expirou.

## Billing

Upgrade é simulado no MVP.

Pagamento real é futuro.
