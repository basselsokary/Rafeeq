using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.Interfaces;
using Domain.ValueObjects;
using Shared.Models;
using static Domain.Common.Constants.DomainConstants.Review;

namespace Domain.Entities.ReviewAggregate;

public class Review : BaseAuditableEntity, IAggregateRoot
{
    public Guid AttractionId { get; private set; }
    public Guid TouristId { get; private set; }

    public Rating Rating { get; private set; } = null!;
    public string? Text { get; private set; }

    private Review() { }
    private Review(Guid touristId, Guid placeId, Rating rating, string? text)
    {
        TouristId = touristId;
        AttractionId = placeId;
        Rating = rating;
        Text = text;
    }

    public static Review Create(Guid touristId, Guid placeId, Rating rating, string? text)
    {
        if (touristId == Guid.Empty)
            throw new BusinessRuleValidationException("Tourist ID cannot be null or empty.");
        
        if (placeId == Guid.Empty)
            throw new BusinessRuleValidationException("Place ID cannot be empty.");

        if (rating < MinRatingValue || rating > MaxRatingValue)
            throw new BusinessRuleValidationException($"Rating must be between {MinRatingValue} and {MaxRatingValue}.");

        return new(touristId, placeId, rating, text);
    }

    public Result Update(Rating rating, string? text)
    {
        if (rating < MinRatingValue || rating > MaxRatingValue)
            return Result.Failure(
                Error.Validation("", $"Rating must be between {MinRatingValue} and {MaxRatingValue}."));

        Rating = rating;
        Text = text;
        
        return Result.Success();
    }
}
