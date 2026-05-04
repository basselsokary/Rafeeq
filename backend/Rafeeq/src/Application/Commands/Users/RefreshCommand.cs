using Application.Common.Interfaces.Authentication;

namespace Application.Commands.Users;

public sealed record RefreshCommand(
    string AccessToken,
    string RefreshToken) : ICommand<RefreshResponse>;

public sealed class RefreshCommandHandler(
    IIdentityService identityService) : ICommandHandler<RefreshCommand, RefreshResponse>
{
    public async Task<Result<RefreshResponse>> HandleAsync(RefreshCommand command, CancellationToken cancellationToken)
    {
        var authenticationResult = await identityService.RefreshTokenAsync(
            command.AccessToken,
            command.RefreshToken);
        
        if (authenticationResult.Failed)
            return authenticationResult.To<RefreshResponse>();

        return new RefreshResponse(
            authenticationResult.AccessToken,
            authenticationResult.RefreshToken,
            authenticationResult.AccessTokenExpiresAtInHours,
            authenticationResult.RefreshTokenExpiresAtInDays);
    }
}

public sealed record RefreshResponse(string AccessToken, string RefreshToken, int AccessTokenExpiresAtInHours, int RefreshTokenExpiresAtInDays);