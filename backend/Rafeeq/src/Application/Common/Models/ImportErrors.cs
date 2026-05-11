namespace Application.Common.Models;

public static class ImportErrors
{
    public static Error CsvParsingFailed(string message)
        => Error.Failure("CSV_PARSING_FAILED", $"Failed to parse CSV file: {message}");
}