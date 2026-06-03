using Application.Common.Interfaces.Services;
using Application.DTOs.Services;
using Application.Services;
using Domain.Common;
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
            if (imageStream.CanSeek)
                imageStream.Position = 0;

            bool needsResize;

            // Peek at dimensions without fully decoding pixel data
            var imageInfo = await Image.IdentifyAsync(imageStream, ct);
            if (imageInfo is null)
                return Result.Failure<ProcessedImage>(ImageErrors.ProcessingFailed);

            needsResize = _options.EnableImageResize &&
                        (imageInfo.Width  > _options.MaxWidthPx ||
                        imageInfo.Height > _options.MaxHeightPx);

            if (!needsResize)
            {
                if (imageStream.CanSeek)
                    imageStream.Position = 0;

                var passthroughBuffer = new MemoryStream();
                await imageStream.CopyToAsync(passthroughBuffer, ct);
                passthroughBuffer.Position = 0;

                return Result.Success(
                    new ProcessedImage(passthroughBuffer, contentType, passthroughBuffer.Length));
            }

            if (imageStream.CanSeek)
                imageStream.Position = 0;

            using var image = await Image.LoadAsync(imageStream, ct);

            image.Mutate(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(_options.MaxWidthPx, _options.MaxHeightPx),
                Mode = ResizeMode.Max,
            }));

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
            return Result.Failure<ProcessedImage>(ImageErrors.ProcessingFailed);
        }
    }

    public async Task<Result<ProcessedImage>> CompressForMlAsync(
        Stream imageStream,
        ImageContentType contentType,
        CancellationToken ct = default)
    {
        try
        {
            if (imageStream.CanSeek)
                imageStream.Position = 0;

            const int MlMaxDimension = 512;

            // Peek at dimensions without decoding pixel data
            var imageInfo = await Image.IdentifyAsync(imageStream, ct);
            if (imageInfo is null)
                return Result.Failure<ProcessedImage>(ImageErrors.ProcessingFailed);

            bool needsResize =
                imageInfo.Width  > MlMaxDimension ||
                imageInfo.Height > MlMaxDimension;

            if (!needsResize)
            {
                if (imageStream.CanSeek)
                    imageStream.Position = 0;

                if (contentType == ImageContentType.Jpeg)
                {
                    var passthroughBuffer = new MemoryStream();
                    await imageStream.CopyToAsync(passthroughBuffer, ct);
                    passthroughBuffer.Position = 0;

                    return Result.Success(
                        new ProcessedImage(passthroughBuffer, contentType, passthroughBuffer.Length));
                }

                // Load only to re-encode — no Mutate, no resize
                using var smallImage = await Image.LoadAsync(imageStream, ct);

                smallImage.Metadata.ExifProfile = null;
                smallImage.Metadata.IptcProfile = null;
                smallImage.Metadata.XmpProfile  = null;
                smallImage.Metadata.IccProfile  = null;

                var fastOutput = new MemoryStream();
                await smallImage.SaveAsync(fastOutput, new JpegEncoder { Quality = 75 }, ct);
                fastOutput.Position = 0;

                return Result.Success(
                    new ProcessedImage(fastOutput, ImageContentType.Jpeg, fastOutput.Length));
            }

            if (imageStream.CanSeek)
                imageStream.Position = 0;

            using var image = await Image.LoadAsync(imageStream, ct);

            image.Mutate(ctx => ctx.Resize(new ResizeOptions
            {
                Size    = new Size(MlMaxDimension, MlMaxDimension),
                Mode    = ResizeMode.Max,
                Sampler = KnownResamplers.Triangle
            }));

            image.Metadata.ExifProfile = null;
            image.Metadata.IptcProfile = null;
            image.Metadata.XmpProfile  = null;
            image.Metadata.IccProfile  = null;

            var output = new MemoryStream();
            await image.SaveAsync(output, new JpegEncoder { Quality = 75 }, ct);
            output.Position = 0;

            return Result.Success(
                new ProcessedImage(output, ImageContentType.Jpeg, output.Length));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ML compression failed");
            return Result.Failure<ProcessedImage>(ImageErrors.ProcessingFailed);
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
