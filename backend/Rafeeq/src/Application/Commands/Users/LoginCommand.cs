using Application.Common.Interfaces.Authentication;

namespace Application.Commands.Users;

public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<LoginResponse>;

public sealed class LoginCommandHandler(
    IIdentityService identityService) : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var authenticationResult = await identityService.LoginAsync(
            command.Email,
            command.Password);

        if (authenticationResult.Failed)
            return authenticationResult.To<LoginResponse>();

        return new LoginResponse(
            authenticationResult.AccessToken,
            authenticationResult.RefreshToken,
            authenticationResult.AccessTokenExpirationInMinutes,
            authenticationResult.RefreshTokenExpirationInHours);
    }
}

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int AccessTokenExpirationInMinutes,
    int RefreshTokenExpirationInHours);