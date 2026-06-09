using Convertly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Convertly.Infrastructure.Persistence.Configurations;

public sealed class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("plans");

        builder.HasKey(plan => plan.Id);

        builder.Property(plan => plan.Id).HasColumnName("id");
        builder.Property(plan => plan.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        builder.Property(plan => plan.Slug).HasColumnName("slug").HasMaxLength(50).IsRequired();
        builder.Property(plan => plan.MonthlyConversionLimit).HasColumnName("monthly_conversion_limit").IsRequired();
        builder.Property(plan => plan.MaxFileSizeMb).HasColumnName("max_file_size_mb").IsRequired();
        builder.Property(plan => plan.RetentionHours).HasColumnName("retention_hours").IsRequired();
        builder.Property(plan => plan.PriceCents).HasColumnName("price_cents").IsRequired();
        builder.Property(plan => plan.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
        builder.Property(plan => plan.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(plan => plan.Slug).IsUnique();
        builder.HasData(PlanSeed.Plans);
    }
}
