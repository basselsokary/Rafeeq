using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Cities;
using Domain.Entities.CityAggregate;

namespace Application.Queries.Cities.LocalizedContents;

public sealed record GetCityLocalizedContentByIdQuery(Guid CityId, Guid ContentId) : IQuery<CityLocalizedContentDto>;

internal sealed class GetCityLocalizedContentByIdQueryHandler(
    ICityQueryService queryService) : IQueryHandler<GetCityLocalizedContentByIdQuery, CityLocalizedContentDto>
{
    public async Task<Result<CityLocalizedContentDto>> HandleAsync(GetCityLocalizedContentByIdQuery query, CancellationToken cancellationToken)
    {
        var localizedContent = await queryService.GetLocalizedContentByIdAsync(query.CityId, query.ContentId, cancellationToken);
        if (localizedContent is null)
            return CityErrors.LocalizedContentNotFound;

        return Result.Success(localizedContent);
    }
}
