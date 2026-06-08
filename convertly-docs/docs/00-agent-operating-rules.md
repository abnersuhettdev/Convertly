# 00 — Regras Operacionais para o Agente Codificador

A documentação é a fonte oficial da verdade.

O agente codificador deve implementar o projeto Convertly seguindo as fases definidas em `19-waterfall-roadmap.md`.

## Regra principal

Não implementar tudo de uma vez.

Implementar uma fase por vez.

Para cada fase:

1. ler os documentos relacionados;
2. identificar arquivos que serão criados/alterados;
3. implementar somente o escopo da fase;
4. rodar build/testes;
5. corrigir erros;
6. validar critérios;
7. avançar apenas após a fase estar estável.

## Ordem obrigatória

1. Repositório e documentação
2. Docker local inicial
3. Backend base
4. Banco Supabase PostgreSQL e migrations
5. Autenticação
6. Planos, assinatura e uso mensal
7. Supabase Storage
8. Upload e registro de arquivos
9. Hangfire e jobs
10. Conversão DOCX para PDF
11. Download e histórico
12. Frontend base
13. Frontend auth
14. Dashboard e conversões
15. Billing simulado
16. Testes
17. Deploy gratuito
18. README final e portfólio

## Tecnologias oficiais

Backend:

- .NET 8 LTS
- ASP.NET Core Web API
- Entity Framework Core 8
- Supabase PostgreSQL
- Supabase Storage
- JWT Bearer Authentication
- Refresh Token persistido
- Hangfire
- LibreOffice headless
- xUnit
- Docker

Frontend:

- React
- TypeScript
- Vite
- TailwindCSS
- React Hook Form
- Zod
- TanStack Query
- Axios
- Vercel

Deploy:

- Vercel para frontend
- Koyeb para backend/API + Worker via Docker
- Supabase PostgreSQL para banco
- Supabase Storage para arquivos

## Decisões travadas

- Usar .NET 8 LTS.
- Usar Supabase PostgreSQL como banco oficial do deploy.
- Usar Supabase Storage como storage oficial.
- Usar Koyeb com Docker para backend/API + Worker.
- Usar Vercel para frontend.
- Usar Hangfire no backend.
- Rodar API e Worker no mesmo serviço Koyeb no MVP.
- Usar DOCX -> PDF como conversão inicial.
- Usar upgrade simulado no MVP.
- Não implementar pagamento real no MVP.
- Não retornar entities diretamente na API.
- Não fazer conversão diretamente no controller.
- Não salvar arquivos em disco como storage final.
- Não expor Service Role Key do Supabase no frontend.

## O que não fazer

Não fazer:

- não implementar pagamento real;
- não criar PDF -> DOCX no MVP;
- não usar Supabase Auth;
- não colocar segredo do Supabase no frontend;
- não salvar arquivos permanentemente no disco do container;
- não confiar em validação do frontend;
- não permitir download sem validar dono;
- não deixar API e frontend com CORS aberto para qualquer origem em produção;
- não alterar contratos da API sem atualizar a documentação.

## Princípio Beleza, Força e Fé

### Beleza

- UI clara;
- código organizado;
- nomes consistentes;
- README caprichado.

### Força

- regras no backend;
- validações completas;
- uso mensal transacional;
- jobs controlados;
- storage externo;
- logs e testes.

### Fé

- arquitetura preparada para crescer;
- storage abstraído;
- conversores abstraídos;
- pagamento futuro isolado;
- deploy gratuito bem documentado.
