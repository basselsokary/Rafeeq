using Application.Common.Interfaces.Authentication;
using Application.Common.Models;
using Domain.Entities.UserAggregate;
using Domain.Enums;
using Infrastructure.Persistence.IdentityContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IdentityDbContext _identityContext;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IdentityDbContext identityContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _identityContext = identityContext;
    }

    public async Task<Result> RegisterAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        UserRole role = UserRole.Tourist)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return Result.Failure(UserErrors.EmailAlreadyInUse);
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = false,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ValidationError.FromErrors(result.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }

        // Assign default role
        await _userManager.AddToRoleAsync(user, role.ToString());
        return Result.Success();
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return AuthenticationResult.Failure(UserErrors.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return AuthenticationResult.Failure(UserErrors.InActiveUser);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                return AuthenticationResult.Failure(UserErrors.LockedAccount);
            }

            return AuthenticationResult.Failure(UserErrors.InvalidCredentials);
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        // Save refresh token
        await SaveRefreshTokenAsync(user.Id, refreshToken, ipAddress: string.Empty);

        return AuthenticationResult.Success(accessToken, refreshToken, user.Id);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return AuthenticationResult.Failure(UserErrors.InvalidToken);
        }

        var userIdClaim = principal.FindFirst("userId")?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return AuthenticationResult.Failure(UserErrors.InvalidClaims);
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || !user.IsActive)
        {
            return AuthenticationResult.Failure(UserErrors.InActiveUser);
        }

        var storedRefreshToken = await _identityContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (storedRefreshToken == null || !storedRefreshToken.IsActive)
        {
            return AuthenticationResult.Failure(UserErrors.InvalidRefreshToken);
        }

        // Revoke old refresh token
        storedRefreshToken.IsRevoked = true;
        storedRefreshToken.RevokedAt = DateTime.UtcNow;

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        // Save new refresh token
        storedRefreshToken.ReplacedByToken = newRefreshToken;
        await _identityContext.SaveChangesAsync();

        await SaveRefreshTokenAsync(user.Id, newRefreshToken, ipAddress: string.Empty);

        return AuthenticationResult.Success(newAccessToken, newRefreshToken, user.Id);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var token = await _identityContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null || !token.IsActive)
        {
            return false;
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await _identityContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<bool> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    private async Task SaveRefreshTokenAsync(Guid userId, string token, string ipAddress)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ipAddress,
            IsRevoked = false
        };

        _identityContext.RefreshTokens.Add(refreshToken);
        await _identityContext.SaveChangesAsync();
    }
}
