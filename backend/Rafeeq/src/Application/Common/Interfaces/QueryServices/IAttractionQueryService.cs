using Application.DTOs.Admins;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IAttractionQueryService
{
    Task<AttractionDetailDto?> GetByIdAsync(
        Guid id,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<AttractionAdminDetailDto?> GetByIdForAdminAsync(
        Guid siteId,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AttractionListDto>> GetBySiteIdAsync(
        Guid siteId,
        AttractionType? type,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    
    Task<List<AttractionLocalizedContentDto>> GetLocalizedContentsAsync(Guid attractionId, CancellationToken cancellationToken);

    Task<AttractionLocalizedContentDto?> GetLocalizedContentByIdAsync(Guid attractionId, Guid contentId, CancellationToken cancellationToken);

    Task<List<ImageDto>> GetImagesAsync(Guid attractionId, CancellationToken cancellationToken);

    Task<ImageDto?> GetImageByIdAsync(Guid attractionId, Guid imageId, CancellationToken cancellationToken);
    
    /// Get all attraction from all sites
    Task<PagedResult<AttractionListDto>> GetAllAsync(
        string? searchTerm, AttractionType? type, PagingParameters paging, CancellationToken cancellationToken);
    
    Task<AdminAttractionDashboardDto> GetDashboardAsync(CancellationToken cancellationToken);
}
