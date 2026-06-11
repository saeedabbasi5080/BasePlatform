using BasePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasePlatform.Infrastructure.Persistence.Configurations;

public class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(f => f.OriginalFileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(f => f.ContentType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(f => f.StoragePath)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(f => f.StorageProvider)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired();
    }
}