using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Tourists;
using Domain.Enums;

namespace Application.Queries.Users;

public record GetAllUsersQuery(
    PagingParameters Paging,
    string? SearchTerm = null,
    bool? EmailVerified = null,
    UserStatus Status = UserStatus.Active) : IQuery<PagedResult<TouristListDto>>;

internal class GetAllUsersQueryHandler(
    ITouristQueryService queryService) : IQueryHandler<GetAllUsersQuery, PagedResult<TouristListDto>>
{
    public async Task<Result<PagedResult<TouristListDto>>> HandleAsync(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var pagedResult = await queryService.GetAllAsync(
            query.Paging,
            query.SearchTerm,
            query.Status,
            query.EmailVerified,
            cancellationToken);
    
        return Result.Success(pagedResult);
    }
}