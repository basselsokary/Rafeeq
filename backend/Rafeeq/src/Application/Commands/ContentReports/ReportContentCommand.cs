using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.ContentReportAggregate;
using Domain.Enums;

namespace Application.Commands.ContentReports;

public sealed record ReportContentCommand(
    Guid ReportedEntityId,
    ReportReason Reason,
    string Description) : ICommand;

internal sealed class ReportContentCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<ReportContentCommand>
{
    public async Task<Result> HandleAsync(ReportContentCommand command, CancellationToken cancellationToken)
    {
        var contentResult = ContentReport.Create(
            userContext.Id,
            command.ReportedEntityId,
            command.Reason,
            command.Description);

        if (contentResult.Failed)
            return contentResult;

        await unitOfWork.ContentReports.AddAsync(contentResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
