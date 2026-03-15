using API.Controllers.Base;
using Application.Commands.ContentReports;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Application.Queries.ContentReports;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class ContentReportsController : ApiBaseController
{
	#region Queries
	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetContentReportByIdQuery, ContentReportDetailDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetContentReportByIdQuery(id));

		return HandleResult(result);
	}

	[HttpGet]
	[Authorize(Roles = nameof(UserRole.Admin))]
	public async Task<IActionResult> GetByPriority(
		[FromQuery] int priority,
		[FromQuery] ReportStatus? status,
		[FromQuery] ReportReason? reason,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] IQueryHandler<GetContentReportsByHighPriorityQuery, PagedResult<ContentReportListDto>> queryHandler)
	{
		var paging = new PagingParameters(page, pageSize);

		var query = new GetContentReportsByHighPriorityQuery(priority, paging, status, reason);
		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}
	#endregion

	#region Commands
	[HttpPost]
	public async Task<IActionResult> Report(
		[FromBody] ReportContentCommand command,
		[FromServices] ICommandHandler<ReportContentCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(command);

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
		string? Notes);

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
	#endregion
}
