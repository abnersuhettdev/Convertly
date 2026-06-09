using Convertly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Convertly.Infrastructure.Persistence.Configurations;

public sealed class FileAssetConfiguration : IEntityTypeConfiguration<FileAsset>
{
    public void Configure(EntityTypeBuilder<FileAsset> builder)
    {
        builder.ToTable("file_assets");

        builder.HasKey(fileAsset => fileAsset.Id);

        builder.Property(fileAsset => fileAsset.Id).HasColumnName("id");
        builder.Property(fileAsset => fileAsset.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(fileAsset => fileAsset.OriginalFileName).HasColumnName("original_file_name").HasMaxLength(255).IsRequired();
        builder.Property(fileAsset => fileAsset.StoredFileName).HasColumnName("stored_file_name").HasMaxLength(255).IsRequired();
        builder.Property(fileAsset => fileAsset.StoragePath).HasColumnName("storage_path").IsRequired();
        builder.Property(fileAsset => fileAsset.BucketName).HasColumnName("bucket_name").HasMaxLength(120).IsRequired();
        builder.Property(fileAsset => fileAsset.Extension).HasColumnName("extension").HasMaxLength(20).IsRequired();
        builder.Property(fileAsset => fileAsset.MimeType).HasColumnName("mime_type").HasMaxLength(120).IsRequired();
        builder.Property(fileAsset => fileAsset.SizeBytes).HasColumnName("size_bytes").IsRequired();
        builder.Property(fileAsset => fileAsset.Kind).HasColumnName("kind").HasConversion<int>().IsRequired();
        builder.Property(fileAsset => fileAsset.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(fileAsset => fileAsset.ExpiresAt).HasColumnName("expires_at");

        builder
            .HasOne(fileAsset => fileAsset.User)
            .WithMany(user => user.FileAssets)
            .HasForeignKey(fileAsset => fileAsset.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
