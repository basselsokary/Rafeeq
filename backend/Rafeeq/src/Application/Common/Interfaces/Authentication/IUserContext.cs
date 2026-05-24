using Application.DTOs.Users;
using Domain.Enums;

namespace Application.Common.Interfaces.Authentication;

public interface IUserContext
{
    Guid Id { get; }
    string UserName { get; }
    LanguageCode Language { get; }
    UserDto User { get; }
    bool IsInRoles(params string[] roles);
    bool IsInAnyRole(params string[] roles);
    bool IsAuthenticated { get; }
}
