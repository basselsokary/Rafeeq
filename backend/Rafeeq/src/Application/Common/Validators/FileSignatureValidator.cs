namespace Application.Common.Validators;

/// <summary>
/// Validates file type by reading the actual byte signature (magic numbers).
/// This is the ONLY trustworthy way to validate file type — never trust
/// the Content-Type header or file extension provided by the client.
/// </summary>
public static class FileSignatureValidator
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

    // .jpeg maps to same signatures as .jpg
    static FileSignatureValidator()
    {
        _signatures[".jpeg"] = _signatures[".jpg"];
    }

    /// <summary>
    /// Validates a collection of stream/extension pairs.
    /// Each stream must be paired with the extension that belongs to it.
    /// </summary>
    public static bool IsValid(IEnumerable<(Stream Stream, string Extension)> files)
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
    /// known magic bytes. Does NOT advance the stream past what it reads —
    /// always resets Position to 0 after inspection.
    /// </summary>
    public static bool IsValid(Stream stream, string extension)
    {
        if (!_signatures.TryGetValue(extension.ToLowerInvariant(), out var signatures))
            return false;

        // Read the header bytes (12 covers all signatures above, including WebP marker)
        const int headerSize = 12;
        var headerBytes = new byte[headerSize];

        var originalPosition = stream.Position;
        try
        {
            var bytesRead = stream.Read(headerBytes, 0, headerSize);
            if (bytesRead < 4) return false; // Too small to be a valid image

            // Special-case WebP: must be RIFF at [0..3] AND WEBP at [8..11]
            if (extension.Equals(".webp", StringComparison.OrdinalIgnoreCase))
            {
                return IsWebP(headerBytes, bytesRead);
            }

            return signatures.Any(sig =>
                bytesRead >= sig.Length &&
                sig.SequenceEqual(headerBytes.AsSpan()[..sig.Length]));
        }
        finally
        {
            stream.Position = originalPosition;
        }
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

    /// <summary>
    /// Maps a verified extension to its canonical MIME type.
    /// Used to set Content-Type in storage — again, never trust client input.
    /// </summary>
    public static string GetCanonicalMimeType(string extension) =>
        extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png"            => "image/png",
            ".webp"           => "image/webp",
            _                 => "application/octet-stream"
        };
}