# 17 — Estratégia de Testes

## Backend

Usar xUnit.

Testar:

- cadastro;
- login;
- refresh;
- plano Free automático;
- reserva de uso;
- limite mensal;
- devolução de uso em falha;
- upload DOCX;
- registro FileAsset com bucket/path;
- criação de job;
- ownership;
- download privado.

## Storage

Criar testes unitários usando mock de `IFileStorageService`.

Evitar depender de Supabase real em testes unitários.

## Integração manual

Testar com Supabase real em ambiente de desenvolvimento:

1. criar buckets;
2. configurar envs;
3. subir backend;
4. enviar DOCX;
5. verificar objeto no bucket;
6. converter;
7. verificar PDF no bucket;
8. baixar pelo frontend.

## Frontend

Testar fluxo manual:

- cadastro;
- login;
- dashboard;
- upload;
- polling;
- download;
- billing simulado.
