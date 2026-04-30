using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Authentication;
using Application.DTOs.Cities;
using Domain.Entities.CityAggregate;

namespace Application.Queries.Cities;

public sealed record GetCityByIdQuery(
    Guid Id) : IQuery<CityDetailDto>;

internal sealed class GetCityByIdQueryHandler(
    ICityQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetCityByIdQuery, CityDetailDto>
{
    public async Task<Result<CityDetailDto>> HandleAsync(GetCityByIdQuery query, CancellationToken cancellationToken)
    {
        var cityDetailDto = await queryService.GetByIdAsync(
            query.Id,
            userContext.Language,
            cancellationToken);
        if (cityDetailDto == null)
            return CityErrors.NotFound(query.Id);

        return Result.Success(cityDetailDto);
    }
}