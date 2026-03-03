using FluentValidation;

namespace Application.Features.Reviews.Commands;

public record CreateReviewCommand() : ICommand;

internal class CreateReviewCommandHandler : ICommandHandler<CreateReviewCommand>
{
    public Task<Result> HandleAsync(CreateReviewCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        throw new NotImplementedException();
    }
}
