using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Application.Queries.Attractions;
using Domain.Enums;
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

	[HttpGet("site/{siteId:guid}")]
	public async Task<IActionResult> GetAttractionsForSite(
		[FromRoute] Guid siteId,
		[FromQuery] AttractionType type,
		[FromQuery] int pageNumber,
		[FromQuery] int page,
		[FromServices] IQueryHandler<GetAttractionsByTypeQuery, PagedResult<AttractionListDto>> queryHandler)
	{
		var paging = new PagingParameters(pageNumber, page);

		var result = await queryHandler.HandleAsync(new GetAttractionsByTypeQuery(siteId, type, paging));

		return HandleResult(result);
	}
}
