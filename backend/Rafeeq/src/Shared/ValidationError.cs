namespace Shared;

public record ValidationError : Error 
{ 
    public ValidationError(IEnumerable<Error> errors)
        : base(
            "VALIDATION_GENERAL",
            "One or more validation errors occurred",
            ErrorType.Validation)
    { 
        if (errors == null || !errors.Any()) 
        { 
            throw new ArgumentException("ValidationError must contain at least one error.", nameof(errors)); 
        }
        
        Errors = errors.Where(e => e != None)
            .Where(e => e.Type == ErrorType.Validation)
            .ToList(); 
    } 
    
    public IReadOnlyList<Error> Errors { get; } 
    
    public static ValidationError FromErrors(IEnumerable<Error> errors)
        => new(errors); 
    
    public static ValidationError FromResults(IEnumerable<Result> results)
        => new(results.Where(r => r.Failed)
            .Select(r => r.Error)
            .ToList());
}