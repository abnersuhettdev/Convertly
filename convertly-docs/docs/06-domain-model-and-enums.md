# 06 — Modelo de Domínio e Enums

## Entidades

```txt
User
RefreshToken
Plan
UserSubscription
MonthlyUsage
FileAsset
ConversionJob
```

## FileAsset

Como o storage oficial é Supabase Storage, `FileAsset` deve guardar caminho/chave do objeto, não caminho local.

Campos principais:

```txt
Id
UserId
OriginalFileName
StoredFileName
StoragePath
BucketName
Extension
MimeType
SizeBytes
Kind
CreatedAt
ExpiresAt
```

## ConversionJob

Campos principais:

```txt
Id
UserId
SourceFileId
OutputFileId
SourceFormat
TargetFormat
Status
ErrorMessage
UsageReserved
CreatedAt
StartedAt
CompletedAt
ExpiresAt
```

## Enums

### ConversionStatus

```csharp
public enum ConversionStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Expired = 5
}
```

### SubscriptionStatus

```csharp
public enum SubscriptionStatus
{
    Active = 1,
    Cancelled = 2,
    Expired = 3,
    PaymentPending = 4
}
```

### FileAssetKind

```csharp
public enum FileAssetKind
{
    Original = 1,
    Converted = 2
}
```

## Constantes

### PlanSlugs

```csharp
public static class PlanSlugs
{
    public const string Free = "free";
    public const string Pro = "pro";
    public const string Business = "business";
}
```

### SupportedFormats

```csharp
public static class SupportedFormats
{
    public const string Docx = "docx";
    public const string Pdf = "pdf";
}
```

### StorageBuckets

```csharp
public static class StorageBuckets
{
    public const string Originals = "convertly-originals";
    public const string Converted = "convertly-converted";
}
```
