using API.Controllers.Base;
using API.DTOs;
using Application.Commands.Cities;
using Application.Commands.Cities.LocalizedContents;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Admins;
using Application.DTOs.Cities;
using Application.Queries.Cities;
using Application.Queries.Cities.LocalizedContents;
using Application.Services;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admins/cities/")]
[Authorize(Policy = Policies.CanManageCities)]
public class AdminCitiesController : ApiBaseController
{
	#region Basic CRUD Operations
    [HttpGet("{id:guid}")]
	public async Task<ActionResult<CityAdminDetailDto>> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetCityByIdForAdminQuery, CityAdminDetailDto> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetCityByIdForAdminQuery(id);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}
    
	public sealed record CreateCityRequest(string Name, string Description, LocationRequest CenterLocation, int DisplayOrder);
	
	[HttpPost]
	public async Task<ActionResult<Guid>> Create(
		[FromForm] CreateCityRequest request,
		IFormFile image,
		[FromServices] ICommandHandler<CreateCityCommand, Guid> commandHandler,
		CancellationToken cancellationToken = default)
	{		
		await using var imageStream = image.OpenReadStream();
		var command = new CreateCityCommand(
			new FileUploadInput(imageStream, image.FileName, image.Length),
			request.Name,
			request.Description,
			request.CenterLocation.Latitude,
			request.CenterLocation.Longitude,
			request.DisplayOrder);

		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}

	public sealed record UpdateCityRequest(LocationRequest CenterLocation, int DisplayOrder);

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(
		[FromRoute] Guid id,
		[FromForm] UpdateCityRequest request,
		[FromServices] ICommandHandler<UpdateCityCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var command = new UpdateCityCommand(
			id,
			request.CenterLocation.Latitude,
			request.CenterLocation.Longitude,
			request.DisplayOrder);

		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(
		[FromRoute] Guid id,
		[FromServices] ICommandHandler<DeleteCityCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var command = new DeleteCityCommand(id);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Partial Updates (PATCH)
	[HttpPatch("{id:guid}/image")]
	public async Task<ActionResult> SetImage(
		[FromRoute] Guid id,
		IFormFile image,
		[FromServices] ICommandHandler<SetCityImageCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		await using var imageStream = image.OpenReadStream();
		var command = new SetCityImageCommand(
			id,
			new FileUploadInput(imageStream, image.FileName, image.Length));

		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Localized Contents
	[HttpGet("{id:guid}/localized-contents")]
	public async Task<ActionResult<List<CityLocalizedContentDto>>> GetLocalizedContents(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetCityLocalizedContentsQuery, List<CityLocalizedContentDto>> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetCityLocalizedContentsQuery(id);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

	[HttpPost("{id:guid}/localized-contents")]
    public async Task<IActionResult> AddLocalizedContents(
        [FromRoute] Guid id,
        [FromBody] List<AddCityLocalizedContentsDtoCommand> request,
        [FromServices] ICommandHandler<AddCityLocalizedContentsCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new AddCityLocalizedContentsCommand(id, request);
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

	[HttpPut("{id:guid}/localized-contents")]
	public async Task<IActionResult> UpdateLocalizedContents(
		[FromRoute] Guid id,
		[FromBody] List<UpdateCityLocalizedContentsDtoCommand> request,
		[FromServices] ICommandHandler<UpdateCityLocalizedContentCommand> commandHandler,
        CancellationToken cancellationToken = default)
	{
		var command = new UpdateCityLocalizedContentCommand(id, request);
		var result = await commandHandler.HandleAsync(command, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	#region Dashboard
	[HttpGet("dashboard")]
	public async Task<ActionResult<AdminCityDashboardDto>> GetDashboard(
		[FromServices] IQueryHandler<GetCityDashboardQuery, AdminCityDashboardDto> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetCityDashboardQuery();
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}
	#endregion

	[HttpPost("import")]
    [Authorize(Policy = Policies.CanImportData)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImportCitiesResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImportCitiesResultDto>> ImportCities(
        IFormFile file,
        [FromServices] ICommandHandler<ImportCitiesCommand, ImportCitiesResultDto> commandHandler,
        [FromQuery] bool dryRun = true)
    {
        await using var stream = file.OpenReadStream(); 
        var command = new ImportCitiesCommand(stream, file.FileName, dryRun);
        var result = await commandHandler.HandleAsync(command);
        
        return HandleResult(result);
    }
}
