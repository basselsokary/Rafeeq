using Application.Common.Models;

namespace Application.Common.Interfaces.Authentication;

public interface IIdentityService
{
    Task<Result> RegisterAsync(
        Guid userId,
        string userName,
        string email,
        string password);
    Task<Result> RegisterAsync(
        Guid userId,
        string userName,
        string email);
    Task<AuthenticationResult> LoginAsync(string email, string password);
    Task<AuthenticationResult> LoginAsync(string email);
    Task<AuthenticationResult> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<Result> RevokeTokenAsync(string refreshToken);
    Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<Result> ResetPasswordAsync(string token, string email, string newPassword);
    Task<Result<(string ResetToken, string UserName)>> GeneratePasswordResetTokenAsync(string email);
    Task<Result> ConfirmEmailAsync(string token, string email);
    Task<Result<(string ResetToken, string UserName)>> GenerateEmailConfirmationTokenAsync(string email);
    Task<bool> IsUserExist(string email, CancellationToken cancellationToken);
    // Task<UserDto?> GetUserDtoByEmailAsync(string email);
}
