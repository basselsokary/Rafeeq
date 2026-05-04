using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.TouristAggregate;

public class UserStatusChangedEvent(Guid UserId, UserStatus NewStatus) : BaseEvent
{
    public Guid UserId { get; } = UserId;
    public UserStatus NewStatus { get; } = NewStatus;
}
