using Domain.Enums;

namespace Application.Helpers;

public static class UserRoleExtension
{
    public static string GetName(this UserRole role) => role switch
    {
        UserRole.Admin => nameof(UserRole.Admin),
        UserRole.Tourist => nameof(UserRole.Tourist),
        UserRole.Guest => nameof(UserRole.Guest),
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
    };

    public static UserRole GetRole(this string roleString) => roleString switch
    {
        nameof(UserRole.Admin) => UserRole.Admin,
        nameof(UserRole.Tourist) => UserRole.Tourist,
        nameof(UserRole.Guest) => UserRole.Guest,
        _ => throw new ArgumentOutOfRangeException(roleString, roleString, null)
    };
}
