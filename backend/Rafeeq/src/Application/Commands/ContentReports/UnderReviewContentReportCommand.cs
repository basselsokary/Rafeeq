using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Messaging;
using Domain.Common.Interfaces;
using Domain.Entities.ContentReportAggregate;

namespace Application.Commands.ContentReports;

public record UnderReviewContentReportCommand(Guid Id) : ICommand;

internal class UnderReviewContentReportCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<UnderReviewContentReportCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserContext _userContext = userContext;

    public async Task<Result> HandleAsync(UnderReviewContentReportCommand command, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.ContentReports.GetByIdAsync(command.Id, cancellationToken);
        if (content == null)
            return ContentReportErrors.NotFound(command.Id);
        
        var contentResult = content.MarkAsUnderReview(_userContext.Id);
        if (contentResult.Failed)
            return contentResult;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}