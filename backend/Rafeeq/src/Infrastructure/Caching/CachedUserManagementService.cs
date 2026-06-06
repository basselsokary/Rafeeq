using Application.Commands.Users.Admins;
using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using Shared;

namespace Infrastructure.Caching;

internal sealed class CachedUserManagementService(IUserManagementService inner, IMemoryCache cache)
    : BaseCache("user-management", cache), IUserManagementService
{
    public async Task<PagedResult<UserListDto>> GetUsersAsync(
        PagingParameters paging,
        string? searchTerm,
        string? role,
        bool? emailVerified,
        string? sortBy,
        string? sortOrder,
        UserStatus? status,
        CancellationToken cancellationToken)
    {
        var normalizedSearch = (searchTerm ?? string.Empty).Trim().ToLowerInvariant();
        var key = $"{Prefix}:list:search={normalizedSearch}:status={status}:email-verified={emailVerified?.ToString() ?? "all"}:role={role ?? "all"}:sortBy={sortBy ?? "createdAt"}:sortOrder={sortOrder ?? "desc"}:{FormatPaging(paging)}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetUsersAsync(paging, searchTerm, role, emailVerified, sortBy, sortOrder, status, cancellationToken));
    }

    public async Task<UserDetailsDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:detail:{userId}";
        return await GetOrCreateNullableAsync(
            key,
            MediumTtl10_Minutes,
            () => inner.GetUserByIdAsync(userId, cancellationToken));
    }

    public async Task<List<UserSearchResultDto>> SearchUsersAsync(
        string searchTerm,
        int limit,
        CancellationToken cancellationToken)
    {
        var normalizedSearch = (searchTerm ?? string.Empty).Trim().ToLowerInvariant();
        var key = $"{Prefix}:list:search:{normalizedSearch}:{limit}";
        return await GetOrCreateAsync(
            key,
            MediumTtl10_Minutes,
            () => inner.SearchUsersAsync(searchTerm ?? string.Empty, limit, cancellationToken));
    }

    public async Task<Result<UserDetailsDto>> CreateModeratorAsync(string userName, string firstName, string lastName, string fullName, string email, string password, CancellationToken cancellationToken)
    {
        var result = await inner.CreateModeratorAsync(userName, firstName, lastName, fullName, email, password, cancellationToken);
        await InvalidateAsync(result.Succeeded);
        return result;
    }

    public async Task<Result<UserDetailsDto>> PromoteToAdminAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await inner.PromoteToAdminAsync(userId, cancellationToken);
        await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result<UserDetailsDto>> DemoteToModeratorAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await inner.DemoteToModeratorAsync(userId, cancellationToken);
        await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result<UserDetailsDto>> UpdateUserRolesAsync(
        Guid userId,
        List<string> roles,
        string? reason,
        CancellationToken cancellationToken)
    {
        var result = await inner.UpdateUserRolesAsync(userId, roles, reason, cancellationToken);
        await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result> LockUserAccountAsync(
        Guid userId,
        string reason,
        DateTime? lockUntil,
        CancellationToken cancellationToken)
    {
        var result = await inner.LockUserAccountAsync(userId, reason, lockUntil, cancellationToken);
        await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result> UnlockUserAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await inner.UnlockUserAccountAsync(userId, cancellationToken);
        await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result> SuspendUserAccountAsync(
        Guid userId,
        DateTime suspendUntil,
        string reason,
        bool notifyUser,
        CancellationToken cancellationToken)
    {
        var result = await inner.SuspendUserAccountAsync(userId, suspendUntil, reason, notifyUser, cancellationToken);
        await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result> ReactivateUserAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await inner.ReactivateUserAccountAsync(userId, cancellationToken);
        await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result<PasswordResetResultDto>> ResetUserPasswordAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await inner.ResetUserPasswordAsync(userId, cancellationToken);
        // await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result> RequirePasswordChangeAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await inner.RequirePasswordChangeAsync(userId, cancellationToken);
        await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result> ConfirmUserEmailAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await inner.ConfirmUserEmailAsync(userId, cancellationToken);
        await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result<EmailSentResultDto>> ResendVerificationEmailAsync(Guid userId, CancellationToken cancellationToken)
    {
        var result = await inner.ResendVerificationEmailAsync(userId, cancellationToken);
        // await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result> DeleteUserAsync(
        Guid userId,
        string reason,
        bool notifyUser,
        CancellationToken cancellationToken)
    {
        var result = await inner.DeleteUserAsync(userId, reason, notifyUser, cancellationToken);
        await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<Result> PermanentlyDeleteUserAsync(
        Guid userId,
        string reason,
        bool confirmDeletion,
        CancellationToken cancellationToken)
    {
        var result = await inner.PermanentlyDeleteUserAsync(userId, reason, confirmDeletion, cancellationToken);
        await InvalidateAsync(result.Succeeded, userId);
        return result;
    }

    public async Task<PagedResult<UserActivityDto>> GetUserActivityAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:activity:{userId}:{page}:{pageSize}";
        return await GetOrCreateAsync(
            key,
            MediumTtl10_Minutes,
            () => inner.GetUserActivityAsync(userId, page, pageSize, cancellationToken));
    }

    public async Task<PagedResult<LoginHistoryDto>> GetUserLoginHistoryAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:login-history:{userId}:{page}:{pageSize}";
        return await GetOrCreateAsync(
            key,
            MediumTtl10_Minutes,
            () => inner.GetUserLoginHistoryAsync(userId, page, pageSize, cancellationToken));
    }

    public async Task<UserStatisticsDto> GetUserStatisticsAsync(CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:statistics";
        return await GetOrCreateAsync(
            key,
            ShortTtl2_Minutes,
            () => inner.GetUserStatisticsAsync(cancellationToken));
    }

    private async Task InvalidateAsync(bool succeeded, Guid userId = default)
    {
        if (!succeeded)
            return;

        await RemoveByPrefixAsync(Prefix + ":list");
        await RemoveByPrefixAsync(Prefix + ":statistics");
        
        if (userId != default)
        {
            await RemoveByIdAsync(Prefix, userId.ToString());
        }
    }
}
