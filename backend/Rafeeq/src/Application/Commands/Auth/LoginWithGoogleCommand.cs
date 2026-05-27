using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.Services;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;

namespace Application.Commands.Auth;

public record LoginWithGoogleCommand(string IdToken) : ICommand<LoginWithGoogleResponse>;

internal sealed class LoginWithGoogleCommandHandler(
    IExternalIdentityService externalIdentityService,
    IIdentityService identityService,
    IUnitOfWork unitOfWork,
    IUserCredentialService emailGeneratorService,
    IErrorLocalizer errorLocalizer) : ICommandHandler<LoginWithGoogleCommand, LoginWithGoogleResponse>
{
    public async Task<Result<LoginWithGoogleResponse>> HandleAsync(LoginWithGoogleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var email = await externalIdentityService.ValidateGoogleToken(command.IdToken);
            if (email == null)
            {
                return Result.Failure<LoginWithGoogleResponse>(UserErrors.InvalidGoogleIdToken);
            }

            if (!await identityService.IsUserExist(email, cancellationToken))
            {
                var registrationResult = await RegisterUser(email, cancellationToken);
                if (registrationResult.Failed)
                    return registrationResult.Error;
            }

            var authenticationResult = await identityService.LoginAsync(email);
            if (authenticationResult.Failed)
                return authenticationResult.Error;
            
            return new LoginWithGoogleResponse(
                authenticationResult.AccessToken,
                authenticationResult.RefreshToken,
                authenticationResult.AccessTokenExpirationInMinutes,
                authenticationResult.RefreshTokenExpirationInHours);
        }
        catch
        {
            return Result.Failure<LoginWithGoogleResponse>(errorLocalizer[Error.None.Code]);
        }
    }

    private async Task<Result> RegisterUser(string email, CancellationToken cancellationToken)
    {
        var firstName = ExtractUsernameFromEmail(email);
        var lastName = GenerateRandomNumber(10, 99).ToString();
        Guid userId = Guid.NewGuid();

        var userResult = Tourist.Create(
            userId,
            firstName,
            lastName);

        if (userResult.Failed)
            return userResult;

        var userName = await emailGeneratorService.GenerateUniqueUsernameAsync(firstName, lastName, cancellationToken);
        
        return await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var result = await identityService.RegisterAsync(
                userId,
                userName,
                email,
                UserRoles.Tourist);

            if (result.Failed)
                return result;

            await unitOfWork.Tourists.AddAsync(userResult.Value, cancellationToken);

            return Result.Success();

        }, cancellationToken);
    }

    private static string ExtractUsernameFromEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        return atIndex > 0 ? email.Substring(0, atIndex) : email;
    }

    private static int GenerateRandomNumber(int v1, int v2)
    {
        var random = new Random();
        return random.Next(v1, v2);
    }
}

public sealed record LoginWithGoogleResponse(string AccessToken, string RefreshToken, int AccessTokenExpiresAtInHours, int RefreshTokenExpiresAtInDays);