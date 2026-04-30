using Domain.Entities.ContentReportAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Common.Constants.DomainConstants.ContentReport;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ContentReports;

public sealed class ContentReportConfiguration : IEntityTypeConfiguration<ContentReport>
{
    public void Configure(EntityTypeBuilder<ContentReport> builder)
    {
        builder.Property(cr => cr.ReportedBy)
            .IsRequired();

        builder.Property(cr => cr.ContentId)
            .IsRequired();

        builder.Property(cr => cr.Description)
            .HasMaxLength(MaxDescriptionLength)
            .IsRequired();

        builder.Property(cr => cr.ReviewNotes)
            .IsRequired(false)
            .HasMaxLength(MaxReviewNotesLength);

        builder.Property(cr => cr.ReportedAt)
            .IsRequired();

        builder.Property(cr => cr.ActionTaken)
            .IsRequired(false);

        builder.HasIndex(cr => cr.ReportedBy)
            .HasDatabaseName("IX_ContentReports_ReportedBy");

        builder.HasIndex(cr => cr.ContentId)
            .HasDatabaseName("IX_ContentReports_ContentId");

        builder.HasIndex(cr => cr.Status)
            .HasDatabaseName("IX_ContentReports_Status");

        builder.HasIndex(cr => cr.Priority)
            .HasDatabaseName("IX_ContentReports_Priority");

        builder.HasIndex(cr => cr.ReportedAt)
            .HasDatabaseName("IX_ContentReports_ReportedAt");
    }
}
