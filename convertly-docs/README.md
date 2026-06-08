# Convertly

Convertly é um SaaS freemium de conversão de arquivos, criado como projeto full stack para portfólio.

O produto permite que usuários criem conta, enviem documentos e convertam arquivos entre formatos suportados, respeitando limites mensais por plano.

## Stack oficial

- Frontend: React, TypeScript, Vite, TailwindCSS
- Backend: C#, .NET 8 LTS, ASP.NET Core Web API
- Banco: Supabase PostgreSQL
- Storage: Supabase Storage
- Jobs: Hangfire rodando junto com a API no mesmo serviço
- Deploy frontend: Vercel
- Deploy backend/API + Worker: Koyeb com Docker
- Auth: JWT + Refresh Token
- Conversão inicial: DOCX para PDF com LibreOffice headless

## Objetivo

Construir um produto com cara de SaaS real, demonstrando:

- arquitetura em camadas;
- autenticação segura;
- controle de planos e limites;
- upload e download privado de arquivos;
- processamento assíncrono;
- histórico de conversões;
- integração frontend/backend;
- Supabase PostgreSQL;
- Supabase Storage;
- Docker;
- deploy gratuito;
- documentação técnica.

## Princípio do projeto

Este projeto segue o padrão interno:

**Beleza, Força e Fé**

- **Beleza:** interface limpa, código organizado e experiência agradável.
- **Força:** backend robusto, validações reais, segurança, jobs e testes.
- **Fé:** arquitetura preparada para crescimento, novos formatos, planos pagos reais e infraestrutura melhor no futuro.

## Modelo de implementação

O projeto deve ser implementado em modelo **waterfall incremental**.

Cada fase deve ser concluída, testada e validada antes da próxima começar.

## Estrutura final esperada

```txt
convertly/
  docs/
  backend/
    src/
      Convertly.Api/
      Convertly.Application/
      Convertly.Domain/
      Convertly.Infrastructure/
    tests/
      Convertly.Tests/
  frontend/
  docker-compose.yml
  README.md
```

## Deploy oficial do portfólio

```txt
Frontend: Vercel
Backend/API + Worker: Koyeb via Docker
Database: Supabase PostgreSQL
Storage: Supabase Storage
```

## Importante

Pagamento real não faz parte do MVP.

O MVP terá:

- plano Free real;
- limite real de 5 conversões por mês;
- planos Pro e Business no banco;
- upgrade simulado;
- arquitetura preparada para pagamento real futuro.
