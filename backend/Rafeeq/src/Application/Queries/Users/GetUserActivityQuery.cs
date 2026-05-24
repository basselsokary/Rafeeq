using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Application.DTOs.Users;

namespace Application.Queries.Users;

public sealed record GetUserActivityQuery(
    Guid UserId,
    int Page,
    int PageSize) : IQuery<PagedResult<UserActivityDto>>;

internal sealed class GetUserActivityQueryHandler(IUserManagementService userManagementService)
    : IQueryHandler<GetUserActivityQuery, PagedResult<UserActivityDto>>
{
    public async Task<Result<PagedResult<UserActivityDto>>> HandleAsync(GetUserActivityQuery query, CancellationToken cancellationToken)
    {
        var userActivity = await userManagementService.GetUserActivityAsync(
            query.UserId,
            query.Page,
            query.PageSize,
            cancellationToken);

        return Result.Success(userActivity);
    }
}
