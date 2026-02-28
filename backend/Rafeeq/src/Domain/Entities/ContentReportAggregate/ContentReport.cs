using Domain.Common;
using Domain.Common.Constants;
using Domain.Common.Interfaces;
using Domain.Enums;
using Shared.Models;

namespace Domain.Entities.ContentReportAggregate;

public class ContentReport : BaseAuditableEntity, IAggregateRoot
{
    public Guid ReportedBy { get; private set; }
    public Guid ReportedEntityId { get; private set; }
    // public string ReportedEntityType { get; private set; }

    public ReportReason Reason { get; private set; }
    public string Description { get; private set; } = null!;
    public ReportStatus Status { get; private set; }
    public DateTime ReportedAt { get; private set; }

    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewNotes { get; private set; }
    public ModerationAction? ActionTaken { get; private set; }
    public int Priority { get; private set; }

    private ContentReport() { }
    private ContentReport(
        Guid reportedBy,
        Guid reportedEntityId,
        ReportReason reason,
        string description)
    {
        ReportedBy = reportedBy;
        ReportedEntityId = reportedEntityId;
        Reason = reason;
        Description = description;
        Status = ReportStatus.Pending;
        ReportedAt = DateTime.UtcNow;
        Priority = CalculatePriority(reason);
    }

    public static Result<ContentReport> Create(
        Guid reportedBy,
        Guid reportedEntityId,
        ReportReason reason,
        string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return ContentReportErrors.DescriptionRequired;

        var report = new ContentReport(
            reportedBy,
            reportedEntityId,
            reason,
            description.Trim());

        // report.RaiseDomainEvent(new ContentReportedEvent(report.Id, report.ReportedEntityId));

        return report;
    }

    public Result Solve(Guid reviewerId, ModerationAction action, string? notes = null)
    {
        if (Status != ReportStatus.Pending && Status != ReportStatus.UnderReview)
            return ContentReportErrors.CantBeSolved;

        ReviewedBy = reviewerId;
        ReviewedAt = DateTime.UtcNow;
        ActionTaken = action;
        ReviewNotes = notes?.Trim();
        Status = ReportStatus.Resolved;
        
        MarkAsUpdated();

        // RaiseDomainEvent(new ContentReportResolvedEvent(Id, action));

        return Result.Success();
    }

    public Result MarkAsUnderReview(Guid reviewerId)
    {
        if (Status != ReportStatus.Pending)
            return ContentReportErrors.CantBeReviewed;

        Status = ReportStatus.UnderReview;
        ReviewedBy = reviewerId;
        
        MarkAsUpdated();

        return Result.Success();
    }

    public Result Dismiss(Guid reviewerId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return ContentReportErrors.CantBeDismissed;

        ReviewedBy = reviewerId;
        ReviewedAt = DateTime.UtcNow;
        Status = ReportStatus.Dismissed;
        ReviewNotes = reason.Trim();
        
        MarkAsUpdated();

        return Result.Success();
    }

    public void Escalate()
    {
        Priority = Math.Min(Priority + 1, 5);
        MarkAsUpdated();
    }

    public bool IsHighPriority()
    {
        return Priority >= DomainConstants.ContentReport.HighPriority;
    }

    public TimeSpan GetPendingDuration()
    {
        return DateTime.UtcNow - ReportedAt;
    }

    private static int CalculatePriority(ReportReason reason)
    {
        return reason switch
        {
            ReportReason.HateSpeech => 5,
            ReportReason.Violence => 5,
            ReportReason.ChildSafety => 5,
            ReportReason.Harassment => 4,
            ReportReason.Spam => 2,
            ReportReason.Misinformation => 3,
            ReportReason.Inappropriate => 3,
            ReportReason.Copyright => 2,
            _ => 1
        };
    }
}
