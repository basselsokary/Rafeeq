using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Domain.Entities.SponsorAggregate;

namespace Application.Queries.Sponsors.Images;

public sealed record GetSponsorImagesByIdQuery(Guid SponsorId) : IQuery<List<ImageDto>>;

internal sealed class GetSponsorImagesByIdQueryHandler(
    ISponsorQueryService queryService,
    IFileStorageService fileStorageService) : IQueryHandler<GetSponsorImagesByIdQuery, List<ImageDto>>
{
    public async Task<Result<List<ImageDto>>> HandleAsync(GetSponsorImagesByIdQuery query, CancellationToken cancellationToken)
    {
        var sponsorExist = await queryService.AnyAsync(query.SponsorId, cancellationToken);
        if (!sponsorExist)
            return SponsorErrors.NotFound(query.SponsorId);

        var images = await queryService.GetImagesAsync(query.SponsorId, cancellationToken);
        
        return Result.Success(images.Select(i => i with
        {
            StorageKey = i.StorageKey,
            Url = fileStorageService.GetOptimizedUrl(i.StorageKey)
        }).ToList());
    }
}
