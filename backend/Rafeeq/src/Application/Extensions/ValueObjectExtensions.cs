using Application.DTOs.Common;
using Application.DTOs.Sites;
using Domain.ValueObjects;

namespace Application.Extensions;

public static class ValueObjectExtensions
{
    public static MoneyDto ToDto(this Money? money)
    {
        if (money == null)
        {
            return null!;
        }

        return new MoneyDto(
            money.Amount,
            money.Currency,
            money.ToString());
    }

    public static LocationDto ToDto(this GeoLocation? location)
    {
        if (location == null)
        {
            return null!;
        }
        return new LocationDto(
            location.Latitude,
            location.Longitude);
    }

    public static OpeningHourDto ToDto(this OpeningHour? openingHour)
    {
        if (openingHour == null)
        {
            return null!;
        }

        return new OpeningHourDto(
            openingHour.Day,
            openingHour.OpeningTime.StartTime,
            openingHour.OpeningTime.EndTime,
            openingHour.IsClosed);
    }

    public static TimeRangeDto ToDto(this TimeRange? timeRange)
    {
        if (timeRange == null)
        {
            return null!;
        }

        return new TimeRangeDto(
            timeRange.StartTime,
            timeRange.EndTime,
            timeRange.DurationInMinutes);
    }

    public static DateRangeDto ToDto(this DateRange? dateRange)
    {
        if (dateRange == null)
        {
            return null!;
        }

        return new DateRangeDto(
            dateRange.StartDate,
            dateRange.EndDate,
            dateRange.DurationInDays);
    }

    public static TicketDto ToDto(this Ticket? ticket, string? notes)
    {
        if (ticket == null)
        {
            return null!;
        }

        return new TicketDto(
            ticket.EgyptianPrice.ToDto(),
            ticket.ForeignerPrice.ToDto(),
            notes);
    }
}