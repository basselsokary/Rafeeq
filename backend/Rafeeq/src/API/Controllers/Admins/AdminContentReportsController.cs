// using API.Controllers.Base;
// using Application.Commands.ContentReports;
// using Application.Common.Interfaces.Messaging;
// using Application.DTOs.Common;
// using Application.DTOs.ContentReports;
// using Application.Queries.ContentReports;
// using Domain.Common.Constants;
// using Domain.Enums;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;

// namespace API.Controllers.Admins;

// [Route("api/admins/content-reports/")]
// [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Moderator}")]
// public class AdminContentReportsController : ApiBaseController
// {
//     [HttpGet]
// 	public async Task<ActionResult<PagedResult<ContentReportListDto>>> Get(
//         [FromQuery] int? priority,
//         [FromQuery] ReportStatus? status,
//         [FromQuery] ReportReason? reason,
// 		[FromServices] IQueryHandler<GetContentReportsByHighPriorityQuery, PagedResult<ContentReportListDto>> queryHandler,
// 		[FromQuery] int page = 1,
// 		[FromQuery] int pageSize = 20,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var paging = new PagingParameters(page, pageSize);

// 		var query = new GetContentReportsByHighPriorityQuery(paging, priority, status, reason);
// 		var result = await queryHandler.HandleAsync(query, cancellationToken);

// 		return HandleResult(result);
// 	}

//     [HttpPatch("{id:guid}/escalate")]
// 	public async Task<IActionResult> Escalate(
// 		[FromRoute] Guid id,
// 		[FromServices] ICommandHandler<EscalateContentReportCommand> commandHandler,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var result = await commandHandler.HandleAsync(new EscalateContentReportCommand(id), cancellationToken);

// 		return HandleResult(result);
// 	}

// 	[HttpPatch("{id:guid}/under-review")]
// 	public async Task<IActionResult> MarkUnderReview(
// 		[FromRoute] Guid id,
// 		[FromServices] ICommandHandler<UnderReviewContentReportCommand> commandHandler,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var result = await commandHandler.HandleAsync(new UnderReviewContentReportCommand(id), cancellationToken);

// 		return HandleResult(result);
// 	}

// 	public record ResolveContentReportRequest(
// 		string Reason,
// 		ModerationAction Action);

// 	[HttpPatch("{id:guid}/resolve")]
// 	public async Task<IActionResult> Resolve(
// 		[FromRoute] Guid id,
// 		[FromBody] ResolveContentReportRequest request,
// 		[FromServices] ICommandHandler<ResolveContentReportCommand> commandHandler,
// 		CancellationToken cancellationToken = default)
// 	{
// 		var command = new ResolveContentReportCommand(id, request.Action, request.Reason);
// 		var result = await commandHandler.HandleAsync(command, cancellationToken);

// 		return HandleResult(result);
// 	}
// }
