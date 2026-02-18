namespace Shared.Models;

public enum ErrorType : byte
{
    None,
    Failure,
    NotFound,
    Validation,
    Conflict,
    Unauthorized,
    Forbidden
}