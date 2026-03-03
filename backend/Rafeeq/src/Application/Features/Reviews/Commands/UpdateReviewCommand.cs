using FluentValidation;

namespace Application.Features.Reviews.Commands;

public record UpdateReviewCommand() : ICommand;

internal class UpdateReviewCommandHandler : ICommandHandler<UpdateReviewCommand>
{
    public Task<Result> HandleAsync(UpdateReviewCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        throw new NotImplementedException();
    }
}
