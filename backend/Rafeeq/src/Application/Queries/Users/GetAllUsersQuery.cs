using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Enums;

namespace Application.Queries.Users;

public record GetAllUsersQuery(
    string? SearchTerm = null,
    UserRole? Role = null,
    UserStatus Status = UserStatus.Active,
    PagingParameters? Paging = null) : IQuery<PagedResult<UserListDto>>;

internal class GetAllUsersQueryHandler(
    IUserQueryService queryService) : IQueryHandler<GetAllUsersQuery, PagedResult<UserListDto>>
{
    public async Task<Result<PagedResult<UserListDto>>> HandleAsync(GetAllUsersQuery query, CancellationToken cancellationToken)
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