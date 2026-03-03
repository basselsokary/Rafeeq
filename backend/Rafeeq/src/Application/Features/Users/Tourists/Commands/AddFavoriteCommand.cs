using FluentValidation;

namespace Application.Features.Users.Tourists.Commands;

public record AddFavoriteCommand() : ICommand;

internal class AddFavoriteCommandHandler : ICommandHandler<AddFavoriteCommand>
{
    public Task<Result> HandleAsync(AddFavoriteCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class AddFavoriteCommandValidator : AbstractValidator<AddFavoriteCommand>
{
    public AddFavoriteCommandValidator()
    {
        throw new NotImplementedException();
    }
}
