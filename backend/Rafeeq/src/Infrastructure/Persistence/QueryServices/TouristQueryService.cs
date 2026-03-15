using Application.Common.Interfaces.QueryServices;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Common;
using Application.DTOs.Tourists;

namespace Infrastructure.Persistence.QueryServices;

internal class TouristQueryService(ApplicationDbContext context) : ITouristQueryService
{
    public Task<PagedResult<TouristListDto>> GetAllAsync(string? searchTerm, TouristStatus status = TouristStatus.Active, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TouristProfileDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TouristProfileDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Guid>> GetFavoriteSiteIdsAsync(Guid touristId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<FavoriteSiteDto>> GetFavoriteSitesAsync(Guid touristId, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasFavoritedSiteAsync(Guid touristId, Guid siteId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

}
