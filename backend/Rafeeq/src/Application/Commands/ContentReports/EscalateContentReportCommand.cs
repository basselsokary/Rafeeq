using Domain.Common.Interfaces;
using Domain.Entities.ContentReportAggregate;

namespace Application.Commands.ContentReports;

public sealed record EscalateContentReportCommand(Guid Id) : ICommand;

internal sealed class EscalateContentReportCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<EscalateContentReportCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> HandleAsync(EscalateContentReportCommand command, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.ContentReports.GetByIdAsync(command.Id, cancellationToken);
        if (content == null)
            return ContentReportErrors.NotFound(command.Id);
        
        content.Escalate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}