using Domain.Entities.ContentReportAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ContentReports;

public sealed class ContentReportConfiguration : IEntityTypeConfiguration<ContentReport>
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

        builder.Property(cr => cr.ActionTaken)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(cr => cr.ReviewNotes)
            .HasMaxLength(1000);

        builder.Property(cr => cr.Priority)
            .IsRequired();

        builder.Property(cr => cr.ReportedAt)
            .IsRequired();

        builder.Property(cr => cr.CreatedAt)
            .IsRequired();

        builder.Property(cr => cr.LastModifiedAt);

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

        builder.Ignore(cr => cr.DomainEvents);
    }
}
