using Domain.Common;
using Shared;

namespace Domain.ValueObjects;

public class DateRange : ValueObject
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    private DateRange() { }
    private DateRange(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Result<DateRange> Create(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            return DateRangeErrors.StartDateNotBeforeEndDate;

        return new DateRange(startDate.Date, endDate.Date);
    }

    public int DurationInDays => (EndDate - StartDate).Days + 1;

    public bool IsWithinRange(DateTime date)
    {
        var checkDate = date.Date;
        return checkDate >= StartDate && checkDate <= EndDate;
    }

    public bool OverlapsWith(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    public override string ToString()
    {
        return $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
    }
}
