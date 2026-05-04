using Domain.Entities.TripAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Trip;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Trips;

internal sealed class TripNotesConfiguration : IEntityTypeConfiguration<TripNote>
{
    public void Configure(EntityTypeBuilder<TripNote> builder)
    {
        builder.ToTable("TripNotes");

        builder.Property(tn => tn.Content)
            .HasMaxLength(MaxDescriptionLength)
            .IsRequired();

        builder.Property(tn => tn.Title)
            .HasMaxLength(MaxNameLength)
            .IsRequired(false);

        builder.HasIndex("TripId")
            .HasDatabaseName("IX_TripNotes_TripId");
    }
}