using Application.DTOs.Admins;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface ISponsorQueryService
{
    Task<SponsorDetailDto?> GetByIdAsync(
        Guid id,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    
    Task<AdminSponsorDetailDto?> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<SponsorListDto>> GetAsync(
        SponsorFilters filters,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<List<NearbySponsorDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        SponsorFilters filters,
        double radiusKm = 3,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    
    Task<List<SponsorMapMarkerDto>> GetNearbyMarkerAsync(
        double latitude,
        double longitude,
        int radiusKm = 20,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<PagedResult<SponsorListDto>> SearchAsync(
        string searchTerm,
        SponsorFilters filters,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    
    Task<List<SponsorOfferDto>> GetOffersAsync(
        Guid sponsorId,
        bool activeOnly = true,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<SponsorOfferDto?> GetOfferByIdAsync(
        Guid offerId,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<List<ImageDto>> GetImagesAsync(
        Guid sponsorId,
        CancellationToken cancellationToken = default);

    Task<ImageDto?> GetImageByIdAsync(
        Guid sponsorId,
        Guid imageId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all offers (from all sponsors)
    /// </summary>
    Task<PagedResult<SponsorOfferListDto>> GetAllOffersAsync(
        SponsorFilters filters,
        PagingParameters paging,
        bool activeOnly = true,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    
    Task<List<SponsorOfferSummaryDto>> GetActiveOffersAsync(
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    
    Task<AdminOfferLocalizedContentDto?> GetOfferLocalizedContentByIdAsync(
        Guid offerId,
        Guid contentId,
        CancellationToken cancellationToken = default);

    Task<List<AdminOfferLocalizedContentDto>> GetOfferLocalizedContentsAsync(
        Guid offerId,
        CancellationToken cancellationToken = default);

    Task<List<AdminSponsorLocalizedContentDto>> GetLocalizedContentsAsync(
        Guid sponsorId,
        CancellationToken cancellationToken = default);

    Task<AdminSponsorLocalizedContentDto?> GetLocalizedContentByIdAsync(
        Guid sponsorId,
        Guid contentId,
        CancellationToken cancellationToken = default);
    
    Task<AdminSponsorDashboardDto> GetDashboardAsync(CancellationToken cancellationToken);
}
