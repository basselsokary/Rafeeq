using Domain.Common;
using Domain.Enums;

namespace Domain.ValueObjects;

public class OpeningHour : ValueObject
{
    public WeekDay Day { get; }
    public TimeRange OpeningTime { get; } = null!;
    public bool IsClosed { get; }

    private OpeningHour() { }
    private OpeningHour(WeekDay day, TimeRange openingTime, bool isClosed)
    {
        Day = day;
        OpeningTime = openingTime;
        IsClosed = isClosed;
    }

    public static OpeningHour Create(WeekDay day, TimeRange openingTime, bool isClosed)
    {
        return new(day, openingTime, isClosed);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Day;
        yield return OpeningTime;
        yield return IsClosed;
    }
}