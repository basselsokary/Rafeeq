using Domain.Entities.SponsorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sponsors;

public sealed class SponsorConfiguration : IEntityTypeConfiguration<Sponsor>
{
    public void Configure(EntityTypeBuilder<Sponsor> builder)
    {
        builder.Property(s => s.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(s => s.MainImageUrl)
            .HasMaxLength(500);

        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Tier)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Website)
            .HasMaxLength(500);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(false);

        builder.Property(s => s.TotalRedemptions)
            .HasDefaultValue(0);

        builder.Property(s => s.ContractStartDate)
            .IsRequired();

        builder.Property(s => s.ContractEndDate)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.LastModifiedAt);

        builder.OwnsOne(s => s.Location, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("Latitude")
                .HasPrecision(9, 6)
                .IsRequired();

            location.Property(l => l.Longitude)
                .HasColumnName("Longitude")
                .HasPrecision(9, 6)
                .IsRequired();
        });

        builder.OwnsOne(s => s.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.Region)
                .HasColumnName("Region")
                .HasMaxLength(100);

            address.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(20);
        });

        builder.OwnsOne(s => s.ContactPhone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("ContactPhone")
                .HasMaxLength(20);

            phone.WithOwner();
        });

        builder.Navigation(s => s.ContactPhone).IsRequired(false);

        builder.OwnsOne(s => s.ContactEmail, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("ContactEmail")
                .HasMaxLength(100);

            email.WithOwner();
        });

        builder.Navigation(s => s.ContactEmail).IsRequired(false);

        builder.HasMany(s => s.Offers)
            .WithOne(o => o.Sponsor)
            .HasForeignKey("SponsorId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Images)
            .WithOne()
            .HasForeignKey("SponsorId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.Title)
            .HasDatabaseName("IX_Sponsors_Title");

        builder.HasIndex(s => s.Type)
            .HasDatabaseName("IX_Sponsors_Type");

        builder.HasIndex(s => s.Tier)
            .HasDatabaseName("IX_Sponsors_Tier");

        builder.HasIndex(s => s.IsActive)
            .HasDatabaseName("IX_Sponsors_IsActive");

        builder.HasIndex(s => new { s.ContractStartDate, s.ContractEndDate })
            .HasDatabaseName("IX_Sponsors_ContractDates");

        builder.Ignore(s => s.DomainEvents);
    }
}
