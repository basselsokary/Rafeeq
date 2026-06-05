using Application.Commands.Users.Admins;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Enums;

namespace Application.Common.Interfaces.Services;

public interface IUserManagementService
{
    Task<PagedResult<UserListDto>> GetUsersAsync(
        PagingParameters paging,
        string? searchTerm,
        string? role,
        bool? emailVerified,
        string? sortBy,
        string? sortOrder,
        UserStatus? status,
        CancellationToken cancellationToken);

    Task<UserDetailsDto?> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<List<UserSearchResultDto>> SearchUsersAsync(
        string searchTerm,
        int limit,
        CancellationToken cancellationToken);

    Task<Result<UserDetailsDto>> CreateModeratorAsync(
        string userName,
        string firstName,
        string lastName,
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<Result<UserDetailsDto>> PromoteToAdminAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result<UserDetailsDto>> DemoteToModeratorAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result<UserDetailsDto>> UpdateUserRolesAsync(
        Guid userId,
        List<string> roles,
        string? reason,
        CancellationToken cancellationToken);

    Task<Result> LockUserAccountAsync(
        Guid userId,
        string reason,
        DateTime? lockUntil,
        CancellationToken cancellationToken);

    Task<Result> UnlockUserAccountAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result> SuspendUserAccountAsync(
        Guid userId,
        DateTime suspendUntil,
        string reason,
        bool notifyUser,
        CancellationToken cancellationToken);

    Task<Result> ReactivateUserAccountAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result<PasswordResetResultDto>> ResetUserPasswordAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result> RequirePasswordChangeAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result> ConfirmUserEmailAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result<EmailSentResultDto>> ResendVerificationEmailAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Result> DeleteUserAsync(
        Guid userId,
        string reason,
        bool notifyUser,
        CancellationToken cancellationToken);

    Task<Result> PermanentlyDeleteUserAsync(
        Guid userId,
        string reason,
        bool confirmDeletion,
        CancellationToken cancellationToken);

    Task<PagedResult<UserActivityDto>> GetUserActivityAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<PagedResult<LoginHistoryDto>> GetUserLoginHistoryAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<UserStatisticsDto> GetUserStatisticsAsync(
        CancellationToken cancellationToken);
}
