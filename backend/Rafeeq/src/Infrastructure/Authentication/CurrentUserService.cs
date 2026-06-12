using Application.Common.Interfaces.Authentication;
using Application.DTOs.Users;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Authentication;

internal class CurrentUserService(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid Id
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var id = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            
            bool successfulParse = Guid.TryParse(id, out Guid guidId);
            if (successfulParse)
                return guidId;

            throw new UnauthorizedAccessException("Invalid user ID.");
        }
    }
    
    public string UserName
    {
        get
        {
            var fullName = httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.PreferredUsername);
            
            return fullName ?? "System";
        }
    }

    public bool IsAuthenticated
        => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public LanguageCode Language
    {
        get
        {
            var cultureName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            LanguageCode languageCode = cultureName switch
            {
                "ar" => LanguageCode.Arabic,
                "fr" => LanguageCode.French,
                "ru" => LanguageCode.Russian,
                "es" => LanguageCode.Spanish,
                "de" => LanguageCode.German,
                _ => LanguageCode.English  // default
            };

            return languageCode;
        }
    }

    public bool IsInAnyRole(params string[] roles)
    {
        var user = httpContextAccessor.HttpContext?.User;
        foreach (var role in roles)
        {
            if (user?.IsInRole(role) == true)
                return true;
        }

        return false;
    }

    public bool IsInRoles(params string[] roles)
    {
        var user = httpContextAccessor.HttpContext?.User;
        foreach (var role in roles)
        {
            if (user?.IsInRole(role) == false)
                return false;
        }

        return true;
    }

    internal Guid UserId
    {
        get
        {
            string? id = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool successfulParse = Guid.TryParse(id, out Guid guidId);
            return successfulParse ? guidId : default;
        }
    }

    public UserDto User
    {
        get
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var user = httpContextAccessor.HttpContext?.User!;

            var id = UserId;
            var userName = user.FindFirstValue(JwtRegisteredClaimNames.PreferredUsername)!;
            var email = user.FindFirstValue(ClaimTypes.Email)!;
            var roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            return new UserDto(id, userName, email, roles);
        }
    }
}
