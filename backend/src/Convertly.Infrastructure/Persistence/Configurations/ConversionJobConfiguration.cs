using Convertly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Convertly.Infrastructure.Persistence.Configurations;

public sealed class ConversionJobConfiguration : IEntityTypeConfiguration<ConversionJob>
{
    public void Configure(EntityTypeBuilder<ConversionJob> builder)
    {
        builder.ToTable("conversion_jobs");

        builder.HasKey(conversionJob => conversionJob.Id);

        builder.Property(conversionJob => conversionJob.Id).HasColumnName("id");
        builder.Property(conversionJob => conversionJob.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(conversionJob => conversionJob.SourceFileId).HasColumnName("source_file_id").IsRequired();
        builder.Property(conversionJob => conversionJob.OutputFileId).HasColumnName("output_file_id");
        builder.Property(conversionJob => conversionJob.SourceFormat).HasColumnName("source_format").HasMaxLength(20).IsRequired();
        builder.Property(conversionJob => conversionJob.TargetFormat).HasColumnName("target_format").HasMaxLength(20).IsRequired();
        builder.Property(conversionJob => conversionJob.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(conversionJob => conversionJob.ErrorMessage).HasColumnName("error_message");
        builder.Property(conversionJob => conversionJob.UsageReserved).HasColumnName("usage_reserved").HasDefaultValue(true).IsRequired();
        builder.Property(conversionJob => conversionJob.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(conversionJob => conversionJob.StartedAt).HasColumnName("started_at");
        builder.Property(conversionJob => conversionJob.CompletedAt).HasColumnName("completed_at");
        builder.Property(conversionJob => conversionJob.ExpiresAt).HasColumnName("expires_at");

        builder
            .HasOne(conversionJob => conversionJob.User)
            .WithMany(user => user.ConversionJobs)
            .HasForeignKey(conversionJob => conversionJob.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(conversionJob => conversionJob.SourceFile)
            .WithMany(fileAsset => fileAsset.SourceConversionJobs)
            .HasForeignKey(conversionJob => conversionJob.SourceFileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(conversionJob => conversionJob.OutputFile)
            .WithMany(fileAsset => fileAsset.OutputConversionJobs)
            .HasForeignKey(conversionJob => conversionJob.OutputFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
