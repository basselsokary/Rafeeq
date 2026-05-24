using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization.Requirements;

public class ResourceOwnerRequirement : IAuthorizationRequirement
{
    public bool AllowAdminOverride { get; }

    public ResourceOwnerRequirement(bool allowAdminOverride = true)
    {
        AllowAdminOverride = allowAdminOverride;
    }
}
