using Application.Common.Interfaces.Services;
using Domain.Enums;
using Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Shared;

namespace Infrastructure.Services;

internal class AdminService(
    UserManager<ApplicationUser> userManager) : IAdminService
{
    public async Task<Result> AddModeratorAsync(
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
            firstName,
            lastName,
            fullName,
            email);

        if (moderatorResult.Failed)
            return Result.Failure(moderatorResult.Error);

        var moderator = moderatorResult.Value;
        var identityResult = await userManager.CreateAsync(moderator, password);

        if (!identityResult.Succeeded)
            return ValidationError.FromErrors(identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        
        identityResult = await userManager.AddToRoleAsync(moderator, moderator.Role.ToString());
        if (!identityResult.Succeeded)
        {
            // Rollback user creation if role assignment fails
            await userManager.DeleteAsync(moderator);
            return ValidationError.FromErrors(identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }

        return Result.Success();
    }

    public async Task<Result> AssignRoleAsync(Guid userId, UserRole role)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return ApplicationUserErrors.NotFound(userId);
        
        var identityResult = await userManager.AddToRoleAsync(user, user.Role.ToString());
        if (!identityResult.Succeeded)
        {
            return ValidationError.FromErrors(identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }

        return Result.Success();
    }

    public async Task<Result> RemoveRoleAsync(Guid userId, UserRole role)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return ApplicationUserErrors.NotFound(userId);
        
        var identityResult = await userManager.RemoveFromRoleAsync(user, user.Role.ToString());
        if (!identityResult.Succeeded)
        {
            return ValidationError.FromErrors(identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }

        return Result.Success();
    }
}
