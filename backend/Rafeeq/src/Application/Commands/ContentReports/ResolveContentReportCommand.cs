using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Messaging;
using Domain.Common.Interfaces;
using Domain.Entities.ContentReportAggregate;
using Domain.Enums;

namespace Application.Commands.ContentReports;

public record ResolveContentReportCommand(
    Guid Id,
    string? Reason,
    ModerationAction? Action,
    string? Notes = null) : ICommand;

internal class ResolveContentReportCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<ResolveContentReportCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserContext _userContext = userContext;

    public async Task<Result> HandleAsync(ResolveContentReportCommand command, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.ContentReports.GetByIdAsync(command.Id, cancellationToken);
        if (content == null)
            return ContentReportErrors.NotFound(command.Id);
        
        if (command.Reason != null)
        {
            var contentResult = content.Dismiss(_userContext.Id, command.Reason);
            if (contentResult.Failed)
                return contentResult;
        } else if (command.Action != null)
        {
            var contentResult = content.Solve(_userContext.Id, command.Action.Value, command.Notes);
            if (contentResult.Failed)
                return contentResult;
        }
        else
        {
            return ContentReportErrors.CannotBeResolved;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}