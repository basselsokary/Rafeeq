using Application.DTOs.Common;
using Application.DTOs.Sponsors;

namespace Application.Common.Interfaces.QueryServices;

public interface ISponsorQueryService
{
    Task<SponsorDetailDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<SponsorListDto>> GetAsync(
        SponsorFilters filters,
        PagingParameters paging,
        CancellationToken cancellationToken = default);

    Task<List<NearbySponsorDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        SponsorFilters filters,
        double radiusKm = 3,
        int count = 10,
        CancellationToken cancellationToken = default);

    Task<PagedResult<SponsorListDto>> SearchAsync(
        string searchTerm,
        SponsorFilters filters,
        PagingParameters paging,
        CancellationToken cancellationToken = default);
    
    Task<List<SponsorOfferDto>> GetOffersAsync(
        Guid sponsorId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);
    
    Task<SponsorOfferDto?> GetOfferByIdAsync(
        Guid offerId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all offers (from all sponsors)
    /// </summary>
    Task<PagedResult<SponsorOfferListDto>> GetAllOffersAsync(
        SponsorFilters filters,
        PagingParameters paging,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);
}
