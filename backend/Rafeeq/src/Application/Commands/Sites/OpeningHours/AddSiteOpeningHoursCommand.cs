using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites.OpeningHours;

public sealed record AddSiteOpeningHoursCommand(
    Guid Id,
    List<AddSiteOpeningHoursDtoCommand> OpeningHours) : ICommand;

public sealed record AddSiteOpeningHoursDtoCommand(
    WeekDay Day,
    TimeSpan StartTime,
    TimeSpan EndTime,
    bool IsClosed);

internal sealed class AddSiteOpeningHoursCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<AddSiteOpeningHoursCommand>
{
    public async Task<Result> HandleAsync(AddSiteOpeningHoursCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithOpeningHoursAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        foreach (var openingHour in command.OpeningHours)
        {
            var timeRange = TimeRange.Create(openingHour.StartTime, openingHour.EndTime);
            if (timeRange.Failed)
                return timeRange;

            Result<OpeningHour> result = site.AddOpeningHour(openingHour.Day, timeRange.Value, openingHour.IsClosed);
            if (result.Failed)
                return result;
            
            // await unitOfWork.AddAsync(result.Value, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
