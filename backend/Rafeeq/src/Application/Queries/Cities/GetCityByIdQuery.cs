using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Cities;
using Domain.Entities.CityAggregate;

namespace Application.Queries.Cities;

public record GetCityByIdQuery(
    Guid Id) : IQuery<CityDetailDto>;

internal class GetCityByIdQueryHandler(
    ICityQueryService queryService) : IQueryHandler<GetCityByIdQuery, CityDetailDto>
{
    public async Task<Result<CityDetailDto>> HandleAsync(GetCityByIdQuery query, CancellationToken cancellationToken)
    {
        var cityDetailDto = await queryService.GetByIdAsync(query.Id, cancellationToken);
        if (cityDetailDto == null)
            return CityErrors.NotFound(query.Id);

        return Result.Success(cityDetailDto);
    }
}