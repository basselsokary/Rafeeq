namespace Application.DTOs.Reviews;

/// <summary>
/// Review response DTO
/// </summary>
public record ReviewResponseDto(
    Guid Id,
    Guid ResponderId,
    string ResponderName,
    string? ResponderProfileImageUrl,
    string Content,
    bool IsOfficialResponse,
    DateTime ResponseDate);
