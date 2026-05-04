using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites.Images;

public sealed record GetSiteImagesByIdQuery(Guid SiteId) : IQuery<List<ImageDto>>;

internal sealed class GetSiteImagesByIdQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteImagesByIdQuery, List<ImageDto>>
{
    public async Task<Result<List<ImageDto>>> HandleAsync(GetSiteImagesByIdQuery query, CancellationToken cancellationToken)
    {
        var site = await queryService.GetByIdForAdminAsync(query.SiteId, cancellationToken);
        if (site is null)
            return SiteErrors.NotFound(query.SiteId);

        var images = await queryService.GetImagesAsync(query.SiteId, cancellationToken);
        return Result.Success(images);
    }
}
