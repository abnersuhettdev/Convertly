using Convertly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Convertly.Infrastructure.Persistence.Configurations;

public sealed class MonthlyUsageConfiguration : IEntityTypeConfiguration<MonthlyUsage>
{
    public void Configure(EntityTypeBuilder<MonthlyUsage> builder)
    {
        builder.ToTable("monthly_usages");

        builder.HasKey(monthlyUsage => monthlyUsage.Id);

        builder.Property(monthlyUsage => monthlyUsage.Id).HasColumnName("id");
        builder.Property(monthlyUsage => monthlyUsage.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(monthlyUsage => monthlyUsage.Year).HasColumnName("year").IsRequired();
        builder.Property(monthlyUsage => monthlyUsage.Month).HasColumnName("month").IsRequired();
        builder.Property(monthlyUsage => monthlyUsage.ConversionsUsed).HasColumnName("conversions_used").HasDefaultValue(0).IsRequired();
        builder.Property(monthlyUsage => monthlyUsage.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(monthlyUsage => monthlyUsage.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(monthlyUsage => new { monthlyUsage.UserId, monthlyUsage.Year, monthlyUsage.Month }).IsUnique();

        builder
            .HasOne(monthlyUsage => monthlyUsage.User)
            .WithMany(user => user.MonthlyUsages)
            .HasForeignKey(monthlyUsage => monthlyUsage.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
