using Domain.Entities.SiteAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Site;
using static Domain.Common.Constants.DomainConstants.File;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Domain.ValueObjects;
using System.Text.Json;
using Domain.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sites;

internal sealed class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.Property(s => s.CityId)
            .IsRequired();

        builder.Property(s => s.ContactPhone)
            .HasMaxLength(MaxContactPhoneLength)
            .IsRequired(false);

        builder.Property(s => s.WebsiteUrl)
            .HasMaxLength(MaxWebsiteUrlLength)
            .IsRequired(false);

        builder.Property(s => s.MainImageUrl)
            .HasMaxLength(MaxImageUrlLength)
            .IsRequired(false);

        builder.OwnsOne(s => s.Location, location =>
        {
            location.Configure();

            location.HasIndex(l => new { l.Latitude, l.Longitude })
                .HasDatabaseName("IX_Sites_Location_Latitude_Longitude");
        });

        builder.OwnsOne(s => s.EntryTicket, ticket =>
        {
            ticket.Configure();
        });

        builder.OwnsMany(s => s.OpeningHours, openingHours =>
        {
            openingHours.Configure("Site");

            openingHours.HasIndex("SiteId", nameof(OpeningHour.Day))
                .IsUnique()
                .HasDatabaseName("IX_Sites_OpeningHours_SiteId_DayOfWeek");
            
            openingHours.HasIndex("SiteId")
                .HasDatabaseName("IX_Sites_OpeningHours_SiteId");
        });

        builder.HasMany(s => s.Images)
            .WithOne()
            .HasForeignKey("SiteId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.LocalizedContents)
            .WithOne()
            .HasForeignKey("SiteId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property<List<FacilityType>>("_facilities")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<FacilityType>>(v, (JsonSerializerOptions)null!)!)
            .HasColumnName("Facilities")
            .HasColumnType("nvarchar(1024)")
            .Metadata.SetValueComparer(new ValueComparer<List<FacilityType>>(
                (c1, c2) => c1!.OrderBy(x => x).SequenceEqual(c2!.OrderBy(x => x)),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));

        builder.HasMany(s => s.NearestTransportations)
            .WithOne()
            .HasForeignKey(nt => nt.SiteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.CityId)
            .HasDatabaseName("IX_Sites_CityId");

        builder.HasIndex(s => s.Type)
            .HasDatabaseName("IX_Sites_Type");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_Sites_Status");

        builder.HasIndex(s => s.AverageRating)
            .HasDatabaseName("IX_Sites_AverageRating");

        builder.HasIndex(s => s.IsFeatured)
            .HasDatabaseName("IX_Sites_IsFeatured");
        
        builder.HasIndex(s => s.IsHiddenGem)
            .HasDatabaseName("IX_Sites_IsHiddenGem");
        
        builder.HasIndex(s => s.IsPopular)
            .HasDatabaseName("IX_Sites_IsPopular");
    }
}
