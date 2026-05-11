using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Common.Constants;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

namespace Infrastructure.Persistence.ApplicationContext.Configurations;

internal sealed class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable("StoredFiles");
        
        builder.OwnsOne(sf => sf.Hash, hash =>
        {
            hash.Configure();
            
            hash.HasIndex(x => x.Value)
                .HasDatabaseName("IX_StoredFiles_Hash");
        });

        builder.OwnsOne(sf => sf.StorageKey, key =>
        {
            key.Configure();
        });
        
        builder.OwnsOne(sf => sf.ContentType, ct =>
        {
            ct.Configure();
        });
        
        builder.Property(x => x.Size)
            .IsRequired();

        builder.Property(x => x.FirstUploadedAt)
            .IsRequired();

    }
}
