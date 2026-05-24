using Application.Common.Interfaces.Authentication;

namespace Application.Commands.Auth;

public sealed record RefreshCommand(
    string RefreshToken) : ICommand<RefreshResponse>;

public sealed class RefreshCommandHandler(
    IIdentityService identityService) : ICommandHandler<RefreshCommand, RefreshResponse>
{
    public async Task<Result<RefreshResponse>> HandleAsync(RefreshCommand command, CancellationToken cancellationToken)
    {
        var authenticationResult = await identityService.RefreshTokenAsync(
            command.RefreshToken);
        
        if (authenticationResult.Failed)
            return authenticationResult.To<RefreshResponse>();

        return new RefreshResponse(
            authenticationResult.AccessToken,
            authenticationResult.RefreshToken,
            authenticationResult.AccessTokenExpirationInMinutes,
            authenticationResult.RefreshTokenExpirationInHours);
    }
}

public sealed record RefreshResponse(
    string AccessToken,
    string RefreshToken,
    int AccessTokenExpirationInMinutes,
    int RefreshTokenExpirationInHours);
