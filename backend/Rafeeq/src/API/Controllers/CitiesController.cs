using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Cities;
using Application.DTOs.Common;
using Application.Queries.Cities;
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
}
