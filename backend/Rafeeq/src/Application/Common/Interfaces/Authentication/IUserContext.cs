using Domain.Enums;

namespace Application.Common.Interfaces.Authentication;

public interface IUserContext
{
    Guid Id { get; }
    LanguageCode Language { get; }
    bool IsInRole(UserRole role);
    bool IsAuthenticated { get; }
}
