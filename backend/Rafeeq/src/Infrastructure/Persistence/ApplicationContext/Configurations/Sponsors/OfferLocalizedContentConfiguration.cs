using Domain.Entities.SponsorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Sponsor;

internal sealed class OfferLocalizedContentConfiguration : IEntityTypeConfiguration<OfferLocalizedContent>
{
    public void Configure(EntityTypeBuilder<OfferLocalizedContent> builder)
    {
        builder.ToTable("OfferLocalizedContents");

        builder.Property(lc => lc.Title)
            .IsRequired()
            .HasMaxLength(MaxTitleLength);

        builder.Property(lc => lc.Description)
            .HasMaxLength(MaxDescriptionLength)
            .IsRequired(true);

        builder.Property(lc => lc.TermsAndConditions)
            .HasMaxLength(MaxTermsLength)
            .IsRequired(false);

        builder.HasIndex("OfferId", nameof(SponsorLocalizedContent.Language))
            .IsUnique()
            .HasDatabaseName("IX_OfferLocalizedContents_OfferId_Language");

        builder.HasIndex(s => s.Title)
            .HasDatabaseName("IX_OfferLocalizedContents_Title");
    }
}