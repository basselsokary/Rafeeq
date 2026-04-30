using API.Controllers.Base;
using API.DTOs;
using Application.Commands.Sites;
using Application.Commands.Sites.Facilities;
using Application.Commands.Sites.Images;
using Application.Commands.Sites.LocalizedContents;
using Application.Commands.Sites.OpeningHours;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Admins;
using Application.DTOs.Common;
using Application.Queries.Sites;
using Application.Queries.Sites.Facilities;
using Application.Queries.Sites.Images;
using Application.Queries.Sites.LocalizedContents;
using Application.Queries.Sites.NearestTransportations;
using Application.Queries.Sites.OpeningHours;
using Domain.Common.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admins/sites/")]
//  [Authorize(Roles = UserRoles.Admin)]
public class AdminSitesController : ApiBaseController
{
    #region Basic CRUD Operations
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminSiteDetailDto>> GetById(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSiteByIdForAdminQuery, AdminSiteDetailDto> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSiteByIdForAdminQuery(id);
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    public record CreateSiteRequest(Guid CityId, string Name, string Description, string Address, SiteType Type, LocationRequest Location, int EstimatedDurationMinutes, TicketRequest Ticket, ContactInfo? ContactInfo);
    public record ContactInfo(string? Phone, string? WebsiteUrl);

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSiteRequest request,
        [FromServices] ICommandHandler<CreateSiteCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateSiteCommand(
            request.CityId,
            request.Name,
            request.Description,
            request.Address,
            request.Type,
            request.Location.Latitude,
            request.Location.Longitude,
            request.EstimatedDurationMinutes,
            request.Ticket.EgyptianTicketPrice,
            request.Ticket.ForeignerTicketPrice,
            request.Ticket.ForeignerCurrency,
            request.Ticket.Notes,
            request.Ticket.IsFree,
            request.ContactInfo?.Phone,
            request.ContactInfo?.WebsiteUrl);

        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    public record UpdateSiteRequest(SiteType Type, LocationRequest Location, int EstimatedDurationMinutes, TicketRequest Ticket, ContactInfo? ContactInfo);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateSiteRequest request,
        [FromServices] ICommandHandler<UpdateSiteCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateSiteCommand(
            id,
            request.Type,
            request.Location.Latitude,
            request.Location.Longitude,
            request.EstimatedDurationMinutes,
            request.Ticket.EgyptianTicketPrice,
            request.Ticket.ForeignerTicketPrice,
            request.Ticket.ForeignerCurrency,
            request.Ticket.IsFree,
            request.ContactInfo?.Phone,
            request.ContactInfo?.WebsiteUrl);

        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<DeleteSiteCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteSiteCommand(id);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }
    #endregion

