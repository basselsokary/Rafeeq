using Domain.Common;
using Domain.Common.Exceptions;

namespace Domain.ValueObjects;

public class TimeRange : ValueObject
{
    public TimeSpan StartTime { get; }
    public TimeSpan EndTime { get; }

    private TimeRange() { }
    private TimeRange(TimeSpan startTime, TimeSpan endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public static TimeRange Create(TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime >= endTime)
            throw new BusinessRuleValidationException("Start time must be before end time.");

        return new TimeRange(startTime, endTime);
    }

    public TimeSpan Duration => EndTime - StartTime;

    public bool IsWithinRange(TimeSpan time)
    {
        return time >= StartTime && time <= EndTime;
    }

    public bool OverlapsWith(TimeRange other)
    {
        return StartTime < other.EndTime && EndTime > other.StartTime;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }

    public override string ToString()
    {
        return $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }
}
