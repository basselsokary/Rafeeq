using Domain.Common;
using Domain.Common.Interfaces;
using Domain.ValueObjects;
using Shared.Models;
using Domain.Enums;

namespace Domain.Entities.ReviewAggregate;

public class Review : BaseAuditableEntity, IAggregateRoot
{
    public Guid SiteId { get; private set; }
    public Guid UserId { get; private set; }

    public Rating Rating { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public ReviewStatus Status { get; private set; }

    public int HelpfulCount { get; private set; }
    public int NotHelpfulCount { get; private set; }

    public string? RejectionReason { get; private set; }

    private Review() { }
    private Review(
        Guid touristId,
        Guid siteId,
        Rating rating,
        string title,
        string content)
    {
        UserId = touristId;
        SiteId = siteId;
        Rating = rating;
        Title = title;
        Content = content;

        Status = ReviewStatus.Pending;
        HelpfulCount = 0;
        NotHelpfulCount = 0;
    }

    public static Result<Review> Create(
        Guid touristId,
        Guid siteId,
        Rating rating,
        string title,
        string content)
    {
        if (touristId == Guid.Empty)
            return ReviewErrors.EntityIdRequired("User");
        
        if (siteId == Guid.Empty)
            return ReviewErrors.EntityIdRequired("Site");

        if (string.IsNullOrWhiteSpace(title))
            return ReviewErrors.TitleRequired;

        if (string.IsNullOrWhiteSpace(content))
            return ReviewErrors.ContentRequired;
        
        return new Review(touristId, siteId, rating, title.Trim(), content.Trim());
    }

    public Result Update(Rating rating, string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title))
            return ReviewErrors.TitleRequired;

        if (string.IsNullOrWhiteSpace(content))
            return ReviewErrors.ContentRequired;

        Rating = rating;
        Content = content.Trim();
        Title = title.Trim();
        
        return Result.Success();
    }

    public void Approve()
    {
        if (Status == ReviewStatus.Approved) return;

        Status = ReviewStatus.Approved;
        RejectionReason = null;
        MarkAsUpdated();
    }

    public Result Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return ReviewErrors.RejectionReasonReqiured;

        Status = ReviewStatus.Rejected;
        RejectionReason = reason.Trim();
        MarkAsUpdated();

        return Result.Success();
    }

    public void Flag()
    {
        Status = ReviewStatus.Flagged;
        MarkAsUpdated();
    }

    public void MarkAsHelpful()
    {
        HelpfulCount++;
        MarkAsUpdated();
    }

    public void MarkAsNotHelpful()
    {
        NotHelpfulCount++;
        MarkAsUpdated();
    }

    public double GetHelpfulnessScore()
    {
        var total = HelpfulCount + NotHelpfulCount;
        if (total == 0) return 0;
        return (double)HelpfulCount / total;
    }
}
