namespace Shared.Models;

public static class ResultExtensions
{
    public static Result<T> To<T>(this Result result) => new(result.Succeeded, default, result.Errors);
}