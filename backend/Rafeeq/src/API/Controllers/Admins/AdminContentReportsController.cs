using API.Controllers.Base;
using Application.Commands.ContentReports;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Application.Queries.ContentReports;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admins;

[Route("api/admin/content-reports/")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminContentReportsController : ApiBaseController
{
    [HttpGet]
	public async Task<ActionResult<PagedResult<ContentReportListDto>>> Get(
        [FromQuery] int? priority,
        [FromQuery] ReportStatus? status,
        [FromQuery] ReportReason? reason,
		[FromServices] IQueryHandler<GetContentReportsByHighPriorityQuery, PagedResult<ContentReportListDto>> queryHandler,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20)
	{
		var paging = new PagingParameters(page, pageSize);

		var query = new GetContentReportsByHighPriorityQuery(paging, priority, status, reason);
        var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

    [HttpPut("{id:guid}/escalate")]
	public async Task<IActionResult> Escalate(
		[FromRoute] Guid id,
		[FromServices] ICommandHandler<EscalateContentReportCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new EscalateContentReportCommand(id));

		return HandleResult(result);
	}

	[HttpPut("{id:guid}/under-review")]
	public async Task<IActionResult> MarkUnderReview(
		[FromRoute] Guid id,
		[FromServices] ICommandHandler<UnderReviewContentReportCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new UnderReviewContentReportCommand(id));

		return HandleResult(result);
	}

	public record ResolveContentReportRequest(
		string? Reason,
		ModerationAction? Action,
		string? Notes = null);

	[HttpPut("{id:guid}/resolve")]
	public async Task<IActionResult> Resolve(
		[FromRoute] Guid id,
		[FromBody] ResolveContentReportRequest request,
		[FromServices] ICommandHandler<ResolveContentReportCommand> commandHandler)
	{
		var command = new ResolveContentReportCommand(id, request.Reason, request.Action, request.Notes);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}
}
