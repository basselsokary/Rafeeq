using Application.Common.Interfaces.Authentication;
using Domain.Common.Constants;
using Domain.Common.Interfaces;
using Domain.Entities.ReviewAggregate;

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

        if (!userContext.IsInAnyRole(UserRoles.Admin, UserRoles.Moderator) && review.TouristId != userContext.Id)
            return ReviewErrors.Unauthorized;

        review.Delete();

        await unitOfWork.Reviews.DeleteAsync(review, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

