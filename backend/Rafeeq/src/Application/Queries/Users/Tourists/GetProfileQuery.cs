using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Tourists;
using Domain.Entities.TouristAggregate;

namespace Application.Queries.Users.Tourists;

public record GetProfileQuery : IQuery<TouristProfileDto>;

internal class GetProfileQueryHandler(
    IUserQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetProfileQuery, TouristProfileDto>
{
    public async Task<Result<TouristProfileDto>> HandleAsync(GetProfileQuery query, CancellationToken cancellationToken)
    {
        var userProfileDto = await queryService.GetByIdAsync(userContext.Id, cancellationToken);
        if (userProfileDto == null)
            return TouristErrors.NotFound(userContext.Id);

        return Result.Success(userProfileDto);
    }
}