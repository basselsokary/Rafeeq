using API.Controllers.Base;
using Application.Commands.Artifacts;
using Application.Commands.Artifacts.Images;
using Application.Commands.Artifacts.LocalizedContents;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Admins;
using Application.DTOs.Artifacts;
using Application.DTOs.Common;
using Application.Queries.Artifacts;
using Application.Queries.Artifacts.Images;
using Application.Queries.Artifacts.LocalizedContents;
using Application.Services;
using Domain.Enums;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admins/artifacts/")]
[Authorize(Policy = Policies.CanManageArtifacts)]
public class AdminArtifactsController : ApiBaseController
{
    #region Basic CRUD Operations
    [HttpGet("{id:guid}")]
	public async Task<ActionResult<ArtifactAdminDetailDto>> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetArtifactByIdForAdminQuery, ArtifactAdminDetailDto> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetArtifactByIdForAdminQuery(id);
		var result = await queryHandler.HandleAsync(query, cancellationToken);
		return HandleResult(result);
	}

    [HttpGet]
	public async Task<ActionResult<PagedResult<ArtifactListDto>>> GetAll(
		[FromQuery] string? searchTerm,
		[FromQuery] ArtifactType? type,
		[FromServices] IQueryHandler<GetAllArtifactsQuery, PagedResult<ArtifactListDto>> queryHandler,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20,
		CancellationToken cancellationToken = default)
	{
		var paging = new PagingParameters(page, pageSize);
		var query = new GetAllArtifactsQuery(searchTerm, type, paging);
		
		var result = await queryHandler.HandleAsync(query, cancellationToken);
		return HandleResult(result);
	}

	[HttpGet("site/{siteId:guid}")]
	public async Task<ActionResult<List<ArtifactListDto>>> GetArtifactsBySiteId(
		[FromRoute] Guid siteId,
		[FromQuery] ArtifactType? type,
		[FromServices] IQueryHandler<GetArtifactsBySiteIdQuery, List<ArtifactListDto>> queryHandler,
		[FromQuery] string? searchTerm = null,
		CancellationToken cancellationToken = default)
	{
		var query = new GetArtifactsBySiteIdQuery(siteId, type, searchTerm);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}
    
    public sealed record CreateArtifactRequest(
        Guid? SiteId,
        string Name,
        string Description,
        ArtifactType Type,
        int DisplayOrder);

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateArtifactRequest request,
        [FromServices] ICommandHandler<CreateArtifactCommand, Guid> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateArtifactCommand(
            request.SiteId,
            request.Name,
            request.Description,
            request.Type,
            request.DisplayOrder);

        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    public sealed record UpdateArtifactRequest(int DisplayOrder, ArtifactType Type, Guid? SiteId);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateArtifactRequest request,
        [FromServices] ICommandHandler<UpdateArtifactCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateArtifactCommand(id, request.DisplayOrder, request.Type, request.SiteId);
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<DeleteArtifactCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteArtifactCommand(id);
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Partial Updates (PATCH)
    [HttpPatch("{id:guid}/set-main-image/{imageId:guid}")]
    public async Task<IActionResult> SetMainImage(
        [FromRoute] Guid id,
        [FromRoute] Guid imageId,
        [FromServices] ICommandHandler<SetMainArtifactImageCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new SetMainArtifactImageCommand(id, imageId);
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Images Management
    [HttpGet("{id:guid}/images")]
	public async Task<ActionResult<List<ImageDto>>> GetImages(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetArtifactImagesByIdQuery, List<ImageDto>> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetArtifactImagesByIdQuery(id);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}
    
    public sealed record AddArtifactImagesRequest(List<AddArtifactImageItem> Images);
    public sealed record AddArtifactImageItem(IFormFile Image, bool IsMain, int DisplayOrder, string? Caption);

    [HttpPost("{id:guid}/images")]
    public async Task<IActionResult> AddImages(
        [FromRoute] Guid id,
        [FromForm] AddArtifactImagesRequest request,
        [FromServices] ICommandHandler<AddArtifactImagesCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var images = new List<AddArtifactImageDto>();

        foreach (var item in request.Images)
        {
            var imageStream = new MemoryStream();
            await item.Image.CopyToAsync(imageStream, cancellationToken);
            imageStream.Position = 0;

            images.Add(new AddArtifactImageDto(
                new FileUploadInput(imageStream, item.Image.FileName, item.Image.Length),
                item.IsMain,
                item.DisplayOrder,
                item.Caption));
        }

        var command = new AddArtifactImagesCommand(id, images);
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/images")]
    public async Task<IActionResult> RemoveImages(
        [FromRoute] Guid id,
        [FromBody] List<Guid> imageIds,
        [FromServices] ICommandHandler<RemoveArtifactImagesCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveArtifactImagesCommand(id, imageIds);
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Localized Contents
    [HttpGet("{id:guid}/localized-contents")]
	public async Task<ActionResult<List<ArtifactLocalizedContentDto>>> GetLocalizedContents(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetArtifactLocalizedContentsQuery, List<ArtifactLocalizedContentDto>> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetArtifactLocalizedContentsQuery(id);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

    [HttpPost("{id:guid}/localized-contents")]
    public async Task<IActionResult> AddLocalizedContents(
        [FromRoute] Guid id,
        [FromBody] List<AddArtifactLocalizedContentsDtoCommand> request,
        [FromServices] ICommandHandler<AddArtifactLocalizedContentsCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new AddArtifactLocalizedContentsCommand(id, request);
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/localized-contents")]
    public async Task<IActionResult> UpdateLocalizedContents(
        [FromRoute] Guid id,
        [FromBody] List<UpdateArtifactLocalizedContentsDtoCommand> request,
        [FromServices] ICommandHandler<UpdateArtifactLocalizedContentCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateArtifactLocalizedContentCommand(id, request);
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Dashboard
    [HttpGet("dashboard")]
	public async Task<ActionResult<AdminArtifactDashboardDto>> GetDashboard(
		[FromServices] IQueryHandler<GetArtifactsDashboardQuery, AdminArtifactDashboardDto> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetArtifactsDashboardQuery();
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	[HttpPost("import")]
    [Authorize(Policy = Policies.CanImportData)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImportArtifactsResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImportArtifactsResultDto>> ImportArtifacts(
        IFormFile file,
        [FromServices] ICommandHandler<ImportArtifactsCommand, ImportArtifactsResultDto> commandHandler,
        [FromQuery] bool dryRun = true)
    {
        await using var stream = file.OpenReadStream(); 
        var command = new ImportArtifactsCommand(stream, file.FileName, dryRun);
        var result = await commandHandler.HandleAsync(command);
        
        return HandleResult(result);
    }
}
