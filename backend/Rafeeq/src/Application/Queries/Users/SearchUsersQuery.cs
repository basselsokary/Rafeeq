using Application.Common.Interfaces.Services;
using Application.DTOs.Users;

namespace Application.Queries.Users;

public sealed record SearchUsersQuery(
    string SearchTerm,
    int Limit = 10) : IQuery<List<UserSearchResultDto>>;

internal sealed class SearchUsersQueryHandler(IUserManagementService userManagementService)
    : IQueryHandler<SearchUsersQuery, List<UserSearchResultDto>>
{
    public async Task<Result<List<UserSearchResultDto>>> HandleAsync(SearchUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await userManagementService.SearchUsersAsync(
            query.SearchTerm,
            query.Limit,
            cancellationToken);
        
        return Result.Success(users);
    }
}
