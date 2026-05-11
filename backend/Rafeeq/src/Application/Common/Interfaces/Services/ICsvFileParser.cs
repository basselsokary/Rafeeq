namespace Application.Common.Interfaces.Services;

public interface ICsvFileParser
{
    List<T> ParseCsv<T>(Stream file);
}