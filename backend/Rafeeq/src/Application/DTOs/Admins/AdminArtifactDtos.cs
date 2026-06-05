using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Admins;

public sealed record AdminArtifactDashboardDto(
    int TotalArtifacts,
    int AssignedToSites);

public record ArtifactAdminDetailDto(
    Guid Id,
    Guid? SiteId,
    string Name,
    string Description,
    string? SiteName,
    string? MainImageUrl,
    ArtifactType Type,
    int DisplayOrder,
    List<ImageDto> Images,
    DateTime CreatedAt,
    Guid CreatedBy,
    DateTime? LastModifiedAt,
    Guid? LastModifiedBy);