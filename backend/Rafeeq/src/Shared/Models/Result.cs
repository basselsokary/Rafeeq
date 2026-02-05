namespace Shared.Models;

public class Result
{
    public bool Succeeded { get; init; }

    public string[] Errors { get; init; }

    protected Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = [.. errors];
    }

    public static Result Success() => new (true, []);

    public static Result<T> Success<T>(T? data = default) => new(true, data, []);

    public static Result Failure(IEnumerable<string> errors) => new(false, errors);

    public static Result<T> Failure<T>(IEnumerable<string> errors) => new(false, default, errors);
}

public class Result<T>(
    bool succeeded,
    T? Data,
    IEnumerable<string> errors) : Result(succeeded, errors)
{
    public T? Data { get; init; } = Data;
}
