namespace Application.DTOs.Users;

public record UserListDto(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    string Status,  // Active, Locked, Suspended, Deleted
    bool EmailConfirmed,
    bool PhoneNumberConfirmed,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    DateTime? LockedUntil)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
}
