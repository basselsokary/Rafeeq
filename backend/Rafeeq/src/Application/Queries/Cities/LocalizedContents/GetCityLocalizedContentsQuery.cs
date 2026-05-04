using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Cities;

namespace Application.Queries.Cities.LocalizedContents;

public sealed record GetCityLocalizedContentsQuery(Guid CityId) : IQuery<List<CityLocalizedContentDto>>;

internal sealed class GetCityLocalizedContentsQueryHandler(
    ICityQueryService queryService) : IQueryHandler<GetCityLocalizedContentsQuery, List<CityLocalizedContentDto>>
{
    public async Task<Result<List<CityLocalizedContentDto>>> HandleAsync(GetCityLocalizedContentsQuery query, CancellationToken cancellationToken)
    {
        var localizedContentDtos = await queryService.GetLocalizedContentsAsync(query.CityId, cancellationToken);
    
        return localizedContentDtos;
    }
}