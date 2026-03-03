using Domain.Enums;
using FluentValidation;

namespace Application.Features.Users.Commands;

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

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        throw new NotImplementedException();
    }
}
