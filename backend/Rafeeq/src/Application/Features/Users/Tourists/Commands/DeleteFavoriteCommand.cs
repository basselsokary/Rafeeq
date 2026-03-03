using FluentValidation;

namespace Application.Features.Users.Tourists.Commands;

public record DeleteFavoriteCommand() : ICommand;

internal class DeleteFavoriteCommandHandler : ICommandHandler<DeleteFavoriteCommand>
{
    public Task<Result> HandleAsync(DeleteFavoriteCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class DeleteFavoriteCommandValidator : AbstractValidator<DeleteFavoriteCommand>
{
    public DeleteFavoriteCommandValidator()
    {
        throw new NotImplementedException();
    }
}
