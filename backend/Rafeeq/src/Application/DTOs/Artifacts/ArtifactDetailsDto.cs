using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Artifacts;

public record ArtifactDetailsDto(
    Guid Id,
    Guid SiteId,
    string Name,
    string Description,
    string SiteName,
    string? MainImageUrl,
    ArtifactType Type,
    List<ImageDto> Images,
    string TypeDisplay = "");
