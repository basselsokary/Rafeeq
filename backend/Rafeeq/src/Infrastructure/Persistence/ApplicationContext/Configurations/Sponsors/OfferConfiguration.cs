using Domain.Entities.SponsorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sponsors;

public sealed class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.Property(o => o.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(o => o.DiscountPercentage);

        builder.Property(o => o.TermsAndConditions)
            .HasMaxLength(2000);

        builder.Property(o => o.PromoCode)
            .HasMaxLength(50);

        builder.Property(o => o.IsActive)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(o => o.RedemptionCount)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(o => o.MaxRedemptions);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.LastModifiedAt);

        builder.OwnsOne(o => o.DiscountAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("DiscountAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("DiscountCurrency")
                .HasMaxLength(3);
        });

        builder.Navigation(o => o.DiscountAmount).IsRequired(false);

        builder.OwnsOne(o => o.ValidityPeriod, period =>
        {
            period.Property(p => p.StartDate)
                .HasColumnName("ValidityStartDate")
                .IsRequired();

            period.Property(p => p.EndDate)
                .HasColumnName("ValidityEndDate")
                .IsRequired();

            period.HasIndex(p => new { p.StartDate, p.EndDate })
                .HasDatabaseName("IX_Offers_SponsorId_Validity");
        });

        builder.HasOne(o => o.Sponsor)
            .WithMany(s => s.Offers)
            .HasForeignKey("SponsorId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("SponsorId", nameof(Offer.IsActive))
            .HasDatabaseName("IX_Offers_SponsorId_IsActive");

        builder.HasIndex(o => o.PromoCode)
            .HasDatabaseName("IX_Offers_PromoCode");

        builder.Ignore(o => o.DomainEvents);
    }
}
