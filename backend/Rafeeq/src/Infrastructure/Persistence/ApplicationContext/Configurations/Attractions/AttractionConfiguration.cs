using System.Text.Json;
using Domain.Entities.AttractionAggregate;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Image;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Attractions;

internal sealed class AttractionConfiguration : IEntityTypeConfiguration<Attraction>
{
    public void Configure(EntityTypeBuilder<Attraction> builder)
    {
        builder.Property(a => a.MainImageUrl)
            .HasMaxLength(MaxImageUrlLength);

        builder.OwnsOne(a => a.Location, location => 
        {
            location.Configure();
        });

        builder.Property<List<HistoricalPeriod>>("_historicalPeriods")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<HistoricalPeriod>>(v, (JsonSerializerOptions)null!)!)
            .HasColumnName("HistoricalPeriods")
            .HasColumnType("nvarchar(1024)")
            .Metadata.SetValueComparer(new ValueComparer<List<HistoricalPeriod>>(
                (c1, c2) => c1!.OrderBy(x => x).SequenceEqual(c2!.OrderBy(x => x)),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));

        builder.HasMany(a => a.Images)
            .WithOne()
            .HasForeignKey("AttractionId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.LocalizedContents)
            .WithOne()
            .HasForeignKey("AttractionId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(a => a.SiteId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.Type)
            .HasDatabaseName("IX_Attractions_Type");

        builder.HasIndex(a => a.IsFeatured)
            .HasDatabaseName("IX_Attractions_IsFeatured");

        builder.HasIndex(a => a.SiteId)
            .HasDatabaseName("IX_Attractions_SiteId");
    }
}
