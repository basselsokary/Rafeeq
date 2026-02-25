using Shared.Models;

namespace Domain.Entities.TouristAggregate;

public class UserErrors
{
    public static Error Unauthorized(string? userId) => Error.Unauthorized(
        "UNAUTHORIZED_USER",
        $"The user with id = '{userId}' is unauthorized.");

    public static Error NotFound(string key) => Error.NotFound(
        "USER_NOT_FOUND",
        $"The user with = '{key}' was not found.");

    public static Error InvalidCredentials() => Error.Failure(
        "INVALID_CREDENTAILS",
        "");

    public static Error EmailAlreadyInUse(string email) => Error.Conflict(
        "EMAIL_ALREADY_IN_USE",
        $"Email {email} already in use.");
    
    public static Error SomethingWentWrong() => Error.Failure(
        "SOMETHING_WENT_WRONG",
        "Something went wrong.");
}
