using Application.Common.Interfaces.Authentication;
using Application.Common.Models;
using Domain.Common.Constants;
using Infrastructure.Authentication;
using Infrastructure.Identity.Entities;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;

namespace Infrastructure.Identity;

internal class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    JwtTokenGenerator jwtTokenGenerator,
    IOptions<JwtOptions> jwtSettings,
    ApplicationDbContext appContext) : IIdentityService
{
    private readonly JwtOptions _jwtSettings = jwtSettings.Value;

    public async Task<Result<string>> RegisterAsync(Guid userId, string userName, string email, string role, string password)
    {
        var touristResult = await RegisterCoreAsync(userId, userName, email, role, password);
        if (touristResult.Failed)
            return touristResult.Error;

        var resetToken = await userManager.GenerateEmailConfirmationTokenAsync(touristResult.Value);
        return Result.Success(resetToken);
    }

    public async Task<Result> RegisterAsync(Guid userId, string userName, string email, string role)
    {
        return await RegisterCoreAsync(userId, userName, email, role, password: null);
    }

    private async Task<Result<TouristUser>> RegisterCoreAsync(Guid userId, string userName, string email, string role, string? password)
    {
        var touristResult = TouristUser.Create(userId, userName, email);
        if (touristResult.Failed)
            return touristResult;

        string normalizedUserName = userManager.NormalizeName(userName);
        if (await userManager.Users.AnyAsync(u => u.NormalizedUserName == normalizedUserName))
            return ApplicationUserErrors.UserNameAlreadyInUse;

        string normalizedEmail = userManager.NormalizeEmail(email);
        if (await userManager.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail))
            return ApplicationUserErrors.EmailAlreadyInUse;

        TouristUser tourist = touristResult.Value;

        var result = password is not null
            ? await userManager.CreateAsync(tourist, password)
            : await userManager.CreateAsync(tourist);

        if (!result.Succeeded)
            return ValidationError.FromErrors(result.Errors.Select(e => Error.Validation(e.Code, e.Description)));

        var identityResult = await userManager.AddToRoleAsync(tourist, role);
        if (!identityResult.Succeeded)
        {
            await userManager.DeleteAsync(tourist);
            return ValidationError.FromErrors(identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }

        return Result.Success(tourist);
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password)
    {
        return await LoginCoreAsync(email, password);
    }

    public async Task<AuthenticationResult> LoginAsync(string email)
    {
        return await LoginCoreAsync(email, password: null);
    }

    private async Task<AuthenticationResult> LoginCoreAsync(string email, string? password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return AuthenticationResult.Failure(ApplicationUserErrors.InvalidCredentials);

        if (!user.IsActive)
            return AuthenticationResult.Failure(ApplicationUserErrors.InActiveUser);

        // if (user.MustChangePassword)
        //     return AuthenticationResult.Failure(ApplicationUserErrors.MustChangePassword);

        if (password is not null)
        {
            // if (!await userManager.IsEmailConfirmedAsync(user))
            // {
            //     return AuthenticationResult.Failure(ApplicationUserErrors.EmailNotConfirmed);
            // }

            var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            if (!signInResult.Succeeded)
            {
                return AuthenticationResult.Failure(signInResult.IsLockedOut
                    ? ApplicationUserErrors.LockedAccount
                    : ApplicationUserErrors.InvalidCredentials);
            }
        }

        var roles = await userManager.GetRolesAsync(user);
        bool isAdmin = IsAdmin(roles);
        if (isAdmin)
        {
            return AuthenticationResult.Failure(ApplicationUserErrors.InvalidCredentials);
        }

        user.RecordLastLogin();
        await userManager.UpdateAsync(user);

        var accessToken = jwtTokenGenerator.GenerateAccessToken(user, roles);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        var saveTokenResult = await SaveRefreshTokenAsync(user.Id, refreshToken, isAdmin);
        if (saveTokenResult.Failed)
            return AuthenticationResult.Failure(saveTokenResult.Error);

        return AuthenticationResult.Success(
            accessToken,
            refreshToken,
            isAdmin ? _jwtSettings.AccessTokenExpirationForAdminInMinutes : _jwtSettings.AccessTokenExpirationInMinutes,
            isAdmin ? _jwtSettings.RefreshTokenExpirationForAdminInHours : _jwtSettings.RefreshTokenExpirationInHours,
            user.Id);
    }

    public async Task<AuthenticationResult> AdminLoginAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return AuthenticationResult.Failure(ApplicationUserErrors.InvalidCredentials);

        if (!user.IsActive)
            return AuthenticationResult.Failure(ApplicationUserErrors.InActiveUser);

        // if (user.MustChangePassword)
        //     return AuthenticationResult.Failure(ApplicationUserErrors.MustChangePassword);

        var roles = await userManager.GetRolesAsync(user);
        if (!IsAdmin(roles))
            return AuthenticationResult.Failure(ApplicationUserErrors.InvalidCredentials);

        if (password is not null)
        {
            var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            if (!signInResult.Succeeded)
            {
                return AuthenticationResult.Failure(signInResult.IsLockedOut
                    ? ApplicationUserErrors.LockedAccount
                    : ApplicationUserErrors.InvalidCredentials);
            }
        }

        user.RecordLastLogin();
        await userManager.UpdateAsync(user);

        var accessToken = jwtTokenGenerator.GenerateAccessToken(user, roles);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        var saveTokenResult = await SaveRefreshTokenAsync(user.Id, refreshToken, true);
        if (saveTokenResult.Failed)
            return AuthenticationResult.Failure(saveTokenResult.Error);

        return AuthenticationResult.Success(
            accessToken,
            refreshToken,
            _jwtSettings.AccessTokenExpirationForAdminInMinutes,
            _jwtSettings.RefreshTokenExpirationForAdminInHours,
            user.Id);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await appContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedRefreshToken == null || !storedRefreshToken.IsActive)
        {
            return AuthenticationResult.Failure(ApplicationUserErrors.InvalidRefreshToken);
        }

        var user = await userManager.FindByIdAsync(storedRefreshToken.UserId.ToString());
        if (user == null || !user.IsActive)
        {
            return AuthenticationResult.Failure(ApplicationUserErrors.InActiveUser);
        }

        // Revoke old refresh token
        storedRefreshToken.Revoke();

        var roles = await userManager.GetRolesAsync(user);
        bool isAdmin = IsAdmin(roles);

        var newAccessToken = jwtTokenGenerator.GenerateAccessToken(user, roles);
        var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();

        await appContext.SaveChangesAsync();

        await SaveRefreshTokenAsync(user.Id, newRefreshToken, isAdmin);

        return AuthenticationResult.Success(
            newAccessToken,
            newRefreshToken,
            isAdmin ? _jwtSettings.AccessTokenExpirationForAdminInMinutes : _jwtSettings.AccessTokenExpirationInMinutes,
            isAdmin ? _jwtSettings.RefreshTokenExpirationForAdminInHours : _jwtSettings.RefreshTokenExpirationInHours,
            user.Id);
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

    private static bool IsAdmin(IList<string> roles)
    {
        return !roles.Contains(UserRoles.Tourist, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Result> SaveRefreshTokenAsync(Guid userId, string token, bool isAdmin = false)
    {
        var refreshTokenResult = RefreshToken.Create(
            token,
            userId,
            isAdmin  
                ? DateTime.UtcNow.AddHours(_jwtSettings.RefreshTokenExpirationForAdminInHours)
                : DateTime.UtcNow.AddHours(_jwtSettings.RefreshTokenExpirationInHours)
        );

        if (refreshTokenResult.Failed)
            return refreshTokenResult;

        appContext.RefreshTokens.Add(refreshTokenResult.Value);
        await appContext.SaveChangesAsync();
        
        return Result.Success();
    }
}
