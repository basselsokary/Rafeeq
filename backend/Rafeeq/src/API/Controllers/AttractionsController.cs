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
	public async Task<ActionResult<AttractionDetailDto>> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetAttractionByIdQuery, AttractionDetailDto> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var result = await queryHandler.HandleAsync(new GetAttractionByIdQuery(id), cancellationToken);

		return HandleResult(result);
	}

	[HttpGet("site/{siteId:guid}")]
	public async Task<ActionResult<List<AttractionListDto>>> GetAttractionsBySiteId(
		[FromRoute] Guid siteId,
		[FromQuery] AttractionType? type,
		[FromServices] IQueryHandler<GetAttractionsBySiteIdQuery, List<AttractionListDto>> queryHandler,
		[FromQuery] string? searchTerm = null,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20,
		CancellationToken cancellationToken = default)
	{
		var paging = new PagingParameters(page, pageSize);
		var query = new GetAttractionsBySiteIdQuery(siteId, type, searchTerm);

		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}
}
