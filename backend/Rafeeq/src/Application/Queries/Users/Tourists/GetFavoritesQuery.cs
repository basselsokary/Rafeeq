using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Tourists;

namespace Application.Queries.Users.Tourists;

public sealed record GetFavoritesQuery(PagingParameters Paging) : IQuery<PagedResult<FavoriteSiteDto>>;

internal sealed class GetFavoritesQueryHandler(
    ITouristQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetFavoritesQuery, PagedResult<FavoriteSiteDto>>
{
    public async Task<Result<PagedResult<FavoriteSiteDto>>> HandleAsync(GetFavoritesQuery query, CancellationToken cancellationToken)
    {
        var pagedResult = await queryService.GetFavoriteSitesAsync(
            userContext.Id,
            query.Paging,
            userContext.Language,
            cancellationToken);

        var localizedData = pagedResult.Data.Select(favoriteSite => favoriteSite with
        {
            SiteTypeDisplay = enumLocalizer.Localize(favoriteSite.SiteType)
        }).ToList();
    
        return Result.Success(pagedResult with { Data = localizedData });
    }
}