using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Application.DTOs.Users;

namespace Application.Queries.Users;

public sealed record GetUserLoginHistoryQuery(
    Guid UserId,
    int Page,
    int PageSize) : IQuery<PagedResult<LoginHistoryDto>>;

internal sealed class GetUserLoginHistoryQueryHandler(IUserManagementService userManagementService)
    : IQueryHandler<GetUserLoginHistoryQuery, PagedResult<LoginHistoryDto>>
{
    public async Task<Result<PagedResult<LoginHistoryDto>>> HandleAsync(GetUserLoginHistoryQuery query, CancellationToken cancellationToken)
    {
        var userLoginHistory = await userManagementService.GetUserLoginHistoryAsync(
            query.UserId,
            query.Page,
            query.PageSize,
            cancellationToken);

        return Result.Success(userLoginHistory);
    }
}
