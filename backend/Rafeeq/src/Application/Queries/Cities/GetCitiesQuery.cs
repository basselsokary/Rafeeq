using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Cities;
using Application.DTOs.Common;

namespace Application.Queries.Cities;

public record GetCitiesQuery(PagingParameters Paging) : IQuery<PagedResult<CityListDto>>;

internal class GetCitiesQueryHandler(
    ICityQueryService queryService) : IQueryHandler<GetCitiesQuery, PagedResult<CityListDto>>
{
    public async Task<Result<PagedResult<CityListDto>>> HandleAsync(GetCitiesQuery query, CancellationToken cancellationToken)
    {
        PagedResult<CityListDto> pagedResult = await queryService.GetAsync(query.Paging, cancellationToken);

        return Result.Success(pagedResult);
    }
}
