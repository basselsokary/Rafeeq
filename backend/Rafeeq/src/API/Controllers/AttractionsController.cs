using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Application.Queries.Attractions;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Shared;

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
	public async Task<ActionResult<PagedResult<AttractionListDto>>> GetAttractionsBySiteId(
		[FromRoute] Guid siteId,
		[FromQuery] AttractionType? type,
		[FromServices] IQueryHandler<GetAttractionsBySiteIdQuery, List<AttractionListDto>> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var query = new GetAttractionsBySiteIdQuery(siteId, type);
		var result = await queryHandler.HandleAsync(query, cancellationToken);
		var pagesResult = new PagedResult<AttractionListDto>(result.Value, result.Value.Count, 1, result.Value.Count);

		return HandleResult(result.To(pagesResult));
	}
}
