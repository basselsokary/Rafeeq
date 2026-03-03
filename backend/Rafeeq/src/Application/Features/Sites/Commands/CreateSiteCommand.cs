using FluentValidation;

namespace Application.Features.Sites.Commands;

public record CreateSiteCommand() : ICommand;

internal class CreateSiteCommandHandler : ICommandHandler<CreateSiteCommand>
{
    public Task<Result> HandleAsync(CreateSiteCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class CreateSiteCommandValidator : AbstractValidator<CreateSiteCommand>
{
    public CreateSiteCommandValidator()
    {
        throw new NotImplementedException();
    }
}
