using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Users;
using Domain.Entities.UserAggregate;

namespace Application.Queries.Users.Tourists;

public record GetProfileQuery : IQuery<UserProfileDto>;

internal class GetProfileQueryHandler(
    IUserQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetProfileQuery, UserProfileDto>
{
    public async Task<Result<UserProfileDto>> HandleAsync(GetProfileQuery query, CancellationToken cancellationToken)
    {
        var userProfileDto = await queryService.GetByIdAsync(userContext.Id, cancellationToken);
        if (userProfileDto == null)
            return UserErrors.NotFound(userContext.Id);

        return Result.Success(userProfileDto);
    }
}