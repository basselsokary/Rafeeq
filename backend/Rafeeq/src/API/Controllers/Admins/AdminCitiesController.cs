using API.Controllers.Base;
using Application.Commands.Cities;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Cities;
using Application.Queries.Cities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admin/cities/")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminCitiesController : ApiBaseController
{
    [HttpGet("{id:guid}")]
	public async Task<IActionResult> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetCityByIdForAdminQuery, CityAdminDetailDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetCityByIdForAdminQuery(id));

		return HandleResult(result);
	}

    [HttpPost]
	public async Task<IActionResult> Create(
		[FromBody] CreateCityCommand command,
		[FromServices] ICommandHandler<CreateCityCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	public record UpdateCityRequest(
		string Name,
		string Description,
		GeoLocation CenterLocation,
		int DisplayOrder,
		string? ImageUrl);

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(
		[FromRoute] Guid id,
		[FromBody] UpdateCityRequest request,
		[FromServices] ICommandHandler<UpdateCityCommand> commandHandler)
	{
		var command = new UpdateCityCommand(
			id,
			request.Name,
			request.Description,
			request.CenterLocation,
			request.DisplayOrder,
			request.ImageUrl);

		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(
		[FromRoute] Guid id,
		[FromServices] ICommandHandler<DeleteCityCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new DeleteCityCommand(id));

		return HandleResult(result);
	}
}
