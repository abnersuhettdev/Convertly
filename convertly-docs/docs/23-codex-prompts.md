# 23 — Prompts para o Codex

Use um prompt por vez.

## Prompt 1 — Setup backend/base

```txt
Você será o agente codificador do projeto Convertly.

Leia /docs antes de codar, principalmente:
00-agent-operating-rules.md
03-technical-decisions.md
19-waterfall-roadmap.md
20-coding-standards.md

Implemente apenas:
- estrutura do repositório;
- solution .NET 8;
- projetos em camadas;
- Swagger;
- CORS;
- Health Check;
- Dockerfile inicial do backend com base para instalar LibreOffice.

Não implemente auth, banco ou frontend ainda.
```

## Prompt 2 — Supabase PostgreSQL

```txt
Configure EF Core usando Supabase PostgreSQL.

Implemente:
- entidades;
- enums;
- DbContext;
- migrations;
- seeds Free, Pro e Business.

Siga 06 e 07.
```

## Prompt 3 — Auth

```txt
Implemente cadastro, login, JWT, refresh token e /auth/me.

Usuário novo deve receber assinatura Free.

Não use Supabase Auth.
```

## Prompt 4 — Planos e uso

```txt
Implemente /plans, /subscription/me, upgrade simulado e reserva de uso mensal transacional.
```

## Prompt 5 — Supabase Storage

```txt
Implemente SupabaseFileStorageService.

Use buckets privados:
- convertly-originals
- convertly-converted

Service Role Key apenas no backend.
```

## Prompt 6 — Upload e job

```txt
Implemente POST /conversions:
- validar DOCX;
- validar limite;
- reservar uso;
- salvar original no Supabase Storage;
- criar FileAsset;
- criar ConversionJob Pending.

Ainda não converta.
```

## Prompt 7 — Hangfire

```txt
Configure Hangfire e IConversionJobProcessor.
Ao criar conversão, enfileire job.
```

## Prompt 8 — Conversão

```txt
Implemente DOCX -> PDF:
- baixar original do Supabase Storage;
- converter com LibreOffice;
- salvar PDF no Supabase Storage;
- atualizar status;
- devolver uso em falha.
```

## Prompt 9 — Histórico/download

```txt
Implemente listagem paginada, detalhes e download seguro.
```

## Prompt 10 — Frontend

```txt
Crie frontend React + Vite + TypeScript + Tailwind.
Implemente auth, dashboard, conversões, polling, histórico, download e billing simulado.
```

## Prompt 11 — Deploy

```txt
Prepare deploy:
- backend Docker para Koyeb;
- envs de produção;
- CORS com URL da Vercel;
- frontend na Vercel;
- README com instruções.
```
