using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IAttractionQueryService
{
    Task<AttractionDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
    Task<PagedResult<AttractionListDto>> GetByTypeAsync(
        AttractionType type,
        PagingParameters? paging = null,
        CancellationToken cancellationToken = default);
}
