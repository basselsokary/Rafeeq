using FluentValidation;

namespace Application.Features.Attractions.Commands;

public record DeleteAttractionCommand() : ICommand;

internal class DeleteAttractionCommandHandler : ICommandHandler<DeleteAttractionCommand>
{
    public Task<Result> HandleAsync(DeleteAttractionCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class DeleteAttractionCommandValidator : AbstractValidator<DeleteAttractionCommand>
{
    public DeleteAttractionCommandValidator()
    {
        throw new NotImplementedException();
    }
}
