using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Enums;

namespace Application.Queries.Users;

public sealed record GetUsersQuery(
    PagingParameters Paging,
    string? SearchTerm = null,
    string? Role = null,
    bool? EmailVerified = null,
    UserStatus Status = UserStatus.Active) : IQuery<PagedResult<UserListDto>>;

internal sealed class GetUsersQueryHandler(
    IUserManagementService userManagementService) : IQueryHandler<GetUsersQuery, PagedResult<UserListDto>>
{
    public async Task<Result<PagedResult<UserListDto>>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await userManagementService.GetUsersAsync(
            query.Paging,
            query.SearchTerm,
            query.Role,
            query.EmailVerified,
            query.Status,
            cancellationToken);

        return Result.Success(users);
    }
}