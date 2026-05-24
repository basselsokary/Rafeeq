using Domain.Enums;
using Shared;

namespace Infrastructure.Identity.Entities;

public sealed class AdminUser : StaffUser
{
    private AdminUser() : base() {}
    private AdminUser(Guid userId, string userName, string email, string firstName, string lastName, string fullName)
        : base(userId, userName, email, firstName, lastName, fullName)
    {
    }

    public static Result<AdminUser> Create(
        Guid userId,
        string userName,
        string email,
        string firstName,
        string lastName,
        string fullName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return ApplicationUserErrors.UserNameRequired;

        if (string.IsNullOrWhiteSpace(email))
            return ApplicationUserErrors.EmailRequired;

        if (string.IsNullOrWhiteSpace(fullName))
            return ApplicationUserErrors.FullNameRequired;

        if (string.IsNullOrWhiteSpace(firstName))
            return ApplicationUserErrors.FirstNameRequired;

        if (string.IsNullOrWhiteSpace(lastName))
            return ApplicationUserErrors.LastNameRequired;

        return new AdminUser(userId, userName, email, firstName, lastName, fullName);
    }
}
