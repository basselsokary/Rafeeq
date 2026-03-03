using FluentValidation;

namespace Application.Features.Attractions.Commands;

public record CreateAttractionCommand() : ICommand;

internal class CreateAttractionCommandHandler : ICommandHandler<CreateAttractionCommand>
{
    public Task<Result> HandleAsync(CreateAttractionCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class CreateAttractionCommandValidator : AbstractValidator<CreateAttractionCommand>
{
    public CreateAttractionCommandValidator()
    {
        throw new NotImplementedException();
    }
}
