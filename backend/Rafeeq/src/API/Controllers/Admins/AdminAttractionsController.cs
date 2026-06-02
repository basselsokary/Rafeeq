using API.Controllers.Base;
using API.DTOs;
using Application.Commands.Attractions;
using Application.Commands.Attractions.Images;
using Application.Commands.Attractions.LocalizedContents;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Admins;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Application.Queries.Attractions;
using Application.Queries.Attractions.Images;
using Application.Queries.Attractions.LocalizedContents;
using Application.Services;
using Domain.Enums;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admins/attractions/")]
[Authorize(Policy = Policies.CanManageAttractions)]
public class AdminAttractionsController : ApiBaseController
{
	#region Basic CRUD Operations
    [HttpGet("{id:guid}")]
	public async Task<ActionResult<AttractionAdminDetailDto>> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetAttractionByIdForAdminQuery, AttractionAdminDetailDto> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetAttractionByIdForAdminQuery(id);
		var result = await queryHandler.HandleAsync(query, cancellationToken);
		return HandleResult(result);
	}

    [HttpGet]
	public async Task<ActionResult<PagedResult<AttractionListDto>>> GetAll(
		[FromQuery] string? searchTerm,
		[FromQuery] AttractionType? type,
		[FromServices] IQueryHandler<GetAllAttractionsQuery, PagedResult<AttractionListDto>> queryHandler,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20,
		CancellationToken cancellationToken = default)
	{
		var paging = new PagingParameters(page, pageSize);
		var query = new GetAllAttractionsQuery(searchTerm, type, paging);
		
		var result = await queryHandler.HandleAsync(query, cancellationToken);
		return HandleResult(result);
	}

	public sealed record CreateAttractionRequest(Guid SiteId, string Name, string Description, string? LocationDescription, AttractionType Type, bool IsFeatured, LocationRequest? Location, List<HistoricalPeriod> HistoricalPeriod);
	
	[HttpPost]
	public async Task<ActionResult<Guid>> Create(
		[FromBody] CreateAttractionRequest request,
		[FromServices] ICommandHandler<CreateAttractionCommand, Guid> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var command = new CreateAttractionCommand(
			request.SiteId,
			request.Name,
			request.Description,
			request.LocationDescription,
			request.Type,
			request.IsFeatured,
			request.Location?.Latitude,
			request.Location?.Longitude,
			request.HistoricalPeriod);
			
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}

	public sealed record UpdateAttractionRequest(AttractionType Type, List<HistoricalPeriod> HistoricalPeriod, LocationRequest ExactLocation);

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(
		[FromRoute] Guid id,
		[FromBody] UpdateAttractionRequest request,
		[FromServices] ICommandHandler<UpdateAttractionCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var command = new UpdateAttractionCommand(
			id,
			request.Type,
			request.HistoricalPeriod,
			request.ExactLocation.Latitude,
			request.ExactLocation.Longitude);

		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(
		[FromRoute] Guid id,
		[FromServices] ICommandHandler<DeleteAttractionCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var command = new DeleteAttractionCommand(id);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Partial Updates (PATCH)
	public sealed record MarkAttractionAsFeaturedRequest(bool IsFeatured);

	[HttpPatch("{id:guid}")]
	public async Task<IActionResult> MarkAsFeatured(
		[FromRoute] Guid id,
		[FromBody] MarkAttractionAsFeaturedRequest request,
		[FromServices] ICommandHandler<MarkAttractionAsFeaturedCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var command = new MarkAttractionAsFeaturedCommand(id, request.IsFeatured);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}

	[HttpPatch("{id:guid}/set-main-image/{imageId:guid}")]
	public async Task<IActionResult> SetMainImage(
		[FromRoute] Guid id,
		[FromRoute] Guid imageId,
		[FromServices] ICommandHandler<SetMainAttractionImageCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var command = new SetMainAttractionImageCommand(id, imageId);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Images Management
	[HttpGet("{id:guid}/images")]
	public async Task<ActionResult<List<ImageDto>>> GetImages(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetAttractionImagesByIdQuery, List<ImageDto>> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetAttractionImagesByIdQuery(id);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

	[HttpGet("{id:guid}/images/{imageId:guid}")]
	public async Task<ActionResult<ImageDto>> GetImageById(
		[FromRoute] Guid id,
		[FromRoute] Guid imageId,
		[FromServices] IQueryHandler<GetAttractionImageByIdQuery, ImageDto> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetAttractionImageByIdQuery(id, imageId);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}
	public sealed record AddAttractionImagesRequest(List<AddAttractionImageItem> Images);
    public sealed record AddAttractionImageItem(IFormFile Image, bool IsMain, int DisplayOrder, string? Caption);

	[HttpPost("{id:guid}/images")]
	public async Task<IActionResult> AddImages(
		[FromRoute] Guid id,
		[FromForm] AddAttractionImagesRequest request,
		[FromServices] ICommandHandler<AddAttractionImagesCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var images = new List<AddAttractionImageDto>();

		foreach (var item in request.Images)
		{
			var imageStream = new MemoryStream();
			await item.Image.CopyToAsync(imageStream, cancellationToken);
			imageStream.Position = 0;

			images.Add(new AddAttractionImageDto(
				new FileUploadInput(imageStream, item.Image.FileName, item.Image.Length),
				item.IsMain,
				item.DisplayOrder,
				item.Caption));
		}

		var command = new AddAttractionImagesCommand(id, images);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}/images")]
	public async Task<IActionResult> RemoveImages(
		[FromRoute] Guid id,
		[FromBody] List<Guid> imageIds,
		[FromServices] ICommandHandler<RemoveAttractionImagesCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var command = new RemoveAttractionImagesCommand(id, imageIds);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Localized Contents
	[HttpGet("{id:guid}/localized-contents")]
	public async Task<ActionResult<List<AttractionLocalizedContentDto>>> GetLocalizedContents(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetAttractionLocalizedContentsQuery, List<AttractionLocalizedContentDto>> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetAttractionLocalizedContentsQuery(id);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

	[HttpPost("{id:guid}/localized-contents")]
	public async Task<IActionResult> AddLocalizedContents(
		[FromRoute] Guid id,
		[FromBody] List<AddAttractionLocalizedContentsDtoCommand> request,
		[FromServices] ICommandHandler<AddAttractionLocalizedContentsCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var command = new AddAttractionLocalizedContentsCommand(id, request);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}

	[HttpPut("{id:guid}/localized-contents")]
	public async Task<IActionResult> UpdateLocalizedContents(
		[FromRoute] Guid id,
		[FromBody] List<UpdateAttractionLocalizedContentsDtoCommand> request,
		[FromServices] ICommandHandler<UpdateAttractionLocalizedContentCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var command = new UpdateAttractionLocalizedContentCommand(id, request);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Dashboard
	[HttpGet("dashboard")]
	public async Task<ActionResult<AdminAttractionDashboardDto>> GetDashboard(
		[FromServices] IQueryHandler<GetAttractionsDashboardQuery, AdminAttractionDashboardDto> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetAttractionsDashboardQuery();
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	[HttpPost("import")]
    [Authorize(Policy = Policies.CanImportData)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImportAttractionsResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImportAttractionsResultDto>> ImportAttractions(
        IFormFile file,
        [FromServices] ICommandHandler<ImportAttractionsCommand, ImportAttractionsResultDto> commandHandler,
        [FromQuery] bool dryRun = true)
    {
        await using var stream = file.OpenReadStream(); 
        var command = new ImportAttractionsCommand(stream, file.FileName, dryRun);
        var result = await commandHandler.HandleAsync(command);
        
        return HandleResult(result);
    }
}
