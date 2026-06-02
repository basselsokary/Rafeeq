using Application.Commands.Users.Admins;
using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Common.Constants;
using Domain.Enums;
using Infrastructure.Identity.Entities;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Infrastructure.Services;

internal sealed class UserManagementService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager) : IUserManagementService
{
    public async Task<PagedResult<UserListDto>> GetUsersAsync(
        PagingParameters paging,
        string? searchTerm,
        string? role,
        bool? emailVerified,
        UserStatus status,
        CancellationToken cancellationToken)
    {
        var query = ApplyFilters(context.Users.AsNoTracking(), searchTerm, role: role, emailVerified, status);

        var totalCount = await query.CountAsync(cancellationToken);
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        var items = await MapUserListAsync(users, cancellationToken);

        return new PagedResult<UserListDto>(
            items,
            totalCount,
            paging.Page,
            paging.PageSize);
    }

    public async Task<UserDetailsDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return null;

        var dto = await MapUserDetailsAsync(user, cancellationToken);
        return dto;
    }

    public async Task<List<UserSearchResultDto>> SearchUsersAsync(
        string searchTerm,
        int limit,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return [];

        var query = ApplyFilters(context.Users.AsNoTracking(), searchTerm, role: null, emailVerified: null, status: null);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var results = await MapUserSearchAsync(users, cancellationToken);
        return results;
    }

    public async Task<Result<UserDetailsDto>> CreateModeratorAsync(
        string userName,
        string firstName,
        string lastName,
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var moderatorResult = ModeratorUser.Create(
            Guid.NewGuid(),
            userName,
            email,
            firstName,
            lastName,
            fullName);

        if (moderatorResult.Failed)
            return Result.Failure<UserDetailsDto>(moderatorResult.Error);

        var moderator = moderatorResult.Value;
        var identityResult = await userManager.CreateAsync(moderator, password);

        if (!identityResult.Succeeded)
            return ValidationError.FromErrors(identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        
        identityResult = await userManager.AddToRoleAsync(moderator, UserRoles.Moderator);
        if (!identityResult.Succeeded)
        {
            // Rollback user creation if role assignment fails
            await userManager.DeleteAsync(moderator);
            return ValidationError.FromErrors(identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }

        var dto = await MapUserDetailsAsync(moderator, cancellationToken);
        return Result.Success(dto);
    }

    public async Task<Result<UserDetailsDto>> PromoteToAdminAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure<UserDetailsDto>(ApplicationUserErrors.NotFound(userId));

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Contains(UserRoles.Admin))
            return Result.Failure<UserDetailsDto>(ApplicationUserErrors.AlreadyAdmin);

        if (!currentRoles.Contains(UserRoles.Moderator))
            return Result.Failure<UserDetailsDto>(ApplicationUserErrors.NotModerator);

        await userManager.RemoveFromRoleAsync(user, UserRoles.Moderator);
        await userManager.AddToRoleAsync(user, UserRoles.Admin);

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return Result.Failure<UserDetailsDto>(ApplicationUserErrors.PromotionFailed(errors));
        }

        var dto = await MapUserDetailsAsync(user, cancellationToken);
        return Result.Success(dto);
    }

    public async Task<Result<UserDetailsDto>> DemoteToModeratorAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure<UserDetailsDto>(ApplicationUserErrors.NotFound(userId));

        var currentRoles = await userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(UserRoles.Admin))
            return Result.Failure<UserDetailsDto>(ApplicationUserErrors.NotAdmin);

        await userManager.RemoveFromRoleAsync(user, UserRoles.Admin);
        await userManager.AddToRoleAsync(user, UserRoles.Moderator);

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return Result.Failure<UserDetailsDto>(ApplicationUserErrors.DemotionFailed(errors));
        }

        var dto = await MapUserDetailsAsync(user, cancellationToken);
        return Result.Success(dto);
    }

    public async Task<Result<UserDetailsDto>> UpdateUserRolesAsync(
        Guid userId,
        List<string> roles,
        string? reason,
        CancellationToken cancellationToken)
    {
        if (roles.Count == 0)
            return Result.Failure<UserDetailsDto>(ApplicationUserErrors.RolesRequired);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure<UserDetailsDto>(ApplicationUserErrors.NotFound(userId));

        var currentRoles = await userManager.GetRolesAsync(user);
        var toRemove = currentRoles.Except(roles, StringComparer.OrdinalIgnoreCase).ToList();
        var toAdd = roles.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToList();

        if (toRemove.Count > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, toRemove);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                return Result.Failure<UserDetailsDto>(ApplicationUserErrors.RemoveRolesFailed(errors));
            }
        }

        if (toAdd.Count > 0)
        {
            var addResult = await userManager.AddToRolesAsync(user, toAdd);
            if (!addResult.Succeeded)
            {
                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                return Result.Failure<UserDetailsDto>(ApplicationUserErrors.AddRolesFailed(errors));
            }
        }

        var dto = await MapUserDetailsAsync(user, cancellationToken);
        return Result.Success(dto);
    }

    public async Task<Result> LockUserAccountAsync(
        Guid userId,
        string reason,
        DateTime? lockUntil,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure(ApplicationUserErrors.NotFound(userId));

        var roles = await userManager.GetRolesAsync(user);
        
        if (roles.Contains(UserRoles.Admin))
            return Result.Failure(ApplicationUserErrors.AdminDeleteNotAllowed);
        if (roles.Contains(UserRoles.SuperAdmin))
            return Result.Failure(ApplicationUserErrors.AdminDeleteNotAllowed);

        var lockoutEnd = lockUntil.HasValue
            ? new DateTimeOffset(lockUntil.Value, TimeSpan.Zero)
            : DateTimeOffset.MaxValue;

        var result = await userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure(ApplicationUserErrors.LockFailed(errors));
        }

        await userManager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result> UnlockUserAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure(ApplicationUserErrors.NotFound(userId));

        var result = await userManager.SetLockoutEndDateAsync(user, null);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure(ApplicationUserErrors.UnlockFailed(errors));
        }

        await userManager.ResetAccessFailedCountAsync(user);
        await userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result> SuspendUserAccountAsync(
        Guid userId,
        DateTime suspendUntil,
        string reason,
        bool notifyUser,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure(ApplicationUserErrors.NotFound(userId));

        var roles = await userManager.GetRolesAsync(user);
        
        if (roles.Contains(UserRoles.SuperAdmin))
            return Result.Failure(ApplicationUserErrors.AdminDeleteNotAllowed);

        user.SuspendAccount();

        var lockoutEnd = new DateTimeOffset(suspendUntil, TimeSpan.Zero);
        var lockResult = await userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        if (!lockResult.Succeeded)
        {
            var errors = string.Join(", ", lockResult.Errors.Select(e => e.Description));
            return Result.Failure(ApplicationUserErrors.SuspendFailed(errors));
        }

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return Result.Failure(ApplicationUserErrors.SuspendFailed(errors));
        }

        return Result.Success();
    }

    public async Task<Result> ReactivateUserAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure(ApplicationUserErrors.NotFound(userId));

        user.Activate();

        var lockResult = await userManager.SetLockoutEndDateAsync(user, null);
        if (!lockResult.Succeeded)
        {
            var errors = string.Join(", ", lockResult.Errors.Select(e => e.Description));
            return Result.Failure(ApplicationUserErrors.ReactivateFailed(errors));
        }

        await userManager.ResetAccessFailedCountAsync(user);
        await userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result<PasswordResetResultDto>> ResetUserPasswordAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure<PasswordResetResultDto>(ApplicationUserErrors.NotFound(userId));

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var result = new PasswordResetResultDto(user.Email!, user.UserName!, token);
        return Result.Success(result);
    }

    public async Task<Result> RequirePasswordChangeAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure(ApplicationUserErrors.NotFound(userId));

        user.RequirePasswordChange();
        await userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result> ConfirmUserEmailAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure(ApplicationUserErrors.NotFound(userId));

        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result<EmailSentResultDto>> ResendVerificationEmailAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure<EmailSentResultDto>(ApplicationUserErrors.NotFound(userId));

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var result = new EmailSentResultDto(user.Email!, user.UserName!, token);
        return Result.Success(result);
    }

    public async Task<Result> DeleteUserAsync(
        Guid userId,
        string reason,
        bool notifyUser,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure(ApplicationUserErrors.NotFound(userId));

        var roles = await userManager.GetRolesAsync(user);
        
        if (roles.Contains(UserRoles.Admin))
            return Result.Failure(ApplicationUserErrors.AdminDeleteNotAllowed);
        if (roles.Contains(UserRoles.SuperAdmin))
            return Result.Failure(ApplicationUserErrors.AdminDeleteNotAllowed);
        
        user.MarkDeleted();

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure(ApplicationUserErrors.DeleteFailed(errors));
        }

        return Result.Success();
    }

    public async Task<Result> PermanentlyDeleteUserAsync(
        Guid userId,
        string reason,
        bool confirmDeletion,
        CancellationToken cancellationToken)
    {
        if (!confirmDeletion)
            return Result.Failure(ApplicationUserErrors.DeletionConfirmationRequired);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Failure(ApplicationUserErrors.NotFound(userId));

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains(UserRoles.Admin))
            return Result.Failure(ApplicationUserErrors.AdminDeleteNotAllowed);
        if (roles.Contains(UserRoles.SuperAdmin))
            return Result.Failure(ApplicationUserErrors.AdminDeleteNotAllowed);

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure(ApplicationUserErrors.PermanentDeleteFailed(errors));
        }

        return Result.Success();
    }

    public Task<PagedResult<UserActivityDto>> GetUserActivityAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var empty = new PagedResult<UserActivityDto>(new List<UserActivityDto>(), page, pageSize, 0);
        return Task.FromResult(empty);
    }

    public Task<PagedResult<LoginHistoryDto>> GetUserLoginHistoryAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var empty = new PagedResult<LoginHistoryDto>(new List<LoginHistoryDto>(), page, pageSize, 0);
        return Task.FromResult(empty);
    }

    public async Task<UserStatisticsDto> GetUserStatisticsAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var last7Days = now.AddDays(-7);
        var last30Days = now.AddDays(-30);
        var last24Hours = now.AddHours(-24);

        // Single round-trip: all scalar aggregates in one query
        var stats = await context.Users
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total              = g.Count(),
                Active             = g.Count(u => u.Status == UserStatus.Active && u.DeletedAt == null),
                Locked             = g.Count(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow),
                Suspended          = g.Count(u => u.Status == UserStatus.Suspended),
                Deleted            = g.Count(u => u.DeletedAt != null || u.Status == UserStatus.Deleted),
                CreatedToday       = g.Count(u => u.CreatedAt.Date == today),
                CreatedLast7Days   = g.Count(u => u.CreatedAt >= last7Days),
                CreatedLast30Days  = g.Count(u => u.CreatedAt >= last30Days),
                ActiveLast24Hours  = g.Count(u => u.LastLoginAt.HasValue && u.LastLoginAt.Value >= last24Hours),
                ActiveLast7Days    = g.Count(u => u.LastLoginAt.HasValue && u.LastLoginAt.Value >= last7Days),
                ActiveLast30Days   = g.Count(u => u.LastLoginAt.HasValue && u.LastLoginAt.Value >= last30Days),
                UnconfirmedEmails  = g.Count(u => !u.EmailConfirmed),
                MustChangePassword = g.Count(u => u.MustChangePassword),
                TwoFactorEnabled   = g.Count(u => u.TwoFactorEnabled),
            })
            .OrderByDescending(x => x.CreatedToday) // just to have a deterministic order
            .FirstOrDefaultAsync(cancellationToken);

        // Second round-trip: daily breakdown for the chart (still all in SQL)
        var dailyCounts = await context.Users
            .AsNoTracking()
            .Where(u => u.CreatedAt >= last30Days)
            .GroupBy(u => u.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Date, x => x.Count, cancellationToken);

        // Fill in days with zero registrations (pure C#, no DB needed)
        var dailyBreakdown = Enumerable.Range(0, 30)
            .Select(offset => today.AddDays(-29 + offset))           // oldest → newest
            .Select(date => new DailyUserCountDto(
                date,
                dailyCounts.GetValueOrDefault(date, 0)))
            .ToList();

        return new UserStatisticsDto(
            stats?.Total             ?? 0,
            stats?.Active            ?? 0,
            stats?.Locked            ?? 0,
            stats?.Suspended         ?? 0,
            stats?.Deleted           ?? 0,
            new UsersGrowthDto(
                stats?.CreatedToday      ?? 0,
                stats?.CreatedLast7Days  ?? 0,
                stats?.CreatedLast30Days ?? 0,
                dailyBreakdown),
            new UserActivityStatsDto(
                stats?.ActiveLast24Hours ?? 0,
                stats?.ActiveLast7Days   ?? 0,
                stats?.ActiveLast30Days  ?? 0),
            stats?.UnconfirmedEmails     ?? 0,
            stats?.MustChangePassword    ?? 0,
            stats?.TwoFactorEnabled      ?? 0);
    }

    private IQueryable<ApplicationUser> ApplyFilters(
        IQueryable<ApplicationUser> query,
        string? searchTerm,
        string? role,
        bool? emailVerified,
        UserStatus? status)
    {
        if (status.HasValue)
            query = query.Where(u => u.Status == status.Value);

        if (emailVerified.HasValue)
            query = query.Where(u => u.EmailConfirmed == emailVerified.Value);

        if (!string.IsNullOrWhiteSpace(role) && UserRoles.AllRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            query = query.Where(u => context.UserRoles
                .Any(ur => ur.UserId == u.Id &&
                        context.Roles.Any(r => r.Id == ur.RoleId && r.NormalizedName == role.ToUpper())));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();

            var adminIds = context.AdminsUsers.AsNoTracking()
                .Where(u => EF.Functions.Like(u.FullName, $"%{term}%"))
                .Select(u => u.Id);

            var moderatorIds = context.ModeratorUsers.AsNoTracking()
                .Where(u => EF.Functions.Like(u.FullName, $"%{term}%"))
                .Select(u => u.Id);

            var touristIds = context.Tourists.AsNoTracking()
                .Where(t => EF.Functions.Like(t.FirstName, $"%{term}%") ||
                            EF.Functions.Like(t.LastName, $"%{term}%"))
                .Select(t => t.Id);

            query = query.Where(u =>
                EF.Functions.Like(u.Email!, $"%{term}%") ||
                EF.Functions.Like(u.UserName!, $"%{term}%") ||
                adminIds.Contains(u.Id) ||
                moderatorIds.Contains(u.Id) ||
                touristIds.Contains(u.Id));
        }

        return query;
    }

    private async Task<List<UserListDto>> MapUserListAsync(
        List<ApplicationUser> users,
        CancellationToken cancellationToken)
    {
        if (users.Count == 0)
            return [];

        var touristIds = users.OfType<TouristUser>().Select(u => u.Id).ToList();
        var touristMap = await context.Tourists.AsNoTracking()
            .Where(t => touristIds.Contains(t.Id))
            .Select(t => new TouristName(t.Id, t.FirstName, t.LastName))
            .ToDictionaryAsync(t => t.Id, cancellationToken);

        var list = new List<UserListDto>(users.Count);
        foreach (var user in users)
        {
            var (firstName, lastName) = GetNames(user, touristMap);

            list.Add(new UserListDto(
                user.Id,
                user.Email ?? string.Empty,
                firstName,
                lastName,
                FormatStatus(user),
                user.EmailConfirmed,
                user.PhoneNumberConfirmed,
                user.CreatedAt,
                user.LastLoginAt,
                user.LockoutEnd?.UtcDateTime));
        }

        return list;
    }

    private async Task<List<UserSearchResultDto>> MapUserSearchAsync(
        List<ApplicationUser> users,
        CancellationToken cancellationToken)
    {
        if (users.Count == 0)
            return [];

        var touristIds = users.OfType<TouristUser>().Select(u => u.Id).ToList();
        var touristMap = await context.Tourists.AsNoTracking()
            .Where(t => touristIds.Contains(t.Id))
            .Select(t => new TouristName(t.Id, t.FirstName, t.LastName))
            .ToDictionaryAsync(t => t.Id, cancellationToken);

        var list = new List<UserSearchResultDto>(users.Count);
        foreach (var user in users)
        {
            var (firstName, lastName) = GetNames(user, touristMap);

            list.Add(new UserSearchResultDto(
                user.Id,
                user.Email ?? string.Empty,
                firstName,
                lastName,
                FormatStatus(user)));
        }

        return list;
    }

    private async Task<UserDetailsDto> MapUserDetailsAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var roles = await userManager.GetRolesAsync(user);
        var (firstName, lastName) = await GetNamesAsync(user, cancellationToken);

        string? nationality = null;
        TouristDetailsDto? touristDetails = null;

        if (user is TouristUser)
        {
            var tourist = await context.Tourists.AsNoTracking()
                .Where(t => t.Id == user.Id)
                .Select(t => new { t.TotalRatings, t.TotalTrips, t.Nationality })
                .FirstOrDefaultAsync(cancellationToken);

            if (tourist != null)
            {
                nationality = tourist.Nationality;
                touristDetails = new TouristDetailsDto(tourist.TotalTrips, tourist.TotalRatings);
            }
        }

        return new UserDetailsDto(
            user.Id,
            user.Email ?? string.Empty,
            user.EmailConfirmed,
            user.PhoneNumber,
            user.PhoneNumberConfirmed,
            firstName,
            lastName,
            nationality,
            roles.ToList(),
            FormatStatus(user),
            user.TwoFactorEnabled,
            user.LockoutEnabled,
            user.AccessFailedCount,
            user.LockoutEnd?.UtcDateTime,
            user.MustChangePassword,
            user.LastPasswordChangedAt,
            user.CreatedAt,
            user.LastLoginAt ?? user.CreatedAt,
            user.LastLoginAt,
            user.DeletedAt,
            touristDetails);
    }

    private async Task<(string? FirstName, string? LastName)> GetNamesAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        if (user is StaffUser staff)
            return (staff.FirstName, staff.LastName);

        if (user is TouristUser)
        {
            var tourist = await context.Tourists.AsNoTracking()
                .Select(t => new { t.Id, t.FirstName, t.LastName })
                .FirstOrDefaultAsync(t => t.Id == user.Id, cancellationToken);

            if (tourist != null)
                return (tourist.FirstName, tourist.LastName);
        }

        return (null, null);
    }

    private static (string? FirstName, string? LastName) GetNames(
        ApplicationUser user,
        IReadOnlyDictionary<Guid, TouristName> touristMap)
    {
        if (user is StaffUser staff)
            return (staff.FirstName, staff.LastName);

        if (touristMap.TryGetValue(user.Id, out var tourist))
            return (tourist.FirstName, tourist.LastName);

        return (null, null);
    }

    private static string FormatStatus(ApplicationUser user)
    {
        if (user.DeletedAt.HasValue)
            return "Deleted";

        if (user.IsLocked)
            return "Locked";

        return user.Status.ToString();
    }

    private sealed record TouristName(Guid Id, string FirstName, string LastName);
}
