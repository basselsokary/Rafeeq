// using API.Controllers.Base;
// using Application.Commands.ContentReports;
// using Application.Common.Interfaces.Messaging;
// using Application.DTOs.Common;
// using Application.DTOs.ContentReports;
// using Application.Queries.ContentReports;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;

// namespace API.Controllers;

// [Route("api/[controller]")]
// [Authorize]
// public class ContentReportsController : ApiBaseController
// {
// 	[HttpGet("{id:guid}")]
// 	public async Task<ActionResult<ContentReportDetailDto>> GetById(
// 		[FromRoute] Guid id,
// 		[FromServices] IQueryHandler<GetContentReportByIdQuery, ContentReportDetailDto> queryHandler,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var result = await queryHandler.HandleAsync(new GetContentReportByIdQuery(id), cancellationToken);

// 		return HandleResult(result);
// 	}

// 	[HttpGet]
// 	public async Task<ActionResult<PagedResult<ContentReportListDto>>> GetAllForTourist(
// 		[FromServices] IQueryHandler<GetContentReportsForTouristQuery, PagedResult<ContentReportListDto>> queryHandler,
// 		[FromQuery] int page = 1,
// 		[FromQuery] int pageSize = 20,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var paging = new PagingParameters(page, pageSize);
// 		var query = new GetContentReportsForTouristQuery(paging);
// 		var result = await queryHandler.HandleAsync(query, cancellationToken);

// 		return HandleResult(result);
// 	}

// 	[HttpPost]
// 	public async Task<IActionResult> Report(
// 		[FromBody] ReportContentCommand command,
// 		[FromServices] ICommandHandler<ReportContentCommand> commandHandler,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var result = await commandHandler.HandleAsync(command, cancellationToken);

// 		return HandleResult(result);
// 	}
// }
