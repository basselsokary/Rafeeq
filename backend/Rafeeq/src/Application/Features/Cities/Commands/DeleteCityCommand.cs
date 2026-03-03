using FluentValidation;

namespace Application.Features.Cities.Commands;

public record DeleteCityCommand() : ICommand;

internal class DeleteCityCommandHandler : ICommandHandler<DeleteCityCommand>
{
    public Task<Result> HandleAsync(DeleteCityCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class DeleteCityCommandValidator : AbstractValidator<DeleteCityCommand>
{
    public DeleteCityCommandValidator()
    {
        throw new NotImplementedException();
    }
}
