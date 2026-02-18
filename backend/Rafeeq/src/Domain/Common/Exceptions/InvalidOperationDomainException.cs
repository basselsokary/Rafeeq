namespace Domain.Common.Exceptions;

/// <summary>
/// Exception thrown when an invalid operation is attempted
/// </summary>
public class InvalidOperationDomainException : DomainException
{
    public InvalidOperationDomainException(string message) : base(message)
    {
    }
}
