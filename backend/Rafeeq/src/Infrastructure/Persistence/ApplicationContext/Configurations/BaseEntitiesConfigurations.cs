using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.ApplicationContext.Configurations;

internal sealed class BaseEntityConfiguration : IEntityTypeConfiguration<BaseEntity>
{
    public void Configure(EntityTypeBuilder<BaseEntity> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.DomainEvents);
    }
}

internal sealed class BaseAuditableEntityConfiguration : IEntityTypeConfiguration<BaseAuditableEntity>
{
    public void Configure(EntityTypeBuilder<BaseAuditableEntity> builder)
    {
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.CreatedByName).HasMaxLength(256).IsRequired();

        builder.Property(x => x.LastModifiedAt).IsRequired(false);
        builder.Property(x => x.LastModifiedBy).IsRequired(false);
        builder.Property(x => x.LastModifiedByName).HasMaxLength(256).IsRequired(false);

        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.CreatedBy);
        builder.HasIndex(x => x.LastModifiedAt);
        builder.HasIndex(x => x.LastModifiedBy);
    }
}