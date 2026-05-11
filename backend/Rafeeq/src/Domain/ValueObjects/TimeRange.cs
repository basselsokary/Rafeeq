using Domain.Common;
using Shared;

namespace Domain.ValueObjects;

public class TimeRange : ValueObject
{
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }

    private TimeRange() { }
    private TimeRange(TimeOnly startTime, TimeOnly endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public static Result<TimeRange> Create(TimeOnly startTime, TimeOnly endTime, bool differentDays = false)
    {
        if (startTime > endTime && !differentDays)
            return TimeRangeErrors.StartTimeNotBeforeEndTime;

        return new TimeRange(startTime, endTime);
    }

    public TimeSpan Duration
    {
        get
        {
            var start = StartTime.ToTimeSpan();
            var end = EndTime.ToTimeSpan();

            return end >= start
                ? end - start
                : TimeSpan.FromDays(1) - start + end;
        }
    }

    public int DurationInMinutes => (int)Duration.TotalMinutes;
    public int DurationInHours => (int)Duration.TotalHours;

    public bool IsOvernight => EndTime < StartTime;

    public bool IsWithinRange(TimeOnly time)
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
