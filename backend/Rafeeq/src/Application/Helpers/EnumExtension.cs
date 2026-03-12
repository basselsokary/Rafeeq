namespace Application.Helpers;

public static class EnumExtension
{
    /// <summary>
    /// Returns all defined values of the given enum type.
    /// </summary>
    public static IEnumerable<T> GetValues<T>() where T : struct, Enum
        => Enum.GetValues<T>();

    /// <summary>
    /// Tries to parse a string to the given enum type (case-insensitive).
    /// Returns true and sets result on success; otherwise false and default.
    /// </summary>
    public static bool TryParse<T>(string? value, out T result) where T : struct, Enum
        => Enum.TryParse(value, ignoreCase: true, out result);

    /// <summary>
    /// Parses a string to the given enum type, or returns <paramref name="defaultValue"/> if parsing fails.
    /// </summary>
    public static T ParseOrDefault<T>(string? value, T defaultValue = default) where T : struct, Enum
        => Enum.TryParse(value, ignoreCase: true, out T result) ? result : defaultValue;

    /// <summary>
    /// Returns true if the integer corresponds to a defined enum member.
    /// </summary>
    public static bool IsDefined<T>(int value) where T : struct, Enum
        => Enum.IsDefined(typeof(T), value);

    /// <summary>
    /// Returns true if the enum value is contained within the provided set.
    /// </summary>
    public static bool IsIn<T>(this T value, params T[] values) where T : struct, Enum
        => values.Contains(value);
}
