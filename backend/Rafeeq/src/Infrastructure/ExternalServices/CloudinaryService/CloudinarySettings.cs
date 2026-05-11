namespace Infrastructure.ExternalServices.CloudinaryService;

public class CloudinarySettings
{
    public const string SectionName = "Cloudinary";

    public string CloudName { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public string ApiSecret { get; init; } = string.Empty;

    /// <summary>
    /// Folder prefix for all uploaded assets inside your Cloudinary account.
    /// e.g. "rafeeq/uploads" → all files land under that folder.
    /// </summary>
    public string UploadFolder { get; init; } = "uploads";

    /// <summary>
    /// How long signed delivery URLs remain valid (seconds).
    /// Signed URLs are used when your Cloudinary account is set to
    /// "Restricted" delivery type — otherwise unsigned URLs are returned.
    /// </summary>
    public int SignedUrlExpirySeconds { get; init; } = 3600;
}