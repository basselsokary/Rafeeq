using FluentValidation;

namespace Application.Features.Users.Tourists.Commands;

public record UpdateProfileCommand() : ICommand;

internal class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand>
{
    public Task<Result> HandleAsync(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        throw new NotImplementedException();
    }
}
