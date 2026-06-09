using API.Controllers.Base;
using Application.Commands.Users.Tourists;
using Application.Commands.Users.Tourists.Favorites;
using Application.Commands.Users.Tourists.Visited;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.Tourists;
using Application.Queries.Users.Tourists;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = Policies.TouristOnly)]
public class AccountController : ApiBaseController
{
    [HttpGet("profile")]
    public async Task<ActionResult<TouristProfileDto>> GetProfile(
        [FromServices] IQueryHandler<GetProfileQuery, TouristProfileDto> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await queryHandler.HandleAsync(new GetProfileQuery(), cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("favorites")]
    public async Task<ActionResult<PagedResult<FavoriteSiteDto>>> GetFavorites(
        [FromServices] IQueryHandler<GetFavoritesQuery, PagedResult<FavoriteSiteDto>> queryHandler,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFavoritesQuery(new PagingParameters(page, pageSize));
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("visited")]
    public async Task<ActionResult<PagedResult<VisitedSiteDto>>> GetVisitedSites(
        [FromServices] IQueryHandler<GetVisitedSitesQuery, PagedResult<VisitedSiteDto>> queryHandler,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetVisitedSitesQuery(new PagingParameters(page, pageSize));
        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile(
        [FromBody] UpdateProfileCommand command,
        [FromServices] ICommandHandler<UpdateProfileCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("favorites")]
    public async Task<ActionResult> AddFavorite(
        [FromBody] AddFavoriteCommand command,
        [FromServices] ICommandHandler<AddFavoriteCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("favorites")]
    public async Task<ActionResult> RemoveFavorite(
        [FromBody] RemoveFavoriteCommand command,
        [FromServices] ICommandHandler<RemoveFavoriteCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("visited")]
    public async Task<ActionResult> MarkAsVisited(
        [FromBody] MarkAsVisitedCommand command,
        [FromServices] ICommandHandler<MarkAsVisitedCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("rate")]
    public async Task<ActionResult> RateSite(
        [FromBody] RateSiteCommand command,
        [FromServices] ICommandHandler<RateSiteCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("rate")]
    public async Task<ActionResult> RemoveRating(
        [FromBody] RemoveRatingCommand command,
        [FromServices] ICommandHandler<RemoveRatingCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("account")]
    public async Task<ActionResult> DeleteAccount(
        [FromServices] ICommandHandler<DeleteAccountCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(new DeleteAccountCommand(), cancellationToken);

        return HandleResult(result);
    }
}
