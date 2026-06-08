# 19 — Roadmap Waterfall Incremental

## Fase 0 — Repositório e documentação

Criar estrutura e adicionar docs.

## Fase 1 — Backend base e Dockerfile inicial

Criar solution .NET, projetos em camadas, Swagger, CORS, Health Check e Dockerfile inicial.

## Fase 2 — Supabase PostgreSQL

Configurar connection string, EF Core, entidades, migrations e seeds.

## Fase 3 — Autenticação

Cadastro, login, JWT, refresh token e `/auth/me`.

## Fase 4 — Planos e uso mensal

`/plans`, `/subscription/me`, upgrade simulado, reserva de uso.

## Fase 5 — Supabase Storage

Criar buckets, configurar envs, implementar `SupabaseFileStorageService`.

## Fase 6 — Upload e criação de job

Receber DOCX, validar, salvar original no Supabase Storage, criar FileAsset e ConversionJob Pending.

## Fase 7 — Hangfire

Configurar Hangfire, enfileirar job e processador base.

## Fase 8 — Conversão DOCX para PDF

Baixar original, converter com LibreOffice, enviar PDF para Supabase Storage, atualizar status.

## Fase 9 — Histórico e download

Listagem paginada, detalhes, ownership e download seguro.

## Fase 10 — Frontend base

React, Vite, Tailwind, rotas, Axios.

## Fase 11 — Frontend auth

Register, login, sessão e rotas protegidas.

## Fase 12 — Dashboard e conversões

Dashboard, upload, polling, histórico, download.

## Fase 13 — Billing simulado

Tela de planos e troca de plano.

## Fase 14 — Testes

Testes principais e fluxo manual.

## Fase 15 — Deploy gratuito

- Supabase PostgreSQL;
- Supabase Storage;
- backend no Koyeb via Docker;
- frontend na Vercel;
- CORS produção;
- envs produção.

## Fase 16 — Portfólio polish

README final, prints, vídeo demo, roadmap futuro.

## Fase futura — Pagamento real

Não implementar no MVP.
