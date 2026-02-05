namespace Domain.Errors;

public class UserErrors
{
    public static IEnumerable<string> Unauthorized(string? userId) =>
    [
        "Unauthorized User",
        $"The user with id = '{userId}' is unauthorized."
    ];

    public static IEnumerable<string> NotFound(string prop) =>
    [
        "User Was Not Found",
        $"The user with = '{prop}' was not found."
    ];

    public static IEnumerable<string> InvalidCredentials() =>
    [
        "Invalid Credentials"
    ];

    public static IEnumerable<string> AlreadyExist(string email) =>
    [
        "User Already Exist",
        $"Email {email} already in use."
    ];

    public static IEnumerable<string> OrganizerNotAllowed() =>
    [
        "Organizer Not Allowed",
        "Your account is locked. Contact with us to know more."
    ];

    public static IEnumerable<string> InvalidRefreshToken() =>
    [
        "Invalid Refresh Token",
        "The refresh token is invalid or has expired."
    ];
    
    public static IEnumerable<string> SomethingWentWrong() =>
    [
        "Something Went Wrong"
    ];
}
