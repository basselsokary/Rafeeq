using Domain.Common;
using Domain.Common.Interfaces;
using Domain.ValueObjects;
using Shared.Models;
using Domain.Enums;
using Domain.Events;
using Domain.Entities.SiteAggregate;

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
            return ReviewErrors.TouristIdRequired;
        
        if (siteId == Guid.Empty)
            return ReviewErrors.SiteIdRequired;

        if (string.IsNullOrWhiteSpace(title))
            return ReviewErrors.TitleRequired;

        if (string.IsNullOrWhiteSpace(content))
            return ReviewErrors.ContentRequired;
        
        var review = new Review(touristId, siteId, rating, title.Trim(), content.Trim());
        
        // review.RaiseDomainEvent(new ReviewCreatedEvent(review.Id, review.SiteId, review.UserId, review.Rating));
        
        return review;
    }

    public Result Update(Rating rating, string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title))
            return ReviewErrors.TitleRequired;

        if (string.IsNullOrWhiteSpace(content))
            return ReviewErrors.ContentRequired;

        var oldRating = Rating;

        Rating = rating;
        Content = content.Trim();
        Title = title.Trim();
        Status = ReviewStatus.Pending;

        MarkAsUpdated();
        
        RaiseDomainEvent(new ReviewUpdatedEvent(Id, SiteId, UserId, oldRating, Rating));
        
        return Result.Success();
    }

    public void Delete()
    {
        RaiseDomainEvent(new ReviewDeletedEvent(Id, SiteId, UserId, Rating));
    }

    public Result Approve()
    {
        if (Status == ReviewStatus.Approved) return Result.Success();

        Status = ReviewStatus.Approved;
        RejectionReason = null;
        MarkAsUpdated();
        
        RaiseDomainEvent(new ReviewApprovedEvent(Id, SiteId, UserId, Rating));

        return Result.Success();
    }

    public Result Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return ReviewErrors.RejectionReasonRequired;

        Status = ReviewStatus.Rejected;
        RejectionReason = reason.Trim();
        MarkAsUpdated();

        return Result.Success();
    }

    public Result Flag()
    {
        Status = ReviewStatus.Flagged;
        MarkAsUpdated();
        return Result.Success();
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
