using Domain.Enums;

namespace Application.DTOs.Artifacts;

public record ArtifactLocalizedContentDto(
    Guid Id,
    LanguageCode Language,
    string Name,
    string Description);
