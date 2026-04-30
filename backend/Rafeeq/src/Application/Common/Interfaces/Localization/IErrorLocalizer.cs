namespace Application.Common.Interfaces.Localization;

/// <summary>
/// Thin wrapper around <see cref="IStringLocalizer{SharedResource}"/> that adds:
///   • Format-string support (string.Format-style args)
///   • Strongly-typed key constants so callers never use magic strings
/// </summary>
public interface IErrorLocalizer
{
    string this[string key] { get; }
    string Format(string key, params object[] args);
}
