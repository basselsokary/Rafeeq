using Domain.Common;
using Domain.Common.Interfaces;
using Domain.ValueObjects;
using Domain.Enums;
using Domain.Entities.SiteAggregate;
using Domain.Entities.TouristAggregate;
using Shared;

namespace Domain.Entities.ReviewAggregate;

public class Review : BaseAuditableEntity, IAggregateRoot
{
    public Guid SiteId { get; private set; }
    public Guid TouristId { get; private set; }
    
    /// Only to use them for read queries
    public Site Site { get; private set; } = null!;
    public Tourist Tourist { get; private set; } = null!;

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
        TouristId = touristId;
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
        
        // review.RaiseDomainEvent(new ReviewCreatedEvent(review.Id, review.SiteId, review.TouristId, review.Rating));
        
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

        RaiseDomainEvent(new ReviewUpdatedEvent(Id, SiteId, oldRating, Rating));
        
        return Result.Success();
    }

    public void Delete()
    {
        RaiseDomainEvent(new ReviewDeletedEvent(Id, SiteId, Rating));
    }

    public Result Approve()
    {
        if (Status == ReviewStatus.Approved) return Result.Success();

        Status = ReviewStatus.Approved;
        RejectionReason = null;
        
        RaiseDomainEvent(new ReviewApprovedEvent(Id, SiteId, TouristId, Rating));

        return Result.Success();
    }

    public Result Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return ReviewErrors.RejectionReasonRequired;

        Status = ReviewStatus.Rejected;
        RejectionReason = reason.Trim();

        return Result.Success();
    }

    public Result Flag()
    {
        Status = ReviewStatus.Flagged;
        return Result.Success();
    }

    public void MarkAsHelpful()
    {
        HelpfulCount++;
    }

    public void MarkAsNotHelpful()
    {
        NotHelpfulCount++;
    }

    public double GetHelpfulnessScore()
    {
        var total = HelpfulCount + NotHelpfulCount;
        if (total == 0) return 0;
        return (double)HelpfulCount / total;
    }
}
