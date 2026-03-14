using Application.Common.Interfaces.Authentication;
using Shared.Extensions;

namespace Application.Commands.Users;

public record RefreshCommand(
    string AccessToken,
    string RefreshToken) : ICommand<RefreshResponse>;

public class RefreshCommandHandler(
    IIdentityService identityService) : ICommandHandler<RefreshCommand, RefreshResponse>
{
    public async Task<Result<RefreshResponse>> HandleAsync(RefreshCommand command, CancellationToken cancellationToken)
    {
        var authenticationResult = await identityService.RefreshTokenAsync(
            command.AccessToken,
            command.RefreshToken);
        
        
        if (authenticationResult.Failed)
            return authenticationResult.To<RefreshResponse>();

        return new RefreshResponse(authenticationResult.AccessToken!, authenticationResult.RefreshToken!);
    }
}

public record RefreshResponse(string Token, string RefreshToken);