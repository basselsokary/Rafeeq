using API.Controllers.Base;
using API.RequestDTOs;
using Application.Commands.Sites;
using Application.Commands.Sites.Facilities;
using Application.Commands.Sites.Images;
using Application.Commands.Sites.LocalizedContents;
using Application.Commands.Sites.OpeningHours;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.Sites;
using Application.Queries.Sites;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admin/sites/")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminSitesController : ApiBaseController
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetSiteByIdForAdminQuery, SiteAdminDetailDto> queryHandler)
    {
        var result = await queryHandler.HandleAsync(new GetSiteByIdForAdminQuery(id));

        return HandleResult(result);
    }

    public record CreateSiteRequest(
        Guid CityId,
        string Name,
        string Description,
        SiteType Type,
        LocationRequest Location,
        AddressRequest Address);

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSiteRequest request,
        [FromServices] ICommandHandler<CreateSiteCommand> commandHandler)
    {
        var command = new CreateSiteCommand(
            request.CityId,
            request.Name,
            request.Description,
            request.Type,
            request.Location.Latitude,
            request.Location.Longitude,
            request.Address.Street,
            request.Address.City,
            request.Address.Region,
            request.Address.PostalCode);

        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    public record UpdateSiteRequest(
        string Name,
        string Description,
        SiteType Type,
        LocationRequest Location,
        AddressRequest Address,
        decimal? Fee);

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
            request.Location.Latitude,
            request.Location.Longitude,
            request.Address.Street,
            request.Address.City,
            request.Address.Region,
            request.Address.PostalCode,
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

    public record ActivateSiteRequest(bool Activate);

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
    
    public record SetStatusRequest(SiteStatus Status);

    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(
        [FromRoute] Guid id,
        [FromBody] SetStatusRequest request,
        [FromServices] ICommandHandler<SetSiteStatusCommand> commandHandler)
    {
        var command = new SetSiteStatusCommand(id, request.Status);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    public record MarkSiteAsFeaturedRequest(bool IsFeatured);

	[HttpPut("{id:guid}/featured")]
	public async Task<IActionResult> MarkAsFeatured(
		[FromRoute] Guid id,
		[FromBody] MarkSiteAsFeaturedRequest request,
		[FromServices] ICommandHandler<MarkSiteAsFeaturedCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new MarkSiteAsFeaturedCommand(id, request.IsFeatured));

		return HandleResult(result);
	}

    [HttpPost("{id:guid}/localized-contents")]
    public async Task<IActionResult> AddLocalizedContents(
        [FromRoute] Guid id,
        [FromBody] List<AddSiteLocalizedContentsDtoCommand> request,
        [FromServices] ICommandHandler<AddSiteLocalizedContentsCommand> commandHandler)
    {
        var command = new AddSiteLocalizedContentsCommand(id, request);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    public record UpdateSiteLocalizedContentRequest(
		Guid ContentId,
		string Name,
		string Description);

	[HttpPut("{id:guid}/localized-contents")]
	public async Task<IActionResult> UpdateLocalizedContent(
		[FromRoute] Guid id,
		[FromBody] UpdateSiteLocalizedContentRequest request,
		[FromServices] ICommandHandler<UpdateSiteLocalizedContentCommand> commandHandler)
	{
		var command = new UpdateSiteLocalizedContentCommand(id, request.ContentId, request.Name, request.Description);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

    [HttpPost("{id:guid}/images")]
    public async Task<IActionResult> AddImages(
        [FromRoute] Guid id,
        [FromBody] List<AddSiteImageDto> request,
        [FromServices] ICommandHandler<AddSiteImagesCommand> commandHandler)
    {
        var command = new AddSiteImagesCommand(id, request);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/images")]
    public async Task<IActionResult> RemoveImages(
        [FromRoute] Guid id,
        [FromBody] List<Guid> imageIds,
        [FromServices] ICommandHandler<RemoveSiteImagesCommand> commandHandler)
    {
        var result = await commandHandler.HandleAsync(new RemoveSiteImagesCommand(id, imageIds));

        return HandleResult(result);
    }

    [HttpPost("{id:guid}/facilities")]
    public async Task<IActionResult> AddFacilities(
        [FromRoute] Guid id,
        [FromBody] List<AddSiteFacilitiesDtoCommand> request,
        [FromServices] ICommandHandler<AddSiteFacilitiesCommand> commandHandler)
    {
        var command = new AddSiteFacilitiesCommand(id, request);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/facilities")]
    public async Task<IActionResult> RemoveFacilities(
        [FromRoute] Guid id,
        [FromBody] List<Guid> facilityIds,
        [FromServices] ICommandHandler<RemoveSiteFacilitiesCommand> commandHandler)
    {
        var result = await commandHandler.HandleAsync(new RemoveSiteFacilitiesCommand(id, facilityIds));

        return HandleResult(result);
    }

    [HttpPost("{id:guid}/opening-hours")]
    public async Task<IActionResult> AddOpeningHours(
        [FromRoute] Guid id,
        [FromBody] List<AddSiteOpeningHoursDtoCommand> request,
        [FromServices] ICommandHandler<AddSiteOpeningHoursCommand> commandHandler)
    {
        var command = new AddSiteOpeningHoursCommand(id, request);
        var result = await commandHandler.HandleAsync(command);

        return HandleResult(result);
    }
}