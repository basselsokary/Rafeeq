using Domain.Common;
using Domain.Exceptions;
using static Domain.Common.Constants.DomainConstants.Review;

namespace Domain.ValueObjects;

public class Rating : ValueObject
{
    public int Value { get; }

    private Rating(int value)
    {
        Value = value;
    }

    public static Rating Create(int value)
    {
        if (value < MinRatingValue || value > MaxRatingValue)
            throw new BusinessRuleValidationException($"Rating must be between {MinRatingValue} and {MaxRatingValue}.");

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
