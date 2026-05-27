using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites.Images;

public sealed record GetSiteImagesByIdQuery(Guid SiteId) : IQuery<List<ImageDto>>;

internal sealed class GetSiteImagesByIdQueryHandler(
    ISiteQueryService queryService,
    IFileStorageService fileStorageService) : IQueryHandler<GetSiteImagesByIdQuery, List<ImageDto>>
{
    public async Task<Result<List<ImageDto>>> HandleAsync(GetSiteImagesByIdQuery query, CancellationToken cancellationToken)
    {
        var siteExist = await queryService.AnyAsync(query.SiteId, cancellationToken);
        if (!siteExist)
            return SiteErrors.NotFound(query.SiteId);

        var images = await queryService.GetImagesAsync(query.SiteId, cancellationToken);
        
        return Result.Success(images.Select(i => i with
        {
            StorageKey = i.StorageKey,
            Url = fileStorageService.GetOptimizedUrl(i.StorageKey)
        }).ToList());
    }
}
