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
            if (user == null || (!user.Identity?.IsAuthenticated ?? true))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(id))
                throw new UnauthorizedAccessException("User ID claim is missing.");

            bool successfulParse = Guid.TryParse(id, out Guid guidId);
            if (successfulParse)
                return guidId;

            throw new UnauthorizedAccessException("Coudn't parse user ID to GUID");
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }

    public LanguageCode Language
    {
        get
        {
            string? language = _httpContextAccessor.HttpContext?.Request.Headers["Accept-Language"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(language))
                return LanguageCode.English;
            
            bool success = Enum.TryParse<LanguageCode>(language, out var status);
            return success ? status : LanguageCode.English;
        }
    }
}
