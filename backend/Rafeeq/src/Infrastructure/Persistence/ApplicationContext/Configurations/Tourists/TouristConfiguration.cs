using Domain.Entities.TouristAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Tourist;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Tourists;

internal sealed class TouristConfiguration : IEntityTypeConfiguration<Tourist>
{
    public void Configure(EntityTypeBuilder<Tourist> builder)
    {
        builder.Property(t => t.FirstName)
            .HasMaxLength(MaxFirstNameLength)
            .IsRequired();

        builder.Property(t => t.LastName)
            .HasMaxLength(MaxLastNameLength)
            .IsRequired();

        builder.Property(t => t.Nationality)
            .HasMaxLength(MaxNationalityLength)
            .IsRequired(false);

        builder.HasMany(t => t.Favourites)
            .WithOne()
            .HasForeignKey("TouristId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.VisitedSites)
            .WithOne()
            .HasForeignKey(vs => vs.TouristId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Tourists_Status");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Tourists_CreatedAt");

        builder.Ignore(t => t.DomainEvents);
    }
}
