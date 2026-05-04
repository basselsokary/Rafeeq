using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.ReviewAggregate;
using Domain.Enums;

namespace Application.Commands.Reviews;

public sealed record DeleteReviewCommand(Guid Id) : ICommand;

internal sealed class DeleteReviewCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<DeleteReviewCommand>
{
    public async Task<Result> HandleAsync(DeleteReviewCommand command, CancellationToken cancellationToken)
    {
        var review = await unitOfWork.Reviews.GetByIdAsync(command.Id, cancellationToken);
        if (review == null)
            return ReviewErrors.NotFound(command.Id);

        if (!userContext.IsInAnyRole(UserRole.Admin, UserRole.Moderator) && review.TouristId != userContext.Id)
            return ReviewErrors.Unauthorized;

        review.Delete();

        await unitOfWork.Reviews.DeleteAsync(review, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

