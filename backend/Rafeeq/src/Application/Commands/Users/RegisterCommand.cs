using Domain.Enums;

namespace Application.Commands.Users;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    LanguageCode PreferredLanguage) : ICommand;

public class RegisterCommandHandler() : ICommandHandler<RegisterCommand>
{
    public async Task<Result> HandleAsync(RegisterCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
