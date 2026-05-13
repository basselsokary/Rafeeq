using System.Reflection;

namespace Infrastructure.Persistence.Seeding;

/// <summary>
/// Resolves embedded CSV resources from the Infrastructure assembly.
///
/// Convention: every CSV file lives under
///   Infrastructure/Persistence/Seeding/Data/&lt;FileName&gt;.csv
/// and must have its Build Action set to <c>Embedded Resource</c> in the .csproj:
///
/// <code>
///   &lt;ItemGroup&gt;
///     &lt;EmbeddedResource Include="Persistence\Seeding\Data\*.csv" /&gt;
///   &lt;/ItemGroup&gt;
/// </code>
///
/// The manifest resource name produced by the SDK is:
///   Infrastructure.Persistence.Seeding.Data.&lt;FileName&gt;.csv
/// </summary>
internal static class EmbeddedResourceHelper
{
    private static readonly Assembly _assembly = typeof(EmbeddedResourceHelper).Assembly;

    // Adjust this prefix if the default namespace of the Infrastructure project differs.
    private const string ResourcePrefix = "Infrastructure.Persistence.Seeding.Data";

    /// <summary>
    /// Opens a stream for the embedded CSV resource with the given file name (e.g. "Cities.csv").
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resource cannot be found — most likely because the file is missing
    /// or its Build Action is not set to <c>Embedded Resource</c>.
    /// </exception>
    public static Stream GetCsvStream(string fileName)
    {
        var resourceName = $"{ResourcePrefix}.{fileName}";

        var stream = _assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            var available = string.Join(", ", _assembly.GetManifestResourceNames());
            throw new InvalidOperationException(
                $"Embedded resource '{resourceName}' not found. " +
                $"Available resources: [{available}]. " +
                $"Make sure the file exists under 'Persistence/Seeding/Data/' " +
                $"and its Build Action is set to 'Embedded Resource'.");
        }

        return stream;
    }
}
