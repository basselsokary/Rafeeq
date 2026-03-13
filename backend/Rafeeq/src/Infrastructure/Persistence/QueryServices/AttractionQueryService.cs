using Application.Common.Interfaces.QueryServices;
using Infrastructure.Persistence.ApplicationContext;
using Infrastructure.Identity;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Domain.Enums;

namespace Infrastructure.Persistence.QueryServices;

internal class AttractionQueryService(
    ApplicationDbContext context,
    CurrentUser currentUser) : IAttractionQueryService
{
    public Task<AttractionDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<AttractionListDto>> GetByTypeAsync(Guid siteId, AttractionType type, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
