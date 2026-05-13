using Domain.Entities.SiteAggregate;
using Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.Site;
using static Domain.Common.Constants.DomainConstants.Ticket;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.Sites;

internal sealed class SiteLocalizedContentConfiguration : IEntityTypeConfiguration<SiteLocalizedContent>
{
    public void Configure(EntityTypeBuilder<SiteLocalizedContent> builder)
    {
        builder.Property(c => c.Name)
            .HasMaxLength(MaxNameLength)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(MaxDescriptionLength)
            .IsRequired();

        builder.Property(t => t.EntryTicketNotes)
            .HasMaxLength(MaxNotesLength)
            .IsRequired(false);

        builder.OwnsOne(s => s.Address, address =>
        {
            address.Configure();
        });

        builder.HasIndex("SiteId", nameof(SiteLocalizedContent.Language))
            .IsUnique()
            .HasDatabaseName("IX_SiteLocalizedContents_SiteId_Language");
        
        builder.HasIndex("SiteId")
            .HasDatabaseName("IX_SiteLocalizedContents_SiteId");
        
        builder.HasIndex(s => new { s.Language, s.Name })
            .HasDatabaseName("IX_SiteLocalizedContents_Language_Name");

        builder.HasIndex(s => s.Name)
            .IsUnique()
            .HasDatabaseName("IX_SiteLocalizedContents_Name");
    }
}