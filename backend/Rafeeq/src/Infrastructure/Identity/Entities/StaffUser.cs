using Domain.Enums;
using Shared;

namespace Infrastructure.Identity.Entities;

public abstract class StaffUser : ApplicationUser
{
    protected StaffUser() : base() { }
    protected StaffUser(Guid userId, string userName, string email, string firstName, string lastName, string fullName)
        : base(userId, userName, email, mustChangePassword: true, emailConfirmed: true)
    {
        FirstName = firstName;
        LastName = lastName;
        FullName = fullName;
    }

    public string FirstName { get; protected set; } = null!;
    public string LastName { get; protected set; } = null!;
    public string FullName { get; protected set; } = null!;

    public Result Update(string firstName, string lastName, string fullName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return ApplicationUserErrors.FirstNameRequired;

        if (string.IsNullOrWhiteSpace(lastName))
            return ApplicationUserErrors.LastNameRequired;

        if (string.IsNullOrWhiteSpace(fullName))
            return ApplicationUserErrors.FullNameRequired;
        
        FirstName = firstName;
        LastName = lastName;
        FullName = fullName;

        return Result.Success();
    }
}