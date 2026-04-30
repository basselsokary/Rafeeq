using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;

namespace Application.Commands.Sites.OpeningHours;

public sealed record RemoveSiteOpeningHoursCommand(
    Guid Id,
    List<WeekDay> Days) : ICommand;

internal sealed class RemoveSiteOpeningHoursCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSiteOpeningHoursCommand>
{
    public async Task<Result> HandleAsync(RemoveSiteOpeningHoursCommand command, CancellationToken cancellationToken)
    {
        var site = await unitOfWork.Sites.GetWithOpeningHoursAsync(command.Id, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.Id);

        foreach (var day in command.Days)
        {
            Result result = site.RemoveOpeningHour(day);
            if (result.Failed)
                return result;    
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
