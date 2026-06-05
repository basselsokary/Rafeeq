using Domain.Enums;
using Shared;

namespace Infrastructure.Identity.Entities;

public sealed class TouristUser : ApplicationUser
{
    private TouristUser() : base() { }
    private TouristUser(Guid userId, string userName, string email, bool emailConfirmed = false)
        : base(userId, userName, email, emailConfirmed: emailConfirmed) { }

    public static Result<TouristUser> Create(Guid userId, string userName, string email, bool emailConfirmed = false)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return ApplicationUserErrors.UserNameRequired;

        if (string.IsNullOrWhiteSpace(email))
            return ApplicationUserErrors.EmailRequired;

        return new TouristUser(userId, userName, email, emailConfirmed);
    }

    public void BanAccount()
    {
        Status = UserStatus.Banned;
        IsActive = false;
    }
}
