using Application.DTOs.Cities;
using Application.DTOs.Common;

namespace Application.Common.Interfaces.QueryServices;

public interface ICityQueryService
{
    Task<CityDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CityAdminDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<List<CitySummaryDto>> GetAsync(
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<CityListDto>> GetAsync(
        PagingParameters paging,
        CancellationToken cancellationToken = default);
}
