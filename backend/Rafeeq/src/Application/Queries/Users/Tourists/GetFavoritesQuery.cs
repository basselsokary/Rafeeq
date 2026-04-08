using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Tourists;

namespace Application.Queries.Users.Tourists;

public record GetFavoritesQuery(PagingParameters Paging) : IQuery<PagedResult<FavoriteSiteDto>>;

internal class GetFavoritesQueryHandler(
    ITouristQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetFavoritesQuery, PagedResult<FavoriteSiteDto>>
{
    public async Task<Result<PagedResult<FavoriteSiteDto>>> HandleAsync(GetFavoritesQuery query, CancellationToken cancellationToken)
    {
        var pagedResult = await queryService.GetFavoriteSitesAsync(
            userContext.Id,
            query.Paging,
            cancellationToken);
    
        return Result.Success(pagedResult);
    }
}
