using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Authentication;
using Domain.Entities.CityAggregate;
using Application.DTOs.Admins;

namespace Application.Queries.Cities;

public sealed record GetCityByIdForAdminQuery(
    Guid Id) : IQuery<CityAdminDetailDto>;

internal sealed class GetCityByIdForAdminQueryHandler(
    ICityQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetCityByIdForAdminQuery, CityAdminDetailDto>
{
    public async Task<Result<CityAdminDetailDto>> HandleAsync(GetCityByIdForAdminQuery query, CancellationToken cancellationToken)
    {
        var cityDetailDto = await queryService.GetByIdForAdminAsync(
            query.Id,
            userContext.Language,
            cancellationToken);
        if (cityDetailDto == null)
            return CityErrors.NotFound(query.Id);

        return Result.Success(cityDetailDto);
    }
}