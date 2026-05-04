using Domain.Entities.SiteAggregate;
using Domain.Entities.TouristAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Tourist;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Tourists;

internal sealed class VisitedSiteConfiguration : IEntityTypeConfiguration<VisitedSite>
{
    public void Configure(EntityTypeBuilder<VisitedSite> builder)
    {
        builder.ToTable("VisitedSites");

        builder.Property(f => f.Notes)
            .HasMaxLength(MaxNotesLength)
            .IsRequired(false);
        
        builder.OwnsOne(v => v.Rating, rating =>
        {
            rating.Configure();
        });
        
        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(f => f.SiteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("TouristId", nameof(VisitedSite.SiteId))
            .IsUnique()
            .HasDatabaseName("IX_VisitedSites_TouristId_SiteId");
    }
}
