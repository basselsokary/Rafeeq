using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization.Requirements;

public class RolesRequirement : IAuthorizationRequirement
{
    public string[] Roles { get; }

    public RolesRequirement(params string[] roles)
    {
        Roles = roles;
    }
}
