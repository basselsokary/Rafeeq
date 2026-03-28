using API.Controllers.Base;
using Application.Commands.ContentReports;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.ContentReports;
using Application.Queries.ContentReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ContentReportsController : ApiBaseController
{
	[HttpGet("{id:guid}")]
	public async Task<ActionResult<ContentReportDetailDto>> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetContentReportByIdQuery, ContentReportDetailDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetContentReportByIdQuery(id));

		return HandleResult(result);
	}
	
	[HttpPost]
	public async Task<IActionResult> Report(
		[FromBody] ReportContentCommand command,
		[FromServices] ICommandHandler<ReportContentCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}
}
