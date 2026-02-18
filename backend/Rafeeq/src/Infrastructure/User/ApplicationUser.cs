using Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.User;

public class ApplicationUser : IdentityUser
{
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;

    // public ICollection<RefreshToken> RefreshTokens { get; set; }
}
