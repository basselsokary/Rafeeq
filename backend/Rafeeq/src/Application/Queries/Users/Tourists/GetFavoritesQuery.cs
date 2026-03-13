using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Users;

namespace Application.Queries.Users.Tourists;

public record GetFavoritesQuery(PagingParameters? Paging = null) : IQuery<PagedResult<FavoriteSiteDto>>;

internal class GetFavoritesQueryHandler(
    IUserQueryService queryService,
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
