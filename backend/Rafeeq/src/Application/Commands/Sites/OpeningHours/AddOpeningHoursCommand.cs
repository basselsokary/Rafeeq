using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sites.OpeningHours;

public record AddSiteOpeningHoursCommand(
    Guid Id,
    DayOfWeek DayOfWeek,
    TimeRange OpeningTime,
    bool IsClosed) : ICommand;

internal class AddSiteOpeningHoursCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteOpeningHoursCommand>
{
    public async Task<Result> HandleAsync(AddSiteOpeningHoursCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        Result result = site.AddOpeningHours(command.DayOfWeek, command.OpeningTime, command.IsClosed);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
