using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sites.OpeningHours;

public record AddSiteOpeningHoursCommand(
    Guid Id,
    List<AddSiteOpeningHoursDtoCommand> OpeningHours) : ICommand;

public record AddSiteOpeningHoursDtoCommand(
    DayOfWeek DayOfWeek,
    TimeRange OpeningTime,
    bool IsClosed);

internal class AddSiteOpeningHoursCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteOpeningHoursCommand>
{
    public async Task<Result> HandleAsync(AddSiteOpeningHoursCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithOpeningHoursAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        foreach (var openingHour in command.OpeningHours)
        {
            Result result = site.AddOpeningHours(openingHour.DayOfWeek, openingHour.OpeningTime, openingHour.IsClosed);
            if (result.Failed)
                return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
