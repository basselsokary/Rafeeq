using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.ContentReportAggregate;

namespace Infrastructure.Persistence.ApplicationContext.Configurations;

public class ContentReportConfiguration : IEntityTypeConfiguration<ContentReport>
{
    public void Configure(EntityTypeBuilder<ContentReport> builder)
    {
        builder.Property(cr => cr.ReportedBy)
            .IsRequired();

        builder.Property(cr => cr.ContentId)
            .IsRequired();

        builder.Property(cr => cr.Reason)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(cr => cr.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(cr => cr.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(cr => cr.ReviewedAt);

        builder.Property(cr => cr.ReviewedBy);

        builder.Property(cr => cr.ReviewNotes)
            .HasMaxLength(1000);

        builder.Property(cr => cr.ActionTaken)
            .HasMaxLength(500);

        builder.Property(cr => cr.CreatedAt)
            .IsRequired();

        builder.Property(cr => cr.LastModifiedAt);

        // Indexes
        builder.HasIndex(cr => cr.ReportedBy)
            .HasDatabaseName("IX_ContentReports_ReporterId");

        builder.HasIndex(cr => cr.ContentId)
            .HasDatabaseName("IX_ContentReports_ContentId");

        builder.HasIndex(cr => cr.Status)
            .HasDatabaseName("IX_ContentReports_Status");

        builder.HasIndex(cr => cr.CreatedAt)
            .HasDatabaseName("IX_ContentReports_CreatedAt");

        // Ignore domain events
        builder.Ignore(cr => cr.DomainEvents);
    }
}
