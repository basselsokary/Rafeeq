using Domain.Enums;

namespace Application.DTOs.Sites;

public record OpeningHourDto(
    WeekDay Day,
    TimeOnly? OpenTime,
    TimeOnly? CloseTime,
    bool IsClosed,
    string DayDisplay = "");
