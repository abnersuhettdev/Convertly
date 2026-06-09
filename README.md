# Convertly

Convertly e um SaaS freemium de conversao de arquivos, criado como projeto full stack para portfolio.

O produto permite que usuarios criem conta, enviem documentos e convertam arquivos entre formatos suportados, respeitando limites mensais por plano.

## Stack oficial

- Frontend: React, TypeScript, Vite, TailwindCSS
- Backend: C#, .NET 8 LTS, ASP.NET Core Web API
- Banco: Supabase PostgreSQL
- Storage: Supabase Storage
- Jobs: Hangfire rodando junto com a API no mesmo servico
- Deploy frontend: Vercel
- Deploy backend/API + Worker: Render com Docker
- Auth: JWT + Refresh Token
- Conversao inicial: DOCX para PDF com LibreOffice headless

## Objetivo

Construir um produto com cara de SaaS real, demonstrando:

- arquitetura em camadas;
- autenticacao segura;
- controle de planos e limites;
- upload e download privado de arquivos;
- processamento assincrono;
- historico de conversoes;
- integracao frontend/backend;
- Supabase PostgreSQL;
- Supabase Storage;
- Docker;
- deploy gratuito;
- documentacao tecnica.

## Principio do projeto

Este projeto segue o padrao interno:

**Beleza, Forca e Fe**

- **Beleza:** interface limpa, codigo organizado e experiencia agradavel.
- **Forca:** backend robusto, validacoes reais, seguranca, jobs e testes.
- **Fe:** arquitetura preparada para crescimento, novos formatos, planos pagos reais e infraestrutura melhor no futuro.

## Modelo de implementacao

O projeto deve ser implementado em modelo **waterfall incremental**.

Cada fase deve ser concluida, testada e validada antes da proxima comecar.

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

## Deploy oficial do portfolio

```txt
Frontend: Vercel
Backend/API + Worker: Render via Docker
Database: Supabase PostgreSQL
Storage: Supabase Storage
```

### Deploy gratuito

O backend deve ser publicado no Render a partir de `backend/Dockerfile`. A imagem usa `ASPNETCORE_URLS=http://+:8080` como fallback e respeita a variavel `PORT` quando o Render fornece essa configuracao. A API e o worker Hangfire rodam juntos no mesmo container, com LibreOffice instalado para DOCX -> PDF.

O frontend deve ser publicado na Vercel com root directory `frontend`, build command `npm run build`, output directory `dist` e apenas a variavel `VITE_API_BASE_URL=https://<backend-render-app>.onrender.com/api`.

Variaveis sensiveis como `Supabase__ServiceRoleKey`, `Jwt__Secret` e `ConnectionStrings__DefaultConnection` ficam somente no backend/Render. Veja o guia completo em `docs/28-deployment-guide.md`.

## Importante

Pagamento real nao faz parte do MVP.

O MVP tera:

- plano Free real;
- limite real de 5 conversoes por mes;
- planos Pro e Business no banco;
- upgrade simulado;
- arquitetura preparada para pagamento real futuro.
