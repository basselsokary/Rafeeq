using Application.Services;
using Microsoft.Extensions.Options;

namespace Application.Common.Validators;

/// <summary>
/// Validates file type by reading the actual byte signature (magic numbers).
/// This is the ONLY trustworthy way to validate file type — never trust
/// the Content-Type header or file extension provided by the client.
/// </summary>
public class FileSignatureValidator
{
    private static readonly Dictionary<string, List<byte[]>> _signatures = new()
    {
        [".jpg"] =
        [
            [0xFF, 0xD8, 0xFF, 0xE0],  // JFIF
            [0xFF, 0xD8, 0xFF, 0xE1],  // EXIF
            [0xFF, 0xD8, 0xFF, 0xE2],  // Canon CIFF
            [0xFF, 0xD8, 0xFF, 0xDB],  // Raw JPEG
            [0xFF, 0xD8, 0xFF, 0xEE],  // SPIFF
        ],
        [".png"] =
        [
            [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]  // PNG
        ],
        [".webp"] =
        [
            // WebP: "RIFF????WEBP" — bytes 0-3 are RIFF, bytes 8-11 are WEBP
            // We check RIFF header + WEBP marker at offset 8
            [0x52, 0x49, 0x46, 0x46]   // "RIFF" — additional WEBP check below
        ],
    };
    private readonly FileUploadOptions _options;

    public FileSignatureValidator(IOptions<FileUploadOptions> options)
    {
        _signatures[".jpeg"] = _signatures[".jpg"];
        _options = options.Value;
    }

    public bool IsValid(IEnumerable<(Stream Stream, string Extension)> files)
    {
        foreach (var file in files)
        {
            if (!IsValid(file.Stream, file.Extension))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Reads up to 12 bytes from the START of the stream and checks against
    /// known magic bytes.
    /// </summary>
    public bool IsValid(Stream stream, string extension)
    {
        if (!IsAllowedExtension(extension))
            return false;

        const int headerSize = 12;
        var headerBytes = new byte[headerSize];

        var originalPosition = stream.Position;
        try
        {
            stream.Position = 0;

            var bytesRead = stream.Read(headerBytes, 0, headerSize);

            if (bytesRead < 4)
                return false;

            if (_signatures[".jpg"].Any(sig =>
                bytesRead >= sig.Length &&
                sig.SequenceEqual(headerBytes.AsSpan()[..sig.Length])))
                return true;

            if (_signatures[".png"].Any(sig =>
                bytesRead >= sig.Length &&
                sig.SequenceEqual(headerBytes.AsSpan()[..sig.Length])))
                return true;

            if (IsWebP(headerBytes, bytesRead))
                return true;

            return false;
        }
        finally
        {
            stream.Position = originalPosition;
        }

    }

    public static string GetContentType(string extension)
    {
        return extension.Trim().ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }

    private static bool IsWebP(byte[] header, int bytesRead)
    {
        if (bytesRead < 12) return false;

        // "RIFF"
        bool isRiff = header[0] == 0x52 && header[1] == 0x49
                   && header[2] == 0x46 && header[3] == 0x46;
        // "WEBP"
        bool isWebP = header[8] == 0x57 && header[9] == 0x45
                   && header[10] == 0x42 && header[11] == 0x50;

        return isRiff && isWebP;
    }

    // private static bool IsWebP(byte[] header, int bytesRead)
    // {
    //     if (bytesRead < 12) return false;

    //     bool riff = header[0] == 0x52 && header[1] == 0x49 &&
    //                 header[2] == 0x46 && header[3] == 0x46;

    //     bool webp = header[8] == 0x57 && header[9] == 0x45 &&
    //                 header[10] == 0x42 && header[11] == 0x50;

    //     if (!riff || !webp) return false;

    //     if (bytesRead >= 16)
    //     {
    //         var chunk = System.Text.Encoding.ASCII.GetString(header, 12, 4);

    //         return chunk is "VP8 " or "VP8L" or "VP8X";
    //     }

    //     return false;
    // }

    private bool IsAllowedExtension(string extension)
    {
        return _options.AllowedExtensions.Contains(extension.Trim().ToLowerInvariant());
    }
}