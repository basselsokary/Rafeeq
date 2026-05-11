using Domain.Enums;

namespace Application.DTOs.Artifacts;

public record ArtifactListDto(
    Guid Id,
    string Name,
    string? MainImageUrl,
    ArtifactType Type);