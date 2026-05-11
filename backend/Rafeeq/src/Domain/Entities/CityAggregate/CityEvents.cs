using Domain.Common;

namespace Domain.Entities.CityAggregate;

public class CityCreatedEvent(Guid id, string name) : BaseEvent
{
    public Guid CityId { get; } = id;
    public string Name { get; } = name;
}

public class CityUpdatedEvent(Guid id) : BaseEvent
{
    public Guid CityId { get; } = id;
}

public class CityDeletedEvent(Guid cityId) : BaseEvent
{
    public Guid CityId { get; } = cityId;
}

public class CityLocalizedContentUpdatedEvent(Guid cityId) : BaseEvent
{
    public Guid CityId { get; } = cityId;
}