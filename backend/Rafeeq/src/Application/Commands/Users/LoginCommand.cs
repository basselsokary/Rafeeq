using Application.Common.Interfaces.Authentication;

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
        Result<LoginResponse> result = await identityService.SignInAsync(
            command.Email,
            command.Password,
            command.RememberMe);

        if (result.Failed)
            return result;
        
        return result.Value;
    }
}

public record LoginResponse(string Token, string RefreshToken);