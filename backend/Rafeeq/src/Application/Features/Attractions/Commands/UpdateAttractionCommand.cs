using FluentValidation;

namespace Application.Features.Attractions.Commands;

public record UpdateAttractionCommand() : ICommand;

internal class UpdateAttractionCommandHandler : ICommandHandler<UpdateAttractionCommand>
{
    public Task<Result> HandleAsync(UpdateAttractionCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class UpdateAttractionCommandValidator : AbstractValidator<UpdateAttractionCommand>
{
    public UpdateAttractionCommandValidator()
    {
        throw new NotImplementedException();
    }
}
