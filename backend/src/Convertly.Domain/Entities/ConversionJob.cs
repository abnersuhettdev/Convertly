using Convertly.Domain.Enums;

namespace Convertly.Domain.Entities;

public sealed class ConversionJob
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SourceFileId { get; set; }
    public Guid? OutputFileId { get; set; }
    public string SourceFormat { get; set; } = string.Empty;
    public string TargetFormat { get; set; } = string.Empty;
    public ConversionStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public bool UsageReserved { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public User User { get; set; } = null!;
    public FileAsset SourceFile { get; set; } = null!;
    public FileAsset? OutputFile { get; set; }
}
