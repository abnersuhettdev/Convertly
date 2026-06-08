# 11 — Supabase Storage

## Decisão oficial

O storage oficial do MVP será Supabase Storage.

Não usar storage local como solução final de deploy.

## Buckets

Criar dois buckets privados:

```txt
convertly-originals
convertly-converted
```

## Acesso

Apenas o backend deve acessar buckets usando credencial segura.

O frontend não deve receber Service Role Key.

## Configuração backend

Variáveis:

```txt
Supabase__Url
Supabase__ServiceRoleKey
Supabase__OriginalsBucket=convertly-originals
Supabase__ConvertedBucket=convertly-converted
```

## Interface

```csharp
public interface IFileStorageService
{
    Task<FileStorageResult> SaveOriginalAsync(Stream file, string originalFileName, string contentType, CancellationToken cancellationToken);
    Task<FileStorageResult> SaveConvertedAsync(Stream file, string fileName, string contentType, CancellationToken cancellationToken);
    Task<Stream> GetAsync(string bucketName, string storagePath, CancellationToken cancellationToken);
    Task DeleteAsync(string bucketName, string storagePath, CancellationToken cancellationToken);
    Task<string> CreateSignedDownloadUrlAsync(string bucketName, string storagePath, TimeSpan expiresIn, CancellationToken cancellationToken);
}
```

## Implementação

```txt
SupabaseFileStorageService
```

## Paths

Usar paths previsíveis e seguros:

```txt
users/{userId}/originals/{conversionId}/{fileId}.docx
users/{userId}/converted/{conversionId}/{fileId}.pdf
```

## Registro no banco

Salvar em `file_assets`:

```txt
bucket_name
storage_path
stored_file_name
original_file_name
mime_type
size_bytes
extension
kind
expires_at
```

## Upload

O backend recebe upload do frontend e envia para Supabase Storage.

O frontend não faz upload direto no Supabase no MVP.

## Download

Estratégia MVP recomendada:

1. frontend chama backend;
2. backend valida dono/status/expiração;
3. backend baixa arquivo do Supabase;
4. backend retorna stream.

Alternativa:

1. backend valida;
2. backend gera signed URL curta;
3. frontend baixa pela signed URL.

## Segurança

- buckets privados;
- Service Role Key apenas no backend;
- paths com userId e UUID;
- validar ownership pelo banco;
- nunca confiar no path vindo do frontend.

## Desenvolvimento local

Pode usar Supabase remoto também no ambiente local.

Opcionalmente, criar LocalFileStorageService apenas para testes, mas não como implementação oficial.
