using API.Controllers.Base;
using Application.Commands.Sponsors.Offers;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;
using Application.Queries.Sponsors;
using Application.Queries.Sponsors.Offers;
using Domain.Common.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class SponsorsController : ApiBaseController
{
	[HttpGet]
	public async Task<ActionResult<PagedResult<SponsorListDto>>> GetAll(
		[FromQuery] string? searchTerm,
		[FromQuery] SponsorType? type,
		[FromQuery] SponsorTier? tier,
		[FromQuery] bool? activeOnly,
		[FromServices] IQueryHandler<GetSponsorsQuery, PagedResult<SponsorListDto>> queryHandler,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20,
		CancellationToken cancellationToken = default)
	{
		var filters = new SponsorFilters(type, tier, activeOnly);
		var paging = new PagingParameters(page, pageSize);

		var query = new GetSponsorsQuery(filters, paging, searchTerm);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<SponsorDetailDto>> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetSponsorByIdQuery, SponsorDetailDto> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var result = await queryHandler.HandleAsync(new GetSponsorByIdQuery(id), cancellationToken);

		return HandleResult(result);
	}

	[HttpGet("nearby")]
	[Authorize(Roles = UserRoles.Tourist)]
	public async Task<ActionResult<List<NearbySponsorDto>>> GetNearby(
		[FromQuery] double latitude,
		[FromQuery] double longitude,
		[FromQuery] int radiusKm,
		[FromQuery] SponsorType? type,
		[FromQuery] SponsorTier? tier,
		[FromQuery] bool? activeOnly,
		[FromServices] IQueryHandler<GetNearbySponsorsQuery, List<NearbySponsorDto>> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var filters = new SponsorFilters(type, tier, activeOnly);
		var query = new GetNearbySponsorsQuery(latitude, longitude, filters, RadiusKm: radiusKm);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

	[HttpGet("{id:guid}/offers")]
	public async Task<ActionResult<List<SponsorOfferDto>>> GetOffersBySponsor(
		[FromRoute] Guid id,
		[FromQuery] bool activeOnly,
		[FromServices] IQueryHandler<GetSponsorOffersByIdQuery, List<SponsorOfferDto>> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var result = await queryHandler.HandleAsync(new GetSponsorOffersByIdQuery(id, activeOnly), cancellationToken);

		return HandleResult(result);
	}

	[HttpGet("offers")]
	public async Task<ActionResult<PagedResult<SponsorOfferListDto>>> GetAllOffers(
		[FromQuery] SponsorType? type,
		[FromQuery] SponsorTier? tier,
		[FromQuery] bool? sponsorActiveOnly,
		[FromQuery] bool activeOnly,
		[FromServices] IQueryHandler<GetAllSponsorsOffersQuery, PagedResult<SponsorOfferListDto>> queryHandler,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20,
		CancellationToken cancellationToken = default)
	{
		var filters = new SponsorFilters(type, tier, sponsorActiveOnly);
		var paging = new PagingParameters(page, pageSize);

		var query = new GetAllSponsorsOffersQuery(filters, paging, activeOnly);
		var result = await queryHandler.HandleAsync(query, cancellationToken);

		return HandleResult(result);
	}

	[HttpGet("offers/{offerId:guid}")]
	public async Task<ActionResult<SponsorOfferDto>> GetOfferById(
		[FromRoute] Guid offerId,
		[FromServices] IQueryHandler<GetSponsorOfferByIdQuery, SponsorOfferDto> queryHandler,
		CancellationToken cancellationToken = default)
	{
		var result = await queryHandler.HandleAsync(new GetSponsorOfferByIdQuery(offerId), cancellationToken);

		return HandleResult(result);
	}

	[HttpPut("{id:guid}/offers/{offerId:guid}/redeem")]
	[Authorize(Roles = UserRoles.Tourist)]
	public async Task<IActionResult> RedeemOffer(
		[FromRoute] Guid id,
		[FromRoute] Guid offerId,
		[FromServices] ICommandHandler<RedeemSponsorOfferCommand> commandHandler,
		CancellationToken cancellationToken = default)
	{
		var result = await commandHandler.HandleAsync(new RedeemSponsorOfferCommand(id, offerId), cancellationToken);

		return HandleResult(result);
	}
}
