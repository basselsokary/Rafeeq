using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.ContentReportAggregate;
using Domain.Enums;

namespace Application.Commands.ContentReports;

public sealed record ResolveContentReportCommand(
    Guid Id,
    ModerationAction Action,
    string Reason) : ICommand;

internal sealed class ResolveContentReportCommandHandler(
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
        
        if (command.Action == ModerationAction.NoAction)
        {
            var contentResult = content.Dismiss(_userContext.Id, command.Reason);
            if (contentResult.Failed)
                return contentResult;
        } else
        {
            var contentResult = content.Solve(_userContext.Id, command.Action, command.Reason);
            if (contentResult.Failed)
                return contentResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}