using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Authentication;
using Application.DTOs.Cities;

namespace Application.Queries.Cities;

public sealed record GetCitiesQuery() : IQuery<List<CityListDto>>;

internal sealed class GetCitiesQueryHandler(
    ICityQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetCitiesQuery, List<CityListDto>>
{
    public async Task<Result<List<CityListDto>>> HandleAsync(GetCitiesQuery query, CancellationToken cancellationToken)
    {
        List<CityListDto> pagedResult = await queryService.GetAsync(
            userContext.Language,
            cancellationToken);

        return Result.Success(pagedResult);
    }
}
