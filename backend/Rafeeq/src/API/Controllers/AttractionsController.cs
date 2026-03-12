using API.Controllers.Base;
using Application.Commands.Attractions;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Application.Queries.Attractions;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class AttractionsController : ApiBaseController
{
	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetAttractionByIdQuery, AttractionDetailDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetAttractionByIdQuery(id));

		return HandleResult(result);
	}

	[HttpGet("by-type")]
	public async Task<IActionResult> GetByType(
		[FromQuery] Guid siteId,
		[FromQuery] AttractionType type,
		[FromQuery] int pageNumber,
		[FromQuery] int pageSize,
		[FromServices] IQueryHandler<GetAttractionsByTypeQuery, PagedResult<AttractionListDto>> queryHandler)
	{
		var paging = new PagingParameters(pageNumber, pageSize);

		var result = await queryHandler.HandleAsync(new GetAttractionsByTypeQuery(siteId, type, paging));

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
		GeoLocation? ExactLocation,
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
			request.ExactLocation,
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

	public record AddAttractionImageRequest(
		string ImageUrl,
		bool IsMain,
		string? Caption);

	[HttpPost("{id:guid}/images")]
	public async Task<IActionResult> AddImage(
		[FromRoute] Guid id,
		[FromBody] AddAttractionImageRequest request,
		[FromServices] ICommandHandler<AddAttractionImagesCommand> commandHandler)
	{
		var command = new AddAttractionImagesCommand(id, request.ImageUrl, request.IsMain, request.Caption);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}/images/{imageId:guid}")]
	public async Task<IActionResult> RemoveImage(
		[FromRoute] Guid id,
		[FromRoute] Guid imageId,
		[FromServices] ICommandHandler<RemoveAttractionImagesCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new RemoveAttractionImagesCommand(id, imageId));

		return HandleResult(result);
	}

	public record AddAttractionLocalizedContentRequest(
		LanguageCode Language,
		string Name,
		string Description);

	[HttpPost("{id:guid}/localized-contents")]
	public async Task<IActionResult> AddLocalizedContent(
		[FromRoute] Guid id,
		[FromBody] AddAttractionLocalizedContentRequest request,
		[FromServices] ICommandHandler<AddAttractionLocalizedContentCommand> commandHandler)
	{
		var command = new AddAttractionLocalizedContentCommand(id, request.Language, request.Name, request.Description);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}
}
