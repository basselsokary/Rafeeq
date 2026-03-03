using FluentValidation;

namespace Application.Features.Reviews.Commands;

public record DeleteReviewCommand() : ICommand;

internal class DeleteReviewCommandHandler : ICommandHandler<DeleteReviewCommand>
{
    public Task<Result> HandleAsync(DeleteReviewCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        throw new NotImplementedException();
    }
}
