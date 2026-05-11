using System.Security.Claims;
using Application.Common.Interfaces.Authentication;
using Application.Common.Models;
using Domain.Enums;
using Infrastructure.Identity.Entities;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Infrastructure.Identity;

internal class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    JwtTokenGenerator jwtTokenGenerator,
    ApplicationDbContext appContext) : IIdentityService
{
    public async Task<Result> RegisterAsync(
        Guid userId,
        string userName,
        string email,
        string password)
    {
        var touristResult = TouristUser.Create(userId, userName, email);
        if (touristResult.Failed)
            return touristResult;

        string normalizedUserName = userManager.NormalizeName(userName);
        var existingUser = await userManager.Users.AnyAsync(u => u.NormalizedUserName == normalizedUserName);
        if (existingUser)
            return ApplicationUserErrors.UserNameAlreadyInUse;

        string normalizedEmail = userManager.NormalizeEmail(email);
        existingUser = await userManager.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail);
        if (existingUser)
            return ApplicationUserErrors.EmailAlreadyInUse;

        TouristUser tourist = touristResult.Value;

        var result = await userManager.CreateAsync(tourist, password);
        if (!result.Succeeded)
        {
            return ValidationError.FromErrors(result.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }
        
        var identityResult = await userManager.AddToRoleAsync(tourist, tourist.Role.ToString());
        if (!identityResult.Succeeded)
        {
            // Rollback user creation if role assignment fails
            await userManager.DeleteAsync(tourist);
            return ValidationError.FromErrors(identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }
        
        return Result.Success();
    }

    public async Task<Result> RegisterAsync(
        Guid userId,
        string userName,
        string email)
    {
        var touristResult = TouristUser.Create(userId, userName, email);
        if (touristResult.Failed)
            return touristResult;

        string normalizedUserName = userManager.NormalizeName(userName);
        var existingUser = await userManager.Users.AnyAsync(u => u.NormalizedUserName == normalizedUserName);
        if (existingUser)
            return ApplicationUserErrors.UserNameAlreadyInUse;

        string normalizedEmail = userManager.NormalizeEmail(email);
        existingUser = await userManager.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail);
        if (existingUser)
            return ApplicationUserErrors.EmailAlreadyInUse;

        TouristUser tourist = touristResult.Value;

        var result = await userManager.CreateAsync(tourist);
        if (!result.Succeeded)
        {
            return ValidationError.FromErrors(result.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }
        
        var identityResult = await userManager.AddToRoleAsync(tourist, tourist.Role.ToString());
        if (!identityResult.Succeeded)
        {
            // Rollback user creation if role assignment fails
            await userManager.DeleteAsync(tourist);
            return ValidationError.FromErrors(identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }
        
        return Result.Success();
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return AuthenticationResult.Failure(ApplicationUserErrors.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return AuthenticationResult.Failure(ApplicationUserErrors.InActiveUser);
        }

        // if (!await userManager.IsEmailConfirmedAsync(user))
        // {
        //     return AuthenticationResult.Failure(ApplicationUserErrors.EmailNotConfirmed);
        // }

        var result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                return AuthenticationResult.Failure(ApplicationUserErrors.LockedAccount);
            }

            return AuthenticationResult.Failure(ApplicationUserErrors.InvalidCredentials);
        }

        // Update last login
        user.RecordLastLogin();
        await userManager.UpdateAsync(user);

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtTokenGenerator.GenerateAccessToken(user, roles);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        // Save refresh token
        Result saveTokenResult = await SaveRefreshTokenAsync(user.Id, refreshToken, roles);
        if (saveTokenResult.Failed)
        {
            return AuthenticationResult.Failure(saveTokenResult.Error);
        }

        return AuthenticationResult.Success(accessToken, refreshToken, jwtTokenGenerator.GetAccessTokenExpiryInHours, jwtTokenGenerator.GetRefreshTokenExpiryInDays, user.Id);
    }

    public async Task<AuthenticationResult> LoginAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return AuthenticationResult.Failure(ApplicationUserErrors.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return AuthenticationResult.Failure(ApplicationUserErrors.InActiveUser);
        }

        // Update last login
        user.RecordLastLogin();
        await userManager.UpdateAsync(user);

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtTokenGenerator.GenerateAccessToken(user, roles);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        // Save refresh token
        Result saveTokenResult = await SaveRefreshTokenAsync(user.Id, refreshToken, roles);
        if (saveTokenResult.Failed)
        {
            return AuthenticationResult.Failure(saveTokenResult.Error);
        }

        return AuthenticationResult.Success(accessToken, refreshToken, jwtTokenGenerator.GetAccessTokenExpiryInHours, jwtTokenGenerator.GetRefreshTokenExpiryInDays, user.Id);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = jwtTokenGenerator.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return AuthenticationResult.Failure(ApplicationUserErrors.InvalidToken);
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return AuthenticationResult.Failure(ApplicationUserErrors.InvalidClaims);
        }

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null || !user.IsActive)
        {
            return AuthenticationResult.Failure(ApplicationUserErrors.InActiveUser);
        }

        var storedRefreshToken = await appContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (storedRefreshToken == null || !storedRefreshToken.IsActive)
        {
            return AuthenticationResult.Failure(ApplicationUserErrors.InvalidRefreshToken);
        }

        // Revoke old refresh token
        storedRefreshToken.Revoke();

        var roles = await userManager.GetRolesAsync(user);
        var newAccessToken = jwtTokenGenerator.GenerateAccessToken(user, roles);
        var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();

        await appContext.SaveChangesAsync();

        await SaveRefreshTokenAsync(user.Id, newRefreshToken, roles);

        return AuthenticationResult.Success(newAccessToken, newRefreshToken, jwtTokenGenerator.GetAccessTokenExpiryInHours, jwtTokenGenerator.GetRefreshTokenExpiryInDays, user.Id);
    }

    public async Task<Result> RevokeTokenAsync(string refreshToken)
    {
        var token = await appContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null || !token.IsActive)
        {
            return ApplicationUserErrors.InvalidRefreshToken;
        }

        token.Revoke();

        await appContext.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return ApplicationUserErrors.NotFound(userId);
        }

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            return ValidationError.FromErrors(result.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string token, string email, string newPassword)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return ApplicationUserErrors.NotFound(email);
        }

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            return ValidationError.FromErrors(result.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }

        return Result.Success();
    }

    public async Task<Result<(string ResetToken, string UserName)>> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return ApplicationUserErrors.NotFound(email);
        }

        return (await userManager.GeneratePasswordResetTokenAsync(user), user.UserName!);
    }

    public async Task<Result> ConfirmEmailAsync(string token, string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return ApplicationUserErrors.NotFound(email);
        }

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return ValidationError.FromErrors(result.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }

        return Result.Success();
    }

    public async Task<Result<(string ResetToken, string UserName)>> GenerateEmailConfirmationTokenAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return ApplicationUserErrors.NotFound(email);
        }

        return (await userManager.GenerateEmailConfirmationTokenAsync(user), user.UserName!);
    }

    public async Task<bool> IsUserExist(string email, CancellationToken cancellationToken)
    {
        string normalizedEmail = userManager.NormalizeEmail(email);
        var existingUser = await userManager.Users.AnyAsync(
            u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        
        return existingUser;
    }

    private async Task<Result> SaveRefreshTokenAsync(Guid userId, string token, IEnumerable<string> roles)
    {
        var refreshTokenResult = RefreshToken.Create(
            token,
            userId,
            roles.Any(r => r.Equals(UserRole.Tourist.ToString(), StringComparison.OrdinalIgnoreCase)) 
                ? DateTime.UtcNow.AddDays(jwtTokenGenerator.GetRefreshTokenExpiryInDays)
                : DateTime.UtcNow.AddHours(jwtTokenGenerator.GetRefreshTokenExpirationForAdminInHours)
        );

        if (refreshTokenResult.Failed)
            return refreshTokenResult;

        appContext.RefreshTokens.Add(refreshTokenResult.Value);
        await appContext.SaveChangesAsync();
        
        return Result.Success();
    }
}
