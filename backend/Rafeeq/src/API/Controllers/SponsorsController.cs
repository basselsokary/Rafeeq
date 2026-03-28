using API.Controllers.Base;
using Application.Commands.Sponsors.Offers;
using Application.Common.Interfaces.Messaging;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;
using Application.Queries.Sponsors;
using Application.Queries.Sponsors.Offers;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class SponsorsController : ApiBaseController
{
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> GetAll(
		[FromQuery] string? searchTerm,
		[FromQuery] SponsorType? type,
		[FromQuery] SponsorTier? tier,
		[FromQuery] bool? activeOnly,
		[FromServices] IQueryHandler<GetSponsorsQuery, PagedResult<SponsorListDto>> queryHandler,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20)
	{
		var filters = new SponsorFilters(type, tier, activeOnly);
		var paging = new PagingParameters(page, pageSize);

		var query = new GetSponsorsQuery(filters, paging, searchTerm);
		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

	[HttpGet("{id:guid}")]
	[AllowAnonymous]
	public async Task<IActionResult> GetById(
		[FromRoute] Guid id,
		[FromServices] IQueryHandler<GetSponsorByIdQuery, SponsorDetailDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetSponsorByIdQuery(id));

		return HandleResult(result);
	}

	[HttpGet("nearby")]
	public async Task<IActionResult> GetNearby(
		[FromQuery] double latitude,
		[FromQuery] double longitude,
		[FromQuery] int radiusKm,
		[FromQuery] SponsorType? type,
		[FromQuery] SponsorTier? tier,
		[FromQuery] bool? activeOnly,
		[FromServices] IQueryHandler<GetNearbySponsorsQuery, List<NearbySponsorDto>> queryHandler)
	{
		var filters = new SponsorFilters(type, tier, activeOnly);
		var query = new GetNearbySponsorsQuery(latitude, longitude, filters, RadiusKm: radiusKm);
		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

	[HttpGet("{id:guid}/offers")]
	public async Task<IActionResult> GetOffersBySponsor(
		[FromRoute] Guid id,
		[FromQuery] bool activeOnly,
		[FromServices] IQueryHandler<GetSiteOffersQuery, List<SponsorOfferDto>> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetSiteOffersQuery(id, activeOnly));

		return HandleResult(result);
	}

	[HttpGet("offers")]
	public async Task<IActionResult> GetAllOffers(
		[FromQuery] SponsorType? type,
		[FromQuery] SponsorTier? tier,
		[FromQuery] bool? sponsorActiveOnly,
		[FromQuery] bool activeOnly,
		[FromServices] IQueryHandler<GetAllSiteOffersAsync, PagedResult<SponsorOfferListDto>> queryHandler,
		[FromQuery] int page = 1,
		[FromQuery] int pageSize = 20)
	{
		var filters = new SponsorFilters(type, tier, sponsorActiveOnly);
		var paging = new PagingParameters(page, pageSize);

		var query = new GetAllSiteOffersAsync(filters, paging, activeOnly);
		var result = await queryHandler.HandleAsync(query);

		return HandleResult(result);
	}

	[HttpGet("offers/{offerId:guid}")]
	public async Task<IActionResult> GetOfferById(
		[FromRoute] Guid offerId,
		[FromServices] IQueryHandler<GetSiteOfferByIdAsync, SponsorOfferDto> queryHandler)
	{
		var result = await queryHandler.HandleAsync(new GetSiteOfferByIdAsync(offerId));

		return HandleResult(result);
	}

	[HttpPut("{id:guid}/offers/{offerId:guid}/redeem")]
	public async Task<IActionResult> RedeemOffer(
		[FromRoute] Guid id,
		[FromRoute] Guid offerId,
		[FromServices] ICommandHandler<RedeemSponsorOfferCommand> commandHandler)
	{
		var result = await commandHandler.HandleAsync(new RedeemSponsorOfferCommand(id, offerId));

		return HandleResult(result);
	}
}
