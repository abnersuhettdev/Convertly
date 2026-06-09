using Convertly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Convertly.Infrastructure.Persistence.Configurations;

public sealed class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("user_subscriptions");

        builder.HasKey(subscription => subscription.Id);

        builder.Property(subscription => subscription.Id).HasColumnName("id");
        builder.Property(subscription => subscription.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(subscription => subscription.PlanId).HasColumnName("plan_id").IsRequired();
        builder.Property(subscription => subscription.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(subscription => subscription.StartedAt).HasColumnName("started_at").IsRequired();
        builder.Property(subscription => subscription.EndsAt).HasColumnName("ends_at");
        builder.Property(subscription => subscription.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(subscription => subscription.UpdatedAt).HasColumnName("updated_at");

        builder
            .HasOne(subscription => subscription.User)
            .WithMany(user => user.Subscriptions)
            .HasForeignKey(subscription => subscription.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(subscription => subscription.Plan)
            .WithMany(plan => plan.Subscriptions)
            .HasForeignKey(subscription => subscription.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
