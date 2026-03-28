using API.Controllers.Base;
using API.RequestDTOs;
using Application.Commands.Attractions;
using Application.Commands.Attractions.Images;
using Application.Commands.Attractions.LocalizedContents;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Attractions;
using Application.Queries.Attractions;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admin/attractions/")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminAttractionsController : ApiBaseController
{
    [HttpGet("/{id:guid}")]
	public async Task<IActionResult> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetAttractionByIdForAdminQuery, AttractionAdminDetailDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetAttractionByIdForAdminQuery(id));
		return HandleResult(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create(
		[FromBody] CreateAttractionCommand command,
		[FromServices] ICommandHandler<CreateAttractionCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	public record UpdateAttractionRequest(
		string Name,
		string Description,
		AttractionType Type,
		HistoricalPeriod HistoricalPeriod,
		LocationRequest ExactLocation,
		string? LocationDescription);

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(
		[FromRoute] Guid id,
		[FromBody] UpdateAttractionRequest request,
		[FromServices] ICommandHandler<UpdateAttractionCommand> commandHandler)
	{
		var command = new UpdateAttractionCommand(
			id,
			request.Name,
			request.Description,
			request.Type,
			request.HistoricalPeriod,
			request.ExactLocation.Latitude,
			request.ExactLocation.Longitude,
			request.LocationDescription);

		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(
		[FromRoute] Guid id,
		[FromServices] ICommandHandler<DeleteAttractionCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new DeleteAttractionCommand(id));

		return HandleResult(result);
	}

	public record MarkAttractionAsFeaturedRequest(bool IsFeatured);

	[HttpPut("{id:guid}/featured")]
	public async Task<IActionResult> MarkAsFeatured(
		[FromRoute] Guid id,
		[FromBody] MarkAttractionAsFeaturedRequest request,
		[FromServices] ICommandHandler<MarkAttractionAsFeaturedCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new MarkAttractionAsFeaturedCommand(id, request.IsFeatured));

		return HandleResult(result);
	}

	[HttpPost("{id:guid}/images")]
	public async Task<IActionResult> AddImages(
		[FromRoute] Guid id,
		[FromBody] List<AddAttractionImageDto> request,
		[FromServices] ICommandHandler<AddAttractionImagesCommand> commandHandler)
	{
		var command = new AddAttractionImagesCommand(id, request);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}/images")]
	public async Task<IActionResult> RemoveImages(
		[FromRoute] Guid id,
		[FromBody] List<Guid> imageIds,
		[FromServices] ICommandHandler<RemoveAttractionImagesCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new RemoveAttractionImagesCommand(id, imageIds));

		return HandleResult(result);
	}

	public record AddAttractionLocalizedContentRequest(
		LanguageCode Language,
		string Name,
		string Description);

	[HttpPost("{id:guid}/localized-contents")]
	public async Task<IActionResult> AddLocalizedContents(
		[FromRoute] Guid id,
		[FromBody] List<AddAttractionLocalizedContentsDtoCommand> request,
		[FromServices] ICommandHandler<AddAttractionLocalizedContentsCommand> commandHandler)
	{
		var command = new AddAttractionLocalizedContentsCommand(id, request);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	public record UpdateAttractionLocalizedContentRequest(
		Guid ContentId,
		string Name,
		string Description);

	[HttpPut("{id:guid}/localized-contents")]
	public async Task<IActionResult> UpdateLocalizedContent(
		[FromRoute] Guid id,
		[FromBody] UpdateAttractionLocalizedContentRequest request,
		[FromServices] ICommandHandler<UpdateAttractionLocalizedContentCommand> commandHandler)
	{
		var command = new UpdateAttractionLocalizedContentCommand(id, request.ContentId, request.Name, request.Description);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}
}
