using Domain.Common;
using Domain.Common.Exceptions;

namespace Domain.ValueObjects;

public class OpeningHour : ValueObject
{
    public DayOfWeek DayOfWeek { get; }
    public TimeRange OpeningTime { get; } = null!;
    public bool IsClosed { get; }

    private OpeningHour() { }
    private OpeningHour(DayOfWeek dayOfWeek, TimeRange openingTime, bool isClosed)
    {
        DayOfWeek = dayOfWeek;
        OpeningTime = openingTime;
        IsClosed = isClosed;
    }

    public static OpeningHour Create(DayOfWeek dayOfWeek, TimeRange openingTime, bool isClosed)
    {
        return new(dayOfWeek, openingTime, isClosed);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DayOfWeek;
        yield return OpeningTime;
        yield return IsClosed;
    }
}