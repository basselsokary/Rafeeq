using FluentValidation;

namespace Application.Features.Sites.Commands;

public record DeleteSiteCommand() : ICommand;

internal class DeleteSiteCommandHandler : ICommandHandler<DeleteSiteCommand>
{
    public Task<Result> HandleAsync(DeleteSiteCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class DeleteSiteCommandValidator : AbstractValidator<DeleteSiteCommand>
{
    public DeleteSiteCommandValidator()
    {
        throw new NotImplementedException();
    }
}
