using Domain.Entities.SponsorAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Sponsor;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sponsors;

internal sealed class SponsorLocalizedContentConfiguration : IEntityTypeConfiguration<SponsorLocalizedContent>
{
    public void Configure(EntityTypeBuilder<SponsorLocalizedContent> builder)
    {
        builder.Property(c => c.Title)
            .HasMaxLength(MaxTitleLength)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(MaxDescriptionLength)
            .IsRequired();

        builder.OwnsOne(s => s.Address, address =>
        {
            address.Configure();
        });

        builder.HasIndex("SponsorId", nameof(SponsorLocalizedContent.Language))
            .IsUnique()
            .HasDatabaseName("IX_SponsorLocalizedContents_SponsorId_Language");
        
        builder.HasIndex("SponsorId")
            .HasDatabaseName("IX_SponsorLocalizedContents_SponsorId");

        builder.HasIndex(s => new { s.Language, s.Title })
            .HasDatabaseName("IX_SponsorLocalizedContents_Language_Title");

        builder.HasIndex(s => s.Title)
            .HasDatabaseName("IX_SponsorLocalizedContents_Title");
    }
}
