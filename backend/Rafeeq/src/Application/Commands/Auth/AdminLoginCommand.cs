using Application.Common.Interfaces.Authentication;

namespace Application.Commands.Auth;

public sealed record AdminLoginCommand(
    string Email,
    string Password) : ICommand<AdminLoginResponse>;

public sealed class AdminLoginCommandHandler(
    IIdentityService identityService) : ICommandHandler<AdminLoginCommand, AdminLoginResponse>
{
    public async Task<Result<AdminLoginResponse>> HandleAsync(AdminLoginCommand command, CancellationToken cancellationToken)
    {
        var authenticationResult = await identityService.AdminLoginAsync(
            command.Email,
            command.Password);

        if (authenticationResult.Failed)
            return authenticationResult.To<AdminLoginResponse>();

        return new AdminLoginResponse(
            authenticationResult.AccessToken,
            authenticationResult.RefreshToken,
            authenticationResult.AccessTokenExpirationInMinutes,
            authenticationResult.RefreshTokenExpirationInHours);
    }
}

public sealed record AdminLoginResponse(
    string AccessToken,
    string RefreshToken,
    int AccessTokenExpirationInMinutes,
    int RefreshTokenExpirationInHours);