using Convertly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Convertly.Infrastructure.Persistence;

public sealed class ConvertlyDbContext(DbContextOptions<ConvertlyDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<MonthlyUsage> MonthlyUsages => Set<MonthlyUsage>();
    public DbSet<FileAsset> FileAssets => Set<FileAsset>();
    public DbSet<ConversionJob> ConversionJobs => Set<ConversionJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConvertlyDbContext).Assembly);
    }
}
