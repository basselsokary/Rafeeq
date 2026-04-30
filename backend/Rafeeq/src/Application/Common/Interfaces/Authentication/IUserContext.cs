using Domain.Enums;

namespace Application.Common.Interfaces.Authentication;

public interface IUserContext
{
    Guid Id { get; }
    string UserName { get; }
    LanguageCode Language { get; }
    bool IsInRoles(params UserRole[] role);
    bool IsInAnyRole(params UserRole[] role);
    bool IsAuthenticated { get; }
}
