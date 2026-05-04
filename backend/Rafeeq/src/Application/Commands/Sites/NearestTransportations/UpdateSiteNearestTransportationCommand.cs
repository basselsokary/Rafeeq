using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;

namespace Application.Commands.Sites.NearestTransportations;

public sealed record UpdateSiteNearestTransportationCommand(
    Guid SiteId,
    Guid TransportationId,
    TimeSpan StartOperatingHoursTime,
    TimeSpan EndOperatingHoursTime,
    bool IsOperational,
    bool HasAccessibility) : ICommand;

internal sealed class UpdateSiteNearestTransportationCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSiteNearestTransportationCommand>
{
    public async Task<Result> HandleAsync(UpdateSiteNearestTransportationCommand command, CancellationToken cancellationToken = default)
    {
        var timeResult = TimeRange.Create(command.StartOperatingHoursTime, command.EndOperatingHoursTime);
        if (timeResult.Failed)
            return timeResult;

        var site = await unitOfWork.Sites.GetWithNearestTransportationAsync(command.SiteId, cancellationToken);
        if (site == null)
            return SiteErrors.NotFound(command.SiteId);
        
        var transportation = site.NearestTransportations.FirstOrDefault(t => t.Id == command.TransportationId);
        if (transportation is null)
            return SiteErrors.TransportationNotFound;

        transportation.SetOperatingHours(timeResult.Value);
        transportation.SetOperationalStatus(command.IsOperational);
        transportation.SetAccessibility(command.HasAccessibility);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
        
    }
}
