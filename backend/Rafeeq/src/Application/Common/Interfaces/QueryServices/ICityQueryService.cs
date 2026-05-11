using Application.DTOs.Admins;
using Application.DTOs.Cities;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface ICityQueryService
{
    Task<CityDetailDto?> GetByIdAsync(
        Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);

    Task<CityAdminDetailDto?> GetByIdForAdminAsync(
        Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
    
    Task<List<CitySummaryDto>> GetSummariesAsync(
        LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
    
    Task<List<CityListDto>> GetAsync(
        LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
    
    Task<List<CityLocalizedContentDto>> GetLocalizedContentsAsync(
        Guid cityId, CancellationToken cancellationToken = default);
    
    Task<CityLocalizedContentDto?> GetLocalizedContentByIdAsync(
        Guid cityId, Guid contentId, CancellationToken cancellationToken);
    
    Task<AdminCityDashboardDto> GetDashboardAsync(CancellationToken cancellationToken);
}
