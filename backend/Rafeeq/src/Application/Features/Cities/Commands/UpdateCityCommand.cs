using FluentValidation;

namespace Application.Features.Cities.Commands;

public record UpdateCityCommand() : ICommand;

internal class UpdateCityCommandHandler : ICommandHandler<UpdateCityCommand>
{
    public Task<Result> HandleAsync(UpdateCityCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
{
    public UpdateCityCommandValidator()
    {
        throw new NotImplementedException();
    }
}
