using Domain.Common;
using Shared.Models;

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

    public static Result<TimeRange> Create(TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime >= endTime)
            return TimeRangeErrors.StartTimeNotBeforeEndTime;

        return new TimeRange(startTime, endTime);
    }

    public TimeSpan Duration => EndTime - StartTime;
    public int DurationInMinutes => (int)Duration.TotalMinutes;

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
