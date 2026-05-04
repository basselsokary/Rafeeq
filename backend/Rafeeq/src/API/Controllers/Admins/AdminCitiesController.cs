using API.Controllers.Base;
using API.DTOs;
using Application.Commands.Cities;
using Application.Commands.Cities.LocalizedContents;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Cities;
using Application.Queries.Cities;
using Application.Queries.Cities.LocalizedContents;
using Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admins/cities/")]
[Authorize(Roles = UserRoles.Admin)]
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
	public async Task<IActionResult> Create(
		[FromForm] CreateCityRequest request,
		IFormFile Image,
		[FromServices] ICommandHandler<CreateCityCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{		
		await using var imageStream = Image.OpenReadStream();
		var command = new CreateCityCommand(
			imageStream,
			Image.FileName,
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
		IFormFile Image,
		[FromServices] ICommandHandler<UpdateCityCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		await using var imageStream = Image.OpenReadStream();
		var command = new UpdateCityCommand(
			id,
			Image == null || Image.Length == 0 ? null : imageStream,
			Image?.FileName,
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
}
