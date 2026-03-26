using Shared.Models;

namespace Shared.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Converts a non-generic Result to a generic Result&lt;T&gt;.
    /// </summary>
    /// <typeparam name="T">The type of the value in the resulting Result&lt;T&gt;.</typeparam>
    /// <param name="result">The Result to convert.</param>
    /// <returns>A Result&lt;T&gt; with the same success status and error as the original Result, and a default value.</returns>
    public static Result<T> To<T>(this Result result, T? data = default) => new(result.Succeeded, data, result.Error);
    
    /// <summary>
    /// Ensures that the result value satisfies a given predicate condition.
    /// </summary>
    /// <typeparam name="T">The type of the value in the Result&lt;T&gt;.</typeparam>
    /// <param name="result">The Result&lt;T&gt; to validate.</param>
    /// <param name="predicate">A function that returns true if the value is valid; otherwise false.</param>
    /// <param name="error">The error to return if the predicate fails.</param>
    /// <returns>The original result if the predicate passes, or a failure result with the specified error.</returns>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error)
    {
        if (result.Failed)
            return result;

        return predicate(result.Value) 
            ? result 
            : Result.Failure<T>(error);
    }

    /// <summary>
    /// Transforms the value of a successful Result&lt;TIn&gt; to a Result&lt;TOut&gt; using the provided mapper function.
    /// </summary>
    /// <typeparam name="TIn">The type of the value in the input Result.</typeparam>
    /// <typeparam name="TOut">The type of the value in the output Result.</typeparam>
    /// <param name="result">The Result&lt;TIn&gt; to transform.</param>
    /// <param name="mapper">A function that transforms the value from TIn to TOut.</param>
    /// <returns>A Result&lt;TOut&gt; containing the transformed value, or a failure if the input result failed.</returns>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        return result.Succeeded 
            ? Result.Success(mapper(result.Value)) 
            : Result.Failure<TOut>(result.Error);
    }

    /// <summary>
    /// Binds an asynchronous operation to a Result&lt;TIn&gt;, flattening the result.
    /// </summary>
    /// <typeparam name="TIn">The type of the value in the input Result.</typeparam>
    /// <typeparam name="TOut">The type of the value in the output Result.</typeparam>
    /// <param name="result">The Result&lt;TIn&gt; to bind.</param>
    /// <param name="func">An asynchronous function that transforms the value and returns a Result&lt;TOut&gt;.</param>
    /// <returns>A Task that represents the asynchronous operation, returning the Result&lt;TOut&gt; from the function, or a failure if the input result failed.</returns>
    public static async Task<Result<TOut>> Bind<TIn, TOut>(
        this Result<TIn> result, 
        Func<TIn, Task<Result<TOut>>> func)
    {
        return result.Succeeded 
            ? await func(result.Value) 
            : Result.Failure<TOut>(result.Error);
    }

    /// <summary>
    /// Executes an asynchronous action on a successful Result&lt;T&gt; without transforming the value.
    /// </summary>
    /// <typeparam name="T">The type of the value in the Result.</typeparam>
    /// <param name="result">The Result&lt;T&gt; to tap into.</param>
    /// <param name="action">An asynchronous action to execute if the result is successful.</param>
    /// <returns>A Task that represents the asynchronous operation, returning the original Result if successful, or a failure result.</returns>
    public static async Task<Result> Tap<T>(this Result<T> result, Func<T, Task> action)
    {
        if (result.Succeeded)
        {
            await action(result.Value);
            return Result.Success();
        }

        return Result.Failure(result.Error);
    }
}