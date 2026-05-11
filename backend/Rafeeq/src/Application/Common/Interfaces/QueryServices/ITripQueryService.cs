using Application.DTOs.Common;
using Application.DTOs.Trips;

namespace Application.Common.Interfaces.QueryServices;

public interface ITripQueryService
{
    Task<TripDetailDto?> GetByIdAsync(
        Guid id,
        Guid touristId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<TripListDto>> GetByTouristIdAsync(
        Guid touristId,
        PagingParameters paging,
        CancellationToken cancellationToken = default);
}
