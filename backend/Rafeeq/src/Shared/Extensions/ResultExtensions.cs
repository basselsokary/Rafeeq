namespace Shared.Models;

public static class ResultExtensions
{
    public static Result<T> To<T>(this Result result) => new(result.Succeeded, default, result.Error);
    
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error)
    {
        if (result.Failed)
            return result;

        return predicate(result.Value) 
            ? result 
            : Result.Failure<T>(error);
    }

    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        return result.Succeeded 
            ? Result.Success(mapper(result.Value)) 
            : Result.Failure<TOut>(result.Error);
    }

    public static async Task<Result<TOut>> Bind<TIn, TOut>(
        this Result<TIn> result, 
        Func<TIn, Task<Result<TOut>>> func)
    {
        return result.Succeeded 
            ? await func(result.Value) 
            : Result.Failure<TOut>(result.Error);
    }

    public static async Task<Result> Tap<T>(this Result<T> result, Func<T, Task> action)
    {
        if (result.Succeeded)
        {
            await action(result.Value);
            Result.Success();
        }

        return Result.Failure(result.Error);
    }
}