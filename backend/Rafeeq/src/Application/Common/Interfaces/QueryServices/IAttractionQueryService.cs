using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IAttractionQueryService
{
    Task<AttractionDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AttractionAdminDetailDto?> GetByIdForAdminAsync(Guid siteId, CancellationToken cancellationToken);

    Task<PagedResult<AttractionListDto>> GetBySiteIdAsync(
        Guid siteId,
        AttractionType type,
        PagingParameters paging,
        CancellationToken cancellationToken = default);
}
