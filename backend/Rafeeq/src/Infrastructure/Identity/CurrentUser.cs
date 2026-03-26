using Application.Common.Interfaces.Authentication;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Identity;

public class CurrentUser : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid Id
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            bool successfulParse = Guid.TryParse(id, out Guid guidId);
            if (successfulParse)
                return guidId;

            throw new UnauthorizedAccessException("Invalid user ID.");
        }
    }

    public bool IsAuthenticated
        => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public LanguageCode Language
    {
        get
        {
            var language = _httpContextAccessor.HttpContext?
                .Request.Headers["Accept-Language"]
                .FirstOrDefault();
            
            bool success = Enum.TryParse<LanguageCode>(language, out var code);
            return success ? code : LanguageCode.English;
        }
    }

    public bool IsInRole(UserRole role)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.IsInRole(role.ToString()) ?? false;
    }
}
