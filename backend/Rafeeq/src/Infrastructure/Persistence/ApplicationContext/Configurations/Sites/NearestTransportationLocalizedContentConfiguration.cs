using Domain.Entities.SiteAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Site;

internal sealed class NearestTransportationLocalizedContentConfiguration : IEntityTypeConfiguration<NearestTransportationLocalizedContent>
{
    public void Configure(EntityTypeBuilder<NearestTransportationLocalizedContent> builder)
    {
        builder.ToTable("NearestTransportationLocalizedContents");

        builder.Property(lc => lc.Name)
            .IsRequired()
            .HasMaxLength(MaxNameLength);

        builder.Property(lc => lc.Description)
            .HasMaxLength(MaxDescriptionLength)
            .IsRequired(false);

        builder.OwnsOne(lc => lc.Address, address =>
        {
            address.Configure();
        });

        builder.HasIndex("TransportationId", nameof(SiteLocalizedContent.Language))
            .IsUnique()
            .HasDatabaseName("IX_NearestTransportationLocalizedContents_TransportationId_Language");

        builder.HasIndex(s => s.Name)
            .HasDatabaseName("IX_NearestTransportationLocalizedContents_Name");
    }
}