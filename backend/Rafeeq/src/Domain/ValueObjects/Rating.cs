using Domain.Common;
using Shared.Models;
using static Domain.Common.Constants.DomainConstants.Review;

namespace Domain.ValueObjects;

public class Rating : ValueObject
{
    public int Value { get; }

    private Rating(int value)
    {
        Value = value;
    }

    public static Result<Rating> Create(int value)
    {
        if (value < MinRatingValue || value > MaxRatingValue)
            return RatingErrors.OutOfRange(MinRatingValue, MaxRatingValue);

        return new Rating(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return $"{Value}/5";
    }

    public static implicit operator int(Rating rating) => rating.Value;
}
