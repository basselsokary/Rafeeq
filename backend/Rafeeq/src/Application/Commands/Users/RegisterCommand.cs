using Application.Common.Interfaces.Authentication;
using Domain.Enums;

namespace Application.Commands.Users;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    LanguageCode PreferredLanguage) : ICommand;

public class RegisterCommandHandler(
    IIdentityService identityService) : ICommandHandler<RegisterCommand>
{
    public async Task<Result> HandleAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.RegisterAsync(
            command.Email,
            command.Password,
            command.FirstName,
            command.LastName);

        return result;
    }
}
