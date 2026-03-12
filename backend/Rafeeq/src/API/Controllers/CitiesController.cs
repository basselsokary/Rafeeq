using API.Controllers.Base;
using Application.Commands.Cities;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Cities;
using Application.DTOs.Common;
using Application.Queries.Cities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class CitiesController : ApiBaseController
{
	[HttpGet]
	public async Task<IActionResult> GetAll(
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] IQueryHandler<GetCitiesQuery, PagedResult<CityListDto>> queryHandler)
	{
		var paging = new PagingParameters(page, pageSize);

		var query = new GetCitiesQuery(paging);
		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

	[HttpGet("summaries")]
	public async Task<IActionResult> GetSummaries(
		[FromServices] IQueryHandler<GetCitySummariesQuery, List<CitySummaryDto>> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetCitySummariesQuery());

		return HandleResult(result);
	}

	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetCityByIdQuery, CityDetailDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetCityByIdQuery(id));

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
