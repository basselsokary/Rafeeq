namespace Application.DTOs.Common;

public sealed record ImageDto(
    Guid Id,
    string StorageKey,
    string Url,
    string? Caption,
    bool IsMain,
    int DisplayOrder);

public sealed record ImageMetadata(
    bool IsMain,
    int DisplayOrder,
    string? Caption);