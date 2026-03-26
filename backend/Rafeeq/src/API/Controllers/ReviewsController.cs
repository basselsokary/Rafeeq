using API.Controllers.Base;
using Application.Commands.Reviews;
using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Application.Queries.Reviews;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ApiBaseController
{
	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetReviewByIdQuery, ReviewDetailDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetReviewByIdQuery(id));

		return HandleResult(result);
	}

	[HttpGet("site/{siteId:guid}")]
	[AllowAnonymous]
	public async Task<IActionResult> GetBySiteId(
		[FromRoute] Guid siteId,
		[FromQuery] ReviewStatus? status,
		[FromQuery] ReviewOrderBy? sortBy,
		[FromQuery] int? rating,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] IQueryHandler<GetReviewsBySiteIdQuery, PagedResult<ReviewListDto>> queryHandler)
	{
		var paging = new PagingParameters(page, pageSize);

		var query = new GetReviewsBySiteIdQuery(
            siteId,
            paging,
			rating,
            status ?? ReviewStatus.Approved,
            sortBy ?? ReviewOrderBy.Helpful);

		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

	[HttpGet("me")]
	public async Task<IActionResult> GetMyReviews(
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] IQueryHandler<GetReviewsByUserIdQuery, PagedResult<TouristReviewDto>> queryHandler)
	{
		var paging = new PagingParameters(page, pageSize);

		var result = await queryHandler.HandleAsync(new GetReviewsByUserIdQuery(paging));

		return HandleResult(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create(
		[FromBody] CreateReviewCommand command,
		[FromServices] ICommandHandler<CreateReviewCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	public record UpdateReviewRequest(
		Rating Rating,
		string Title,
		string Content);

	[HttpPut("{id:guid}")]
	public async Task<IActionResult> Update(
		[FromRoute] Guid id,
		[FromBody] UpdateReviewRequest request,
		[FromServices] ICommandHandler<UpdateReviewCommand> commandHandler)
	{
		var command = new UpdateReviewCommand(id, request.Rating, request.Title, request.Content);
		var result = await commandHandler.HandleAsync(command);

		return HandleResult(result);
	}

	public record MarkHelpfulRequest(bool IsHelpful);

	[HttpPut("{id:guid}/helpful")]
	public async Task<IActionResult> MarkHelpful(
		[FromRoute] Guid id,
		[FromBody] MarkHelpfulRequest request,
		[FromServices] ICommandHandler<HelpfulReviewCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new HelpfulReviewCommand(id, request.IsHelpful));

		return HandleResult(result);
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(
		[FromRoute] Guid id,
		[FromServices] ICommandHandler<DeleteReviewCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new DeleteReviewCommand(id));

		return HandleResult(result);
	}
}
