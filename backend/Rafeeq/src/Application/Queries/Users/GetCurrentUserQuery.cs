using Application.Common.Interfaces.Authentication;

namespace Application.Queries.Users;

public sealed record GetCurrentUserQuery : IQuery<CurrentUserResponse>;

internal sealed class GetCurrentUserQueryHandler(IUserContext userContext)
    : IQueryHandler<GetCurrentUserQuery, CurrentUserResponse>
{
    public async Task<Result<CurrentUserResponse>> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        var currentUser = userContext.User;

        if (currentUser is null)
            return Result.Failure<CurrentUserResponse>("No authenticated user found.");

        var response = new CurrentUserResponse(
            currentUser.Id,
            currentUser.UserName,
            currentUser.Email,
            currentUser.Roles);

        return Result.Success(response);
    }
}

public sealed record CurrentUserResponse(
    Guid Id,
    string UserName,
    string Email,
    List<string> Roles);