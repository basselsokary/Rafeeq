using Domain.Entities.SponsorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using static Domain.Common.Constants.DomainConstants.File;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sponsors;

internal sealed class SponsorConfiguration : IEntityTypeConfiguration<Sponsor>
{
    public void Configure(EntityTypeBuilder<Sponsor> builder)
    {
        builder.Property(s => s.MainImageUrl)
            .HasMaxLength(MaxImageUrlLength)
            .IsRequired(false);

        builder.Property(s => s.WebsiteUrl)
            .HasMaxLength(500)
            .IsRequired(false);
        
        builder.OwnsOne(s => s.ContractDate, contractDate =>
        {
            contractDate.Configure();

            contractDate.HasIndex(cd => cd.EndDate)
                .HasDatabaseName("IX_Sponsors_ContractDate_EndDate");
        });
    
        builder.OwnsOne(s => s.Location, location =>
        {
            location.Configure();

            location.HasIndex(l => new { l.Latitude, l.Longitude })
                .HasDatabaseName("IX_Sponsors_Location_Latitude_Longitude");
        });

        builder.OwnsOne(s => s.ContactPhone, phone =>
        {
            phone.Configure();
        });
        
        builder.OwnsOne(s => s.ContactEmail, email =>
        {
            email.Configure();

            email.HasIndex(e => e.Value)
                .HasDatabaseName("IX_Sponsors_ContactEmail");
        });

        builder.HasMany(s => s.Offers)
            .WithOne(o => o.Sponsor)
            .HasForeignKey(o => o.SponsorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Images)
            .WithOne()
            .HasForeignKey("SponsorId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(s => s.LocalizedContents)
            .WithOne()
            .HasForeignKey("SponsorId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.Type)
            .HasDatabaseName("IX_Sponsors_Type");

        builder.HasIndex(s => s.Tier)
            .HasDatabaseName("IX_Sponsors_Tier");
        
        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_Sponsors_Status");

        builder.Ignore(s => s.DomainEvents);
    }
}
