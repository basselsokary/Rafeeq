using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.SponsorAggregate;

public class SponsorCreatedEvent(Guid SponsorId, string Name, SponsorType Type) : BaseEvent
{
    public Guid SponsorId { get; } = SponsorId;
    public string Name { get; } = Name;
    public SponsorType Type { get; } = Type;
}

public class SponsorUpdatedEvent(Guid SponsorId) : BaseEvent
{
    public Guid SponsorId { get; } = SponsorId;
}

public class SponsorDeletedEvent(Guid sponsorId) : BaseEvent
{
    public Guid SponsorId { get; } = sponsorId;
}

public class SponsorTierChangedEvent(Guid SponsorId, SponsorTier NewTier) : BaseEvent
{
    public Guid SponsorId { get; } = SponsorId;
    public SponsorTier NewTier { get; } = NewTier;
}

public class SponsorImageUpdatedEvent(Guid SponsorId) : BaseEvent
{
    public Guid SponsorId { get; } = SponsorId;
}

public class SponsorLocalizedContentUpdatedEvent(Guid SponsorId) : BaseEvent
{
    public Guid SponsorId { get; } = SponsorId;
}

public class SponsorOfferChangedEvent(Guid SponsorId, Guid OfferId) : BaseEvent
{
    public Guid SponsorId { get; } = SponsorId;
    public Guid OfferId { get; } = OfferId;
}