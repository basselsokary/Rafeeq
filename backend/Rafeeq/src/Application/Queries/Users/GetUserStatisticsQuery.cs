using Application.Common.Interfaces.Services;
using Application.DTOs.Users;

namespace Application.Queries.Users;

public sealed record GetUserStatisticsQuery : IQuery<UserStatisticsDto>;

internal sealed class GetUserStatisticsQueryHandler(IUserManagementService userManagementService)
    : IQueryHandler<GetUserStatisticsQuery, UserStatisticsDto>
{
    public async Task<Result<UserStatisticsDto>> HandleAsync(GetUserStatisticsQuery query, CancellationToken cancellationToken)
    {
        var stats = await userManagementService.GetUserStatisticsAsync(cancellationToken);
        return Result.Success(stats);
    }
}
