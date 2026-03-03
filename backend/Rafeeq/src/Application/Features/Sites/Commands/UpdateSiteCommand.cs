using FluentValidation;

namespace Application.Features.Sites.Commands;

public record UpdateSiteCommand() : ICommand;

internal class UpdateSiteCommandHandler : ICommandHandler<UpdateSiteCommand>
{
    public Task<Result> HandleAsync(UpdateSiteCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class UpdateSiteCommandValidator : AbstractValidator<UpdateSiteCommand>
{
    public UpdateSiteCommandValidator()
    {
        throw new NotImplementedException();
    }
}
