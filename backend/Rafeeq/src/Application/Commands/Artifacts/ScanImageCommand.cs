using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using Application.DTOs.Artifacts;
using Domain.Entities.ArtifactAggregate;

namespace Application.Commands.Artifacts;

public record ScanImageCommand(Stream Image, string ContentType)
    : ICommand<ArtifactDetailsDto>;

public sealed class ScanImageHandler(
    IImageScannerService scannerService,
    IImageProcessingService imageProcessingService,
    IArtifactQueryService artifactQueryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : ICommandHandler<ScanImageCommand, ArtifactDetailsDto>
{
    public async Task<Result<ArtifactDetailsDto>> HandleAsync(ScanImageCommand command, CancellationToken ct)
    {
        var processedImageResult = await imageProcessingService.ProcessAsync(command.Image, command.ContentType, ct);
        if (processedImageResult.Failed)
            return processedImageResult.Error;
        
        var result = await scannerService.ScanAsyncV2(processedImageResult.Value.Stream, command.ContentType, ct);
        if (result.Failed)
            return result.Error;
        
        var label = result.Value.Label;
        if (string.IsNullOrEmpty(label) || label.Equals("unknown", StringComparison.OrdinalIgnoreCase))
            return ArtifactErrors.NotFound;
            // return await ScanAsync(command, processedImageResult.Value.Stream, ct);
        
        var artifact = await artifactQueryService.GetByNameAsync(label, userContext.Language, ct);
        if (artifact == null)
            return ArtifactErrors.NotFound;
            // return await ScanAsync(command, processedImageResult.Value.Stream, ct);
        
        return Result.Success(artifact with
        {
            TypeDisplay = enumLocalizer.Localize(artifact.Type)
        });
    }

    private async Task<Result<ArtifactDetailsDto>> ScanAsync(ScanImageCommand command, Stream processedImage, CancellationToken ct)
    {
        processedImage.Position = 0;
        var result = await scannerService.ScanAsync(processedImage, command.ContentType, ct);
        if (result.Failed)
            return result.Error;
        
        if (result.Value.Results.Count == 0)
            return Result.Failure<ArtifactDetailsDto>(Error.None);
        
        var results = result.Value.Results;
        var imageResultDto = results.OrderByDescending(r => r.Distance)
            .FirstOrDefault();
        
        if (string.IsNullOrEmpty(imageResultDto?.ArtifactId) || imageResultDto.Distance < 0.8)
            return ArtifactErrors.NotFound;

        var artifact = await artifactQueryService.GetByNameAsync(imageResultDto.ArtifactId, userContext.Language, ct);
        
        if (artifact == null)
            return ArtifactErrors.NotFound;
        
        return Result.Success(artifact with
        {
            TypeDisplay = enumLocalizer.Localize(artifact.Type)
        });
    }
}