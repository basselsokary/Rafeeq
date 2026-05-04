namespace Shared;

public class Result
{
    public bool Succeeded { get; }
    public bool Failed => !Succeeded;
    public Error Error { get; }

    protected Result(bool succeeded, Error error)
    {
        if (succeeded && error != Error.None)
            throw new InvalidOperationException("Successful result cannot have an error");
        
        if (!succeeded && error == Error.None)
            throw new InvalidOperationException("Failed result must have an error");

        Succeeded = succeeded;
        Error = error;
    }

    public static Result Success() => new (true, Error.None);
    public static Result<T> Success<T>(T data) => new(true, data, Error.None);

    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Failure<T>(Error error) => new(false, default, error);
    
    public static Result Failure(string message) => new(false, Error.General(message));
    public static Result<T> Failure<T>(string message) => new(false, default, Error.General(message));    
    
    public static implicit operator Result(Error error) => Failure(error);
}

public class Result<T>(bool succeeded, T? value, Error error) : Result(succeeded, error)
{
    private readonly T? _value = value;

    public T Value =>
        Succeeded
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure<T>(error);
}
