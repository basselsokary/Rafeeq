using Domain.Common;
using Shared;

namespace Domain.ValueObjects;

public class Rating : ValueObject
{
    public const int Min = 1;
    public const int Max = 5;

    public int Value { get; }

    private Rating() { }
    private Rating(int value)
    {
        Value = value;
    }

    public static Result<Rating> Create(int value)
    {
        if (value < Min || value > Max)
            return RatingErrors.OutOfRange(Min, Max);

        return new Rating(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return $"{Value}/{Max}";
    }

    public static implicit operator int(Rating rating) => rating.Value;
}