	#region Partial Updates (PATCH)
    public record SiteStatusUpdateRequest(SiteStatus Status, bool IsFeatured, bool IsHiddenGem);

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] Guid id,
        [FromBody] SiteStatusUpdateRequest request,
        [FromServices] ICommandHandler<UpdateSiteStatusCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateSiteStatusCommand(
            id,
            request.Status,
            request.IsFeatured,
            request.IsHiddenGem);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }
    #endregion

    #region Localized Contents
    [HttpGet("{id:guid}/localized-contents")]
    public async Task<ActionResult<List<AdminSiteLocalizedContentDto>>> GetLocalizedContents(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSiteLocalizedContentsQuery, List<AdminSiteLocalizedContentDto>> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSiteLocalizedContentsQuery(id);
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{id:guid}/localized-contents")]
    public async Task<IActionResult> AddLocalizedContents(
        [FromRoute] Guid id,
        [FromBody] List<AddSiteLocalizedContentsDtoCommand> request,
        [FromServices] ICommandHandler<AddSiteLocalizedContentsCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new AddSiteLocalizedContentsCommand(id, request);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

	[HttpPut("{id:guid}/localized-contents")]
	public async Task<IActionResult> UpdateLocalizedContents(
		[FromRoute] Guid id,
		[FromBody] List<UpdateSiteLocalizedContentsDto> request,
		[FromServices] ICommandHandler<UpdateSiteLocalizedContentCommand> commandHandler,
        CancellationToken cancellationToken = default)
	{
		var command = new UpdateSiteLocalizedContentCommand(id, request);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
    #endregion

    #region Images
    [HttpGet("{id:guid}/images")]
    public async Task<ActionResult<List<ImageDto>>> GetImages(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSiteImagesByIdQuery, List<ImageDto>> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSiteImagesByIdQuery(id);
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}/images/{imageId:guid}")]
    public async Task<ActionResult<ImageDto>> GetImageById(
        [FromRoute] Guid id,
        [FromRoute] Guid imageId,
        [FromServices] IQueryHandler<GetSiteImageByIdQuery, ImageDto> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSiteImageByIdQuery(id, imageId);
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    public sealed record AddSiteImagesRequest(List<AddSiteImageItem> Images);
    public sealed record AddSiteImageItem(IFormFile Image, bool IsMain, int DisplayOrder, string? Caption);

    [HttpPost("{id:guid}/images")]
    public async Task<IActionResult> AddImages(
        [FromRoute] Guid id,
        [FromForm] AddSiteImagesRequest request,
        [FromServices] ICommandHandler<AddSiteImagesCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var images = new List<AddSiteImageDto>();

		foreach (var item in request.Images)
		{
			var imageStream = new MemoryStream();
			await item.Image.CopyToAsync(imageStream, cancellationToken);
			imageStream.Position = 0;

			images.Add(new AddSiteImageDto(
				imageStream,
				item.Image.FileName,
				item.IsMain,
				item.DisplayOrder,
				item.Caption));
		}

        var command = new AddSiteImagesCommand(id, images);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/images")]
    public async Task<IActionResult> RemoveImages(
        [FromRoute] Guid id,
        [FromBody] List<Guid> imageIds,
        [FromServices] ICommandHandler<RemoveSiteImagesCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveSiteImagesCommand(id, imageIds);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }
    #endregion

    #region Facilities
    [HttpGet("{id:guid}/facilities")]
    public async Task<ActionResult<List<AdminSiteFacilityDto>>> GetFacilities(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSiteFacilitiesByIdQuery, List<AdminSiteFacilityDto>> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSiteFacilitiesByIdQuery(id);
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{id:guid}/facilities")]
    public async Task<IActionResult> AddFacilities(
        [FromRoute] Guid id,
        [FromBody] List<FacilityType> facilities,
        [FromServices] ICommandHandler<AddSiteFacilitiesCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new AddSiteFacilitiesCommand(id, facilities);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/facilities")]
    public async Task<IActionResult> RemoveFacilities(
        [FromRoute] Guid id,
        [FromBody] List<FacilityType> facilities,
        [FromServices] ICommandHandler<RemoveSiteFacilitiesCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveSiteFacilitiesCommand(id, facilities);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }
    #endregion

    #region Nearest Transportation
    [HttpGet("{id:guid}/nearest-transportation/{transportationId:guid}")]
    public async Task<ActionResult<AdminSiteNearestTransportationDto>> GetNearestTransportationById(
        [FromRoute] Guid id,
        [FromRoute] Guid transportationId,
        [FromServices] IQueryHandler<GetSiteNearestTransportationByIdQuery, AdminSiteNearestTransportationDto> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSiteNearestTransportationByIdQuery(id, transportationId);
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}/nearest-transportation")]
    public async Task<ActionResult<List<AdminSiteNearestTransportationDto>>> GetNearestTransportations(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSiteNearestTransportationsByIdQuery, List<AdminSiteNearestTransportationDto>> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSiteNearestTransportationsByIdQuery(id);
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}/nearest-transportation/{transportationId:guid}/localized-contents/{contentId:guid}")]
    public async Task<ActionResult<AdminSiteNearestTransportationLocalizedContentDto>> GetNearestTransportationLocalizedContentById(
        [FromRoute] Guid id,
        [FromRoute] Guid transportationId,
        [FromRoute] Guid contentId,
        [FromServices] IQueryHandler<GetSiteNearestTransportationLocalizedContentByIdQuery, AdminSiteNearestTransportationLocalizedContentDto> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSiteNearestTransportationLocalizedContentByIdQuery(id, transportationId, contentId);
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }
    // public record AddSiteNearestTransportationRequest(string Name, string? Description, TransportationType Type, LocationRequest Location, double DistanceKm, string Address, List<AddNearestTransportationLocalizedContentDto> LocalizedContents);

    // [HttpPost("{id:guid}/nearest-transportation")]
    // public async Task<IActionResult> AddNearestTransportation(
    //     [FromRoute] Guid id,
    //     [FromBody] AddSiteNearestTransportationRequest request,
    //     [FromServices] ICommandHandler<AddSiteNearestTransportationCommand> commandHandler,
    //     CancellationToken cancellationToken = default)
    // {
    //     var command = new AddSiteNearestTransportationCommand(
    //         id,
    //         request.Type,
    //         request.Location.Latitude,
    //         request.Location.Longitude,
    //         request.DistanceKm,
    //         request.LocalizedContents);

    //     var result = await commandHandler.HandleAsync(command, cancellationToken);

    //     return HandleResult(result);
    // }

    // public sealed record UpdateSiteNearestTransportationRequest(TimeSpan StartOperatingHoursTime, TimeSpan EndOperatingHoursTime, bool IsOperational, bool HasAccessibility);

    // [HttpPut("{id:guid}/nearest-transportation/{transportationId:guid}")]
    // public async Task<IActionResult> UpdateNearestTransportation(
    //     [FromRoute] Guid id,
    //     [FromRoute] Guid transportationId,
    //     [FromBody] UpdateSiteNearestTransportationRequest request,
    //     [FromServices] ICommandHandler<UpdateSiteNearestTransportationCommand> commandHandler,
    //     CancellationToken cancellationToken = default)
    // {
    //     var command = new UpdateSiteNearestTransportationCommand(
    //         id,
    //         transportationId,
    //         request.StartOperatingHoursTime,
    //         request.EndOperatingHoursTime,
    //         request.IsOperational,
    //         request.HasAccessibility);

    //     var result = await commandHandler.HandleAsync(command, cancellationToken);

    //     return HandleResult(result);
    // }

    // [HttpDelete("{id:guid}/nearest-transportations/{transportationId:guid}")]
    // public async Task<IActionResult> RemoveNearestTransportation(
    //     [FromRoute] Guid id,
    //     [FromRoute] Guid transportationId,
    //     [FromServices] ICommandHandler<RemoveSiteNearestTransportationCommand> commandHandler,
    //     CancellationToken cancellationToken = default)
    // {
    //     var command = new RemoveSiteNearestTransportationCommand(id, transportationId);
    //     var result = await commandHandler.HandleAsync(command, cancellationToken);

    //     return HandleResult(result);
    // }
    #endregion

    #region Opening Hours
    [HttpGet("{id:guid}/opening-hours")]
    public async Task<ActionResult<List<AdminSiteOpeningHourDto>>> GetOpeningHours(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSiteOpeningHoursByIdQuery, List<AdminSiteOpeningHourDto>> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSiteOpeningHoursByIdQuery(id);
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{id:guid}/opening-hours")]
    public async Task<IActionResult> AddOpeningHours(
        [FromRoute] Guid id,
        [FromBody] List<AddSiteOpeningHoursDtoCommand> request,
        [FromServices] ICommandHandler<AddSiteOpeningHoursCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new AddSiteOpeningHoursCommand(id, request);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/opening-hours")]
    public async Task<IActionResult> RemoveOpeningHours(
        [FromRoute] Guid id,
        [FromBody] List<WeekDay> weekDays,
        [FromServices] ICommandHandler<RemoveSiteOpeningHoursCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveSiteOpeningHoursCommand(id, weekDays);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }
    #endregion

    #region Dashboard & Statistics
    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminSiteDashboardDto>> GetDashboard(
        [FromServices] IQueryHandler<GetSitesDashboardQuery, AdminSiteDashboardDto> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSitesDashboardQuery();
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }
    #endregion
    
    // public sealed record ImportSitesRequest(IFormFile File);

    // [HttpPost("import")]
    // [Authorize(Policy = "SuperAdminOnly")]
    // [Consumes("multipart/form-data")]
    // [ProducesResponseType(typeof(ImportSitesResultDto), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // public async Task<ActionResult<ImportSitesResultDto>> ImportSites(
    //     [FromForm] ImportSitesRequest request,
    //     [FromServices] ICommandHandler<ImportSitesCommand, ImportSitesResultDto> commandHandler)
    // {
    //     await using var stream = request.File.OpenReadStream(); 
    //     var command = new ImportSitesCommand(stream, request.File.FileName);
    //     var result = await commandHandler.HandleAsync(command);
        
    //     return HandleResult(result);
    // }
}