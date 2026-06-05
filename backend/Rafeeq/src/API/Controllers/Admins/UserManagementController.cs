using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Queries.Users;
using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.Commands.Users.Admins;
using Domain.Enums;
using Infrastructure.Authorization;

namespace API.Controllers.Admins;

[Route("api/admins/users")]
[Authorize(Policy = Policies.CanManageUsers)]
public class UserManagementController : ApiBaseController
{
    #region  USER LISTING & SEARCH
    /// <summary>
    /// Get paginated list of all users with filtering and search capabilities
    /// </summary>
    /// <param name="query">Filter and pagination parameters</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<UserListDto>>> GetUsers(
        [FromServices] IQueryHandler<GetUsersQuery, PagedResult<UserListDto>> handler,
        string? earchTerm = null,
        bool? emailVerified = null,
        string? role = null,
        UserStatus? status = null,
        string? sortBy = null,
        string? sortOrder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var paging = new PagingParameters(page, pageSize);
        var query = new GetUsersQuery(paging, earchTerm, role, emailVerified, sortBy, sortOrder, status);
        var result = await handler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get detailed information about a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Detailed user information</returns>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> GetUserById(
        Guid userId,
        [FromServices] IQueryHandler<GetUserByIdQuery, UserDetailsDto> handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(userId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Search users by email or name
    /// </summary>
    /// <param name="searchTerm">Search term (email or name)</param>
    /// <param name="limit">Maximum number of results (default: 10)</param>
    /// <returns>List of matching users</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<UserSearchResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserSearchResultDto>>> SearchUsers(
        [FromQuery] SearchUsersQuery query,
        [FromServices] IQueryHandler<SearchUsersQuery, List<UserSearchResultDto>> handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }
    #endregion

    /// <summary>
    /// Create a new moderator account
    /// </summary>
    /// <param name="command">Moderator creation details</param>
    /// <returns>Created moderator details</returns>
    [HttpPost("moderators")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDetailsDto>> CreateModerator(
        [FromBody] CreateModeratorCommand command,
        [FromServices] ICommandHandler<CreateModeratorCommand, UserDetailsDto> handler,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Promote a moderator to admin role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Updated user details</returns>
    [HttpPost("{userId:guid}/promote-to-admin")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> PromoteToAdmin(
        Guid userId,
        [FromServices] ICommandHandler<PromoteToAdminCommand, UserDetailsDto> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new PromoteToAdminCommand(userId);
        var result = await handler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Demote an admin to moderator role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Updated user details</returns>
    [HttpPost("{userId:guid}/demote-to-moderator")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> DemoteToModerator(
        Guid userId,
        [FromServices] ICommandHandler<DemoteToModeratorCommand, UserDetailsDto> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new DemoteToModeratorCommand(userId);
        var result = await handler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    public record UpdateUserRolesRequest(List<string> Roles, string? Reason);

    /// <summary>
    /// Update user roles
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="command">Role update details</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{userId:guid}/roles")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> UpdateUserRoles(
        Guid userId,
        [FromBody] UpdateUserRolesRequest request,
        [FromServices] ICommandHandler<UpdateUserRolesCommand, UserDetailsDto> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserRolesCommand(userId, request.Roles, request.Reason);
        var result = await handler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Lock a user account (prevent login)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="command">Lockout details including reason and duration</param>
    /// <returns>Success response</returns>
    [HttpPost("{userId:guid}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LockUserAccount(
        Guid userId,
        [FromBody] LockUserAccountCommand command,
        [FromServices] ICommandHandler<LockUserAccountCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var request = new LockUserAccountCommand(userId, command.Reason, command.LockUntil);
        var result = await handler.HandleAsync(request, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Unlock a user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success response</returns>
    [HttpPost("{userId:guid}/unlock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnlockUserAccount(
        Guid userId,
        [FromServices] ICommandHandler<UnlockUserAccountCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UnlockUserAccountCommand(userId);
        var result = await handler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    public record SuspendUserAccountRequest(string Reason, DateTime SuspendUntil, bool NotifyUser = true);

    /// <summary>
    /// Suspend a user account temporarily
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="command">Suspension details</param>
    /// <returns>Success response</returns>
    [HttpPost("{userId:guid}/suspend")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SuspendUserAccount(
        Guid userId,
        [FromBody] SuspendUserAccountRequest request,
        [FromServices] ICommandHandler<SuspendUserAccountCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new SuspendUserAccountCommand(userId, request.Reason, request.SuspendUntil, request.NotifyUser);
        var result = await handler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Reactivate a suspended user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success response</returns>
    [HttpPost("{userId:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReactivateUserAccount(
        Guid userId,
        [FromServices] ICommandHandler<ReactivateUserAccountCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new ReactivateUserAccountCommand(userId);
        var result = await handler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Reset a user's password (admin-initiated)
    /// Sends password reset email to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success response</returns>
    [HttpPost("{userId:guid}/reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PasswordResetResultDto>> ResetUserPassword(
        Guid userId,
        [FromServices] ICommandHandler<ResetUserPasswordCommand, PasswordResetResultDto> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new ResetUserPasswordCommand(userId);
        var result = await handler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Force password change on next login
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success response</returns>
    [HttpPost("{userId:guid}/require-password-change")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequirePasswordChange(
        Guid userId,
        [FromServices] ICommandHandler<RequirePasswordChangeCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new RequirePasswordChangeCommand(userId);
        var result = await handler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Manually confirm a user's email (bypass verification)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success response</returns>
    [HttpPost("{userId:guid}/confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmUserEmail(
        Guid userId,
        [FromServices] ICommandHandler<ConfirmUserEmailCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new ConfirmUserEmailCommand(userId);
        var result = await handler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Resend email verification link
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success response</returns>
    [HttpPost("{userId:guid}/resend-verification-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmailSentResultDto>> ResendVerificationEmail(
        Guid userId,
        [FromServices] ICommandHandler<ResendVerificationEmailCommand, EmailSentResultDto> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new ResendVerificationEmailCommand(userId);
        var result = await handler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get user activity log
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <returns>Paginated activity log</returns>
    [HttpGet("{userId:guid}/activity")]
    [ProducesResponseType(typeof(PagedResult<UserActivityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<UserActivityDto>>> GetUserActivity(
        Guid userId,
        [FromServices] IQueryHandler<GetUserActivityQuery, PagedResult<UserActivityDto>> handler,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserActivityQuery(userId, page, pageSize);
        var result = await handler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get user login history
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <returns>Paginated login history</returns>
    [HttpGet("{userId:guid}/login-history")]
    [ProducesResponseType(typeof(PagedResult<LoginHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<LoginHistoryDto>>> GetUserLoginHistory(
        Guid userId,
        [FromServices] IQueryHandler<GetUserLoginHistoryQuery, PagedResult<LoginHistoryDto>> handler,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserLoginHistoryQuery(userId, page, pageSize);
        var result = await handler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Soft delete a user account (marks as deleted, retains data)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="command">Deletion details including reason</param>
    /// <returns>Success response</returns>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(
        Guid userId,
        [FromBody] DeleteUserCommand command,
        [FromServices] ICommandHandler<DeleteUserCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteUserCommand(userId, command.Reason);
        var result = await handler.HandleAsync(request, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Permanently delete a user account (GDPR compliance)
    /// WARNING: This action is irreversible and removes all user data
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="command">Permanent deletion confirmation</param>
    /// <returns>Success response</returns>
    [HttpDelete("{userId:guid}/permanent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PermanentlyDeleteUser(
        Guid userId,
        [FromBody] PermanentlyDeleteUserCommand command,
        [FromServices] ICommandHandler<PermanentlyDeleteUserCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var request = new PermanentlyDeleteUserCommand(userId, command.Reason, command.ConfirmDeletion);
        var result = await handler.HandleAsync(request, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get user statistics dashboard
    /// </summary>
    /// <returns>User statistics including counts by role, status, etc.</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(UserStatisticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserStatisticsDto>> GetUserStatistics(
        [FromServices] IQueryHandler<GetUserStatisticsQuery, UserStatisticsDto> handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserStatisticsQuery();
        var result = await handler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }
}
