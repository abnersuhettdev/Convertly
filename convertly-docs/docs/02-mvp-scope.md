# 02 — Escopo do MVP

## Objetivo

Entregar uma versão funcional e deployada do Convertly.

## Incluído

### Autenticação

- cadastro;
- login;
- JWT;
- refresh token;
- `/auth/me`;
- rotas privadas.

### Planos

- Free;
- Pro;
- Business;
- assinatura ativa;
- upgrade simulado;
- consulta de uso mensal.

### Limites

- Free: 5 conversões/mês;
- Pro: 100 conversões/mês;
- Business: 500 conversões/mês.

### Arquivos

- upload de um DOCX por vez;
- validação de extensão;
- validação de MIME type;
- validação de tamanho;
- armazenamento no Supabase Storage.

### Conversão

- DOCX para PDF;
- job em background com Hangfire;
- status da conversão;
- download seguro.

### Dashboard

- plano atual;
- uso mensal;
- histórico recente;
- botão de nova conversão.

### Deploy

- frontend na Vercel;
- backend no Koyeb;
- banco no Supabase;
- storage no Supabase Storage.

## Não incluído

- pagamento real;
- OCR;
- PDF para DOCX;
- conversão em lote;
- painel admin;
- login social;
- recuperação de senha por e-mail.

## Critérios do MVP

O MVP estará pronto quando:

- usuário cadastrar e logar;
- usuário novo receber Free;
- planos existirem no banco;
- usuário enviar DOCX;
- arquivo original for salvo no Supabase Storage;
- job for criado como Pending;
- Hangfire processar;
- LibreOffice gerar PDF;
- PDF convertido for salvo no Supabase Storage;
- usuário baixar o PDF;
- limite mensal for respeitado;
- frontend e backend estiverem deployados.
