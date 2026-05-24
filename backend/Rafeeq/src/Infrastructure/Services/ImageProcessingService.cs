using Application.Common.Interfaces.Services;
using Application.DTOs.Services;
using Application.Services;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services;

internal class ImageProcessingService(
    IOptions<FileUploadOptions> options,
    ILogger<ImageProcessingService> logger) : IImageProcessingService
{
    private readonly FileUploadOptions _options = options.Value;

    public async Task<Result<ProcessedImage>> ProcessAsync(
        Stream imageStream,
        ImageContentType contentType,
        CancellationToken ct = default)
    {
        try
        {
            imageStream.Position = 0;
            using var image = await Image.LoadAsync(imageStream, ct);

            if (_options.EnableImageResize &&
                (image.Width > _options.MaxWidthPx || image.Height > _options.MaxHeightPx))
            {
                var before = $"{image.Width}x{image.Height}";
                image.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(_options.MaxWidthPx, _options.MaxHeightPx),
                    Mode = ResizeMode.Max,
                }));
            }

            // Strip EXIF — prevents GPS, device serial, timestamps leaking out
            image.Metadata.ExifProfile = null;
            image.Metadata.IptcProfile = null;

            var output  = new MemoryStream();
            var encoder = GetEncoder(contentType);
            await image.SaveAsync(output, encoder, ct);
            output.Position = 0;

            return Result.Success(
                new ProcessedImage(output, contentType, output.Length));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Image processing failed");
            return Result.Failure<ProcessedImage>("Image could not be processed.");
        }
    }

    private static IImageEncoder GetEncoder(ImageContentType ct) =>
        ct.Value switch
        {
            "image/png"  => new PngEncoder  { CompressionLevel = PngCompressionLevel.BestCompression },
            "image/webp" => new WebpEncoder { Quality = 82 },
            _            => new JpegEncoder { Quality = 85 },
        };
}
