using Domain.Common;

namespace Domain.Entities.TouristAggregate;

public class UserRegisteredEvent(string Email, string UserName) : BaseEvent
{
    public string Email { get; } = Email;
    public string UserName { get; } = UserName;
}