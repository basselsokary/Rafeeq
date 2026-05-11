using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Cities;
using Application.Queries.Cities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class CitiesController : ApiBaseController
{
	[HttpGet]
	public async Task<ActionResult<List<CityListDto>>> GetAll(
		[FromServices] IQueryHandler<GetCitiesQuery, List<CityListDto>> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetCitiesQuery();
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

	[HttpGet("summaries")]
	public async Task<ActionResult<List<CitySummaryDto>>> GetSummaries(
		[FromServices] IQueryHandler<GetCitySummariesQuery, List<CitySummaryDto>> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var result = await queryHandler.HandleAsync(new GetCitySummariesQuery(), cancellationToken);

		return HandleResult(result);
	}
}
