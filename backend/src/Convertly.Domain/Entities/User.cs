namespace Convertly.Domain.Entities;

public sealed class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<UserSubscription> Subscriptions { get; set; } = [];
    public ICollection<MonthlyUsage> MonthlyUsages { get; set; } = [];
    public ICollection<FileAsset> FileAssets { get; set; } = [];
    public ICollection<ConversionJob> ConversionJobs { get; set; } = [];
}
