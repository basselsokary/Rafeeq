using Domain.Common;

namespace Domain.Entities.SiteAggregate;

public class SiteCreatedEvent(Guid siteId) : BaseEvent
{
    public Guid SiteId { get; } = siteId;
}

public class SiteUpdatedEvent(Guid siteId) : BaseEvent
{
    public Guid SiteId { get; } = siteId;
}

public class SiteDeletedEvent(Guid siteId) : BaseEvent
{
    public Guid SiteId { get; } = siteId;
}

public class SiteImageUpdatedEvent(Guid siteId) : BaseEvent
{
    public Guid SiteId { get; } = siteId;
}

public class SiteLocalizedContentUpdatedEvent(Guid siteId) : BaseEvent
{
    public Guid SiteId { get; } = siteId;
}
// public class SiteStatusChangedEvent(Guid SiteId, SiteStatus NewStatus) : BaseEvent
// {
//     public Guid SiteId { get; } = SiteId;
//     public SiteStatus NewStatus { get; } = NewStatus;
// }