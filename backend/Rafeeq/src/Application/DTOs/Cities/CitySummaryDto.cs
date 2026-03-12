namespace Application.DTOs.Cities;

public record CitySummaryDto(
    Guid Id,
    string Name,
    int DisplayOrder);
