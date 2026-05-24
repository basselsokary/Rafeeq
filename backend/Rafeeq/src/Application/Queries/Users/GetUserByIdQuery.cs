using Application.Common.Interfaces.Services;
using Application.DTOs.Users;
using Domain.Common;

namespace Application.Queries.Users;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserDetailsDto>;

internal sealed class GetUserByIdQueryHandler(IUserManagementService userManagementService)
    : IQueryHandler<GetUserByIdQuery, UserDetailsDto>
{
    public async Task<Result<UserDetailsDto>> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await userManagementService.GetUserByIdAsync(query.UserId, cancellationToken);
        if (user == null)
            return Result.Failure<UserDetailsDto>(UserErrors.NotFound(query.UserId.ToString()));

        return Result.Success(user);
    }
}
