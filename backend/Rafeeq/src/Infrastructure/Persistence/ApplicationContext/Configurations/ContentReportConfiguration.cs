using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RafeeqApp.Domain.Entities.ContentReportAggregate;

namespace RafeeqApp.Infrastructure.Persistence.ApplicationDbContext.Configurations;

public class ContentReportConfiguration : IEntityTypeConfiguration<ContentReport>
{
    public void Configure(EntityTypeBuilder<ContentReport> builder)
    {
        builder.ToTable("ContentReports");

        builder.HasKey(cr => cr.Id);

        // Properties
        builder.Property(cr => cr.ReporterId)
            .IsRequired();

        builder.Property(cr => cr.ContentType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(cr => cr.ContentId)
            .IsRequired();

        builder.Property(cr => cr.ReportReason)
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

        builder.Property(cr => cr.UpdatedAt);

        // Indexes
        builder.HasIndex(cr => cr.ReporterId)
            .HasDatabaseName("IX_ContentReports_ReporterId");

        builder.HasIndex(cr => cr.ContentId)
            .HasDatabaseName("IX_ContentReports_ContentId");

        builder.HasIndex(cr => cr.Status)
            .HasDatabaseName("IX_ContentReports_Status");

        builder.HasIndex(cr => new { cr.ContentType, cr.ContentId })
            .HasDatabaseName("IX_ContentReports_Content");

        builder.HasIndex(cr => cr.CreatedAt)
            .HasDatabaseName("IX_ContentReports_CreatedAt");

        // Ignore domain events
        builder.Ignore(cr => cr.DomainEvents);
    }
}
