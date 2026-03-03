using FluentValidation;

namespace Application.Features.Cities.Commands;

public record CreateCityCommand() : ICommand;

internal class CreateCityCommandHandler : ICommandHandler<CreateCityCommand>
{
    public Task<Result> HandleAsync(CreateCityCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
        throw new NotImplementedException();
    }
}
