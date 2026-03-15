using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Tourists;
using Domain.Enums;

namespace Application.Queries.Users;

public record GetAllUsersQuery(
    string? SearchTerm = null,
    UserRole? Role = null,
    TouristStatus Status = TouristStatus.Active,
    PagingParameters? Paging = null) : IQuery<PagedResult<TouristListDto>>;

internal class GetAllUsersQueryHandler(
    ITouristQueryService queryService) : IQueryHandler<GetAllUsersQuery, PagedResult<TouristListDto>>
{
    public async Task<Result<PagedResult<TouristListDto>>> HandleAsync(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var pagedResult = await queryService.GetAllAsync(
            query.SearchTerm,
            query.Role,
            query.Status,
            query.Paging,
            cancellationToken);
    
        return Result.Success(pagedResult);
    }
}