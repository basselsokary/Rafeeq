using Domain.Entities.SiteAggregate;
using Domain.Entities.TouristAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Tourist;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Tourists;

internal sealed class FavouriteSiteConfiguration : IEntityTypeConfiguration<FavouriteSite>
{
    public void Configure(EntityTypeBuilder<FavouriteSite> builder)
    {
        builder.ToTable("Favourites");

        builder.Property(f => f.SiteId)
            .IsRequired();

        builder.Property(f => f.Notes)
            .HasMaxLength(MaxNotesLength)
            .IsRequired(false);
        
        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(f => f.SiteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("TouristId", nameof(FavouriteSite.SiteId))
            .IsUnique()
            .HasDatabaseName("IX_Favourites_TouristId_SiteId");
    }
}
