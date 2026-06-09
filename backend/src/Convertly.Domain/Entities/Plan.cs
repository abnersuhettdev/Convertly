namespace Convertly.Domain.Entities;

public sealed class Plan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int MonthlyConversionLimit { get; set; }
    public int MaxFileSizeMb { get; set; }
    public int RetentionHours { get; set; }
    public int PriceCents { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public ICollection<UserSubscription> Subscriptions { get; set; } = [];
}
