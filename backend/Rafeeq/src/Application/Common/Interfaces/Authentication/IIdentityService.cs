using Application.Common.Models;
using Domain.Enums;

namespace Application.Common.Interfaces.Authentication;

public interface IIdentityService
{
    Task<Result> RegisterAsync(string email, string password, string firstName, string lastName, UserRole role = UserRole.Tourist);
    Task<AuthenticationResult> LoginAsync(string email, string password);
    Task<AuthenticationResult> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    Task<string> GeneratePasswordResetTokenAsync(string email);
    Task<bool> ConfirmEmailAsync(Guid userId, string token);
    Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);
}
