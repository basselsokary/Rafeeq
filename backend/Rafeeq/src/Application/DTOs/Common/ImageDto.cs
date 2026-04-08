namespace Application.DTOs.Common;

/// <summary>
/// Represents an image
/// </summary>
public record ImageDto(
    Guid Id,
    string Url,
    string? Caption,
    bool IsMain,
    int DisplayOrder);
