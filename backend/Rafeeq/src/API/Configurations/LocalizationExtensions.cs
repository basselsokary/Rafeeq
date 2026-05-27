namespace API.Configurations;

public static class LocalizationExtensions
{
    private static readonly string[] languages =
    [
        "ar", // Arabic
        "en", // English 
        "ru", // Russian
        "fr", // French
        "es", // Spanish
        "de", // German
        "zh", // Chinese
        "ja", // Japanese
        "it"  // Italian
    ];

    public static IApplicationBuilder UseLocalization(
        this IApplicationBuilder builder)
    {
        builder.UseRequestLocalization(options =>
        {
            var supportedCultures = languages;

            options.SetDefaultCulture("en");
            options.AddSupportedCultures(supportedCultures);
            options.AddSupportedUICultures(supportedCultures);
        });

        return builder;
    }
}
