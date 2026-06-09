using Convertly.Domain.Enums;

namespace Convertly.Domain.Entities;

public sealed class FileAsset
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public FileAssetKind Kind { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<ConversionJob> SourceConversionJobs { get; set; } = [];
    public ICollection<ConversionJob> OutputConversionJobs { get; set; } = [];
}
