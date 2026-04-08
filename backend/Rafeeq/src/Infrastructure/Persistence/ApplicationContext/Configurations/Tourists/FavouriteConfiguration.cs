using Domain.Entities.TouristAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Tourists;

public sealed class FavouriteConfiguration : IEntityTypeConfiguration<Favourite>
{
    public void Configure(EntityTypeBuilder<Favourite> builder)
    {
        builder.Property(f => f.SiteId)
            .IsRequired();

        builder.Property(f => f.Notes)
            .HasMaxLength(1000);

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.LastModifiedAt);

        builder.HasIndex("TouristId", nameof(Favourite.SiteId))
            .IsUnique()
            .HasDatabaseName("IX_Favourites_TouristId_SiteId");

        builder.Ignore(f => f.DomainEvents);
    }
}
