namespace Infrastructure.ExternalServices.CloudinaryService;

public sealed class FileUploadOptions
{
    public const string SectionName = "FileUpload";

    public long MaxFileSizeBytes { get; init; } = 10 * 1024 * 1024; // 10 MB default
    public string[] AllowedExtensions { get; init; } = [".jpg", ".jpeg", ".png", ".webp"];
    public int MaxFilesPerRequest { get; init; } = 10;
    public bool EnableDuplicateDetection { get; init; } = true;
    public bool EnableImageResize { get; init; } = false;
    public int MaxWidthPx { get; init; } = 2048;
    public int MaxHeightPx { get; init; } = 2048;
}

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