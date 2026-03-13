using Application.Commands.Users;

namespace Application.Common.Interfaces.Authentication;

public interface IIdentityService
{
    // Task<UserDto?> GetUserDtoByEmailAsync(string email);
    // Task<UserDto?> GetUserDtoByIdAsync(string userId);
    
    Task<Result<LoginResponse>> SignInAsync(string email, string password, bool rememberMe = false, bool lockoutOnFailure = false);
    Task<Result<RefreshResponse>> SignInAsync(string email, string password);
    Task<Result> SignOutAsync();

    Task<(Result Result, string CustomerId)> CreateCustomerAsync(RegisterCommand registerCommand);
    
    Task<bool> CheckPasswordAsync(string email, string password);
    Task<Result> CheckUserExistsAsync(string userName, string email);

    Task<Result> DeleteUserAsync(string userId);

    Task<string?> GetRolesAsync(string userId);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AddRoleToUserAsync(string userId, string role);
}
