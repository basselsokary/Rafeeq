using Domain.Entities.SponsorAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Sponsor;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sponsors;

internal sealed class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.Property(o => o.PromoCode)
            .HasMaxLength(MaxPromoCodeLength);

        builder.OwnsOne(o => o.DiscountAmount, money =>
        {
            money.Configure();
        });

        builder.OwnsOne(o => o.ValidityPeriod, period =>
        {
            period.Configure();

            period.HasIndex(p => new { p.StartDate, p.EndDate })
                .HasDatabaseName("IX_Offers_ValidityPeriod_StartDate_EndDate");
        });

        builder.HasMany(s => s.LocalizedContents)
            .WithOne()
            .HasForeignKey("OfferId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex("SponsorId", nameof(Offer.IsActive))
            .HasDatabaseName("IX_Offers_SponsorId_IsActive");

        builder.HasIndex(o => o.PromoCode)
            .HasDatabaseName("IX_Offers_PromoCode");
    }
}
