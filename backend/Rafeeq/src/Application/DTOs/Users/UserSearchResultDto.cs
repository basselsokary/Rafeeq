namespace Application.DTOs.Users;

public record UserSearchResultDto(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    string Status)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
}
