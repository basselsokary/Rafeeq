using Application.Common.Models;
using Domain.Enums;

namespace Application.Common.Interfaces.Authentication;

public interface IIdentityService
{
    Task<Result> RegisterAsync(
        Guid userId,
        string userName,
        string email,
        string password,
        UserRole role = UserRole.Tourist);
    Task<AuthenticationResult> LoginAsync(string email, string password, bool RememberMe = false);
    Task<AuthenticationResult> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<Result> RevokeTokenAsync(string refreshToken);
    Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<Result> ResetPasswordAsync(string id, string token, string newPassword);
    Task<Result<(string ResetToken, string UserName)>> GeneratePasswordResetTokenAsync(string email);
    Task<Result> ConfirmEmailAsync(Guid userId, string token);
    Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId);
}