using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.Sites;
using Application.Queries.Sites;
using Application.Queries.Sites.LocalizedContents;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class SitesController : ApiBaseController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<SiteListDto>>> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] SiteType? type,
        [FromQuery] Guid? city,
        [FromQuery] bool? isFree,
        [FromQuery] int? minRating,
        [FromQuery] int? maxRating,
        [FromServices] IQueryHandler<GetSitesQuery, PagedResult<SiteListDto>> queryHandler,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var filters = new SiteFilters(type, city, isFree, minRating, maxRating);
        var paging = new PagingParameters(page, pageSize);

        var query = new GetSitesQuery(filters, paging, searchTerm);
        var result = await queryHandler.HandleAsync(query);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<SiteDetailDto>> GetById(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSiteByIdQuery, SiteDetailDto> queryHandler)
    {
        var query = new GetSiteByIdQuery(id);
        var result = await queryHandler.HandleAsync(query);

        return HandleResult(result);
    }

    [HttpGet("featured")]
    [AllowAnonymous]
    public async Task<ActionResult<List<SiteListDto>>> GetFeatured(
        [FromQuery] Guid? city,
        [FromServices] IQueryHandler<GetFeaturedSitesQuery, List<SiteListDto>> queryHandler)
    {
        var query = new GetFeaturedSitesQuery(city);
        var result = await queryHandler.HandleAsync(query);

        return HandleResult(result);
    }

    [HttpGet("nearby")]
    public async Task<ActionResult<List<SiteListDto>>> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] int radiusKm,
        [FromQuery] SiteType? types,
        [FromQuery] Guid? city,
        [FromQuery] bool? isFree,
        [FromQuery] int? minRating,
        [FromQuery] int? maxRating,
        [FromServices] IQueryHandler<GetNearbySitesQuery, List<SiteListDto>> queryHandler)
    {
        var filters = new SiteFilters(types, city, isFree, minRating, maxRating);
        var query = new GetNearbySitesQuery(
            latitude,
            longitude,
            filters,
            radiusKm);

        var result = await queryHandler.HandleAsync(query);

        return HandleResult(result);
    }

    [HttpGet("within-bounds")]
    public async Task<ActionResult<List<SiteMapMarkerDto>>> GetWithinBounds(
        [FromQuery] double northLatitude,
        [FromQuery] double southLatitude,
        [FromQuery] double eastLongitude,
        [FromQuery] double westLongitude,
        [FromQuery] SiteType? type,
        [FromQuery] Guid? city,
        [FromQuery] bool? isFree,
        [FromQuery] int? minRating,
        [FromQuery] int? maxRating,
        [FromServices] IQueryHandler<GetSitesWithinBoundsQuery, List<SiteMapMarkerDto>> queryHandler)
    {
        var bounds = new BoundingBox(northLatitude, southLatitude, eastLongitude, westLongitude);
        var filters = new SiteFilters(type, city, isFree, minRating, maxRating);
        var query = new GetSitesWithinBoundsQuery(bounds, filters);
        var result = await queryHandler.HandleAsync(query);

        return HandleResult(result);
    }

	[HttpGet("{id:guid}/localized-contents")]
    public async Task<ActionResult<List<LocalizedContentDto>>> GetLocalizedContents(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSiteLocalizedContentsQuery, List<LocalizedContentDto>> queryHandler)
    {
        var result = await queryHandler.HandleAsync(new GetSiteLocalizedContentsQuery(id));

        return HandleResult(result);
    }
}
