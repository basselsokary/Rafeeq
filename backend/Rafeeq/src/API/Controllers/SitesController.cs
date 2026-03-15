using API.Controllers.Base;
using Application.Commands.Sites;
using Application.Commands.Sites.Facilities;
using Application.Commands.Sites.Images;
using Application.Commands.Sites.LocalizedContents;
using Application.Commands.Sites.OpeningHours;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.Sites;
using Application.Queries.Sites;
using Application.Queries.Sites.LocalizedContents;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class SitesController() : ApiBaseController
{
    #region Queries
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] SiteType? type,
        [FromQuery] Guid? city,
        [FromQuery] bool? isFree,
        [FromQuery] int? minRating,
        [FromQuery] int? maxRating,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromServices] IQueryHandler<GetSitesQuery, PagedResult<SiteListDto>> queryHandler)
    {
        var filters = new SiteFilters(type, city, isFree, minRating, maxRating);
        var paging = new PagingParameters(page, pageSize);

        var query = new GetSitesQuery(searchTerm, filters, paging);
        var result = await queryHandler.HandleAsync(query);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSiteByIdQuery, SiteDetailDto> queryHandler)
    {
        var query = new GetSiteByIdQuery(id);
        var result = await queryHandler.HandleAsync(query);

        return HandleResult(result);
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured(
        [FromQuery] Guid? city,
        [FromServices] IQueryHandler<GetFeaturedSitesQuery, List<SiteListDto>> queryHandler)
    {
        var query = new GetFeaturedSitesQuery(city);
        var result = await queryHandler.HandleAsync(query);

        return HandleResult(result);
    }

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(
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
    public async Task<IActionResult> GetWithinBounds(
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

	#region Localized Contents
    [HttpGet("{id:guid}/localized-contents")]
    public async Task<IActionResult> GetLocalizedContents(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSiteLocalizedContentsQuery, List<LocalizedContentDto>> queryHandler)
    {
        var result = await queryHandler.HandleAsync(new GetSiteLocalizedContentsQuery(id));

        return HandleResult(result);
    }
    #endregion
    #endregion

    #region Commands
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSiteCommand command,
        [FromServices] ICommandHandler<CreateSiteCommand> commandHandler)
    {
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    public record UpdateSiteRequest(
        string Name,
        string Description,
        SiteType Type,
        GeoLocation Location,
        Address Address,
        Money? Fee);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateSiteRequest request,
        [FromServices] ICommandHandler<UpdateSiteCommand> commandHandler)
    {
        var command = new UpdateSiteCommand(
            id,
            request.Name,
            request.Description,
            request.Type,
            request.Location,
            request.Address,
            request.Fee);

        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<DeleteSiteCommand> commandHandler)
    {
        var command = new DeleteSiteCommand(id);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

     public record ActivateSiteRequest(
        bool Activate);

    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        [FromRoute] Guid id,
        [FromBody] ActivateSiteRequest request,
        [FromServices] ICommandHandler<ActivateSiteCommand> commandHandler)
    {
        var command = new ActivateSiteCommand(id, request.Activate);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    public record SetContactInfoRequest(
        string Phone,
        string? WebsiteUrl);

    [HttpPost("{id:guid}/contact-info")]
    public async Task<IActionResult> SetContactInfo(
        [FromRoute] Guid id,
        [FromBody] SetContactInfoRequest request,
        [FromServices] ICommandHandler<SetSiteContactInfoCommand> commandHandler)
    {
        var command = new SetSiteContactInfoCommand(id, request.Phone, request.WebsiteUrl);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    public record SetSiteStatusRequest(
        SiteStatus Status);
    
    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(
        [FromRoute] Guid id,
        [FromBody] SetSiteStatusRequest request,
        [FromServices] ICommandHandler<SetSiteStatusCommand> commandHandler)
    {
        var command = new SetSiteStatusCommand(id, request.Status);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }    
    
	#region Localized Contents
    public record AddSiteLocalizedContentRequest(
        LanguageCode Language,
        string Name,
        string Description);

    [HttpPost("{id:guid}/localized-contents")]
    public async Task<IActionResult> AddLocalizedContent(
        [FromRoute] Guid id,
        [FromBody] AddSiteLocalizedContentRequest request,
        [FromServices] ICommandHandler<AddSiteLocalizedContentCommand> commandHandler)
    {
        var command = new AddSiteLocalizedContentCommand(id, request.Language, request.Name, request.Description);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }
    #endregion

	#region Images
    public record AddSiteImageRequest(
        string ImageUrl,
        bool IsMain,
        string? Caption);

    [HttpPost("{id:guid}/images")]
    public async Task<IActionResult> AddImage(
        [FromRoute] Guid id,
        [FromBody] AddSiteImageRequest request,
        [FromServices] ICommandHandler<AddSiteImagesCommand> commandHandler)
    {
        var command = new AddSiteImagesCommand(id, request.ImageUrl, request.IsMain, request.Caption);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> RemoveImage(
        [FromRoute] Guid id,
        [FromRoute] Guid imageId,
        [FromServices] ICommandHandler<RemoveSiteImagesCommand> commandHandler)
    {
        var result = await commandHandler.HandleAsync(new RemoveSiteImagesCommand(id, imageId));

        return HandleResult(result);
    }
    #endregion

	#region Facilities
    public record AddSiteFacilityRequest(
        string FacilityName,
        string FacilityDescription);

    [HttpPost("{id:guid}/facilities")]
    public async Task<IActionResult> AddFacility(
        [FromRoute] Guid id,
        [FromBody] AddSiteFacilityRequest request,
        [FromServices] ICommandHandler<AddSiteFacilityCommand> commandHandler)
    {
        var command = new AddSiteFacilityCommand(id, request.FacilityName, request.FacilityDescription);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/facilities/{facilityId:guid}")]
    public async Task<IActionResult> RemoveFacility(
        [FromRoute] Guid id,
        [FromRoute] Guid facilityId,
        [FromServices] ICommandHandler<RemoveSiteFacilityCommand> commandHandler)
    {
        var result = await commandHandler.HandleAsync(new RemoveSiteFacilityCommand(id, facilityId));

        return HandleResult(result);
    }
    #endregion

    #region Opening Hours
    public record AddOpeningHoursRequest(
        DayOfWeek DayOfWeek,
        TimeSpan StartTime,
        TimeSpan EndTime,
        bool IsClosed);

    [HttpPost("{id:guid}/opening-hours")]
    public async Task<IActionResult> AddOpeningHours(
        [FromRoute] Guid id,
        [FromBody] AddOpeningHoursRequest request,
        [FromServices] ICommandHandler<AddSiteOpeningHoursCommand> commandHandler)
    {
        var timeRangeResult = TimeRange.Create(request.StartTime, request.EndTime);
        if (timeRangeResult.Failed)
            return BadRequest(timeRangeResult.Error.Message);

        var command = new AddSiteOpeningHoursCommand(id, request.DayOfWeek, timeRangeResult.Value, request.IsClosed);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }
    #endregion
    #endregion
}
