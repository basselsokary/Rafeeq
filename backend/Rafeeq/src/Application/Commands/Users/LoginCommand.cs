using Application.Common.Interfaces.Authentication;
using Shared.Extensions;

namespace Application.Commands.Users;

public record LoginCommand(
    string Email,
    string Password,
    bool RememberMe = false) : ICommand<LoginResponse>;

public class LoginCommandHandler(
    IIdentityService identityService) : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var authenticationResult = await identityService.LoginAsync(
            command.Email,
            command.Password,
            command.RememberMe);

        if (authenticationResult.Failed)
            return authenticationResult.To<LoginResponse>();

        return new LoginResponse(authenticationResult.AccessToken!, authenticationResult.RefreshToken!);
    }
}

public record LoginResponse(string Token, string RefreshToken);