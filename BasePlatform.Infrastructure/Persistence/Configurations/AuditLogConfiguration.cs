using BasePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BasePlatform.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.ActorEmail)
            .HasMaxLength(256);

        builder.Property(a => a.Action)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.TargetEntityType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.TargetEntityId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Details)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.HasIndex(a => a.ActorId);
        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => a.Action);
    }
}