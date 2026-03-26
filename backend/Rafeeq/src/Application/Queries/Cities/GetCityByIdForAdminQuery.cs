using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Cities;
using Domain.Entities.CityAggregate;

namespace Application.Queries.Cities;

public record GetCityByIdForAdminQuery(
    Guid Id) : IQuery<CityAdminDetailDto>;

internal class GetCityByIdForAdminQueryHandler(
    ICityQueryService queryService) : IQueryHandler<GetCityByIdForAdminQuery, CityAdminDetailDto>
{
    public async Task<Result<CityAdminDetailDto>> HandleAsync(GetCityByIdForAdminQuery query, CancellationToken cancellationToken)
    {
        var cityDetailDto = await queryService.GetByIdForAdminAsync(query.Id, cancellationToken);
        if (cityDetailDto == null)
            return CityErrors.NotFound(query.Id);

        return Result.Success(cityDetailDto);
    }
}